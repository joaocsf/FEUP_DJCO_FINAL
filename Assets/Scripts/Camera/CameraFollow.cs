using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {



	 public float sensitivity = 3;

    public float radius = 3;

    public Vector2 mouse;

    private Vector3 cameraPos;
    public Transform obj;


	void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
		
	}

	public void SetTransform(Transform obj){
		this.obj = obj;
	}

	void Update () {

		mouse.x += Input.GetAxis("Mouse X")  * sensitivity;
		mouse.y -= Input.GetAxis("Mouse Y") * sensitivity;

		mouse.x %= 360;
		mouse.y = Mathf.Clamp(mouse.y, -80, 70);

		radius -= Input.GetAxisRaw("Mouse ScrollWheel") * 2;


		if (obj == null) return;

		PositionCamera(obj, Time.deltaTime);
		Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
	}

	public Vector3 GetPlaneDirection(Vector3 point){
		Matrix4x4 matrix = 
			Matrix4x4.Rotate(Quaternion.Euler(new Vector3(0,mouse.x,0)))
			* Matrix4x4.identity;

		return matrix.MultiplyPoint(point);
	}

	public Vector3 GetDirection()
	{
		Matrix4x4 matrix = 
			Matrix4x4.Rotate(Quaternion.Euler(new Vector3(0,mouse.x,0)))
			* Matrix4x4.Rotate(Quaternion.Euler(new Vector3(mouse.y,0,0)))
			* Matrix4x4.Translate(-Vector3.forward) 
			* Matrix4x4.identity;

		return matrix.MultiplyPoint(new Vector3(0,0,0));
	}

	private void OnDrawGizmos()
	{
		if(obj == null) return;
		Vector3 cameraDir = GetDirection();
		Gizmos.color = Color.red;
		Gizmos.DrawLine(obj.position, obj.position + cameraDir * radius);
	}

	void PositionCamera(Transform obj, float delta)
	{
		Vector3 cameraDir = GetDirection();
		RaycastHit hit;
		Vector3 point = Vector3.zero;
		if (Physics.Raycast(obj.position, cameraDir, out hit, radius)){
			cameraPos = hit.point - cameraDir.normalized;
		}else
			cameraPos = obj.position + cameraDir * radius;

		transform.position = Vector3.Lerp(transform.position, cameraPos, 10f * delta);
		transform.eulerAngles = new Vector3(mouse.y, mouse.x, 0);
	}
}
