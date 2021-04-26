using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;
using RetroLookPro.Enums;
using UnityEngine.Experimental.Rendering;
using LimitlessDev.RetroLookPro;
[Serializable]
public sealed class resModeParameter : VolumeParameter<ResolutionMode> { };
[Serializable]
public sealed class Vector2IntParameter : VolumeParameter<Vector2Int> { };
[Serializable]
public sealed class preLParameter : VolumeParameter<effectPresets> { };
[Serializable, VolumeComponentMenu("Post-processing/Retro Look Pro/ColormapPalette_RLPRO")]
public sealed class ColormapPalette_RLPRO : CustomPostProcessVolumeComponent, IPostProcessComponent
{
	[Tooltip("Controls the intensity of the effect.")]
	public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
	public ClampedIntParameter pixelSize = new ClampedIntParameter(1, 1, 10);
	[Range(0f, 1f), Tooltip("Opacity.")]
	public ClampedFloatParameter opacity = new ClampedFloatParameter(1f, 0f, 1f);
	[Range(0f, 1f), Tooltip("Dithering effect.")]
	public ClampedFloatParameter dither = new ClampedFloatParameter(1f, 0f, 1f);
	public preLParameter presetsList = new preLParameter { };

	public IntParameter presetIndex = new IntParameter(0);
	[Tooltip("Dither texture.")]
	public TextureParameter bluenoise = new TextureParameter(null);
	Material m_Material;
	public int tempPresetIndex = 0;
	private bool m_Init;
	Texture2D colormapPalette;
	Texture3D colormapTexture;
	private Vector2 m_Res;
	private int m_TempPixelSize;
	RTHandle lowresTexture;
	public bool IsActive() => m_Material != null && intensity.value > 0f;

	public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

	public override void Setup()
	{
		if (Shader.Find("Hidden/Shader/ColormapPaletteEffect_RLPRO") != null)
			m_Material = new Material(Shader.Find("Hidden/Shader/ColormapPaletteEffect_RLPRO"));
		lowresTexture = RTHandles.Alloc(Vector2.one / pixelSize.value, TextureXR.slices, colorFormat: GraphicsFormat.B10G11R11_UFloatPack32, dimension: TextureDimension.Tex2DArray, enableRandomWrite: true, useDynamicScale: true, name: "lowresTexture");
		m_Init = true;
	}

	public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
	{
		if (m_Material == null)
			return;

		m_Material.SetFloat("_Intensity", intensity.value);
		m_Material.SetTexture("_InputTexture", source);
		ApplyMaterialVariables(m_Material, out m_Res);

		if (m_Init || intHasChanged(tempPresetIndex, presetIndex.value) || m_TempPixelSize != pixelSize.value)
		{
			tempPresetIndex = presetIndex.value;
			ApplyColormapToMaterial(m_Material);
			m_Init = false;
			m_TempPixelSize = pixelSize.value;
			lowresTexture = RTHandles.Alloc(Vector2.one / pixelSize.value, TextureXR.slices, colorFormat: GraphicsFormat.B10G11R11_UFloatPack32, dimension: TextureDimension.Tex2DArray, enableRandomWrite: true, useDynamicScale: true, name: "lowresTexture");
		}

		HDUtils.DrawFullScreen(cmd, m_Material, lowresTexture, shaderPassId: 0);
		m_Material.SetVector("Resolution", new Vector4(m_Res.x, m_Res.y, 0, 0));
		m_Material.SetTexture("_InputTexture3", lowresTexture);
		HDUtils.DrawFullScreen(cmd, m_Material, destination, shaderPassId: 2);

	}

	public void ApplyMaterialVariables(Material bl, out Vector2 res)
	{

		res.x = Screen.width / pixelSize.value;
		res.y = Screen.height / pixelSize.value;

		opacity.value = Mathf.Clamp01(opacity.value);
		dither.value = Mathf.Clamp01(dither.value);

		bl.SetFloat("_Dither", dither.value);
		bl.SetFloat("_Opacity", opacity.value);
	}
	public void ApplyColormapToMaterial(Material bl)
	{

		if (presetsList.value != null)
		{
			if (bluenoise.value != null)
			{
				bl.SetTexture("_BlueNoise", bluenoise.value);

			}
			ApplyPalette(bl);
			ApplyMap(bl);
		}
	}
	void ApplyPalette(Material bl)
	{
		colormapPalette = new Texture2D(256, 1, TextureFormat.RGB24, false);
		colormapPalette.filterMode = FilterMode.Point;
		colormapPalette.wrapMode = TextureWrapMode.Clamp;

		for (int i = 0; i < presetsList.value.presetsList[presetIndex.value].preset.numberOfColors; ++i)
		{
			colormapPalette.SetPixel(i, 0, presetsList.value.presetsList[presetIndex.value].preset.palette[i]);
		}

		colormapPalette.Apply();

		bl.SetTexture("_Palette", colormapPalette);
	}
	public void ApplyMap(Material bl)
	{
		int colorsteps = 64;
		colormapTexture = new Texture3D(colorsteps, colorsteps, colorsteps, TextureFormat.RGB24, false)
		{
			filterMode = FilterMode.Point,
			wrapMode = TextureWrapMode.Clamp
		};
		colormapTexture.SetPixels32(presetsList.value.presetsList[presetIndex.value].preset.pixels);
		colormapTexture.Apply();
		bl.SetTexture("_Colormap", colormapTexture);

	}
	public bool intHasChanged(int A, int B)
	{
		bool result = false;
		if (B != A)
		{
			A = B;
			result = true;
		}
		return result;
	}

	public override void Cleanup()
	{
		CoreUtils.Destroy(m_Material);
	}
}
