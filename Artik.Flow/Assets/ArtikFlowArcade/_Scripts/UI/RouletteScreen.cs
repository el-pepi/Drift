using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFArcade {

public class RouletteScreen : IRouletteScreen 
{
	Transform rouletteFront;
	Transform pickPivot;
	UILabel labelFreeSpin;
	GameObject buttonFreeSpin;
	GameObject buttonGemsSpin;

	List<RuntimeRouletteSlice> currentSlices;
	int currentWinningSlice;

	bool spinning;
	bool hasExtraSpin;

	protected override void Awake()
	{
		base.Awake();

		rouletteFront = transform.Find("Roulette").Find("RouletteFront");
		pickPivot = transform.Find("Roulette").Find("PickPivot");
		labelFreeSpin = transform.Find("Label_FreeSpin").GetComponent<UILabel>();
		buttonFreeSpin = transform.Find("Button_Spin").gameObject;
		buttonGemsSpin = transform.Find("Button_SpinGems").gameObject;
	}

	void Update()
	{
		if (!spinning && Input.GetKeyDown(KeyCode.Escape) && PopupManager.instance.getCurrentPopup() == null)
			onBack();

		if(DailyButton.getSecondsUntilReward() > 0 && !hasExtraSpin)
		{
			string timeLeft = DailyButton.getTimeString(DailyButton.getSecondsUntilReward());
			labelFreeSpin.text = Language.get("Roulette.TimeLeft").Replace("%", timeLeft);
		}
		else if(labelFreeSpin.gameObject.activeSelf)
		{
			labelFreeSpin.gameObject.SetActive(false);
			buttonGemsSpin.SetActive(false);
			buttonFreeSpin.SetActive(true);
		}
		
	}

	private void populateRoulette()
	{
		rouletteFront.eulerAngles = Vector3.zero;

		currentSlices = base.getRouletteItems(); 
		currentWinningSlice = base.pickWinningSlice(currentSlices);

		GameObject template = rouletteFront.GetChild(0).gameObject;
		Vector2 templateDistance = template.transform.position - rouletteFront.position;

		int i = 0;
		foreach(RuntimeRouletteSlice slice in currentSlices)
		{
			// Create new child
			GameObject newItem = NGUITools.AddChild(rouletteFront.gameObject, template);
			newItem.name = "" + i + "." + slice.item.name;

			// Set its params
			UITexture itemTexture = newItem.GetComponent<UITexture>();
			float original_width = itemTexture.width;
			itemTexture.mainTexture = slice.item.itemTexture;
			itemTexture.MakePixelPerfect();
			float factor = itemTexture.width / original_width;
			itemTexture.width = (int) original_width;
			itemTexture.height = (int) (itemTexture.height / factor);

			slice.item.setText();
			UILabel label = newItem.transform.Find("Label").GetComponent<UILabel>();
			if(slice.item.itemText == null || slice.item.itemText.Length == 0)
				label.gameObject.SetActive(false);
			else	
			{
				label.gameObject.SetActive(true);
				label.text = slice.item.itemText;
			}

			// Set its position
			float angle = getAngleFromSliceIndex(i, currentSlices.Count);
			newItem.transform.eulerAngles = new Vector3(0f, 0f, angle);
			newItem.transform.position = rouletteFront.position + (Quaternion.Euler(0f, 0f, angle) * templateDistance);

			i ++;
		}

		template.SetActive(false);
	}

	private void unpopulateRoulette()
	{
		foreach(Transform t in rouletteFront)
		{
			if(t.name == "Template")
				t.gameObject.SetActive(true);
			else
				Destroy(t.gameObject);
		}
	}

	float getAngleFromSliceIndex(int sliceIdx, int sliceCount)
	{
		return (360f / sliceCount) * sliceIdx;
	}

	IEnumerator performSpin(int winningIndex)
	{
		spinning = true;

		float anglePerSlice = 360f / currentSlices.Count;
		float angleTarget = clamp360(getAngleFromSliceIndex(winningIndex, currentSlices.Count) * 1);

		float angleDifference = (clamp360(rouletteFront.eulerAngles.z - angleTarget) * 1) / 360f;
		angleDifference += UnityEngine.Random.Range(-anglePerSlice / 360f / 3f, anglePerSlice / 360f / 3f);
		iTween.RotateBy(rouletteFront.gameObject, iTween.Hash("z", -8f + angleDifference, "easeType", iTween.EaseType.easeOutCubic, "time", 6f));
		
		yield return new WaitForSeconds(5.5f);

		spinning = false;
		givePrize();
	}

	IEnumerator movePick()
	{
		float prev_rot = rouletteFront.eulerAngles.z;
		float anglePerSlice = 360f / currentSlices.Count;
		float offsetted_roulette_rot;

		while(true)
		{
			offsetted_roulette_rot = rouletteFront.eulerAngles.z + (anglePerSlice / 2f);

			float dif = offsetted_roulette_rot % anglePerSlice - prev_rot % anglePerSlice;
			if(dif > 0)
				pickPivot.eulerAngles = new Vector3(0f, 0f, 62f - ((dif / anglePerSlice) * 32f));

			pickPivot.eulerAngles = new Vector3(0f, 0f, pickPivot.eulerAngles.z / 1.2f);
			prev_rot = offsetted_roulette_rot;

			yield return null;
		}
	}

	float clamp360(float val)
	{
		while(val < 0)
			val += 360;
		while(val >= 360)
			val -= 360;
		return val;
	}

	void givePrize()
	{
		float anglePerSlice = 360f / currentSlices.Count;
		float offsetted_roulette_rot = rouletteFront.eulerAngles.z + (anglePerSlice / 2f);
		int winIdx = (currentSlices.Count - Mathf.FloorToInt(clamp360(offsetted_roulette_rot) / anglePerSlice)) % currentSlices.Count;

		RouletteItem prizeWon = currentSlices[winIdx].item;
		IPopup_RouletteWin.prize = prizeWon;
		PopupManager.instance.showPopup<IPopup_RouletteWin>();

		// Post-prize
		SaveGameSystem.instance.increaseDailysCollected();
		DateTime nextReward = DateTime.Now.AddMinutes(calculateTimeForNextReward());
		SaveGameSystem.instance.setNextDailyTime(nextReward);

		labelFreeSpin.gameObject.SetActive(true);
		buttonGemsSpin.SetActive(true);
		buttonFreeSpin.SetActive(false);

		Invoke("repopulateRoulette", 0.7f);
	}

	void repopulateRoulette()
	{
		unpopulateRoulette();
		populateRoulette();
	}

	int calculateTimeForNextReward()
	{
		int[] timeForRewards = ArtikFlowArcade.instance.configuration.timeForRewards;
		int collected = SaveGameSystem.instance.getDailysCollected();

		if (collected == 0)		// First gift is instantaneous
			return 0;
		else
			collected--;

		if (collected >= timeForRewards.Length)
			return timeForRewards[timeForRewards.Length - 1];

		return timeForRewards[collected];
	}

	// --- Callbacks ---

	protected override void onHide()
	{
		gameObject.SetActive(false);

		unpopulateRoulette();
	}

	protected override void onShow(ArtikFlowArcade.State oldState)
	{
		gameObject.SetActive(true);

		if(DailyButton.getSecondsUntilReward() > 0 && !hasExtraSpin)
		{
			buttonFreeSpin.gameObject.SetActive(false);
			buttonGemsSpin.gameObject.SetActive(true);
			labelFreeSpin.gameObject.SetActive(true);
		}
		else
		{
			buttonFreeSpin.gameObject.SetActive(true);
			buttonGemsSpin.gameObject.SetActive(false);
			labelFreeSpin.gameObject.SetActive(false);
		}

		populateRoulette();
		StartCoroutine(movePick());
	}

	protected override void spin()
	{
		StartCoroutine(performSpin(currentWinningSlice));
	}

	public override void giveExtraFreeSpin()
	{
		hasExtraSpin = true;
		buttonFreeSpin.SetActive(true);
		buttonGemsSpin.SetActive(false);
	}

	public void onFreeSpinClick()
	{
		buttonFreeSpin.SetActive(false);
		buttonGemsSpin.SetActive(false);

		if(!hasExtraSpin)		// This is a free gifted spin
			IPopup_RouletteWin.videoSpinsLeft = 1;		// Show video button on next RouletteWin popup.

		hasExtraSpin = false;

		spin();
	}

	public void onGemsSpinClick()
	{
		int cost = ArtikFlowArcade.instance.configuration.gemsToSpin;

		if(cost > SaveGameSystem.instance.getCoins())
			PopupManager.instance.showPopup<IPopup_IAP>();
		else
		{
			buttonFreeSpin.SetActive(false);
			buttonGemsSpin.SetActive(false);

			SaveGameSystem.instance.setCoins(SaveGameSystem.instance.getCoins() - cost);
			spin();
		}
	}

	public void onBack()
	{
		ArtikFlowArcade.instance.setState(ArtikFlowArcade.State.START_SCREEN);
	}

}

}
