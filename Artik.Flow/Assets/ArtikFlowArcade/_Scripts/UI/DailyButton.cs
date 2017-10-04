using UnityEngine;
using System.Collections;
using System;

namespace AFArcade {

public class DailyButton : MonoBehaviour
{
	public UILabel dailyLabel;
	public GameObject glowSprite;

	public enum State
	{
		INVISIBLE,
		UNAVAILABLE,
		AVAILABLE,
	}
	public State state { get; protected set; }
	
	protected UIButton button;
	bool started;

	void Awake()
	{
		button = GetComponent<UIButton>();
    }

	void Start()
	{
		if(!(this is StarterPackButton))
		{
			if(ArtikFlowArcade.instance.configuration.replaceDailyWithRoulette)
			{
				GetComponent<UISprite>().atlas = null;
				transform.GetChild(0).gameObject.SetActive(true);

				glowSprite.GetComponent<UISprite>().atlas = null;
				glowSprite.transform.GetChild(0).gameObject.SetActive(true);
			}
			else
			{
				// GetComponent<UISprite>().spriteName = "MenuGift"; 	<- Commented, just don't change it from the way it's set up in unity
				transform.GetChild(0).gameObject.SetActive(false);

				// glowSprite.GetComponent<UISprite>().spriteName = "MenuGiftGlow";
				glowSprite.transform.GetChild(0).gameObject.SetActive(false);
			}
		}

		started = true;
	}

	void OnEnable()
	{
		glowSprite.transform.position = transform.position;
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
				int seconds = DailyButton.getSecondsUntilReward();

				if (seconds < 0)
				{
					setState(State.AVAILABLE);
				}
				else
				{
					dailyLabel.text = getTimeString(seconds);
				}

			}

			yield return new WaitForSeconds(1);
		}
	}

	public static string getTimeString(int d)
	{
		var h = Mathf.Floor(d / 3600);
		var m = Mathf.Floor(d % 3600 / 60);
		var s = Mathf.Floor(d % 3600 % 60);
		return (h < 10 ? "0" : "") + h + ":" + (m < 10 ? "0" : "") + m + ":" + (s < 10 ? "0" : "") + s;
	}

	// --- Public methods ---

	public virtual void setState(State newState)
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

			if(ArtikFlowArcade.instance.configuration.replaceDailyWithRoulette)
				dailyLabel.text = Language.get("Roulette.FreeSpin");
			else
				dailyLabel.text = "";
        }
		else if (newState == State.UNAVAILABLE)
		{
			gameObject.SetActive(true);
			dailyLabel.gameObject.SetActive(true);
			
			if(ArtikFlowArcade.instance.configuration.replaceDailyWithRoulette == false)
			{
				GetComponent<TweenRotation>().enabled = false;
				transform.localRotation = Quaternion.identity;
				button.isEnabled = false;
				glowSprite.SetActive(false);
			}
			
			dailyLabel.text = "...";
		}

		state = newState;
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

}