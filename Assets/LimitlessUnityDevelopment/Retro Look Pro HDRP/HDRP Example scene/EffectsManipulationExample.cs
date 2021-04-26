using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine;

public class EffectsManipulationExample : MonoBehaviour
{
	// Your Post-processing volume;
	public Volume volume;

	private Bleed_RLPRO_HDRP bleed;

	private void Start()
	{
		Bleed_RLPRO_HDRP tempBleed;
		if (volume == null)
			return;
		if (volume.profile.TryGet<Bleed_RLPRO_HDRP>(out tempBleed))
		{
			bleed = tempBleed;
		}

	}
	private void FixedUpdate()
	{
		if (volume == null)
			return;
		//
		bleed.bleedAmount.value = UnityEngine.Random.Range(0.5f, 3);
	}
}