using UnityEngine;
using System.Collections;

public class SWOnJoinedInstantiate : MonoBehaviour
{
	public static int playerWhoIsIt = 0;
	private static PhotonView ScenePhotonView;
	public Transform SpawnPosition;
	public float PositionOffset = 2.0f;
	public GameObject[] PrefabsToInstantiate;
	public GameObject asteroid;
	public float boundry = 400f;
	bool alive = true;

	// set in inspector

	// Use this for initialization
	public void Start ()
	{
		ScenePhotonView = this.GetComponent<PhotonView> ();
	}

	public void OnJoinedRoom ()
	{

		GameObject[] targets = GameObject.FindGameObjectsWithTag ("Enemy");
		foreach (var item in targets) {
			PhotonNetwork.Destroy (item.GetPhotonView ());
		}


		createSpaceShip (null, 0);

		if (PhotonNetwork.isMasterClient == false) {
			return;
		}

		Debug.Log ("I am master");

		StartCoroutine (createAsteroids ());
		StartCoroutine (createEnemies ());

	}

	public void OnPhotonPlayerConnected (PhotonPlayer player)
	{
		Debug.Log ("OnPhotonPlayerConnected: " + player);

		// when new players join, we send "who's it" to let them know
		// only one player will do this: the "master"

		if (PhotonNetwork.isMasterClient) {
			TagPlayer (playerWhoIsIt);
		} else {
			
		}
	}

	public void OnPhotonPlayerDisconnected (PhotonPlayer player)
	{
		Debug.Log ("OnPhotonPlayerDisconnected: " + player.ID);
		TheOthersManager.instance.RemoveOtherPLayer (player.ID);

		if (PhotonNetwork.isMasterClient) {
			if (player.ID == playerWhoIsIt) {
				// if the player who left was "it", the "master" is the new "it"
				TagPlayer (PhotonNetwork.player.ID);

			}
		}
	}

	public static void TagPlayer (int playerID)
	{
		Debug.Log ("TagPlayer: " + playerID);
		ScenePhotonView.RPC ("TaggedPlayer", PhotonTargets.All, playerID);
	}

	[PunRPC]
	public void TaggedPlayer (int playerID)
	{
		playerWhoIsIt = playerID;
		Debug.Log ("TaggedPlayer: " + playerID);
	}

	public void OnMasterClientSwitched ()
	{
		Debug.Log ("OnMasterClientSwitched");
	}

	void OnDestroy ()
	{
		alive = false;
	}

	void createAsteroid ()
	{
		float rX = UnityEngine.Random.Range (-boundry + 20f, boundry - 20f);
		float rY = UnityEngine.Random.Range (-boundry + 20f, boundry - 20f);

		Vector3 randomPosition = new Vector3 (rX, rY, transform.position.z);

		GameObject asteroidClone = PhotonNetwork.InstantiateSceneObject (asteroid.name, randomPosition, Quaternion.identity, 0, null) as GameObject;

		asteroidClone.GetComponent<AsteroidController> ().died += createAsteroid;
	}

	void createSpaceShip (string tag, int id)
	{
		
		if (this.PrefabsToInstantiate != null) {
			foreach (GameObject o in this.PrefabsToInstantiate) {
				Debug.Log ("Instantiating: " + o.name);
				Vector3 spawnPos = Vector3.up;
				if (this.SpawnPosition != null) {
					spawnPos = this.SpawnPosition.position;
				}
				Vector3 random = Random.insideUnitSphere;
				random.y = 0;
				random = random.normalized;
				Vector3 itempos = spawnPos + this.PositionOffset * random;

				if (tag != null) {
					Debug.Log ("create enemy");
					GameObject spaceShipClone = PhotonNetwork.InstantiateSceneObject (o.name, itempos, Quaternion.identity, 0, null) as GameObject;
					spaceShipClone.tag = tag;
					spaceShipClone.GetComponent<PhotonView> ().ownerId = id;
					spaceShipClone.GetComponent<EnemyController> ().enabled = true;
					TheOthersManager.instance.AddOtherPLayer (id, spaceShipClone);
				} else {
					Debug.Log ("create me");
					GameObject spaceShipClone = PhotonNetwork.Instantiate (o.name, itempos, Quaternion.identity, 0) as GameObject;
				}
			}
		}
	}

	IEnumerator createAsteroids ()
	{
		for (int i = 0; i < 100; i++) {
			createAsteroid ();
			yield return null;
		}
	}

	IEnumerator createEnemies ()
	{
		for (int i = 0; i < 10; i++) {
			createSpaceShip ("Enemy", i + 10000);
			yield return new WaitForSeconds (2f);
		}
	}
}
