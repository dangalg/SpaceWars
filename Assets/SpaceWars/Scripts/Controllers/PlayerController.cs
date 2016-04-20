using UnityEngine;
using System.Collections;

public class PlayerController : Photon.MonoBehaviour
{
	private PlayerDetails _pd;
	private float _boundry;

	public GameObject explosion;

	[SerializeField] private float distanceFromCamera = 10.0f;

	public static PlayerController instance = null;

	float direction = -1f;

	Rigidbody2D _rb;

	public PlayerDetails PD {
		get {
			return _pd;
		}
	}

	//Awake is always called before any Start functions
	void Awake ()
	{
		//Check if instance already exists
		if (instance == null)

			//if not, set instance to this
			instance = this;

		//If instance already exists and it's not this:
		else if (instance != this)

			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
			Destroy (gameObject);    

		//Sets this to not be destroyed when reloading scene
		DontDestroyOnLoad (gameObject);

		//Call the InitGame function to initialize the first level 
		InitGame ();
	}

	//Initializes the game for each level.
	void InitGame ()
	{
		
	}

	// Use this for initialization
	void Start ()
	{
		_rb = GetComponent<Rigidbody2D> ();
//		_boundry = TheOthersController.instance.boundry;
		ResetPlayer ();
//		StartCoroutine (growPlayerOverTime ());
	}


	void movePlayer (PlayerDetails pd)
	{
//		transform.position = Vector2.Lerp (transform.position, _pd.TargetPosition, _pd.MoveSpeed * Time.deltaTime);
	}

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButton (0)) {

			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit2D hit = Physics2D.GetRayIntersection (ray, Mathf.Infinity);

			if (hit.collider != null && hit.collider.transform == transform) {
				// raycast hit this gameobject
				Debug.LogError ("clicked me");
				_pd.Move = true;
			} else {
				_pd.Move = true;
				_pd.TargetPosition = Input.mousePosition;
				_pd.TargetPosition = new Vector3 (_pd.TargetPosition.x, _pd.TargetPosition.y, (distanceFromCamera + _pd.TargetPosition.z));
				_pd.TargetPosition = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x,
					Input.mousePosition.y, _pd.TargetPosition.z));

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

		} else {
			_pd.Move = false;
		}

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
			
			_rb.velocity = _rb.velocity * 0.99f;
			_rb.angularVelocity = _rb.angularVelocity * 0.99f;
		}


	}

	void OnCollisionEnter2D (Collision2D coll)
	{
//		if (coll.gameObject.tag == "Enemy") {
		coll.gameObject.SendMessage ("ApplyDamage", 1);
		ApplyDamage (1);
//		}
	}

	void ApplyDamage (int hitPower)
	{
		_pd.Life -= hitPower;
	}

	void Heal (int hitPower)
	{
		_pd.Life += hitPower;
	}

	void IDied (PlayerDetails pd)
	{
		// dissapear player and make explosion at player location
		gameObject.SetActive (false);
//		GameObject explosionClone = Instantiate (explosion) as GameObject;
//		explosionClone.transform.position = pd.CurrentPosition;
//		explosionClone.GetComponent<ParticleSystem> ().Play ();

		ResetPlayer ();
	}

	public void ResetPlayer ()
	{
		gameObject.SetActive (true);

		float rX = UnityEngine.Random.Range (-_boundry + 20f, _boundry - 20f);
		float rY = UnityEngine.Random.Range (-_boundry + 20f, _boundry - 20f);

		_pd = new PlayerDetails ();
		_pd.Alive = true;

		Vector3 randomPosition = Vector3.one;//new Vector3 (rX, rY, _pd.Depth);
		_pd.CurrentPosition = randomPosition;
		_pd.TargetPosition = randomPosition;
		transform.position = randomPosition;

		transform.localScale = new Vector3 (_pd.Size, _pd.Size, 1);
		Camera.main.orthographicSize = _pd.Size * 30.0f;

		_pd.Radius = gameObject.GetComponent<SpriteRenderer> ().bounds.extents.x;

		_pd.Died = IDied;

	}
		
}

