Shader "Hidden/Shader/Glitch3Effect_RLPRO"
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
	float _ColorIntensity;
    TEXTURE2D_X(_InputTexture);
	float Time;
	float speed;
	float density;
	float maxDisplace;

	inline float rand(float2 seed)
	{
		return frac(sin(dot(seed * floor(Time*10 * speed), float2(127.1, 311.7))) * 43758.5453123);
	}

	inline float rand(float seed)
	{
		return rand(float2(seed, 1.0));
	}

	float4 Frag(Varyings i) : SV_Target
	{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		float2 rblock = rand(floor(i.texcoord * density*10));
		float displaceNoise = pow(rblock.x, 8.0) * pow(rblock.x, 3.0) - pow(rand(7.2341), 17.0) * maxDisplace;
		half2 uv = i.texcoord * _ScreenSize.xy;
		float4 r = LOAD_TEXTURE2D_X(_InputTexture, uv);
		float4 g = LOAD_TEXTURE2D_X(_InputTexture, uv + half2(displaceNoise * 20.05 * rand(7.0), 0.0));
		float4 b = LOAD_TEXTURE2D_X(_InputTexture, uv - half2(displaceNoise * 20.05 * rand(13.0), 0.0));

		return half4(r.r, g.g, b.b, 1);
	}

	//	float4 Frag2(Varyings i) : COLOR
	//{
	//	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
	//	float4 col = LOAD_TEXTURE2D_X(_InputTexture1,i.texcoord * _ScreenSize.xy);
	//	return col;
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
		//	Pass
		//{
		//	Name "#CopyBlit#"

		//	ZWrite Off
		//	ZTest Always
		//	Blend SrcAlpha OneMinusSrcAlpha
		//	Cull Off

		//	HLSLPROGRAM
		//		#pragma fragment Frag2
		//		#pragma vertex Vert
		//	ENDHLSL
		//}
    }
    Fallback Off
}