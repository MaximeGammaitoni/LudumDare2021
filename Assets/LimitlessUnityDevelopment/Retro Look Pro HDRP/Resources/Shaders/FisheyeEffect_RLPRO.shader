Shader "Hidden/Shader/FisheyeEffect_RLPRO"
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

	TEXTURE2D_X(_InputTexture);

#pragma shader_feature VHS_FISHEYE_ON
#pragma shader_feature VHS_FISHEYE_HYPERSPACE
#define fixCoord (p - float2( 0.5 * ONE_X, 0.0)) 

	half cutoffX = 2.0;
	half cutoffY = 3.0;
	half cutoffFadeX = 100.0;
	half cutoffFadeY = 100.0;
	float fisheyeSize = 1.2;
	float fisheyeBend = 2.0;
	float time_ = 0.0;
	float ONE_X = 0.0;
	float ONE_Y = 0.0;
	float _Intensity;
	half fade;

	float2 fishEye(float2 uv, float size, float bend)
	{
#if !VHS_FISHEYE_HYPERSPACE
		uv -= float2(0.5, 0.5);
		uv *= size * (1.0 / size + bend * uv.x * uv.x * uv.y * uv.y);
		uv += float2(0.5, 0.5);
#endif 

#if VHS_FISHEYE_HYPERSPACE
		float mx = bend / 50.0;
		float2 p = (uv * _ScreenSize.xy) / _ScreenSize.x;
		float prop = _ScreenSize.x / _ScreenSize.y;
		float2 m = float2(0.5, 0.5 / prop);
		float2 d = p - m;
		float r = sqrt(dot(d, d));
		float bind;
		float power = (2.0 * 3.141592 / (2.0 * sqrt(dot(m, m)))) * (mx - 0.5);
		if (power > 0.0) bind = sqrt(dot(m, m));
		else { if (prop < 1.0) bind = m.x; else bind = m.x; }
		if (power > 0.0)
			uv = m + normalize(d) * tan(r * power) * bind / tan(bind * power);
		else if (power < 0.0)
			uv = m + normalize(d) * atan(r * -power * 10.0) * bind / atan(-power * bind * 10.0);
		else uv = p;
		uv.y *= prop;
#endif 
		return uv;
	}

	float4 CustomPostProcess(Varyings input) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		uint2 positionSS = input.texcoord * _ScreenSize.xy;
		float2 p = input.texcoord;
		ONE_X = 1.0 / _ScreenSize.x;
		ONE_Y = 1.0 / _ScreenSize.y;
		p = fishEye(p, fisheyeSize, fisheyeBend);
		float3 col = LOAD_TEXTURE2D_X(_InputTexture, p * _ScreenSize.xy).rgb;
		half far;
		half2 hco = half2(ONE_X * cutoffX, ONE_Y * cutoffY);
		half2 sco = half2(ONE_X * cutoffFadeX, ONE_Y * cutoffFadeY);

		if (p.x <= (0.0 + hco.x) || p.x >= (1.0 - hco.x) ||
			p.y <= (0.0 + hco.y) || p.y >= (1.0 - hco.y))
		{
			col = half3(0.0, 0.0, 0.0);
		}
		else
		{
			if (
				(p.x > (0.0 + hco.x) && p.x < (0.0 + (sco.x + hco.x))) ||
				(p.x > (1.0 - (sco.x + hco.x)) && p.x < (1.0 - hco.x))
				) {
				if (p.x < 0.5)	far = (0.0 - hco.x + p.x) / (sco.x);
				else			far = (1.0 - hco.x - p.x) / (sco.x);
				col *= far;
			};
			if (
				(p.y > (0.0 + hco.y) && p.y < (0.0 + (sco.y + hco.y))) ||
				(p.y > (1.0 - (sco.y + hco.y)) && p.y < (1.0 - hco.y))
				) {
				if (p.y < 0.5)	far = (0.0 - hco.y + p.y) / (sco.y);
				else			far = (1.0 - hco.y - p.y) / (sco.y);
				col *= far;
			}
		}
		return float4(col.rgb,1);
	}

		ENDHLSL

		SubShader
	{
		Pass
		{
			Name "#VHSFisheye#"

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