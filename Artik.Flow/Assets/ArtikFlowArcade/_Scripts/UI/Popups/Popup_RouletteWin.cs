using UnityEngine;
using System.Collections;

namespace AFArcade {

public class Popup_RouletteWin : IPopup_RouletteWin
{
	GameObject buttonOk;
	GameObject buttonVideo;
	bool watchedRewarded=false;

	protected override void Awake()
	{
		base.Awake();

		buttonOk = transform.Find("Button_Ok").gameObject;
		buttonVideo = transform.Find("Button_SpinAgain").gameObject;
	}

	void Start()
	{
		gameObject.SetActive(false);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			base.close();
	}

	protected override void onShow()
	{
		SetListenerRewardedVideo ();
		// Set texture
		UITexture itemTexture = transform.Find("Prize").Find("Texture").GetComponent<UITexture>();
		setTextureAndKeepWidth(itemTexture, prize.itemTexture);

		// Set small texture
		itemTexture = transform.Find("Prize_Desc").Find("Texture").GetComponent<UITexture>();
		setTextureAndKeepWidth(itemTexture, prize.itemTexture);

		// Set count
		transform.Find("Prize_Desc").Find("Label_Count").GetComponent<UILabel>().text = prize.itemCount.ToString();

		// Set label
		if(videoSpinsLeft > 0 && AFBase.Ads.instance.isRewardedVideoAvailable())
		{
			buttonOk.SetActive(false);
			buttonVideo.SetActive(true);
			videoSpinsLeft --;
			GoogleAnalyticsV4.instance.LogEvent("video_impression", "impression", "roulette video impression", 0);
		}
		else
		{
			buttonOk.SetActive(true);
			buttonVideo.SetActive(false);
		}

		prize.onEarn();

		base.onShow();
	}

	void setTextureAndKeepWidth(UITexture UITextureToSet, Texture newTexture)
	{
		float original_width = UITextureToSet.width;
		UITextureToSet.mainTexture = newTexture;
		UITextureToSet.MakePixelPerfect();
		float factor = UITextureToSet.width / original_width;
		UITextureToSet.width = (int) original_width;
		UITextureToSet.height = (int) (UITextureToSet.height / factor);
	}

	// --- Callbacks ---

	void SetListenerRewardedVideo()
	{

		AFBase.Ads.RewardedVideoListener listener = delegate (AFBase.Ads.RewardedVideoResult result) {
			if(result == AFBase.Ads.RewardedVideoResult.SUCCESS){
				RouletteScreen_Custom.customInstance.giveExtraFreeSpin();
				GoogleAnalyticsV4.instance.LogEvent("video_reward", "play", "roulette spin again", 0);
				watchedRewarded=true;
				onCloseClick();
			}
			else if (result == AFBase.Ads.RewardedVideoResult.FAIL||result == AFBase.Ads.RewardedVideoResult.SKIPPED)
			{
				watchedRewarded=false;
				onCloseClick();
			}

		};

		AFBase.Ads.instance.SetRewardedVideoCallback(listener);	
	}


		public void onSpinAgainClick()
		{
			AFBase.Ads.instance.ShowRewardedVideo ();
		}

	public void onCloseClick()
	{
		base.close();
	}

}

}