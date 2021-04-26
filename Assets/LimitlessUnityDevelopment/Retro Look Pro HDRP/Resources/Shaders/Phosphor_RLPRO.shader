Shader "Hidden/Shader/Phosphor_RLPRO"
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
	
	float _Intensity;
	TEXTURE2D_X(_InputTexture);
	TEXTURE2D_X(_Tex);

	float speed = 10.00;
	half amount = 5;
	half fade;
	float T;

	float fract(float x) {
		return  x - floor(x);
	}
	float2 fract(float2 x) {
		return  x - floor(x);
	}

	float random(float2 noise)
	{
		return fract(sin(dot(noise.xy, float2(0.0001, 98.233))) * 925895933.14159265359);
	}

	float random_color(float noise)
	{
		return frac(sin(noise));
	}
	float4 Frag0(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		float4 col = LOAD_TEXTURE2D_X(_InputTexture,i.texcoord * _ScreenSize.xy);
		float4 result = col + LOAD_TEXTURE2D_X(_Tex, i.texcoord * _ScreenSize.xy);
		return lerp(col,result,fade*.01);
	}

	half4 Frag(Varyings i) : SV_Target
	{
		half2 uv = fract(i.texcoord.xy / 12 * ((T.x * speed)));
		half4 color = float4(random(uv.xy), random(uv.xy), random(uv.xy), random(uv.xy));
		
		if (color.a < 33 - amount) color.a = 0.0;
		
		color.r *= random_color(sin(T.x * speed));
		color.g *= random_color(cos(T.x * speed));
		color.b *= random_color(tan(T.x * speed));

		return color;

	}

		ENDHLSL

		SubShader
	{
		Pass
		{
			Name "#MixPass#"

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
			Name "#NoisePass#"

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