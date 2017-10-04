using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTierManager : MonoBehaviour
{
	public UISprite tierContainer;

	public Color tier1Color;
	public Color tier2Color;
	public Color tier3Color;


	public void SetShopTier(string tier)
	{
		if (tier == "Drift.Tier1") {
			SetGlows (tier1Color);
		}
		if (tier == "Drift.Tier2") {
			SetGlows (tier2Color);
		}
		if (tier == "Drift.Tier3") {
			SetGlows (tier3Color);
		}
	}

	void SetGlows(Color newColor)
	{
		tierContainer.color = newColor;
	}

}
