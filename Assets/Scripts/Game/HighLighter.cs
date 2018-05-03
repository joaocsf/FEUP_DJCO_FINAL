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

		
		private Dictionary<Renderer, Shader> current;

		private Renderer[] _renderer;
		private Renderer[] renderer{
			get{ 
				if(_renderer == null) _renderer = GetComponentsInChildren<Renderer>();
				return _renderer;
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
		}

		private void ResetMaterials(){
			if(current == null) UpdateDefault();
			foreach(Renderer r in renderer){
				r.material.shader = current[r];
			}
		}

		public void SetSelected(HighLight highlight){
			switch(highlight){
				case HighLight.Selected:
					UpdateShader(selectedShader, selected);
					break;
				case HighLight.Highlighted:
					UpdateShader(selectedShader, highlighted);
					break;
				case HighLight.None:
					ResetMaterials();
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