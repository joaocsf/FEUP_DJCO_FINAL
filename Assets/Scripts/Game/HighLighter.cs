using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Search_Shell.Game{
	public enum HighLight {
		Selected,
		Highlighted,
		None
	}
	public class HighLighter : MonoBehaviour {

		// Use this for initialization
		
		public Color selected;	
		public Color highlighted;	
		public Shader selectedShader;

		public float selectionRadius;
		public Texture selectionTexture;
		public float selectionScale;
		public Texture overlay;

		private static float time = 3f;
		private float lastLerpValue = 0;

		
		private Dictionary<Renderer, Shader> current;

		private Renderer[] _renderer;
		private Renderer[] renderer{
			get{ 
				if(_renderer == null) {
					List<Renderer> tmp = new List<Renderer>();	
					SearchRenderer(transform, ref tmp);
					_renderer = tmp.ToArray();
				}
				return _renderer;
			}
		}

		private void SearchRenderer(Transform ts, ref List<Renderer> list){
			foreach(Transform t in ts){
				Renderer r = t.GetComponent<Renderer>();
				if(r != null && t.tag != "IgnoreMaterials")
					list.Add(r);
				SearchRenderer(t, ref list);	
			}
		}

		private void UpdateDefault(){
			current = new Dictionary<Renderer, Shader>();
			foreach(Renderer r in renderer){
				current.Add(r, r.material.shader);
			}
		}

		private void UpdateShader(Shader shader, Color c){
			if(current == null) UpdateDefault();
			foreach(Renderer r in renderer){
				r.material.shader = shader;
				r.material.SetColor("_SelectionColor", c);
				r.material.SetFloat("_SelectionRadius", selectionRadius);
				r.material.SetTexture("_SelectionTexture", selectionTexture);
				r.material.SetFloat("_SelectionScale", selectionScale);
				r.material.SetTexture("_OverLay", overlay);
			}
			StartCoroutine(LerpOverLay(0, null));
		}

		private IEnumerator LerpOverLay(float direction, Action action){
			float t = lastLerpValue;
			if(direction > 0.5f) t = 0;

			while(t <= 1f){
				t+=Time.fixedDeltaTime * time;

				foreach(Renderer r in renderer)
					r.material.SetFloat("_SelectionThreshold", Mathf.Abs(Mathf.Clamp01(t) - direction));

				yield return new WaitForFixedUpdate();
			}
			lastLerpValue = 1 - direction;
			if(action != null)
				action();
		}

		private void ResetMaterials(){
			if(current == null) UpdateDefault();
			foreach(Renderer r in renderer){
				r.material.shader = current[r];
			}
		}

		public void SetSelected(HighLight highlight){
			Debug.Log(name + " - " + highlight);
			switch(highlight){
				case HighLight.Selected:
					UpdateShader(selectedShader, selected);
					break;
				case HighLight.Highlighted:
					UpdateShader(selectedShader, highlighted);
					break;
				case HighLight.None:
					StartCoroutine(LerpOverLay(1f, null));
					break;
			}
		}

		public void CopyMaterial(Material material){
			selectedShader = material.shader;
			selectionRadius = material.GetFloat("_SelectionRadius");
			selectionTexture = material.GetTexture("_SelectionTexture");
			selectionScale = material.GetFloat("_SelectionScale");
			overlay = material.GetTexture("_OverLay");
		}
	}
}