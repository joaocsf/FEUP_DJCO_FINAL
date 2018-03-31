﻿using System.Collections;
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
				obj.SnapPosition();
				obj.CalculateVolume();
				// Debug.Log("Registered:" + obj.name);
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

		public GridObject AssignObjectToPosition(GridObject obj, Vector3 pos){

			GridObject solid;	
			//Debug.Log(pos.x + " - " + pos.y  + " - " + pos.z);
			if(position2Object.TryGetValue(pos, out solid))
				return solid;

			position2Object.Add(pos, obj);

			return null;
		}

		public void RemoveObjectFromPosition(GridObject obj, Vector3 pos){
			if(position2Object.ContainsKey(pos))
				if(position2Object[pos] == obj)
					position2Object.Remove(pos);
			}


		public void ClearObject(GridObject obj, List<Vector3> positions){
			foreach(Vector3 position in positions){
				RemoveObjectFromPosition(obj, position);
			}
		}

		public void ClearObject(GridObject obj){
			ClearObject(obj, obj.GetVolumePositions());
		}

		public void SlideObject(GridObject obj, Vector3 movement){
			ClearObject(obj);
			List<Vector3> calculatedMovement = obj.CalculateMovement(movement);
			if(RegisterObject(obj, calculatedMovement)){
				obj.Slide(movement);				
			}else{
				ClearObject(obj, calculatedMovement);
				RegisterObject(obj);
			}


		}

		public bool RegisterObject(GridObject obj, List<Vector3> positions){

			foreach(Vector3 position in positions){
				GridObject existing = AssignObjectToPosition(obj, position);
				if(existing != null) {
					ClearObject(obj);
					return false;
				}
			}

			objects.Add(obj);
			return true;			
		}

		public bool RegisterObject(GridObject obj){
			if(!RegisterObject(obj, obj.GetVolumePositions())){
					Debug.LogError("Cannot Assign Object to this position! Overlap!!");
					return false;
			}
			return true;
		}

		void Update () {
			
		}
	}
}