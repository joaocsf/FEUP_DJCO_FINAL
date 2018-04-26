using System.Collections;
using System.Collections.Generic;
using Search_Shell.Grid;
using UnityEngine;

namespace Search_Shell.Controllers.Movement {
	public class PushController : MovementController {

		public bool update = false;
		public override HashSet<GridObject> Move(Vector3 input, ref HashSet<GridObject> mayfall)
		{
			HashSet<GridObject> colliding = new HashSet<GridObject>();
			List<Vector3> volumes = obj.CalculateSlide(input);
			mayfall = gridManager.CheckCollision(obj, volumes);

			List<GridObject> objs = new List<GridObject>(mayfall);
			//Sort in the movement direction
			objs.Sort( (o1, o2) => {
				if(o1.finalPosition.x > o2.finalPosition.x) return 1;
				else if(o1.finalPosition.x < o2.finalPosition.x) return -1;
				return 0;
				});

			update = objs.Count == 0;

			foreach(GridObject obj in objs){
				Debug.Log(obj);
				if(!gridManager.PushObject(obj, input, 
					() => {
						update = true;
						Debug.Log("Update" + update);
						Animate(Vector3.zero);
					})){
					colliding.Add(obj);
				}			
			}

			if(colliding.Count == 0){
				Animate(input);
			}

			return colliding;
		}

		protected override bool OnFinishAnimation(Vector3 input){
			Debug.Log("Update" + update);
			obj.Slide(input);
			return update;
		}
	}
}