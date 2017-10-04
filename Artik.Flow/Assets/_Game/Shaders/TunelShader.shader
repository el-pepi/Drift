Shader "Unlit/TunelShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ColorTex ("Color Texture", 2D) = "white" {}
		_WhiteTex ("White Texture", 2D) = "black" {}
		_LinesTex ("Lines Texture", 2D) = "black" {}
		_BackColor("Back color",Color) = (0,0,0,1)
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
				float2 uv2 : TEXCOORD1;
				float2 uv3 : TEXCOORD3;
				float2 uv4 : TEXCOORD4;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _ColorTex;
			sampler2D _WhiteTex;
			sampler2D _LinesTex;
			float4 _MainTex_ST;
			float4 _ColorTex_ST;
			float4 _WhiteTex_ST;
			float4 _LinesTex_ST;
			float4 _BackColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv.y -= _Time.b;
				o.uv2 = TRANSFORM_TEX(v.uv, _ColorTex);
				o.uv2.y =  o.uv.y/32;

				o.uv3 = TRANSFORM_TEX(v.uv, _WhiteTex);
				o.uv3.y  = o.uv.y;

				o.uv4 = TRANSFORM_TEX(v.uv, _LinesTex);
				o.uv4.y -=  _Time.a * 1;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = lerp(_BackColor, tex2D(_ColorTex, i.uv2),tex2D(_MainTex, i.uv).b) + tex2D(_WhiteTex, i.uv3) + tex2D(_LinesTex,i.uv4);

				// apply fog
				return col;
			}
			ENDCG
		}
	}
}
