Shader "Hidden/Shader/Glitch1Effect_RLPRO"
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
    float _Intensity;
    TEXTURE2D_X(_InputTexture);
	 float T;
	 float Speed;
	 float Strength;
	 float Fade;
	float x = 127.1;
	float angleY = 311.7;
	float y = 43758.5453123;
	 float Stretch = 0.02;

	 float mR = 0.08;
	 float mG = 0.07;
	 float mB = 0.0;

	float hash(float2 d)
	{
		float m = dot(d, float2(x, angleY));
		return -1.0 + 2.0 * frac(sin(m) * y);
	}
	float noise(float2 d)
	{
		float2 i = floor(d);
		float2 f = frac(d);
		float2 u = f * f * (3.0 - 2.0 * f);
		return lerp(lerp(hash(i + float2(0.0, 0.0)), hash(i + float2(1.0, 0.0)), u.x), lerp(hash(i + float2(0.0, 1.0)), hash(i + float2(1.0, 1.0)), u.x), u.y);
	}
	float noise1(float2 d)
	{
		float2 s = float2 (1.6, 1.2);
		float f = 0.0;
		for (int i = 1; i < 3; i++) { float mul = 1.0 / pow(1.0, float(i)); f += mul * noise(d); d = s * d; }
		return f;
	}

	float4 Frag(Varyings i) : SV_Target
	{
		//UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		Strength *= 10;
		float4 result = float4(0,0,0,0);
		float2 uv = i.texcoord.xy ;
		float glitch = pow(cos(T * Speed * 5.5) * 1.2 + 1.0, 1.2);
		glitch = saturate(glitch);
		float2 hp = float2(0.0, uv.y);
		float nh = noise1(hp * 7.0 + T * Speed * 10.0) * (noise(hp + T * Speed * 0.3) * 0.8);
		nh += noise1(hp * 100.0 + T * Speed * 10.0) * Stretch;
		float rnd = 0.0;
		if (glitch > 0.0) { rnd = hash(uv*_ScreenParams.xy); if (glitch < 1.0) { rnd *= glitch; } }
		nh *= glitch + rnd;
		half shiftR = 0.08 * mR;
		half shiftG = 0.07 * mG;
		half shiftB = mB;
		float4 r = LOAD_TEXTURE2D_X(_InputTexture,( uv * _ScreenSize.xy) + float2(nh, shiftR*20) * nh * Strength);
		float4 g = LOAD_TEXTURE2D_X(_InputTexture,( uv * _ScreenSize.xy) + float2(nh - shiftG*20, 0.0) * nh * Strength);
		float4 b = LOAD_TEXTURE2D_X(_InputTexture, (uv * _ScreenSize.xy) + float2(nh, shiftB*20) * nh * Strength);
		float4 kkk = LOAD_TEXTURE2D_X(_InputTexture, i.texcoord * _ScreenSize.xy);
		float4 col = float4(r.r, g.g, b.b, 1.0);
		result = lerp(kkk,col,Fade);
		return result;
	}
	//	float4 Frag0(Varyings i) : SV_Target
	//{
	//	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
	//	float4 col = LOAD_TEXTURE2D_X(_InputTexture,i.texcoord * _ScreenSize.xy);
	//	float2 fragCoord = i.texcoord.xy * _ScreenParams.xy;
	//	float2 pos = Warp1(fragCoord.xy / _ScreenParams.xy);
	//	float4 col2 = LOAD_TEXTURE2D_X(_InputTexture,pos * _ScreenSize.xy);
	//	return lerp(col,col2,fade);
	//}



    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "#GlitchPass#"

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