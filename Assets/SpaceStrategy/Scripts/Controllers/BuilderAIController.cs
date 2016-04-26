using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SpaceStrategy
{
	[RequireComponent (typeof(SpaceShipController))]
	public class BuilderAIController : MonoBehaviour
	{

		SpaceShipController controller;
		public GameObject target;
		public GameObject finalDetected;
		bool targetAquired = false;
		public float firingRange = 50.0f;
		public float firingDelay = 0.5f;
		public float evasionDelay = 0.2f;
		public float escapeAfterHitTime = 0.05f;
		public float dodgeAfterHitAmount = 30f;
		public bool targeted = true;

		float lastFireTime;
		float lastHitTime;
		float lastEvadeTime;

		bool setEvadeValues = false;

		int[] directionChoice;

		public string targetTagName = "SpaceShip";

		// Use this for initialization
		void Start ()
		{
			lastFireTime = Time.time;
			lastHitTime = Time.time;
			lastEvadeTime = Time.time;
			directionChoice = new int[]{ -1, 1 };
			evasionDelay += escapeAfterHitTime;
			controller = GetComponent<SpaceShipController> ();
			controller.hit += hit;
			controller.targeted += startEvasiveManouvers;
			//velocty = (target.position - transform.position).normalized * speed;

		}

		void aquireTarget ()
		{
			if (!targetAquired || target == null || (target != null && !target.GetActive ())) {
				targetAquired = true;
			
				GameObject[] targets = GameObject.FindGameObjectsWithTag (targetTagName);
				Debug.Log ("targets: " + targets.Length);
//			GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");

//			GameObject[] newArray = new GameObject[targets.Length + enemies.Length];
//			System.Array.Copy (targets, newArray, targets.Length);
//			System.Array.Copy (enemies, 0, newArray, targets.Length, enemies.Length);

//			Debug.Log ("newArray: " + newArray.Length);

				List<GameObject> finalListWithoutMe = new List<GameObject> ();

				foreach (GameObject item in targets) {
					if (item != gameObject) {
						finalListWithoutMe.Add (item);
					}
				}

				Debug.Log ("finalListWithoutMe: " + finalListWithoutMe.Count);

				int rIndex = UnityEngine.Random.Range (0, finalListWithoutMe.Count);

				target = finalListWithoutMe [rIndex];

				if (target != null && target.GetActive ()) {
					Debug.Log ("rIndex: " + rIndex);
					Debug.Log ("target: " + target);
					Debug.Log ("finalListWithoutMe.Count: " + finalListWithoutMe.Count);
					SpaceShipController ssc = target.GetComponent<SpaceShipController> ();
					if (ssc != null) {
						ssc.destroyed += targetDestroyed;
					} else {
						targetAquired = true;
						target = null;
					}
				}

			}

		}

		void hit (int hitPower)
		{
			setEvadeValues = false;
			lastEvadeTime = Time.time;
			int r = UnityEngine.Random.Range (0, 1);
			dodgeAfterHitAmount = Mathf.Abs (dodgeAfterHitAmount) * directionChoice [r];
		}

		void startEvasiveManouvers ()
		{
			targeted = true;
		}

		void evade ()
		{
			if (targeted && !setEvadeValues && Time.time > lastEvadeTime + evasionDelay) {
				setEvadeValues = true;
				lastEvadeTime = Time.time;
				int r = UnityEngine.Random.Range (0, 1);
				dodgeAfterHitAmount = Mathf.Abs (dodgeAfterHitAmount) * directionChoice [r];
			}
		}

		bool evasiveManouvers ()
		{
			if (Time.time < lastEvadeTime + escapeAfterHitTime && controller != null) {
				setEvadeValues = false;
				controller.AimShip (new Vector3 ((transform.position.x * -1) + dodgeAfterHitAmount, (transform.position.y * -1) + dodgeAfterHitAmount, transform.position.z), false);
				controller.MoveShip ();
				return true;
			} 

			return false;
		}

		void targetDestroyed ()
		{
			targetAquired = false;
			target = null;
		}

		void moveToTarget ()
		{
			if (target != null) {

				controller.AimShip (target.transform.position, false);
				controller.MoveShip ();
			}
		}

		void OnTriggerStay2D (Collider2D other)
		{
			if (other != null && other.gameObject != null && target != null && other.gameObject.tag == target.tag) {
				if (Time.time > lastFireTime + firingDelay) {
					lastFireTime = Time.time;
					controller.FireWeapon ();
				}
			}
		}

		// Update is called once per frame
		void FixedUpdate ()
		{
			if (!evasiveManouvers ()) {

				aquireTarget ();

				moveToTarget ();

				evade ();
			}
		}

	}
}
