using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace AFArcade {

	public class Popup_RouletteWin_Custom : IPopup_RouletteWin
	{
		GameObject buttonOk;
		GameObject buttonVideo;
		UILabel prizeDesc;

		GameObject buttonClose;

		bool watchedRewarded=false;
		UILabel claimText;
		UILabel spinAgainText;
		public UILabel label_GemsDesc;


		protected override void Awake()
		{
			base.Awake();

			buttonOk = transform.Find("Button_Claim").gameObject;
			buttonVideo = transform.Find("Button_SpinAgain").gameObject;



			prizeDesc  = transform.Find ("label_GemsAmount").GetComponent<UILabel>();

			buttonClose = transform.Find ("Button_Close").gameObject;

			claimText = buttonOk.GetComponentInChildren<UILabel> ();
			spinAgainText = buttonVideo.GetComponentInChildren<UILabel> ();
		}



		void Start()
		{
			gameObject.SetActive(false);

			transform.Find ("label_Title").GetComponent<UILabel>().text = Language.get ("Roulette.YouGot");

			claimText.text = Language.get ("Roulette.ClaimPrize");

			spinAgainText.text = Language.get ("Roulette.SpinAgain");

		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
				base.close();
		}

		protected override void onShow()
		{
			SetListenerRewardedVideo ();


			// Set label
			 if(videoSpinsLeft > 0 && AFBase.Ads.instance.isRewardedVideoAvailable())
			 {
			 	buttonOk.SetActive(false);
			 	buttonVideo.SetActive(true);
			 	videoSpinsLeft --;
				GoogleAnalyticsV4.instance.LogEvent("video_impression", "impression", "roulette video impression", 0);

				buttonClose.SetActive (true);
			 }
			 else
			 {
				buttonOk.SetActive(true);
				buttonVideo.SetActive(false);
				buttonClose.SetActive (false);
			 }

			DriftGameplayScreen.instance.freezeCoins = true;

			prize.onEarn();
	
			prizeDesc.text = prize.itemCount.ToString();
			label_GemsDesc.text = Language.get ("Roulette.YouGot") +" "+ Language.get ("Reward.Coins").Replace("%",prize.itemCount.ToString());
			base.onShow();
		}




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

			DriftGameplayScreen.instance.freezeCoins = false;

			base.hide();



			if(watchedRewarded == false) {
				//GameManager.instance.closedRouletteReward.Invoke();
				if(buttonVideo.activeSelf) {
					AFBase.Ads.instance.showInterstitial();
				}
			}
			watchedRewarded = false;
		}




	}

}