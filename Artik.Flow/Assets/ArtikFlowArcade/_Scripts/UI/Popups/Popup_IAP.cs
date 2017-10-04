using UnityEngine;
using System.Collections;
using System;

namespace AFArcade {

public class Popup_IAP : IPopup_IAP
{
	UILabel packPrice;
	UILabel removeAdsPrice;
	UILabel gemsPrice;
    UILabel unlockCharactersPrice;
	UILabel duplicatePrice;
	UISprite spriteStamp;

	bool gemPackEnabled;

	protected override void Awake()
	{
		base.Awake();

		gemPackEnabled = ArtikFlowArcade.instance.configuration.enableCoins && ArtikFlowArcade.instance.configuration.gemPackCount > 0;

		// Localization
		transform.Find("Row_SuperPack").Find("Button_SuperPack").Find("Label_Title").GetComponent<UILabel>().text = Language.get("IAP.SuperPackPromotion");
		transform.Find("Row_SuperPack").Find("Button_SuperPack").Find("Label_Details").GetComponent<UILabel>().text = 
			"- " + Language.get("IAP.PackLine1") +
			(gemPackEnabled ? ("\n- " + Language.get("Reward.Coins").Replace("%", "" + ArtikFlowArcade.instance.configuration.gemPackCount)) : "") +
			(ArtikFlowArcade.instance.configuration.charactersEnabled ? ("\n- " + Language.get("IAP.PackLine2")) : "") +
			(ArtikFlowArcade.instance.configuration.enableCoins ? ("\n- " + Language.get("IAP.PackLine3")) : "");

		spriteStamp = transform.Find("Row_SuperPack").Find("Button_SuperPack").Find("Sprite_HalfStamp").GetComponent<UISprite>();
		packPrice = transform.Find("Row_SuperPack").Find("Button_SuperPack").Find("Label_Price").GetComponent<UILabel>();

		transform.Find("Row_IAPs").Find("Button_RemoveAds").Find("Label_Details").GetComponent<UILabel>().text = Language.get("IAP.RemoveAds");
		removeAdsPrice = transform.Find("Row_IAPs").Find("Button_RemoveAds").Find("Label_Price").GetComponent<UILabel>();

		transform.Find("Row_IAPs").Find("Button_Gems").Find("Label_Details").GetComponent<UILabel>().text = Language.get("IAP.Gems").Replace("%", "" + ArtikFlowArcade.instance.configuration.gemPackCount);
		gemsPrice = transform.Find("Row_IAPs").Find("Button_Gems").Find("Label_Price").GetComponent<UILabel>();

		transform.Find("Row_IAPs").Find("Button_Characters").Find("Label_Details").GetComponent<UILabel>().text = Language.get("IAP.UnlockAllCharacters");
		unlockCharactersPrice = transform.Find("Row_IAPs").Find("Button_Characters").Find("Label_Price").GetComponent<UILabel>();

		transform.Find("Row_IAPs").Find("Button_Duplicate").Find("Label_Details").GetComponent<UILabel>().text = Language.get("IAP.Duplicate");
		duplicatePrice = transform.Find("Row_IAPs").Find("Button_Duplicate").Find("Label_Price").GetComponent<UILabel>();

		transform.Find("Row_Restore").Find("Button_Restore").Find("Label_Details").GetComponent<UILabel>().text = Language.get("IAP.Restore");

		transform.Find("Row_Notice").Find("Label_Notice").GetComponent<UILabel>().text = Language.get("IAP.Notice");

		// Enabling
		transform.Find("Row_IAPs").Find("Button_Duplicate").gameObject.SetActive(ArtikFlowArcade.instance.configuration.enableCoins);
		transform.Find("Row_IAPs").Find("Button_Gems").gameObject.SetActive(gemPackEnabled);
		transform.Find("Row_IAPs").Find("Button_Characters").gameObject.SetActive(ArtikFlowArcade.instance.configuration.charactersEnabled);
#if UNITY_IOS
		transform.Find("Row_Restore").gameObject.SetActive(true);
#else
		transform.Find("Row_Restore").gameObject.SetActive(false);
#endif
		if(!ArtikFlowArcade.instance.configuration.enableCoins && !ArtikFlowArcade.instance.configuration.charactersEnabled)    // Hide pack
			transform.Find("Row_SuperPack").gameObject.SetActive(false);

		packPrice.text = "...";
		gemsPrice.text = "...";
		removeAdsPrice.text = "...";
		unlockCharactersPrice.text = "...";
		duplicatePrice.text = "...";
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

		removeAdsPrice.text = AFBase.Purchaser.instance.localizedPrice("noads");
		
		if(ArtikFlowArcade.instance.configuration.charactersEnabled)
			unlockCharactersPrice.text = AFBase.Purchaser.instance.localizedPrice("unlockall");
		if (gemPackEnabled)
			gemsPrice.text = AFBase.Purchaser.instance.localizedPrice("gems");
		if (ArtikFlowArcade.instance.configuration.enableCoins)
			duplicatePrice.text = AFBase.Purchaser.instance.localizedPrice("duplicate"); 
		if (ArtikFlowArcade.instance.configuration.enableCoins || ArtikFlowArcade.instance.configuration.charactersEnabled)
		{
			if(Arcade_Purchaser.instance.isDuringHolidayPack())
				packPrice.text = AFBase.Purchaser.instance.localizedPrice("packhalf");
			else
				packPrice.text = AFBase.Purchaser.instance.localizedPrice("pack");
		}
	
		spriteStamp.gameObject.SetActive(Arcade_Purchaser.instance.isDuringHolidayPack());

		toggleButton(removeAdsPrice.transform.parent.GetComponent<UIButton>(), !SaveGameSystem.instance.hasNoAds());
		toggleButton(gemsPrice.transform.parent.GetComponent<UIButton>(), true);
		toggleButton(unlockCharactersPrice.transform.parent.GetComponent<UIButton>(), !CharacterManager.instance.hasAllCharacters());
		toggleButton(duplicatePrice.transform.parent.GetComponent<UIButton>(), !SaveGameSystem.instance.hasDuplicate());
		toggleButton(packPrice.transform.parent.GetComponent<UIButton>(), (!SaveGameSystem.instance.hasDuplicate() || !CharacterManager.instance.hasAllCharacters() || !SaveGameSystem.instance.hasNoAds()));

		if(spriteStamp.gameObject.activeInHierarchy)
		{
			iTween.ScaleFrom(spriteStamp.gameObject, iTween.Hash("scale", new Vector3(10f, 10f, 10f), "time", 0.3f, "easetype", iTween.EaseType.linear));
			spriteStamp.color = new Color32(255, 255, 255, 0);
			TweenAlpha.Begin(spriteStamp.gameObject, 0.5f, 1f);
		}
	}

