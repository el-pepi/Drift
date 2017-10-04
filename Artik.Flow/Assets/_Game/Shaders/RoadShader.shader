Shader "Pepi/RoadShader"
{
	Properties
	{
		_MainTex ("Lines", 2D) = "white" {}
        _Glow ("Glow", 2D) = "white" {}
        _Alpha ("Alpha", 2D) = "white" {}
        _Reflex ("Reflex", 2D) = "white" {}
       // _ReflexReal ("ReflexReal", 2D) = "black" {}
    	_Color ("Color", Color) = (1,1,0,1)
	}
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 150

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha 

		Pass
		{
			Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
                float4 scrPos : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _Glow;
			sampler2D _Alpha;
			sampler2D _Reflex;
			//sampler2D _ReflexReal;
			fixed4 _MainTex_ST;
			fixed4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);

				//float4 vv = mul( _Object2World, v.vertex );
	            //vv.xyz -= _WorldSpaceCameraPos.xyz;
	          	//vv = float4( 0.0f, (o.vertex.z * o.vertex.z) * - 0.001, 0.0f, 0.0f );
	 			//o.vertex += mul(_World2Object,vv);



				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.scrPos = ComputeScreenPos(o.vertex);

 
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) + tex2D(_Glow, i.uv) * _Color + tex2D(_Reflex, i.scrPos.xy/i.scrPos.w) * _Color;// +  tex2D(_ReflexReal, i.scrPos.xy/i.scrPos.w) * 0.5;
				col.a =  tex2D(_Alpha, i.uv).r;

				return col;
			}
			ENDCG
		}
	}
}
