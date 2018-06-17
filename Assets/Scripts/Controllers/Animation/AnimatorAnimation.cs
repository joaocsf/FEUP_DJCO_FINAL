using System.Collections;
using System.Collections.Generic;
using Search_Shell.Grid;
using UnityEngine;

namespace Search_Shell.Controllers.Animation {
	public class AnimatorAnimation : AnimationController {

		public float duration = 0.2f;

		public Animator animator;
		float time;
		bool hasObject;

    protected override void OnAnimate(Vector3 input)
    {
			time = duration;
			animator.SetFloat("speed", 1);
			hasObject = Physics.Raycast(transform.position, input, 1f);	
			falseState = hasObject;
			SetPush(hasObject);
    }

		private void SetPush(bool newValue){
			bool oldValue = animator.GetBool("push");
			Debug.Log(newValue);
			if(oldValue != newValue)
				animator.SetBool("push", newValue);
		}

		private bool falseState = false;
		IEnumerator DisablePush(){
			falseState = false;
			yield return new WaitForSeconds(0.2f);
			SetPush(falseState);
		}

    protected override void OnUpdate(float delta)
    {
			time -= delta;

			if(time <= 0){	
				animator.SetFloat("speed", 0);
				StartCoroutine(DisablePush());
				Finish();
			}
    }
	}
}