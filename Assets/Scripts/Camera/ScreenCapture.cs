using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCapture : MonoBehaviour {

    Camera cam;
    public int resolution = 512;
    Cubemap cubemap;
    public Camera cam2;
    public Material skybox;
    float scale = 10;
	// Use this for initialization
	void Start () {

        cam = GetComponent<Camera>();
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 0);
        Shader.SetGlobalTexture("_Skybox", rt);
        cam.targetTexture = rt;
	}

	void LateUpdate () {
        cam.transform.localPosition = cam2.transform.localPosition / scale;
        cam.transform.localEulerAngles = cam2.transform.localEulerAngles;
	}
}
