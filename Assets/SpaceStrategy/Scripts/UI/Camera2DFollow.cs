using System;
using UnityEngine;
using System.Collections.Generic;

namespace SpaceStrategy
{
	public class Camera2DFollow : MonoBehaviour
	{
		public GameObject target;
		bool targetAquired = false;
		public float damping = 0.05f;
		public float lookAheadFactor = 0f;
		public float lookAheadReturnSpeed = 0.5f;
		public float lookAheadMoveThreshold = 0.1f;
		public float z = -10;

		private float m_OffsetZ;
		private Vector3 m_LastTargetPosition;
		private Vector3 m_CurrentVelocity;
		private Vector3 m_LookAheadPos;

		public string[] targetTags;

		// Use this for initialization
		private void Start ()
		{
			aquireTarget ();
			if (target != null) {
				m_LastTargetPosition = target.transform.position;
				m_OffsetZ = (transform.position - target.transform.position).z;
				transform.parent = null;
			}
		}


		// Update is called once per frame
		private void Update ()
		{
			aquireTarget ();

			if (target != null) {
				// only update lookahead pos if accelerating or changed direction
				float xMoveDelta = (target.transform.position - m_LastTargetPosition).x;

				bool updateLookAheadTarget = Mathf.Abs (xMoveDelta) > lookAheadMoveThreshold;

				if (updateLookAheadTarget) {
					m_LookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign (xMoveDelta);
				} else {
					m_LookAheadPos = Vector3.MoveTowards (m_LookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
				}

				Vector3 aheadTargetPos = target.transform.position + m_LookAheadPos + Vector3.forward * m_OffsetZ;
				Vector3 newPos = Vector3.SmoothDamp (transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);

				transform.position = new Vector3 (newPos.x, newPos.y, z);

				m_LastTargetPosition = target.transform.position;
			}
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
				} else {
					targetAquired = false;
					target = null;
				}

			} 
		}

		void targetDestroyed ()
		{
			Debug.Log ("targetDestroyed");
			targetAquired = false;
			target = null;
		}
	}

}