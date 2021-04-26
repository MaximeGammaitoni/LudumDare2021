Shader "Hidden/Shader/TV_RLPRO_HDRP"
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
	float maskDark = 0.5;
	float maskLight = 1.5;
	float hardScan = -8.0;
	float hardPix = -3.0;
	float2 warp = float2(1.0 / 32.0, 1.0 / 24.0);
	float2 res;
	float resScale;
	float scale;
	float fade;
	
	float3 Fetch(float2 pos, float2 off)
	{
		pos = floor(pos * res + off) / res;
		return SAMPLE_TEXTURE2D_X_LOD(_InputTexture, s_linear_clamp_sampler,pos,0.0).rgb;
	}

	float2 Dist(float2 pos) { pos = pos * res; return -((pos - floor(pos)) - float2(0.5, 0.5)); }
	float Gaus(float pos, float scale) { return exp2(scale * pos * pos); }
	float3 Horz3(float2 pos, float off)
	{
		float3 b = Fetch(pos, float2(-1.0, off));
		float3 c = Fetch(pos, float2(0.0, off));
		float3 d = Fetch(pos, float2(1.0, off));
		float dst = Dist(pos).x;
		float scale = hardPix;
		float wb = Gaus(dst - 1.0, scale);
		float wc = Gaus(dst + 0.0, scale);
		float wd = Gaus(dst + 1.0, scale);
		return (b * wb + c * wc + d * wd) / (wb + wc + wd);
	}
	float3 Horz5(float2 pos, float off)
	{
		float3 a = Fetch(pos, float2(-2.0, off));
		float3 b = Fetch(pos, float2(-1.0, off));
		float3 c = Fetch(pos, float2(0.0, off));
		float3 d = Fetch(pos, float2(1.0, off));
		float3 e = Fetch(pos, float2(2.0, off));
		float dst = Dist(pos).x;
		float scale = hardPix;
		float wa = Gaus(dst - 2.0, scale);
		float wb = Gaus(dst - 1.0, scale);
		float wc = Gaus(dst + 0.0, scale);
		float wd = Gaus(dst + 1.0, scale);
		float we = Gaus(dst + 2.0, scale);
		return (a * wa + b * wb + c * wc + d * wd + e * we) / (wa + wb + wc + wd + we);
	}
	float Scan(float2 pos, float off)
	{
		float dst = Dist(pos).y;
		return Gaus(dst + off, hardScan);
	}

	float3 Tri(float2 pos)
	{
		float3 a = Horz3(pos, -1.0);
		float3 b = Horz5(pos, 0.0);
		float3 c = Horz3(pos, 1.0);
		float wa = Scan(pos, -1.0);
		float wb = Scan(pos, 0.0);
		float wc = Scan(pos, 1.0);
		return a * wa + b * wb + c * wc;
	}

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

	float3 Mask(float2 pos)
	{
		pos.x += pos.y * 3.0;
		float3 mask = float3(maskDark, maskDark, maskDark);
		pos.x = frac(pos.x / 6.0);
		if (pos.x < 0.333)mask.r = maskLight;
		else if (pos.x < 0.666)mask.g = maskLight;
		else mask.b = maskLight;
		return mask;
	}

    float4 CustomPostProcessTVWarp(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        uint2 positionSS = input.texcoord * _ScreenParams.xy;
        float4 outColor = LOAD_TEXTURE2D_X(_InputTexture, positionSS);			
		res = _ScreenParams.xy / resScale;
		float2 fragCoord = input.texcoord.xy * _ScreenParams.xy;
		float4 fragColor = 0;
		float2 pos = Warp1(fragCoord.xy / _ScreenParams.xy);
		fragColor.rgb = Tri(pos) * Mask(fragCoord);
		fragColor.a = 1;
		return lerp(outColor, fragColor, fade);
    }

	float4 CustomPostProcessTVCubic(Varyings input) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		uint2 positionSS = input.texcoord * _ScreenParams.xy;
		float4 outColor = LOAD_TEXTURE2D_X(_InputTexture, positionSS);
		res = _ScreenParams.xy / resScale;
		float2 fragCoord = input.texcoord.xy * _ScreenParams.xy;
		float4 fragColor = 0;
		float2 pos = Warp(fragCoord.xy / _ScreenParams.xy);
		fragColor.rgb = Tri(pos) * Mask(fragCoord);
		fragColor.a = LOAD_TEXTURE2D_X(_InputTexture, pos).a;
		return lerp(outColor, fragColor, fade);
	}

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "#Warp#"

            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            HLSLPROGRAM
                #pragma fragment CustomPostProcessTVWarp
                #pragma vertex Vert
            ENDHLSL
        }
			Pass
		{
			Name "#Cubic#"

			ZWrite Off
			ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off

			HLSLPROGRAM
				#pragma fragment CustomPostProcessTVCubic
				#pragma vertex Vert
			ENDHLSL
		}
    }
    Fallback Off
}