using UnityEngine;
using System.Collections;
using UnityEngine.Purchasing;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AFArcade {

public class ArtikFlowArcadeConfiguration : AFBase.ArtikFlowBaseConfiguration
{
	// [Header("ArtikFlow Arcade")]
	[Tooltip("Enables characters in game")]
	public bool charactersEnabled = true;
	[Tooltip("Displays the score ingame")]
	public bool displayScoreIngame = true;
	[Tooltip("Enables or disables the whole coin functionality altogether")]
	public bool enableCoins = true;
	[Tooltip("Displays the coins ingame (only works if enableCoins is activated)")]
	public bool displayCoinsIngame = true;
	[Tooltip("Enable revive (only works if enableCoins is activated)")]
	public bool enableRevive = true;
	[Tooltip("Makes black strings white in some screens")]
	public bool useWhiteLabels = false;
	[Tooltip("Game Icon for rate popup and other uses.")]
	public Texture2D icon;
	[Tooltip("Logo texture to be displayed in the screenshot and menus.")]
	public Texture2D gameLogoTexture;
	[Tooltip("Wether to enable PlayerStatsAPI or not")]
	public bool disablePlayerStatsAPI = false;
	[Tooltip("List of game mode scripts.")]
	public ArtikFlowArcadeGame[] gameModes;

	// [Header("Cheats (set to 0 for release!)")]
	[Tooltip("Starting amount of coins. Set to 0 for release!")]
	public int moneyCheat = 0;

	// [Header("Characters")]
	[Tooltip("Array of Characters, or a subclass of Character specific for the game")]
	public Character[] characters;
	[Tooltip("Amount of coins from which the Tier 2 characters start")]
	public int characterTier2Start;
	[Tooltip("Amount of coins from which the Tier 3 characters start")]
	public int characterTier3Start;

	// [Header("Powerups")]
	[Tooltip("Array of Powerups available")]
	public Powerup[] powerups;

	// [Header("Sharing")]
	[Tooltip("Text displayed during sharing dialogs. % replaced with score. * replaced with game name.")]
	public string sharingText = "I made a score of % in #*! #ArtikGames #android #iOS";

	// [Header("In-app purchase products")]
	[Tooltip("Coins given by the gem pack IAP. Set to 0 to disable. It requires a product in 'products' with id 'gems'.")]
	public int gemPackCount = 0;
	[Tooltip("Coins given by the starter pack IAP. Set to 0 to disable. It requires a product in 'products' with id 'starter'.")]
	public int gemStarterPackCount = 0;
	[Tooltip("Coins given by the super pack IAP. Set to 0 to disable. It requires a product in 'products' with id 'superpack'.")]
	public int gemSuperPackCount = 0;
	[Tooltip("If half priced pack is offered, the day/month the promotion starts. It requires a product named 'packhalf'.")]
	public int IAPPromoStartDay;
	[Tooltip("If half priced pack is offered, the day/month the promotion starts. It requires a product named 'packhalf'.")]
	public int IAPPromoStartMonth;
	[Tooltip("If half priced pack is offered, how many days the promo lasts. It requires a product named 'packhalf'.")]
	public int IAPPromoDaysLength;
	[Tooltip("If half priced pack is offered, every how many games it is shown. It requires a product named 'packhalf'.")]
	public int IAPPromoGameToShow = 4;

	// [Header("Ads")]
	[Tooltip("Turn on to show the banner at the bottom, off to show at the top")]
	public bool bannerAtBottom = true;
	[Tooltip("Every how many games an intestitial is shown")]
	public int interstitialFrequency = 5;

	// [Header("Screen extensions")]
	public GameScreen[] replacementScreens;
	public Popup[] replacementPopups;
	[Serializable]
	public struct ScreenPopupAttachment {
		public string originalInterface;
		public string scriptToAttach;
	}
	public ScreenPopupAttachment[] screenPopupAttachments;
	public Popup[] extraPopups;

	// [Header("Roulette configuration")]
	[Serializable]
	public struct RouletteSlice {
		public float probability;
		public RouletteItem[] possibleItems;
		public float[] itemProbabilities;
	}
	public int rouletteSliceCount;
	public RouletteSlice[] rouletteSlices;
	
	// [Header("Fonts override")]
	[Serializable]
	public struct FontOverrideProperties {
		public string path;

		public bool overrideFont;
		public UIFont newFont;

		public bool overrideColor;
		public Color newColor;

		public bool overrideEffect;
		public UILabel.Effect newEffect;

		public bool overrideFontSize;
		public int newFontSize;

		public bool overrideSpacingX;
		public int newSpacingX;
	}
	public FontOverrideProperties[] fontOverrideProps;

	// [Header("StartScreen")]
	[Tooltip("If the roulette should be used instead of the daily gift.")]
	public bool replaceDailyWithRoulette = false;
	[Tooltip("Amount of gems that a spin costs.")]
	public int gemsToSpin = 30;
	[Tooltip("Daily reward times in minutes. The last one loops.")]
	public int[] timeForRewards = new int[] { 3, 6, 30, 60, 360 };
	[Tooltip("Coins given with the daily reward.")]
	public int dailyPrize = 100;
	[Tooltip("Shows \"tap to play\" on the start screen")]
	public bool showTapToPlay = true;
	[Tooltip("The vertical position of the scores on the start screen")]
	public float scoresYPosition = -215f;
	[Tooltip("Disable HiddenLogo")]
	public bool hiddenLogoEnabled = true;

	// [Header("Shop")]
	[Tooltip("URL opened when the with FB Buy characters")]
	public string Facebook_buy_Url = "http://www.facebook.com/artikgames";
	[Tooltip("URL opened when the with Twitter Buy characters")]
	public string Twitter_buy_Url = "https://twitter.com/artikgames";
	[Tooltip("URL opened when the with Instagram Buy characters")]
	public string Instagram_buy_Url = "https://www.instagram.com/artik.games/";
	[Tooltip("Height of the character in the char selection and death screens.")]
	public float characterHeight = 117f;
	[Tooltip("Icon to replase the default skin shop button on the main screen.")]
	public Texture2D shopIcon;

	// [Header("GameplayScreen")]
	[Tooltip("How many coins does it cost to revive")]
	public int coinsToRevive = 20;
	[Tooltip("Is the revive close button available?")]
	public bool canCancel = true;
	[Tooltip("How many seconds the revive popup appears")]
	public int timeToRevive = 10;

	// [Header("LostScreen")]
	[Tooltip("Linked URL in the more games button for Android")]
	public string Android_MoreURL = "https://play.google.com/store/search?q=pub:Artik%20Games";
	[Tooltip("Linked URL in the more games button for Playphone")]
	public string Playphone_MoreURL = "psgn://getgames.info/?jump_to=gamesbydeveloper&id=16543";
	[Tooltip("Linked URL in the more games button for iOS")]
	public string iOS_MoreURL = "https://itunes.apple.com/us/developer/niels-benjamins/id1101859183";
	[Tooltip("How many coins to give with the rewarded video")]
	public int videoReward = 80;
	[Tooltip("Every how many games the rewarded video should be shown")]
	public int videoAdFrequency = 2;

	// [Header("Popup Rate")]
	[Tooltip("If the rate popup should be disabled")]
	public bool disableRatePopup = false;
	[Tooltip("After how many deaths to show the rate popup")]
	public int gameToShowRate = 7;

	// [Header("Audio")]
	[Tooltip("A gameObject that contains child gameObjects with an AudioSource each. These can then be triggered using Audio.instance.playName(\"clipName\")")]
	public GameObject gameAudio;

	// [Header("Notifications")]
	[Tooltip("Gift offered to the player during the first week without playing.")]
	public int notificationFirstGift = 0;
	[Tooltip("Gift offered to the player during the next month without playing.")]
	public int notificationSecondGift = 200;
	[Tooltip("Gift offered to the player after more than a month without playing.")]
	public int notificationThirdGift = 1000;

	// Backwards compatibility:
    [HideInInspector]
    public int scoreForBronze = 2147483644;
	[HideInInspector]
	public int scoreForSilver = 2147483644;
	[HideInInspector]
	public int scoreForGold = 2147483644;
	[HideInInspector]
	public int bronzeCoins = 0;
	[HideInInspector]
	public int silverCoins = 0;
	[HideInInspector]
	public int goldCoins = 0;
	[HideInInspector]
	public int noMedalVideoReward = 0;

}

}