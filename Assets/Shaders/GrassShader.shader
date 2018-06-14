Shader "Custom/GrassShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Color2 ("Color2", Color) = (1,1,1,1)
		_Height ("Height", float) = 1.0
		_MaxHeight ("MaxHeight", float) = 1.0
		_Bounds ("Bounds", vector) = (1,1,1,1) 
		_Wind ("Wind", float) = 1.0
		_Map("Map", 2D) = "white" {}
		_MainTex ("DepthMap", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull Off

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert addshadow

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _Map;

		struct Input {
			float2 uv_MainTex;
			float3 pos;
		};

		half _Glossiness;
		half _Metallic;
		half _Height;
		half _MaxHeight;
		half _Wind;
		fixed4 _Color;
		fixed4 _Color2;
		fixed4 _Bounds;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		float2 getPosition(float3 position){
			float2 pos = position.xz - _Bounds.xy;
			float2 dist = _Bounds.zw - _Bounds.xy;
			pos.x /=dist.x;
			pos.y /=dist.y;

			return pos;
		}

		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
			float xx = cos(_Time.w + v.vertex.x);
			float yy = cos(_Time.w + v.vertex.y);
			float zz = cos(_Time.w + v.vertex.z);
			float2 pos = getPosition(v.vertex.xyz);
			float4 colors = tex2Dlod(_Map, float4( pos ,0,0));
			float height = colors.r * _MaxHeight;
			o.pos = v.vertex;
			v.vertex.y -= height * v.color.r + height;
			v.vertex.xyz += float3(xx, 00, zz) * (1.0 - colors.r) * _Wind *  v.color.r;

		}


		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			c = tex2D(_Map, getPosition(IN.pos));
			o.Albedo = c.rgb;
			o.Albedo = lerp(_Color, _Color2, c.rgb.r);
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
