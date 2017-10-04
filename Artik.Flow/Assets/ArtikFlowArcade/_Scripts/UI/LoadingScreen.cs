using UnityEngine;
using System.Collections;
using System;

/* -------------------------------------------------------------
*	UI/LoadingScreen script.
*	Zamaroht | January, 2016
*
*	Manages the initial splash.
------------------------------------------------------------- */

namespace AFArcade {

public class LoadingScreen : MonoBehaviour
{
	public delegate void FadeInOutListener();

	public static LoadingScreen instance;

	public float fadeTransitionTime = 0.2f;

	public bool doingFade {	get; private set; }
	
	UISprite fadeBlackSprite;

	void Awake()
	{
		instance = this;
	
		fadeBlackSprite = transform.Find("Sprite_Fader").GetComponent<UISprite>();
	}

	IEnumerator performFade(FadeInOutListener listener)
	{
		doingFade = true;
        fadeBlackSprite.color = new Color(0f, 0f, 0f, 0f);
		fadeBlackSprite.gameObject.SetActive(true);
        TweenAlpha.Begin(fadeBlackSprite.gameObject, fadeTransitionTime, 1f);
		yield return new WaitForSeconds(fadeTransitionTime);

		listener();

		TweenAlpha.Begin(fadeBlackSprite.gameObject, fadeTransitionTime, 0f);
		yield return new WaitForSeconds(fadeTransitionTime);
		fadeBlackSprite.gameObject.SetActive(false);
		doingFade = false;
	}

	// --- Public methods ---

	public void fadeInOut(FadeInOutListener listener)
	{
		StartCoroutine(performFade(listener));
	}	

}

}