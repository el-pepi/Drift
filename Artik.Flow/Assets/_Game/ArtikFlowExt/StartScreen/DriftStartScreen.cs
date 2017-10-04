using UnityEngine;
using System.Collections;

/* -------------------------------------------------------------
*	UI/StartScreen script.
*	Zamaroht | January, 2016
*
*	Manages the StartScreen.
------------------------------------------------------------- */

namespace AFArcade {

	public class DriftStartScreen : IStartScreen
	{
		StarterPackButton_Custom starterPackButton;
		RouletteButton rouletteButton;

		public GameObject noti_ShopCar;

		bool canAddCounterToOffer;
		int currentCounterOffer;
		int thresholdOffer;
		public int minThresholdToOffer;
		public int maxThresholdToOffer;

		protected override void Awake()
		{
			base.Awake();

			rouletteButton = transform.Find("RouletteIcon").GetComponentInChildren<RouletteButton>();
			starterPackButton = transform.Find("Button_Starter").GetComponentInChildren<StarterPackButton_Custom>();

			if(ArtikFlowArcade.instance.configuration.showTapToPlay) {
				transform.Find("PlayHint").Find("Label_Tap").GetComponent<UILabel>().text = Language.get("StartScreen.TapToPlay");
			} else {
				transform.Find("PlayHint").gameObject.SetActive(false);
			}

			SetTheresholdOffer ();
		}

		protected override void Start()
		{
			
			base.Start();

			PopupManager.instance.eventPopupHide.AddListener(onPopupHide);

			UITexture logoTexture = transform.Find("Center").Find("Logo").GetComponent<UITexture>();
			float original_width = logoTexture.width;
			logoTexture.mainTexture = ArtikFlowArcade.instance.configuration.gameLogoTexture;
			logoTexture.MakePixelPerfect();
			logoTexture.width = (int) original_width;
			float factor = original_width / (ArtikFlowArcade.instance.configuration.gameLogoTexture.width);
			logoTexture.height = (int) (ArtikFlowArcade.instance.configuration.gameLogoTexture.height * factor);


			gameObject.SetActive(false);
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape) && PopupManager.instance.getCurrentPopup() == null && !vg_interstitial.showing)
				Application.Quit();

			if (Input.GetKeyDown(KeyCode.Space))
				onPlay();
		}

		void OnEnable()
		{
			if (!LoadingScreen.instance.doingFade)
			{
				transform.Find("Center").GetComponent<TweenPosition>().enabled = true;
				transform.Find("Center").GetComponent<TweenPosition>().ResetToBeginning();

			}
		}

		void OnDisable()
		{
			transform.Find("PlayHint").Find("Label_Tap").GetComponent<UILabel>().alpha = 0.02f;
		}

		protected override void onHide()
		{
			gameObject.SetActive(false);
		}

		protected override void onShow(ArtikFlowArcade.State oldState)
		{
			CheckAndSpawnPowerupOffer ();
			int gamesPlayed = SaveGameSystem.instance.getGamesPlayed ();

			if (gamesPlayed == 0) 
			{
				rouletteButton.setState (RouletteButton.State.INVISIBLE);
			} else {
				rouletteButton.setState (RouletteButton.State.AVAILABLE);
				rouletteButton.CheckNotification ();
			}


			if (Arcade_Purchaser.instance.isDuringStarterPack () && gamesPlayed >= 5)
			{
				starterPackButton.setState (StarterPackButton_Custom.State.AVAILABLE);
				starterPackButton.transform.parent.gameObject.SetActive (true);

			} else
			{
				starterPackButton.setState (StarterPackButton_Custom.State.INVISIBLE);
				starterPackButton.transform.parent.gameObject.SetActive(false);
			}
			updatePowerupsGlow();
			noti_ShopCar.SetActive( CharacterManager.instance.canBuyACharacter() );
			gameObject.SetActive(true);
		}

		void updatePowerupsGlow()
		{
			bool incentive_powerups = false;
			if(ArtikFlowArcade.instance.configuration.powerups.Length > 0)
			{
				foreach(Powerup p in ArtikFlowArcade.instance.configuration.powerups)
				{
					if(SaveGameSystem.instance.getCoins() >= p.option1Price || SaveGameSystem.instance.getCoins() >= p.option1Price)
					{
						incentive_powerups = true;
						break;
					}
				}
			}

		}

		// --- Callbacks ---

		public void onPlay()
		{
			AddCounterToOffer ();
			if(!Arcade_TryNBuy.instance.canPlay())
			{
				AFBase.TryNBuy.instance.tryNBuy();
				return;
			}

			base.startGame();
		}

		public void onDaily()
		{
			if (ArtikFlowArcade.instance.configuration.replaceDailyWithRoulette == false && DailyButton.getSecondsUntilReward() > 0)
				return;

			base.openDaily();
		}

		public void onStarterPack()
		{
			PopupManager.instance.showPopup<IPopup_HalfPack>();
		}

		public void onPowerups()
		{
			PopupManager.instance.showPopup<IPopup_Powerups>();
		}

		public void onShop()
		{
			ArtikFlowArcade.instance.setState(ArtikFlowArcade.State.SHOP_SCREEN);
		}

		void AddCounterToOffer()
		{
			/*if (canAddCounterToOffer = true)
			{*/
				currentCounterOffer++;
				//canAddCounterToOffer = false;
				
			//}
		}

		void SetTheresholdOffer()
		{
			thresholdOffer = Random.Range (minThresholdToOffer,maxThresholdToOffer);
			currentCounterOffer = 0;
		}

		void CheckAndSpawnPowerupOffer()
		{
			if (currentCounterOffer >= thresholdOffer)
			{
				PopupManager.instance.showPopup<IPopup_Powerups> ();
				PopupManager.instance.getCurrentPopup ().GetComponent<Popup_DriftPowerups>().SetOnlyPowerUps();
				SetTheresholdOffer ();
			}
		}



		void onPopupHide(Popup popup)
		{
			if (popup.GetType() == typeof(Popup_Powerups))
			{
				updatePowerupsGlow();
			}
		}

	}

}