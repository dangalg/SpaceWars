using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(SpaceShipController))]
public class EnemyController : MonoBehaviour
{

	SpaceShipController controller;
	public GameObject target;
	public GameObject finalDetected;
	bool targetAquired = false;
	public float firingRange = 50.0f;
	public float firingDelay = 0.5f;
	public float escapeAfterHitTime = 0.05f;
	public float dodgeAfterHitAmount = 30f;
	float lastFireTime = Time.time;
	float lastHitTime = Time.time;

	int[] directionChoice = new int[]{ -1, 1 };

	// Use this for initialization
	void Start ()
	{
		controller = GetComponent<SpaceShipController> ();
		controller.hit += hit;
		//velocty = (target.position - transform.position).normalized * speed;

	}

	void aquireTarget ()
	{
		if (!targetAquired || target == null || (target != null && !target.GetActive ())) {
			targetAquired = true;
			
			GameObject[] targets = GameObject.FindGameObjectsWithTag ("SpaceShip");
			GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");

			GameObject[] newArray = new GameObject[targets.Length + enemies.Length];
			System.Array.Copy (targets, newArray, targets.Length);
			System.Array.Copy (enemies, 0, newArray, targets.Length, enemies.Length);

			Debug.Log ("newArray: " + newArray.Length);

			List<GameObject> finalListWithoutMe = new List<GameObject> ();

			foreach (GameObject item in newArray) {
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
		lastHitTime = Time.time;
		int r = UnityEngine.Random.Range (0, 1);
		dodgeAfterHitAmount = dodgeAfterHitAmount * directionChoice [r];
	}

	bool escape ()
	{
		if (Time.time < lastHitTime + escapeAfterHitTime && controller != null && target != null) {
			controller.AimShip (new Vector3 (transform.position.x + dodgeAfterHitAmount, transform.position.y + dodgeAfterHitAmount, transform.position.z), false);
			controller.MoveShip ();
			return true;
		}
		dodgeAfterHitAmount = Mathf.Abs (dodgeAfterHitAmount);
		return false;
	}

	void targetDestroyed (int targetID)
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

	bool isTargetInRange ()
	{
		if (target != null) {

			Vector3 start = finalDetected.transform.position;
			Vector3 direction = (finalDetected.transform.position - transform.position).normalized;
			float distance = firingRange;

			//draw the ray in the editor
			Debug.DrawRay (start, direction * distance, Color.red);

			//do the ray test
			RaycastHit2D sightTest = Physics2D.Raycast (start, direction, distance);
			if (sightTest.collider != null) {
				Debug.Log ("sightTest" + sightTest.collider.gameObject.tag + " target " + target.tag);
				if (sightTest.collider.gameObject.tag == target.tag) {
					return true;
				}
			}
	
		}

		return false;

	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		if (!escape ()) {
			aquireTarget ();

			moveToTarget ();

			if (isTargetInRange ()) {
				if (Time.time > lastFireTime + firingDelay) {
					lastFireTime = Time.time;
					controller.photonView.RPC ("FireWeapon", PhotonTargets.All, controller.photonView.ownerId);
				}
			}
		}
	}




}

