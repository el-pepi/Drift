using UnityEngine;
using System.Collections;

/* -------------------------------------------------------------
*	UI/StartScreen script.
*	Zamaroht | January, 2016
*
*	Manages the StartScreen.
------------------------------------------------------------- */

namespace AFArcade {

public class StartScreen : IStartScreen
{
	UIButton recordButton;
	UIButton powerupsButton;
	DailyButton dailyButton;
	DailyButton secondaryDailyButton;
	StarterPackButton starterPackButton;

	UILabel bestLabel;
	UILabel playsLabel;

	protected override void Awake()
	{
		base.Awake();

		recordButton = transform.Find("Button_Rec").GetComponent<UIButton>();
		recordButton.gameObject.SetActive(false);

		powerupsButton = transform.Find("Powerups").Find("Button_Powerups").GetComponent<UIButton>();
		powerupsButton.gameObject.SetActive( ArtikFlowArcade.instance.configuration.powerups.Length > 0 );

		dailyButton = transform.Find("Daily").GetComponentInChildren<DailyButton>();
		secondaryDailyButton = transform.Find("Daily_Secondary").GetComponentInChildren<DailyButton>();
		starterPackButton = transform.Find("StarterPack").GetComponentInChildren<StarterPackButton>();
		bestLabel = transform.Find("Scores").Find("Label_Best").GetComponent<UILabel>();
		playsLabel = transform.Find("Scores").Find("Label_Plays").GetComponent<UILabel>();

		bestLabel.transform.parent.localPosition = Vector3.up * ArtikFlowArcade.instance.configuration.scoresYPosition;
		
		if(ArtikFlowArcade.instance.configuration.showTapToPlay) {
			transform.Find("PlayHint").Find("Label_Tap").GetComponent<UILabel>().text = Language.get("StartScreen.TapToPlay");
		} else {
			transform.Find("PlayHint").gameObject.SetActive(false);
		}
	}

	protected override void Start()
	{
		base.Start();

		AFBase.Replay.instance.eventRecordingAvailable.AddListener(onRecordingAvailable);
		AFBase.Replay.instance.eventRecordingUnavailable.AddListener(onRecordingUnavailable);
		AFBase.Replay.instance.eventStartedRecording.AddListener(onStartedRecording);
		AFBase.Replay.instance.eventStoppedRecording.AddListener(onStoppedRecording);
		AFBase.Replay.instance.eventTryingToRecord.AddListener(onTryingToRecord);
		
		PopupManager.instance.eventPopupShow.AddListener(onPopupShow);
		PopupManager.instance.eventPopupHide.AddListener(onPopupHide);

		SaveGameSystem.instance.eventGameModeChanged.AddListener(onGameModeChanged);

		UITexture logoTexture = transform.Find("Center").Find("Logo").GetComponent<UITexture>();
		float original_width = logoTexture.width;
		logoTexture.mainTexture = ArtikFlowArcade.instance.configuration.gameLogoTexture;
		logoTexture.MakePixelPerfect();
		logoTexture.width = (int) original_width;
		float factor = original_width / (ArtikFlowArcade.instance.configuration.gameLogoTexture.width);
		logoTexture.height = (int) (ArtikFlowArcade.instance.configuration.gameLogoTexture.height * factor);

		if (ArtikFlowArcade.instance.configuration.useWhiteLabels)
		{
			bestLabel.color = Color.white;
			playsLabel.color = Color.white;
			transform.Find("PlayHint").Find("Label_Tap").GetComponent<UILabel>().color = Color.white;
			transform.Find("Daily").Find("Label_Daily").GetComponent<UILabel>().color = Color.white;
		}

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
			transform.Find("Scores").GetComponent<TweenAlpha>().enabled = true;
			transform.Find("Scores").GetComponent<TweenAlpha>().ResetToBeginning();

			starterPackButton.GetComponent<TweenPosition>().enabled = true;
			starterPackButton.GetComponent<TweenPosition>().ResetToBeginning();
			starterPackButton.transform.parent.Find("Label_Starter").GetComponent<TweenPosition>().enabled = true;
			starterPackButton.transform.parent.Find("Label_Starter").GetComponent<TweenPosition>().ResetToBeginning();

			secondaryDailyButton.GetComponent<TweenPosition>().enabled = true;
			secondaryDailyButton.GetComponent<TweenPosition>().ResetToBeginning();
			secondaryDailyButton.transform.parent.Find("Label_Daily").GetComponent<TweenPosition>().enabled = true;
			secondaryDailyButton.transform.parent.Find("Label_Daily").GetComponent<TweenPosition>().ResetToBeginning();

			dailyButton.GetComponent<TweenPosition>().enabled = true;
			dailyButton.GetComponent<TweenPosition>().ResetToBeginning();
			dailyButton.transform.parent.Find("Label_Daily").GetComponent<TweenPosition>().enabled = true;
			dailyButton.transform.parent.Find("Label_Daily").GetComponent<TweenPosition>().ResetToBeginning();

			powerupsButton.GetComponent<UITweener>().duration = 0.5f;
			powerupsButton.GetComponent<TweenPosition>().enabled = true;
			powerupsButton.GetComponent<TweenPosition>().ResetToBeginning();
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
		DailyButton currDaily = (Arcade_Purchaser.instance.isDuringStarterPack() ? secondaryDailyButton : dailyButton);

		if (SaveGameSystem.instance.getGamesPlayed() == 0)
			currDaily.setState(DailyButton.State.INVISIBLE);
		else if (oldState == ArtikFlowArcade.State.DAILY_SCREEN || oldState == ArtikFlowArcade.State.ROULETTE_SCREEN || currDaily.state == DailyButton.State.INVISIBLE)     // Comming back from a daily reward, or it was invisible
			currDaily.setState(DailyButton.State.UNAVAILABLE);

		if(Arcade_Purchaser.instance.isDuringStarterPack())
			starterPackButton.setState(DailyButton.State.AVAILABLE);
		else
			starterPackButton.setState(DailyButton.State.INVISIBLE);

		if(Arcade_Purchaser.instance.isDuringStarterPack())
		{
			dailyButton.transform.parent.gameObject.SetActive(false);
			secondaryDailyButton.transform.parent.gameObject.SetActive(true);
			starterPackButton.transform.parent.gameObject.SetActive(true);
		}
		else
		{
			dailyButton.transform.parent.gameObject.SetActive(true);
			secondaryDailyButton.transform.parent.gameObject.SetActive(false);
			starterPackButton.transform.parent.gameObject.SetActive(false);
		}

		updatePowerupsGlow();

		updateScores();
		gameObject.SetActive(true);
	}

