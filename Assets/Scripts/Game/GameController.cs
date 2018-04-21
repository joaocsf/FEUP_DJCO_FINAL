using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Search_Shell.Grid;
using Search_Shell.Controllers;
using Search_Shell.Controllers.Detector;
using Search_Shell.Controllers.Movement;

namespace Search_Shell.Game{

	public class GameController : MonoBehaviour, IMovementListener, IGridEvents {

		public Material highlighted;
		public Material selected;

		public GridManager level;
		public GridManager subLevel;

		public GridObject levelObject;
		public GridObject subLevelObject;

		public Camera levelCamera;
		public Camera subLevelCamera;

		private Vector3 lastInput;

		private HashSet<GridObject> nearObjects = new HashSet<GridObject>();

		private HashSet<GridObject> movedObjects = new HashSet<GridObject>();

		private bool canControll = true;

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
			subLevel.AddListener(this);
			ControllObject(subLevelObject);
		}
		
		public void ControllObject(GridObject obj) {
			if(subLevelObject != null)
				GetHighLighter(subLevelObject).SetSelected(HighLight.None);
			subLevelObject = obj;
			subLevelCamera.GetComponent<CameraFollow>().SetTransform(subLevelObject.transform);
			UpdateReachableObjects();
			GetHighLighter(obj).SetSelected(HighLight.Selected);
		}
		private HighLighter GetHighLighter(GridObject obj){
			HighLighter hl = obj.GetComponent<HighLighter>();
			if(hl == null){
				hl = obj.gameObject.AddComponent<HighLighter>();
				hl.highlighted = highlighted;
				hl.selected = selected;
			}
			return hl;
		}		
		void UpdateMaterial(HighLight highlight){
			foreach(GridObject obj in nearObjects){
				GetHighLighter(obj).SetSelected(highlight);
			}
		}

		void UpdateReachableObjects(){
			UpdateMaterial(HighLight.None);
			DetectorController detect = subLevelObject.GetComponent<DetectorController>();
			nearObjects = detect.NearObjects();
			UpdateMaterial(HighLight.Highlighted);
		}

		void SwitchObject(){
			RaycastHit hit; 
			Ray r = subLevelCamera.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(r, out hit)){
				GridObject obj = hit.collider.GetComponent<GridObject>();
				if(nearObjects.Contains(obj)){
					ControllObject(obj);
				}
			}
		}

		void Update () {
			
			if(!canControll) return;

			if(Input.GetMouseButtonDown(0))
				SwitchObject();

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

			movedObjects.Clear();

			MovementController controller = subLevelObject.GetComponent<MovementController>();
			if(controller == null) return;
			controller.SetListener(this);

			if(controller.Animating) return;

			HashSet<GridObject> colls =  controller.Move(input, ref movedObjects);
			if(colls.Count > 0) return;
			canControll = false;
			movedObjects = subLevel.GetMovingObjects();
			//movedObjects.Add(subLevelObject);

		}

    public void OnFinishedMovement()
    {
			canControll = !subLevel.VerifyGravity(movedObjects);
			if(canControll)
				UpdateReachableObjects();
    }

    public void OnFinishedGravity()
    {
			canControll = true;
			UpdateReachableObjects();
    }
  }
}