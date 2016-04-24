using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{

	public Text maxPlayers;
	public Text playerName;


	// Use this for initialization
	void Start ()
	{
	}

	public void startNetworkGame ()
	{
		PlayerPrefs.SetString (PlayerManager.PLAYER_NAME, playerName.text);
		PlayerPrefs.SetInt (PlayerManager.MAX_PLAYERS, int.Parse (maxPlayers.text));
		Debug.Log ("Network Game started Name: " + playerName.text + " max players " + maxPlayers.text);
		Application.LoadLevel ("Room");
	}
}
