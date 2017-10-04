using UnityEngine;
using Heyzap;
using System.Collections;

namespace AFBase {

	public class Ads : MonoBehaviour {
		public delegate void RewardedVideoListener(RewardedVideoResult result);

		public delegate void VideoAvailableListener(bool result);

		public enum RewardedVideoResult{
			SUCCESS,
			FAIL,
			CRITICAL_FAIL,
			AVAILABLE,
			SKIPPED
		}

		public static Ads instance;

		bool showingBanner;

		void Awake(){
			instance = this;
		}

		void Start(){
			// Vungle.init(ArtikFlow.instance.configuration.VungleAndroidId, ArtikFlow.instance.configuration.VungleIosId);
			HeyzapAds.Start(ArtikFlowBase.instance.configuration.ARTIK_HEYZAP_ID, HeyzapAds.FLAG_NO_OPTIONS);

			// HeyzapAds.ShowDebugLogs();
			// HeyzapAds.ShowThirdPartyDebugLogs();


			HZIncentivizedAd.Fetch();
			Invoke("rewardedVideoCheckAndRetry", 10f);

			// Invoke("testsuite", 5f);
		}

		void testsuite(){
			HeyzapAds.ShowMediationTestSuite();
		}

		void rewardedVideoCheckAndRetry(){
			/*
	if( !HZIncentivizedAd.IsAvailable() ){
		HZIncentivizedAd.Fetch();
	}*/

			if( !HZInterstitialAd.IsAvailable() ){
				HZInterstitialAd.Fetch();
			}

			Invoke("rewardedVideoCheckAndRetry", 10f);
		}

		// Se llama desde afuera cuando se compra la IAP no-ads
		public void hideBanner(){
			showingBanner = false;
			HZBannerAd.Hide();
		}

		public void showBanner(bool bottom){
			showingBanner = true;

			HZBannerAd.AdDisplayListener listener = ((string state, string tag) => {
				if( state == "error" ){  // Retry
					if( showingBanner ){
						StartCoroutine(retryShowBanner(bottom));
					}
				}
			});

			HZBannerAd.SetDisplayListener(listener);

			HZBannerShowOptions showOptions = new HZBannerShowOptions();
			if( bottom ){
				showOptions.Position = HZBannerShowOptions.POSITION_BOTTOM;
			} else {
				showOptions.Position = HZBannerShowOptions.POSITION_TOP;
			}

			HZBannerAd.ShowWithOptions(showOptions);
		}

		IEnumerator retryShowBanner(bool bottom){
			yield return new WaitForSeconds(2f);
			showBanner(bottom);
		}

		public void showInterstitial(){
			HZInterstitialAd.Show();
			HZInterstitialAd.Fetch();
		}

		public bool isRewardedVideoAvailable(){
			return HZIncentivizedAd.IsAvailable();
		}

		public void FetchVideo(){
			HZIncentivizedAd.Fetch();
		}

		public void FetchInterstitial(){
			HZInterstitialAd.Fetch();
		}

		public void SetRewardedVideoCallback(RewardedVideoListener listener){

			HZIncentivizedAd.AdDisplayListener heyzap_listener = delegate (string adState, string adTag){

				if( adState.Equals("available") ){
					listener(RewardedVideoResult.AVAILABLE);
				}
				else if( adState.Equals("incentivized_result_complete") ){
					listener(RewardedVideoResult.SUCCESS);
				} else if( adState.Equals("incentivized_result_incomplete") ){
					listener(RewardedVideoResult.SKIPPED);
				} else if ((adState.Equals("failed"))||(adState.Equals("fetch_failed"))) {
					listener(RewardedVideoResult.FAIL);
				}
			};

			HZIncentivizedAd.SetDisplayListener(heyzap_listener);
		}

		public void ShowRewardedVideo()
		{
			HZIncentivizedAd.Fetch();
			HZIncentivizedAd.Show();
		}

	}

}