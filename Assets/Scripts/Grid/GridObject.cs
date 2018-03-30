using System.Collections;
using System.Collections.Generic;
using Search_Shell.Helper;
using UnityEngine;

namespace Search_Shell.Grid
{
	public class GridObject : MonoBehaviour {

		public HashSet<Vector3> volume = new HashSet<Vector3>();
		public Vector3 finalAngles = Vector3.zero;
		public Vector3 finalPosition = Vector3.zero;
		public Vector3 offset = Vector3.zero;

		public void RemoveVolume(){
			volume.Clear();
		}
		public void AddVolume(Vector3 pos){
			Debug.Log("Adding Point" + pos);
			volume.Add(pos);
		}

		void Start () {
			PositionBox();
		}

		private Color c = Color.red;
		public void PositionBox(){

			finalPosition = transform.localPosition;
			finalAngles = transform.localEulerAngles;
			Debug.Log("Current Position: "  + finalPosition + " Current Rotation" + finalAngles);
			Vector3 tmp = finalPosition + transform.parent.position;
			Debug.DrawLine(tmp, tmp + Vector3.up, Color.white, 2);
			Drawer.instance.AddDrawable( new DrawableCube(tmp, Vector3.one/2, Color.yellow, true), 100);
			c = Color.green;
			GridBounds bounds = new GridBounds();	
			List<Vector3> volumes = GetVolumePositions();
			if(volumes.Count == 0)
				return;

			foreach(Vector3 pos in volumes){
				Debug.Log( "Drawing:" + pos);
				Drawer.instance.AddDrawable( new DrawableCube(transform.parent.position + finalPosition, Vector3.one/3f, Color.cyan, true), 100);
				Drawer.instance.AddDrawable( new DrawableCube(transform.parent.position + pos, Vector3.one/2f, Color.yellow, false), 100);
				Vector3 basePoint = transform.parent.position + pos;
				Debug.DrawLine(basePoint, basePoint + Vector3.down, Color.white, 2);
				Debug.Log(basePoint);
				bounds.Encapsulate(pos - finalPosition);
			}

			Vector3 tempCenter = bounds.center + transform.localPosition;
			Drawer.instance.AddDrawable( new DrawableLine(transform.parent.position + tempCenter, transform.position, Color.green));
			Vector3 size = bounds.size + Vector3.one;
			tempCenter.x = (int)(tempCenter.x) + Mathf.Sign(tempCenter.x) * ( ((int)size.x % 2 == 0)? 0.5f : 0f );
			tempCenter.y = (int)(tempCenter.y) + Mathf.Sign(tempCenter.y) * ( ((int)size.y % 2 == 0)? 0.5f : 0f );
			tempCenter.z = (int)(tempCenter.z) + Mathf.Sign(tempCenter.z) * ( ((int)size.z % 2 == 0)? 0.5f : 0f );

			Debug.Log("Center:" + bounds.center);
			c = Color.blue;
			Debug.Log(transform.localPosition);
			transform.position = transform.parent.position + tempCenter - bounds.center;
			Debug.Log(transform.localPosition);
			finalPosition = transform.localPosition;
			finalAngles = transform.localEulerAngles;

			GetVolumePositions();

		}

		[ExecuteInEditMode]
		void OnDrawGizmos(){
			Drawer.instance.Draw();
		}
		
		void Update () {
			
		}

		private Vector3 ComputeRealPosition (Vector3 volumePos) {

			Matrix4x4 transformation = 
				Matrix4x4.Translate(finalPosition + offset) *
				Matrix4x4.Rotate(Quaternion.Euler(finalAngles)) *
				Matrix4x4.identity;

			Debug.Log("Trasnforming Point" + volumePos);
			Vector3 res = transformation.MultiplyPoint(volumePos);
			Debug.Log("Result Point" + res);
			Drawer.instance.AddDrawable(new DrawableCube(transform.parent.position + res, c), 1000);
			return res;
		}

		public List<Vector3> GetVolumePositions () {

			List<Vector3> res = new List<Vector3>();

			foreach(Vector3 point in volume){
				Vector3 realPosition = ComputeRealPosition(point);
				res.Add(realPosition);
			}

			return res;
		}
	}
}
