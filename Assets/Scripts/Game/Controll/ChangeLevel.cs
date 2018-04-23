using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Search_Shell.Controllers;

namespace Search_Shell.Game.Controll{
	public class ChangeLevel : BaseController, IControllEvents{

    public string levelName = "";

    public bool once = false;

    private bool executed = false;

    public void OnControll()
    {
        if(once && executed) return;
        executed = true;

        if(levelName.Trim().Equals(""))
			gridManager.LoadNextLevel();
        else
            gridManager.LoadNextSubLevel(levelName);
    }
	}
}