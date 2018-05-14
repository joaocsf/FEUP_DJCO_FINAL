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
        public abstract HashSet<GridObject> Move(Vector3 input, ref HashSet<GridObject> mayfall);

        protected void Animate(Vector3 input){
            Debug.Log(input + " - " + input.sqrMagnitude);
            if(Mathf.Approximately(input.sqrMagnitude,0)){
                Debug.Log("Finish Animation");
                FinishAnimation(input);
                return;
            }

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
            if(--_animating > 0) return;
            Debug.Log("Finishing Animation");
            bool update = OnFinishAnimation(input);
            if(!Mathf.Approximately(input.sqrMagnitude, 0))
                gridManager.RegisterObject(obj);

            Debug.Log("Finishing Animation:" + update);
            if(!update) return;

            Debug.Log("Updating Listeners");
            if(listener != null)
                listener.OnFinishedMovement();
        }

        protected abstract bool OnFinishAnimation(Vector3 input);
	}

}