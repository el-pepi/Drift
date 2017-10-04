using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFArcade {

public abstract class IRouletteScreen : GameScreen 
{
	public static IRouletteScreen instance;

	public struct RuntimeRouletteSlice {
		public float probability;
		public RouletteItem item;
	}

	// --- Interface and methods to call:

	protected abstract void onShow(ArtikFlowArcade.State oldState);		// Should display this screen.
	protected abstract void onHide();									// Should hide this screen.
	protected abstract void spin();										// Should spin the roulette.
	public abstract void giveExtraFreeSpin();							// Should give an extra free spin, even if didn't reach the free spin time yet.

	// Returns a list of slices to show in the roulette, with their probabilities. This is a potentially 'heavy' operation.
	protected virtual List<RuntimeRouletteSlice> getRouletteItems()
	{
		string debugstr = "[INFO] Slices: ";
		List<RuntimeRouletteSlice> slices = new List<RuntimeRouletteSlice>();

		for(int i = 0; i < ArtikFlowArcade.instance.configuration.rouletteSliceCount; i ++)
		{
			RuntimeRouletteSlice newSlice = new RuntimeRouletteSlice();
			newSlice.probability = ArtikFlowArcade.instance.configuration.rouletteSlices[i].probability;
			newSlice.item = pickItemForSlice(i);
			slices.Add(newSlice);

			debugstr += "{" + newSlice.item.name + ", " + newSlice.probability + "}, ";
		}

#if UNITY_EDITOR
		Debug.Log(debugstr);
#endif

		slices.Shuffle();
		return slices;
	}

	// Returns a winning slice index depending on probabilities passed by parameter
	protected virtual int pickWinningSlice(List<RuntimeRouletteSlice> slices)
	{
		float total = 0f;
		foreach(RuntimeRouletteSlice s in slices)
			total += s.probability;
		
		float pick = Random.Range(0f, total);
		int retIndex = -1;
		int i = 0;
		foreach(RuntimeRouletteSlice s in slices)
		{
			pick -= s.probability;
			if(pick <= 0)
			{
				retIndex = i;
				break;
			}

			i ++;
		}

		if(retIndex == -1)
			retIndex = slices.Count - 1;

		return retIndex;
	}

	// --- Partial private implementation (unity callbacks can be extended)

	protected override void Awake()
	{
		base.Awake();

		instance = this;
	}

	protected virtual void Start()
	{
		ArtikFlowArcade.instance.eventStateChange.AddListener(onArtikFlowStateChange);
	}

	private void onArtikFlowStateChange(ArtikFlowArcade.State oldstate, ArtikFlowArcade.State newstate)
	{
		if (newstate != ArtikFlowArcade.State.ROULETTE_SCREEN)
			onHide();
		else if(newstate == ArtikFlowArcade.State.ROULETTE_SCREEN)
			onShow(oldstate);
	}

	RouletteItem pickItemForSlice(int sliceIdx)
	{
		float total = 0f;
		foreach(float prob in ArtikFlowArcade.instance.configuration.rouletteSlices[sliceIdx].itemProbabilities)
			total += prob;
		
		float pick = Random.Range(0f, total);
		for(int i = 0; i < ArtikFlowArcade.instance.configuration.rouletteSlices[sliceIdx].itemProbabilities.Length; i ++)
		{
			pick -= ArtikFlowArcade.instance.configuration.rouletteSlices[sliceIdx].itemProbabilities[i];
			if(pick <= 0)
				return ArtikFlowArcade.instance.configuration.rouletteSlices[sliceIdx].possibleItems[i];
		}

		return ArtikFlowArcade.instance.configuration.rouletteSlices[sliceIdx].possibleItems[
				ArtikFlowArcade.instance.configuration.rouletteSlices[sliceIdx].possibleItems.Length - 1
		];
	}

}

public static class IListExtensions {
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts) {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i) {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}

}
