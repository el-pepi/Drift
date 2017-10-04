using UnityEngine;
using System.Collections;
using System;

namespace AFArcade {

public class DailyScreen : GameScreen
{
	public static DailyScreen instance;

	private int[] timeForRewards;        // ArtikFlowConfiguration
	private int dailyPrize;              // ArtikFlowConfiguration

	protected override void Awake()
	{
		base.Awake();

		instance = this;

		transform.Find("TopPanel").Find("Label_Tap").GetComponent<UILabel>().text = Language.get("Reward.TapToClaim").ToUpper();
	}

	void Start()
	{
		timeForRewards = ArtikFlowArcade.instance.configuration.timeForRewards;
		dailyPrize = ArtikFlowArcade.instance.configuration.dailyPrize;

		ArtikFlowArcade.instance.eventStateChange.AddListener(onArtikFlowStateChange);

		if(SaveGameSystem.instance.getGamesPlayed() == 0 && SaveGameSystem.instance.getSessionCount() == 0)		// First time opening the game
		{
			DateTime nextReward = DateTime.Now.AddMinutes(calculateTimeForNextReward());
			SaveGameSystem.instance.setNextDailyTime(nextReward);
		}

		gameObject.SetActive(false);
	}

	void OnEnable()
	{
		
	}

	int calculateTimeForNextReward()
	{
		int collected = SaveGameSystem.instance.getDailysCollected();

		if (collected == 0)		// First gift is instantaneous
			return 0;
		else
			collected--;

		if (collected >= timeForRewards.Length)
			return timeForRewards[timeForRewards.Length - 1];

		return timeForRewards[collected];
	}

	// --- Public ---

	public void claim()
	{
		GetComponent<Animator>().SetTrigger("claim");
	}

	public void playSound(string sound)
	{
		Audio.instance.playName(sound);
	}

	// --- Callbacks ---

	void onArtikFlowStateChange(ArtikFlowArcade.State oldstate, ArtikFlowArcade.State newstate)
	{
		if (newstate != ArtikFlowArcade.State.DAILY_SCREEN)
			gameObject.SetActive(false);

		if (newstate == ArtikFlowArcade.State.DAILY_SCREEN)
		{
			transform.Find("Background").GetComponent<UISprite>().color = new Color(0f, 0f, 0f, 0.92f);     // Anim fix
			gameObject.SetActive(true);
		}
	}

	public void onClaimAnimFinish()
	{
		transform.Find("Background").GetComponent<UISprite>().color = new Color(0f, 0f, 0f, 0.92f);		// Anim fix
		transform.Find("TopPanel").Find("Label_Tap").GetComponent<UILabel>().color = Color.white;

		RewardNotification.instance.give(dailyPrize);

		SaveGameSystem.instance.increaseDailysCollected();
		// DateTime nextReward = DateTime.Now;
		DateTime nextReward = DateTime.Now.AddMinutes(calculateTimeForNextReward());
		SaveGameSystem.instance.setNextDailyTime(nextReward);

		Invoke("end", 0.1f);
	}

	void end()
	{
		ArtikFlowArcade.instance.setState(ArtikFlowArcade.State.START_SCREEN);
	}
	
}

}