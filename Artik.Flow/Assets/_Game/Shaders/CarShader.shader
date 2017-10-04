Shader "Pepi/Car" 
 {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
     // _Reflex ("Reflex", 2D) = "white" {}
      _Cube ("Cubemap", CUBE) = "" {}
	  _MountainDensity("ReflexDensity",Float) = 0
    }

    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf Lambert nofog noshadow nolightmap nometa noforwardadd novertexlights  //vertex:vert

      struct Input {
          float2 uv_MainTex;
          fixed3 worldRefl;
         // float3 customColor;

          fixed2 wuv;
      };

      void vert (inout appdata_full v, out Input o) {
         	UNITY_INITIALIZE_OUTPUT(Input,o);
         //	o.wuv = mul(unity_ObjectToWorld, v.vertex).rb * _MountainDensity;
      }

      sampler2D _MainTex;
     // sampler2D _Reflex;
      samplerCUBE _Cube;

      void surf (Input IN, inout SurfaceOutput o) {
          o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * 0.5;
          o.Emission = texCUBE (_Cube, IN.worldRefl).rgb; //+ tex2D(_Reflex, IN.wuv);
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }