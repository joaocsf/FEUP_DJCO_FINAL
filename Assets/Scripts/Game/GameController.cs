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

		void SetupCamera () {

			levelCamera.transform.parent = levelObject.transform;
			subLevelCamera.transform.parent = subLevel.transform;
			subLevelCamera.transform.localPosition = Vector3.zero + Vector3.one;
		}
		
		void Update () {

			Vector3 input = new Vector3(
				Input.GetAxisRaw("Horizontal"),
				(Input.GetKey(KeyCode.Space)? 1 : (Input.GetKey(KeyCode.LeftShift)? -1 : 0)),
				Input.GetAxisRaw("Vertical"));


			if(input == lastInput) return;
			lastInput = input;
			input = subLevelCamera.transform.TransformDirection(input);
			input = subLevel.transform.InverseTransformDirection(input);
			

			if((int)input.sqrMagnitude != 0){
				input.x = Mathf.Round(input.x);
				input.y = Mathf.Round(input.y);
				input.z = Mathf.Round(input.z);
			}

			subLevel.SlideObject(subLevelObject, input);


		}
	}
}