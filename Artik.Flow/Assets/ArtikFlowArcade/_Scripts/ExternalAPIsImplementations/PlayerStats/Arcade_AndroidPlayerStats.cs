#if UNITY_ANDROID

using UnityEngine;
using System.Collections;
using GooglePlayGames;
using AFBase;

namespace AFArcade {

public class Arcade_AndroidPlayerStats : Arcade_BasePlayerStats
{
	GooglePlayGames.BasicApi.PlayerStats playerStats = null;

	protected override void Awake()
	{
		instance = this;
	}

	protected void Start()
	{
		GameServices.instance.eventAuthFinish.AddListener(onGameServicesAuth);
		ArtikFlowArcade.instance.eventStateChange.AddListener(onArtikFlowStateChange);

		/*
		// Hardcoded props for testing:
		playerStats = new GooglePlayGames.BasicApi.PlayerStats();
		playerStats.SpendPercentile = 0.9f;
		playerStats.SessPercentile = 0.9f;
		playerStats.NumberOfSessions = 20;
		playerStats.DaysSinceLastPlayed = 8;
		playerStats.ChurnProbability = 0.9f;
		*/
	}

	void onArtikFlowStateChange(ArtikFlowArcade.State oldState, ArtikFlowArcade.State newState)
	{
		if (newState == ArtikFlowArcade.State.LOST_SCREEN)
		{
			if(playerStats != null && playerStats.SpendProbability >= HIGH)
			{
				if (ArtikFlowArcade.instance.playsThisSession % 7 == 6)
					PopupManager.instance.showPopup<IPopup_IAP>();
			}

			if (playerStats != null && playerStats.SessPercentile >= HIGH && playerStats.SpendPercentile >= 0 && playerStats.SpendPercentile <= LOW)
			{
				if (ArtikFlowArcade.instance.playsThisSession % 9 == 8)
					Invoke("showSharePopup", 0.2f);
			}

			if(playerStats != null && playerStats.NumberOfSessions >= 6 && playerStats.SpendPercentile >= HIGH && playerStats.DaysSinceLastPlayed >= 3)
			{
				if (ArtikFlowArcade.instance.playsThisSession == 2)
					PopupManager.instance.showPopup<IPopup_HalfPack>();
			}
		}
	}

	void onGameServicesAuth(bool success)
	{
		if (success == false)
			return;

		Invoke("getPlayerStats", 2f);
	}

	void getPlayerStats()
	{
		((PlayGamesLocalUser)Social.localUser).GetStats((rc, stats) => {
			if (rc <= 0)
			{
				playerStats = stats;
			}
		});

		// Init finished. Start processing:
		if (playerStats != null)
		{
			// Do something
		}
	}

	// ---

	void showSharePopup()
	{
		PopupManager.instance.showPopup<IPopup_Share>();
	}

	/// Should the revive popup be always shown, regardless of score made?
	public override bool alwaysRevive()
	{
		if (playerStats == null)
			return false;
		else
			return playerStats.SpendProbability >= 0 && playerStats.SpendProbability <= LOW;
	}

	/// Should the frequency of video ads in the lost screen be increased
	public override bool increaseVideoAdFrequency()
	{
		if (playerStats == null)
			return false;
		else
			return playerStats.SpendProbability >= 0 && playerStats.SpendProbability <= LOW;
	}

	/// If the GameServices should be highlighted or not
	public override bool shouldHighlightGameServices()
	{
		if (playerStats == null)
			return false;
		else
			return playerStats.SpendPercentile >= HIGH && playerStats.ChurnProbability >= HIGH;
	}

}

}
#endif
