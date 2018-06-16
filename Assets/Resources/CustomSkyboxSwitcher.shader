Shader "Custom/SkyboxSwitcher"
{
    HLSLINCLUDE

        #include "Assets/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        TEXTURE2D_SAMPLER2D(_Skybox, sampler_Skybox);
        float _Opacity;
        
        struct fragOut
        {
            half4 color: COLOR;
            float depth: DEPTH;
        };

        fragOut Frag(VaryingsDefault i) : SV_Target
        {
            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
            float4 color2 = SAMPLE_TEXTURE2D(_Skybox, sampler_MainTex, i.texcoord);
            if (color.a == 0)
					color = color2;
				color = lerp(color, color2, _Opacity);
            fragOut o;
            o.color = color;
            o.depth = color.a;
            return o;
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite On ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}