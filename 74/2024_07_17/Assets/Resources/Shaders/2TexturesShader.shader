Shader "Custom/2Textures"
{
Properties 
{ 
        _Color ("Main Color", COLOR) = (1,1,1,1) 
        _MainTex("Texture", 2D) = "white" {} 
        _SubTex("Texture", 2D) = "white" {} 
} 
    SubShader 
    { 
        Tags { "Queue" = "Transparent" }

             Pass {  Blend SrcAlpha OneMinusSrcAlpha 
                
                 SetTexture [_MainTex] 
                 { 
                          Combine texture 
                 } 
                                                                       
                 SetTexture [_SubTex] 
                 { 
                        ConstantColor [_Color]
                        Combine texture lerp(texture) previous , constant
				 }

             } 
         } 
    }
