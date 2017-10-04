using UnityEngine;
using System.Collections;
using System;

namespace AFArcade {

public class Popup_HalfPack : IPopup_HalfPack
{
	UILabel labelBeforePrice;
	UILabel labelNowPrice;
	UILabel labelOfferEnd;

	Transform rayParticles;
	GameObject[] scaleSprites;
	
	UITweener subpopupTween;

	enum PackType {
		HOLIDAY,		// Shown during holidays
		STARTER_PACK,	// Shown every a few hours
		EPIC_PACK,		// Shown with PlayerStats API
	}
	PackType packType;

	protected override void Awake()
	{
		base.Awake();

		transform.Find("Sprite_Stamp").Find("Particles").GetComponent<ParticleSystem>().playbackSpeed = 0.7f;

		labelBeforePrice = transform.Find("Background").Find("StarsBack").Find("Label_Before").GetComponent<UILabel>();
		labelNowPrice = transform.Find("Background").Find("StarsBack").Find("Label_Price").GetComponent<UILabel>();
		labelOfferEnd = transform.Find("Background").Find("StarsBack").Find("Label_OfferEnd").GetComponent<UILabel>();
		rayParticles = transform.Find("RayParticles");

		scaleSprites = new GameObject[4];
		scaleSprites[0] = transform.Find("Background").Find("StarsBack").Find("Sprite_Chest").gameObject;
		scaleSprites[1] = transform.Find("Background").Find("StarsBack").Find("Sprite_Chars").gameObject;
		scaleSprites[2] = transform.Find("Background").Find("StarsBack").Find("Sprite_Noads").gameObject;
		scaleSprites[3] = transform.Find("Background").Find("StarsBack").Find("Sprite_Duplicate").gameObject;

		subpopupTween = transform.Find("SubPopup").GetComponent<UITweener>();

		transform.Find("Background").Find("StarsBack").Find("Label_Now").GetComponent<UILabel>().text = Language.get("HalfPack.Now");
		transform.Find("Button_Submit").Find("Label").GetComponent<UILabel>().text = Language.get("HalfPack.Purchase");

		transform.Find("SubPopup").Find("BackBox").Find("Label_Title").GetComponent<UILabel>().text = Language.get("HalfPack.Includes") + ":";

		bool gemPackEnabled = ArtikFlowArcade.instance.configuration.enableCoins && ArtikFlowArcade.instance.configuration.gemPackCount > 0;
		string strElements =
			(ArtikFlowArcade.instance.configuration.charactersEnabled ? ("- " + Language.get("IAP.UnlockAllCharacters") + "\n") : "") +
			"- " + Language.get("IAP.RemoveAds") + "\n" +
			(ArtikFlowArcade.instance.configuration.enableCoins ? ("- " + Language.get("IAP.Duplicate") + "\n") : "") +
			(gemPackEnabled ? ("- " + Language.get("IAP.Gems").Replace("%", "" + ArtikFlowArcade.instance.configuration.gemPackCount) + "\n") : "");
		transform.Find("SubPopup").Find("BackBox").Find("Label_Desc").GetComponent<UILabel>().text = strElements;

		UIGrid grid = transform.Find("SubPopup").Find("BackBox").Find("Grid").GetComponent<UIGrid>();
		grid.transform.Find("Sprite_Duplicate").gameObject.SetActive(ArtikFlowArcade.instance.configuration.enableCoins);
		grid.transform.Find("Sprite_Noads").gameObject.SetActive(true);
		grid.transform.Find("Sprite_Chars").gameObject.SetActive(ArtikFlowArcade.instance.configuration.charactersEnabled);
		grid.transform.Find("Sprite_GemPack").gameObject.SetActive(gemPackEnabled);
		grid.enabled = true;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			base.close();
	}

