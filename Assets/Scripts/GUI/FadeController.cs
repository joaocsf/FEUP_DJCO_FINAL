using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(CanvasGroup))]
public class FadeController : MonoBehaviour {

	public bool _state;

	Animator anim;
	CanvasGroup group;

	void Start(){
		anim = GetComponent<Animator>();
		anim.enabled = false;
		group = GetComponent<CanvasGroup>();
		group.blocksRaycasts = _state;
		group.interactable = _state;
		group.alpha = _state ? 1f : 0f;
	}

	public void SetActive(bool state){
		if(state != _state){
			anim.enabled = true;
			anim.Rebind();
			anim.SetInteger("activate", state? 1 : 2);
		}
		_state = state;
	}
	void Update () {
		
	}
}
