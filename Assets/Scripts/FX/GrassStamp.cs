using System.Collections;
using System.Collections.Generic;
using Search_Shell.Grid;
using UnityEngine;

[RequireComponent(typeof(GridObject))]
public class GrassStamp : MonoBehaviour, ISoundEvent
{

	private void Stamp(){
		List<Vector3> positions = GetComponent<GridObject>().GetVolumePositions();
		RaycastHit hit;
		foreach(Vector3 pos in positions){

			if(Physics.Raycast(transform.parent.position + pos, Vector3.down, out hit, 1f)){
				GrassFX grass = hit.collider.GetComponent<GrassFX>();
				if(grass == null)
					continue;
				
				grass.Paint(hit.point);
			}
		}
	}
  public void GravityEnd(SurfaceType surface)
  {
		Stamp();
  }

  public void GravityStart(SurfaceType surface)
  {
  }

  public void JumpEnd(SurfaceType surface)
  {
		Stamp();
  }

  public void JumpStart(SurfaceType surface)
  {
  }

  public void PushEnd(SurfaceType surface)
  {
		Stamp();
  }

  public void PushStart(SurfaceType surface)
  {
  }

  public void RollEnd(SurfaceType surface)
  {
		Stamp();
  }

  public void RollStart(SurfaceType surface)
  {
  }
}
