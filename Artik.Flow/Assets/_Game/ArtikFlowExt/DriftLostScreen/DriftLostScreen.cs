using UnityEngine;
using System.Collections;

/* -------------------------------------------------------------
*	UI/LostScreen script.
*	Zamaroht | January, 2016
*
*	Manages the LostScreen.
------------------------------------------------------------- */

namespace AFArcade {
	
public class DriftLostScreen : ILostScreen
{
	private string Android_MoreURL;      // ArtikFlowConfiguration
	private string Playphone_MoreURL;    // ArtikFlowConfiguration
	private string iOS_MoreURL;          // ArtikFlowConfiguration
	private int videoReward;             // ArtikFlowConfiguration

	bool shareOpened;
	bool started;

	UIButton replayButton;
	UILabel bestLabel;
	UILabel scoreLabel;
	GameObject newHighScore;

	UIButton videoAdButton;
	UIButton moreGamesButton;
	UITexture screenshotTexture;

	Animator screenshotButtonAnim;
	GameObject screenshotBackground;

	float lastScreenTouchStamp;


	protected override void Awake()
	{
		base.Awake();

		transform.Find("Scoreboard").Find("Label_Score").GetComponent<UILabel>().text = Language.get("LostScreen.Score");
		transform.Find("Scoreboard").Find("Label_Best").GetComponent<UILabel>().text = Language.get("LostScreen.BestScore");
		transform.Find("Scoreboard").Find("Label_New").GetComponent<UILabel>().text = Language.get("LostScreen.New");

		replayButton = transform.Find("Button_Replay").GetComponent<UIButton>();
		bestLabel = transform.Find("Scoreboard").Find("Label_BestNum").GetComponent<UILabel>();
		scoreLabel = transform.Find("Scoreboard").Find("Label_ScoreNum").GetComponent<UILabel>();
		newHighScore = transform.Find("Scoreboard").Find("Label_New").gameObject;
		videoAdButton = transform.Find("Button_VideoAd").GetComponent<UIButton>();
		moreGamesButton = transform.Find("Button_MoreGames").GetComponent<UIButton>();
		screenshotTexture = transform.Find("Button_Screenshot").GetComponent<UITexture>();

		screenshotButtonAnim = screenshotTexture.GetComponent<Animator>();
		screenshotBackground = transform.Find("Background_Screenshot").gameObject;
		screenshotBackground.SetActive(false);
	}

	protected override void Start()
	{
		base.Start();
		Android_MoreURL = ArtikFlowArcade.instance.configuration.Android_MoreURL;
		Playphone_MoreURL = ArtikFlowArcade.instance.configuration.Playphone_MoreURL;
		iOS_MoreURL = ArtikFlowArcade.instance.configuration.iOS_MoreURL;
		videoReward = ArtikFlowArcade.instance.configuration.videoReward;
		
		UITexture logoTexture = transform.Find("Logo").GetComponent<UITexture>();
		float original_width = logoTexture.width;
		logoTexture.mainTexture = ArtikFlowArcade.instance.configuration.gameLogoTexture;
		logoTexture.MakePixelPerfect();
		logoTexture.width = (int)original_width;
		float factor = original_width / (ArtikFlowArcade.instance.configuration.gameLogoTexture.width);
		logoTexture.height = (int)(ArtikFlowArcade.instance.configuration.gameLogoTexture.height * factor);
		
		videoAdButton.transform.Find("Label_Coins").GetComponent<UILabel>().text = "+" + videoReward;

		if(ArtikFlowArcade.instance.configuration.storeTarget == ArtikFlowArcadeConfiguration.StoreTarget.FRENCH_PREMIUM)
		{
			screenshotTexture.gameObject.SetActive(false);
			moreGamesButton.gameObject.SetActive(false);
			videoAdButton.gameObject.SetActive(false);
		}

		AFBase.Replay.instance.eventReplayAvailable.AddListener(onReplayAvailable);

		gameObject.SetActive(false);
		started = true;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && PopupManager.instance.getCurrentPopup() == null)
			onPlay();
	}

	void OnEnable()
	{
		if (!started)
			return;

		if (PopupManager.instance.getCurrentPopup() is IPopup_Revive)
			PopupManager.instance.getCurrentPopup().hide();

		IGameplayScreen.instance.toggleScore(false);
		replayButton.gameObject.SetActive(false);
		
		bestLabel.text = "" + SaveGameSystem.instance.getHighScore();
		scoreLabel.text = "" + ArtikFlowArcade.instance.getScore();

		newHighScore.SetActive( ArtikFlowArcade.instance.madeNewHighscore );

		// Reset tweens
		transform.Find("Logo").GetComponent<TweenPosition>().enabled = true;
		transform.Find("Logo").GetComponent<TweenPosition>().ResetToBeginning();

		transform.Find("Scoreboard").GetComponent<TweenScale>().enabled = true;
		transform.Find("Scoreboard").GetComponent<TweenScale>().ResetToBeginning();
		transform.Find("Button_Screenshot").GetComponent<TweenRotation>().enabled = true;
		transform.Find("Button_Screenshot").GetComponent<TweenRotation>().ResetToBeginning();
		transform.Find("Button_Screenshot").GetComponent<TweenPosition>().enabled = true;
		transform.Find("Button_Screenshot").GetComponent<TweenPosition>().ResetToBeginning();

		screenshotButtonAnim.enabled = false;
	}

