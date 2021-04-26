Shader "Hidden/Shader/BottomNoiseEffect_RLPRO"
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
		float2 texcoordStereo   : TEXCOORD1;
        UNITY_VERTEX_OUTPUT_STEREO
    };
	
	TEXTURE2D_X(_InputTexture);
	SAMPLER(_NoiseTexture);
	float _Intensity;
	float _OffsetNoiseX;
	float _OffsetNoiseY;
	half _NoiseBottomHeight;
	half _NoiseBottomIntensity;
	half tileX = 0;
	half tileY = 0;

    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
		output.texcoordStereo = ClampAndScaleUV(output.texcoord + float2(_OffsetNoiseX - 0.2f, _OffsetNoiseY), _ScreenSize.zw * float2(tileY, tileX), 1.0);
		output.texcoordStereo *= float2(tileY, tileX);
        return output;
    }

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        uint2 positionSS = input.texcoord * _ScreenSize.xy;
        float4 outColor = LOAD_TEXTURE2D_X(_InputTexture, positionSS);
		half2 uv = input.texcoord;		
		float condition = saturate(floor(_NoiseBottomHeight / uv.y));
		float4 noise_bottom = tex2D(_NoiseTexture, input.texcoordStereo) * condition * _NoiseBottomIntensity;
		outColor = lerp(outColor, noise_bottom, -noise_bottom * ((uv.y / _NoiseBottomHeight) - 1.0));		
		return float4(pow(outColor.xyz, float3(1.0, 1.0, 1.0)), outColor.a);
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
                #pragma fragment CustomPostProcess
                #pragma vertex Vert
            ENDHLSL
        }
    }
    Fallback Off
}