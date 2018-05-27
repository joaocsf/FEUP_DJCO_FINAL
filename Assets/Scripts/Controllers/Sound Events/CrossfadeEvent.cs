using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossfadeEvent : MonoBehaviour, ISoundEvent{
	
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GravityEnd()
    {
		  Debug.LogWarning("Ended falling");
    }

    public void GravityStart()
    {
		  Debug.LogWarning("Started falling");
    }

    public void JumpEnd()
    {

    }

    public void JumpStart()
    {

    }

    public void PushEnd()
    {
		  Debug.LogWarning("Stopped pushing");
    }

    public void PushStart()
    {
		  Debug.LogWarning("Started pushing");
    }

    public void RollEnd()
    {

    }

    public void RollStart()
    {

    }
}
