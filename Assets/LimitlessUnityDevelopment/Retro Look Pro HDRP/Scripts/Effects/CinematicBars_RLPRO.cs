using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Retro Look Pro/CinematicBars_RLPRO")]
public sealed class CinematicBars_RLPRO : CustomPostProcessVolumeComponent, IPostProcessComponent
{
	[Tooltip("Controls the intensity of the effect.")]
	public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
	[Range(0.5f, 0.01f), Tooltip("Black bars amount (width)")]
	public ClampedFloatParameter amount = new ClampedFloatParameter(0.5f, 0.01f, 0.51f);
	[Range(0f, 1f), Tooltip("Fade black bars.")]
	public ClampedFloatParameter fade = new ClampedFloatParameter(1f, 0f, 1f);
	Material m_Material;

	public bool IsActive() => m_Material != null && intensity.value > 0f;

	public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

	public override void Setup()
	{
		if (Shader.Find("Hidden/Shader/CinematicBarsEffect_RLPRO") != null)
			m_Material = new Material(Shader.Find("Hidden/Shader/CinematicBarsEffect_RLPRO"));
	}

	public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
	{
		if (m_Material == null)
			return;
		m_Material.SetFloat("_Stripes", 0.51f - amount.value);
		m_Material.SetFloat("_Fade", fade.value);
		m_Material.SetFloat("_Intensity", intensity.value);
		m_Material.SetTexture("_InputTexture", source);
		HDUtils.DrawFullScreen(cmd, m_Material, destination);
	}

	public override void Cleanup()
	{
		CoreUtils.Destroy(m_Material);
	}
}
