using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Retro Look Pro/NTSCEncode_RLPRO")]
public sealed class NTSCEncode_RLPRO : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
    [Tooltip("Blur size.")]
    public ClampedFloatParameter blur = new ClampedFloatParameter(0.83f, 0.01f, 2f);
    [Tooltip("Brigtness.")]
    public ClampedFloatParameter brigtness = new ClampedFloatParameter(3f, 1f, 40f);
    [Tooltip("Floating lines speed")]
    public ClampedFloatParameter lineSpeed = new ClampedFloatParameter(0.01f, 0f, 10f);

    Material m_Material;
	private float T;
    public bool IsActive() => m_Material != null && intensity.value > 0f;
    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/NTSCEncode_RLPRO") != null)
            m_Material = new Material(Shader.Find("Hidden/Shader/NTSCEncode_RLPRO"));
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;
        T += Time.deltaTime;
        m_Material.SetFloat("T", T);
        m_Material.SetFloat("_Intensity", intensity.value);
        m_Material.SetTexture("_InputTexture", source);
        m_Material.SetFloat("Bsize", brigtness.value);
        m_Material.SetFloat("val1", lineSpeed.value);
        m_Material.SetFloat("val2", blur.value);
        
        HDUtils.DrawFullScreen(cmd, m_Material, destination);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}
