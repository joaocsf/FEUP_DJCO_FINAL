using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Search_Shell.Grid;
using Search_Shell.Helper;
using Search_Shell.Controllers;
using Search_Shell.Controllers.Movement;
using System;

namespace Search_Shell.Controllers.Animation {

    [RequireComponent(typeof(MovementController))]
	public abstract class AnimationController : BaseController{
        
        private Action finishAction;
        protected bool animating = false;

        protected abstract void OnAnimate(Vector3 input);
        protected abstract void OnUpdate(float delta);

        public void Animate(Vector3 input, Action finish){
            finishAction = finish;
            animating = true;
            OnAnimate(input);
        }


        void Update() {
            if(animating)
                OnUpdate(Time.deltaTime);
        }


        public void Finish(){
            animating = false;
           finishAction();
        }

  }

}