	void toggleButton(UIButton button, bool toggle)
	{
		button.isEnabled = toggle;

		button.transform.Find("Label_Details").GetComponent<UILabel>().color = toggle ? new Color32(94, 44, 252, 255) : new Color32(52, 23, 128, 255);

		if (button.transform.Find("Sprite_IAP") != null)
			button.transform.Find("Sprite_IAP").GetComponent<UISprite>().color = toggle ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 50);

		if(button.transform.Find("Label_Title") != null)
			button.transform.Find("Label_Title").GetComponent<UILabel>().color = toggle ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 50);

		if (button.transform.Find("Label_Percentage") != null)
			button.transform.Find("Label_Percentage").GetComponent<UILabel>().color = toggle ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 50);
	}

	// --- Callbacks ---

	void onPurchased(string productId)
	{
		// The IAP is processed and saved in Purchaser.cs

		if (productId == "pack" || productId == "noads" || productId == "gems" || productId == "unlockall" || productId == "duplicate")
			base.hide();
	}

	public void onSuperPack()
	{
		if(Arcade_Purchaser.instance.isDuringHolidayPack())
			AFBase.Purchaser.instance.BuyProductID("packhalf");
		else
			AFBase.Purchaser.instance.BuyProductID("pack");
	}

	public void onRemoveAds()
	{
		AFBase.Purchaser.instance.BuyProductID("noads");
	}

	public void onGems()
	{
		AFBase.Purchaser.instance.BuyProductID("gems");
	}

	public void onUnlockCharacters()
	{
		AFBase.Purchaser.instance.BuyProductID("unlockall");
	}

	public void onDuplicate()
	{
		AFBase.Purchaser.instance.BuyProductID("duplicate");
	}

	public void onRestorePurchases()
	{
		AFBase.Purchaser.instance.RestorePurchases();
	}
	
}

}