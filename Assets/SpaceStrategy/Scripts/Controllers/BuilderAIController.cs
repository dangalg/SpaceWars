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

		float lastFireTime;
		float lastHitTime;
		float lastEvadeTime;

		Vector3 moveToPosition;

		public bool evade = false;
		float rEvadeMoveMent = 0f;
		bool setEvadeDirection = false;

		public bool dock = false;

		int[] directionChoice;

		public string[] targetTags;

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
			moveToPosition = transform.position;

		}

		void aquireTarget ()
		{
			if (!targetAquired || target == null || (target != null && !target.GetActive ())) {
				targetAquired = true;

				target = TargetTools.aquireRandomTargetByTagsWithoutMe (gameObject, targetTags);

				if (target != null && target.GetActive ()) {
					ITarget itarget = target.GetComponent<ITarget> ();
					if (itarget != null) {
						itarget.targetDestroyed += targetDestroyed;

					} else {

						targetAquired = false;
						target = null;
					}
					moveToPosition = target.transform.position;
				}
			}



		}

		void hit (int hitPower)
		{
			evade = true;
			setEvadeDirection = false;
			lastEvadeTime = Time.time;
		}

		void startEvasiveManouvers ()
		{
			evade = true;
		}

		void evasiveManouvers ()
		{
			if (Time.time < lastEvadeTime + evasionDelay) {
				if (!setEvadeDirection) {
					setEvadeDirection = true;
					lastEvadeTime = Time.time;
					rEvadeMoveMent = UnityEngine.Random.Range (-dodgeAfterHitAmount, dodgeAfterHitAmount);
					Vector3 fixedTargetPosition = new Vector3 ((transform.transform.position.x * -1) + rEvadeMoveMent, (transform.transform.position.y * -1) + rEvadeMoveMent, transform.position.z); 
					moveToPosition = fixedTargetPosition;
				}
			} else {
				setEvadeDirection = false;
				evade = false;
				targetAquired = false;
				target = null;
				dock = false;
			}

		}

		void targetDestroyed ()
		{
			targetAquired = false;
			target = null;
			evade = false;
			dock = false;
		}

		void moveToTarget ()
		{
			if (target != null) {
				
				controller.AimShip (moveToPosition, false);
				controller.MoveShip ();
			}
		}

		void OnTriggerStay2D (Collider2D other)
		{
			target = other.gameObject;
			if (!other.name.Contains ("Star")) {
				setEvadeDirection = false;
				lastEvadeTime = Time.time;
				dock = false;
				evade = true;
			} else {
				dock = true;
			}

		}

		void OnTriggerExit2D (Collider2D other)
		{
			targetAquired = false;
			target = null;
			dock = false;

		}

		// Update is called once per frame
		void FixedUpdate ()
		{
			if (dock) {
				return;
			}

			if (evade) {
				evasiveManouvers ();
			} else if (!targetAquired) {
				aquireTarget ();
			} else if (target != null) {
				moveToPosition = target.transform.position;
			}

			moveToTarget ();

		}

	}
}
