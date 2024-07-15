Shader "Custom/TextureAndColor"
{
    Properties 
   { 
        _MyColor ("Main Color", COLOR) = (0,0,1,1) 
        _MainTex ("Base Texture", 2D) = "white" { }
   } 


    
    SubShader
    {
     Pass
     {
           Material
           {
			   Diffuse[_MyColor]
               Ambient[_MyColor]
           }
           Lighting On
           SetTexture [_MainTex] {  Combine texture * primary DOUBLE }
		
     } 
    }

    FallBack "Diffuse"
}
