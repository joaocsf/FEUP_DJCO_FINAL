using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Search_Shell.Grid;
using Search_Shell.Helper;
using System;

[CustomEditor(typeof(GridObject))]
public class GridObjectEditor : Editor {

	GridObject obj;
	void OnEnable()
	{
		obj = (GridObject)target;
	}

	public void OnSceneGUI(){
		BoxCollider[] colliders = obj.GetComponents<BoxCollider>();

		foreach(BoxCollider col in colliders)
			HandleCollider(col);

	}

	private void HandleCollider(BoxCollider col){
		Vector3 max = col.center + col.size/2f;
		Vector3 min = col.center - col.size/2f;
		Vector3 center = col.center;
		max = obj.transform.TransformPoint(max);
		min = obj.transform.TransformPoint(min);
		center = obj.transform.TransformPoint(center);

		EditorGUI.BeginChangeCheck();
		Handles.color = Color.red;
		max = Handles.FreeMoveHandle(max, Quaternion.identity, 0.2f, Vector3.one, Handles.SphereHandleCap);
		Handles.color = Color.blue;
		min = Handles.FreeMoveHandle(min, Quaternion.identity, 0.2f, Vector3.one, Handles.SphereHandleCap);
		if(EditorGUI.EndChangeCheck()){
	
			max = obj.transform.InverseTransformPoint(max);
			min = obj.transform.InverseTransformPoint(min);

			col.size = max-min;
			col.center = (max + min)/2;

			obj.SnapPosition();
			obj.CalculateVolume();
		}

		EditorGUI.BeginChangeCheck();
		Handles.color = Color.green;
		center = Handles.FreeMoveHandle(center, Quaternion.identity, 0.3f, Vector3.one, Handles.SphereHandleCap);
		if(EditorGUI.EndChangeCheck()){
			center = obj.transform.InverseTransformPoint(center);
			col.center = center;
			obj.SnapPosition();
			obj.CalculateVolume();
		}
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

	private void ToggleDebug(Action<GridObject> action){

			GridObject[] objs = obj.transform.parent.GetComponentsInChildren<GridObject>();

			foreach(GridObject obj in objs){
				obj.SnapPosition();
				obj.CalculateVolume();
				action(obj);
			}
	}

	public override void OnInspectorGUI()
	{
		obj = (GridObject)target;
		AllOptions();
		EditorGUILayout.Separator();
		GUILayout.Label("This Object");
		Properties(obj);
		ThisObject(obj);

		if(GUILayout.Button("Update Shape")){
			obj.SnapPosition();
			obj.CalculateVolume();
		}
	}

	public void Properties(GridObject obj) {
		GUILayout.BeginVertical();
		obj.properties.canControll = EditorGUILayout.Toggle("Controll", obj.properties.canControll);
		obj.properties.isStatic = EditorGUILayout.Toggle("Static", obj.properties.isStatic);
		obj.properties.surface = (SurfaceType)EditorGUILayout.EnumPopup("Type", obj.properties.surface);
		GUILayout.EndVertical();
	}

	public void VerticalField(String name, Action action){
		GUILayout.BeginVertical();
		GUILayout.Label(name);
		if(GUILayout.Button("Toggle"))
			action();
		GUILayout.EndVertical();


	}

	public void ThisObject(GridObject obj){

		GUILayout.BeginHorizontal();
		VerticalField("Bounding Box", () => obj.debugBoundingBox = !obj.debugBoundingBox);
		VerticalField("Volumes", () => obj.debugVolumes = !obj.debugVolumes);
		VerticalField("Pivots", () => obj.debugPivot = !obj.debugPivot);
		GUILayout.EndHorizontal();


	}

	public void AllOptions(){
		GUILayout.Label("All Objects");
		GUILayout.BeginHorizontal();

		GUILayout.BeginVertical();
		GUILayout.Label("Bounding Boxes:");
		if(GUILayout.Button("Show"))
			ToggleDebug(obj => obj.debugBoundingBox = true);
		if(GUILayout.Button("Hide"))
			ToggleDebug(obj => obj.debugBoundingBox = false);

		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		GUILayout.Label("Volume Boxes:");
		if(GUILayout.Button("Show"))
			ToggleDebug(obj => obj.debugVolumes = true);
		if(GUILayout.Button("Hide"))
			ToggleDebug(obj => obj.debugVolumes = false);

		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}
}
