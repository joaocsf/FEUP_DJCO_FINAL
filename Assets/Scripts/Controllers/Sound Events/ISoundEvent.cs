using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISoundEvent {

	//Start Roll event
	void RollStart ();
	
	//End Roll event
	void RollEnd ();

	//Start Roll event
	void JumpStart ();
	
	//End Roll event
	void JumpEnd ();

	//Start Roll event
	void PushStart ();
	
	//End Roll event
	void PushEnd ();

	//Start Roll event
	void GravityStart ();
	
	//End Roll event
	void GravityEnd ();
}