	void updateScores()
	{
		bestLabel.text = Language.get("StartScreen.BestScore") + ": " + SaveGameSystem.instance.getHighScore();
		playsLabel.text = Language.get("StartScreen.TimesPlayed") + ": " + SaveGameSystem.instance.getGamesPlayed();
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
		powerupsButton.transform.Find("Notif").gameObject.SetActive(incentive_powerups);
	}

	// --- Callbacks ---

	public void onPlay()
	{
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

	public void onRec()
	{
		AFBase.Replay.instance.startRecording();
	}

	void onRecordingAvailable()
	{
		recordButton.gameObject.SetActive(true);
		recordButton.isEnabled = true;
		// to-do Show feedback
		// recordButton.GetComponentInChildren<UILabel>().text = "REC";
	}

	void onTryingToRecord()
	{
		recordButton.isEnabled = false;
		// to-do: Show feedback
		// recordButton.GetComponentInChildren<UILabel>().text = "...";
	}

	void onRecordingUnavailable()
	{
		recordButton.gameObject.SetActive(false);
	}

	void onStartedRecording()
	{
		recordButton.gameObject.SetActive(false);
	}

	void onStoppedRecording()
	{
		recordButton.gameObject.SetActive(true);
	}

	void onPopupShow(Popup popup)
	{
		if(popup.GetType() == typeof(Popup_Powerups))
		{
			powerupsButton.GetComponent<UITweener>().duration = 0.2f;
			powerupsButton.GetComponent<UITweener>().PlayReverse();
		}
	}

	void onPopupHide(Popup popup)
	{
		if (popup.GetType() == typeof(Popup_Powerups))
		{
			powerupsButton.GetComponent<UITweener>().duration = 0.5f;
			powerupsButton.GetComponent<UITweener>().PlayForward();
			updatePowerupsGlow();
		}
	}

	void onGameModeChanged(int gamemodeID){
		updateScores();
	}
}

}