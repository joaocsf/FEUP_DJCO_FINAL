using System.Collections;
using System.Collections.Generic;
using Search_Shell.Grid;
using UnityEngine;

public class PlaySoundBehaviour : MonoBehaviour, ISoundEvent {

	[EnumFlagsArray]
	public EventType eventType;

	[HideInInspector]
	public List<SoundEventType> events;
	
	[System.Flags]
	public enum EventType {
		Gravity, Jump, Push, Roll
	}

	void Start(){
		foreach(SoundEventType type in events){
			type.gameObject = gameObject;
		}
	}

	private bool IsOfType(EventType t){
		return ((int)eventType & (1 << (int)t)) != 0;
	}

	private void PlayEvents(bool start, SurfaceType surface){
		foreach(SoundEventType e in events)
			if(start)
				e.PlayStartEvents(surface);
			else
				e.PlayEndEvents(surface);
	}

  public void GravityEnd(SurfaceType surface)
  {
		if(IsOfType(EventType.Gravity))
			PlayEvents(false, surface);
  }

  public void GravityStart(SurfaceType surface)
  {
		if(IsOfType(EventType.Gravity))
			PlayEvents(true, surface);
  }

  public void JumpEnd(SurfaceType surface)
  {
		if(IsOfType(EventType.Jump))
			PlayEvents(false, surface);
  }

  public void JumpStart(SurfaceType surface)
  {
		if(IsOfType(EventType.Jump))
			PlayEvents(true, surface);
  }

  public void PushEnd(SurfaceType surface)
  {
		if(IsOfType(EventType.Push))
			PlayEvents(false, surface);
  }

  public void PushStart(SurfaceType surface)
  {
		if(IsOfType(EventType.Push))
			PlayEvents(true, surface);
  }

  public void RollEnd(SurfaceType surface)
  {
		if(IsOfType(EventType.Roll))
			PlayEvents(false, surface);
  }

  public void RollStart(SurfaceType surface)
  {
		if(IsOfType(EventType.Roll))
			PlayEvents(true, surface);
  }
}
