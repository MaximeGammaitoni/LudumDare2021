using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Retro Look Pro/BottomNoise_RLPRO")]
public sealed class BottomNoise_RLPRO : CustomPostProcessVolumeComponent, IPostProcessComponent
{
	[Tooltip("Controls the intensity of the effect.")]
	public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
	[Range(0.01f, 0.5f), Tooltip("Noise Height.")]
	public ClampedFloatParameter height = new ClampedFloatParameter(0.2f, 0.01f, 0.5f);
	[Tooltip("Noise tiling.")]
	public Vector2Parameter tile = new Vector2Parameter(new Vector2(1, 1));
	[Range(0f, 3f), Tooltip("Noise intensity.")]
	public ClampedFloatParameter intencity = new ClampedFloatParameter(1.5f, 0f, 3f);

	public TextureParameter noiseTexture = new TextureParameter(null);
	Material m_Material;

	public bool IsActive() => m_Material != null && intensity.value > 0f;

	public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

	public override void Setup()
	{
		if (Shader.Find("Hidden/Shader/BottomNoiseEffect_RLPRO") != null)
			m_Material = new Material(Shader.Find("Hidden/Shader/BottomNoiseEffect_RLPRO"));
	}

	public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
	{
		if (m_Material == null)
			return;

		if (m_Material.HasProperty("_OffsetNoiseY"))
		{
			float offsetNoise1 = m_Material.GetFloat("_OffsetNoiseY");
			m_Material.SetFloat("_OffsetNoiseY", offsetNoise1 + UnityEngine.Random.Range(-0.05f, 0.05f));
		}
		m_Material.SetFloat("_OffsetNoiseX", UnityEngine.Random.Range(0f, 1.0f));

		m_Material.SetFloat("_NoiseBottomHeight", height.value);

		m_Material.SetFloat("_NoiseBottomIntensity", intencity.value);
		if (noiseTexture.value != null)
		{
			m_Material.SetTexture("_NoiseTexture", noiseTexture.value);
		}
		m_Material.SetFloat("tileX", tile.value.x);
		m_Material.SetFloat("tileY", tile.value.y);
		m_Material.SetFloat("_Intensity", intensity.value);
		m_Material.SetTexture("_InputTexture", source);
		HDUtils.DrawFullScreen(cmd, m_Material, destination);
	}

	public override void Cleanup()
	{
		CoreUtils.Destroy(m_Material);
	}
}
