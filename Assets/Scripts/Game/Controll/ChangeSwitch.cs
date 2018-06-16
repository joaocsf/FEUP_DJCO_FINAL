using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Search_Shell.Game;

namespace Search_Shell.Game.Controll{
	public class ChangeSwitch : MonoBehaviour, IControllEvents {
		public AK.Wwise.State _state;

    public void OnControll()
    {
			FindObjectOfType<GameController>().PlayState(_state);
		}
	}
}
