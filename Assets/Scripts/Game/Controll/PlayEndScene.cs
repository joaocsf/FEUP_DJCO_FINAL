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
		public string endString;

    public void OnControll()
    {
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
				cam.transform.position = Vector3.Lerp(cam.transform.position, pivot.position, Time.deltaTime*0.5f);
				cameraLookPos = Vector3.Lerp(cameraLookPos, kite.transform.position, 0.2f* Time.deltaTime);
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
