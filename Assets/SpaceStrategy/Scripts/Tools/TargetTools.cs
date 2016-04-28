using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SpaceStrategy
{
	public static class TargetTools
	{

		public static GameObject aquireRandomTargetByTagsWithoutMe (GameObject me, string[] tags)
		{
			GameObject target = null;
			List<GameObject> targets = new List<GameObject> ();
			foreach (string tag in tags) {
				List<GameObject> tagTargets = new List<GameObject> (GameObject.FindGameObjectsWithTag (tag));

				foreach (GameObject item in tagTargets) {
					targets.Add (item);
				}
			}

			List<GameObject> finalListWithoutMe = new List<GameObject> ();

			foreach (GameObject item in targets) {
				if (item != me) {
					finalListWithoutMe.Add (item);
				}
			}

			if (finalListWithoutMe.Count > 0) {
				//					Debug.Log ("finalListWithoutMe: " + finalListWithoutMe.Count);

				int rIndex = UnityEngine.Random.Range (0, finalListWithoutMe.Count);
				target = finalListWithoutMe [rIndex];
				return target;
			} 
			
			return null;
		}

	}
}