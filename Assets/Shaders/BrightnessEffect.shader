Shader "Hidden/Brightness Effect" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	    _Brightness("Brightness", Float) = 1.0
	}

	SubShader {
		Pass {
			ZTest Always Cull Off ZWrite Off
				
	        CGPROGRAM
	        #pragma vertex vert_img
	        #pragma fragment frag
	        #include "UnityCG.cginc"

	        uniform sampler2D _MainTex;
            float _Brightness;

	        fixed4 frag (v2f_img i) : SV_Target {
		        return tex2D(_MainTex, i.uv) * _Brightness;
	        }
	        ENDCG
		}
	}

	Fallback off
}
