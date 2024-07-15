Shader "Custom/2TextureShader"
{
    Properties
    {
        _Color ("Main Color", COLOR) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {} 
        _SubTex("Texture", 2D) = "white" {}
        
    }
    SubShader
    {
        Tags{ "Queue" = "Transparent" }

           Pass { 
               Blend SrcAlpha OneMinusSrcAlpha
               SetTexture [_MainTex] 
               { 
                 Combine texture //DOUBLE 
               } 
                                                 
               SetTexture [_SubTex] 
               { 
                   constantColor [_Color]
                 Combine texture lerp(texture) previous , constant
               } 
           } 
    }

    FallBack "Diffuse"
}
