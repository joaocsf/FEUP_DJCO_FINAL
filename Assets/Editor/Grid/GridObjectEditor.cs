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
			obj.CalculateVolume();
			obj.SnapPosition();
		}
	}
}
