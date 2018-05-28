using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Search_Shell.Grid;
using Search_Shell.Helper;
using System;
using UnityEditorInternal;

[CustomEditor(typeof(PlaySoundBehaviour))]
public class PlaySoundEditor : Editor {

	private ReorderableList list;
	private float xOffset = 0;

	private void drawElement(Rect rect, string fieldName, float size, SerializedProperty element){
		float width = size * rect.width;
		GUIContent content = GUIContent.none;
		EditorGUI.PropertyField(
			new Rect(rect.x + xOffset, rect.y, width, EditorGUIUtility.singleLineHeight),
			element.FindPropertyRelative(fieldName), content);
		xOffset += width;
	}

	private void OnEnable() {
		list = new ReorderableList(serializedObject, serializedObject.FindProperty("events"), true, true, true, true);

		list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			rect.y +=2;
			xOffset = 0;

			drawElement(rect, "surfaceType", 0.2f, element);
			drawElement(rect, "startEvent", 0.4f, element);
			drawElement(rect, "endEvent", 0.4f, element);
		};
	}

	public override void OnInspectorGUI()
	{
		PlaySoundBehaviour obj = (PlaySoundBehaviour)target;
		base.OnInspectorGUI();	

		serializedObject.Update();
		list.DoLayoutList();
		serializedObject.ApplyModifiedProperties();
	
	}
}
