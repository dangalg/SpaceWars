using UnityEngine;
using System.Collections;

namespace SpaceStrategy
{
	public class AsteroidController : MonoBehaviour, ITarget
	{

		public GameObject explosion;
		private System.Action _targetDestroyed;

		public System.Action targetDestroyed {
			get {
				return _targetDestroyed;
			}
			set {
				_targetDestroyed = value;
			}
		}

		int life = 1;

		Rigidbody2D _rb;

		void Awake ()
		{
			_rb = GetComponent<Rigidbody2D> ();
		}

		// Use this for initialization
		void Start ()
		{
//			float randomSpeed = UnityEngine.Random.Range (2f, 8f);
//			int randomScale = UnityEngine.Random.Range (1, 10);
//			life = randomScale;
//			transform.localScale = Vector3.one * (float)randomScale;
//			_rb.mass = (float)randomScale;
//
//			float randomAutoDestroy = UnityEngine.Random.Range (3f, 20f);

			//Uncomment this to move asteroids
//		float rX = UnityEngine.Random.Range (20f, -20f);
//		float rY = UnityEngine.Random.Range (-20f, -20f);
//		GetComponent<Rigidbody2D> ().velocity = new Vector2 (rX, rY) * randomSpeed;

		}

		void OnCollisionEnter2D (Collision2D coll)
		{
			if (coll.transform != transform.parent) {
				coll.gameObject.SendMessage ("ApplyDamage", 1);
				ApplyDamage (1);
			}
		}

		void ApplyDamage (int hitPower)
		{
			life -= hitPower;
			if (life <= 0) {
				
				CancelInvoke ("ApplyDamage");
				GameObject explosionClone = Instantiate (explosion, transform.position, transform.rotation) as GameObject;
				explosionClone.transform.position = transform.position;
				explosionClone.GetComponent<ParticleSystem> ().Play ();
				Destroy (explosionClone, 2f);
				Destroy (gameObject);
			}
		}

		void OnDestroy ()
		{
			if (_targetDestroyed != null) {
				_targetDestroyed ();

			}
			CancelInvoke ("ApplyDamage");
		}
	}
}