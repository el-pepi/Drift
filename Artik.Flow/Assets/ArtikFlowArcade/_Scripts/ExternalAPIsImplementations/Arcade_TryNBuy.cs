using System.Collections;
using System.Collections.Generic;
using Pixtel;
using UnityEngine;
using AFBase;

namespace AFArcade {

public class Arcade_TryNBuy : MonoBehaviour 
{
	public static Arcade_TryNBuy instance;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		TryNBuy.instance.eventGamePurchased.AddListener(onGamePurchased);
	}

	void onGamePurchased()
	{
		Debug.Log("TryNBuy: Game unlocked!");
		SaveGameSystem.instance.setTryNBuyUnlocked(true);
		SaveGameSystem.instance.setCoins(SaveGameSystem.instance.getCoins() + 2500);
	}

	// ---

	public bool canPlay()
	{
		if (ArtikFlowBase.instance.configuration.storeTarget != ArtikFlowBaseConfiguration.StoreTarget.FRENCH_PREMIUM || !ArtikFlowBase.instance.configuration.FRENCH_PREMIUM_tryNBuy)
			return true;

		PXInappProduct product = PXInapp.getInappProduct(1);
		if (product == null)
			return true;

		if (SaveGameSystem.instance.getTryNBuyUnlocked())
			return true;
		else
			return SaveGameSystem.instance.getGamesPlayed() < product.amount;
	}

}

}