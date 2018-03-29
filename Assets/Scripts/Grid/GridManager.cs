using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Search_Shell.Grid{
	public class GridManager : MonoBehaviour {

		public bool debug = true;
		public Vector2 dims;
		HashSet<GridObject> objects = new HashSet<GridObject>();
		Dictionary<Vector3, GridObject> position2Object = new Dictionary<Vector3, GridObject>();

		IEnumerator Start () {

			yield return new WaitForFixedUpdate();

			GridObject[] objs = GetComponentsInChildren<GridObject>();	
			foreach(GridObject obj in objs){
				Debug.Log("Here");
				RegisterObject(obj);
			}
		}

		void OnDrawGizmos(){

			if(!debug) return;
			
			Vector3 local = transform.position + Vector3.one/2f;
			for(int i = -(int)dims.x; i < dims.x; i++){
				for(int j = -(int)dims.y; j < dims.y; j++){
					Gizmos.DrawLine(new Vector3(-dims.x,j,i) + local, local + new Vector3(dims.x,j,i));
					Gizmos.DrawLine(new Vector3(i,j,-dims.y) + local, local + new Vector3(i,j,dims.y));
				}
			}
		}

		public bool AssignObjectToPosition(GridObject obj, Vector3 pos){

			if(position2Object.ContainsKey(pos)) return false;

			position2Object.Add(pos, obj);

			return true;

		}

		public void RemoveObjectFromPosition(GridObject obj, Vector3 pos){
			if(position2Object.ContainsKey(pos))
				if(position2Object[pos] == obj)
					position2Object.Remove(pos);
			}

		public void ClearObject(GridObject obj){
			List<Vector3> positions = obj.GetVolumePositions();

			foreach(Vector3 position in positions){
				RemoveObjectFromPosition(obj, position);
			}

		}
		public void RegisterObject(GridObject obj){
			List<Vector3> positions = obj.GetVolumePositions();

			foreach(Vector3 position in positions){
				if(!AssignObjectToPosition(obj, position)) {
					Debug.LogError("Cannot Assign Object to this position! Overlap!!");
					ClearObject(obj);
				}
			}


			objects.Add(obj);
			


		}
		void Update () {
			
		}
	}
}