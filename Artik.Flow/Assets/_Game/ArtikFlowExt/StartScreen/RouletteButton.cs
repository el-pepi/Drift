using System.Collections;
using AFArcade;
using UnityEngine;
using System;

public class RouletteButton : MonoBehaviour {

	public GameObject notificationPrefab;

	public GameObject backSprite;

	public GameObject freeSpinContainer;

	public enum State
	{
		INVISIBLE,
		UNAVAILABLE,
		AVAILABLE,
	}
	public State state { get; protected set; }

	bool started;

	void Awake()
	{
	}

	void Start()
	{
		started = false;
	}

	void OnEnable()
	{
		StartCoroutine(updateLabel(!started));  // Stopped automatically by Unity when the gameObject is deactivated
	}

	protected virtual IEnumerator updateLabel(bool delayBeforeStart = false)
	{
		if(delayBeforeStart)
			yield return new WaitForSeconds(1);

		while (true)
		{
			if (state == State.UNAVAILABLE)
			{
				int seconds = getSecondsUntilReward();

				if (seconds < 0)
				{
					setState(State.AVAILABLE);
				}
				else
				{
				}

			}

			yield return new WaitForSeconds(1);
		}
	}

	public void CheckNotification()
	{
		int seconds = getSecondsUntilReward();

		Debug.Log ("Seconds: " + seconds);

		if (seconds < 0)
		{
			setState(State.AVAILABLE);
		}
		else
		{
			setState (State.UNAVAILABLE);
		}
	}


	// --- Public methods ---

	public virtual void setState(State newState)
	{
		if(newState == State.INVISIBLE)
		{
			gameObject.SetActive(false);
			notificationPrefab.SetActive(false);
			backSprite.SetActive (false);
			freeSpinContainer.SetActive (false);
		}
		else if(newState == State.AVAILABLE)
		{
			gameObject.SetActive(true);
			notificationPrefab.SetActive(true);
			notificationPrefab.SetActive(true);
			backSprite.SetActive (true);
			freeSpinContainer.SetActive (true);

		}
		else if (newState == State.UNAVAILABLE)
		{
			gameObject.SetActive(true);

			backSprite.SetActive (false);
			freeSpinContainer.SetActive (false);
			notificationPrefab.SetActive(false);
			

		}

		state = newState;
	}

	public static string getTimeString(int d)
	{
		var h = Mathf.Floor(d / 3600);
		var m = Mathf.Floor(d % 3600 / 60);
		var s = Mathf.Floor(d % 3600 % 60);
		return (h < 10 ? "0" : "") + h + ":" + (m < 10 ? "0" : "") + m + ":" + (s < 10 ? "0" : "") + s;
	}


	public static int getSecondsUntilReward()
	{
		DateTime now = System.DateTime.Now;
		System.TimeSpan travel = (SaveGameSystem.instance.getNextDailyTime() - now);
		double secondsTS = travel.TotalSeconds;

		return (int)secondsTS;
	}

	public void Shake(){
		iTween.ShakeRotation(gameObject, new Vector3(10f, 10f, 10f), 0.3f);
	}
}

