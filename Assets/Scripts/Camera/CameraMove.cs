using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {


    Vector2 mouse = Vector2.zero;

    public float scale = 10;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Vector3 movement = transform.forward * input.y
                          + transform.right * input.x;

        if (Input.GetKey(KeyCode.Space))
            movement += Vector3.up;
        else if(Input.GetKey(KeyCode.LeftShift))
            movement -= Vector3.up;

        transform.position += movement * Time.deltaTime * scale;

        mouse.x += Input.GetAxisRaw("Mouse X");
        mouse.y -= Input.GetAxisRaw("Mouse Y");

        mouse.y = Mathf.Clamp(mouse.y, -90, 90);

        transform.eulerAngles = new Vector3(mouse.y, mouse.x, 0);

		
	}
}
