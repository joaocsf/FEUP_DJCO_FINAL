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

        private IMovementListener listener;

        public void SetListener(IMovementListener listener){
            this.listener = listener;
        }
        public abstract HashSet<GridObject> Move(Vector3 input);

        protected void Animate(Vector3 input){
            AnimationController[] controllers = GetComponents<AnimationController>();

            _animating = 0;

            gridManager.ClearObject(obj);
            foreach(AnimationController controller in controllers){
                if(!controller.animateOnMovement) continue;
                _animating ++;
                controller.Animate(input, () => FinishAnimation(input));
            }
        }
        
        public void FinishAnimation(Vector3 input) {
            if(--_animating == 0){
                OnFinishAnimation(input);
                gridManager.RegisterObject(obj);
            }

            if(listener != null)
                listener.OnFinishedMovement();
        }

        protected abstract void OnFinishAnimation(Vector3 input);
	}

}