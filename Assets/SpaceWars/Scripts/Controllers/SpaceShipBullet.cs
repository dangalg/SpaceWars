using UnityEngine;

//This script will handle the bullet adding itself back to the pool
public class SpaceShipBullet : MonoBehaviour
{
	public int speed = 10;
	//How fast the bullet moves
	public float lifeTime = 2;
	//How long the bullet lives in seconds
	public int power = 1;
	//Power of the bullet
	public GameObject explosion;


	void OnEnable ()
	{
		//Send the bullet "forward"
		GetComponent<Rigidbody2D> ().velocity = transform.up.normalized * speed;
		//Invoke the Die method
		Invoke ("Die", lifeTime);
	}

	void OnCollisionEnter2D (Collision2D coll)
	{
		if (coll != null) {
			coll.gameObject.SendMessage ("ApplyDamage", 1);
			ApplyDamage (1);
		}
	}

	void ApplyDamage (int hitPower)
	{
		power -= hitPower;
		if (power <= 0) {
			GameObject explosionClone = Instantiate (explosion, transform.position, transform.rotation) as GameObject;
			explosionClone.transform.position = transform.position;
			explosionClone.GetComponent<ParticleSystem> ().Play ();
			CancelInvoke ("Die");
			Die ();

		}
	}

	void OnDisable ()
	{
		//Stop the Die method (in case something else put this bullet back in the pool)
		CancelInvoke ("Die");
	}

	void Die ()
	{
		//Add the bullet back to the pool
		Destroy (gameObject);
	}
}