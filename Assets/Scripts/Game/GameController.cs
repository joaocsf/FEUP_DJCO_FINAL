using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Search_Shell.Grid;

namespace Search_Shell.Game{

	public class GameController : MonoBehaviour {

		public GridManager level;
		public GridManager subLevel;

		public GridObject levelObject;
		public GridObject subLevelObject;

		public Camera levelCamera;
		public Camera subLevelCamera;

		private Vector3 lastInput;

		void Start () {
			SetupCamera();
		}

		void OnDrawGizmos() {
		}

		void SetupCamera () {

			levelCamera.transform.parent = levelObject.transform;
			subLevelCamera.transform.parent = subLevel.transform;
			subLevelCamera.transform.localPosition = Vector3.zero + Vector3.one;
			subLevelCamera.GetComponent<CameraFollow>().SetTransform(subLevelObject.transform);
		}
		
		void Update () {

			Vector3 input = new Vector3(
				Input.GetAxisRaw("Horizontal"),
				0,
				Input.GetAxisRaw("Vertical"));

			if(input == lastInput) return;
			lastInput = input;
			if(input.magnitude == 0) return;
			input = subLevelCamera.GetComponent<CameraFollow>().GetPlaneDirection(input);
			Debug.DrawLine(subLevelCamera.transform.position, subLevelCamera.transform.position + input, Color.black, 2);
			input = subLevel.transform.InverseTransformDirection(input);
			input.y = 0;
			input = input.normalized;
			Debug.Log(input + "- " + input.magnitude);
			if(Mathf.Abs(input.x) > 0.7f){
				input.x = Mathf.Sign(input.x);
				input.z = 0;
			}else{
				input.z = Mathf.Sign(input.z);
				input.x = 0;
			}

			if((int)input.sqrMagnitude != 0){
				input.x = Mathf.Round(input.x);
				input.y = Mathf.Round(input.y);
				input.z = Mathf.Round(input.z);
			}

			subLevel.SlideObject(subLevelObject, input);


		}
	}
}