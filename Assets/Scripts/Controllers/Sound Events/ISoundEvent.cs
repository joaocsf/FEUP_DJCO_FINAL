using System.Collections;
using System.Collections.Generic;
using Search_Shell.Grid;
using UnityEngine;

public interface ISoundEvent {

	//Start Roll event
	void RollStart (SurfaceType surface);
	
	//End Roll event
	void RollEnd (SurfaceType surface);

	//Start Roll event
	void JumpStart (SurfaceType surface);
	
	//End Roll event
	void JumpEnd (SurfaceType surface);

	//Start Roll event
	void PushStart (SurfaceType surface);
	
	//End Roll event
	void PushEnd (SurfaceType surface);

	//Start Roll event
	void GravityStart (SurfaceType surface);
	
	//End Roll event
	void GravityEnd (SurfaceType surface);
}
