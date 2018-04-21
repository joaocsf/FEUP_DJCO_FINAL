using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxHandler : MonoBehaviour {

    public Material mat;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, mat);
        
    }

    public void SetOpacity(float f){
        mat.SetFloat("_Opacity", f);
    }

}
