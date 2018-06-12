Shader "Unlit/Stamper"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Coord ("Coordinate", Vector) = (1,1,1,1)
		_Distance ("Distance", Vector) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Coord;
			float4 _Distance;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				float distX = 1 - abs(i.uv.x - _Coord.x);
				float distY = 1 - abs(i.uv.y - _Coord.y);
				float2 distances = abs(i.uv - _Coord.xy);
				distances.x *= _Coord.w;
				distances.y *= _Coord.z;

				distances = saturate((_Distance.x - distances)/_Distance.x);

				float draw = saturate(distances.x * distances.y);
				draw = pow(draw*1, 1);
				//return float4(0,0,0,0);
				// float draw = pow(saturate(1 - distance(i.uv, _Coord.xy)), _Distance);
				return saturate(col + draw*(float4(1,0,0,1)));
			}
			ENDCG
		}
	}
}