	protected override void onShow()
	{
		base.onShow();

		labelBeforePrice.text = Language.get("HalfPack.Before") + " [s]" + AFBase.Purchaser.instance.localizedPrice("pack") + "[/s]";
		labelNowPrice.text = AFBase.Purchaser.instance.localizedPrice("packhalf");
		iTween.Stop(rayParticles.gameObject);
		rayParticles.localScale = Vector3.one;
		iTween.ScaleFrom(rayParticles.gameObject, iTween.Hash("scale", Vector3.zero, "time", 3f, "delay", 0.3f));
		int i = 0;
		foreach(GameObject go in scaleSprites)
		{
			iTween.Stop(go);
			go.transform.localScale = Vector3.one;
			iTween.ScaleFrom(go, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", (i * 0.3f) + 1f));
			i++;
		}

		subpopupTween.enabled = false;
		subpopupTween.ResetToBeginning();

		if(Arcade_Purchaser.instance.isDuringHolidayPack())			// Holiday offer
		{
			packType = PackType.HOLIDAY;
			transform.Find("Label_Title").GetComponent<UILabel>().text = Language.get("HalfPack.Title");
			transform.Find("Label_Pack").GetComponent<UILabel>().text = Language.get("HalfPack.Pack");
			labelOfferEnd.gameObject.SetActive(true);
        }
		else if(Arcade_Purchaser.instance.isDuringStarterPack())	// Starter pack
		{
			packType = PackType.STARTER_PACK;
			transform.Find("Label_Title").GetComponent<UILabel>().text = Language.get("HalfPack.OneTime");
			transform.Find("Label_Pack").GetComponent<UILabel>().text = Language.get("HalfPack.StarterPack");
			labelOfferEnd.gameObject.SetActive(true);
		}
		else	// One-Time offer
		{
			packType = PackType.EPIC_PACK;
			transform.Find("Label_Title").GetComponent<UILabel>().text = Language.get("HalfPack.OneTime");
			transform.Find("Label_Pack").GetComponent<UILabel>().text = Language.get("HalfPack.Pack");
			labelOfferEnd.gameObject.SetActive(false);
		}

		AFBase.Purchaser.instance.eventPurchased.AddListener(onPurchase);
		StartCoroutine(timeUpdate());
	}

	protected override void onHide()
	{
		base.onHide();

		AFBase.Purchaser.instance.eventPurchased.RemoveListener(onPurchase);
	}

	IEnumerator timeUpdate()	// Gets stopped automagically when the gameObject is disabled.
	{
		while(true)
		{
			// Update price
			if(labelNowPrice.text == "...")
			{
				labelBeforePrice.text = Language.get("HalfPack.Before") + " [s]" + AFBase.Purchaser.instance.localizedPrice("pack") + "[/s]";
				labelNowPrice.text = AFBase.Purchaser.instance.localizedPrice("packhalf");
			}

			// Update time
			updateTimeLeft();
			yield return new WaitForSecondsRealtime(1f);
		}
	}

	void updateTimeLeft()
	{
		TimeSpan timeLeft;
		if(packType == PackType.HOLIDAY)
			timeLeft = Arcade_Purchaser.instance.timeLeftForHolidayPack();
		else if(packType == PackType.STARTER_PACK)
			timeLeft = Arcade_Purchaser.instance.TimeLeftForStarterPack();
		else
			timeLeft = TimeSpan.Zero;

		string str = "";
		if (timeLeft.TotalDays >= 1)
			str = Mathf.CeilToInt((float)timeLeft.TotalDays) + " " + Language.get("HalfPack.Days");
		else
			str = timeLeft.Hours + ":" + timeLeft.Minutes + ":" + timeLeft.Seconds;

		labelOfferEnd.text = Language.get("HalfPack.OfferEnds") + ": " + str;
	}

	void onPurchase(string productID)
	{
		if(PopupManager.instance.getCurrentPopup() != null)
			PopupManager.instance.getCurrentPopup().hide();
	}

	// --- Callbacks ---

	public void onPurchaseClick()
	{
		Audio.instance.playName("button");

		AFBase.Purchaser.instance.BuyProductID("packhalf");
	}

	public void onInfoClick()
	{
		subpopupTween.ResetToBeginning();
		subpopupTween.enabled = true;
	}

}

}