using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFArcade {

public class StarterPackButton : DailyButton
{

	protected override IEnumerator updateLabel(bool delayBeforeStart = false)
	{
		if(delayBeforeStart)
			yield return new WaitForSeconds(1);

		while (true)
		{
			int seconds = StarterPackButton.getSecondsUntilReward();
			dailyLabel.text = getTimeString(seconds);

			yield return new WaitForSeconds(1);
		}
	}

	public static new int getSecondsUntilReward()
	{
		DateTime now = System.DateTime.Now;
		System.TimeSpan travel = Arcade_Purchaser.instance.TimeLeftForStarterPack();
		double secondsTS = travel.TotalSeconds;

		return (int)secondsTS;
	}

	public override void setState(State newState)
	{
		if(newState == State.INVISIBLE)
		{
			GetComponent<TweenRotation>().enabled = false;
			gameObject.SetActive(false);
			glowSprite.SetActive(false);
			dailyLabel.gameObject.SetActive(false);
        }
		else if(newState == State.AVAILABLE)
		{
			gameObject.SetActive(true);
			dailyLabel.gameObject.SetActive(true);
			GetComponent<TweenRotation>().enabled = true;
			button.isEnabled = true;
			glowSprite.SetActive(true);
        }
		else if (newState == State.UNAVAILABLE)
		{
			gameObject.SetActive(true);
			dailyLabel.gameObject.SetActive(true);
			GetComponent<TweenRotation>().enabled = false;
			transform.localRotation = Quaternion.identity;
			button.isEnabled = false;
			glowSprite.SetActive(false);
			dailyLabel.text = "...";
		}

		state = newState;
	}

}

}