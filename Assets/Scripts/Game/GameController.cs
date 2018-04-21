using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Search_Shell.Grid;
using Search_Shell.Controllers;
using Search_Shell.Controllers.Detector;
using Search_Shell.Controllers.Movement;
using Search_Shell.Game.Controll;
using System;

namespace Search_Shell.Game{

	public class GameController : MonoBehaviour, IMovementListener, IGridEvents {

		public Material highlighted;
		public Material selected;

		public GridManager level;

		public string levelLayer = "Level";
		public string subLevelLayer = "SubLevel";
		public float defaultCameraZoom = 5;
		public AnimationCurve cameraCurve;
		
		public float transitionTime = 1;
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
			/* 
			levelCamera.transform.parent = levelObject.transform;
			subLevelCamera.transform.parent = subLevel.transform;
			subLevelCamera.transform.localPosition = Vector3.zero + Vector3.one;
			subLevelCamera.GetComponent<CameraFollow>().SetTransform(subLevelObject.transform);
			subLevelCamera.GetComponent<CameraFollow>().radius = defaultCameraZoom;
			*/
			subLevel.AddListener(this);
			SetLevel(level, levelObject);
			SetSubLevel(subLevel, subLevelObject);
			ControllObject(subLevelObject);
		}

		public void SetLevel(GridManager level, GridObject levelObject){
			this.level = level;
			this.levelObject = levelObject;
			if(levelObject != null)
				levelCamera.transform.parent = levelObject.transform;

			SetLayer(subLevel.transform,
				obj => {
					if(obj.GetComponent<Renderer>()){
						obj.gameObject.layer = LayerMask.NameToLayer(levelLayer);
					}
				});
		}

		private void SetLayer(Transform transform, Action<Transform> action){
			foreach(Transform t in transform){
				action(t);
				SetLayer(t, action);
			}
		}

		public void SetSubLevel(GridManager subLevel, GridObject sublevelObject){
			if(this.subLevel != null)
				this.subLevel.RemoveListener(this);

			this.subLevel = subLevel;
			this.subLevelObject = sublevelObject;
			subLevelCamera.transform.parent = this.subLevel.transform;
			levelCamera.GetComponent<ScreenCapture>().scale = subLevel.scale;
			subLevelCamera.GetComponent<SkyboxHandler>().SetOpacity(0);
			subLevel.AddListener(this);
			ControllObject(subLevelObject);
			SetLayer(subLevel.transform,
				obj => {
					if(obj.GetComponent<Renderer>()){
						obj.gameObject.layer = LayerMask.NameToLayer(subLevelLayer);
					}
				});
		}
		
		public void ControllObject(GridObject obj) {
			if(subLevelObject != null)
				GetHighLighter(subLevelObject).SetSelected(HighLight.None);
			subLevelObject = obj;
			subLevelCamera.GetComponent<CameraFollow>().SetTransform(subLevelObject.transform);
			UpdateReachableObjects();
			GetHighLighter(obj).SetSelected(HighLight.Selected);
			IControllEvents[] events = subLevelObject.GetComponents<IControllEvents>();
			foreach(IControllEvents listener in events){
				listener.OnControll();
			}
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

    public void OnLoadNextLevel()
    {
			StartCoroutine(CameraTransition());
    }

		IEnumerator CameraTransition(){
			canControll = false;
			CameraFollow cam = subLevelCamera.GetComponent<CameraFollow>();
			SkyboxHandler handler = subLevelCamera.GetComponent<SkyboxHandler>();
			cam.SetTransform(subLevel.transform);
			float time = 0;
			float radius = cam.radius;
			float diff = subLevel.scale * defaultCameraZoom - radius;
			
			while(time < transitionTime){
				time += Time.fixedDeltaTime;
				handler.SetOpacity(time/transitionTime);
				cam.radius = radius + cameraCurve.Evaluate(time/transitionTime) * diff;	
				yield return new WaitForFixedUpdate();
			}
			handler.SetOpacity(0);

			float dist = levelCamera.transform.localPosition.magnitude;
			subLevelCamera.transform.position = levelCamera.transform.position;
			cam.radius = defaultCameraZoom;
			SetSubLevel(level, levelObject);
			canControll = true;
		}
  }
}