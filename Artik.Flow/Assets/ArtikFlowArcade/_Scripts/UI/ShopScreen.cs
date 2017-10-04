using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace AFArcade {

public class ShopScreen : IShopScreen
{
	private string Facebook_buy_Url;    // ArtikFlowConfiguration
	private string Twitter_buy_Url;		// ArtikFlowConfiguration
	private string Instagram_buy_Url;   // ArtikFlowConfiguration

	Character selectedChar;

	UIScrollView scrollView;
	UIGrid charGrid;
	UILabel counterLabel;
	UILabel coinsLabel;
	UIButton duplicateButton;

	protected override void Awake()
	{
		base.Awake();
		
		scrollView = transform.Find("Scroller").GetComponent<UIScrollView>();
		charGrid = scrollView.transform.Find("Grid").GetComponent<UIGrid>();
		counterLabel = scrollView.transform.Find("Label_Count").GetComponent<UILabel>();
		coinsLabel = transform.Find("Top").Find("Coins").Find("Label").GetComponent<UILabel>();
		duplicateButton = scrollView.transform.Find("Button_Duplicate").GetComponent<UIButton>();
		duplicateButton.transform.Find("Label_Details").GetComponent<UILabel>().text = Language.get("IAP.Duplicate");

		transform.Find("Top").Find("Label_Shop").GetComponent<UILabel>().text = Language.get("Shop.Shop");		
	}

	protected override void Start()
	{		
		base.Start();

		Facebook_buy_Url = ArtikFlowArcade.instance.configuration.Facebook_buy_Url;
		Twitter_buy_Url = ArtikFlowArcade.instance.configuration.Twitter_buy_Url;
		Instagram_buy_Url = ArtikFlowArcade.instance.configuration.Instagram_buy_Url;

		if (ArtikFlowArcade.instance.configuration.storeTarget == ArtikFlowArcadeConfiguration.StoreTarget.FRENCH_PREMIUM)
			duplicateButton.gameObject.SetActive(false);

		populateCharacters();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && PopupManager.instance.getCurrentPopup() == null)
			onBackClick();
	}

	void OnDisable()
	{
		SpringPanel springPanel = transform.Find("Scroller").GetComponent<SpringPanel>();
		if (springPanel != null)
			Destroy(springPanel);
	}

	// --- Callbacks ---

	protected override void onHide()
	{
		gameObject.SetActive(false);
	}

	protected override void onShow(ArtikFlowArcade.State oldState){
		selectedChar = ArtikFlowArcade.instance.getCurrentCharacter();
		updateCounters();
		GetComponent<Animator>().enabled = true;
		gameObject.SetActive(true);

		// Update unlocked chars
		foreach(Character c in CharacterManager.instance.characters){
			if( CharacterManager.instance.isUnlocked(c) ){
				if( selectedChar == c ){
					getShopItem(c).setState(ShopItem.State.SELECTED);
				} else {
					getShopItem(c).setState(ShopItem.State.UNLOCKED);
				}
			} else {
				getShopItem(c).setState(ShopItem.State.LOCKED);
			}
		}

		// Select selected char
		GameObject selected = getShopItem(ArtikFlowArcade.instance.getCurrentCharacter()).gameObject;
		onCharSelected(selected);

		// Update duplicate price
		if(ArtikFlowArcade.instance.configuration.storeTarget != ArtikFlowArcadeConfiguration.StoreTarget.FRENCH_PREMIUM)
		{
			duplicateButton.transform.Find("Label_Price").GetComponent<UILabel>().text = AFBase.Purchaser.instance.localizedPrice("duplicate");
			duplicateButton.gameObject.SetActive(!SaveGameSystem.instance.hasDuplicate());
		}
	}

	public void onCharSelected(GameObject uiChar){
		int id;
		bool success = int.TryParse(uiChar.name.Split('.')[0], out id);
		if( success ){
			Character c = CharacterManager.instance.getCharacter(id);
			if( CharacterManager.instance.isOwned(c) ){
				select(c);
			} else {
				buy(c);
			}
		}
	}

	public void onBackClick()
	{
		base.selectCharacter(selectedChar);

		GetComponent<Animator>().enabled = true;
		GetComponent<Animator>().Play("shopScreenOut", -1, 0f);
	}

	public void onDuplicateClick()
	{
		PopupManager.instance.showPopup<IPopup_IAP>();
		base.selectCharacter(selectedChar);

		GetComponent<Animator>().enabled = true;
		GetComponent<Animator>().Play("shopScreenOut", -1, 0f);
	}

	public void onAnimInFinish()
	{
		GetComponent<Animator>().enabled = false;
	}

	public void onAnimOutFinish()
	{
		base.goBack();
	}

	// --- Methods ---

	void populateCharacters()
	{
		// Populate characters
		GameObject template = charGrid.transform.GetChild(0).gameObject;

		foreach (Character c in CharacterManager.instance.characters)
		{
			GameObject item = NGUITools.AddChild(template.transform.parent.gameObject, template);
			item.name = "" + String.Format("{0:D3}", c.id) + "." + c.internalName;

			if (c.price >= ArtikFlowArcade.instance.configuration.characterTier3Start)
				item.GetComponent<ShopItem>().setTier(3);				
			else if (c.price >= ArtikFlowArcade.instance.configuration.characterTier2Start)
				item.GetComponent<ShopItem>().setTier(2);
			else
				item.GetComponent<ShopItem>().setTier(1);

			UITexture itemTexture = item.transform.Find("Button").GetChild(0).GetComponent<UITexture>();

			itemTexture.mainTexture = c.menuTexture;
			itemTexture.MakePixelPerfect();

			float factor = itemTexture.height / (ArtikFlowArcade.instance.configuration.characterHeight);
			itemTexture.height = (int) ArtikFlowArcade.instance.configuration.characterHeight;
			itemTexture.width = (int) (itemTexture.width / factor);

			item.transform.Find("Price").Find("Label").GetComponent<UILabel>().text = (c.price > 0) ? ("" + c.price) : Language.get("Shop.Free");
			string icon = "IngameShopCurrency1";
			if(c.price == -1)
				icon = "IngameShopCurrency2";
			else if (c.price == -2)
				icon = "IngameShopCurrency4";
			else if (c.price == -3)
				icon = "IngameShopCurrency3";
			item.transform.Find("Price").Find("Icon").GetComponent<UISprite>().spriteName = icon;

            item.transform.Find("Price").GetComponent<UITable>().enabled = true;    // Recenter

			// Set click trigger
			UIEventTrigger eventTrigger = item.GetComponent<UIEventTrigger>();

			EventDelegate del = new EventDelegate(this, "onCharSelected");
			del.parameters[0] = new EventDelegate.Parameter(item);
			EventDelegate.Add(eventTrigger.onClick, del);

			item.GetComponent<ShopItem>().setState(CharacterManager.instance.isUnlocked(c) ? ShopItem.State.UNLOCKED : ShopItem.State.LOCKED);
        }

		Destroy(template);

		charGrid.enabled = true;
		
		// Disable scrollview if not enough items:
		int files = Mathf.CeilToInt(((charGrid.GetChildList().Count) + 1) / charGrid.maxPerLine);
		if (files < 4)
			scrollView.enabled = false;
		else
		{
			charGrid.pivot = UIWidget.Pivot.Top;
			charGrid.enabled = true;
		}

		Invoke("repositionBottom", 0.2f);
	}

	void repositionBottom()
	{
		GameObject lastItem = getShopItem(CharacterManager.instance.getCharacter(CharacterManager.instance.characters.Length - 1)).gameObject;
		duplicateButton.transform.localPosition = new Vector3(0f, lastItem.transform.localPosition.y + charGrid.transform.localPosition.y - 210, 0f);
		scrollView.transform.Find("Widget_Spacer").localPosition = new Vector3(0f, duplicateButton.transform.localPosition.y - 150, 0f);
	}

	void updateCounters()
	{
		coinsLabel.text = "" + SaveGameSystem.instance.getCoins();
		counterLabel.text = String.Format("{0:D3}/{1:D3}", CharacterManager.instance.getUnlockedCharacters().Count, CharacterManager.instance.characters.Length);
    }

	ShopItem getShopItem(Character c)
	{
		foreach(Transform t in charGrid.transform)
		{
			if (t.name.StartsWith("" + String.Format("{0:D3}", c.id) + "."))
				return t.GetComponent<ShopItem>();
		}

		return null;
	}

	void buy(Character c)
	{
		// Can buy?
		if(SaveGameSystem.instance.getCoins() >= c.price)
		{
			if(c.price < 0) 
			{
				base.unlockCharacter(c);

				GoogleAnalyticsV4.instance.LogEvent("Shop", "SocialUnlock", 
				                                	(c.price == -1 ? "facebook" : (c.price == -2 ? "instagram" : (c.price == -3 ? "twitter" : "unknown")))
													, 0);
				/*
				UnityEngine.Analytics.Analytics.CustomEvent("characterSocialUnlock", new Dictionary<string, object> {
					{ "network", (c.price == -1 ? "facebook" : (c.price == -2 ? "instagram" : (c.price == -3 ? "twitter" : "unknown"))) },
				});
				*/

				if(c.price == -1)
					Application.OpenURL(Facebook_buy_Url);
				else if(c.price == -2)
					Application.OpenURL(Instagram_buy_Url);
				else if(c.price == -3)
					Application.OpenURL(Twitter_buy_Url);

				StartCoroutine(unlockLike(c));
			}
			else 
			{
				Audio.instance.playName("buy_button");
				SaveGameSystem.instance.setCoins(SaveGameSystem.instance.getCoins() - c.price);
				unlock(c);
			}
				GoogleAnalyticsV4.instance.LogEvent("shop", "skins", c.name + " sold", 0);
		}
		else
		{
			Audio.instance.playName("buy_button_disabled");
			iTween.ShakeRotation(getShopItem(c).gameObject, new Vector3(10f, 10f, 10f), 0.3f);

			if(Arcade_Purchaser.instance.isDuringStarterPack())
				PopupManager.instance.showPopup<IPopup_HalfPack>();
			else
				PopupManager.instance.showPopup<IPopup_IAP>();
		}

	}

	void unlock(Character c)
	{
		base.unlockCharacter(c);
		select(c);

		updateCounters();
	}

	IEnumerator unlockLike(Character c)
	{
		yield return new WaitForSeconds(2f);
		unlock(c);
	}

	void select(Character c){
		if( selectedChar != null && CharacterManager.instance.isUnlocked(selectedChar) ){
			getShopItem(selectedChar).setState(ShopItem.State.UNLOCKED);
		}

        getShopItem(c).setState(ShopItem.State.SELECTED);
		selectedChar = c;

		Audio.instance.playName("button");
	}


}

}