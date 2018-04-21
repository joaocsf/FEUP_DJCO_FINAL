using System.Collections;
using System.Collections.Generic;
using Search_Shell.Grid;
using UnityEngine;

namespace Search_Shell.Controllers.Movement {
	public class JumpController : MovementController {

		public bool CheckCollision(Vector3 movement, out HashSet<GridObject> intersect){
			List<Vector3> volumes = obj.CalculateSlide(movement);
			intersect = gridManager.CheckCollision(obj, volumes);

			return intersect.Count != 0;
		}

    public override HashSet<GridObject> Move(Vector3 input, ref HashSet<GridObject> mayfall)
    {
			mayfall.UnionWith(gridManager.CheckCollision(obj,obj.CalculateSlide(Vector3.up)));
			HashSet<GridObject> check = new HashSet<GridObject>();
			if(!CheckCollision(input, out check)){
				FinishMovement(input);
			}else if(!CheckCollision(Vector3.up, out check)){
				if(!CheckCollision(input + Vector3.up, out check)){
					FinishMovement(input+Vector3.up);
				}
			}

			return check;
    }

		private void FinishMovement(Vector3 endMovement){
			Animate(endMovement);
		}

    protected override void OnFinishAnimation(Vector3 input)
    {
			obj.Slide(input);
    }
	}
}