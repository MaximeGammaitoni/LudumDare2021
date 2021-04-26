Shader "Hidden/Shader/UltimateVignetteEffect_RLPRO"
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
	half4 _Params;
	half3 _InnerColor;
	half4 _Center;
#pragma shader_feature VIGNETTE_CIRCLE
#pragma shader_feature VIGNETTE_SQUARE
#pragma shader_feature VIGNETTE_ROUNDEDCORNERS
	half2 _Params1;

    // List of properties to control your post process effect
    float _Intensity;
    TEXTURE2D_X(_InputTexture);

		float4 Frag(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
	half4 color = LOAD_TEXTURE2D_X(_InputTexture, i.texcoord * _ScreenSize.xy);

#if VIGNETTE_CIRCLE
	half d = distance(i.texcoord, _Center.xy);
	half multiplier = smoothstep(0.8, _Params.x * 0.799, d * (_Params.y + _Params.x));
#elif VIGNETTE_ROUNDEDCORNERS
	half2 uv = -i.texcoord * i.texcoord + i.texcoord;
	half v = saturate(uv.x * uv.y * _Params1.x + _Params1.y);
	half multiplier = smoothstep(0.8, _Params.x * 0.799, v * (_Params.y + _Params.x));
#else
	half multiplier = 1.0;
#endif
	_InnerColor = -_InnerColor;
	color.rgb = (color.rgb - _InnerColor) * max((1.0 - _Params.z * (multiplier - 1.0) - _Params.w), 1.0) + _InnerColor;
	color.rgb *= multiplier;

	return color;
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