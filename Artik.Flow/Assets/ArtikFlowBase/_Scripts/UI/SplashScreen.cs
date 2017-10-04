using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AFBase {

public class SplashScreen : MonoBehaviour 
{
	public delegate void SplashHiddenListener();

	[HideInInspector]
	public UnityEvent eventSplashHidden = new UnityEvent();

	public static SplashScreen instance;

	Camera cam;
	UITexture fadeSplash;

	void Awake()
	{
		instance = this;
		
		fadeSplash = GetComponentInChildren<UITexture>(true);
		cam = transform.parent.GetComponentInChildren<Camera>();
	}

	IEnumerator kill(SplashHiddenListener listener = null)
	{
		yield return new WaitForSeconds(1f);

		fadeSplash.gameObject.SetActive(false);
		if(listener != null)
			listener();
		
		cam.gameObject.SetActive(false);
		transform.parent.gameObject.SetActive(false);
		Destroy(gameObject);
	}

	// ---

	public void showSplash()
	{
		fadeSplash.gameObject.SetActive(true);
	}

	public void hideSplash(SplashHiddenListener listener = null)
	{
		TweenAlpha.Begin(fadeSplash.gameObject, 1f, 0f);

		StartCoroutine(kill(listener));
	}

}

}