using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;


public class SpaceShipController : Photon.MonoBehaviour
{

	public GameObject myCamera;

	private PlayerDetails _pd;
	public float _boundry = 400f;
	public float maxSpeed = 100f;
	public GameObject explosion;
	public List<GameObject> bullet;
	public List<GameObject> shotPosition;
	public float shotDelay = 0.0f;
	public float clickDelay = 1f;
	public ParticleSystem jetFlare;
	public int startingLife = 5;
	public AudioClip weaponFire;

	public TextMesh playerNameText;
	public string playerName;

	private bool thrusting = false;
	private float lastShotTime = 0f;

	private float lastButtonDownTime = 0f;
	private bool buttonDown = false;

	public static Action<int> shipHit;
	public static Action<int> shipReset;

	public Action<int> destroyed;
	public Action<int> hit;


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
		if (photonView.isMine && tag != "Enemy") {
			Debug.Log ("I am active");
			myCamera.SetActive (true);
			playerName = PlayerPrefs.GetString (PlayerManager.PLAYER_NAME);
			photonView.RPC ("SetupShip", PhotonTargets.All, photonView.ownerId, playerName);

		} else {
			Debug.Log ("Other player is active id: " + photonView.ownerId);
		}

		if (tag == "Enemy") {
			_rb.freezeRotation = true;
		}

		if (photonView.isMine) {
			ResetPlayer ();
		}


	}

	public void setupShip (string playerName)
	{
		photonView.RPC ("SetupShip", PhotonTargets.All, photonView.ownerId, playerName);
	}

	[PunRPC]
	public void SetupShip (int playerID, string playerName)
	{
		if (playerID == photonView.ownerId) {
			Debug.Log ("Setting up player " + playerName);
			playerName = playerName;
			playerNameText.text = playerName;
		}
	}

	[PunRPC]
	public void FireWeapon (int playerID)
	{
		if (playerID == photonView.ownerId) {
			if (Time.time > lastShotTime + shotDelay) {
				lastShotTime = Time.time;
				for (int i = 0; i < shotPosition.Count; i++) {
					GameObject shotClone = Instantiate (bullet [i], shotPosition [i].transform.position, shotPosition [i].transform.rotation) as GameObject;
					shotClone.transform.SetParent (transform.parent);
				}
				SWSoundManager.instance.PlaySingle (weaponFire);
			}
		}
	}
		
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (photonView.isMine && tag != "Enemy") {
			if (Input.GetMouseButton (0)) {
				

				if (!buttonDown) {
					buttonDown = true;
					lastButtonDownTime = Time.time;
				}

				AimShip (Input.mousePosition, true);

			} else {
				_pd.Move = false;
			}

			if (Input.GetMouseButtonUp (0)) {
				buttonDown = false;
				if (Time.time - lastButtonDownTime < clickDelay) {
					photonView.RPC ("FireWeapon", PhotonTargets.All, photonView.ownerId);
				}
			}

			MoveShip ();

		}
	}

	[PunRPC]
	public void StartThrottle (int playerID)
	{
		if (playerID == photonView.ownerId) {
			jetFlare.Play ();
		}
	}

	[PunRPC]
	public void StopThrottle (int playerID)
	{
		if (playerID == photonView.ownerId) {
			jetFlare.Stop ();
		}
	}

	public void AimShip (Vector3 targetPosition, bool isMouse)
	{
		if (_pd != null) {
			if (!thrusting) {
				thrusting = true;
				photonView.RPC ("StartThrottle", PhotonTargets.All, photonView.ownerId);
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

			if (_pd.TargetPosition.y > transform.position.y) {
				direction = -1f;
			} else {
				direction = 1f;
			}	
		}
	}

	public void MoveShip ()
	{
		if (_pd != null) {
			if (_pd.Move && _pd.TargetPosition != transform.position) {

				_pd.CurrentPosition = transform.position;

				Vector3 rotateDirection = transform.position - _pd.TargetPosition;
				Vector3 moveDirection = _pd.TargetPosition - transform.position;

				var newRotation = Quaternion.LookRotation (rotateDirection, Vector3.right * direction);
				newRotation.x = 0.0f;
				newRotation.y = 0.0f;
				transform.rotation = Quaternion.Slerp (transform.rotation, newRotation, _pd.RotateSpeed * Time.deltaTime);

				_rb.AddForce (moveDirection * Time.deltaTime * _pd.MoveSpeed);
			} else {
				if (thrusting) {
					thrusting = false;
					photonView.RPC ("StopThrottle", PhotonTargets.All, photonView.ownerId);
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
			Debug.Log ("damage applied");
			if (shipHit != null && tag != "Enemy") {
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
		if (destroyed != null) {
			destroyed (photonView.ownerId);
		}
		if (photonView.isMine) {	
			// dissapear player and make explosion at player location
			GameObject explosionClone = PhotonNetwork.Instantiate (explosion.name, transform.position, transform.rotation, 0) as GameObject;
			explosionClone.transform.position = pd.CurrentPosition;
			explosionClone.GetComponent<ParticleSystem> ().Play ();
			gameObject.SetActive (false);

			if (tag != "Enemy") {
				ResetPlayer ();
			} else {
				photonView.RPC ("DestroyNPC", PhotonTargets.All, photonView.ownerId);
			}
		}
	}

	[PunRPC]
	public void DestroyNPC (int playerID)
	{
		if (playerID == photonView.ownerId) {
			Debug.Log ("DestroyNPC");
			Destroy (gameObject);
		}
	}

	public void ResetPlayer ()
	{
		if (photonView.isMine) {
			gameObject.SetActive (true);

			float rX = UnityEngine.Random.Range (-_boundry + 20f, _boundry - 20f);
			float rY = UnityEngine.Random.Range (-_boundry + 20f, _boundry - 20f);

			_pd = new PlayerDetails ();

			_pd.Alive = true;

			Vector3 randomPosition = new Vector3 (rX, rY, _pd.Depth);
			_pd.CurrentPosition = randomPosition;
			_pd.TargetPosition = randomPosition;
			transform.position = randomPosition;

			transform.localScale = new Vector3 (_pd.Size, _pd.Size, 1);
			Camera.main.orthographicSize = _pd.Size * 30.0f;

			_pd.Radius = gameObject.GetComponent<SpriteRenderer> ().bounds.extents.x;

			_pd.Died = IDied;

			_pd.Life = startingLife;

			if (shipReset != null && tag != "Enemy") {
				shipReset (_pd.Life);
			}

			Debug.Log ("reset player");
		}
	}
}

