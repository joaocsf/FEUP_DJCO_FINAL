using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Search_Shell.Controllers.Animation {
	public class LinearAnimation : AnimationController {

		public float duration = 1;

		public float yOffset = 0;
		public AnimationCurve curve;

		private float time = 0;

		private Vector3 lastPos;

		private Vector3 middlePos;
		private Vector3 startPos;
    protected override void OnAnimate(Vector3 input)
    {
			lastPos = obj.finalPosition + input;
			startPos = obj.finalPosition;
			time = duration;
    }

    protected override void OnUpdate(float delta)
    {
			time -= delta;
			float t = curve.Evaluate( 1f - time/duration );
			
			obj.transform.localPosition = Vector3.Lerp(startPos, lastPos, t);

			if(time <= 0)	
				Finish();
    }
	}
}