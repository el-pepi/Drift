Shader "Unlit/NewCarShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ReflexTex ("Reflex texture", 2D) = "white" {}
		_ShadingTex ("Shading texture", 2D) = "white" {}
		_Emission ("Emission texture", 2D) = "black" {}
		_RimSize("RimLight size",Range(0,1)) = 0
		_RimColor("RimLight color",Color) = (1,1,1,1)
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
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 sphereUv : TEXCOORD1;
				float4 vertex : SV_POSITION;
				float4 shadow : COLOR1;
			};

			sampler2D _MainTex;
			sampler2D _ReflexTex;
			sampler2D _ShadingTex;
			sampler2D _Emission;
			float4 _MainTex_ST;
			float4 _RimColor;

			float _RimSize;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.shadow = saturate(1-dot(UnityObjectToWorldNormal(v.normal),normalize(WorldSpaceViewDir(v.vertex)))-_RimSize) * _RimColor;

				float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
	            float3 r = reflect(-viewDir, v.normal);
	            r = mul((float3x3)UNITY_MATRIX_MV, r);
	            r.z += 1;
	            float m = 2 * length(r);
	            o.sphereUv = r.xy / m + 0.5;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = lerp(tex2D(_ShadingTex, i.sphereUv),1,tex2D(_Emission, i.uv).r); 
				col *= tex2D(_MainTex, i.uv); 
				//fixed4 col = tex2D(_ReflexTex,i.sphereUv);
				col += tex2D(_ReflexTex,i.sphereUv);
				//col *= tex2D(_MainTex, i.uv);
				col += (i.shadow) * 3;
				return col;
			}
			ENDCG
		}
	}
}
