using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(SkyboxRenderer), PostProcessEvent.BeforeStack, "Custom/SkyboxRenderer")]
public sealed class SkyboxEffect : PostProcessEffectSettings {

    [Range(0f, 1f), Tooltip("Switch Effect")]
    public FloatParameter blend = new FloatParameter{value = 1.0f};

}

public sealed class SkyboxRenderer : PostProcessEffectRenderer<SkyboxEffect>
{
    public override void Render(PostProcessRenderContext context){
        Shader s = Shader.Find("Custom/SkyboxSwitcher");
        var sheet = context.propertySheets.Get(s);
        sheet.properties.SetFloat("_Opacity", settings.blend);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
