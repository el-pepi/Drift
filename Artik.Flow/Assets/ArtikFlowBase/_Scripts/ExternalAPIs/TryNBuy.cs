using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;
using Pixtel;

/// <summary>
/// TryNBuy system. Used only for FRENCH_PREMIUM builds, if the option is enabled in the ArtikFlowConfiguration.
/// </summary>

namespace AFBase {

public class TryNBuy : MonoBehaviour, PXInapp.PaymentCallback
{
	public static TryNBuy instance;

	[HideInInspector]
	public UnityEvent eventGamePurchased = new UnityEvent();

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		if (ArtikFlowBase.instance.configuration.storeTarget != ArtikFlowBaseConfiguration.StoreTarget.FRENCH_PREMIUM || !ArtikFlowBase.instance.configuration.FRENCH_PREMIUM_tryNBuy)
			return;

		PXInapp.create(this, ArtikFlowBase.instance.configuration.FRENCH_PREMIUM_tryNBuyId, PXInapp.UI_MODE_INTEGRATED, false);

		Invoke("initialize", 0.55f);
	}

	void initialize()
	{
		if (ArtikFlowBase.instance.configuration.storeTarget != ArtikFlowBaseConfiguration.StoreTarget.FRENCH_PREMIUM || !ArtikFlowBase.instance.configuration.FRENCH_PREMIUM_tryNBuy)
			return;

		PXInapp.setPaymentCallback(this);
	}

	void onApplicationFocus(bool focusState)
	{
		if (ArtikFlowBase.instance.configuration.storeTarget != ArtikFlowBaseConfiguration.StoreTarget.FRENCH_PREMIUM || !ArtikFlowBase.instance.configuration.FRENCH_PREMIUM_tryNBuy)
			return;

		if (focusState)
			PXInapp.resume();
		else
			PXInapp.pause();
	}

	void onApplicationQuit()
	{
		if (ArtikFlowBase.instance.configuration.storeTarget != ArtikFlowBaseConfiguration.StoreTarget.FRENCH_PREMIUM || !ArtikFlowBase.instance.configuration.FRENCH_PREMIUM_tryNBuy)
			return;

		PXInapp.destroy();
	}

	public void onPayment(PXInappProduct inappProduct, int result)
	{
		if(result < 0)
			Debug.Log("TryNBuy: Error proccessing purchase");
		else
		{
			if(inappProduct.id == 1)		// Gmae unlocked
				eventGamePurchased.Invoke();
		}

	}

	// --- Methods

	public bool needSMSButton()
	{
		if (ArtikFlowBase.instance.configuration.storeTarget != ArtikFlowBaseConfiguration.StoreTarget.FRENCH_PREMIUM || !ArtikFlowBase.instance.configuration.FRENCH_PREMIUM_tryNBuy)
			return false;

		return PXInapp.getPaymentAskforSmsCode() == PXInapp.RESULT_YES;
	}

	public void tryNBuy()
	{
		if (ArtikFlowBase.instance.configuration.storeTarget != ArtikFlowBaseConfiguration.StoreTarget.FRENCH_PREMIUM || !ArtikFlowBase.instance.configuration.FRENCH_PREMIUM_tryNBuy)
			return;

		/*
		// Testing:
		PXInappProduct p = new PXInappProduct();
		p.id = 1;
		onPayment(p, 1);
		*/

		PXInapp.startIntegratedUI(1, null);
	}

	public void tryNBuySMS()
	{
		if (ArtikFlowBase.instance.configuration.storeTarget != ArtikFlowBaseConfiguration.StoreTarget.FRENCH_PREMIUM || !ArtikFlowBase.instance.configuration.FRENCH_PREMIUM_tryNBuy)
			return;

		if (needSMSButton())
			PXInapp.startCodeUI();
	}

}

}