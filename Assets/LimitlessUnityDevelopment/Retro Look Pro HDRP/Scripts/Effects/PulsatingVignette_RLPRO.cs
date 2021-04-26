using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Retro Look Pro/PulsatingVignette_RLPRO")]
public sealed class PulsatingVignette_RLPRO : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
	[Range(0.001f, 50f), Tooltip("Vignette shake speed.")]
	public ClampedFloatParameter speed = new ClampedFloatParameter(1f,0.001f, 50f);
	[Range(0.001f, 50f), Tooltip("Vignette amount.")]
	public ClampedFloatParameter amount = new ClampedFloatParameter(1f,0.001f, 50f);
	Material m_Material;
	private float T;
	public bool IsActive() => m_Material != null && intensity.value > 0f;

    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/PulsatingVignetteEffect_RLPRO") != null)
            m_Material = new Material(Shader.Find("Hidden/Shader/PulsatingVignetteEffect_RLPRO"));
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;
		T += Time.deltaTime;
		m_Material.SetFloat("Time", T);
		m_Material.SetFloat("vignetteSpeed", speed.value);
		m_Material.SetFloat("vignetteAmount", amount.value);
		m_Material.SetFloat("_Intensity", intensity.value);
        m_Material.SetTexture("_InputTexture", source);
        HDUtils.DrawFullScreen(cmd, m_Material, destination);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}
