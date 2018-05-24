using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Search_Shell.Grid;
using Search_Shell.Game;
using System;
using Search_Shell.Controllers.Detector;
using Search_Shell.Controllers.Movement;
using Search_Shell.Controllers.Animation;

[CustomEditor(typeof(GridManager))]
public class GridManagerEditor : Editor {


	[MenuItem("Search Shell/Level")]
	private static void CreateLevel(){
		GameObject level = new GameObject();
		level.name = "New Level";
		level.AddComponent<GridManager>();
		level.AddComponent<LevelProperties>();
		Selection.activeGameObject = level;
	}

	private static Transform selected;
	public override void OnInspectorGUI(){
		base.OnInspectorGUI();
		selected = ((GridManager)target).transform;	
		btn("Static Object", CreateStaticObject);
		btn("Static Controllable", CreateStaticControllable);
		btn("Jump Object", CreateJumpObject);
		btn("Push Object", CreatePushObject);
		btn("Roll Object", CreateRollObject);

	}

	private void btn(string name, Action action){
		if(GUILayout.Button(name))
			action();
	}

	public static void CreateStaticObject(){
		GridObjectBuilder helper = new GridObjectBuilder(selected);
		helper.SetObjectProperties(false, true);
	}

	public static void CreateStaticControllable() {
		GridObjectBuilder helper = new GridObjectBuilder(selected);
		helper.SetObjectProperties(true, true);
	}

	public static void CreateJumpObject() {
		GridObjectBuilder helper = new GridObjectBuilder(selected);
		helper.SetObjectProperties(true, false);
		helper.SetJumpProperties();
	}

	public static void CreatePushObject() {
		GridObjectBuilder helper = new GridObjectBuilder(selected);
		helper.SetObjectProperties(true, false);
		helper.SetPushProperties();
	}

	public static void CreateRollObject() {
		GridObjectBuilder helper = new GridObjectBuilder(selected);
		helper.SetObjectProperties(true, false);
		helper.SetRollProperties();
	}

	public class GridObjectBuilder {
		public GameObject obj;

		public GridObjectBuilder(Transform parent) {
			obj = new GameObject("New Obj");
			obj.transform.parent = parent;
			obj.transform.localPosition = Vector3.zero;
			obj.AddComponent<BoxCollider>();
			obj.AddComponent<GridObject>();
		}

		public void SetObjectProperties(bool canControll, bool isStatic) {
			GridObject tmp = obj.GetComponent<GridObject>();
			tmp.properties = new GridObjectProperties{canControll = canControll, isStatic = isStatic};
			if(canControll)
				obj.AddComponent<AdjacentObjects>();
		}

		private void SoftStartAnimation(AnimationCurve curve){
			Keyframe start = new Keyframe(0f, 0f, 0f, 1f);
			Keyframe end = new Keyframe(1f, 1f, 0f, 0f);
			curve.AddKey(start);
			curve.AddKey(end);
		}
		private void HardStartAnimation(AnimationCurve curve){
			Keyframe start = new Keyframe(0f, 0f, 0f, 0f);
			Keyframe end = new Keyframe(1f, 1f, 1f, 0f);
			curve.AddKey(start);
			curve.AddKey(end);
		}

		private void AddGravityAnimation(float time = 0.3f){
			LinearAnimation anim = obj.AddComponent<LinearAnimation>();
			anim.animateOnMovement = false;
			anim.duration = time;
			anim.yOffset = 0f;	
			SoftStartAnimation(anim.curve);
		}

		public void SetJumpProperties(float time = 0.3f){
			obj.AddComponent<JumpController>();
			JumpAnimation anim = obj.AddComponent<JumpAnimation>();
			anim.animateOnMovement = true;
			anim.duration = time;
			HardStartAnimation(anim.curve);
			AddGravityAnimation();
		}

		public void SetPushProperties(float time = 0.3f){
			obj.AddComponent<PushController>();
			AddGravityAnimation();	
			LinearAnimation lin = obj.GetComponent<LinearAnimation>();
			lin.animateOnMovement = true;
		}

		public void SetRollProperties(float time = 0.3f){
			obj.AddComponent<RollController>();
			RollAnimation anim = obj.AddComponent<RollAnimation>();
			anim.animateOnMovement = true;
			anim.duration = time;
			SoftStartAnimation(anim.curve);
			AddGravityAnimation();
		}

	}
}
