Shader "Hidden/Shader/WarpEffect_RLPRO"
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
	float2 warp = float2(1.0 / 32.0, 1.0 / 24.0);
	float scale;
	float fade;
    half clamp;
	float2 Warp(float2 pos)
	{
		float2 h = pos - float2(0.5, 0.5);
		float r2 = dot(h, h);
		float f = 1.0 + r2 * (warp.x + warp.y * sqrt(r2));
		return f * scale * h + 0.5;
	}
	float2 Warp1(float2 pos)
	{
		pos = pos * 2.0 - 1.0;
		pos *= float2(1.0 + (pos.y * pos.y) * warp.x, 1.0 + (pos.x * pos.x) * warp.y);
		return pos * scale + 0.5;
	}

    // List of properties to control your post process effect
    float _Intensity;
    TEXTURE2D_X(_InputTexture);

		float4 Frag0(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		float4 col = LOAD_TEXTURE2D_X(_InputTexture,i.texcoord * _ScreenSize.xy);
		float2 fragCoord = i.texcoord.xy * _ScreenParams.xy;
		float2 pos = Warp1(fragCoord.xy / _ScreenParams.xy);
        float4 col2;
        if(clamp>0)
            col2 = SAMPLE_TEXTURE2D_X_LOD(_InputTexture, s_point_clamp_sampler, pos , 0.0);
        else
            col2 = LOAD_TEXTURE2D_X(_InputTexture, pos * _ScreenSize.xy);
            
		return lerp(col,col2,fade);
	}

		float4 Frag(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		float4 col = LOAD_TEXTURE2D_X(_InputTexture,i.texcoord * _ScreenSize.xy);
		float2 fragCoord = i.texcoord.xy * _ScreenParams.xy;
		float2 pos = Warp(fragCoord.xy / _ScreenParams.xy);
        float4 col2;
        if (clamp > 0)
            col2 = SAMPLE_TEXTURE2D_X_LOD(_InputTexture, s_point_clamp_sampler, pos, 0.0);
        else
            col2 = LOAD_TEXTURE2D_X(_InputTexture, pos * _ScreenSize.xy);
		return lerp(col,col2,fade);
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
                #pragma fragment Frag0
                #pragma vertex Vert
            ENDHLSL
        }
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