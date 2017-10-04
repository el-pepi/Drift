using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFArcade {

public abstract class IFlowButtons : GameScreen
{
	// --- Interface and methods to call:

	protected abstract void onShow(ArtikFlowArcade.State oldState);		// Should display this screen.
	protected abstract void onHide();									// Should hide this screen.
	protected abstract void onSound_turnOn();							// Called when the sound is turned on from a game call.
	protected abstract void onSound_turnOff();							// Called when the sound is turned off from a game call.

	// Must be called when clicking the like button
	protected void like() { onLike(); }

	// Must be called when clicking the rate button
	protected void rate() { onRate(); }

	// Must be called when clicking the shop button
	protected void shop() { onShop(); }

	// Must be called when clicking the stats button
	protected void stats() { onStats(); }

	// Must be called when clicking the IAP button
	protected void iap() { onIAP(); }


	// --- Partial private implementation (unity callbacks can be extended)

	protected virtual void Start()
	{
		ArtikFlowArcade.instance.eventStateChange.AddListener(onArtikFlowStateChange);
		ArtikFlowArcade.instance.eventToggleSound.AddListener(onSoundToggled);            // Toggle sound externally
	}

	private void onArtikFlowStateChange(ArtikFlowArcade.State oldstate, ArtikFlowArcade.State newstate)
	{
		if (newstate == ArtikFlowArcade.State.START_SCREEN || newstate == ArtikFlowArcade.State.LOST_SCREEN)
		{
			if (oldstate == ArtikFlowArcade.State.START_SCREEN || oldstate == ArtikFlowArcade.State.LOST_SCREEN)
				return;

			onShow(oldstate);
		}
		else
			onHide();
	}

	void onSoundToggled(bool toggle)        // Allows for external toggling of sound.
	{
		if (toggle)
			onSound_turnOn();
		else
			onSound_turnOff();
	}

	void onLike()
	{
		Application.OpenURL(ArtikFlowArcade.instance.configuration.Facebook_buy_Url);
	}

	void onRate()
	{
		#if UNITY_IOS
		float osVersion = -1f;
		string versionString = SystemInfo.operatingSystem.Replace("iPhone OS ", "");
		float.TryParse(versionString.Substring(0, 1), out osVersion);
		if(osVersion >= 10.3f) {
			NativeReviewRequest.RequestReview();
			return;
		}
		#endif

		if(ArtikFlowArcade.instance.configuration.disableRatePopup)
		{
#if UNITY_ANDROID
			if (ArtikFlowArcade.instance.configuration.storeTarget == ArtikFlowArcadeConfiguration.StoreTarget.PLAYPHONE)
				Application.OpenURL(ArtikFlowArcade.instance.configuration.Playphone_StarUrl.Replace("%", Application.bundleIdentifier));
			else
				Application.OpenURL(ArtikFlowArcade.instance.configuration.Android_StarUrl.Replace("%", Application.bundleIdentifier));
#elif UNITY_IOS
			Application.OpenURL(ArtikFlowArcade.instance.configuration.iOS_StarUrl.Replace("%", ArtikFlowArcade.instance.configuration.iOS_StoreId));
#endif
		}
		else
			PopupManager.instance.showPopup<IPopup_Rate>();
	}

	void onShop()
	{
		ArtikFlowArcade.instance.setState(ArtikFlowArcade.State.SHOP_SCREEN);
	}

	void onStats()
	{
#if UNITY_ANDROID
		PopupManager.instance.showPopup<IPopup_GameServices>();
#else
		AFBase.GameServices.instance.ShowLeaderboardUI();
#endif
	}

	void onIAP()
	{
		PopupManager.instance.showPopup<IPopup_IAP>();
	}

}

}