using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
[CustomEditor(typeof(GrassFX))]
public class GrassFXEditor : Editor {

    public override void OnInspectorGUI(){
        base.OnInspectorGUI();
        GrassFX grass = (GrassFX)target;
        if(GUILayout.Button("Generate")){
            grass.GenerateMesh();
        }
    }
}