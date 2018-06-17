using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Search_Shell.Game;

namespace Search_Shell.Game.Controll{
	public class PlayEndScene : MonoBehaviour, IControllEvents {
		public Animator animator;

		public Transform pivot;
		public Transform kite;

		private GameObject cam = null;
		private Vector3 cameraLookPos;

		public float waitTime = 5f;

		public float look_speed = 0.2f;
		public float transform_speed = 0.5f;
		public string endString;

    public void OnControll()
    {
			if(animator != null)
				animator.SetBool("play", true);
			GameController controller = FindObjectOfType<GameController>();
			controller.canControll = false;
			controller.DisableHighlights();
			CameraFollow camF = FindObjectOfType<CameraFollow>();
			camF.enabled = false;
			cam = camF.gameObject;
			cameraLookPos = cam.transform.position + cam.transform.forward;
			StartCoroutine(OpenUI());
		}

		void Update()
		{
			if(cam != null){
				cam.transform.position = Vector3.Lerp(cam.transform.position, pivot.position, Time.deltaTime * transform_speed);
				cameraLookPos = Vector3.Lerp(cameraLookPos, kite.transform.position, look_speed * Time.deltaTime);
				cam.transform.LookAt(cameraLookPos);
			}
		}


		IEnumerator OpenUI(){
			UIManager UIManager = FindObjectOfType<UIManager>();
			UIManager.DisableUI();
			UIManager.end = true;
			yield return new WaitForSeconds(waitTime);
			UIManager.ActivateEnding(true, endString);
		}
	}
}
