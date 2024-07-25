// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Simplified Bumped shader. Differences from regular Bumped one:
// - no Main Color
// - Normalmap uses Tiling/Offset of the Base texture
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Custom/Silhouette Bumped Diffuse" {
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    [NoScaleOffset] _BumpMap ("Normalmap", 2D) = "bump" {}
}

SubShader {

//--- ù��° �׸���
    Tags { "RenderType"="Opaque" }

    Zwrite Off
    ZTest Always
    Lighting Off

    LOD 250

CGPROGRAM
#pragma surface surf Lambert noforwardadd

sampler2D _MainTex;
sampler2D _BumpMap;

struct Input {
    float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
    //fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
    //o.Albedo = c.rgb;
    //o.Alpha = c.a;
    //o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
    o.Emission = float4(0.58, 1.0, 0.6, 1.0);
}
ENDCG
//--- ù��° �׸���

//--- �ι�° �׸���
    Tags { "RenderType"="Opaque" }
    LOD 250

    //--- �⺻���� ���� ���� �׸���..
    Zwrite On
    ZTest LEqual
    Lighting On   
    //--- �⺻���� ���� ���� �׸���..

CGPROGRAM
#pragma surface surf Lambert noforwardadd

sampler2D _MainTex;
sampler2D _BumpMap;

struct Input {
    float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
    o.Albedo = c.rgb;
    o.Alpha = c.a;
    o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
}
ENDCG
//--- �ι�° �׸���
}

FallBack "Mobile/Diffuse"
}