using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Retro Look Pro/AnalogTVNoise_RLPRO")]
public sealed class AnalogTVNoise_RLPRO : CustomPostProcessVolumeComponent, IPostProcessComponent
{
	[Tooltip("Controls the intensity of the effect.")]
	public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
	[Tooltip("Option enables static noise (without movement).")]
	public BoolParameter staticNoise = new BoolParameter(false);
	[Tooltip("Horizontal/Vertical Noise lines.")]
	public BoolParameter Horizontal = new BoolParameter(true);
	[Range(0f, 1f), Tooltip("Effect Fade.")]
	public ClampedFloatParameter Fade = new ClampedFloatParameter(1f, 0f, 1f);
	[Range(0f, 60f), Tooltip("Noise bar width.")]
	public ClampedFloatParameter barWidth = new ClampedFloatParameter(21f, 0f, 60f);
	[Range(0f, 60f), Tooltip("Noise tiling.")]
	public Vector2Parameter tile = new Vector2Parameter(new Vector2(1, 1));
	[Range(0f, 1f), Tooltip("Noise texture angle.")]
	public ClampedFloatParameter textureAngle = new ClampedFloatParameter(1f, 0f, 1f);
	[Range(0f, 100f), Tooltip("Noise bar edges cutoff.")]
	public ClampedFloatParameter edgeCutOff = new ClampedFloatParameter(0f, 0f, 100f);
	[Range(-1f, 1f), Tooltip("Noise cutoff.")]
	public ClampedFloatParameter CutOff = new ClampedFloatParameter(1f, -1f, 1f);
	[Range(-10f, 10f), Tooltip("Noise bars speed.")]
	public ClampedFloatParameter barSpeed = new ClampedFloatParameter(1f, -60f, 60f);
	[Tooltip("Noise texture.")]
	public TextureParameter texture = new TextureParameter(null);
	//
	Material m_Material;
	float TimeX;
	public bool IsActive() => m_Material != null && intensity.value > 0f;

	public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

	public override void Setup()
	{
		if (Shader.Find("Hidden/Shader/AnalogTVNoiseEffect_RLPRO") != null)
			m_Material = new Material(Shader.Find("Hidden/Shader/AnalogTVNoiseEffect_RLPRO"));
	}

	public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
	{
		if (m_Material == null)
			return;
		TimeX += Time.deltaTime;
		if (TimeX > 100) TimeX = 0;

		m_Material.SetFloat("TimeX", TimeX);
		m_Material.SetFloat("_Fade", Fade.value);
		if (texture.value != null)
			m_Material.SetTexture("_Pattern", texture.value);
		m_Material.SetFloat("barHeight", barWidth.value);
		m_Material.SetFloat("barSpeed", barSpeed.value);
		m_Material.SetFloat("cut", CutOff.value);
		m_Material.SetFloat("edgeCutOff", edgeCutOff.value);
		m_Material.SetFloat("angle", textureAngle.value);
		m_Material.SetFloat("tileX", tile.value.x);
		m_Material.SetFloat("tileY", tile.value.y);
		m_Material.SetFloat("horizontal", Horizontal.value ? 1 : 0);
		if (!staticNoise.value)
		{
			m_Material.SetFloat("_OffsetNoiseX", UnityEngine.Random.Range(0f, 0.6f));
			m_Material.SetFloat("_OffsetNoiseY", UnityEngine.Random.Range(0f, 0.6f));
		}
		m_Material.SetFloat("_Intensity", intensity.value);
		m_Material.SetTexture("_InputTexture", source);
		HDUtils.DrawFullScreen(cmd, m_Material, destination);
	}

	public override void Cleanup()
	{
		CoreUtils.Destroy(m_Material);
	}
}
