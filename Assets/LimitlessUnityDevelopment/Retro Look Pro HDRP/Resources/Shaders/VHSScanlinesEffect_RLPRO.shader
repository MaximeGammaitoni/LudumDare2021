Shader "Hidden/Shader/VHSScanlinesEffect_RLPRO"
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
	float4 _ScanLinesColor;
	float _ScanLines;
	sampler2D _MainTex;
	float speed;
	float fade;
	float _OffsetDistortion;
	float sferical;
	float barrel;
	float scale;
	float _OffsetColor;
	float2 _OffsetColorAngle;
	float Time;
    // List of properties to control your post process effect
    float _Intensity;
    TEXTURE2D_X(_InputTexture);

		///////

		float2 FisheyeDistortion(float2 coord, float spherical, float barrel, float scale)
	{
		float2 h = coord.xy - float2(0.5, 0.5);
		float r2 = dot(h, h);
		float f = 1.0 + r2 * (spherical + barrel * sqrt(r2));
		return f * scale * h + 0.5;
	}

	float4 FragH(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		float2 coord = FisheyeDistortion(i.texcoord, sferical, barrel, scale);
		half4 color = LOAD_TEXTURE2D_X(_InputTexture, i.texcoord * _ScreenSize.xy);
		float lineSize = _ScreenParams.y * 0.005;
		float displacement = ((_Time / 4 * 1000) * speed) % _ScreenParams.y;
		float ps;
		ps = displacement + (coord.y * _ScreenParams.y / i.positionCS.w);
		float sc = i.texcoord.y;
		float4 result;
		result = ((int)(ps / floor(_ScanLines * lineSize)) % 2 == 0) ? color : _ScanLinesColor;
		result += color * sc;
		return lerp(color,result,fade);
	}

		float4 FragHD(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		float2 coord = FisheyeDistortion(i.texcoord, sferical, barrel, scale);
		half4 color = LOAD_TEXTURE2D_X(_InputTexture, i.texcoord * _ScreenSize.xy);
		float lineSize = _ScreenParams.y * 0.005;
		float displacement = ((_Time / 4 * 1000) * speed) % _ScreenParams.y;
		float ps;
		i.texcoord.y = frac(i.texcoord.y + cos((coord.x + _Time / 4) * 100)  * _OffsetDistortion*0.1);
		ps = displacement + (i.texcoord.y * _ScreenParams.y / i.positionCS.w);
		float sc = i.texcoord.y;
		float4 result;
		result = ((int)(ps / floor(_ScanLines * lineSize)) % 2 == 0) ? color : _ScanLinesColor;
		result += color * sc;
		return lerp(color,result,fade);
	}

		float4 FragV(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		float2 coord = FisheyeDistortion(i.texcoord, sferical, barrel, scale);
		half4 color = LOAD_TEXTURE2D_X(_InputTexture, i.texcoord * _ScreenSize.xy);
		float lineSize = _ScreenParams.y * 0.005;
		float displacement = ((_Time / 4 * 1000) * speed) % _ScreenParams.y;
		float ps;
		ps = displacement + (coord.x * _ScreenParams.x / i.positionCS.w);
		float sc = i.texcoord.y;
		float4 result;
		result = ((int)(ps / floor(_ScanLines * lineSize)) % 2 == 0) ? color : _ScanLinesColor;
		result += color * sc;
		return lerp(color,result,fade);
	}

		float4 FragVD(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		float2 coord = FisheyeDistortion(i.texcoord, sferical, barrel, scale);
		half4 color = LOAD_TEXTURE2D_X(_InputTexture, i.texcoord * _ScreenSize.xy);
		float lineSize = _ScreenParams.y * 0.005;
		float displacement = ((_Time / 4 * 1000) * speed) % _ScreenParams.y;
		float ps;
		i.texcoord.x = frac(i.texcoord.x + cos((coord.y + (_Time / 4)) * 100)  * _OffsetDistortion*0.1);
		ps = displacement + (i.texcoord.x * _ScreenParams.x / i.positionCS.w);
		float sc = i.texcoord.y;
		float4 result;
		result = ((int)(ps / floor(_ScanLines * lineSize)) % 2 == 0) ? color : _ScanLinesColor;
		result += color * sc;
		return lerp(color,result,fade);
	}
    ENDHLSL

    SubShader
    {
		Cull Off ZWrite Off ZTest Always

			Pass
		{
			HLSLPROGRAM

				#pragma vertex Vert
				#pragma fragment FragH

			ENDHLSL
		}
			Pass
		{
			HLSLPROGRAM

				#pragma vertex Vert
				#pragma fragment FragHD

			ENDHLSL
		}
			Pass
		{
			HLSLPROGRAM

				#pragma vertex Vert
				#pragma fragment FragV

			ENDHLSL
		}
			Pass
		{
			HLSLPROGRAM

				#pragma vertex Vert
				#pragma fragment FragVD

			ENDHLSL
		}
    }
    Fallback Off
}