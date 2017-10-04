using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AFArcade;


public class StarterPackButton_Custom : MonoBehaviour
{
	public UILabel dailyLabel;

	public enum State
	{
		INVISIBLE,
		UNAVAILABLE,
		AVAILABLE,
	}
	public State state { get; protected set; }

	protected UIButton button;

	void OnEnable()
	{
		StartCoroutine(updateLabel());
	}

	protected  IEnumerator updateLabel()
	{
		//yield return new WaitForSeconds(1);

		while (true)
		{
			int seconds = StarterPackButton_Custom.getSecondsUntilReward();
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

	public  void setState(State newState)
	{
		if(newState == State.INVISIBLE)
		{
			DeleteKey ();
			gameObject.SetActive(false);
			dailyLabel.gameObject.SetActive(false);
		}
		else if(newState == State.AVAILABLE)
		{
			gameObject.SetActive(true);
			dailyLabel.gameObject.SetActive(true);
			OpenStarter ();
		}
		else if (newState == State.UNAVAILABLE)
		{
			gameObject.SetActive(true);
			dailyLabel.gameObject.SetActive(true);
			transform.localRotation = Quaternion.identity;
			dailyLabel.text = "...";
		}

		state = newState;
	}

	void OpenStarter()
	{
		if(!PlayerPrefs.HasKey ("OpenStarter"))
		{
			PlayerPrefs.SetInt ("OpenStarter", 0);
			PopupManager.instance.showPopup<IPopup_HalfPack> ();
		}
	}

	void DeleteKey()
	{
		if(PlayerPrefs.HasKey ("OpenStarter")&&
			(!CharacterManager.instance.hasAllCharacters() || 
			!SaveGameSystem.instance.hasNoAds() || 
			!SaveGameSystem.instance.hasDuplicate()))
		{
			PlayerPrefs.DeleteKey ("OpenStarter");
		}
	}




	public static string getTimeString(int d)
	{
		var h = Mathf.Floor(d / 3600);
		var m = Mathf.Floor(d % 3600 / 60);
		var s = Mathf.Floor(d % 3600 % 60);
		return (h < 10 ? "0" : "") + h + ":" + (m < 10 ? "0" : "") + m + ":" + (s < 10 ? "0" : "") + s;
	}
}
