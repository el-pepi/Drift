using UnityEngine;
using System.Collections;

/* -------------------------------------------------------------
*	UI/GameplayScreen script.
*	Zamaroht | January, 2016
*
*	Manages the GameplayScreen.
------------------------------------------------------------- */

namespace AFArcade {

public class DriftGameplayScreen : IGameplayScreen
{
	const float TIME_TO_SET_COINS = 1.5f;

	GameObject score;
	UILabel scoreLabel;
	UILabel bestLabel;
	GameObject coins;
	UILabel coinsLabel;
	UISprite coinsSprite;
	GameObject recordingSprite;

	float coinsSetTime;
	bool settingCoins;
	float newCoins;

	protected override void Awake()
	{
		base.Awake();

		score = transform.Find("Score").gameObject;
		scoreLabel = score.transform.Find("Label_Score").GetComponent<UILabel>();
		bestLabel = score.transform.Find("Label_Best").GetComponent<UILabel>();
		coins = transform.Find("Coins").gameObject;
		coinsLabel = coins.transform.Find("Label").GetComponent<UILabel>();
		coinsSprite = coins.transform.Find("Sprite").GetComponent<UISprite>();
		recordingSprite = transform.Find("Sprite_Recording").gameObject;
        recordingSprite.SetActive(false);
	}

	protected override void Start()
	{
		base.Start();

		if(ArtikFlowArcade.instance.configuration.useWhiteLabels)
		{
			scoreLabel.color = Color.white;
			bestLabel.color = Color.white;
		}

		AFBase.Replay.instance.eventStartedRecording.AddListener(onRecordingStarted);
		AFBase.Replay.instance.eventStoppedRecording.AddListener(onRecordingFinished);
	}

	void Update()
	{
		if (Time.time - coinsSetTime < TIME_TO_SET_COINS)
		{
			int curr;
			if (!int.TryParse(coinsLabel.text, out curr))
				curr = 0;

			coinsLabel.text = "" + Mathf.Ceil(Mathf.Lerp(curr, newCoins, (Time.time - coinsSetTime) / TIME_TO_SET_COINS));
		}
		else if(settingCoins)
		{
			coinsLabel.text = "" + SaveGameSystem.instance.getCoins();
			settingCoins = false;
		}
	}

	// --- Public ---

	public override Vector3 getCoinsPosition()
	{
		return coins.transform.position;
	}

	// --- Callbacks ---

	void onRecordingStarted()
	{
		recordingSprite.SetActive(true);
	}

	void onRecordingFinished()
	{
		recordingSprite.SetActive(false);
	}

	protected override void onToggleScore(bool show)
	{
		score.SetActive(show);
	}

	protected override void onToggleCoins(bool show)
	{
		coins.SetActive(show);
	}

	protected override void onScoreUpdate(int score)
	{
		scoreLabel.text = "" + score;
	}

	protected override void onCoinsUpdate(int coins)
	{
		int oldcoins;
		if (!int.TryParse(coinsLabel.text, out oldcoins))
			oldcoins = 0;

		settingCoins = true;
		coinsSetTime = Time.time;
		newCoins = coins;

		coinsSprite.transform.localScale = Vector3.one;
		iTween.ScaleFrom(coinsSprite.gameObject, iTween.Hash("scale", new Vector3(1.23f, 1.23f, 1.23f), "time", TIME_TO_SET_COINS / 2f, "delay", TIME_TO_SET_COINS / 2f));
	}

	protected override void onHighscoreUpdate(int newHigh)
	{
		bestLabel.text = Language.get("GameplayScreen.Best") + " " + newHigh;
    }

}

}