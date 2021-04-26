using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Retro Look Pro/Jitter_RLPRO")]
public sealed class Jitter_RLPRO : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
	[Tooltip("Enable Twitch on X axes.")]
	public BoolParameter twitchHorizontal = new BoolParameter (false);
	[Range(0f, 5f), Tooltip("Twitch frequency on X axes.")]
	public ClampedFloatParameter horizontalFreq = new ClampedFloatParameter(1f, 0f, 5f);
	[Space]
	[Tooltip("Enable Twitch on Y axes.")]
	public BoolParameter twitchVertical = new BoolParameter(false);
	[Range(0f, 5f), Tooltip("Twitch frequency on Y axes.")]
	public ClampedFloatParameter verticalFreq = new ClampedFloatParameter(1f, 0f, 5f);
	[Space]
	[Tooltip("Enable Stretch.")]
	public BoolParameter stretch = new BoolParameter(false);
	[Tooltip("Stretch Resolution.")]
	public FloatParameter stretchResolution = new FloatParameter (1f);
	[Space]
	[Tooltip("Enable Horizontal Interlacing.")]
	public BoolParameter jitterHorizontal = new BoolParameter(false);
	[Range(0f, 5f), Tooltip("Amount of horizontal interlacing.")]
	public ClampedFloatParameter jitterHorizontalAmount = new ClampedFloatParameter(1f, 0f, 5f);
	[Space]
	[Tooltip("Shake Vertical.")]
	public BoolParameter jitterVertical = new BoolParameter(false);
	[Range(0f, 15f), Tooltip("Amount of shake.")]
	public ClampedFloatParameter jitterVerticalAmount = new ClampedFloatParameter(1f, 0f, 15f);
	[Range(0f, 15f), Tooltip("Speed of vertical shake. ")]
	public ClampedFloatParameter jitterVerticalSpeed = new ClampedFloatParameter(1f, 0f, 15f);
	[Space]
	[Tooltip("Time.unscaledTime .")]
	public BoolParameter unscaledTime = new BoolParameter(false);
	Material m_Material;
	private float _time;
	public bool IsActive() => m_Material != null && intensity.value > 0f;

    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/JitterEffect_RLPRO") != null)
            m_Material = new Material(Shader.Find("Hidden/Shader/JitterEffect_RLPRO"));
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;

        m_Material.SetFloat("_Intensity", intensity.value);
        m_Material.SetTexture("_InputTexture", source);

		if ( unscaledTime.value) { _time = Time.unscaledTime; }
		else _time = Time.time;

		m_Material.SetFloat("screenLinesNum",  stretchResolution.value);
		m_Material.SetFloat("time_", _time);
		ParamSwitch(m_Material,  twitchHorizontal.value, "VHS_TWITCH_H_ON");
		m_Material.SetFloat("twitchHFreq",  horizontalFreq.value);
		ParamSwitch(m_Material,  twitchVertical.value, "VHS_TWITCH_V_ON");
		m_Material.SetFloat("twitchVFreq",  verticalFreq.value);
		ParamSwitch(m_Material,  stretch.value, "VHS_STRETCH_ON");

		ParamSwitch(m_Material,  jitterHorizontal.value, "VHS_JITTER_H_ON");
		m_Material.SetFloat("jitterHAmount",  jitterHorizontalAmount.value);

		ParamSwitch(m_Material,  jitterVertical.value, "VHS_JITTER_V_ON");
		m_Material.SetFloat("jitterVAmount",  jitterVerticalAmount.value);
		m_Material.SetFloat("jitterVSpeed",  jitterVerticalSpeed.value);


		HDUtils.DrawFullScreen(cmd, m_Material, destination);
    }
	private void ParamSwitch(Material mat, bool paramValue, string paramName)
	{
		if (paramValue) mat.EnableKeyword(paramName);
		else mat.DisableKeyword(paramName);
	}

	public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}
