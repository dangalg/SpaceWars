using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TheOthersManager : MonoBehaviour
{
	private Dictionary<int, OtherPlayerDetails> otherPlayers;
	public GameObject arrowPrefab;
	public GameObject otherPlayerContainer;
	public GameObject starContainer;
	public GameObject mainPlayer;
	public int mainPlayerID;

	public static TheOthersManager instance = null;

	private bool radarOn = false;

	public bool RadarOn {
		get {
			return radarOn;
		}
		set {
			radarOn = value;
			if (value) {
				foreach (OtherPlayerDetails opd in otherPlayers.Values) {

					opd.Arrow.GetComponent<Tracker> ().On = true;
				}
			} else {
				foreach (OtherPlayerDetails opd in otherPlayers.Values) {

					opd.Arrow.GetComponent<Tracker> ().On = false;
				}
			}
		}
	}

	public struct OtherPlayerDetails
	{
		int otherPlayerID;
		GameObject otherPlayerObject;
		GameObject arrow;

		public int OtherPlayerID {
			get {
				return otherPlayerID;
			}
			set {
				otherPlayerID = value;
			}
		}

		public GameObject OtherPlayerObject {
			get {
				return otherPlayerObject;
			}
			set {
				otherPlayerObject = value;
			}
		}

		public GameObject Arrow {
			get {
				return arrow;
			}
			set {
				arrow = value;
			}
		}

		public OtherPlayerDetails (int otherPlayerID, GameObject otherPlayerObject, GameObject arrow)
		{
			this.otherPlayerID = otherPlayerID;
			this.otherPlayerObject = otherPlayerObject;
			this.arrow = arrow;
		}
	}


	public void AddOtherPLayer (int otherPlayerID, GameObject otherPlayer)
	{
		otherPlayer.transform.SetParent (otherPlayerContainer.transform);

		GameObject arrowClone = Instantiate (arrowPrefab) as GameObject;
		arrowClone.transform.SetParent (otherPlayer.transform);
		arrowClone.GetComponent<Tracker> ().goToTrack = otherPlayer;

		OtherPlayerDetails opd = new OtherPlayerDetails (otherPlayerID, otherPlayer, arrowClone);

		otherPlayers.Add (otherPlayerID, opd);
	}

	public void RemoveOtherPLayer (int otherPlayerId)
	{
		if (otherPlayers.ContainsKey (otherPlayerId)) {
			
			otherPlayers.Remove (otherPlayerId);
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
		otherPlayers = new Dictionary<int, OtherPlayerDetails> ();
	}
		
}

