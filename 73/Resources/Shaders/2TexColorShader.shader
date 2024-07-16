Shader "Custom/2TexColor"
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
             Pass { 
                    Material
                    {
                        Diffuse [_Color]  //(4,4,0,1)
                        Ambient [_Color]  //(4,4,4,1)
                    }
                    Lighting On


                 Blend SrcAlpha OneMinusSrcAlpha 
                
                 SetTexture [_MainTex] 
                 { 
                          Combine texture 
                 } 
                                                                       
                 SetTexture [_SubTex] 
                 { 
                        Combine texture lerp(texture) previous
                       
                 }

                 SetTexture [_SubTex] 
                 { 
						Combine previous * primary DOUBLE
					   
				 }

             } 
         } 
    }
