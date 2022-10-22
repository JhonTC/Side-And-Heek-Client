Shader "Custom/Globdule"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        //_Glossiness("Smoothness", Range(0,1)) = 0.5
        //_Metallic("Metallic", Range(0,1)) = 0.0
        _Strength("Strength", Range(0,20)) = 1
        _Speed("Speed", Range(0,10)) = 1
        _Dampening("Dampening", Range(0,1)) = 1
        _Random("Random", Range(0,1000)) = 0
    }

    SubShader 
    {
        Tags { "RenderType" = "Opaque"}

        Pass 
        {
            Tags {"LightMode" = "ForwardBase"}

            CGPROGRAM

            #pragma vertex vertexFunc
            #pragma fragment fragmentFunc

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 _Color;
            sampler2D _MainTex;
            float _Strength;
            float _Speed;
            float _Dampening;
            float _Random;

            float random(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453123);
            }

            v2f vertexFunc(appdata IN)
            {
                v2f OUT;

                IN.vertex.x += sin((_Time.y + _Random) * _Speed + ((IN.vertex.y) / 2) * _Strength) * _Dampening;
                IN.vertex.y += sin((_Time.y + _Random) * _Speed + ((IN.vertex.z) / 2) * _Strength) * _Dampening;
                IN.vertex.z += sin((_Time.y + _Random) * _Speed + ((IN.vertex.x) / 2) * _Strength) * _Dampening;

                OUT.position = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.uv;

                return OUT;
            }

            fixed4 fragmentFunc(v2f IN) : SV_Target
            {
                fixed4 pixelColor = tex2D(_MainTex, IN.uv);

                return pixelColor * _Color;
            }

            ENDCG
        }
    }
}
