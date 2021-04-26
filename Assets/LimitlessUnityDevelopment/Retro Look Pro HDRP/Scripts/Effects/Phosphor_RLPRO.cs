using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;
using UnityEngine.Experimental.Rendering;

[Serializable, VolumeComponentMenu("Post-processing/Retro Look Pro/Phosphor_RLPRO")]
public sealed class Phosphor_RLPRO : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
	public ClampedFloatParameter width = new ClampedFloatParameter(0.4f, 0f, 20f);
	public ClampedFloatParameter amount = new ClampedFloatParameter(0.5f, 0f, 1f);
	public ClampedFloatParameter fade = new ClampedFloatParameter(1f, 0f, 1f);
	private RTHandle texTape = null;
	bool stop;
	Material m_Material;
	float T;
    public bool IsActive() => m_Material != null && intensity.value > 0f;

    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/Phosphor_RLPRO") != null)
            m_Material = new Material(Shader.Find("Hidden/Shader/Phosphor_RLPRO"));

		texTape = RTHandles.Alloc(Vector2.one, TextureXR.slices, colorFormat: GraphicsFormat.B10G11R11_UFloatPack32, dimension: TextureDimension.Tex2DArray, enableRandomWrite: true, useDynamicScale: true, name: "texLast");//RTHandles.Alloc(texWidth, texHeight, TextureXR.slices, colorFormat: GraphicsFormat.B10G11R11_UFloatPack32, dimension: TextureDimension.Tex2DArray, enableRandomWrite: true, useDynamicScale: true, name: "previous");
		stop = false;
	}

	public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;

		if (!stop)
		{
			stop = true;
			HDUtils.DrawFullScreen(cmd, m_Material, texTape, shaderPassId: 1);
		}
		HDUtils.DrawFullScreen(cmd, m_Material, texTape, shaderPassId: 1);
		m_Material.SetTexture("_Tex", texTape);
		T = Time.time;
        m_Material.SetFloat("_Intensity", intensity.value);
		m_Material.SetFloat("T", T);
		m_Material.SetFloat("speed", width.value);
		m_Material.SetFloat("amount", amount.value+1);
		m_Material.SetFloat("fade", fade.value);
		m_Material.SetTexture("_InputTexture", source);
        HDUtils.DrawFullScreen(cmd, m_Material, destination, shaderPassId: 0);
		texTape.rt.Release();
	}

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}
