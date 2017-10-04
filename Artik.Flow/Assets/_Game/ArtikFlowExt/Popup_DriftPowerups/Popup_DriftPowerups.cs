using UnityEngine;
using System.Collections;
using System;

namespace AFArcade {

public class Popup_DriftPowerups : IPopup_Powerups
{
	// Refs
	UIGrid powerGrid;

	GameObject[] levelButtons;
	GameObject[] buyButtons;
	LevelState[] buttonStates;

	UISprite spritePicker;

	public enum LevelState {
		UNLOCKED,			// The powerup has been bought, but it´s not selected.
		SELECTED,			// The level is bought and selected.
		LOCKED_CANBUY,		// The level can't be selected, but can be bought.
		LOCKED_NOBUY,		// The level can't be selected, and can't be bought because a previous one should be bought first.
		LOCKED_LOCKED,		// The level can't be selected, and can't be bought because the player didn't beat it yet.
	}

	// Vars
	bool usedFreePowerup = false;
	public UISprite background;
	protected override void Awake()
	{
		base.Awake();
		
		transform.Find("LevelSelect").Find("Label_Title").GetComponent<UILabel>().text = Language.get("Drift.LevelSelect");
		transform.Find("Powerups").Find("Label_Title").GetComponent<UILabel>().text = Language.get("Powerups.Title");

		levelButtons = new GameObject[5];
		buyButtons = new GameObject[5];
		buttonStates = new LevelState[5];
		for(int i = 0; i < 5; i ++) {
				levelButtons[i] = transform.Find("LevelSelect").Find("Levels").Find("Button" + i).gameObject;
				buyButtons[i] = transform.Find("LevelSelect").Find("BuyButtons").Find("Button" + i).gameObject;
		}

		powerGrid = transform.Find("Powerups").Find("Grid").GetComponent<UIGrid>();
		spritePicker = transform.Find("LevelSelect").Find("LevelPicker").GetComponent<UISprite>();
    }

	void Start()
	{
		/*SaveGameSystem.instance.setPowerupCount("lvl0", 1);
		SaveGameSystem.instance.setPowerupCount("lvl1", 1);
		SaveGameSystem.instance.setPowerupCount("lvl2", 1);
		SaveGameSystem.instance.setPowerupCount("lvl3", 1);
		SaveGameSystem.instance.setPowerupCount("lvl4", 1);*/

		ArtikFlowArcade.instance.eventStateChange.AddListener(onArtikFlowStateChange);

		// Update level prices
		foreach(Powerup p in ArtikFlowArcade.instance.configuration.powerups)
		{
			if(!p.internalId.StartsWith("lvl"))
				continue;

			int levelId = int.Parse(p.internalId.Substring(3));
			buyButtons[levelId].transform.Find("Label_In").GetComponent<UILabel>().text = "" + p.option1Price;
		}

		// Populate not-level powerups
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

		background.height = 725;
		transform.Find ("LevelSelect").gameObject.SetActive (true);

		refreshLevelButtons();
		refreshPowerupButtons();
	}

	void onArtikFlowStateChange(ArtikFlowArcade.State oldstate, ArtikFlowArcade.State newstate)
	{
		if (newstate == ArtikFlowArcade.State.LOST_SCREEN)
		{
			if(usedFreePowerup)
			{
				PlayerPrefs.SetInt("powerupTutorialStep", PlayerPrefs.GetInt("powerupTutorialStep", 0) + 1);
				usedFreePowerup = false;
			}
		}
	}

	// --- Callbacks ---

	public void onLevel0Click() { onLevelClick(0); }
	public void onLevel1Click() { onLevelClick(1); }
	public void onLevel2Click() { onLevelClick(2); }
	public void onLevel3Click() { onLevelClick(3); }
	public void onLevel4Click() { onLevelClick(4); }
	public void onBuy0Click() { onBuyClick(0); }
	public void onBuy1Click() { onBuyClick(1); }
	public void onBuy2Click() { onBuyClick(2); }
	public void onBuy3Click() { onBuyClick(3); }
	public void onBuy4Click() { onBuyClick(4); }
	
	void onLevelClick(int levelId)
	{
		if(buttonStates[levelId] == LevelState.UNLOCKED)
		{
			Audio.instance.playName("popup_in");
			PlayerPrefs.SetInt("selectedLevel", levelId);
			refreshLevelButtons();
			PowerUpManager.instace.SetBoss();
		}
		else if(buttonStates[levelId] != LevelState.SELECTED)
			Audio.instance.playName("buy_button_disabled");
	}
	
	public void onBuyClick(int levelId)
	{
		Powerup powerup = getPowerupByName("lvl" + levelId);

		if(SaveGameSystem.instance.getCoins() < powerup.option1Price)
		{
			base.close();
			PopupManager.instance.showPopup<IPopup_IAP>();
			return;
		}

		SaveGameSystem.instance.setCoins(SaveGameSystem.instance.getCoins() - powerup.option1Price);
		SaveGameSystem.instance.setPowerupCount(powerup.internalId, 1);

		Audio.instance.playName("buy_button");
		PlayerPrefs.SetInt("selectedLevel", levelId);
		refreshLevelButtons();
		PowerUpManager.instace.SetBoss();
	}

