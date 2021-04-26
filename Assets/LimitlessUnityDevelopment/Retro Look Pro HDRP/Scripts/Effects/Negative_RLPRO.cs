using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Retro Look Pro/Negative_RLPRO")]
public sealed class Negative_RLPRO : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
	[Range(0f, 2f), Tooltip("Brightness.")]
	public ClampedFloatParameter luminosity = new ClampedFloatParameter(0f, 0f, 1.1f);
	[Range(0f, 1f), Tooltip("Vignette amount.")]
	public ClampedFloatParameter vignette = new ClampedFloatParameter(1f, 0f, 1f);
	[Range(0f, 1f), Tooltip("Contrast amount.")]
	public ClampedFloatParameter contrast = new ClampedFloatParameter(0.7f, 0f, 1f);
	[Range(0f, 1f), Tooltip("Negative amount.")]
	public ClampedFloatParameter negative = new ClampedFloatParameter(1f, 0f, 1f);
	Material m_Material;
	float T;

	public bool IsActive() => m_Material != null && intensity.value > 0f;

    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/NegativeEffect_RLPRO") != null)
            m_Material = new Material(Shader.Find("Hidden/Shader/NegativeEffect_RLPRO"));
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;

        m_Material.SetFloat("_Intensity", intensity.value);
        m_Material.SetTexture("_InputTexture", source);
		T += Time.deltaTime;
		if (T > 100) T = 0;

		m_Material.SetFloat("T", T);
		m_Material.SetFloat("Luminosity", 2 - luminosity.value);
		m_Material.SetFloat("Contrast", 1-contrast.value);
		m_Material.SetFloat("Vignette", 1 - vignette.value);
		m_Material.SetFloat("Negative", negative.value);
		HDUtils.DrawFullScreen(cmd, m_Material, destination);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}
