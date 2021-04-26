using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Retro Look Pro/VHSScanlines_RLPRO")]
public sealed class VHSScanlines_RLPRO : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
    [Tooltip("Lines color.")]
    public ColorParameter scanLinesColor = new ColorParameter(new Color());
    [Tooltip("Amount of scanlines.")]
    public FloatParameter scanLines = new FloatParameter(1.5f);
    [Tooltip("Lines speed.")]
    public FloatParameter speed = new FloatParameter(0);
    [Tooltip("Effect fade.")]
    public ClampedFloatParameter fade = new ClampedFloatParameter(1f, 0f, 1f);
    [Tooltip("Enable horizontal lines.")]
    public BoolParameter horizontal = new BoolParameter(true);
    [Tooltip("distortion.")]
    public ClampedFloatParameter distortion = new ClampedFloatParameter(0.2f, 0f, 0.5f);
    [Tooltip("distortion1.")]
    public FloatParameter distortion1 = new FloatParameter(0);
    [Tooltip("distortion2.")]
    public FloatParameter distortion2 = new FloatParameter(0);
    [Tooltip("Scale lines size.")]
    public FloatParameter scale = new FloatParameter(1);
    Material m_Material;
    private int pass;
    private float T;
    public bool IsActive() => m_Material != null && intensity.value > 0f;

    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/VHSScanlinesEffect_RLPRO") != null)
            m_Material = new Material(Shader.Find("Hidden/Shader/VHSScanlinesEffect_RLPRO"));
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;
        T += Time.deltaTime;

        m_Material.SetFloat("Time", T);
        m_Material.SetFloat("_ScanLines", scanLines.value);
        m_Material.SetFloat("speed", speed.value);
        m_Material.SetFloat("_OffsetDistortion", distortion.value);
        m_Material.SetFloat("fade", fade.value);
        m_Material.SetFloat("sferical", distortion1.value);
        m_Material.SetFloat("barrel", distortion2.value);
        m_Material.SetFloat("scale", scale.value);
        m_Material.SetColor("_ScanLinesColor", scanLinesColor.value);
        if (horizontal.value)
        {
            if (distortion.value != 0)
                pass = 1;
            else
                pass = 0;
        }
        else
        {
            if (distortion.value != 0)
                pass = 3;
            else
                pass = 2;
        }
        m_Material.SetFloat("_Intensity", intensity.value);
        m_Material.SetTexture("_InputTexture", source);
        HDUtils.DrawFullScreen(cmd, m_Material, destination, shaderPassId: pass);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}
