using UnityEngine;
using System.Collections;
using System;

namespace AFArcade 
{
	



public class Popup_DriftIAP : IPopup_IAP
{
	public static bool showPackOnly = false;		// Set to true before showing to show the pack only

	public UILabel superpackPrice;
	public UILabel doubleGemPrice;
	public UILabel gemPackPrice;
	public UILabel removeAdsPrice;
	public UILabel labelOfferEnds;

	public UILabel superPackOnGemsLabel;
	public UILabel superPackBelowGemsLabel;
	public UILabel packOnGemsLabel;
	public UILabel packGemsLabel;


	public LocalizationLabel[] listLocalization;
	
	public Character character_SuperPack;

	bool packActive;
	
	public BoxCollider superPack_Button;
	public BoxCollider noAds_Button;
	public BoxCollider doubleGem_button;

	protected override void Awake()
	{
		base.Awake();

		superPackBelowGemsLabel.text = Language.get("Drift.IAP.StarterPackGems").Replace("%",ArtikFlowArcade.instance.configuration.gemSuperPackCount.ToString());
		superPackOnGemsLabel.text = ArtikFlowArcade.instance.configuration.gemSuperPackCount.ToString ();
		packOnGemsLabel.text = ArtikFlowArcade.instance.configuration.gemPackCount.ToString ();
		packGemsLabel.text = Language.get("IAP.Gems").Replace("%",ArtikFlowArcade.instance.configuration.gemPackCount.ToString());
		foreach (var item in listLocalization)
		{
				item.SetLabel ();	
		}

		// Enabling
#if UNITY_IOS
		transform.Find("Row_Restore").gameObject.SetActive(true);
#else
		transform.Find("Row_Restore").gameObject.SetActive(false);
#endif
		
		superpackPrice.text = "...";
		doubleGemPrice.text = "...";
		gemPackPrice.text = "...";
		removeAdsPrice.text = "...";
	}

	void Start()
	{
		AFBase.Purchaser.instance.eventPurchased.AddListener(onPurchased);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			base.close();
	}

	protected override void onShow()
	{
		base.onShow();
		
		superpackPrice.text = AFBase.Purchaser.instance.localizedPrice("superPack");
		doubleGemPrice.text = AFBase.Purchaser.instance.localizedPrice("duplicate");
		gemPackPrice.text = AFBase.Purchaser.instance.localizedPrice("gemPack");
		removeAdsPrice.text = AFBase.Purchaser.instance.localizedPrice("noads");

		toggleButton(noAds_Button, !SaveGameSystem.instance.hasNoAds());
		toggleButton(superPack_Button,!CharacterManager.instance.isOwned(character_SuperPack));
		toggleButton(doubleGem_button, !SaveGameSystem.instance.hasDuplicate());

		//IGameplayScreen.instance.toggleCoins(false);

		StartCoroutine(timeUpdate());
	}
	

	void toggleButton(BoxCollider button, bool enable){

		button.enabled = enable;

		GameObject buttonSprite = button.transform.Find("Anim").Find ("Button_Sprite").gameObject;

		if( buttonSprite.transform.Find("label_Price") != null ){
			buttonSprite.transform.Find("label_Price").gameObject.SetActive(enable);
		}

		if( buttonSprite.transform.Find("label_Purchased") != null ){
			buttonSprite.transform.Find("label_Purchased").gameObject.SetActive(!enable);
		}

	}


	protected override void onHide()
	{
		base.onHide();

		IGameplayScreen.instance.toggleCoins(true);
	}


	IEnumerator timeUpdate()	// Gets stopped automagically when the gameObject is disabled.
	{
		while(true)
		{
			// Update price
			if(superpackPrice.text == "...")
			{
				superpackPrice.text = AFBase.Purchaser.instance.localizedPrice("superPack");
				doubleGemPrice.text = AFBase.Purchaser.instance.localizedPrice("duplicate");
				gemPackPrice.text = AFBase.Purchaser.instance.localizedPrice("gemPack");
				removeAdsPrice.text = AFBase.Purchaser.instance.localizedPrice("noads");
			}

			// Update time
			updateTimeLeft();
			yield return new WaitForSecondsRealtime(1f);
		}
	}

	void updateTimeLeft()
	{
		TimeSpan timeLeft = Arcade_Purchaser.instance.TimeLeftForStarterPack();
		
		string str = "";
		if (timeLeft.TotalDays >= 1)
			str = Mathf.CeilToInt((float)timeLeft.TotalDays) + " " + Language.get("HalfPack.Days");
		else if(timeLeft.TotalHours >= 1)
			str = Mathf.CeilToInt((float)timeLeft.TotalHours) + " " + Language.get("Drift.IAP.Hours");
		else
			str = timeLeft.Minutes + ":" + timeLeft.Seconds;

		labelOfferEnds.text = Language.get("Drift.IAP.OfferEnds") + " " + str;
	}
	
	void GiveReward(int reward)
	{
			PopupManager.instance.showPopup<Popup_Reward_Custom> ();

			Popup_Reward_Custom popupReward = (Popup_Reward_Custom)PopupManager.instance.getCurrentPopup ();

			popupReward.SetPopUp(reward);
	}
	// --- Callbacks ---

	void onPurchased(string productId)
	{
		// Exclusive IAPs for Drift, process them here

		if (productId == "superPack" || productId == "gemPack" || productId == "noads" || productId == "duplicate")
		{
			if(productId == "superPack")
			{
				DriftShopScreen.instance.unlock (character_SuperPack);
				GameManager.instance.reset (character_SuperPack);
					ArtikFlowArcade.instance.setCharacter(character_SuperPack);
				SaveGameSystem.instance.setNoAds(true);
				// to-do!
				base.hide();
				
				GiveReward(ArtikFlowArcade.instance.configuration.gemSuperPackCount);
			}
			else if(productId == "gemPack")
			{
				base.hide();
				GiveReward(ArtikFlowArcade.instance.configuration.gemPackCount);
			}
			else if(productId == "noads")
			{
				SaveGameSystem.instance.setNoAds(true);
				base.hide();
			}
			else if(productId == "duplicate")
			{
				SaveGameSystem.instance.setDuplicate(true);
				base.hide();
			}

			//SaveGameSystem.instance.setNoAds(true);
			//base.hide();
		}
	}

	public void onSuperPack()
	{
		AFBase.Purchaser.instance.BuyProductID("superPack");
	}

	public void onDoubleGems()
	{
		AFBase.Purchaser.instance.BuyProductID("duplicate");
	}

	public void onGemPack()
	{
		AFBase.Purchaser.instance.BuyProductID("gemPack");
	}

	public void onNoAds()
	{
		AFBase.Purchaser.instance.BuyProductID("noads");
	}

	public void onRestorePurchases()
	{
		AFBase.Purchaser.instance.RestorePurchases();
	}
	
}

}