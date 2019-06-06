Shader "Custom/Solid Color" {

	Properties {
	
		_Color ("Main Color", Color) = (1,1,1,0.5)
		
	}

    SubShader {
        Pass {
			
            Tags { "LightMode" = "ForwardBase" }
         
            CGPROGRAM
 
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            #pragma multi_compile_fwdbase
 
            #include "AutoLight.cginc"
 
			fixed4 _Color;
			fixed4 _ShadowColor;
 
            struct v2f{
                float4 pos : SV_POSITION;
				fixed3 ambient : COLOR1;
                LIGHTING_COORDS(0,1)
            };
 
 
            v2f vert(appdata_base v){
			
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
				
            }
 
            fixed4 frag(v2f i):COLOR{
			
                fixed shadow = SHADOW_ATTENUATION(i);
				return _Color * shadow;
				
            }
 
            ENDCG
        }
    }
	
    Fallback "VertexLit"
	
}