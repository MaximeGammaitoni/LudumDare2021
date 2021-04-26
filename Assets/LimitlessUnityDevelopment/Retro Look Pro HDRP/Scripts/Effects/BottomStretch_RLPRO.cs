using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Retro Look Pro/BottomStretch_RLPRO")]
public sealed class BottomStretch_RLPRO : CustomPostProcessVolumeComponent, IPostProcessComponent
{
	[Tooltip("Controls the intensity of the effect.")]
	public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
	[Tooltip("Height of Noise.")]
	public ClampedFloatParameter height = new ClampedFloatParameter(0.2f, 0.01f, 0.5f);
	[Space]
	[Tooltip("Stretch noise distortion.")]
	public BoolParameter distort = new BoolParameter(true);
	[Tooltip("Noise distortion frequency.")]
	public ClampedFloatParameter frequency = new ClampedFloatParameter(0.2f, 0.1f, 100f);
	[Tooltip("Noise distortion amplitude.")]
	public ClampedFloatParameter amplitude = new ClampedFloatParameter(0.2f, 0.01f, 200f);
	[Tooltip("Enable noise distortion random frequency.")]
	public BoolParameter distortRandomly = new BoolParameter(true);
	//
	Material m_Material;
	private float T;
	public bool IsActive() => m_Material != null && intensity.value > 0f;

	public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

	public override void Setup()
	{
		if (Shader.Find("Hidden/Shader/BottomStretchEffect_RLPRO") != null)
			m_Material = new Material(Shader.Find("Hidden/Shader/BottomStretchEffect_RLPRO"));
	}

	public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
	{
		if (m_Material == null)
			return;
		T += Time.deltaTime;
		m_Material.SetFloat("Time", T);
		m_Material.SetFloat("_NoiseBottomHeight", height.value);
		m_Material.SetFloat("frequency", frequency.value);
		m_Material.SetFloat("amplitude", amplitude.value);
		m_Material.SetFloat("_Intensity", intensity.value);
		m_Material.SetTexture("_InputTexture", source);
		if (distort.value)
		{
			HDUtils.DrawFullScreen(cmd, m_Material, destination, shaderPassId: distortRandomly.value ? 1 : 0);
		}
		else
			HDUtils.DrawFullScreen(cmd, m_Material, destination, shaderPassId: 2);
	}

	public override void Cleanup()
	{
		CoreUtils.Destroy(m_Material);
	}
}
