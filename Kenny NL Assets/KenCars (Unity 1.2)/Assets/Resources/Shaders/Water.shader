Shader "Custom/SurfaceDepthTextureColorized" {
    
	Properties{
	
        _Color("Color", Color) = (1,1,1,1)
        _BlendColor("Blend Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _InvFade("Soft Factor", Range(0.01,3.0)) = 1.0
        _FadeLimit("Fade Limit", Range(0.00,1.0)) = 0.3
		
    }
	
	SubShader{
	
        Tags{ "RenderType" = "Opaque" }
        
		LOD 200

        CGPROGRAM
        
		#pragma surface surf Standard vertex:vert nolightmap
		#pragma target 3.0

        sampler2D _MainTex;

    struct Input {
        float2 uv_MainTex;
        float4 screenPos;
        float eyeDepth;
    };

    half _Glossiness;
    half _Metallic;
    fixed4 _Color;
    fixed4 _BlendColor;
    sampler2D_float _CameraDepthTexture;
    float4 _CameraDepthTexture_TexelSize;

    float _FadeLimit;
    float _InvFade;

    void vert(inout appdata_full v, out Input o)
    {
        UNITY_INITIALIZE_OUTPUT(Input, o);
        COMPUTE_EYEDEPTH(o.eyeDepth);
    }

    void surf(Input IN, inout SurfaceOutputStandard o) {
        // Albedo comes from a texture tinted by color
        fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
        o.Albedo = c.rgb;
        // Metallic and smoothness come from slider variables
        o.Metallic = _Metallic;
        o.Smoothness = _Glossiness;

        float rawZ = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos));
        float sceneZ = LinearEyeDepth(rawZ);
        float partZ = IN.eyeDepth;

        float fade = 1.0;
        if (rawZ > 0.0) // Make sure the depth texture exists
            fade = saturate(_InvFade * (sceneZ - partZ));
        //o.Alpha = c.a * fade; //(original line)
        //the rest are lines I've input
        o.Alpha = 1;
        if(fade<_FadeLimit)
        o.Albedo = c.rgb * fade + _BlendColor * (1 - fade);
    }
    ENDCG
    }
}