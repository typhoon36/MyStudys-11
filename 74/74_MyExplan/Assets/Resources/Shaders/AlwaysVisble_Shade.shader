Shader "Custom/AlwaysVisible"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("AlwaysVisible Color", Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 100

        pass
        {
            Cull Back
            ZWrite Off  // 이 객체의 픽셀이 깊이 버퍼에 쓰여지는지를 제어한다
            //(기본값은 켜짐). 고체 객체를 그릴 때는 이 옵션을 켜두세요.
            // 반투명 효과를 그릴 때는 ZWrite를 Off로 전환하세요.<--반투명이라 꺼준것.

            ZTest Always

                 CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
			{
				float4 vertex : POSITION;
				
			};

            struct v2f
            {
             float4 vertex : SV_POSITION;
            };


            float4 _Color;


             v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                return _Color;
            }

            ENDCG
        }




        Pass
        {
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
