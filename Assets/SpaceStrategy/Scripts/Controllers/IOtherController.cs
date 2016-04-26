using UnityEngine;
using System.Collections;

namespace SpaceStrategy
{
	public interface IOtherController
	{

		PlayerDetails PD {
			get;
		}

		void setPlayerSettings (PlayerDetails PD);
	}
}
