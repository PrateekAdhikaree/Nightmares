Shader "Custom/Rim Lit Bumped Specular Dissolve" {

	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_ActualMainTex("Main Texture", 2D) = "white"{}
		_MainTex("Dissolve Shape", 2D) = "white"{}
		_Cutoff("Dissolve Value", Range(0, 1)) = 0
		_LineWidth("Line Width", Range(0, 0.2)) = 0
		_LineColor("Line Color", Color) = (1, 1, 1, 1)
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range(0.03,1)) = 0.078125
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_RimColor ("Rim Color", Color) = (0.2, 0.2, 0.2, 0.0)
		_RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0
	}
	
	SubShader {
		Tags {"Queue" = "AlphaTest" "RenderType" = "TransparentCutout"}
		
		CGPROGRAM
		#pragma surface surf BlinnPhong
		#pragma target 3.0
		
		sampler2D _ActualMainTex;
		sampler2D _MainTex;
		fixed _Cutoff;
		fixed4 _LineColor;
		float _LineWidth;
		fixed4 SpecColor;
		half _Shininess;
		sampler2D _BumpMap;
		float4 _RimColor;
		float _RimPower;
		
		struct Input {
			float2 uv_ActualMainTex;
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_ActualMainTex, IN.uv_ActualMainTex);  
			o.Albedo = tex.rgb;
			o.Gloss = tex.a;

			o.Specular = _Shininess;
			o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
			half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
			//o.Emission = _RimColor.rgb * pow (rim, _RimPower);

			half4 dissolve = tex2D(_MainTex, IN.uv_MainTex);
			int isAtLeastLine = int(1 - (dissolve.a - _Cutoff) + _LineWidth);
			o.Albedo = lerp(o.Albedo, _LineColor, isAtLeastLine);
            
            o.Emission = _RimColor.rgb * pow(rim, _RimPower) + lerp(half4(0, 0, 0, 0), _LineColor, isAtLeastLine);
		
			clip(dissolve.a - _Cutoff);
		}
		ENDCG
	}
	
	Fallback "Transparent/Cutout/VertexLit"
}
