using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Retro Look Pro/CustomTexture_RLPRO")]
public sealed class CustomTexture_RLPRO : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
	[Tooltip("Сustom texture.")]
	public TextureParameter texture = new TextureParameter(null);
	[Range(0f, 1f), Tooltip("Passthrough custom texture alpha chanel.")]
	public BoolParameter alpha = new BoolParameter(true);
    [Range(0f, 1f), Tooltip("fade parameter.")]
    public ClampedFloatParameter fade = new ClampedFloatParameter(1f, 0f, 1f);
    //
    Material m_Material;

    public bool IsActive() => m_Material != null && intensity.value > 0f;

    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/CustomTextureEffect_RLPRO") != null)
            m_Material = new Material(Shader.Find("Hidden/Shader/CustomTextureEffect_RLPRO"));
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;
		if (texture.value != null)
			m_Material.SetTexture("_CustomTexture", texture.value);
		m_Material.SetFloat("fade", fade.value);

        m_Material.SetFloat("alpha", alpha.value?1:0);
        m_Material.SetFloat("_Intensity", intensity.value);
        m_Material.SetTexture("_InputTexture", source);
        HDUtils.DrawFullScreen(cmd, m_Material, destination);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}
