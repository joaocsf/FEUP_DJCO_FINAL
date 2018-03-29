using System.Collections;
using System.Collections.Generic;
using Search_Shell.Helper;
using UnityEngine;

namespace Search_Shell.Grid
{
	public class GridObject : MonoBehaviour {

		public List<Vector3> volume = new List<Vector3>();
		public Vector3 finalAngles = Vector3.zero;
		public Vector3 finalPosition = Vector3.zero;
		public Vector3 offset = Vector3.zero;

		Drawer drawDebug = new Drawer();

		void AddVolume(Vector3 pos){
			volume.Add(pos);
		}

		void Start () {

			AddVolume(Vector3.zero);
			AddVolume(Vector3.up);
			//AddVolume(Vector3.up*2);
			//AddVolume(Vector3.up*3);

			PositionBox();

		}

		void PositionBox(){

			finalPosition = transform.localPosition;
			finalAngles = transform.localEulerAngles;

			Bounds bounds = new Bounds();	
			foreach(Vector3 pos in GetVolumePositions()){
				bounds.Encapsulate(pos-transform.localPosition);
			}

			Vector3 tempCenter = bounds.center + transform.localPosition;
			bounds.size += Vector3.one;
			tempCenter.x = (int)(tempCenter.x) + Mathf.Sign(tempCenter.x) * ( ((int)bounds.size.x % 2 == 0 && bounds.size.x > 0)? 0.5f : 0f );
			tempCenter.y = (int)(tempCenter.y) + Mathf.Sign(tempCenter.y) * ( ((int)bounds.size.y % 2 == 0 && bounds.size.y > 0)? 0.5f : 0f );
			tempCenter.z = (int)(tempCenter.z) + Mathf.Sign(tempCenter.z) * ( ((int)bounds.size.z % 2 == 0 && bounds.size.z > 0)? 0.5f : 0f );

			drawDebug.AddDrawable( new DrawableCube(transform.parent.position + tempCenter, Vector3.one/2, Color.black, false), 500);

			finalPosition = transform.localPosition;
			finalAngles = transform.localEulerAngles;

			transform.localPosition = tempCenter - bounds.center;

		}

		void OnDrawGizmos(){
			drawDebug.Draw();
		}
		
		void Update () {
			
		}

		private Vector3 ComputeRealPosition (Vector3 volumePos) {

			Matrix4x4 transformation = 
				Matrix4x4.Translate(finalPosition + offset) *
				Matrix4x4.Rotate(Quaternion.Euler(finalAngles)) *
				Matrix4x4.identity;

			Vector3 res = transformation.MultiplyPoint(volumePos);

			drawDebug.AddDrawable(new DrawableCube(res + transform.parent.position), 500);

			Debug.Log(res);
			return res;
		}

		public List<Vector3> GetVolumePositions () {

			List<Vector3> res = new List<Vector3>();

			for(int i = 0; i < volume.Count; i++){
				Vector3 realPosition = ComputeRealPosition(volume[i]);
				res.Add(realPosition);
			}

			return res;
		}
	}
}