	void onCoinClick(Powerup powerup)
	{
		// Tutorial
		int price = getPowerupPrice(powerup.internalId);
		if(price == 0)
			usedFreePowerup = true;
		
		// The rest
		if(SaveGameSystem.instance.getCoins() < price)
		{
			base.close();
			PopupManager.instance.showPopup<IPopup_IAP>();
			return;
		}

		SaveGameSystem.instance.setCoins(SaveGameSystem.instance.getCoins() - price);
		SaveGameSystem.instance.setPowerupCount(powerup.internalId, 1);

		Audio.instance.playName("buy_button");
		refreshPowerupButtons();
	}

	void onVideoClick(Powerup powerup)
	{
		if (!AFBase.Ads.instance.isRewardedVideoAvailable())
			return;

		AFBase.Ads.RewardedVideoListener listener = delegate (AFBase.Ads.RewardedVideoResult result) {
			if (result == AFBase.Ads.RewardedVideoResult.SUCCESS)
			{
				SaveGameSystem.instance.setPowerupCount(powerup.internalId, 1);
				Audio.instance.playName("buy_button");
			}
			else if (result == AFBase.Ads.RewardedVideoResult.FAIL)
			{
				// Feedback?
			}

			refreshPowerupButtons();
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
		foreach(Powerup powerup in ArtikFlowArcade.instance.configuration.powerups)
		{
			DriftPowerup p = powerup as DriftPowerup;
			if(p == null)
				continue;

			GameObject item = NGUITools.AddChild(template.transform.parent.gameObject, template);
			item.name = p.internalId;

			UITexture itemTexture = item.transform.Find("Powerup_Texture").GetComponent<UITexture>();
			itemTexture.mainTexture = p.menuTexture;

			item.transform.Find("Label_Title").GetComponent<UILabel>().text = Language.get(p.titleTag);
			if(i == 1)
				item.transform.Find("Label_Title").GetComponent<UILabel>().color = new Color32(77, 210, 255, 255);
			else if(i == 2)
				item.transform.Find("Label_Title").GetComponent<UILabel>().color = new Color32(238, 254, 75, 255);
			else if(i == 0)
				item.transform.Find("Label_Title").GetComponent<UILabel>().color = new Color32(252, 75, 203, 255);

			item.transform.Find("Label_Desc").GetComponent<UILabel>().text = Language.get(p.descriptionTag);
			item.transform.Find("Button_Equipped").GetComponentInChildren<UILabel>().text = Language.get("Drift.Equipped");
			item.transform.Find("Button_Coins").GetComponentInChildren<UILabel>().text = "" + p.option1Price;

            EventDelegate del = new EventDelegate(this, "onCoinClick");
			del.parameters[0] = new EventDelegate.Parameter(p);
			UIPlayAnimation button = item.transform.Find("Button_Coins").GetComponent<UIPlayAnimation>();
			
			EventDelegate.Add(button.onFinished, del);
			del = new EventDelegate(this, "onVideoClick");
			del.parameters[0] = new EventDelegate.Parameter(p);
			button = item.transform.Find("Button_Video").GetComponent<UIPlayAnimation>();
			EventDelegate.Add(button.onFinished, del);

			i++;
		}
		Destroy(template);

		powerGrid.enabled = true;
	}

	void refreshPowerupButtons()
	{
		int i = 0;
		foreach(Transform t in powerGrid.transform)
		{
			string powerName = t.name;

			if(SaveGameSystem.instance.getPowerupCount(powerName) >= 1)
			{
				t.Find("Button_Video").gameObject.SetActive(false);
				t.Find("Button_Coins").gameObject.SetActive(false);
				t.Find("Button_Equipped").gameObject.SetActive(true);
			}
			else
			{
				int price = getPowerupPrice(powerName);

				// Setup
				if(price == 0)
				{
					t.Find("Button_Coins").gameObject.SetActive(false);
					t.Find("Button_Video").gameObject.SetActive(true);
				}
				else
				{
					// Add video condition here
					t.Find("Button_Coins").gameObject.SetActive(true);
					t.Find("Button_Video").gameObject.SetActive(false);
				}
				
				t.Find("Button_Equipped").gameObject.SetActive(false);
			}

			i ++;
		}
	}

	void refreshLevelButtons()
	{
		int maxLevel = PlayerPrefs.GetInt("maxLevelReached", 0);
		int selectedLevel = PlayerPrefs.GetInt("selectedLevel", 0);
		bool showedBuyable = false;

		for(int i = 0; i < 5; i ++)
		{
			if(selectedLevel == i)
				setLevelState(i, LevelState.SELECTED);
			else if(SaveGameSystem.instance.getPowerupCount("lvl" + i) > 0 || i == 0)
				setLevelState(i, LevelState.UNLOCKED);
			else
			{
				if(i > maxLevel)
					setLevelState(i, LevelState.LOCKED_LOCKED);
				else
				{
					if(!showedBuyable)
					{
						setLevelState(i, LevelState.LOCKED_CANBUY);
						showedBuyable = true;
					}
					else
						setLevelState(i, LevelState.LOCKED_NOBUY);
				}
			}
		}
	}

	IEnumerator animLevelPicker()
	{
		yield return null;
		iTween.ScaleTo(spritePicker.gameObject, iTween.Hash("x", 1f, "y", 1f, "time", 0.5f));
	}

	public void SetOnlyPowerUps()
	{
		background.height = 490;
		transform.Find ("LevelSelect").gameObject.SetActive (false);
	}

	void setLevelState(int levelId, LevelState state)
	{
		buttonStates[levelId] = state;

		if(state == LevelState.SELECTED)
		{
			levelButtons [levelId].gameObject.SetActive (true);
			levelButtons[levelId].transform.Find("Label_In").gameObject.SetActive(true);
			levelButtons[levelId].transform.Find("Sprite_Lock").gameObject.SetActive(false);
			buyButtons[levelId].gameObject.SetActive(false);
				levelButtons [levelId].GetComponent<UISprite> ().enabled = true;
				/*
			Color col = levelButtons[levelId].GetComponent<UISprite>().color;
			col.a = 1f;
			levelButtons[levelId].defaultColor = col;
			levelButtons[levelId].hover = col;
			levelButtons[levelId].pressed = col;*/

			// Move picker:
			spritePicker.transform.localPosition = new Vector3(
				levelButtons[levelId].transform.localPosition.x,
				spritePicker.transform.localPosition.y,
				spritePicker.transform.localPosition.z
			);
			spritePicker.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
			iTween.Stop(spritePicker.gameObject);
			StartCoroutine(animLevelPicker());
		}
		else if(state == LevelState.UNLOCKED)
		{
			levelButtons [levelId].gameObject.SetActive (true);
			levelButtons[levelId].transform.Find("Label_In").gameObject.SetActive(true);
			levelButtons[levelId].transform.Find("Sprite_Lock").gameObject.SetActive(false);
				levelButtons [levelId].GetComponent<UISprite> ().enabled = true;
			buyButtons[levelId].gameObject.SetActive(false);

				/*
			Color col = levelButtons[levelId].GetComponent<UISprite>().color;
			col.a = 1f;
			levelButtons[levelId].defaultColor = col;
			levelButtons[levelId].hover = col;
			levelButtons[levelId].pressed = col;*/
		}
		else if(state == LevelState.LOCKED_CANBUY)
		{	
				
			levelButtons [levelId].gameObject.SetActive (false);
			buyButtons[levelId].gameObject.SetActive(true);
			
				/*
			Color col = levelButtons[levelId].GetComponent<UISprite>().color;
			col.a = 0.45f;
			levelButtons[levelId].defaultColor = col;
			levelButtons[levelId].hover = col;
			levelButtons[levelId].pressed = col;*/
		}
		else if(state == LevelState.LOCKED_NOBUY)
		{		
			buyButtons[levelId].gameObject.SetActive(true);
			levelButtons[levelId].transform.Find("Label_In").gameObject.SetActive(true);
			levelButtons[levelId].transform.Find("Sprite_Lock").gameObject.SetActive(false);
			levelButtons [levelId].GetComponent<UISprite> ().enabled = true;
			buyButtons[levelId].gameObject.SetActive(false);

				/*
			Color col = levelButtons[levelId].GetComponent<UISprite>().color;
			col.a = 0.45f;
			levelButtons[levelId].defaultColor = col;
			levelButtons[levelId].hover = col;
			levelButtons[levelId].pressed = col;*/
		}
		else if(state == LevelState.LOCKED_LOCKED)
		{		
			buyButtons[levelId].gameObject.SetActive(true);
			levelButtons[levelId].transform.Find("Label_In").gameObject.SetActive(false);
			levelButtons[levelId].transform.Find("Sprite_Lock").gameObject.SetActive(true);
			levelButtons [levelId].GetComponent<UISprite> ().enabled = false;
			buyButtons[levelId].gameObject.SetActive(false);

				/*
			Color col = levelButtons[levelId].GetComponent<UISprite>().color;
			col.a = 0.45f;
			levelButtons[levelId].defaultColor = col;
			levelButtons[levelId].hover = col;
			levelButtons[levelId].pressed = col;*/
		}
	}

	Powerup getPowerupByName(string name)
	{
		foreach(Powerup p in ArtikFlowArcade.instance.configuration.powerups)
		{
			if(p.internalId == name)
				return p;
		}

		return null;
	}
	
	public void OnPlay()
	{
		hide ();
		ArtikFlowArcade.instance.setState(ArtikFlowArcade.State.GAMEPLAY_SCREEN);
	}
	
	int getPowerupPrice(string name)
	{
		// If used free powerup, return real price
		//if(usedFreePowerup)
			return getPowerupByName(name).option1Price;

		// Otherwise, check if in tutorial
		int step = PlayerPrefs.GetInt("powerupTutorialStep", 0);
		if((step == 0 && name == "Bonus Health") || (step == 1 && name == "Magnet") || (step == 2 && name == "Bonus Weapon"))
			return 0;

		return getPowerupByName(name).option1Price;
	}

}

}