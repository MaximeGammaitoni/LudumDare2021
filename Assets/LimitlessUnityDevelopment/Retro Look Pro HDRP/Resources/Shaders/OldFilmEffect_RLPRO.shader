Shader "Hidden/Shader/OldFilmEffect_RLPRO"
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
	uniform float T;
	uniform float FPS;
	uniform float Contrast;
	uniform float Burn;
	uniform float SceneCut;
	uniform float Fade;

	float rand(float2 co)
	{
		return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
	}

	float rand(float c)
	{
		return rand(float2(c, 1.0));
	}

	float randomLine(float seed, float2 uv)
	{
		float aa = rand(seed + 1.0);
		float b = 0.01 * aa;
		float c = aa - 0.5;
		float l;
		if (aa > 0.2)
			l = pow(abs(aa * uv.x + b * uv.y + c), 0.125);
		else
			l = 2.0 - pow(abs(aa * uv.x + b * uv.y + c), 0.125);
		return lerp(0.5 - SceneCut, 1.0, l);
	}

	float randomBlotch(float seed, float2 uv)
	{
		float x = rand(seed);
		float y = rand(seed + 1.0);
		float s = 0.01 * rand(seed + 2.0);
		float2 p = float2(x, y) - uv;
		p.x *= 1;
		float aa = atan(p.y / p.x);
		float v = 1.0;
		float ss = s * s * (sin(6.2831 * aa * x) * 0.1 + 1.0);
		if (dot(p, p) < ss) v = 0.2;
		else v = pow(abs(dot(p, p) - ss), 1.0 / 16.0);
		return lerp(0.3 + 0.2 * (1.0 - (s / 0.02)) - SceneCut, 1.0, v);
	}

    // List of properties to control your post process effect
    float _Intensity;
    TEXTURE2D_X(_InputTexture);

		float4 Frag0(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		float2 uv = i.texcoord;
		float t = float(int(T * FPS));
		float2 suv = uv + 0.002 * float2(rand(t), rand(t + 23.0));
		float3 image = LOAD_TEXTURE2D_X(_InputTexture, float2(suv.x, suv.y) * _ScreenSize.xy).rgb;
		float luma = dot(float3(0.2126, 0.7152, 0.0722), image);
		float3 oldImage = luma * float3(0.7 + Burn, 0.7 + Burn / 2, 0.7) * Contrast;
		oldImage = oldImage * float3(0.7 + Burn, 0.7 + Burn / 8, 0.7) * Contrast;
		float randx = rand(t + 8.);
		float vI = 16.0 * (uv.x * (1.0 - uv.x) * uv.y * (1.0 - uv.y));
		vI *= lerp(0.7, 1.0, randx + .5);
		vI += 1.0 + 0.4 * randx;
		vI *= pow(abs(16.0 * uv.x * (1.0 - uv.x) * uv.y * (1.0 - uv.y)), 0.4);
		int l = int(8.0 * randx);
		if (0 < l) vI *= randomLine(t + 6.0 + 17. * float(0), uv);
		if (1 < l) vI *= randomLine(t + 6.0 + 17. * float(1), uv);
		int s = int(max(8.0 * rand(t + 18.0) - 2.0, 0.0));
		if (0 < s) vI *= randomBlotch(t + 6.0 + 19. * float(0), uv);
		if (1 < s) vI *= randomBlotch(t + 6.0 + 19. * float(1), uv);
		float4 result = float4(oldImage * vI, LOAD_TEXTURE2D_X(_InputTexture, float2(suv.x, suv.y)).a);
		result = lerp(result, LOAD_TEXTURE2D_X(_InputTexture, i.texcoord * _ScreenSize.xy), 1 - Fade);
		return result;
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
    }
    Fallback Off
}