	protected override void onHide()
	{
		iTween.Stop(transform.Find("Button_Play").gameObject);

		gameObject.SetActive(false);
	}
		
	protected override void onShow(ArtikFlowArcade.State oldState)
	{
		gameObject.SetActive(true);
		SetListenerVideoReward ();

		if (ArtikFlowArcade.instance.configuration.storeTarget == ArtikFlowArcadeConfiguration.StoreTarget.FRENCH_PREMIUM)
			return;

		int videoAdFrequency = ArtikFlowArcade.instance.configuration.videoAdFrequency;
		if (Arcade_BasePlayerStats.instance.increaseVideoAdFrequency())
			videoAdFrequency *= 2;

		if ((SaveGameSystem.instance.getGamesPlayed() % videoAdFrequency) != 0 && AFBase.Ads.instance.isRewardedVideoAvailable())
		{
			moreGamesButton.gameObject.SetActive(false);
			videoAdButton.gameObject.SetActive(true);

			GoogleAnalyticsV4.instance.LogEvent("video_impression", "impression", "results video impression", 0);
		}
		else
		{
			moreGamesButton.gameObject.SetActive(true);
			videoAdButton.gameObject.SetActive(false);
		}

		if(ArtikFlowArcade.instance.madeNewHighscore && SaveGameSystem.instance.getHighScore() >= 2)
			Invoke("showHighscorePopup", 0.5f);

		SaveGameSystem.instance.cloudSave();

		Invoke("setScreenshot", 0.2f);
	}

	void setScreenshot()
	{
		screenshotTexture.mainTexture = DriftScreenshotTaker.instance.GetPic();
	}

	void showHighscorePopup()
	{
		PopupManager.instance.showPopup<IPopup_Share>();
	}

	// --- Callbacks ---
	
	void onReplayAvailable()
	{
		replayButton.gameObject.SetActive(true);
	}


	public void onPlay()
	{
		if (LoadingScreen.instance.doingFade)
			return;

		//iTween.ScaleFrom(transform.Find("Button_Play").gameObject, iTween.Hash("scale", new Vector3(0.8f, 0.8f, 0.8f), "time", 0.14f, "easetype", iTween.EaseType.easeInOutSine));
		screenshotButtonAnim.SetBool("Zoom",false);
		

		base.play();
	}

	public void onReplay()
	{
		AFBase.Replay.instance.showReplay();
	}

	public void onMoreGames()
	{
#if UNITY_ANDROID
			if (ArtikFlowArcade.instance.configuration.storeTarget == ArtikFlowArcadeConfiguration.StoreTarget.PLAYPHONE){
				Application.OpenURL(Playphone_MoreURL);}
			else{
				//Application.OpenURL(Android_MoreURL);
				VascoGames.MoreGames.MoreGamesManager.Instance.Show();
			}
#elif UNITY_IOS
		Application.OpenURL(iOS_MoreURL);
#endif
	}


	void GiveReward(int reward)
	{
			
		PopupManager.instance.showPopup<Popup_Reward_Custom> ();

		Popup_Reward_Custom popupReward = (Popup_Reward_Custom)PopupManager.instance.getCurrentPopup ();

		popupReward.SetPopUp (reward);
	}

	public void onVideoAd()
	{
		AFBase.Ads.instance.ShowRewardedVideo ();
	}

	void SetListenerVideoReward()
	{

		AFBase.Ads.RewardedVideoListener listener = delegate (AFBase.Ads.RewardedVideoResult result) {

			if(result == AFBase.Ads.RewardedVideoResult.AVAILABLE)
			{

			}

			if(result == AFBase.Ads.RewardedVideoResult.SUCCESS)
			{

				Popup_Reward_Custom.GoBackToPopup (Popup_Reward_Custom.BackPopup.IAP);
				GiveReward(ArtikFlowArcade.instance.configuration.videoReward);

				GoogleAnalyticsV4.instance.LogEvent("video_reward", "play", "results video reward", 0);



			}
			else if (result == AFBase.Ads.RewardedVideoResult.SKIPPED)
			{

			}
			else if(result == AFBase.Ads.RewardedVideoResult.FAIL)
			{

			}
		};
		AFBase.Ads.instance.SetRewardedVideoCallback(listener);

	}



	public void onScreenshot()
	{
		if (Time.time - lastScreenTouchStamp < 2f)
			return;

		screenshotButtonAnim.enabled = true;
		screenshotButtonAnim.SetBool("Zoom",true);
		screenshotBackground.SetActive(true);
		Invoke("ShareScreenshot",1.5f);

		lastScreenTouchStamp = Time.time;
    }


	void ShareScreenshot(){
		screenshotButtonAnim.SetBool("Zoom",false);
		screenshotBackground.SetActive(false);
		Arcade_Share.instance.shareNativeImageScore(ScreenshotTaker.instance.getLastScreenshot());
		GoogleAnalyticsV4.instance.LogEvent("social", "share", "share score", 0);
	}
}

}