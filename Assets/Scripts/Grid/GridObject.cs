using System.Collections;
using System.Collections.Generic;
using Search_Shell.Helper;
using UnityEngine;

namespace Search_Shell.Grid
{
	public struct Pivot {
		public Vector3 point;
		public Vector3 normal;
	}
	public class GridObject : MonoBehaviour {

		public bool debugPivot = true;
		public bool debugBoundingBox = true;
		public bool debugVolumes = false;
		[SerializeField]
		private List<Vector3> volume = new List<Vector3>();
		public Vector3 finalAngles = Vector3.zero;
		public Vector3 finalPosition = Vector3.zero;
		public Vector3 offset = Vector3.zero;

		private GridBounds _bounds;
		private Drawer _drawer = new Drawer();

		private Vector3 lastDirection;

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
			finalPosition += movement;
			finalPosition = SnapVector(finalPosition);
			transform.localPosition = finalPosition;
		}

		public Vector3 SnapVector (Vector3 v3){
			v3.x = Mathf.Round(v3.x);
			v3.y = Mathf.Round(v3.y);
			v3.z = Mathf.Round(v3.z);

			return v3;
		}
		public Matrix4x4 RollMatrix(Vector3 direction, float angle){
			Pivot pivot = GetRollPivot(direction);
			Matrix4x4 matrix = 
				Matrix4x4.Translate(pivot.point) *
				Matrix4x4.Rotate(Quaternion.Euler(pivot.normal * angle)) *
				Matrix4x4.Translate(-pivot.point) *
				Matrix4x4.identity;
			return matrix;
		}

		public void Roll (Vector3 direction, float angle){
			Roll(direction, angle, ref finalPosition, ref finalAngles);
		}

		public Vector3 MatrixRotation(Vector3 point, Vector3 angles){
			Matrix4x4 mat = Matrix4x4.Rotate(Quaternion.Euler(angles)) * Matrix4x4.identity;

			return mat.MultiplyVector(point);
		}

		public void Roll(Vector3 direction, float angle, ref Vector3 position, ref Vector3 angles){
			Pivot pivot = GetRollPivot(direction);
			Matrix4x4 matrix = RollMatrix(direction,angle);
			Vector3 tempPos = position;
			position = matrix.MultiplyPoint(position);

			Vector3 upVector = matrix.MultiplyVector(Vector3.up);

			Quaternion rot = Quaternion.AngleAxis(angle, pivot.normal) * Quaternion.Euler(angles.x, angles.y, angles.z);

			angles = rot.eulerAngles;
			_bounds = GetBounds();
		}

		public List<Vector3> CalculateRoll(Vector3 direction, float angle){
			lastDirection = direction;
			Pivot pivot = GetRollPivot(direction);
			Vector3 tempPosition = finalPosition;
			Vector3 tempAngles = finalAngles;

			Roll(direction,angle);

			List<Vector3> res = GetVolumePositions();

			finalPosition = tempPosition;
			finalAngles = tempAngles;

			return res;
		}

		public Pivot GetRollPivot(Vector3 direction){
			GridBounds bounds = GetBounds();
			Vector3 position = Vector3.zero;
			position.y = bounds.min.y;

			position.x = (direction.x < -0.5f)? bounds.min.x : (direction.x > 0.5f)? bounds.max.x : bounds.center.x; 
			position.z = (direction.z < -0.5f)? bounds.min.z : (direction.z > 0.5f)? bounds.max.z : bounds.center.z; 
			if(Mathf.Abs(direction.z) > 0){
				direction.z *= 1;
			}
			Vector3 normal = Vector3.Cross(direction, Vector3.up).normalized;

			return new Pivot{point= position, normal = normal};
		} 

		public List<Vector3> CalculateSlide(Vector3 relativePosition){
			Vector3 tempPosition = finalPosition;
			finalPosition = finalPosition + relativePosition;
			finalPosition = SnapVector(finalPosition);
			List<Vector3> res = GetVolumePositions();

			finalPosition = tempPosition;

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

			bounds.Encapsulate(bounds.max + new Vector3(0.5f,0.5f,0.5f));
			bounds.Encapsulate(bounds.min - new Vector3(0.5f,0.5f,0.5f));
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
			_bounds = GetBounds();
		}

		private void DebugPivot(Vector3 dir){
			Pivot pivot = GetRollPivot(dir);
			pivot.point += transform.parent.position;
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(pivot.point, 0.1f);
			Gizmos.color = new Color(1f,0,0,1.0f);
			Gizmos.DrawWireCube(pivot.point + pivot.normal/2f, pivot.normal + Vector3.one*0.05f);


		}
		private void DebugPivots(){
			//DebugPivot(Vector3.left);
			DebugPivot(lastDirection);
			//DebugPivot(Vector3.right);
			//DebugPivot(Vector3.forward);
			//DebugPivot(Vector3.back);
		}

		private void DebugBoundingBox(){
			if(_bounds == null)
				_bounds = GetBounds();
			Color c = Color.blue;
			c.a = 0.5f;
			Gizmos.color = c;
			Gizmos.DrawCube(transform.parent.position + _bounds.center, _bounds.size);
			c.a = 1.0f;
			Gizmos.color = c;
			Gizmos.DrawWireCube(transform.parent.position + _bounds.center, _bounds.size);
		}
		[ExecuteInEditMode]
		void OnDrawGizmos(){
			

			if(debugVolumes)
				_drawer.Draw();

			if(debugBoundingBox)
				DebugBoundingBox();

			if(debugPivot)
				DebugPivots();
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
			return res;
		}

		public List<Vector3> GetVolumePositions () {

			List<Vector3> res = new List<Vector3>();
			_drawer.Clear();


			foreach(Vector3 point in volume){
				Vector3 realPosition = ComputeRealPosition(point);
				_drawer.AddDrawable(new DrawableCube(transform.parent.position + realPosition, Vector3.one, Color.green, false));
				res.Add(realPosition);
			}

			return res;
		}
	}
}
