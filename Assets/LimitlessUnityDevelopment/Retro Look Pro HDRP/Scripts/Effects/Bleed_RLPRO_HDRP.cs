using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;
public enum BleedMode
{
	NTSCOld3Phase,
	NTSC3Phase,
	NTSC2Phase,
	customBleeding
}
[Serializable]
public sealed class bleedModeParameter : VolumeParameter<BleedMode> { };
[Serializable, VolumeComponentMenu("Post-processing/Retro Look Pro/Bleed_RLPRO_HDRP")]
public sealed class Bleed_RLPRO_HDRP : CustomPostProcessVolumeComponent, IPostProcessComponent
{
	[Tooltip("NTSC Bleed modes.")]
	public bleedModeParameter bleedMode = new bleedModeParameter();
	[Tooltip("Bleed Stretch amount.")]
	public FloatParameter bleedAmount = new ClampedFloatParameter(0, 0, 15f);
	[Range(0, 50), Tooltip("Bleed Length.")]
	public IntParameter bleedLength = new IntParameter(0);
	[Tooltip("Debug bleed curve.")]
	public BoolParameter bleedDebug = new BoolParameter(false);
	[Tooltip("Enable this to edit curves and see result instantly (otherwise result will be applied after you enter playmode) .")]
	public BoolParameter editCurves = new BoolParameter(false);
	[Tooltip("Synchronize Y and Q chanels.")]
	public BoolParameter syncYQ = new BoolParameter(false);
	[Tooltip("Curve Y chanel.")]
	public TextureCurveParameter curveY = new TextureCurveParameter(new TextureCurve(new AnimationCurve(), 1f, false, new Vector2(0.5f, 1f)));
	[Tooltip("Curve I chanel.")]
	public TextureCurveParameter curveI = new TextureCurveParameter(new TextureCurve(new AnimationCurve(), 1f, false, new Vector2(0.5f, 1f)));
	[Tooltip("Curve Q chanel.")]
	public TextureCurveParameter curveQ = new TextureCurveParameter(new TextureCurve(new AnimationCurve(), 1f, false, new Vector2(0.5f, 1f)));

	public int bleedModeIndex;
	Material m_Material;
	int max_curve_length = 50;
	Texture2D texCurves = null;
	Vector4 curvesOffest = new Vector4(0, 0, 0, 0);
	float[,] curvesData = new float[50, 3];

	public bool IsActive()
	{
		return true;
	}
	public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

	public override void Setup()
	{
		if (Shader.Find("Hidden/Shader/Bleed_RLPRO_HDRP") != null)
			m_Material = new Material(Shader.Find("Hidden/Shader/Bleed_RLPRO_HDRP"));

		if (bleedMode.value == BleedMode.customBleeding)
		{
			Curves();
		}
	}

	public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
	{
		if (m_Material == null)
			return;

		if ((int)bleedMode.value == 3) { if (editCurves.value) Curves(); }
		if ((int)bleedMode.value == 3)
		{
			if (texCurves == null)
				Curves();
			m_Material.SetTexture("_CurvesTex", texCurves);
		}
		m_Material.SetVector("curvesOffest", curvesOffest);
		m_Material.SetFloat("bleedLength", bleedLength.value);
		ParamSwitch(m_Material, bleedDebug.value, "VHS_DEBUG_BLEEDING_ON");
		m_Material.SetFloat("bleedAmount", bleedAmount.value);
		m_Material.SetFloat("_Intensity", IsActive()?0:1);
		m_Material.SetTexture("_InputTexture", source);
		HDUtils.DrawFullScreen(cmd, m_Material, destination, shaderPassId: (int)bleedMode.value);
	}

	public override void Cleanup()
	{
		CoreUtils.Destroy(m_Material);
	}
	private void ParamSwitch(Material mat, bool paramValue, string paramName)
	{
		if (paramValue) mat.EnableKeyword(paramName);
		else mat.DisableKeyword(paramName);
	}
	private void Curves()
	{
		if (texCurves == null) texCurves = new Texture2D(max_curve_length, 1, TextureFormat.RGBA32, false);
		curvesOffest[0] = 0.0f;
		curvesOffest[1] = 0.0f;
		curvesOffest[2] = 0.0f;
		float t = 0.0f;
		for (int i = 0; i < bleedLength.value; i++)
		{
			t = ((float)i) / ((float)bleedLength.value);
			t = (int)(t * 100);
			curvesData[i, 0] = curveY.value.Evaluate(t);
			curvesData[i, 1] = curveI.value.Evaluate(t);
			curvesData[i, 2] = curveQ.value.Evaluate(t);
			if (syncYQ.value) curvesData[i, 2] = curvesData[i, 1];

			if (curvesOffest[0] > curvesData[i, 0]) curvesOffest[0] = curvesData[i, 0];
			if (curvesOffest[1] > curvesData[i, 1]) curvesOffest[1] = curvesData[i, 1];
			if (curvesOffest[2] > curvesData[i, 2]) curvesOffest[2] = curvesData[i, 2];
		};
		curvesOffest[0] = Mathf.Abs(curvesOffest[0]);
		curvesOffest[1] = Mathf.Abs(curvesOffest[1]);
		curvesOffest[2] = Mathf.Abs(curvesOffest[2]);

		for (int i = 0; i < bleedLength.value; i++)
		{
			curvesData[i, 0] += curvesOffest[0];
			curvesData[i, 1] += curvesOffest[1];
			curvesData[i, 2] += curvesOffest[2];
			texCurves.SetPixel(-2 + bleedLength.value - i, 0, new Color(curvesData[i, 0], curvesData[i, 1], curvesData[i, 2]));
		};

		texCurves.Apply();
	}
}
