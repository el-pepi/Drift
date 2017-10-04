using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFArcade {

public class Popup_DriftHalfPack : IPopup_HalfPack 
{

	public LocalizationLabel[] listLocalization;
	
	public UILabel label_StarterPackPrice;
	
	public Character character_StarterPack;

	public BoxCollider starterPack_Button;

	void Awake()
	{
		foreach (var item in listLocalization)
		{
			item.SetLabel ();	
		}
	}
	
	void Start()
	{
		AFBase.Purchaser.instance.eventPurchased.AddListener(onPurchased);
		
		
	}
	
	void onPurchased(string productId)
	{
		// Exclusive IAPs for Drift, process them here
		if (productId == "starterPack")
		{
			if (productId == "starterPack")
			{
				DriftShopScreen.instance.unlock (character_StarterPack);
				GameManager.instance.reset (character_StarterPack);
				ArtikFlowArcade.instance.setCharacter(character_StarterPack);

				//SaveGameSystem.instance.setDuplicate(true);
				SaveGameSystem.instance.setNoAds(true);
				AFBase.Ads.instance.hideBanner();
				base.hide ();
			}
		}
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

	protected override void onShow()
	{
		base.onShow();
		label_StarterPackPrice.text = AFBase.Purchaser.instance.localizedPrice("starterPack");
		toggleButton(starterPack_Button,!CharacterManager.instance.isOwned(character_StarterPack));
	}

	public void onStarterPack()
	{
		AFBase.Purchaser.instance.BuyProductID("starterPack");
	}
}

}