using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace SpaceStrategy
{
	public class SpaceShipController : MonoBehaviour, ITarget
	{
		
		private PlayerDetails _pd;
		public string playerName;
		public float _boundry = 400f;
		public float maxSpeed = 100f;
		public float rotationSpeed = 5f;
		public GameObject explosion;
		public List<GameObject> bullet;
		public List<GameObject> shotPosition;
		public float shotDelay = 0.0f;
		public float clickDelay = 1f;
		public ParticleSystem jetFlare;
		public int startingLife = 5;
		public AudioClip weaponFire;

		public TextMesh playerNameText;

		private bool thrusting = false;
		private float lastShotTime = 0f;

		private float lastButtonDownTime = 0f;
		private bool buttonDown = false;

		public static Action<int> shipHit;
		public static Action<int> shipReset;

		public Action _targetDestroyed;
		public Action<int> hit;
		public Action targeted;

		public Action targetDestroyed {
			get {
				return _targetDestroyed;
			}
			set {
				_targetDestroyed = value;
			}
		}

		[SerializeField] private float distanceFromCamera = 10.0f;

		float direction = -1f;

		Rigidbody2D _rb;

		public PlayerDetails PD {
			get {
				return _pd;
			}
		}

		// Use this for initialization
		void Start ()
		{
			_rb = GetComponent<Rigidbody2D> ();
			ResetPlayer ();

		}

		public void FireWeapon ()
		{
			if (Time.time > lastShotTime + shotDelay) {
				lastShotTime = Time.time;
				for (int i = 0; i < shotPosition.Count; i++) {
					GameObject shotClone = Instantiate (bullet [i], shotPosition [i].transform.position, shotPosition [i].transform.rotation) as GameObject;
					shotClone.transform.SetParent (transform.parent);
				}
				SoundManager.instance.PlaySingle (weaponFire);
			}
		}


		public void Targeted ()
		{
			if (targeted != null) {
				targeted ();
			}
		}


		public void StartThrottle ()
		{
			
			jetFlare.Play ();

		}


		public void StopThrottle ()
		{
			
			jetFlare.Stop ();

		}

		public void AimShip (Vector3 targetPosition, bool isMouse)
		{
//			if (_pd != null) {
			if (!thrusting) {
				thrusting = true;
			}
			_pd.Move = true;
			_pd.TargetPosition = targetPosition;
			_pd.TargetPosition = new Vector3 (_pd.TargetPosition.x, _pd.TargetPosition.y, (distanceFromCamera + _pd.TargetPosition.z));
			if (isMouse) {
				_pd.TargetPosition = Camera.main.ScreenToWorldPoint (new Vector3 (targetPosition.x, targetPosition.y, _pd.TargetPosition.z));
			}
			// set boundries
			float _x = Mathf.Clamp (_pd.TargetPosition.x, -_boundry, _boundry);
			float _y = Mathf.Clamp (_pd.TargetPosition.y, -_boundry, _boundry);
			_pd.TargetPosition = new Vector3 (_x, _y, _pd.TargetPosition.z);


			Vector3 vectorToTarget = _pd.TargetPosition - transform.position;
			float angle = Mathf.Atan2 (vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
			Quaternion q = Quaternion.AngleAxis (angle, Vector3.forward);
			transform.rotation = Quaternion.Slerp (transform.rotation, q, Time.deltaTime * rotationSpeed);

//			}
		}

		public void MoveShip ()
		{
			if (_pd != null) {
				if (_pd.Move && _pd.TargetPosition != transform.position) {

					_pd.CurrentPosition = transform.position;

//					Vector3 moveDirection = _pd.TargetPosition - transform.position;

					_rb.AddForce (transform.right * Time.deltaTime * _pd.MoveSpeed);
				} else {
					if (thrusting) {
						thrusting = false;
					}
					_rb.velocity = _rb.velocity * 0.99f;
					_rb.angularVelocity = _rb.angularVelocity * 0.5f;
				}

				if (_rb.velocity.magnitude > maxSpeed) {
					_rb.velocity = _rb.velocity.normalized * maxSpeed;
				}
			}
		}

		void ApplyDamage (int hitPower)
		{
			if (_pd != null && _pd.Alive) {
				if (shipHit != null) {
					shipHit (hitPower);
				}

				_pd.Life -= hitPower;
				if (_pd.Life > 0 && hit != null) {
					hit (hitPower);
				}
			}
		}

		void Heal (int hitPower)
		{
			_pd.Life += hitPower;
		}


		void IDied (PlayerDetails pd)
		{
			if (_targetDestroyed != null) {
				_targetDestroyed ();
			}
			// dissapear player and make explosion at player location
			GameObject explosionClone = Instantiate (explosion, transform.position, transform.rotation) as GameObject;
			explosionClone.transform.position = pd.CurrentPosition;
			explosionClone.GetComponent<ParticleSystem> ().Play ();
			gameObject.SetActive (false);


			Destroy (gameObject);


		}

		public void ResetPlayer ()
		{
			
			gameObject.SetActive (true);

//			float rX = UnityEngine.Random.Range (-_boundry + 20f, _boundry - 20f);
//			float rY = UnityEngine.Random.Range (-_boundry + 20f, _boundry - 20f);

			_pd = new PlayerDetails ();

			_pd.Alive = true;

//			Vector3 randomPosition = new Vector3 (rX, rY, _pd.Depth);
			_pd.CurrentPosition = transform.position;
			_pd.TargetPosition = transform.position;
//			transform.position = randomPosition;

			transform.localScale = new Vector3 (_pd.Size, _pd.Size, 1);
			Camera.main.orthographicSize = _pd.Size * 30.0f;

			_pd.Radius = gameObject.GetComponent<SpriteRenderer> ().bounds.extents.x;

			_pd.Died = IDied;

			_pd.Life = startingLife;

			if (shipReset != null && tag != "Enemy") {
				shipReset (_pd.Life);
			}
		}

	}
}
