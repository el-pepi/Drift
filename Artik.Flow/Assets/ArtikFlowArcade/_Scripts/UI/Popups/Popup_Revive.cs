using UnityEngine;
using System.Collections;

namespace AFArcade {

public class Popup_Revive : IPopup_Revive
{
	private int coinsToRevive;      // ArtikFlowConfiguration
	private bool canCancel;         // ArtikFlowConfiguration
	private int timeToRevive;       // ArtikFlowConfiguration
	
	UILabel reviveCountdown;
	UISprite spriteFill;
	UIButton coinsButton;
	UILabel coinsButtonLabel;
	UIButton videoButton;
	UILabel videoButtonLabel;

	float reviveTimer = -150f;  // timer > 0: Countdown in progress. 0 > timer > -100: Waiting for cancel. -100 > timer: Revive not active.

	protected override void Awake()
	{
		base.Awake();

		transform.Find("Label_Revive").GetComponent<UILabel>().text = Language.get("Revive.ReviveAsk");
		transform.Find("Button_Video").Find("Label").GetComponent<UILabel>().text = Language.get("Revive.Watch");
		
		reviveCountdown = transform.Find("Label_Timer").GetComponent<UILabel>();
		spriteFill = transform.Find("Sprite_Fill").GetComponent<UISprite>();

		coinsButton = transform.Find("Button_Coins").GetComponent<UIButton>();
		coinsButtonLabel = coinsButton.transform.Find("Label").GetComponent<UILabel>();
		videoButton = transform.Find("Button_Video").GetComponent<UIButton>();
		videoButtonLabel = videoButton.transform.Find("Label").GetComponent<UILabel>();
	}

	void Start()
	{
		coinsToRevive = ArtikFlowArcade.instance.configuration.coinsToRevive;
		canCancel = ArtikFlowArcade.instance.configuration.canCancel;
		timeToRevive = ArtikFlowArcade.instance.configuration.timeToRevive;

		transform.Find("Button_Close").gameObject.SetActive(canCancel);
		transform.Find("Button_Coins").Find("Label").GetComponent<UILabel>().text = "" + coinsToRevive;

		if (ArtikFlowArcade.instance.configuration.storeTarget == ArtikFlowArcadeConfiguration.StoreTarget.FRENCH_PREMIUM)
		{
			videoButton.gameObject.SetActive(false);
			coinsButton.transform.localPosition = new Vector3(0f, coinsButton.transform.localPosition.y, coinsButton.transform.localPosition.z);
		}

		gameObject.SetActive(false);
	}

	protected override void onShow()
	{
		base.onShow();
		
		SetRewardedVideo ();

		reviveTimer = timeToRevive;
		
		bool can_revive = (ArtikFlowArcade.instance.configuration.enableRevive) && 
			(SaveGameSystem.instance.getCoins() >= coinsToRevive || AFBase.Ads.instance.isRewardedVideoAvailable());

		if (!can_revive)
		{
			print("[INFO] Revive cancelled: No revive method available.");
			onReviveCancel();
			return;
		}

		if(SaveGameSystem.instance.getCoins() >= coinsToRevive)
		{
			coinsButton.isEnabled = true;
			coinsButtonLabel.color = new Color(coinsButtonLabel.color.r, coinsButtonLabel.color.g, coinsButtonLabel.color.b, 1f);
		}
		else
		{
			coinsButton.isEnabled = false;
			coinsButtonLabel.color = new Color(coinsButtonLabel.color.r, coinsButtonLabel.color.g, coinsButtonLabel.color.b, 0.5f);
		}

		if(AFBase.Ads.instance.isRewardedVideoAvailable())
		{
			videoButton.isEnabled = true;
			videoButtonLabel.color = new Color(videoButtonLabel.color.r, videoButtonLabel.color.g, videoButtonLabel.color.b, 1f);
		}
		else
		{
			videoButton.isEnabled = false;
			videoButtonLabel.color = new Color(videoButtonLabel.color.r, videoButtonLabel.color.g, videoButtonLabel.color.b, 0.5f);
		}
		
		spriteFill.fillAmount = 1f;
        reviveCountdown.text = timeToRevive.ToString();
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			onReviveCancel();
		}

		if (reviveTimer > 0f)
		{
			reviveTimer -= Time.deltaTime;
			int secs_left = (int)Mathf.Floor(reviveTimer) + 1;
			if (secs_left >= 0)
			{
				if (reviveCountdown.text != secs_left.ToString())
					reviveCountdown.text = secs_left.ToString();
			}

			spriteFill.fillAmount = ((float) reviveTimer / (float) timeToRevive);
		}
		else
			onReviveCancel();
	}

	// --- Callbacks ---

	public void onReviveCancel()
	{
		base.hide();
		ArtikFlowArcade.instance.terminateGame();
	}

	public void onReviveCoins()
	{
		base.hide();
		SaveGameSystem.instance.setCoins(SaveGameSystem.instance.getCoins() - coinsToRevive);
		ArtikFlowArcade.instance.revive();
	}

	public void onReviveVideo()
	{
		base.hide();
		AFBase.Ads.instance.ShowRewardedVideo ();
	}

	void SetRewardedVideo()
	{

		AFBase.Ads.RewardedVideoListener listener = delegate (AFBase.Ads.RewardedVideoResult result) {
			if (result == AFBase.Ads.RewardedVideoResult.SUCCESS)
			{
				GoogleAnalyticsV4.instance.LogEvent("video-reward", "play", "revive video reward", 0);

				ArtikFlowArcade.instance.revive();		
			}
			else if (result == AFBase.Ads.RewardedVideoResult.FAIL || result == AFBase.Ads.RewardedVideoResult.SKIPPED)
			{
				ArtikFlowArcade.instance.terminateGame();
			}
		};
		AFBase.Ads.instance.SetRewardedVideoCallback(listener);
	}

}

}