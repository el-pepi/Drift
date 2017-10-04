using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using VoxelBusters.NativePlugins;
using AFBase;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace AFArcade {

[CustomEditor(typeof(ArtikFlowArcadeConfiguration))]
public class ArtikFlowConfigurationEditor : Editor
{
	static Color32[] ROULETTEBAR_COLORS = new Color32[8] {
		new Color32(160, 127, 219, 255),
		new Color32(107, 244, 144, 255),
		new Color32(239, 134, 186, 255),
		new Color32(206, 130, 16, 255),
		new Color32(230, 169, 242, 255),
		new Color32(214, 20, 6, 255),
		new Color32(118, 138, 252, 255),
		new Color32(98, 147, 17, 255),
	};
	static GUIStyle smallHeaderStyle;

	public enum Tab	{
		MAIN = 0,
		GAMESERVICES,
		CHARACTERS,
		POWERUPS,
		IAPS,
		DEBUG,
	}

	public struct PostGUIAction {
		public string action;
		public UnityEngine.Object target;
	}

	private Tab currentTab;
	private ArtikFlowArcadeConfiguration config;
	private SerializedObject serializedConfig;

	// Main
	private bool mainBasicOpened = true;
	private Vector2 mainScrollPos;
	private bool mainExtendedOpened;
	private bool mainRouletteOpened;
	private bool mainExtensionsOpened;
	private bool mainFontsOpened;

	// GameServices
	private bool gsShowPlayphone;

	// Characters
	private Vector2 charsScrollPos;
	private Character charSelected;
	private Editor charEditor;
	private PostGUIAction charAction;
	private bool charJustAdded;

	// Powerups
	private Vector2 powerScrollPos;
	private Powerup powerSelected;
	private Editor powerEditor;
	private PostGUIAction powerAction;
	private bool powerJustAdded;

	// RouletteItems
	private RouletteItem rouletteItemSelected;
	private Editor rouletteItemEditor;
	private PostGUIAction rouletteAction;
	private bool rouletteItemJustAdded;

	// IAPs
	private Vector2 IAPScrollPos;
	private ArtikProduct IAPSelected;
	private Editor IAPEditor;
	private PostGUIAction IAPAction;
	private bool IAPJustAdded;

	void Awake()
	{
		smallHeaderStyle = new GUIStyle(EditorStyles.boldLabel);
		smallHeaderStyle.contentOffset = new Vector2(0f, 3f);
	}

	void OnEnable()
	{
		config = (ArtikFlowArcadeConfiguration)target;
		serializedConfig = new SerializedObject(config);
    }

	public override void OnInspectorGUI()
	{
		serializedConfig.Update();
		drawHeader();

		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		GUIContent[] contents = new GUIContent[] {
			new GUIContent("Main", getMainError() == null ? null : Resources.Load<Texture2D>("Editor/Textures/warning")),
			new GUIContent("GameServices"),
			new GUIContent("Characters", getCharactersError() == null ? null : Resources.Load<Texture2D>("Editor/Textures/warning")),
			new GUIContent("Powerups", getPowerupsError() == null ? null : Resources.Load<Texture2D>("Editor/Textures/warning")),
			new GUIContent("IAPs", getIAPsError() == null ? null : Resources.Load<Texture2D>("Editor/Textures/warning")),
			new GUIContent("Debug", isDebugActive() ? Resources.Load<Texture2D>("Editor/Textures/warning") : null),
		};
		Tab newTab = (Tab) GUILayout.Toolbar((int) currentTab, contents, GUILayout.Height(25));
		GUILayout.Space(10);

		if(newTab != currentTab)
		{
			if (newTab == Tab.MAIN)
			{
				mainScrollPos = Vector2.zero;
				rouletteItemSelected = null;
				mainBasicOpened = true;
			}
			else if (newTab == Tab.GAMESERVICES)
				gsShowPlayphone = false;
			else if(newTab == Tab.CHARACTERS)
			{
				charsScrollPos = Vector2.zero;
				charSelected = null;
			}
			else if(newTab == Tab.POWERUPS)
			{
				powerScrollPos = Vector2.zero;
				powerSelected = null;
			}
			else if (newTab == Tab.IAPS)
			{
				IAPScrollPos = Vector2.zero;
				IAPSelected = null;
			}
			currentTab = newTab;
		}

		switch(currentTab)
		{
			case Tab.MAIN:
				drawMain();
				break;

			case Tab.GAMESERVICES:
				drawGameServices();
				break;

			case Tab.CHARACTERS:
				drawCharacters();
				break;

			case Tab.POWERUPS:
				drawPowerups();
				break;

			case Tab.IAPS:
				drawIAPs();
				break;

			case Tab.DEBUG:
				drawDebug();
				break;
		}
		EditorGUILayout.EndVertical();

		onPostGUI();

		serializedConfig.ApplyModifiedPropertiesWithoutUndo();
		if (GUI.changed)
			EditorUtility.SetDirty(config);
	}

	private void onPostGUI()
	{
		// Characters:
		if(charAction.action != null)
		{
			Character c = (Character) charAction.target;
			int charIndex = Array.IndexOf(config.characters, c);

			if (charAction.action == "add")
			{
				if(charIndex == -1)		// Dont add if repeated
				{
					Array.Resize(ref config.characters, config.characters.Length + 1);
					config.characters[config.characters.Length - 1] = c;
				}
			}
			else if(charAction.action == "up")
			{
				Character swap = config.characters[charIndex - 1];
				config.characters[charIndex - 1] = config.characters[charIndex];
				config.characters[charIndex] = swap;
			}
			else if (charAction.action == "down")
			{
				Character swap = config.characters[charIndex + 1];
				config.characters[charIndex + 1] = config.characters[charIndex];
				config.characters[charIndex] = swap;
			}
			else if (charAction.action == "kill")
			{
				// Resize
				for (int i = charIndex; i < config.characters.Length - 1; i ++)
					config.characters[i] = config.characters[i + 1];

				Array.Resize(ref config.characters, config.characters.Length - 1);
			}

			charSelected = null;
			charAction.action = null;
		}

		// Powerups:
		if (powerAction.action != null)
		{
			Powerup p = (Powerup)powerAction.target;
			int powerIndex = Array.IndexOf(config.powerups, p);

			if(powerAction.action == "add")
			{
				if(powerIndex == -1)        // Dont add if repeated
				{
					Array.Resize(ref config.powerups, config.powerups.Length + 1);
					config.powerups[config.powerups.Length - 1] = p;
				}
			}
			else if(powerAction.action == "up")
			{
				Powerup swap = config.powerups[powerIndex - 1];
				config.powerups[powerIndex - 1] = config.powerups[powerIndex];
				config.powerups[powerIndex] = swap;
			}
			else if(powerAction.action == "down")
			{
				Powerup swap = config.powerups[powerIndex + 1];
				config.powerups[powerIndex + 1] = config.powerups[powerIndex];
				config.powerups[powerIndex] = swap;
			}
			else if(powerAction.action == "kill")
			{
				// Resize
				for (int i = powerIndex; i < config.powerups.Length - 1; i++)
					config.powerups[i] = config.powerups[i + 1];

				Array.Resize(ref config.powerups, config.powerups.Length - 1);
			}

			powerSelected = null;
			powerAction.action = null;
		}

		// IAP:
		if (IAPAction.action != null)
		{
			ArtikProduct iap = (ArtikProduct)IAPAction.target;
			int iapIndex = Array.IndexOf(config.products, iap);

			if (IAPAction.action == "add")
			{
				if (iapIndex == -1)        // Dont add if repeated
				{
					Array.Resize(ref config.products, config.products.Length + 1);
					config.products[config.products.Length - 1] = iap;
				}
			}
			else if (IAPAction.action == "up")
			{
				ArtikProduct swap = config.products[iapIndex - 1];
				config.products[iapIndex - 1] = config.products[iapIndex];
				config.products[iapIndex] = swap;
			}
			else if (IAPAction.action == "down")
			{
				ArtikProduct swap = config.products[iapIndex + 1];
				config.products[iapIndex + 1] = config.products[iapIndex];
				config.products[iapIndex] = swap;
			}
			else if (IAPAction.action == "kill")
			{
				// Resize
				for (int i = iapIndex; i < config.products.Length - 1; i++)
					config.products[i] = config.products[i + 1];

				Array.Resize(ref config.products, config.products.Length - 1);
			}

			IAPSelected = null;
			IAPAction.action = null;
		}

		// Roulette Item
		if(rouletteAction.action != null)
		{
			RouletteItem targetItem = (RouletteItem)rouletteAction.target;

			if(rouletteAction.action.StartsWith("add"))
			{
				int sliceIdx = int.Parse(rouletteAction.action.Split('.')[1]);
				RouletteItem[] sliceItems = config.rouletteSlices[sliceIdx].possibleItems;
				int itemIndex = Array.IndexOf(sliceItems, targetItem);

				if(itemIndex == -1)			// Don't add if repeated
				{
					Array.Resize(ref sliceItems, sliceItems.Length + 1);
					Array.Resize(ref config.rouletteSlices[sliceIdx].itemProbabilities, config.rouletteSlices[sliceIdx].itemProbabilities.Length + 1);
					sliceItems[sliceItems.Length - 1] = targetItem;
					config.rouletteSlices[sliceIdx].itemProbabilities[config.rouletteSlices[sliceIdx].itemProbabilities.Length - 1] = 0.5f;
				}
				else
					Debug.Log("[EDITOR WARNING] This item already exists in this slice!");

				config.rouletteSlices[sliceIdx].possibleItems = sliceItems;
			}
			else if(rouletteAction.action.StartsWith("kill"))
			{
				int sliceIdx = int.Parse(rouletteAction.action.Split('.')[1]);
				RouletteItem[] sliceItems = config.rouletteSlices[sliceIdx].possibleItems;
				int itemIndex = Array.IndexOf(sliceItems, targetItem);

				if(itemIndex != -1)
				{
					for(int i = itemIndex; i < sliceItems.Length - 1; i ++)
					{
						sliceItems[i] = sliceItems[i + 1];
						config.rouletteSlices[sliceIdx].itemProbabilities[i] = config.rouletteSlices[sliceIdx].itemProbabilities[i + 1];
					}

					Array.Resize(ref sliceItems, sliceItems.Length - 1);
					Array.Resize(ref config.rouletteSlices[sliceIdx].itemProbabilities, config.rouletteSlices[sliceIdx].itemProbabilities.Length - 1);

					config.rouletteSlices[sliceIdx].possibleItems = sliceItems;
				}
			}

			rouletteItemSelected = null;
			rouletteAction.action = null;
		}
	}

	private void drawHeader()
	{
		GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.BeginVertical(EditorStyles.helpBox);

		headerStyle.fontSize = 17;
		headerStyle.alignment = TextAnchor.UpperLeft;
		GUILayout.Space(-1);
		GUILayout.Label(Application.productName, headerStyle);
		headerStyle.fontSize = 11;
		GUILayout.Space(-7);
		GUILayout.Label(Application.bundleIdentifier, headerStyle);
		GUILayout.Space(-7);
		GUILayout.Label("v" + Application.version, headerStyle);

		EditorGUILayout.EndVertical();
		EditorGUILayout.BeginVertical(EditorStyles.helpBox);

		headerStyle.fontSize = 17;
		headerStyle.alignment = TextAnchor.UpperRight;
		GUILayout.Space(-1);
		GUILayout.Label("ArtikFlow Arcade", headerStyle);
		headerStyle.fontSize = 11;
		GUILayout.Space(-7);
		GUILayout.Label("Base: " + ArtikFlowBase.BASE_VERSION, headerStyle);
		GUILayout.Space(-7);
		GUILayout.Label("Arcade: " + ArtikFlowArcade.VERSION, headerStyle);

		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
    }

	private void drawError(string error)
	{
		if (error == null)
			return;

		GUI.backgroundColor = Color.red;
		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		EditorGUILayout.LabelField("Error:");
		EditorGUILayout.LabelField("- " + error);
		EditorGUILayout.EndVertical();
		GUI.backgroundColor = Color.white;
	}

	private void drawMain()
	{
		drawError(getMainError());
		mainScrollPos = EditorGUILayout.BeginScrollView(mainScrollPos);

		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		mainBasicOpened = GUILayout.Toggle(mainBasicOpened, "Basic", "Button");
		if (mainBasicOpened)
			drawMainBasic();
		EditorGUILayout.EndVertical();

		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		mainExtendedOpened = GUILayout.Toggle(mainExtendedOpened, "Advanced", "Button");
		if (mainExtendedOpened)
			drawMainExtended();
		EditorGUILayout.EndVertical();

		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		mainRouletteOpened = GUILayout.Toggle(mainRouletteOpened, "Roulette configuration", "Button");
		if (mainRouletteOpened)
			drawMainRoulette();
		EditorGUILayout.EndVertical();

		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		mainExtensionsOpened = GUILayout.Toggle(mainExtensionsOpened, "Extensions", "Button");
		if (mainExtensionsOpened)
			drawMainExtensions();
		EditorGUILayout.EndVertical();

		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		mainFontsOpened = GUILayout.Toggle(mainFontsOpened, "Fonts Override", "Button");
		if (mainFontsOpened)
			drawMainFonts();
		EditorGUILayout.EndVertical();

		EditorGUILayout.EndScrollView();

		// --- Bottom box ---
		if(rouletteItemSelected != null)
		{
			if (rouletteItemEditor == null || rouletteItemEditor.target != rouletteItemSelected)
				rouletteItemEditor = Editor.CreateEditor(rouletteItemSelected);

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			rouletteItemEditor.OnInspectorGUI();
			EditorGUILayout.EndVertical();
        }
	}

	private void drawMainBasic()
	{
		// --- Basic ---
		GUILayout.Label("Basic Configuration", smallHeaderStyle);
		config.gameScene = EditorGUILayout.TextField("Game Scene", config.gameScene);

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Icon");
		Texture2D prev_icon = config.icon;
		config.icon = EditorGUILayout.ObjectField(config.icon, typeof(Texture2D), false) as Texture2D;
		if (config.icon != prev_icon && config.icon != null)
		{
			FileUtil.ReplaceFile(AssetDatabase.GetAssetPath(config.icon), "Assets/PluginResources/NativePlugins/Android/notif.png");
			NPSettings.Notification.Android.ColouredSmallIcon = config.icon;
		}
			
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Logo");
		config.gameLogoTexture = EditorGUILayout.ObjectField(config.gameLogoTexture, typeof(Texture2D), false) as Texture2D;
		EditorGUILayout.EndHorizontal();

		EditorGUI.BeginChangeCheck();
		config.storeTarget = (ArtikFlowArcadeConfiguration.StoreTarget) EditorGUILayout.EnumPopup("Store Target", config.storeTarget, new GUILayoutOption[] { });
		if(config.storeTarget == ArtikFlowArcadeConfiguration.StoreTarget.FRENCH_PREMIUM)
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

			config.FRENCH_PREMIUM_tryNBuy = EditorGUILayout.Toggle("Try N Buy", config.FRENCH_PREMIUM_tryNBuy);
			if(config.FRENCH_PREMIUM_tryNBuy)
			{
				config.FRENCH_PREMIUM_tryNBuyId = EditorGUILayout.TextField("Try N Buy ID", config.FRENCH_PREMIUM_tryNBuyId);
			}

			EditorGUILayout.EndVertical();
		}
		if(EditorGUI.EndChangeCheck())
			config.storeUpdated();

		// --- Analytics ---
		GUILayout.Label("Google analytics", smallHeaderStyle);
		config.androidTrackingCode = EditorGUILayout.TextField("Android Tracking Code", config.androidTrackingCode);
		config.iosTrackingCode = EditorGUILayout.TextField("iOS Tracking Code", config.iosTrackingCode);

		// --- Other ---
		GUILayout.Label("Other", smallHeaderStyle);
		config.gameAudio = EditorGUILayout.ObjectField("Game Audio", config.gameAudio, typeof(GameObject), false) as GameObject;
		config.iOS_StoreId = EditorGUILayout.TextField("iOS Store ID", config.iOS_StoreId);
		EditorGUI.BeginChangeCheck();
		config.forceStoragePermission = EditorGUILayout.Toggle("(Android) Force STORAGE Permission", config.forceStoragePermission);
		if(EditorGUI.EndChangeCheck())
			config.storagePermissionUpdated();
		config.disablePlayerStatsAPI = EditorGUILayout.Toggle("Disable Player Stats API", config.disablePlayerStatsAPI);

		GUILayout.Label("Ads Cache", smallHeaderStyle);
		config.cleanOnApplicationPause = EditorGUILayout.Toggle("Clean ads cache on PAUSE", config.cleanOnApplicationPause);
		config.cleanOnApplicationQuit = EditorGUILayout.Toggle("Clean ads cache on QUIT", config.cleanOnApplicationQuit);
	}

	private void drawMainExtended()
	{
		// --- Global ---
		GUILayout.Label("Global", smallHeaderStyle);
		config.charactersEnabled = EditorGUILayout.Toggle("Enable Characters", config.charactersEnabled);
		config.enableCoins = EditorGUILayout.Toggle("Enable Coins", config.enableCoins);
		config.enableRevive = EditorGUILayout.Toggle("Enable Revive", config.enableRevive);
		config.displayScoreIngame = EditorGUILayout.Toggle("Display Score Ingame", config.displayScoreIngame);
		config.displayCoinsIngame = EditorGUILayout.Toggle("Display Coins Ingame", config.displayCoinsIngame);
		config.useWhiteLabels = EditorGUILayout.Toggle("Use White Labels", config.useWhiteLabels);
		config.sharingText = EditorGUILayout.TextField("Sharing Text", config.sharingText);

		// --- StartScreen ---
		GUILayout.Label("StartScreen", smallHeaderStyle);
		EditorGUILayout.PropertyField(serializedConfig.FindProperty("timeForRewards"), true);
		EditorGUILayout.PropertyField(serializedConfig.FindProperty("gameModes"), true);
		config.replaceDailyWithRoulette = EditorGUILayout.Toggle("Use Roulette Instead Of Gift", config.replaceDailyWithRoulette);
		if(config.replaceDailyWithRoulette)
			config.gemsToSpin = EditorGUILayout.IntField("Gems To Spin", config.gemsToSpin);
		else
			config.dailyPrize = EditorGUILayout.IntField("Daily Prize", config.dailyPrize);
		config.showTapToPlay = EditorGUILayout.Toggle("Show Tap To Play", config.showTapToPlay);
		config.scoresYPosition = EditorGUILayout.FloatField("Scores Y Position", config.scoresYPosition);
		config.hiddenLogoEnabled = EditorGUILayout.Toggle("Enable Hidden Logo", config.hiddenLogoEnabled);

		// --- LostScreen ---
		GUILayout.Label("LostScreen", smallHeaderStyle);
		config.videoReward = EditorGUILayout.IntField("Video Reward", config.videoReward);
		config.videoAdFrequency = EditorGUILayout.IntField("Video Ad Frequency", config.videoAdFrequency);

		// --- Shop ---
		GUILayout.Label("Shop", smallHeaderStyle);
		config.characterHeight = EditorGUILayout.FloatField("Character Height", config.characterHeight);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Shop Icon");
		config.shopIcon = EditorGUILayout.ObjectField(config.shopIcon, typeof(Texture2D), false) as Texture2D;
		EditorGUILayout.EndHorizontal();

		// --- Popups ---
		GUILayout.Label("Popups", smallHeaderStyle);
		config.disableRatePopup = EditorGUILayout.Toggle("Disable Rate Popup", config.disableRatePopup);
        config.gameToShowRate = EditorGUILayout.IntSlider("Game To Show Rate", config.gameToShowRate, 1, 30);
		config.IAPPromoGameToShow = EditorGUILayout.IntSlider("Game To Show Holiday Pack (if active)", config.IAPPromoGameToShow, 1, 30);

		// --- Ads ---
		GUILayout.Label("Ads", smallHeaderStyle);
		config.bannerAtBottom = EditorGUILayout.Toggle("Banner At Bottom", config.bannerAtBottom);
		config.interstitialFrequency = EditorGUILayout.IntSlider("Interstitial Frequency", config.interstitialFrequency, 1, 20);

		// --- Revive ---
		GUILayout.Label("Revive", smallHeaderStyle);
		config.coinsToRevive = EditorGUILayout.IntField("Coins To Revive", config.coinsToRevive);
		config.canCancel = EditorGUILayout.Toggle("Can Cancel Revive", config.canCancel);
		config.timeToRevive = EditorGUILayout.IntSlider("Time To Revive", config.timeToRevive, 1, 9);

		// --- Notifications ---
		GUILayout.Label("Notification Gifts (time without playing)", smallHeaderStyle);
		config.notificationFirstGift = EditorGUILayout.IntField("After 1 day", config.notificationFirstGift);
		config.notificationSecondGift = EditorGUILayout.IntField("After 1 week", config.notificationSecondGift);
		config.notificationThirdGift = EditorGUILayout.IntField("After 1 month", config.notificationThirdGift);
		
		// --- URLs ---
		GUILayout.Label("URLs", smallHeaderStyle);
		config.Android_StarUrl = EditorGUILayout.TextField("Review Android URL", config.Android_StarUrl);
		config.iOS_StarUrl = EditorGUILayout.TextField("Review iOS URL", config.iOS_StarUrl);
		config.Playphone_StarUrl = EditorGUILayout.TextField("Review Playphone URL", config.Playphone_StarUrl);
		GUILayout.Space(5);
		config.Android_MoreURL = EditorGUILayout.TextField("MoreGames Android URL", config.Android_MoreURL);
		config.iOS_MoreURL = EditorGUILayout.TextField("MoreGames iOS URL", config.iOS_MoreURL);
		config.Playphone_MoreURL = EditorGUILayout.TextField("MoreGames Playphone URL", config.Playphone_MoreURL);
		GUILayout.Space(5);
		config.Facebook_buy_Url = EditorGUILayout.TextField("Facebook URL", config.Facebook_buy_Url);
		config.Twitter_buy_Url = EditorGUILayout.TextField("Twitter URL", config.Twitter_buy_Url);
		config.Instagram_buy_Url = EditorGUILayout.TextField("Instagram URL", config.Instagram_buy_Url);

		// --- Fonts ---
		GUILayout.Label("Fonts", smallHeaderStyle);
		EditorGUILayout.PropertyField(serializedConfig.FindProperty("referenceFonts"), true);
		EditorGUILayout.PropertyField(serializedConfig.FindProperty("bitmapFonts"), true);
		EditorGUILayout.PropertyField(serializedConfig.FindProperty("dynamicFonts"), true);

		// config.Android_StarUrl = EditorGUILayout.TextField("Android Review URL", config.Android_StarUrl);
		// config.Playphone_StarUrl = EditorGUILayout.TextField("Playphone Review URL", config.Playphone_StarUrl);
	}

	private void drawMainRoulette()
	{
		// --- Main ---
		GUILayout.Label("Main", smallHeaderStyle);
		EditorGUI.BeginChangeCheck();
		config.rouletteSliceCount = Mathf.Clamp(EditorGUILayout.DelayedIntField("Roulette Slices", config.rouletteSliceCount), 0, 16);
		if(EditorGUI.EndChangeCheck())		// Update slices
		{
			ArtikFlowArcadeConfiguration.RouletteSlice[] newSlices = new ArtikFlowArcadeConfiguration.RouletteSlice[config.rouletteSliceCount];
			for(int i = 0; i < Mathf.Min(newSlices.Length, config.rouletteSlices.Length); i ++)		// Copy old values
				newSlices[i] = config.rouletteSlices[i];
			for(int i = config.rouletteSlices.Length; i < newSlices.Length; i ++)					// Default values for new ones
			{
				newSlices[i] = new ArtikFlowArcadeConfiguration.RouletteSlice();
				newSlices[i].probability = 0.5f;
				newSlices[i].possibleItems = new RouletteItem[0];
				newSlices[i].itemProbabilities = new float[0];
			}
			config.rouletteSlices = newSlices;
		}
		
		// --- Slices ---
		if(config.rouletteSlices == null || config.rouletteSliceCount == 0)		// Skips the rest of the roulette config!
			return;

		GUILayout.Label("Slices", smallHeaderStyle);
		GUILayout.Space(30f);

		// Probability distribution
		float totalProb = 0f;
		foreach(ArtikFlowArcadeConfiguration.RouletteSlice slice in config.rouletteSlices)
			totalProb += slice.probability;

		float BAR_LENGTH = 550f;
		float bar_x = GUILayoutUtility.GetLastRect().x + 10f;
		float bar_y = GUILayoutUtility.GetLastRect().y + 7f;
		GUIContent c = new GUIContent();
		for(int i = 0; i < config.rouletteSlices.Length; i ++)
		{
			float size = (config.rouletteSlices[i].probability / totalProb) * BAR_LENGTH;

			EditorGUITools.DrawRect(new Rect(bar_x, bar_y, size, 20f), ROULETTEBAR_COLORS[i % 8], new GUIContent("" + i));
			bar_x += size;
		}

		// Draw each slice properties
		for(int sliceIdx = 0; sliceIdx < config.rouletteSlices.Length; sliceIdx ++)
		{
			GUILayout.Space(15);	
			drawRouletteSlice(sliceIdx);
		}
	}

	private void drawRouletteSlice(int sliceIdx)
	{
		Color sliceColor = ROULETTEBAR_COLORS[sliceIdx % ROULETTEBAR_COLORS.Length];
		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		
		float total = 0f;
		foreach(ArtikFlowArcadeConfiguration.RouletteSlice s in config.rouletteSlices)
			total += s.probability;
		string percent = "" + ((config.rouletteSlices[sliceIdx].probability / total) * 100f).ToString("00.0") + "%";

		GUILayout.Label("Slice " + sliceIdx + " - " + percent, smallHeaderStyle);
		float box_start_y = GUILayoutUtility.GetLastRect().y - 3;
		EditorGUITools.DrawRect(new Rect(8, box_start_y, GUILayoutUtility.GetLastRect().width * 0.382f, 3), sliceColor);

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Slice Probability:", GUILayout.Width(120));
		config.rouletteSlices[sliceIdx].probability = EditorGUILayout.Slider(config.rouletteSlices[sliceIdx].probability, 0f, 1f);
		EditorGUILayout.EndHorizontal();

		// Draw items
		EditorGUILayout.BeginHorizontal();
		int itemIdx = 0;
		foreach(RouletteItem item in config.rouletteSlices[sliceIdx].possibleItems)
		{
			if(item == null)
				rouletteAction = new PostGUIAction() { action = "kill." + sliceIdx, target = item };

			drawRouletteItem(sliceIdx, itemIdx, item);
			itemIdx ++;
		}
		drawRouletteItem(sliceIdx, itemIdx, null);
		EditorGUILayout.EndHorizontal();
		EditorGUITools.DrawRect(new Rect(8, GUILayoutUtility.GetLastRect().y + GUILayoutUtility.GetLastRect().height + 1, GUILayoutUtility.GetLastRect().width, 1), sliceColor);
		EditorGUITools.DrawRect(new Rect(8, box_start_y, 1, (GUILayoutUtility.GetLastRect().y + GUILayoutUtility.GetLastRect().height) - box_start_y + 2f), sliceColor);

		EditorGUILayout.EndVertical();
	}

	private void drawRouletteItem(int sliceIdx, int itemIdx, RouletteItem item)
	{
		EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(120));
		
		if(item == null)	// Draw 'add' box
		{
			string ctrlName = "newRouletteItem." + sliceIdx;

			EditorGUILayout.LabelField("Add Item: ", GUILayout.Width(120));
			GUI.SetNextControlName(ctrlName);
			RouletteItem newItem = (RouletteItem) EditorGUILayout.ObjectField(null, typeof(RouletteItem), false, GUILayout.Width(120));
			if(rouletteItemJustAdded)
			{
				rouletteItemJustAdded = false;
				GUI.FocusControl(ctrlName);
			}
			if(newItem != null)
			{
				rouletteAction = new PostGUIAction() { action = "add." + sliceIdx, target = newItem };
				rouletteItemJustAdded = true;
			}
		}
		else				// Draw item
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(item.name, smallHeaderStyle);
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
			{
				rouletteAction = new PostGUIAction() { action = "kill." + sliceIdx, target = item };
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if(GUILayout.Button(item.itemTexture, GUILayout.Width(50), GUILayout.Height(50)))
			{
				EditorGUIUtility.PingObject(item);
				rouletteItemSelected = item;
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			// Probability
			EditorGUILayout.LabelField("Item Prob:", GUILayout.Width(120));
			config.rouletteSlices[sliceIdx].itemProbabilities[itemIdx] = EditorGUILayout.Slider(config.rouletteSlices[sliceIdx].itemProbabilities[itemIdx], 0f, 1f, GUILayout.Width(120));
			// config.rouletteSlices[sliceIdx].itemProbabilities[itemIdx] = Mathf.Clamp01(EditorGUILayout.FloatField("Item Prob", config.rouletteSlices[sliceIdx].itemProbabilities[itemIdx], GUILayout.Width(120)));

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			float total = 0f;
			foreach(float val in config.rouletteSlices[sliceIdx].itemProbabilities)
				total += val;
			GUILayout.Label("" + ((config.rouletteSlices[sliceIdx].itemProbabilities[itemIdx] / total) * 100f).ToString("00.0") + "%", smallHeaderStyle);
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
		
		EditorGUILayout.EndVertical();
	}

	private void drawMainExtensions()
	{
		// --- Screen extensions ---
		GUILayout.Label("Screen extensions", smallHeaderStyle);
		EditorGUILayout.PropertyField(serializedConfig.FindProperty("replacementScreens"), true);
		EditorGUILayout.PropertyField(serializedConfig.FindProperty("replacementPopups"), true);
		EditorGUILayout.PropertyField(serializedConfig.FindProperty("screenPopupAttachments"), true);
		EditorGUILayout.PropertyField(serializedConfig.FindProperty("extraPopups"), true);
	}

	private void drawMainFonts()
	{
		// --- Fonts override ---
		if(GUILayout.Button("Clear & Populate Labels", GUILayout.Width(200)))
		{
			List<UILabel> allLabels = getAllLabelsInScene(Application.dataPath + "/ArtikFlowArcade/_Scenes/ArtikFlowArcade.unity");

			config.fontOverrideProps = new ArtikFlowArcadeConfiguration.FontOverrideProperties[allLabels.Count];
			int i = 0;
			foreach(UILabel label in allLabels)
			{
				string path = getGameObjectPath(label.gameObject);
				config.fontOverrideProps[i].path = path;
				config.fontOverrideProps[i].newFont = label.bitmapFont;
				config.fontOverrideProps[i].newColor = label.color;
				config.fontOverrideProps[i].newEffect = label.effectStyle;
				config.fontOverrideProps[i].newFontSize = label.fontSize;
				config.fontOverrideProps[i].newSpacingX = label.spacingX;

				i ++;
			}
		}

		if(GUILayout.Button("Apply Changes", GUILayout.Width(200)))
		{
			Scene scene = EditorSceneManager.OpenScene(Application.dataPath + "/ArtikFlowArcade/_Scenes/ArtikFlowArcade.unity", OpenSceneMode.Additive);

			int i = 0;
			foreach(ArtikFlowArcadeConfiguration.FontOverrideProperties p in config.fontOverrideProps)
			{
				try
				{
					string[] path = p.path.Split('.');
					Transform t = null;
					foreach(GameObject g in scene.GetRootGameObjects())
					{
						if(g.name == path[0])
							t = g.transform;
					}
					bool first = false;

					foreach(string s in path)
					{
						if(!first)
							first = true;
						else
							t = t.Find(s);							
					}

					UILabel label = t.GetComponent<UILabel>();
					
					if(label.bitmapFont != p.newFont)
					{
						config.fontOverrideProps[i].overrideFont = true;
						config.fontOverrideProps[i].newFont = label.bitmapFont;
					}

					if(label.color != p.newColor)
					{
						config.fontOverrideProps[i].overrideColor = true;
						config.fontOverrideProps[i].newColor = label.color;
					}

					if(label.effectStyle != p.newEffect)
					{
						config.fontOverrideProps[i].overrideEffect = true;
						config.fontOverrideProps[i].newEffect = label.effectStyle;
					}

					if(label.fontSize != p.newFontSize)
					{
						config.fontOverrideProps[i].overrideFontSize = true;
						config.fontOverrideProps[i].newFontSize = label.fontSize;
					}

					if(label.spacingX != p.newSpacingX)
					{
						config.fontOverrideProps[i].overrideSpacingX = true;
						config.fontOverrideProps[i].newSpacingX = label.spacingX;
					}
				}
				catch (System.Exception e)
				{
					Debug.LogWarning("[ERROR] Error processing font override for path: '" + p.path + "'\nException:\n" + e.ToString());
				}

				i ++;
			}
		}

		GUILayout.Label("Font overrides", smallHeaderStyle);
		EditorGUILayout.PropertyField(serializedConfig.FindProperty("fontOverrideProps"), true);
	}

	string getGameObjectPath(GameObject g)
	{
		Transform t = g.transform;
		string path = "";
		while(t != null)
		{
			path = t.name + "." + path;
			t = t.parent;
		}

		return path.TrimEnd('.');
	}

	List<UILabel> getAllLabelsInScene(string scenePath)
	{
		Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
		List<UILabel> allLabels = new List<UILabel>();

		foreach(GameObject g in scene.GetRootGameObjects())
		{
			foreach(UILabel label in g.GetComponentsInChildren(typeof(UILabel), true))
				allLabels.Add(label);
		}

		return allLabels;
	}

	private void drawGameServices()
	{
		gsShowPlayphone = EditorGUILayout.Toggle("Show Playphone?", gsShowPlayphone);

		// Leaderboards
		GUILayout.Label("Leaderboards", smallHeaderStyle);
		
		for(int row = 0; row < config.GoogleLeaderboards.Length + 1; row ++)
		{
			EditorGUILayout.BeginHorizontal();

			if (row == 0)
			{
				GUILayout.Label("ID", GUILayout.Width(25));
				GUILayout.Label("Android", GUILayout.Width(200));
				GUILayout.Label("iOS", GUILayout.Width(200));
				if (gsShowPlayphone)
					GUILayout.Label("Playphone", GUILayout.Width(200));
			}
			else
			{
				int i = row - 1;
				GUILayout.Label("" + i, GUILayout.Width(25));
				config.GoogleLeaderboards[i] = EditorGUILayout.TextField(config.GoogleLeaderboards[i], GUILayout.Width(200));
				config.iOSLeaderboards[i] = EditorGUILayout.TextField(config.iOSLeaderboards[i], GUILayout.Width(200));
				if (gsShowPlayphone)
					config.playphoneLeaderboards[i] = EditorGUILayout.TextField(config.playphoneLeaderboards[i], GUILayout.Width(200));
			}

			EditorGUILayout.EndHorizontal();
		}

		// Leaderboards, add and delete
		EditorGUILayout.BeginHorizontal();

		int newsize = -1;
		if(GUILayout.Button("+", GUILayout.Width(20)))
			newsize = config.GoogleLeaderboards.Length + 1;
		if (config.GoogleLeaderboards.Length > 0 && GUILayout.Button("-", GUILayout.Width(20)))
			newsize = config.GoogleLeaderboards.Length - 1;

		if(newsize != -1)
		{
			Array.Resize(ref config.GoogleLeaderboards, newsize);
			Array.Resize(ref config.iOSLeaderboards, newsize);
			Array.Resize(ref config.playphoneLeaderboards, newsize);
		}
			
		EditorGUILayout.EndHorizontal();

		// Achievements
		GUILayout.Label("Achievements", smallHeaderStyle);

		for (int row = 0; row < config.GoogleAchievements.Length + 1; row++)
		{
			EditorGUILayout.BeginHorizontal();

			if (row == 0)
			{
				GUILayout.Label("ID", GUILayout.Width(25));
				GUILayout.Label("Android", GUILayout.Width(200));
				GUILayout.Label("iOS", GUILayout.Width(200));
				if (gsShowPlayphone)
					GUILayout.Label("Playphone", GUILayout.Width(200));
			}
			else
			{
				int i = row - 1;
				GUILayout.Label("" + i, GUILayout.Width(25));
				config.GoogleAchievements[i] = EditorGUILayout.TextField(config.GoogleAchievements[i], GUILayout.Width(200));
				config.iOSAchievements[i] = EditorGUILayout.TextField(config.iOSAchievements[i], GUILayout.Width(200));
				if (gsShowPlayphone)
					config.playphoneAchievements[i] = EditorGUILayout.TextField(config.playphoneAchievements[i], GUILayout.Width(200));
			}

			EditorGUILayout.EndHorizontal();
		}

		// Achievements, add and delete
		EditorGUILayout.BeginHorizontal();

		newsize = -1;
		if (GUILayout.Button("+", GUILayout.Width(20)))
			newsize = config.GoogleAchievements.Length + 1;
		if (config.GoogleAchievements.Length > 0 && GUILayout.Button("-", GUILayout.Width(20)))
			newsize = config.GoogleAchievements.Length - 1;

		if (newsize != -1)
		{
			Array.Resize(ref config.GoogleAchievements, newsize);
			Array.Resize(ref config.iOSAchievements, newsize);
			Array.Resize(ref config.playphoneAchievements, newsize);
		}

		EditorGUILayout.EndHorizontal();
	}

	private void drawCharacters()
	{
		drawError(getCharactersError());

		// Tier scrolls
		config.characterTier2Start = EditorGUILayout.IntSlider("Tier2 Start Price", config.characterTier2Start, 1, 1500);
		config.characterTier3Start = EditorGUILayout.IntSlider("Tier3 Start Price", config.characterTier3Start, config.characterTier2Start + 1, 1501);

		charsScrollPos = EditorGUILayout.BeginScrollView(charsScrollPos);
		EditorGUILayout.BeginVertical();

		// Existing chars
		for (int i = 0; i < config.characters.Length; i ++)
			drawChar(i, config.characters[i]);

		// Add char
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Add Character: ", GUILayout.Width(150));
		GUI.SetNextControlName("newCharField");
		Character newChar = (Character) EditorGUILayout.ObjectField(null, typeof(Character), false, GUILayout.Width(280));
		EditorGUILayout.EndHorizontal();
		if (charJustAdded)
		{
			charJustAdded = false;
			GUI.FocusControl("newCharField");
		}
		if (newChar != null)
		{
			charAction = new PostGUIAction() { action = "add", target = newChar };
			charJustAdded = true;
		}
		
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndScrollView();
		GUI.backgroundColor = Color.white;

		// Bottom box
		if(charSelected != null)
		{
			if (charEditor == null || charEditor.target != charSelected)
				charEditor = Editor.CreateEditor(charSelected);

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

			charEditor.OnInspectorGUI();

			EditorGUILayout.EndVertical();
        }
    }

	private void drawChar(int id, Character c)
	{
		GUI.backgroundColor = Color.grey;
		EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Width(230), GUILayout.Height(100));
		EditorGUILayout.BeginVertical(GUILayout.Width(100), GUILayout.Height(100));

		GUI.backgroundColor = (c.price >= config.characterTier3Start) ? new Color32(226, 16, 30, 255) : (c.price >= config.characterTier2Start) ? new Color32(189, 13, 169, 255) : new Color32(112, 238, 248, 255);

		if(GUILayout.Button(c.menuTexture, GUILayout.Width(85), GUILayout.Height(85)))
		{
			EditorGUIUtility.PingObject(c);
			charSelected = c;
        }

		GUI.backgroundColor = Color.white;

		EditorGUILayout.EndVertical();
		EditorGUILayout.BeginVertical(GUILayout.Width(75), GUILayout.Height(100));

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("ID " + id, smallHeaderStyle, GUILayout.Width(232));
		if (id > 0)
		{
			if (GUILayout.Button("▲", GUILayout.Width(20)))
				charAction = new PostGUIAction() { action = "up", target = c };
		}
		else
			GUILayout.Space(25);

		if (id < config.characters.Length - 1)
		{
			if (GUILayout.Button("▼", GUILayout.Width(20)))
				charAction = new PostGUIAction() { action = "down", target = c };
		}
		else
			GUILayout.Space(25);

		if (GUILayout.Button("X", GUILayout.Width(20)))
			charAction = new PostGUIAction() { action = "kill", target = c };
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Internal Name:", GUILayout.Width(150));
		c.internalName = EditorGUILayout.TextField(c.internalName, GUILayout.Width(150));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Display Name:", GUILayout.Width(150));
		c.displayName = EditorGUILayout.TextField(c.displayName, GUILayout.Width(150));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Price:", GUILayout.Width(150));
		c.price = EditorGUILayout.IntField(c.price, GUILayout.Width(70));
		EditorGUILayout.LabelField(getSpecialPriceString(c.price), GUILayout.Width(80));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Unlock Threshold:", GUILayout.Width(150));
		c.unlockThreshold = EditorGUILayout.IntField(c.unlockThreshold, GUILayout.Width(70));
		EditorGUILayout.LabelField(getSpecialPriceString(c.price), GUILayout.Width(80));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Menu Texture:", GUILayout.Width(150));
		c.menuTexture = EditorGUILayout.ObjectField(c.menuTexture, typeof(Texture2D), false, GUILayout.Width(150)) as Texture2D;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Holiday Character?", GUILayout.Width(150));
		bool activated = EditorGUILayout.Toggle(isHolidayCharacter(c), GUILayout.Width(150));
		EditorGUILayout.EndHorizontal();

		if(activated && !isHolidayCharacter(c))
		{
			// Turned on!
			c.promoStartDay = 1;
			c.promoStartMonth = 1;
			c.promoDaysLength = 1;
		}
		else if(!activated && isHolidayCharacter(c))
		{
			// Turned off
			c.promoStartDay = 0;
			c.promoStartMonth = 0;
			c.promoDaysLength = 0;
		}

		// Holiday
		if(isHolidayCharacter(c))
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Holiday Start dd/mm:", GUILayout.Width(150));
			c.promoStartDay = EditorGUILayout.IntField(c.promoStartDay, GUILayout.Width(25));
			EditorGUILayout.LabelField("/", GUILayout.Width(10));
			c.promoStartMonth = EditorGUILayout.IntField(c.promoStartMonth, GUILayout.Width(25));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Holiday Days Length:", GUILayout.Width(150));
			c.promoDaysLength = EditorGUILayout.IntField(c.promoDaysLength, GUILayout.Width(40));
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();

		if (GUI.changed)
			EditorUtility.SetDirty(c);
	}

	private void drawPowerups()
	{
		drawError(getPowerupsError());

		powerScrollPos = EditorGUILayout.BeginScrollView(powerScrollPos);
		EditorGUILayout.BeginVertical();

		// Existing powerups
		for (int i = 0; i < config.powerups.Length; i++)
			drawPowerup(i, config.powerups[i]);

		// Add char
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Add Powerup: ", GUILayout.Width(150));
		GUI.SetNextControlName("newPowerField");
		Powerup newPower = (Powerup)EditorGUILayout.ObjectField(null, typeof(Powerup), false, GUILayout.Width(280));
		EditorGUILayout.EndHorizontal();
		if (powerJustAdded)
		{
			powerJustAdded = false;
			GUI.FocusControl("newPowerField");
		}
		if (newPower != null)
		{
			powerAction = new PostGUIAction() { action = "add", target = newPower };
			powerJustAdded = true;
		}

		EditorGUILayout.EndVertical();
		EditorGUILayout.EndScrollView();
		GUI.backgroundColor = Color.white;

		// Bottom box
		if (powerSelected != null)
		{
			if (powerEditor == null || powerEditor.target != powerSelected)
				powerEditor = Editor.CreateEditor(powerSelected);

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

			powerEditor.OnInspectorGUI();

			EditorGUILayout.EndVertical();
		}
	}

	private void drawPowerup(int id, Powerup p)
	{
		GUI.backgroundColor = Color.grey;
		EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Width(230), GUILayout.Height(100));
		EditorGUILayout.BeginVertical(GUILayout.Width(100), GUILayout.Height(100));

		if (GUILayout.Button(p.texture, GUILayout.Width(85), GUILayout.Height(85)))
		{
			EditorGUIUtility.PingObject(p);
			powerSelected = p;
		}

		GUI.backgroundColor = Color.white;

		EditorGUILayout.EndVertical();
		EditorGUILayout.BeginVertical(GUILayout.Width(75), GUILayout.Height(100));

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("ID " + id, smallHeaderStyle, GUILayout.Width(232));
		if (id > 0)
		{
			if (GUILayout.Button("▲", GUILayout.Width(20)))
				powerAction = new PostGUIAction() { action = "up", target = p };
		}
		else
			GUILayout.Space(25);

		if (id < config.powerups.Length - 1)
		{
			if (GUILayout.Button("▼", GUILayout.Width(20)))
				powerAction = new PostGUIAction() { action = "down", target = p };
		}
		else
			GUILayout.Space(25);

		if (GUILayout.Button("X", GUILayout.Width(20)))
			powerAction = new PostGUIAction() { action = "kill", target = p };
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Internal ID:", GUILayout.Width(150));
		p.internalId = EditorGUILayout.TextField(p.internalId, GUILayout.Width(150));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Name (Localization Tag):", GUILayout.Width(150));
		p.titleTag = EditorGUILayout.TextField(p.titleTag, GUILayout.Width(150));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Desc (Localization Tag):", GUILayout.Width(150));
		p.descriptionTag = EditorGUILayout.TextField(p.descriptionTag, GUILayout.Width(150));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Texture:", GUILayout.Width(150));
		p.texture = EditorGUILayout.ObjectField(p.texture, typeof(Texture2D), false, GUILayout.Width(150)) as Texture2D;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.LabelField("Prices (set to 0 to deactivate option):");

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("- Give", GUILayout.Width(45));
		p.option1Count = EditorGUILayout.IntField(p.option1Count, GUILayout.Width(25));
		EditorGUILayout.LabelField(" for ", GUILayout.Width(30));
		p.option1Price = EditorGUILayout.IntField(p.option1Price, GUILayout.Width(25));
		EditorGUILayout.LabelField("coins", GUILayout.Width(45));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("- Give", GUILayout.Width(45));
		p.option2Count = EditorGUILayout.IntField(p.option2Count, GUILayout.Width(25));
		EditorGUILayout.LabelField(" for ", GUILayout.Width(30));
		p.option2Price = EditorGUILayout.IntField(p.option2Price, GUILayout.Width(25));
		EditorGUILayout.LabelField("coins", GUILayout.Width(45));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("- Give", GUILayout.Width(45));
		p.videoCount = EditorGUILayout.IntField(p.videoCount, GUILayout.Width(25));
		EditorGUILayout.LabelField(" for video", GUILayout.Width(70));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
		
		if (GUI.changed)
			EditorUtility.SetDirty(p);
	}

	private void drawIAPs()
	{
		drawError(getIAPsError());

		// GemPack
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Sell Gem Pack?", GUILayout.Width(200));
		bool activated = EditorGUILayout.Toggle(isGemPackEnabled(), GUILayout.Width(50));
		if (activated && !isGemPackEnabled())       // Turned on!
			config.gemPackCount = 1000;
		else if (!activated && isGemPackEnabled())  // Turned off!
			config.gemPackCount = 0;
		if(isGemPackEnabled())
		{
			EditorGUILayout.LabelField("Count: ", GUILayout.Width(100));
			config.gemPackCount = EditorGUILayout.IntField(config.gemPackCount);
		}

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField ("StarterPack Gems: ",GUILayout.Width(150));
		config.gemStarterPackCount = EditorGUILayout.IntField (config.gemStarterPackCount);
		EditorGUILayout.LabelField ("SuperPack Gems: ",GUILayout.Width(150));
		config.gemSuperPackCount = EditorGUILayout.IntField (config.gemSuperPackCount);
		EditorGUILayout.EndHorizontal();

		// Holiday pack
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Sell Holiday half-price pack?", GUILayout.Width(200));
		activated = EditorGUILayout.Toggle(isHolidayPackActive(), GUILayout.Width(50));
		if (activated && !isHolidayPackActive())       // Turned on!
		{
			config.IAPPromoStartDay = 1;
			config.IAPPromoStartMonth = 1;
			config.IAPPromoDaysLength = 3;
		}
		else if (!activated && isHolidayPackActive())  // Turned off!
		{
			config.IAPPromoStartDay = 0;
			config.IAPPromoStartMonth = 0;
			config.IAPPromoDaysLength = 0;
		}
		if (isHolidayPackActive())
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Start dd/mm:", GUILayout.Width(100));
			config.IAPPromoStartDay = EditorGUILayout.IntField(config.IAPPromoStartDay, GUILayout.Width(25));
			EditorGUILayout.LabelField("/", GUILayout.Width(10));
			config.IAPPromoStartMonth = EditorGUILayout.IntField(config.IAPPromoStartMonth, GUILayout.Width(25));
			EditorGUILayout.LabelField("Days Length:", GUILayout.Width(80));
			config.IAPPromoDaysLength = EditorGUILayout.IntField(config.IAPPromoDaysLength, GUILayout.Width(40));
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndHorizontal();

		IAPScrollPos = EditorGUILayout.BeginScrollView(IAPScrollPos);
		EditorGUILayout.BeginVertical();

		// Existing powerups
		for (int i = 0; i < config.products.Length; i++)
			drawIAP(i, config.products[i]);

		// Add iap
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Add IAP: ", GUILayout.Width(150));
		GUI.SetNextControlName("newIAPField");
		ArtikProduct newIAP = (ArtikProduct)EditorGUILayout.ObjectField(null, typeof(ArtikProduct), false, GUILayout.Width(280));
		EditorGUILayout.EndHorizontal();
		if (IAPJustAdded)
		{
			IAPJustAdded = false;
			GUI.FocusControl("newIAPField");
		}
		if (newIAP != null)
		{
			IAPAction = new PostGUIAction() { action = "add", target = newIAP };
			IAPJustAdded = true;
		}

		EditorGUILayout.EndVertical();
		EditorGUILayout.EndScrollView();
		GUI.backgroundColor = Color.white;

		// Bottom box
		if (IAPSelected != null)
		{
			if (IAPEditor == null || IAPEditor.target != IAPSelected)
				IAPEditor = Editor.CreateEditor(IAPSelected);

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

			IAPEditor.OnInspectorGUI();

			EditorGUILayout.EndVertical();
		}
	}

	private void drawIAP(int id, ArtikProduct iap)
	{
		GUI.backgroundColor = Color.grey;
		EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Width(230), GUILayout.Height(100));
		EditorGUILayout.BeginVertical(GUILayout.Width(100), GUILayout.Height(100));

		if (GUILayout.Button(config.icon, GUILayout.Width(85), GUILayout.Height(85)))
		{
			EditorGUIUtility.PingObject(iap);
			IAPSelected = iap;
		}

		GUI.backgroundColor = Color.white;

		EditorGUILayout.EndVertical();
		EditorGUILayout.BeginVertical(GUILayout.Width(75), GUILayout.Height(100));

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(iap.productID + " (" + id + ")", smallHeaderStyle, GUILayout.Width(232));
		if (id > 0)
		{
			if (GUILayout.Button("▲", GUILayout.Width(20)))
				IAPAction = new PostGUIAction() { action = "up", target = iap };
		}
		else
			GUILayout.Space(25);

		if (id < config.products.Length - 1)
		{
			if (GUILayout.Button("▼", GUILayout.Width(20)))
				IAPAction = new PostGUIAction() { action = "down", target = iap };
		}
		else
			GUILayout.Space(25);

		if (GUILayout.Button("X", GUILayout.Width(20)))
			IAPAction = new PostGUIAction() { action = "kill", target = iap };
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Product ID:", GUILayout.Width(150));
		iap.productID = EditorGUILayout.TextField(iap.productID, GUILayout.Width(150));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Type:", GUILayout.Width(150));
		iap.type = (UnityEngine.Purchasing.ProductType) EditorGUILayout.EnumPopup(iap.type, GUILayout.Width(150));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Google ID:", GUILayout.Width(150));
		iap.googleID = EditorGUILayout.TextField(iap.googleID, GUILayout.Width(150));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Apple ID:", GUILayout.Width(150));
		iap.appleID = EditorGUILayout.TextField(iap.appleID, GUILayout.Width(150));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Playphone ID:", GUILayout.Width(150));
		iap.playphoneID = EditorGUILayout.TextField(iap.playphoneID, GUILayout.Width(150));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();

		if (GUI.changed)
			EditorUtility.SetDirty(iap);
	}

	private void drawDebug()
	{
		config.moneyCheat = EditorGUILayout.IntField("Start Money", config.moneyCheat);
		config.forceLanguage = EditorGUILayout.TextField("Force Language", config.forceLanguage);
	}

	// --------------------------------------------------

	private bool isHolidayCharacter(Character c)
	{
		return c.promoStartDay != 0 || c.promoStartMonth != 0 || c.promoDaysLength != 0;
	}
	
	private bool isDebugActive()
	{
		if(config.moneyCheat != 0)
			return true;

		if (config.forceLanguage != "")
			return true;

		return false;
	}

	private bool isGemPackEnabled()
	{
		return config.gemPackCount > 0;
	}

	private bool isHolidayPackActive()
	{
		return config.IAPPromoStartDay > 0 || config.IAPPromoStartMonth > 0 || config.IAPPromoDaysLength > 0;
	}

	private string getMainError()
	{
		foreach(GameScreen s in config.replacementScreens)
		{
			if(s == null)
				return "There is an empty replacementScreen set.";
		}

		foreach(Popup p in config.replacementPopups)
		{
			if(p == null)
				return "There is an empty replacementPopup set.";
		}

		int i = 0;
		foreach(ArtikFlowArcadeConfiguration.ScreenPopupAttachment a in config.screenPopupAttachments)
		{
			if(System.Type.GetType(a.originalInterface + ",Assembly-CSharp") == null)
				return ("Invalid class name in screenPopupsAttachments[" + i + "].originalInterface");

			if(System.Type.GetType(a.scriptToAttach + ",Assembly-CSharp") == null)
				return ("Invalid class name in screenPopupsAttachments[" + i + "].scriptToAttach");

			i ++;
		}

		i = 0;
		foreach(ArtikFlowArcadeConfiguration.RouletteSlice s in config.rouletteSlices)
		{
			if(s.possibleItems.Length == 0)
				return "No items have been set in roulette slice " + i;
			
			i ++;
		}

		return null;
	}

	private string getIAPsError()
	{
		Dictionary<string, ArtikProduct> products = new Dictionary<string, ArtikProduct>();
		
		for (int i = 0; i < config.products.Length; i++)
		{
			ArtikProduct iap = config.products[i];
			if (isExampleAsset(iap))
				return "IAP " + i + " (" + iap.productID + ") is an example product.";
				
			products.Add(iap.productID, iap);
		}

		if (!products.ContainsKey("noads"))
			return "IAP with ID 'noads' is missing.";
		if (!products.ContainsKey("duplicate"))
			return "IAP with ID 'duplicate' is missing.";
		if (!products.ContainsKey("unlockall"))
			return "IAP with ID 'unlockall' is missing.";
		if (!products.ContainsKey("pack"))
			return "IAP with ID 'pack' is missing.";
		if(!products.ContainsKey("packhalf"))
			return "IAP with ID 'packhalf' is missing. This is used as replacement for the 'pack' IAP during promotion conditions.";

		if(isGemPackEnabled() && !products.ContainsKey("gems"))
			return "IAP with ID 'gems' is missing. This is the gem pack.";

		if (products["noads"].type != UnityEngine.Purchasing.ProductType.NonConsumable)
			return "IAP 'noads' must be a Non-Consumable.";
		if (products["duplicate"].type != UnityEngine.Purchasing.ProductType.NonConsumable)
			return "IAP 'duplicate' must be a Non-Consumable.";
		if (products["unlockall"].type != UnityEngine.Purchasing.ProductType.NonConsumable)
			return "IAP 'unlockall' must be a Non-Consumable.";
		if (products["pack"].type != UnityEngine.Purchasing.ProductType.NonConsumable)
			return "IAP 'pack' must be a Non-Consumable.";

		if (products.ContainsKey("gems") && products["gems"].type != UnityEngine.Purchasing.ProductType.Consumable)
			return "IAP 'gems' must be a Consumable.";
		if (products.ContainsKey("packhalf") && products["packhalf"].type != UnityEngine.Purchasing.ProductType.NonConsumable)
			return "IAP 'packhalf' must be a Non-Consumable.";

		return null;
	}

	private string getCharactersError()
	{
		for(int i = 0; i < config.characters.Length; i ++)
		{
			Character c = config.characters[i];
			if (isExampleAsset(c))
				return "Character " + i + " (" + c.internalName + ") is an example character.";
		}

		return null;
	}

	private string getPowerupsError()
	{
		for (int i = 0; i < config.powerups.Length; i++)
		{
			Powerup power = config.powerups[i];
			if (isExampleAsset(power))
				return "Powerup " + i + " (" + power.internalId + ") is an example powerup.";
		}

		return null;
	}

	private bool isExampleAsset(UnityEngine.Object asset)
	{
		return (AssetDatabase.GetAssetPath(asset).StartsWith("Assets/ArtikFlow/Examples"));
	}

	private string getSpecialPriceString(int price)
	{
		if (price == 0)
			return "FREE";
		else if (price == -1)
			return "FACEBOOK";
		else if (price == -2)
			return "INSTAGRAM";
		else if (price == -3)
			return "TWITTER";
		else if (price <= -4)
			return "INVALID";

		return "";
    }

	// Add menu item to select configuration
	static string foundConfig = "";

	[MenuItem("ArtikFlow/Select configuration #F11")]
	public static void selectConfiguration()
	{
		if(foundConfig == "")
		{
			string[] configs = AssetDatabase.FindAssets("t:ArtikFlowArcadeConfiguration");
			foreach(string c in configs)
			{
				string path = AssetDatabase.GUIDToAssetPath(c);
				if(path.StartsWith("Assets/ArtikFlowArcade/Examples"))
					continue;

				foundConfig = path;
				break;
			}
		}

		if(foundConfig != "")
		{
			Debug.Log(foundConfig);
			Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(foundConfig);
		}
		else
			Debug.Log("Configuration file not found!");
		
	}

}

}