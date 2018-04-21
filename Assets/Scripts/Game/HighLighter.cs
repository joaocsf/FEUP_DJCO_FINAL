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
		
		
		public Material selected;

		public Material highlighted;
		
		private Dictionary<Renderer, Material> current;

		private Renderer[] _renderer;
		private Renderer[] renderer{
			get{ 
				if(_renderer == null) _renderer = GetComponentsInChildren<Renderer>();
				return _renderer;
			}
		}

		private void UpdateDefault(){
			current = new Dictionary<Renderer, Material>();
			foreach(Renderer r in renderer){
				current.Add(r, r.material);
			}
		}

		private void UpdateMaterial(Material material){
			if(current == null) UpdateDefault();
			foreach(Renderer r in renderer){
				r.material = material;
			}
		}

		private void ResetMaterials(){
			if(current == null) UpdateDefault();
			foreach(Renderer r in renderer){
				r.material = current[r];
			}
		}

		public void SetSelected(HighLight highlight){
			switch(highlight){
				case HighLight.Selected:
					UpdateMaterial(selected);
					break;
				case HighLight.Highlighted:
					UpdateMaterial(highlighted);
					break;
				case HighLight.None:
					ResetMaterials();
					break;
			}
		}
	}
}