using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;
using RetroLookPro.Enums;
[Serializable]
public sealed class WarpModeParameter : VolumeParameter<WarpMode> { };

[Serializable, VolumeComponentMenu("Post-processing/Retro Look Pro/TV_RLPRO")]
public sealed class TV_RLPRO : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
	[Range(0f, 1f), Tooltip("Effect fade.")]
	public ClampedFloatParameter fade = new ClampedFloatParameter(1,0,1);
	[Range(0f, 2f), Tooltip("Dark areas adjustment.")]
	public ClampedFloatParameter maskDark = new ClampedFloatParameter(0.5f, 0, 2f);
	[Range(0f, 2f), Tooltip("Light areas adjustment.")]
	public ClampedFloatParameter maskLight = new ClampedFloatParameter(1.5f,0,2f);
	[Range(-8f, -16f), Tooltip("Dark areas fine tune.")]
	public ClampedFloatParameter hardScan = new ClampedFloatParameter(-8f,-8f,16f);
	[Range(1f, 16f), Tooltip("Effect resolution.")]
	public ClampedFloatParameter resScale = new ClampedFloatParameter(4f,1f,16f);
	[Range(-3f, 1f), Tooltip("pixels sharpness.")]
	public ClampedFloatParameter hardPix = new ClampedFloatParameter(-3f,-3f,1f);
	[Tooltip("Warp mode.")]
	public WarpModeParameter warpMode = new WarpModeParameter { };
	[Tooltip("Warp picture.")]
	public Vector2Parameter warp = new Vector2Parameter (new Vector2(0f, 0f) );
	public FloatParameter scale = new FloatParameter (0.5f);
	Material m_Material;

    public bool IsActive() => m_Material != null && intensity.value > 0f;

    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/TV_RLPRO_HDRP") != null)
            m_Material = new Material(Shader.Find("Hidden/Shader/TV_RLPRO_HDRP"));
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;

        m_Material.SetFloat("_Intensity", intensity.value);
        m_Material.SetTexture("_InputTexture", source);

		m_Material.SetFloat("fade",  fade.value);
		m_Material.SetFloat("scale",  scale.value);
		m_Material.SetFloat("hardScan",  hardScan.value);
		m_Material.SetFloat("hardPix",  hardPix.value);
		m_Material.SetFloat("resScale",  resScale.value);
		m_Material.SetFloat("maskDark",  maskDark.value);
		m_Material.SetFloat("maskLight",  maskLight.value);
		m_Material.SetVector("warp",  warp.value);

		HDUtils.DrawFullScreen(cmd, m_Material, destination, shaderPassId: warpMode == WarpMode.SimpleWarp ? 0 : 1);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}
