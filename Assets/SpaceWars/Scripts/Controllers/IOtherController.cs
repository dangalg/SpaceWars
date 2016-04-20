using UnityEngine;
using System.Collections;


public interface IOtherController
{

	PlayerDetails PD {
		get;
	}

	void setPlayerSettings (PlayerDetails PD);
}

