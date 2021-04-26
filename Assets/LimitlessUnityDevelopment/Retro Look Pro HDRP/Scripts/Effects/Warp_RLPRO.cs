using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;
using RetroLookPro.Enums;

[Serializable, VolumeComponentMenu("Post-processing/Retro Look Pro/Warp_RLPRO")]
public sealed class Warp_RLPRO : CustomPostProcessVolumeComponent, IPostProcessComponent
{
	[Tooltip("Controls the intensity of the effect.")]
	public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
	[Range(0f, 1f), Tooltip("Dark areas adjustment.")]
	public ClampedFloatParameter fade = new ClampedFloatParameter(1f, 0f, 1f);
	[Tooltip("Warp mode.")]
	public WarpModeParameter warpMode = new WarpModeParameter();
	[Tooltip("Warp image corners on x/y axes.")]
	public Vector2Parameter warp = new Vector2Parameter(new Vector2(0.03125f, 0.04166f));
	[Tooltip("Warp picture center.")]
	public FloatParameter scale = new FloatParameter(1f);
	[Tooltip("Enables Clamp sampler state.")]
	public BoolParameter clampSampler = new BoolParameter(true);
	Material m_Material;

	public bool IsActive() => m_Material != null && intensity.value > 0f;

	public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

	public override void Setup()
	{
		if (Shader.Find("Hidden/Shader/WarpEffect_RLPRO") != null)
			m_Material = new Material(Shader.Find("Hidden/Shader/WarpEffect_RLPRO"));
	}

	public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
	{
		if (m_Material == null)
			return;
		m_Material.SetFloat("fade", fade.value);
		m_Material.SetFloat("scale", scale.value);
		m_Material.SetVector("warp", warp.value);
		m_Material.SetFloat("clamp", clampSampler.value ? 1 : 0);
		m_Material.SetFloat("_Intensity", intensity.value);
		m_Material.SetTexture("_InputTexture", source);
		HDUtils.DrawFullScreen(cmd, m_Material, destination, shaderPassId: warpMode == WarpMode.SimpleWarp ? 0 : 1);
	}

	public override void Cleanup()
	{
		CoreUtils.Destroy(m_Material);
	}
}
