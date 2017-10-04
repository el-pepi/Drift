using UnityEngine;

using AFBase;

namespace AFArcade {

public class Arcade_Ads : MonoBehaviour {
	public static Arcade_Ads instance;

	// Encender para que el banner se muestre abajo. Apagar para que se muestre arriba.
	private bool bannerAtBottom;            // ArtikFlowConfiguration
	// Cada cuantas partidas se muestra un interstitial.
	private int interstitialFrequency;      // ArtikFlowConfiguration

	void Awake(){
		instance = this;
	}

	void Start(){
		bannerAtBottom = ArtikFlowArcade.instance.configuration.bannerAtBottom;
		interstitialFrequency = ArtikFlowArcade.instance.configuration.interstitialFrequency;

		ArtikFlowArcade.instance.eventStateChange.AddListener(onArtikFlowStateChange);

		if( !SaveGameSystem.instance.hasNoAds() && ArtikFlowBase.instance.configuration.storeTarget != ArtikFlowBaseConfiguration.StoreTarget.FRENCH_PREMIUM ){
			Ads.instance.showBanner(bannerAtBottom);
		}
	}
	
	void onArtikFlowStateChange(ArtikFlowArcade.State oldstate, ArtikFlowArcade.State newstate){
		if( newstate == ArtikFlowArcade.State.LOST_SCREEN ){
			int plays = ArtikFlowArcade.instance.playsThisSession;
			if( plays != 0 && plays % interstitialFrequency == 0 ){
				if( !SaveGameSystem.instance.hasNoAds() ){
					Ads.instance.showInterstitial();
				}
			}
		}
	}

}

}