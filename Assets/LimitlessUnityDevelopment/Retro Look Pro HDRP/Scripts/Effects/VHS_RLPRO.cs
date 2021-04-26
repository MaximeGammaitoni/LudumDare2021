using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;
using RetroLookPro.Enums;
[Serializable]
public sealed class BlendModeParameter : VolumeParameter<BlendingMode> { };
[Serializable, VolumeComponentMenu("Post-processing/Retro Look Pro/VHS_RLPRO")]
public sealed class VHS_RLPRO : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
	[Tooltip("Color Offset.")]
	public ClampedFloatParameter colorOffset = new ClampedFloatParameter(4f,0f, 50f);
	[Tooltip("Color Offset Angle.")]
	public ClampedFloatParameter colorOffsetAngle = new ClampedFloatParameter(1,-3f, 3f);
	[Space]
	[Tooltip("Vertical twitch frequency.")]
	public ClampedFloatParameter verticalOffsetFrequency = new ClampedFloatParameter(1f,0f, 100f);
	[Tooltip("Amount of vertical twitch. ")]
	public ClampedFloatParameter verticalOffset = new ClampedFloatParameter(0.01f,0f, 1f);
	[Tooltip("Amount of horizontal distortion.")]
	public ClampedFloatParameter offsetDistortion = new ClampedFloatParameter(0.008f, 0f, 0.5f);
	[Space]
	[Tooltip("Noise texture.")]
	public TextureParameter noiseTexture = new TextureParameter (null);
	public BlendModeParameter blendMode = new BlendModeParameter { };
	public Vector2Parameter tile = new Vector2Parameter (new Vector2(1, 1));
	[Space]
	[Tooltip("Intensity of noise texture.")]
	public ClampedFloatParameter _textureIntensity = new ClampedFloatParameter(1f,0f, 1f);
	[Space]
	public BoolParameter smoothCut = new BoolParameter(false);
	[Tooltip("Amount of horizontal distortion.")]
	public ClampedIntParameter iterations = new ClampedIntParameter(5,0, 30);
	[Tooltip("Amount of horizontal distortion.")]
	public ClampedFloatParameter smoothSize = new ClampedFloatParameter(0.1f,0f, 0.5f);
	[Tooltip("Amount of horizontal distortion.")]
	public ClampedFloatParameter deviation = new ClampedFloatParameter(0.1f,0f, 0.5f);
	[Space]
	[Tooltip("Cut off.")]
	public ClampedFloatParameter _textureCutOff = new ClampedFloatParameter(1f,-1f, 1f);
	[Space]
	[Tooltip("black bars")]
	public ClampedFloatParameter stripes = new ClampedFloatParameter(0.01f, 0.01f, 0.5f);
	[Space]
	public BoolParameter unscaledTime = new BoolParameter (false);
	Material m_Material;
	private float T;
	public bool IsActive() => m_Material != null && intensity.value > 0f;

    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/VHSEffect_RLPRO") != null)
            m_Material = new Material(Shader.Find("Hidden/Shader/VHSEffect_RLPRO"));
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;
		if (! unscaledTime.value)
			T += Time.deltaTime;
		else
			T += Time.unscaledDeltaTime;
		m_Material.SetFloat("Time", T);
		if (UnityEngine.Random.Range(0, 100 -  verticalOffsetFrequency.value) <= 5)
		{
			if ( verticalOffset == 0.0f)
			{
				m_Material.SetFloat("_OffsetPosY",  verticalOffset.value);
			}
			if ( verticalOffset.value > 0.0f)
			{
				m_Material.SetFloat("_OffsetPosY",  verticalOffset.value - UnityEngine.Random.Range(0f,  verticalOffset.value));
			}
			else if ( verticalOffset.value < 0.0f)
			{
				m_Material.SetFloat("_OffsetPosY",  verticalOffset.value + UnityEngine.Random.Range(0f, - verticalOffset.value));
			}
		}

		m_Material.SetFloat("iterations",  iterations.value);
		m_Material.SetFloat("smoothSize",  smoothSize.value);
		m_Material.SetFloat("_StandardDeviation",  deviation.value);

		m_Material.SetFloat("tileX",  tile.value.x);
		m_Material.SetFloat("smooth",  smoothCut.value ? 1 : 0);
		m_Material.SetFloat("tileY",  tile.value.y);
		m_Material.SetFloat("_OffsetDistortion", offsetDistortion.value);
		m_Material.SetFloat("_Stripes",  0.51f-stripes.value);

		m_Material.SetVector("_OffsetColorAngle", new Vector2(Mathf.Sin( colorOffsetAngle.value),
				Mathf.Cos( colorOffsetAngle.value)));
		m_Material.SetFloat("_OffsetColor",  colorOffset.value * 0.001f);

		m_Material.SetFloat("_OffsetNoiseX", UnityEngine.Random.Range(-0.4f, 0.4f));
		if ( noiseTexture.value != null)
			m_Material.SetTexture("_SecondaryTex",  noiseTexture.value);

		if (m_Material.HasProperty("_OffsetNoiseY"))
		{
		float offsetNoise = m_Material.GetFloat("_OffsetNoiseY");
		m_Material.SetFloat("_OffsetNoiseY", offsetNoise + UnityEngine.Random.Range(-0.03f, 0.03f));
		}
		m_Material.SetFloat("_TexIntensity",  _textureIntensity.value);
		m_Material.SetFloat("_TexCut",  _textureCutOff.value);

		m_Material.SetFloat("_Intensity", intensity.value);
        m_Material.SetTexture("_InputTexture", source);
        HDUtils.DrawFullScreen(cmd, m_Material, destination,shaderPassId: (int)blendMode.value);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}
