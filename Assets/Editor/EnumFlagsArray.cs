using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(EnumFlagsArray))]
public class EnumFlagsArrayUI : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
		property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
	}

}
