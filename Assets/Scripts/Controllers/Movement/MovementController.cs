using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Search_Shell.Grid;
using Search_Shell.Controllers.Animation;

namespace Search_Shell.Controllers.Movement {

	public abstract class MovementController : BaseController{

        private int _animating = 0;

        public bool Animating {
            get{ return _animating > 0; }
        }

        public abstract HashSet<GridObject> Move(Vector3 input);

        protected void Animate(Vector3 input){
            AnimationController[] controllers = GetComponents<AnimationController>();

            _animating = controllers.Length;

            gridManager.ClearObject(obj);
            foreach(AnimationController controller in controllers){
                controller.Animate(input, () => FinishAnimation(input));
            }
        }
        
        public void FinishAnimation(Vector3 input) {
            if(--_animating == 0){
                OnFinishAnimation(input);
                gridManager.RegisterObject(obj);
            }
        }

        protected abstract void OnFinishAnimation(Vector3 input);
	}

}