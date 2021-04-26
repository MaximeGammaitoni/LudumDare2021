using UnityEngine;
using UnityEditor;
using LimitlessDev.RetroLookPro;
using RetroLookPro.Enums;

namespace UnityEditor.Rendering.HighDefinition
{
	[VolumeComponentEditor(typeof(ColormapPalette_RLPRO))]
	internal sealed class ColormapPalette_RLPRO_HDRP_Editor : VolumeComponentEditor
	{
		SerializedDataParameter resolutionMode;
		SerializedDataParameter pixelSize;
		SerializedDataParameter resolution;
		SerializedDataParameter opacity;
		SerializedDataParameter dither;
		SerializedDataParameter presetsList;
		SerializedDataParameter presetsList1;
		SerializedDataParameter presetIndex;
		SerializedDataParameter blueNoise;
		SerializedDataParameter intencity;
		string[] palettePresets;

		public override void OnEnable()
		{
			base.OnEnable();

			var o = new PropertyFetcher<ColormapPalette_RLPRO>(serializedObject);
			intencity = Unpack(o.Find(x => x.intensity));
			pixelSize = Unpack(o.Find(x => x.pixelSize));
			opacity = Unpack(o.Find(x => x.opacity));
			dither = Unpack(o.Find(x => x.dither));
			blueNoise = Unpack(o.Find(x => x.bluenoise));
			presetsList = Unpack(o.Find(x => x.presetsList));
			presetIndex = Unpack(o.Find(x => x.presetIndex));

			string[] paths = AssetDatabase.FindAssets("RetroLookProColorPaletePresetsList");
			string assetpath = AssetDatabase.GUIDToAssetPath(paths[0]);
			effectPresets tempPreset = (effectPresets)AssetDatabase.LoadAssetAtPath(assetpath, typeof(effectPresets));

			palettePresets = new string[tempPreset.presetsList.Count];
			for (int i = 0; i < palettePresets.Length; i++)
			{
				palettePresets[i] = tempPreset.presetsList[i].preset.effectName;
			}
		}

		public override void OnInspectorGUI()
		{
			PropertyField(intencity);
			if (presetsList.value.objectReferenceValue == null)
			{
				string[] efListPaths = AssetDatabase.FindAssets("RetroLookProColorPaletePresetsList");
				string efListPath = AssetDatabase.GUIDToAssetPath(efListPaths[0]);
				presetsList.value.objectReferenceValue = (effectPresets)AssetDatabase.LoadAssetAtPath(efListPath, typeof(effectPresets));
				presetsList.value.serializedObject.ApplyModifiedProperties();

				EditorGUILayout.HelpBox("Please insert Retro Look Pro Color Palete Presets List.", MessageType.Info);
				PropertyField(presetsList);
			}

			if (blueNoise.value.objectReferenceValue == null)
			{
				PropertyField(blueNoise);
			}
			else
			{
				PropertyField(blueNoise);
			}
			presetIndex.value.intValue = EditorGUILayout.Popup("Color Preset", presetIndex.value.intValue, palettePresets);
			PropertyField(pixelSize);
			PropertyField(opacity);
			PropertyField(dither);
			presetIndex.overrideState.boolValue = true;
			presetsList.overrideState.boolValue = true;

		}
	}
}

