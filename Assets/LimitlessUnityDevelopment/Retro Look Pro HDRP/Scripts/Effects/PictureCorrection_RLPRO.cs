using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Retro Look Pro/PictureCorrection_RLPRO")]
public sealed class PictureCorrection_RLPRO : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
	[Range(-0.25f, 0.25f), Tooltip(" Y permanent adjustment..")]
	public ClampedFloatParameter signalAdjustY = new ClampedFloatParameter (0f,-0.25f, 0.25f);
	[Range(-0.25f, 0.25f), Tooltip("I permanent adjustment..")]
	public ClampedFloatParameter signalAdjustI = new ClampedFloatParameter (0f,-0.25f, 0.25f);
	[Range(-0.25f, 0.25f), Tooltip("Q permanent adjustment..")]
	public ClampedFloatParameter signalAdjustQ = new ClampedFloatParameter (0f,-0.25f, 0.25f);
	[Range(-2f, 2f), Tooltip("tweak/shift Y values..")]
	public ClampedFloatParameter signalShiftY = new ClampedFloatParameter (1f,-2f, 2f);
	[Range(-2f, 2f), Tooltip("tweak/shift I values..")]
	public ClampedFloatParameter signalShiftI = new ClampedFloatParameter (1f,-2f, 2f);
	[Range(-2f, 2f), Tooltip("tweak/shift Q values..")]
	public ClampedFloatParameter signalShiftQ = new ClampedFloatParameter (1f,-2f, 2f);
	[Range(0f, 2f), Tooltip("use this to balance the gamma(brightness) of the signal.")]
	public ClampedFloatParameter gammaCorection = new ClampedFloatParameter (1f,-0f, 2f);
	Material m_Material;

    public bool IsActive() => m_Material != null && intensity.value > 0f;

    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/PictureCorrectionEffect_RLPRO") != null)
            m_Material = new Material(Shader.Find("Hidden/Shader/PictureCorrectionEffect_RLPRO"));
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;
		m_Material.SetFloat("signalAdjustY",  signalAdjustY.value);
		m_Material.SetFloat("signalAdjustI",  signalAdjustI.value);
		m_Material.SetFloat("signalAdjustQ",  signalAdjustQ.value);
		m_Material.SetFloat("signalShiftY",  signalShiftY.value);
		m_Material.SetFloat("signalShiftI",  signalShiftI.value);
		m_Material.SetFloat("signalShiftQ",  signalShiftQ.value);
		m_Material.SetFloat("gammaCorection",  gammaCorection.value);
		m_Material.SetFloat("_Intensity", intensity.value);
        m_Material.SetTexture("_InputTexture", source);
        HDUtils.DrawFullScreen(cmd, m_Material, destination);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}
