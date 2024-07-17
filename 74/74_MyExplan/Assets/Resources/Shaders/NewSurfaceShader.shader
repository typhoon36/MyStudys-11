Shader "Custom/TwoTextureColorShader"
{
    Properties
    {
        _Color ("First Texture Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _SecondColor ("Second Texture Color", Color) = (1,1,1,1)
        _SecondTex ("Second Texture (RGB)", 2D) = "white" {}
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _SecondTex;
            float4 _SecondTex_ST;
            float4 _Color;
            float4 _SecondColor;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord) * _Color; // 첫 번째 텍스처에 첫 번째 색상 적용
                fixed4 col2 = tex2D(_SecondTex, i.texcoord) * _SecondColor; // 두 번째 텍스처에 두 번째 색상 적용
                return lerp(col, col2, 0.5); // 두 텍스처 혼합
            }

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
