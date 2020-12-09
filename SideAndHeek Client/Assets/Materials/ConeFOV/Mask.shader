﻿Shader "Custom/Mask" {
	SubShader{
		Tags { "RenderType" = "Opaque" "Queue" = "Geometry"}
		Pass {
			//ColorMask 0
			//ZWrite off
			Stencil {
				Ref 1
				Comp equal
				Pass keep
 			}

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			struct appdata {
				float4 vertex : POSITION;
			};
			struct v2f {
				float4 pos : SV_POSITION;
			};
			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}
			half4 frag(v2f i) : SV_Target {
				return half4(0,0,1,1);
			}
			ENDCG
		}
	}
}
