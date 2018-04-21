using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Search_Shell.Controllers;

namespace Search_Shell.Game.Controll{
	public class ChangeLevel : BaseController, IControllEvents{
    public void OnControll()
    {
			gridManager.LoadNextLevel();
    }
	}
}