Shader "Custom/SelectedShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Bounds ("Bounds", Vector) = (1,1,1,1)
		_SelectionThreshold("Selection Threshold", Range(0,1)) = 1
		_SelectionColor ("Selection Color", Color) = (1,1,1,1)
		_SelectionRadius ("Selection Radius", Range(0,1)) = 1
		_SelectionTexture("Selection Mask", 2D) = "white" {}
		_SelectionScale("Scale", Range(0,1)) = 1
		_OverLay("Overlay", 2D) = "white" {}
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _SelectionTexture;
		sampler2D _OverLay;

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
			float3 worldNormal;
			float4 screenPos;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		float _SelectionRadius;
		float _SelectionThreshold;
		float _SelectionScale;
		fixed4 _SelectionColor;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			float f = dot(IN.viewDir, IN.worldNormal);
			float2 screenUV = IN.screenPos.xy/ IN.screenPos.w;

			screenUV = float2(IN.worldPos.x + IN.worldPos.z, IN.worldPos.y);
			screenUV = IN.worldPos.xz + IN.worldPos.yz + IN.worldPos.yx;
			screenUV.y /=0.5;
			screenUV.x /= 0.5;
			screenUV.x *= 1;
			screenUV.y *= 1;
			
			fixed4 overlay = tex2D (_OverLay, screenUV * _SelectionScale + float2(_CosTime.y - _Time.y, 0) * 0.1);
			f = clamp(f * _SelectionRadius , 0, 1);
			f = 1 - tex2D(_SelectionTexture, float3(f,0,0)).r;
			f += overlay.r;
			f = 1 - clamp(f, 0, 1);
			f *= _SelectionThreshold;
			o.Albedo = lerp(c.rgb, _SelectionColor, f);
			o.Metallic = lerp(_Metallic, 0, f);
			o.Smoothness = lerp(_Glossiness, 0, f);
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
