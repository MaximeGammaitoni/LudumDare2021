Shader "Hidden/Shader/NTSCEncode_RLPRO"
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
	#define Pi2 float(6.283185307)
	#define Zero float4(0.0, 0.0, 0.0, 0.0)

	#define Half float4(0.5, 0.5, 0.5, 0.5)
	#define One float4(1.0, 1.0, 1.0, 1.0)
	#define Two float4(2.0, 2.0, 2.0, 2.0)
	#define A float4(0.5, 0.5, 0.5, 0.5)
	#define B float4(0.5, 0.5, 0.5, 0.5)
	#define P float(1.0)
	#define CCFrequency float(3.59754545)
	#define MinC float4(-1.1183, -1.1183, -1.1183, -1.1183)
	#define CRange float4(3.2366, 3.2366, 3.2366, 3.2366)
	float T;
	half val1;
	half val2;
	float4 CompositeSample(float2 UV) 
	{
		float ScanTime = 52.6 +T* val1;
		float2 InverseRes = 1.0/_ScreenSize.xy;
		float2 InverseP = float2(P, 0.0) * InverseRes;
		float2 C0 = UV;
		float2 C1 = UV + InverseP * 0.25;
		float2 C2 = UV + InverseP * 0.50;
		float2 C3 = UV + InverseP * 0.75;
		float4 Cx = float4(C0.x, C1.x, C2.x, C3.x);
		float4 Cy = float4(C0.y, C1.y, C2.y, C3.y);

		float3 Texel0 = LOAD_TEXTURE2D_X(_InputTexture, C0 * _ScreenSize.xy).rgb;
		float3 Texel1 = LOAD_TEXTURE2D_X(_InputTexture, C1 * _ScreenSize.xy).rgb;
		float3 Texel2 = LOAD_TEXTURE2D_X(_InputTexture, C2 * _ScreenSize.xy).rgb;
		float3 Texel3 = LOAD_TEXTURE2D_X(_InputTexture, C3 * _ScreenSize.xy).rgb;

		float4 T = A * Cy * float4(_ScreenSize.x, _ScreenSize.x, _ScreenSize.x, _ScreenSize.x) * Two + B + Cx;

		const float3 YTransform = float3(0.299, 0.587, 0.114);
		const float3 ITransform = float3(0.595716, -0.274453, -0.321263);
		const float3 QTransform = float3(0.211456, -0.522591, 0.311135);

		float Y0 = dot(Texel0, YTransform);
		float Y1 = dot(Texel1, YTransform);
		float Y2 = dot(Texel2, YTransform);
		float Y3 = dot(Texel3, YTransform);
		float4 Y = float4(Y0, Y1, Y2, Y3);

		float I0 = dot(Texel0, ITransform);
		float I1 = dot(Texel1, ITransform);
		float I2 = dot(Texel2, ITransform);
		float I3 = dot(Texel3, ITransform);
		float4 I = float4(I0, I1, I2, I3);

		float Q0 = dot(Texel0, QTransform);
		float Q1 = dot(Texel1, QTransform);
		float Q2 = dot(Texel2, QTransform);
		float Q3 = dot(Texel3, QTransform);
		float4 Q = float4(Q0, Q1, Q2, Q3);

		float4 W = float4(Pi2 * CCFrequency * ScanTime, Pi2 * CCFrequency * ScanTime, Pi2 * CCFrequency * ScanTime, Pi2 * CCFrequency * ScanTime);
		float4 Encoded = Y + I * cos(T * W) + Q * sin(T * W);
		return (Encoded - MinC) / CRange;
	}
	half Bsize;
	float4 NTSCCodec(float2 UV)
	{
		float YFrequency = 6.0;
		float IFrequency = 1.2;
		float QFrequency = 0.6;
		float NotchHalfWidth = 2.0;
		float ScanTime = 52.6 +T* val1;	
		float2 InverseRes = 1.0 / _ScreenSize.xy;
		float4 YAccum = Zero;
		float4 IAccum = Zero;
		float4 QAccum = Zero;
		float QuadXSize = _ScreenSize.x * Bsize;
		float TimePerSample = ScanTime / QuadXSize;

		float Fc_y1 = (CCFrequency - NotchHalfWidth) * TimePerSample;
		float Fc_y2 = (CCFrequency + NotchHalfWidth) * TimePerSample;
		float Fc_y3 = YFrequency * TimePerSample;
		float Fc_i = IFrequency * TimePerSample;
		float Fc_q = QFrequency * TimePerSample;
		float Pi2Length = Pi2 / 82.0;
		float4 NotchOffset = float4(0.0, 1.0, 2.0, 3.0);

		float4 W = float4(Pi2 * CCFrequency * ScanTime, Pi2 * CCFrequency * ScanTime, Pi2 * CCFrequency * ScanTime, Pi2 * CCFrequency * ScanTime);

		for (float n = -8.0; n < 8.0; n += 4.0)
		{
			float4 n4 = n/2 + NotchOffset;
			float4 CoordX = UV.x + InverseRes.x * n4 * val2;
			float4 CoordY = float4(UV.y, UV.y, UV.y, UV.y);
			float2 TexCoord = float2(CoordX.r, CoordY.r);
			float4 C = CompositeSample(TexCoord) * CRange + MinC;
			float4 WT = W * (CoordX + A * CoordY * Two * _ScreenSize.x + B);
			float4 SincYIn1 = Pi2 * Fc_y1 * n4;
			float4 SincYIn2 = Pi2 * Fc_y2 * n4;
			float4 SincYIn3 = Pi2 * Fc_y3 * n4;
			float4 SincY1 = sin(SincYIn1) / SincYIn1;
			float4 SincY2 = sin(SincYIn2) / SincYIn2;
			float4 SincY3 = sin(SincYIn3) / SincYIn3;

			if (SincYIn1.x == 0.0) SincY1.x = 1.0;
			if (SincYIn1.y == 0.0) SincY1.y = 1.0;
			if (SincYIn1.z == 0.0) SincY1.z = 1.0;
			if (SincYIn1.w == 0.0) SincY1.w = 1.0;
			if (SincYIn2.x == 0.0) SincY2.x = 1.0;
			if (SincYIn2.y == 0.0) SincY2.y = 1.0;
			if (SincYIn2.z == 0.0) SincY2.z = 1.0;
			if (SincYIn2.w == 0.0) SincY2.w = 1.0;
			if (SincYIn3.x == 0.0) SincY3.x = 1.0;
			if (SincYIn3.y == 0.0) SincY3.y = 1.0;
			if (SincYIn3.z == 0.0) SincY3.z = 1.0;
			if (SincYIn3.w == 0.0) SincY3.w = 1.0;
			float4 IdealY = (2.0 * Fc_y1 * SincY1 - 2.0 * Fc_y2 * SincY2) + 2.0 * Fc_y3 * SincY3;
			float4 FilterY = (0.54 + 0.46 * cos(Pi2Length * n4)) * IdealY;

			float4 SincIIn = Pi2 * Fc_i * n4;
			float4 SincI = sin(SincIIn) / SincIIn;
			if (SincIIn.x == 0.0) SincI.x = 1.0;
			if (SincIIn.y == 0.0) SincI.y = 1.0;
			if (SincIIn.z == 0.0) SincI.z = 1.0;
			if (SincIIn.w == 0.0) SincI.w = 1.0;
			float4 IdealI = 2.0 * Fc_i * SincI;
			float4 FilterI = (0.54 + 0.46 * cos(Pi2Length * n4)) * IdealI;

			float4 SincQIn = Pi2 * Fc_q * n4;
			float4 SincQ = sin(SincQIn) / SincQIn;
			if (SincQIn.x == 0.0) SincQ.x = 1.0;
			if (SincQIn.y == 0.0) SincQ.y = 1.0;
			if (SincQIn.z == 0.0) SincQ.z = 1.0;
			if (SincQIn.w == 0.0) SincQ.w = 1.0;
			float4 IdealQ = 2.0 * Fc_q * SincQ;
			float4 FilterQ = (0.54 + 0.46 * cos(Pi2Length * n4)) * IdealQ;

			YAccum = YAccum + C * FilterY;
			IAccum = IAccum + C * cos(WT) * FilterI;
			QAccum = QAccum + C * sin(WT) * FilterQ;
		}

		float Y = YAccum.r + YAccum.g + YAccum.b + YAccum.a;
		float I = (IAccum.r + IAccum.g + IAccum.b + IAccum.a) * 2.0;
		float Q = (QAccum.r + QAccum.g + QAccum.b + QAccum.a) * 2.0;

		float3 YIQ = float3(Y, I, Q);

		float3 OutRGB = float3(dot(YIQ, float3(1.0, 0.956, 0.621)), dot(YIQ, float3(1.0, -0.272, -0.647)), dot(YIQ, float3(1.0, -1.106, 1.703)));

		return float4(OutRGB, 1.0);
	}

		float4 Frag0(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		return NTSCCodec(i.texcoord );
	}

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "#NTSCEncode#"

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