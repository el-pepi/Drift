Shader "Pepi/GridShader"
{
	Properties
	{
		_MainTex ("Grid", 2D) = "white" {}
		_Glow ("Glow", 2D) = "white" {}
        _Reflex ("Reflex", 2D) = "white" {}
        //_HeightMap ("HeightMap", 2D) = "white" {}
        _Wave ("Wave", 2D) = "white" {}
		_OffsetX ("OffsetX", Float ) = 0
		_OffsetZ ("OffsetZ", Float ) = 0
        _MountainDensity ("MountainDensity", Float ) = 0
        _MountainHeight ("MountainHeight", Float ) = 1
        _WaveSpeed ("WaveSpeed", Float ) = 1

    	_Color ("Color", Color) = (1,1,0,1)
	}
	SubShader
	{
		Tags {"RenderType"="Opaque"}
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma glsl
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
                float2 wavePos : TEXCOORD1;
                float4 scrPos : TEXCOORD2;
				float4 vertex : SV_POSITION;

                UNITY_FOG_COORDS(3)
			};

			float4 _MainTex_ST;
			//float4 _HeightMap_ST;
			float4 _Wave_ST;
			fixed4 _Color;
			sampler2D _MainTex;
			sampler2D _Glow;
			sampler2D _Reflex;
			//sampler2D _HeightMap;
			sampler2D _Wave;
			uniform half _OffsetX;
			uniform half _OffsetZ;
            uniform half _MountainDensity;
            uniform half _MountainHeight;
            uniform half _WaveSpeed;

		    uniform half4 unity_FogStart;
		    uniform half4 unity_FogEnd;

            float rand(float2 c){
			    return frac(sin(dot(c.xy ,float2(12.9898,78.233))) * 43758.5453);
			}

			float noise(float2 p, float freq ){
			    float unit = 512/freq;
			    float2 ij = floor(p/unit);
			    float2 xy = fmod(p,unit)/unit;
			    //xy = 3.*xy*xy-2.*xy*xy*xy;
			    xy = .5*(1.-cos(3.14159*xy));
			    float a = rand((ij+float2(0.,0.)));
			    float b = rand((ij+float2(1.,0.)));
			    float c = rand((ij+float2(0.,1.)));
			    float d = rand((ij+float2(1.,1.)));
			    float x1 = lerp(a, b, xy.x);
			    float x2 = lerp(c, d, xy.x);
			    return lerp(x1, x2, xy.y);
			}

			float pNoise(float2 p, int res){
			    float persistance = .5;
			    float n = 0.;
			    float normK = 0.;
			    float f = 4.;
			    float amp = 1.;
			    int iCount = 0;
			    for (int i = 0; i<50; i++){
			        n+=amp*noise(p, f);
			        f*=2.;
			        normK+=amp;
			        amp*=persistance;
			        if (iCount == res) break;
			        iCount++;
			    }
			    float nf = n/normK;
			    return nf*nf*nf*nf;
			}

			//float rand(float3 co)
 			//{
    	 	//	return frac(sin( dot(co.xz ,float2(1.98,7.23) )) * 4.55);
    	 		//return frac(sin( dot(co.xz ,float3(12.9898,78.233,45.5432) )) * 43758.5453);
			//}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);

				v.vertex.y += pNoise(mul(unity_ObjectToWorld, v.vertex).xz *1.2 + half2(2000 + (_OffsetX * 1.2), 1000 + (_OffsetZ * 1.2)),_MountainDensity)*_MountainHeight;

				//v.vertex.y += rand(mul(unity_ObjectToWorld, v.vertex)) * _MountainHeight; 

                //v.vertex.y += ( tex2Dlod(_HeightMap,float4(TRANSFORM_TEX((mul(unity_ObjectToWorld, v.vertex).rb*_MountainDensity), _HeightMap),0.0,0)).rgb*_MountainHeight).y;
                //o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex );

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.scrPos = ComputeScreenPos(o.vertex);

				o.wavePos = TRANSFORM_TEX(v.uv, _Wave);
				o.wavePos.y += _Time.y * _WaveSpeed;


			    //float pos = length(UnityObjectToViewPos(v.vertex).xyz);
			    //float diff = unity_FogEnd.x - unity_FogStart.x;
			    //float invDiff = 1.0f / diff;
			    //o.fog = clamp ((unity_FogEnd.x - pos) * invDiff, 0.0, 1.0);

                UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv) + tex2D(_Glow, i.uv) * _Color + tex2D(_Reflex, i.scrPos.xy/i.scrPos.w) * _Color + tex2D(_MainTex, i.uv) *  tex2D(_Wave, i.wavePos) ;

                //fixed4 color = fixed4(i.texcoord.xy,0,0);
                UNITY_APPLY_FOG(i.fogCoord, col); 

				return col;
			}
			ENDCG
		}
	}
}
