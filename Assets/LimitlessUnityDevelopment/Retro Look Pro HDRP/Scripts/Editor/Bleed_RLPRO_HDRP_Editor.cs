using UnityEngine;
using UnityEditor;
//using UnityEngine.Rendering.PostProcessing;

namespace UnityEditor.Rendering.HighDefinition
{
	[VolumeComponentEditor(typeof(Bleed_RLPRO_HDRP))]
	internal sealed class Bleed_RLPRO_HDRP_Editor : VolumeComponentEditor
	{

		SerializedDataParameter m_BleedModeEnum;
		SerializedDataParameter m_BleedAmount;
		SerializedDataParameter m_BleedLength;
		SerializedDataParameter m_BleedDebug;
		SerializedDataParameter m_EditCurves;
		SerializedDataParameter m_SyncYQ;
		SerializedDataParameter m_SplineCurveY;
		SerializedDataParameter m_SplineCurveI;
		SerializedDataParameter m_SplineCurveQ;

		public override void OnEnable()
		{
			base.OnEnable();

			var o = new PropertyFetcher<Bleed_RLPRO_HDRP>(serializedObject);

			m_BleedModeEnum = Unpack(o.Find(x => x.bleedMode));
			m_BleedAmount = Unpack(o.Find(x => x.bleedAmount));
			m_BleedLength = Unpack(o.Find(x => x.bleedLength));
			m_BleedDebug = Unpack(o.Find(x => x.bleedDebug));
			m_SplineCurveY = Unpack(o.Find(x => x.curveY));
			m_SplineCurveI = Unpack(o.Find(x => x.curveI));
			m_SplineCurveQ = Unpack(o.Find(x => x.curveQ));
			m_EditCurves = Unpack(o.Find(x => x.editCurves));

			m_SyncYQ = Unpack(o.Find(x => x.syncYQ));
		}

		public override void OnInspectorGUI()
		{

			PropertyField(m_BleedModeEnum);
			PropertyField(m_BleedAmount);
			if (m_BleedModeEnum.value.intValue == (int)BleedMode.customBleeding)
			{
				PropertyField(m_SyncYQ);
				PropertyField(m_EditCurves);
				PropertyField(m_SplineCurveY);
				if (!m_SyncYQ.value.boolValue)
					PropertyField(m_SplineCurveI);
				PropertyField(m_SplineCurveQ);

				PropertyField(m_BleedLength);
			}
			PropertyField(m_BleedDebug);
		}
	}
}

