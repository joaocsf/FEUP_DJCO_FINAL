﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Search_Shell.Controllers.Animation {
	public class JumpAnimation : AnimationController {

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
			middlePos =  (lastPos + startPos)/2;
			middlePos.y = Mathf.Max(lastPos.y, startPos.y) + yOffset;
			time = duration;
    }

		public Vector3 LerpPositions(Vector3 start, Vector3 middle, Vector3 end, float t){
			Vector3 p1 = Vector3.Lerp(start, middle, t);
			Vector3 p2 = Vector3.Lerp(middle, end, t);
			return Vector3.Lerp(p1,p2,t);
		}
    protected override void OnUpdate(float delta)
    {
			time -= delta;
			float t = curve.Evaluate( 1f - time/duration );
			
			obj.transform.localPosition = LerpPositions(startPos, middlePos, lastPos, t);

			if(time <= 0)	
				Finish();
    }
	}
}