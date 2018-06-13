using System.Collections;
using System.Collections.Generic;
using Search_Shell.Grid;
using UnityEngine;

[System.Serializable]
public class SoundEventType{

	[EnumFlagsArray]
	public SurfaceType surfaceType;

	public AK.Wwise.Event startEvent;
	public AK.Wwise.Event endEvent;

	[HideInInspector]
	public GameObject gameObject;

	private bool IsOfType(SurfaceType t){
		return ((int)surfaceType & (1 << (int)t)) != 0;
	}

	private void PlayEvents(AK.Wwise.Event[] events){
		foreach(AK.Wwise.Event e in events)
			e.Post(gameObject);
	}

	public void PlayStartEvents(SurfaceType type){
		if(IsOfType(type))
			startEvent.Post(gameObject);
	}
	public void PlayEndEvents(SurfaceType type){
		if(IsOfType(type))
			endEvent.Post(gameObject);
	}
	
	
}
