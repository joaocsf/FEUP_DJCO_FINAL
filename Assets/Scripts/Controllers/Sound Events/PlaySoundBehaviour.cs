using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundBehaviour : MonoBehaviour, ISoundEvent {

	[EnumFlagsArray]
	public EventType eventType;

	public AK.Wwise.Event[] startEvents;
	public AK.Wwise.Event[] endEvents;
	
	[System.Flags]
	public enum EventType {
		Gravity, Jump, Push, Roll
	}

	private bool IsOfType(EventType t){
		return ((int)eventType & (1 << (int)t)) != 0;
	}

	private void PlayEvents(AK.Wwise.Event[] events){
		Debug.Log("Playing Events");
		foreach(AK.Wwise.Event e in events)
			e.Post(gameObject);
	}

  public void GravityEnd()
  {
		if(IsOfType(EventType.Gravity))
			PlayEvents(endEvents);
  }

  public void GravityStart()
  {
		if(IsOfType(EventType.Gravity))
			PlayEvents(startEvents);
  }

  public void JumpEnd()
  {
		if(IsOfType(EventType.Jump))
			PlayEvents(endEvents);
  }

  public void JumpStart()
  {
		if(IsOfType(EventType.Jump))
			PlayEvents(startEvents);
  }

  public void PushEnd()
  {
		if(IsOfType(EventType.Push))
			PlayEvents(endEvents);
  }

  public void PushStart()
  {
		Debug.Log("Start Push" + (int)eventType);
		if(IsOfType(EventType.Push))
			PlayEvents(startEvents);
  }

  public void RollEnd()
  {
		if(IsOfType(EventType.Roll))
			PlayEvents(endEvents);
  }

  public void RollStart()
  {
		Debug.Log("Roll" + eventType + " - " + (int)EventType.Roll);
		if(IsOfType(EventType.Roll))
			PlayEvents(startEvents);
  }
}
