using System.Collections;
using System.Collections.Generic;
using Search_Shell.Grid;
using UnityEngine;

public class CrossfadeEvent : MonoBehaviour, ISoundEvent{
	
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GravityEnd(SurfaceType surface)
    {
		  Debug.LogWarning("Ended falling");
    }

    public void GravityStart(SurfaceType surface)
    {
		  Debug.LogWarning("Started falling");
    }

    public void JumpEnd(SurfaceType surface)
    {

    }

    public void JumpStart(SurfaceType surface)
    {

    }

    public void PushEnd(SurfaceType surface)
    {
		  Debug.LogWarning("Stopped pushing");
    }

    public void PushStart(SurfaceType surface)
    {
		  Debug.LogWarning("Started pushing");
    }

    public void RollEnd(SurfaceType surface)
    {

    }

    public void RollStart(SurfaceType surface)
    {

    }
}
