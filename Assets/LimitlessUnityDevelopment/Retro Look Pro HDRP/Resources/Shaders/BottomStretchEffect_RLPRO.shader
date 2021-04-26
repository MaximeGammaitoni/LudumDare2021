Shader "Hidden/Shader/BottomStretchEffect_RLPRO"
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
	float amplitude;
	float frequency;
	half _NoiseBottomHeight;
	float Time;
	float onOff(float a, float b, float c, float t)
	{
		return step(c, sin(t + a * cos(t * b)));
	}
	float2 twitchHorizonalRand(float amp, float freq, float2 uv, float t)
	{
		float window = 1.0 / (1.0 + 150.0 * (uv.y - fmod(t * freq, 0.1)) * (uv.y - fmod(t * freq, 0.1)));
		uv.x += sin(uv.y * amp + t) / 40.0
			* onOff(2.1, 4.0, 0.3, t)
			* (150.0 + cos(t * 80.0))
			* window;
		uv.x += 20 * _NoiseBottomHeight;
		return uv;
	}
	float2 twitchHorizonal(float amp, float freq, float2 uv, float t)
	{
		float window = 1.0 / (1.0 + 150.0 * (uv.y - fmod(t * freq, 0.1)) * (uv.y - fmod(t * freq, 0.1)));
		uv.x += sin(uv.y * amp + t) / 40.0
			* (150.0 + cos(t * 80.0))
			* window;
		uv.x += 20 * _NoiseBottomHeight;
		return uv;
	}
    
    TEXTURE2D_X(_InputTexture);
	SAMPLER(_SecondaryTex);
    float _Intensity;

	float4 FragDist(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);		
		i.texcoord.y = max(i.texcoord.y, twitchHorizonal(amplitude,frequency, i.texcoord,Time * 100.0).x * (_NoiseBottomHeight / 20));
		uint2 positionSS = i.texcoord * _ScreenSize.xy;
		half4 color = LOAD_TEXTURE2D_X(_InputTexture, positionSS);
		float exp = 1.0;
		return float4(pow(color.xyz, float3(exp, exp, exp)), color.a);
	}
	float4 FragDistRand(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);	
		i.texcoord.y = max(i.texcoord.y, twitchHorizonalRand(amplitude,frequency, i.texcoord,Time * 100.0).x * (_NoiseBottomHeight / 20));
		uint2 positionSS = i.texcoord * _ScreenSize.xy;
		half4 color = LOAD_TEXTURE2D_X(_InputTexture, positionSS);
		float exp = 1.0;
		return float4(pow(color.xyz, float3(exp, exp, exp)), color.a);
	}
	float4 Frag(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		i.texcoord.y = max(i.texcoord.y, (_NoiseBottomHeight / 2) - 0.01);
		uint2 positionSS = i.texcoord * _ScreenSize.xy;
		half4 color = LOAD_TEXTURE2D_X(_InputTexture, positionSS);
		float exp = 1.0;
		return float4(pow(color.xyz, float3(exp, exp, exp)), color.a);
	}
    ENDHLSL

    SubShader
    {
		Cull Off ZWrite Off ZTest Always

			Pass
		{
			HLSLPROGRAM

				#pragma vertex Vert
				#pragma fragment FragDist

			ENDHLSL
		}
			Pass
		{
			HLSLPROGRAM

				#pragma vertex Vert
				#pragma fragment FragDistRand

			ENDHLSL
		}
			Pass
		{
			HLSLPROGRAM

				#pragma vertex Vert
				#pragma fragment Frag

			ENDHLSL
		}
    }
    Fallback Off
}