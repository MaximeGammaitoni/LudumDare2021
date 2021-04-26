using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Experimental.Rendering;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Retro Look Pro/LowRes_RLPRO")]
public sealed class LowRes_RLPRO : CustomPostProcessVolumeComponent, IPostProcessComponent
{
	[Tooltip("Controls the intensity of the effect.")]
	public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
	[Tooltip("Lower value = more downscale.")]
	public ClampedFloatParameter downscale = new ClampedFloatParameter(0f, 0.1f, 1f);
	public bool IsActive() => m_Material != null && intensity.value > 0f;
	//
	Material m_Material;
	RTHandle lowresTexture;
	RTHandle highresTexture;
	float m_PrevValue;
	public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

	public override void Setup()
	{
		if (Shader.Find("Hidden/Shader/LowResolution_RLPRO") != null)
			m_Material = new Material(Shader.Find("Hidden/Shader/LowResolution_RLPRO"));
		lowresTexture = RTHandles.Alloc(Vector2.one * downscale.value, TextureXR.slices, colorFormat: GraphicsFormat.B10G11R11_UFloatPack32, dimension: TextureDimension.Tex2DArray, enableRandomWrite: true, useDynamicScale: true, name: "lowresTexture");
		highresTexture = RTHandles.Alloc(Vector2.one, TextureXR.slices, colorFormat: GraphicsFormat.B10G11R11_UFloatPack32, dimension: TextureDimension.Tex2DArray, enableRandomWrite: true, useDynamicScale: true, name: "highresTexture");
	}

	public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
	{
		if (m_Material == null)
			return;
		if (m_PrevValue != downscale.value)
		{
			lowresTexture = RTHandles.Alloc(Vector2.one * downscale.value, TextureXR.slices, colorFormat: GraphicsFormat.B10G11R11_UFloatPack32, dimension: TextureDimension.Tex2DArray, enableRandomWrite: true, useDynamicScale: true, name: "lowresTexture");
			highresTexture = RTHandles.Alloc(Vector2.one, TextureXR.slices, colorFormat: GraphicsFormat.B10G11R11_UFloatPack32, dimension: TextureDimension.Tex2DArray, enableRandomWrite: true, useDynamicScale: true, name: "highresTexture");
			m_PrevValue = downscale.value;
		}
		m_Material.SetFloat("_Intensity", intensity.value);
		m_Material.SetTexture("_InputTexture", source);
		HDUtils.DrawFullScreen(cmd, m_Material, lowresTexture, shaderPassId: 0);
		m_Material.SetTexture("_InputTexture2", lowresTexture);
		m_Material.SetFloat("downsample", downscale.value);
		HDUtils.DrawFullScreen(cmd, m_Material, highresTexture, shaderPassId: 1);
		m_Material.SetTexture("_InputTexture3", highresTexture);
		m_Material.SetFloat("downsample", downscale.value);
		HDUtils.DrawFullScreen(cmd, m_Material, destination, shaderPassId: 2);
	}

	public override void Cleanup()
	{
		CoreUtils.Destroy(m_Material);
	}
}
