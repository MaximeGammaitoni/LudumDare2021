Shader "Hidden/Shader/PulsatingVignetteEffect_RLPRO"
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
	float vignetteAmount = 1.0;
	float vignetteSpeed = 1.0;
	float Time = 0.0;
    float _Intensity;
    TEXTURE2D_X(_InputTexture);
	float vignette(float2 uv, float t)
	{
		float vigAmt = 2.5 + 0.1 * sin(t + 5.0 * cos(t * 5.0));
		float c = (1.0 - vigAmt * (uv.y - 0.5) * (uv.y - 0.5)) * (1.0 - vigAmt * (uv.x - 0.5) * (uv.x - 0.5));
		c = pow(abs(c), vignetteAmount);
		return c;
	}

		float4 Frag(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		float4 col = LOAD_TEXTURE2D_X(_InputTexture,i.texcoord * _ScreenSize.xy);
		float2 p = i.texcoord.xy;
		//float4 rgb = tex2D(_MainTex, p);
		col.rgb *= vignette(p, Time * vignetteSpeed);
		return half4(col);

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