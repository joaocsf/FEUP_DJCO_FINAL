using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Search_Shell.Grid;
using Search_Shell.Helper;
using Search_Shell.Controllers;
using System;

namespace Search_Shell.Controllers.Detector {

	public class AdjacentObjects : DetectorController {

    private void checkSide(Vector3 dir, ref HashSet<GridObject> objset){
      HashSet<GridObject> objs = (gridManager.CheckCollision(obj, obj.CalculateSlide(dir)));

      foreach(GridObject obj in objs)
        if(obj.properties.canControll)
          objset.Add(obj);
    }

    public override HashSet<GridObject> NearObjects(){
      
      HashSet<GridObject> res = new HashSet<GridObject>();
      checkSide(Vector3.left, ref res);
      checkSide(Vector3.right, ref res);
      checkSide(Vector3.up, ref res);
      checkSide(Vector3.down, ref res);
      checkSide(Vector3.forward, ref res);
      checkSide(Vector3.back, ref res);
      return res;

    }

  }

}