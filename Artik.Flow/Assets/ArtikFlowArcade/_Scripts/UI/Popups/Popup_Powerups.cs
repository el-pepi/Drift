using UnityEngine;
using System.Collections;
using System;

namespace AFArcade {

public class Popup_Powerups : IPopup_Powerups
{
	UISprite background;
	UIGrid powerGrid;

	UISprite subBackground;

	Powerup selectedPowerup;
	UIButton selectedButton;

	UITweener[] tweensToReset;
	
	protected override void Awake()
	{
		base.Awake();

		transform.Find("Label_Title").GetComponent<UILabel>().text = Language.get("Powerups.Title");

		background = transform.Find("Background").GetComponent<UISprite>();
		powerGrid = transform.Find("Grid").GetComponent<UIGrid>();
		subBackground = transform.Find("SubPopup").GetComponent<UISprite>();

		tweensToReset = subBackground.GetComponentsInChildren<UITweener>();
    }

	void Start()
	{
		populatePowerups();

		gameObject.SetActive(false);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			base.close();
	}

	protected override void onShow()
	{
		base.onShow();

		selectedPowerup = null;

		if (selectedButton != null)      // Unselect it
		{
			UITexture texture = selectedButton.transform.Find("Texture").GetComponent<UITexture>();
			texture.width = 110;
			texture.height = 110;
			texture.color = new Color(1f, 1f, 1f);
		}
		selectedButton = null;

        foreach (UITweener tween in tweensToReset)
		{
			tween.enabled = false;
			tween.ResetToBeginning();
		}
		updateCounts();

		// Select first powerup:
		onPowerupSelected(transform.Find("Grid").GetChild(0).Find("Button").gameObject);
	}

	// --- Callbacks ---

	public void onPowerupSelected(GameObject item)
	{
		if (selectedButton == item.GetComponent<UIButton>())
			return;

		UITexture texture;
        string name = item.transform.parent.name;

		if(selectedButton != null)      // Unselect it
		{
			texture = selectedButton.transform.Find("Texture").GetComponent<UITexture>();
            texture.width = 110;
			texture.height = 110;
			texture.color = new Color(1f, 1f, 1f);
		}

		selectedButton = item.GetComponent<UIButton>();     // Select it!
		texture = selectedButton.transform.Find("Texture").GetComponent<UITexture>();
		texture.width = 90;
		texture.height = 90;
		texture.color = new Color(0.7f, 0.7f, 0.7f);

		foreach (Powerup p in ArtikFlowArcade.instance.configuration.powerups)
		{
			if(p.internalId == name)
			{
				selectedPowerup = p;
				setSubpopup(p);
				break;
			}
		}

		foreach (UITweener tween in tweensToReset)
		{
			tween.ResetToBeginning();
			tween.enabled = true;
		}

		Audio.instance.playName("button");
	}

	public void onOption1()
	{
		if (SaveGameSystem.instance.getCoins() < selectedPowerup.option1Price)
		{
			if (ArtikFlowArcade.instance.configuration.gemPackCount > 0)
			{
				base.close();
				Popup_NeedGems.goBackToPowerups = true;
				PopupManager.instance.showPopup<IPopup_NeedGems>();
			}
			return;
		}

		SaveGameSystem.instance.setCoins(SaveGameSystem.instance.getCoins() - selectedPowerup.option1Price);
		SaveGameSystem.instance.setPowerupCount(selectedPowerup.internalId, SaveGameSystem.instance.getPowerupCount(selectedPowerup.internalId) + selectedPowerup.option1Count,true);

		ParticleSystem p = selectedButton.transform.parent.Find("Bought_Particles").GetComponent<ParticleSystem>();
		p.time = 0;
		p.Play();
		Audio.instance.playName("buy_button");

		updateCounts();
		setSubpopup(selectedPowerup);			// Enables/disables buttons
	}

	public void onOption2()
	{
		if (SaveGameSystem.instance.getCoins() < selectedPowerup.option2Price)
		{
			if(ArtikFlowArcade.instance.configuration.gemPackCount > 0)
			{
				base.close();
				Popup_NeedGems.goBackToPowerups = true;
				PopupManager.instance.showPopup<IPopup_NeedGems>();
			}
			return;
		}	

		SaveGameSystem.instance.setCoins(SaveGameSystem.instance.getCoins() - selectedPowerup.option2Price);
		SaveGameSystem.instance.setPowerupCount(selectedPowerup.internalId, SaveGameSystem.instance.getPowerupCount(selectedPowerup.internalId) + selectedPowerup.option2Count,true);

		ParticleSystem p = selectedButton.transform.parent.Find("Bought_Particles").GetComponent<ParticleSystem>();
		p.time = 0;
		p.Play();
		Audio.instance.playName("buy_button");

		updateCounts();
		setSubpopup(selectedPowerup);			// Enables/disables buttons
	}

	public void onOptionVideo()
	{
		if (!AFBase.Ads.instance.isRewardedVideoAvailable())
			return;

		AFBase.Ads.RewardedVideoListener listener = delegate (AFBase.Ads.RewardedVideoResult result) {
			if (result == AFBase.Ads.RewardedVideoResult.SUCCESS)
			{
				SaveGameSystem.instance.setPowerupCount(selectedPowerup.internalId, SaveGameSystem.instance.getPowerupCount(selectedPowerup.internalId) + selectedPowerup.videoCount,true);

				ParticleSystem p = selectedButton.transform.parent.Find("Bought_Particles").GetComponent<ParticleSystem>();
				p.time = 0;
				p.Play();
				Audio.instance.playName("buy_button");
			}
			else if (result == AFBase.Ads.RewardedVideoResult.FAIL)
			{
				// Feedback?
			}

			updateCounts();
			setSubpopup(selectedPowerup);		// Enables/disables buttons
		};


		AFBase.Ads.instance.SetRewardedVideoCallback(listener);

		AFBase.Ads.instance.ShowRewardedVideo ();
	}

