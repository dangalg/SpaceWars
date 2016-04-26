﻿using UnityEngine;
using System.Collections;

namespace SpaceStrategy
{
	public class Tracker : MonoBehaviour
	{
		public GameObject goToTrack;

		private MeshRenderer _renderer;

		bool on = true;

		public bool On {
			get {
				return on;
			}
			set {
				on = value;
			}
		}


		void Start ()
		{
			_renderer = GetComponent<MeshRenderer> ();
		}

		void Update ()
		{
			if (on && goToTrack != null) {
				Vector3 v3Screen = Camera.main.WorldToViewportPoint (goToTrack.transform.position);
				if (v3Screen.x > -0.01f && v3Screen.x < 1.01f && v3Screen.y > -0.01f && v3Screen.y < 1.01f)
					_renderer.enabled = false;
				else {
					_renderer.enabled = true;
					v3Screen.x = Mathf.Clamp (v3Screen.x, 0.05f, 0.95f);
					v3Screen.y = Mathf.Clamp (v3Screen.y, 0.05f, 0.95f);
					transform.position = Camera.main.ViewportToWorldPoint (v3Screen);
				}

				transform.rotation = Quaternion.identity;
			}

		}
	}
}