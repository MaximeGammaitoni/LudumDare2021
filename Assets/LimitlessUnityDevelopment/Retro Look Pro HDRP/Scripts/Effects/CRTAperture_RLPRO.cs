using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Retro Look Pro/CRTAperture_RLPRO")]
public sealed class CRTAperture_RLPRO : CustomPostProcessVolumeComponent, IPostProcessComponent
{
	[Tooltip("Controls the intensity of the effect.")]
	public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

	[Tooltip("Glow Halation.")]
	public ClampedFloatParameter GlowHalation = new ClampedFloatParameter(4.27f, 0f, 5f);
	[Tooltip("Glow Difusion.")]
	public ClampedFloatParameter GlowDifusion = new ClampedFloatParameter(0.83f, 0f, 2f);
	[Tooltip("Mask Colors.")]
	public ClampedFloatParameter MaskColors = new ClampedFloatParameter(0.57f, 0f, 5f);
	[Tooltip("Mask Strength.")]
	public ClampedFloatParameter MaskStrength = new ClampedFloatParameter(0.318f, 0f, 1f);
	[Tooltip("Gamma Input.")]
	public ClampedFloatParameter GammaInput = new ClampedFloatParameter(1.12f, 0f, 5f);
	[Tooltip("Gamma Output.")]
	public ClampedFloatParameter GammaOutput = new ClampedFloatParameter(0.89f, 0f, 5f);
	[Tooltip("Brightness.")]
	public ClampedFloatParameter Brightness = new ClampedFloatParameter(0.85f, 0f, 2.5f);

	Material m_Material;

	public bool IsActive() => m_Material != null && intensity.value > 0f;

	public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

	public override void Setup()
	{
		if (Shader.Find("Hidden/Shader/CRTAperture_RLPRO") != null)
			m_Material = new Material(Shader.Find("Hidden/Shader/CRTAperture_RLPRO"));
	}

	public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
	{
		if (m_Material == null)
			return;
		m_Material.SetFloat("GLOW_HALATION", GlowHalation.value);
		m_Material.SetFloat("GLOW_DIFFUSION", GlowDifusion.value);
		m_Material.SetFloat("MASK_COLORS", MaskColors.value);
		m_Material.SetFloat("MASK_STRENGTH", MaskStrength.value);
		m_Material.SetFloat("GAMMA_INPUT", GammaInput.value);
		m_Material.SetFloat("GAMMA_OUTPUT", GammaOutput.value);
		m_Material.SetFloat("BRIGHTNESS", Brightness.value);
		m_Material.SetFloat("_Intensity", intensity.value);
		m_Material.SetTexture("_InputTexture", source);
		HDUtils.DrawFullScreen(cmd, m_Material, destination);
	}

	public override void Cleanup()
	{
		CoreUtils.Destroy(m_Material);
	}
}