	// --- Methods ---

	void populatePowerups()
	{
		// Populate characters
		GameObject template = powerGrid.transform.GetChild(0).gameObject;

		int i = 0;
		foreach(Powerup p in ArtikFlowArcade.instance.configuration.powerups)
		{
			GameObject item = NGUITools.AddChild(template.transform.parent.gameObject, template);
			item.name = p.internalId;

			UITexture itemTexture = item.transform.Find("Button").Find("Texture").GetComponent<UITexture>();
			itemTexture.mainTexture = p.texture;

			UIButton button = item.transform.Find("Button").GetComponent<UIButton>();
            EventDelegate del = new EventDelegate(this, "onPowerupSelected");
			del.parameters[0] = new EventDelegate.Parameter(button.gameObject);
			EventDelegate.Add(button.onClick, del);

			i++;
		}
		Destroy(template);

		powerGrid.enabled = true;

		updateCounts();
		Invoke("repositionBottom", 0.2f);
	}

	void repositionBottom()
	{
		int count = powerGrid.GetChildList().Count;
		if (count <= 3)
			background.height = 308;
		else
			background.height = 462;
	}

	void updateCounts()
	{
		foreach(Transform item in powerGrid.transform)
			item.Find("Label_Count").GetComponent<UILabel>().text = "" + SaveGameSystem.instance.getPowerupCount(item.name);
	}

	void setSubpopup(Powerup p)
	{
		subBackground.transform.Find("PowerTexture").GetComponent<UITexture>().mainTexture = p.texture;
		subBackground.transform.Find("Label_PowerTitle").GetComponent<UILabel>().text = Language.get(p.titleTag);
		subBackground.transform.Find("Label_PowerDesc").GetComponent<UILabel>().text = Language.get(p.descriptionTag, false);

		subBackground.transform.Find("Grid").Find("Option1").gameObject.SetActive(p.option1Count > 0);
		if(p.option1Count > 0)
		{
			subBackground.transform.Find("Grid").Find("Option1").Find("Label_Count").GetComponent<UILabel>().text = "" + p.option1Count + "x";
			subBackground.transform.Find("Grid").Find("Option1").Find("Button_Buy").Find("Label_Price").GetComponent<UILabel>().text = "" + p.option1Price;

			subBackground.transform.Find("Grid").Find("Option1").Find("Button_Buy").GetComponent<UIButton>().isEnabled = (SaveGameSystem.instance.getCoins() >= p.option1Price) || ArtikFlowArcade.instance.configuration.gemPackCount > 0;
			subBackground.transform.Find("Grid").Find("Option1").Find("Button_Buy").Find("Label_Price").GetComponent<UILabel>().color = (SaveGameSystem.instance.getCoins() >= p.option1Price) || ArtikFlowArcade.instance.configuration.gemPackCount > 0 ? Color.white : new Color(1f, 1f, 1f, 0.3f);
        }

		subBackground.transform.Find("Grid").Find("Option2").gameObject.SetActive(p.option2Count > 0);
		if (p.option2Count > 0)
		{
			subBackground.transform.Find("Grid").Find("Option2").Find("Label_Count").GetComponent<UILabel>().text = "" + p.option2Count + "x";
			subBackground.transform.Find("Grid").Find("Option2").Find("Button_Buy").Find("Label_Price").GetComponent<UILabel>().text = "" + p.option2Price;

			subBackground.transform.Find("Grid").Find("Option2").Find("Button_Buy").GetComponent<UIButton>().isEnabled = (SaveGameSystem.instance.getCoins() >= p.option2Price) || ArtikFlowArcade.instance.configuration.gemPackCount > 0;
			subBackground.transform.Find("Grid").Find("Option2").Find("Button_Buy").Find("Label_Price").GetComponent<UILabel>().color = (SaveGameSystem.instance.getCoins() >= p.option2Price) || ArtikFlowArcade.instance.configuration.gemPackCount > 0 ? Color.white : new Color(1f, 1f, 1f, 0.3f);
		}

		subBackground.transform.Find("Grid").Find("OptionVideo").gameObject.SetActive(p.videoCount > 0);
		if (p.videoCount > 0)
		{
			subBackground.transform.Find("Grid").Find("OptionVideo").Find("Label_Count").GetComponent<UILabel>().text = "" + p.videoCount + "x";
			subBackground.transform.Find("Grid").Find("OptionVideo").Find("Button_Buy").Find("Label_Price").GetComponent<UILabel>().text = Language.get("Shop.Free");

			subBackground.transform.Find("Grid").Find("OptionVideo").Find("Button_Buy").GetComponent<UIButton>().isEnabled = AFBase.Ads.instance.isRewardedVideoAvailable();
			subBackground.transform.Find("Grid").Find("OptionVideo").Find("Button_Buy").Find("Label_Price").GetComponent<UILabel>().color = AFBase.Ads.instance.isRewardedVideoAvailable() ? Color.white : new Color(1f, 1f, 1f, 0.3f);
		}

		subBackground.transform.Find("Grid").GetComponent<UIGrid>().enabled = true;		// Recalculate
    }

}

}