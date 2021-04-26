Shader "Hidden/Shader/ArtefactsEffect_RLPRO"
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
	TEXTURE2D_X(_LastTex);
	TEXTURE2D_X(_FeedbackTex);
	TEXTURE2D_X(_FeedbackTex3);
	TEXTURE2D_X(_InputTexture4);
	TEXTURE2D_X(_InputTexture2);
	TEXTURE2D_X(_InputTexture3);
	/*SAMPLER(_MainTex);*/
	float feedbackAmount = 0.0;
	float feedbackFade = 0.0;
	float feedbackThresh = 5.0;
	half3 feedbackColor = half3(1.0, 0.5, 0.0); //
	//TEXTURE2D_X( _FeedbackTex1);
	float feedbackAmp = 1.0;
	half3 bm_screen(half3 a, half3 b) { return 1.0 - (1.0 - a) * (1.0 - b); }

    // List of properties to control your post process effect
    float _Intensity;
    TEXTURE2D_X(_InputTexture);

	float4 Frag0(Varyings i) : COLOR
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);		
		float one_x = 1.0 / _ScreenParams.x;
		half3 fc = LOAD_TEXTURE2D_X(_InputTexture, i.texcoord * _ScreenSize.xy).rgb;
		half3 fl = LOAD_TEXTURE2D_X(_LastTex, i.texcoord * _ScreenSize.xy).rgb;
		float diff = abs(fl.x - fc.x + fl.y - fc.y + fl.z - fc.z) / 3.0;
		if (diff < feedbackThresh) diff = 0.0;
		half3 fbn = fc * diff * feedbackAmount;
		half3 fbb = half3(0.0, 0.0, 0.0);
		fbb = (
			LOAD_TEXTURE2D_X(_FeedbackTex, i.texcoord * _ScreenSize.xy).rgb +
			LOAD_TEXTURE2D_X(_FeedbackTex, i.texcoord * _ScreenSize.xy + float2(one_x, 0.0)).rgb +
			LOAD_TEXTURE2D_X(_FeedbackTex, i.texcoord * _ScreenSize.xy - float2(one_x, 0.0)).rgb
			) / 3.0;
		fbb *= feedbackFade;
		fbn = bm_screen(fbn, fbb);
		return half4(fbn * feedbackColor, 1.0);
	}

		float4 Frag(Varyings i) : COLOR
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		half3 col = LOAD_TEXTURE2D_X(_InputTexture4, i.texcoord * _ScreenSize.xy).rgb;
		half4 fbb = LOAD_TEXTURE2D_X(_FeedbackTex3, i.texcoord * _ScreenSize.xy);
		col.rgb = bm_screen(col.rgb, fbb);
		return half4(col, fbb.a);
	}

		float4 Frag1(Varyings i) : COLOR
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		float4 col = LOAD_TEXTURE2D_X(_InputTexture2,i.texcoord * _ScreenSize.xy);
		return col;
	}

		float4 Frag2(Varyings i) : COLOR
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		float4 col = LOAD_TEXTURE2D_X(_InputTexture3,i.texcoord * _ScreenSize.xy);
		return col;
	}

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "#first#"

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
			Name "#second#"

			ZWrite Off
			ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off

			HLSLPROGRAM
				#pragma fragment Frag
				#pragma vertex Vert
			ENDHLSL
		}
			Pass
		{
			Name "#CopyBlit#"

			ZWrite Off
			ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off

			HLSLPROGRAM
				#pragma fragment Frag1
				#pragma vertex Vert
			ENDHLSL
		}
			Pass
		{
			Name "#CopyBlit2#"

			ZWrite Off
			ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off

			HLSLPROGRAM
				#pragma fragment Frag2
				#pragma vertex Vert
			ENDHLSL
		}
    }
    Fallback Off
}