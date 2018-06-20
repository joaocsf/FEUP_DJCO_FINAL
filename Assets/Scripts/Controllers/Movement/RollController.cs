using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Search_Shell.Grid;
using Search_Shell.Helper;

namespace Search_Shell.Controllers.Movement {
	public class RollController : MovementController {

		public bool debug = false;
				
		Drawer drawer = new Drawer();

		float f = 0;
		void OnDrawGizmos(){
			drawer.Draw();
		}

		private void DebugRotation(Vector3 input){

			drawer.Clear();	
			Vector3 pos = obj.finalPosition;
			Vector3 rot = obj.finalAngles;
			List<Vector3> vols = obj.CalculateRoll(input, f);
			obj.Roll(input,f, ref pos, ref rot);
			transform.localPosition = pos;
			transform.localEulerAngles = rot;

			f-=22.5f;
			if(f < -90f) f = 0;
			if(f > 90f) f = 0;
			foreach(Vector3 v3 in vols)
				drawer.AddDrawable(new DrawableCube(transform.parent.position + v3, Vector3.one, Color.yellow, true));

		}

		private HashSet<GridObject> CheckRotation(Vector3 input){
			HashSet<GridObject> obstacles = new HashSet<GridObject>();

			for(int i = 0; i <= 4; i++){
				List<Vector3> vols = obj.CalculateRoll(input, -90f*(i/4f));
				HashSet<GridObject> temp = gridManager.CheckCollision(obj, vols);
				obstacles.UnionWith(temp);
			}

			return obstacles;
		}

    public override HashSet<GridObject> Move(Vector3 input, ref HashSet<GridObject> mayfall)
    {
			HashSet<GridObject> obstacles = new HashSet<GridObject>();
			if(debug){
				DebugRotation(input);
				return obstacles;
			}else{
				
				obstacles = gridManager.CheckCollision(obj, obj.CalculateSlide(Vector3.up));
				if(obstacles.Count > 0)
					return obstacles;

				obstacles = CheckRotation(input);					

				if(obstacles.Count == 0){
					Animate(input);
				}
			}
			return obstacles;
    }

    protected override bool OnFinishAnimation(Vector3 input)
    {
			obj.Roll(input,-90f);
			return true;
    }
  }

}