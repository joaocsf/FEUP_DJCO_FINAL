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
	public class RollAnimation : AnimationController{

        public float duration;

        private float time = 0;

        private Vector3 input;

        public AnimationCurve curve;

        private Pivot pivot;

        protected override void OnAnimate(Vector3 input){
            this.animating = true;
            this.input = input;
            this.time = 0;
            pivot = obj.GetRollPivot(input);
        }

        protected override void OnUpdate(float delta) {
            time = Mathf.Clamp(this.time + delta, 0f, duration); 
            float value = Mathf.Clamp01(curve.Evaluate(time/duration)); 
            Matrix4x4 mat = obj.RollMatrix( input, -90 * value );
            Vector3 pos = obj.finalPosition;
            Vector3 rot = obj.finalAngles;
            obj.Roll(input, -90*value, ref pos, ref rot);
            transform.localPosition = pos;
            transform.localEulerAngles = rot;

            if(time == duration)
                Finish();
        }
  }

}