Shader "Custom/Material_Shader"
{
   Properties 
   { 
        _MyColor ("Main Color", COLOR) = (0,0,1,1) 
   } 


    
    SubShader
    {
     Pass{
           Material{
			   Diffuse[_MyColor]
               Ambient[_MyColor]
               }
           Lighting On
         } 
    }



    FallBack "Diffuse"
}
