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
	float lastFireTime = Time.time;

	// Use this for initialization
	void Start ()
	{
		controller = GetComponent<SpaceShipController> ();

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

			target.GetComponent<SpaceShipController> ().shipDestroyed += targetDestroyed;

		}

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

			Vector3 start = transform.position;
			Vector3 direction = (finalDetected.transform.position - transform.position).normalized;
			float distance = firingRange;

			//draw the ray in the editor
			Debug.DrawRay (start, direction * distance, Color.red);

			//do the ray test
			RaycastHit2D sightTest = Physics2D.Raycast (start, direction, distance);
			if (sightTest.collider != null) {
				if (sightTest.collider.gameObject.name == target.name) {
					return true;
				}
			}
	
		}

		return false;

	}

	// Update is called once per frame
	void FixedUpdate ()
	{
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

