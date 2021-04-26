Shader "Hidden/Shader/NegativeEffect_RLPRO"
{
    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 ps4 xboxone vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

    struct Attributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
        return output;
    }


    // List of properties to control your post process effect
    float _Intensity;
	uniform float T;
	uniform float Luminosity;
	uniform float Vignette;
	uniform float Negative;
	uniform float Contrast;
    TEXTURE2D_X(_InputTexture);
	TEXTURE2D_X(_InputTexture2);
	float3 linearLight(float3 s, float3 d)
	{
		return 2.0 * s + d - 1.0 * Luminosity;
	}


		float4 Frag(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

	float2 uv = i.texcoord ;
	float3 col = LOAD_TEXTURE2D_X(_InputTexture, i.texcoord * _ScreenSize.xy).rgb;
	col = lerp(col, 1 - col, Negative*1.5);
	float3 oldfilm = float3(1, 1, 1);
	col *= pow(abs(0.1 * uv.x * (1.0 - uv.x) * uv.y * (1.0 - uv.y)), Contrast) * 1 + Vignette;
	col = dot(float3(0.2126, 0.7152, 0.0722), col);
	col = linearLight(oldfilm, col);
	return float4(col, 1);
	}

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "#NAME#"

            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            HLSLPROGRAM
                #pragma fragment Frag
                #pragma vertex Vert
            ENDHLSL
        }

    }
    Fallback Off
}