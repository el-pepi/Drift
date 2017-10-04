using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace AFBase {
	
public class CrossPromo : MonoBehaviour
{
	[HideInInspector]
	public UnityEvent eventReadyToShow = new UnityEvent();

	public static CrossPromo instance;

	public vg_interstitial vascoInterstitial;

	void Awake()
	{
		instance = this;

		vascoInterstitial.eventReady.AddListener(onReady);
	}

	void onReady()
	{
		eventReadyToShow.Invoke();	
	}

	public void display()
	{
		vascoInterstitial.displayLoadedBanner();
	}

}

}