using System.Collections;
using System.Collections.Generic;
using Search_Shell.Helper;
using UnityEngine;

namespace Search_Shell.Grid
{
	public class GridObject : MonoBehaviour {

		[SerializeField]
		private List<Vector3> volume = new List<Vector3>();
		public Vector3 finalAngles = Vector3.zero;
		public Vector3 finalPosition = Vector3.zero;
		public Vector3 offset = Vector3.zero;

		public void RemoveVolume(){
			volume.Clear();
		}
		public void AddVolume(Vector3 pos){
			if(!volume.Contains(pos))
				volume.Add(pos);
		}

		void Start () {
			SnapPosition();
		}

		public void Slide(Vector3 movement){
			finalPosition = transform.localPosition + movement;
			finalPosition = SnapVector(finalPosition);
			//Animation
			transform.localPosition = finalPosition;
		}

		public Vector3 SnapVector (Vector3 v3){
			v3.x = Mathf.Round(v3.x);
			v3.y = Mathf.Round(v3.y);
			v3.z = Mathf.Round(v3.z);

			return v3;
		}
		public List<Vector3> CalculateMovement(Vector3 relativePosition){
			finalPosition = transform.localPosition + relativePosition;
			finalPosition = SnapVector(finalPosition);
			List<Vector3> res = GetVolumePositions();

			finalPosition = transform.localPosition;

			return res;			

		}

		private Color c = Color.white;
		public Vector3 WorldToLocal(Vector3 position){
        Vector3 scaleInvert = transform.localScale;
        scaleInvert = new Vector3(1f/scaleInvert.x, 1f/scaleInvert.y, 1f/scaleInvert.z);
        return Vector3.Scale(Quaternion.Inverse(transform.rotation)*(position-transform.position), scaleInvert);}

		public void SnapPosition(){
			transform.localPosition = new Vector3(
				Mathf.Floor(transform.localPosition.x), 
				Mathf.Floor(transform.localPosition.y), 
				Mathf.Floor(transform.localPosition.z));
				finalPosition = transform.localPosition;
				finalAngles = transform.localEulerAngles;
				GetVolumePositions();
		}

		public GridBounds GetBounds(){
			GridBounds bounds = new GridBounds();
			List<Vector3> volumes = GetVolumePositions();
			foreach (Vector3 v3 in volumes)
				bounds.Encapsulate(v3);

			return bounds;
		}
		private Vector3 computePosition(Transform transform, Vector3 finalPos){
			Vector3 dir = finalPos - transform.position;
			
			return transform.InverseTransformVector(dir);

		}

		private void SwapMinMax(ref Vector3 min ,ref Vector3 max){
			Vector3 copy = min;
			min.x = Mathf.Min(min.x, max.x);
			min.y = Mathf.Min(min.y, max.y);
			min.z = Mathf.Min(min.z, max.z);

			max.x = Mathf.Max(copy.x, max.x);
			max.y = Mathf.Max(copy.y, max.y);
			max.z = Mathf.Max(copy.z, max.z);
		}

		public void CalculateVolume(){

			RemoveVolume();

			BoxCollider[] cols = GetComponents<BoxCollider>();
			for(int col = 0; col < cols.Length; col++){ 
				BoxCollider collider = cols[col];
				Vector3 min = transform.InverseTransformPoint(collider.bounds.min) + Vector3.one/2f;
				Vector3 max = transform.InverseTransformPoint(collider.bounds.max) - Vector3.one/2f;

				min = computePosition(transform, collider.bounds.min + Vector3.one/2f);
				max = computePosition(transform, collider.bounds.max - Vector3.one/2f);

				SwapMinMax(ref min, ref max);

				int minX = Mathf.RoundToInt(min.x);
				int minY = Mathf.RoundToInt(min.y);
				int minZ = Mathf.RoundToInt(min.z);

				int maxX = Mathf.RoundToInt(max.x);
				int maxY = Mathf.RoundToInt(max.y);
				int maxZ = Mathf.RoundToInt(max.z);

				for(int i = minX; i <= maxX; i++)
					for(int j = minY; j <= maxY; j++)
						for(int k = minZ; k <= maxZ; k++)
							AddVolume(new Vector3(i,j,k));
			}
		}

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
				Drawer.instance.AddDrawable( new DrawableCube(transform.parent.position + finalPosition, Vector3.one/3f, Color.cyan, true), 100);
				Drawer.instance.AddDrawable( new DrawableCube(transform.parent.position + pos, Vector3.one/2f, Color.yellow, false), 100);
				Vector3 basePoint = transform.parent.position + pos;
				Debug.DrawLine(basePoint, basePoint + Vector3.down, Color.white, 2);
				bounds.Encapsulate(pos - finalPosition);
			}

			Vector3 tempCenter = bounds.center + transform.localPosition;
			Drawer.instance.AddDrawable( new DrawableCube(transform.parent.position + tempCenter, Vector3.one/2f, Color.white, false));
			Drawer.instance.AddDrawable( new DrawableLine(transform.parent.position + tempCenter, transform.position, Color.green));
			Vector3 size = bounds.size + Vector3.one;

			Debug.Log( "Temp Center Begin:" + tempCenter + "Size:" + size);

			tempCenter.x = Mathf.FloorToInt(tempCenter.x) + Mathf.Sign(tempCenter.x) * ( (Mathf.RoundToInt(size.x) % 2 == 0)? 0.5f : 0f );
			tempCenter.y = Mathf.FloorToInt(tempCenter.y) + Mathf.Sign(tempCenter.y) * ( (Mathf.RoundToInt(size.y) % 2 == 0)? 0.5f : 0f );
			tempCenter.z = Mathf.FloorToInt(tempCenter.z) + Mathf.Sign(tempCenter.z) * ( (Mathf.RoundToInt(size.z) % 2 == 0)? 0.5f : 0f );

			Debug.Log( "Temp Center End:" + tempCenter + "Size:" + size);

			Drawer.instance.AddDrawable( new DrawableCube(transform.parent.position + tempCenter, Vector3.one/2f, Color.black, false));
			c = Color.blue;
			transform.position = transform.parent.position + tempCenter - bounds.center;
			Debug.Log("End Position:" + transform.localPosition);
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

			// Debug.Log("Trasnforming Point" + volumePos);
			Vector3 res = transformation.MultiplyPoint(volumePos);
			res.x = Mathf.Round(res.x);
			res.y = Mathf.Round(res.y);
			res.z = Mathf.Round(res.z);
			// Debug.Log("Result Point" + res);
			Drawer.instance.AddDrawable(new DrawableCube(transform.parent.position + res, Vector3.one, c, false), 1000);
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
