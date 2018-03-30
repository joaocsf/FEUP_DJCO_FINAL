using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Search_Shell.Grid;
using Search_Shell.Helper;

[CustomEditor(typeof(GridObject))]
public class GridObjectEditor : Editor {

	GridObject obj;
	void OnEnable()
	{
		obj = (GridObject)target;
	}

	public void OnSceneGUI(){
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

	private Vector3 computePosition(Transform transform, Vector3 finalPos){
		Vector3 dir = finalPos - transform.position;
		
		return transform.InverseTransformVector(dir);

	}
	public override void OnInspectorGUI()
	{
		obj = (GridObject)target;
		if(GUILayout.Button("Update Shape")){

			obj.RemoveVolume();
			BoxCollider[] cols = obj.GetComponents<BoxCollider>();
			for(int col = 0; col < cols.Length; col++){ 
				BoxCollider collider = cols[col];
				// Debug.Log("Min" + collider.bounds.min + " - Max:" + collider.bounds.max);
				// Debug.Log("LocalPos" + obj.transform.localPosition);
				Vector3 min = obj.transform.InverseTransformPoint(collider.bounds.min) + Vector3.one/2f;
				Vector3 max = obj.transform.InverseTransformPoint(collider.bounds.max) - Vector3.one/2f;

				min = computePosition(obj.transform, collider.bounds.min + Vector3.one/2f);
				max = computePosition(obj.transform, collider.bounds.max - Vector3.one/2f);

				//min = obj.WorldToLocal(collider.bounds.min) + Vector3.one/2f;
				//max = obj.WorldToLocal(collider.bounds.max) - Vector3.one/2f;

				SwapMinMax(ref min, ref max);
				// Debug.Log((max-min));
				// Debug.Log(min.y);
				// Debug.Log("Min" + min + " - Max:" + max);
				int minX = Mathf.RoundToInt(min.x);
				int minY = Mathf.RoundToInt(min.y);
				int minZ = Mathf.RoundToInt(min.z);

				int maxX = Mathf.RoundToInt(max.x);
				int maxY = Mathf.RoundToInt(max.y);
				int maxZ = Mathf.RoundToInt(max.z);
				// Debug.Log("i:" + minX + " j:" + minY + " k:" + minZ);
				// Debug.Log("i:" + maxX + " j:" + maxY + " k:" + maxZ);
				for(int i = minX; i <= maxX; i++)
					for(int j = minY; j <= maxY; j++)
						for(int k = minZ; k <= maxZ; k++)
							obj.AddVolume(new Vector3(i,j,k));
			}
			obj.SnapPosition();

		}

	}
}
