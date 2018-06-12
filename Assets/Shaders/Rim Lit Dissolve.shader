Shader "Custom/Rim Lit Dissolve" {

	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex("Dissolve Shape", 2D) = "white"{}
		_Cutoff("Dissolve Value", Range(0, 1)) = 0
		_LineWidth("Line Width", Range(0, 0.2)) = 0
		_LineColor("Line Color", Color) = (1, 1, 1, 1)
		_RimColor ("Rim Color", Color) = (0.2, 0.2, 0.2, 0.0)
		_RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0
	}
	
	SubShader {
		Tags {"Queue" = "AlphaTest" "RenderType" = "TransparentCutout"}
		
		CGPROGRAM
		#pragma surface surf BlinnPhong
		#pragma target 3.0
		
		fixed4 _Color;
		sampler2D _MainTex;
		fixed _Cutoff;
		fixed4 _LineColor;
		float _LineWidth;
		float4 _RimColor;
		float _RimPower;
		
		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = _Color.rgb;

			half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb * pow (rim, _RimPower);

			half4 dissolve = tex2D(_MainTex, IN.uv_MainTex);
			int isAtLeastLine = int(1 - (dissolve.a - _Cutoff) + _LineWidth);
			o.Albedo = lerp(o.Albedo, _LineColor, isAtLeastLine);
		
			clip(dissolve.a - _Cutoff);
		}
		ENDCG
	}
	
	Fallback "Transparent/Cutout/VertexLit"
}
