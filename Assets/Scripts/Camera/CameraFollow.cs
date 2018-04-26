using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {



	 public float sensitivity = 3;

	 public float angle = 45f;

    public float radius = 3;

    public Vector2 mouse;

    public Vector2 tempMousePos;

    private Vector3 cameraPos;
    public Transform obj;

		public float lerpVelocity = 10;

		public bool collide = false;

		public Vector2 lastPressed = Vector2.zero;

	void Start () {
		
	}

	public void SetTransform(Transform obj){
		this.obj = obj;
	}

	

	void Update () {
		if(Input.GetKeyDown(KeyCode.Q))
			lastPressed.x = 1;
		else if(Input.GetKeyDown(KeyCode.E))
			lastPressed.x = -1;
		else
			lastPressed.x = 0;
		//mouse.x += Input.GetAxis("Mouse X")  * sensitivity;
		//mouse.y -= Input.GetAxis("Mouse Y") * sensitivity;
		mouse.x += lastPressed.x * angle;
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

	public void ForcePositionCamera(Vector3 origin, float delta){
		Vector3 cameraDir = GetDirection();
		RaycastHit hit;
		Vector3 point = Vector3.zero;
		if (Physics.Raycast(origin, cameraDir, out hit, radius) && collide){
			cameraPos = hit.point - cameraDir.normalized;
		}else
			cameraPos = origin + cameraDir * radius;

		transform.position = Vector3.Lerp(transform.position, cameraPos, delta * lerpVelocity);
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(mouse.y, mouse.x, 0)), delta*lerpVelocity);
	}

	void PositionCamera(Transform obj, float delta)
	{
		ForcePositionCamera(obj.position, delta);
	}
}
