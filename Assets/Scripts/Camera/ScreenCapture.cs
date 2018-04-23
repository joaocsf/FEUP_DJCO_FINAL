using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCapture : MonoBehaviour {

        Camera cam;
        public Camera cam2;
        public float scale = 10;
        // Use this for initialization
        void Start () {

        cam = GetComponent<Camera>();
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 0);
        Shader.SetGlobalTexture("_Skybox", rt);
        cam.targetTexture = rt;
        }

        void LateUpdate () {
        if(cam.transform.parent == null) return;
        cam.transform.localPosition = cam2.transform.localPosition / Mathf.Clamp(scale,1, Mathf.Infinity);
        cam.transform.localEulerAngles = cam2.transform.localEulerAngles;
        }
}
