using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Experimental.Rendering;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Retro Look Pro/Glitch2_RLPRO")]
public sealed class Glitch2_RLPRO : CustomPostProcessVolumeComponent, IPostProcessComponent
{
	[Tooltip("Controls the intensity of the effect.")]
	public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
	[Tooltip("Effect Speed.")]
	public ClampedFloatParameter speed = new ClampedFloatParameter(1f, 0f, 1f);
	[Tooltip("Effect Amount.")]
	public ClampedFloatParameter amount = new ClampedFloatParameter(1f, 0f, 1f);
	[Tooltip("Color Intensity.")]
	public ClampedFloatParameter ColorIntencity = new ClampedFloatParameter(1f, 0f, 1f);
	[Tooltip("Resolution Multiplier.")]
	public ClampedFloatParameter resolutionMultiplier = new ClampedFloatParameter(1f, 1f, 2f);
	[Tooltip("Stretch Multiplier.")]
	public ClampedFloatParameter stretchMultiplier = new ClampedFloatParameter(0.88f, 0f, 1f);
	//
	Material m_Material;
	private float T;
	RTHandle _trashFrame1;
	RTHandle _trashFrame2;
	Texture2D _noiseTexture;

	public bool IsActive() => m_Material != null && intensity.value > 0f;

	public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

	public override void Setup()
	{
		if (Shader.Find("Hidden/Shader/Glitch2Effect_RLPRO") != null)
			m_Material = new Material(Shader.Find("Hidden/Shader/Glitch2Effect_RLPRO"));
		_trashFrame1 = RTHandles.Alloc(Vector2.one, TextureXR.slices, colorFormat: GraphicsFormat.B10G11R11_UFloatPack32, dimension: TextureDimension.Tex2DArray, enableRandomWrite: true, useDynamicScale: true, name: "_trashFrame1");
		_trashFrame2 = RTHandles.Alloc(Vector2.one, TextureXR.slices, colorFormat: GraphicsFormat.B10G11R11_UFloatPack32, dimension: TextureDimension.Tex2DArray, enableRandomWrite: true, useDynamicScale: true, name: "_trashFrame2");
		SetUpResources(resolutionMultiplier.value);
	}

	public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
	{
		if (m_Material == null)
			return;
		m_Material.SetTexture("_InputTexture", source);
		m_Material.SetTexture("_InputTexture1", source);
		m_Material.SetFloat("_Intensity", intensity.value);
		if (UnityEngine.Random.value > Mathf.Lerp(0.9f, 0.5f, speed.value))
		{
			SetUpResources(resolutionMultiplier.value);
			UpdateNoiseTexture(resolutionMultiplier.value);
		}
		// Update trash frames.
		int fcount = Time.frameCount;

		if (fcount % 13 == 0) HDUtils.DrawFullScreen(cmd, m_Material, _trashFrame1, shaderPassId: 1);
		if (fcount % 73 == 0) HDUtils.DrawFullScreen(cmd, m_Material, _trashFrame2, shaderPassId: 1);

		m_Material.SetFloat("Intensity", ColorIntencity.value);
		m_Material.SetFloat("_ColorIntensity", amount.value);
		if (_noiseTexture == null)
		{
			UpdateNoiseTexture(resolutionMultiplier.value);
		}

		m_Material.SetTexture("_NoiseTex", _noiseTexture);
		m_Material.SetTexture("_TrashTex", UnityEngine.Random.value > 0.5f ? _trashFrame1 : _trashFrame2);
		HDUtils.DrawFullScreen(cmd, m_Material, destination, shaderPassId: 0);

	}
	void SetUpResources(float g_2Res)
	{
		Vector2Int texVec = new Vector2Int((int)(g_2Res * 64), (int)(g_2Res * 32));
		_noiseTexture = new Texture2D(texVec.x, texVec.y, TextureFormat.ARGB32, false)
		{

			hideFlags = HideFlags.DontSave,
			wrapMode = TextureWrapMode.Clamp,
			filterMode = FilterMode.Point
		};
		UpdateNoiseTexture(g_2Res);
	}
	void UpdateNoiseTexture(float g_2Res)
	{
		Color color = RandomColor();
		if (_noiseTexture == null)
		{
			Vector2Int texVec = new Vector2Int((int)(g_2Res * 64), (int)(g_2Res * 32));
			_noiseTexture = new Texture2D(texVec.x, texVec.y, TextureFormat.ARGB32, false);
		}
		for (var y = 0; y < _noiseTexture.height; y++)
		{
			for (var x = 0; x < _noiseTexture.width; x++)
			{
				if (UnityEngine.Random.value > stretchMultiplier.value) color = RandomColor();
				_noiseTexture.SetPixel(x, y, color);
			}
		}

		_noiseTexture.Apply();
	}
	static Color RandomColor()
	{
		return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
	}
	public override void Cleanup()
	{
		CoreUtils.Destroy(m_Material);
	}
}
