using System.Collections;
using System.Collections.Generic;
using Search_Shell.Grid;
using UnityEngine;

namespace Search_Shell.Controllers.Animation {
	public class LookAnimation : AnimationController {

		public float duration = 0.2f;

		
		public AnimationCurve curve;

		public Transform obj;

		public float offSet = 0;
		float finalRotation;
		float initialRotation;

		float time;

    protected override void OnAnimate(Vector3 input)
    {
			time = duration;

			finalRotation = input.x > 0? 90f : input.x < 0 ? -90f : input.z > 0 ? 0f : 180f;
			finalRotation += offSet;
			Debug.Log("Look Animation" + finalRotation);
			initialRotation = obj.localEulerAngles.y;

    }

    protected override void OnUpdate(float delta)
    {
			time -= delta;
			float t = curve.Evaluate( 1f - time/duration );
			Debug.Log(t);	
			float finalY = Mathf.LerpAngle(initialRotation, finalRotation, t);
			
			Debug.Log(finalY);	
			obj.localEulerAngles = new Vector3(0,finalY,0);

			if(time <= 0){	
				Finish();
			}
    }
	}
}