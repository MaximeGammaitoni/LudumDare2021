using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;
public enum FisheyeTypeEnum { Default = 0, Hyperspace = 1 }
[Serializable]
public sealed class FisheyeTypeParameter : VolumeParameter<FisheyeTypeEnum> { };
[Serializable, VolumeComponentMenu("Post-processing/Retro Look Pro/Fisheye_RLPRO")]
public sealed class Fisheye_RLPRO : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
	public FisheyeTypeParameter fisheyeType = new FisheyeTypeParameter { };
	[Range(0f, 50f), Tooltip("Bend Amount.")]
	public ClampedFloatParameter bend = new ClampedFloatParameter(1f,0f, 50f);
	[Range(0f, 50f), Tooltip("Cutoff on X axes.")]
	public ClampedFloatParameter cutOffX = new ClampedFloatParameter(0.5f,0f, 50f);
	[Range(0f, 50f), Tooltip("Cutoff on Y axes.")]
	public ClampedFloatParameter cutOffY = new ClampedFloatParameter(0.5f,0f, 50f);
	[Range(0f, 50f), Tooltip("Fade on X axes.")]
	public ClampedFloatParameter fadeX = new ClampedFloatParameter(1f,0f, 50f);
	[Range(0f, 50f), Tooltip("Fade on Y axes.")]
	public ClampedFloatParameter fadeY = new ClampedFloatParameter(1f,0f, 50f);
	[Range(0.001f, 50f), Tooltip("Fisheye size.")]
	public ClampedFloatParameter size = new ClampedFloatParameter(1f,0.001f, 50f);
	Material m_Material;

    public bool IsActive() => m_Material != null && intensity.value > 0f;

    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/FisheyeEffect_RLPRO") != null)
            m_Material = new Material(Shader.Find("Hidden/Shader/FisheyeEffect_RLPRO"));
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;
		ParamSwitch(m_Material, true, "VHS_FISHEYE_ON");
		m_Material.SetFloat("cutoffX",  cutOffX.value);
		m_Material.SetFloat("cutoffY",  cutOffY.value);
		m_Material.SetFloat("cutoffFadeX",  fadeX.value);
		m_Material.SetFloat("cutoffFadeY",  fadeY.value);
		ParamSwitch(m_Material,  fisheyeType.value == FisheyeTypeEnum.Hyperspace, "VHS_FISHEYE_HYPERSPACE");
		m_Material.SetFloat("fisheyeBend",  bend.value);
		m_Material.SetFloat("fisheyeSize",  size.value);
		m_Material.SetFloat("_Intensity", intensity.value);
        m_Material.SetTexture("_InputTexture", source);
        HDUtils.DrawFullScreen(cmd, m_Material, destination);
    }
	private void ParamSwitch(Material mat, bool paramValue, string paramName)
	{
		if (paramValue) mat.EnableKeyword(paramName);
		else mat.DisableKeyword(paramName);
	}
	public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}
