using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;
using RetroLookPro.Enums;
[Serializable]
public sealed class VignetteModeParameter : VolumeParameter<VignetteShape> { };
[Serializable, VolumeComponentMenu("Post-processing/Retro Look Pro/UltimateVignette_RLPRO")]
public sealed class UltimateVignette_RLPRO : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
	public VignetteModeParameter vignetteShape = new VignetteModeParameter { };
	[Tooltip(".")]
	public Vector2Parameter center = new Vector2Parameter (new Vector2(0.5f, 0.5f));
	[Range(0f, 100), Tooltip(".")]
	public ClampedFloatParameter vignetteAmount = new ClampedFloatParameter(50f,0f, 100);
	[Range(-1f, -100f), Tooltip(".")]
	public ClampedFloatParameter vignetteFineTune = new ClampedFloatParameter(-10f,-100f, -10f);
	[Range(0f, 100f), Tooltip("Scanlines width.")]
	public ClampedFloatParameter edgeSoftness = new ClampedFloatParameter(1.5f,0f, 100f);
	[Range(200f, 0f), Tooltip("Horizontal/Vertical scanlines.")]
	public ClampedFloatParameter edgeBlend = new ClampedFloatParameter(0f,0f, 200f);
	[Range(0f, 200f), Tooltip(".")]
	public ClampedFloatParameter innerColorAlpha = new ClampedFloatParameter(0f,0f, 200f);
	public ColorParameter innerColor = new ColorParameter (new Color());
	Material m_Material;

    public bool IsActive() => m_Material != null && intensity.value > 0f;

    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/UltimateVignetteEffect_RLPRO") != null)
            m_Material = new Material(Shader.Find("Hidden/Shader/UltimateVignetteEffect_RLPRO"));
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;
		m_Material.DisableKeyword("VIGNETTE_CIRCLE");
		m_Material.DisableKeyword("VIGNETTE_ROUNDEDCORNERS");
		switch ( vignetteShape.value)
		{
			case VignetteShape.circle:
				m_Material.EnableKeyword("VIGNETTE_CIRCLE");
				break;
			case VignetteShape.roundedCorners:
				m_Material.EnableKeyword("VIGNETTE_ROUNDEDCORNERS");
				break;
		}
		m_Material.SetVector("_Params", new Vector4( edgeSoftness.value * 0.01f,  vignetteAmount.value * 0.02f,  innerColorAlpha.value * 0.01f,  edgeBlend.value * 0.01f));
		m_Material.SetColor("_InnerColor",  innerColor.value);
		m_Material.SetVector("_Center",  center.value);
		m_Material.SetVector("_Params1", new Vector2( vignetteFineTune.value, 0.8f));
		m_Material.SetFloat("_Intensity", intensity.value);
        m_Material.SetTexture("_InputTexture", source);
        HDUtils.DrawFullScreen(cmd, m_Material, destination);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}
