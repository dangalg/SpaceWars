using UnityEngine;
using System.Collections;

public class AsteroidController : MonoBehaviour
{

	public GameObject explosion;
	public System.Action died;

	int life = 1;

	Rigidbody2D _rb;

	void Awake ()
	{
		_rb = GetComponent<Rigidbody2D> ();
	}

	// Use this for initialization
	void Start ()
	{
		float randomSpeed = UnityEngine.Random.Range (2f, 8f);
		int randomScale = UnityEngine.Random.Range (1, 10);
		life = randomScale;
		transform.localScale = Vector3.one * (float)randomScale;
		_rb.mass = (float)randomScale;

		float randomAutoDestroy = UnityEngine.Random.Range (3f, 20f);

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
			if (died != null) {
				died ();
			}
			CancelInvoke ("ApplyDamage");
			GameObject explosionClone = PhotonNetwork.Instantiate (explosion.name, transform.position, transform.rotation, 0) as GameObject;
			explosionClone.transform.position = transform.position;
			explosionClone.GetComponent<ParticleSystem> ().Play ();
			Destroy (explosionClone, 2f);
			Destroy (gameObject);
		}
	}

	void OnDestroy ()
	{
		CancelInvoke ("ApplyDamage");
	}
}
