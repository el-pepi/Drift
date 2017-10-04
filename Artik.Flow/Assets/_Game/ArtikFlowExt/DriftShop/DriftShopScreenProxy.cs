using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DriftShopScreenProxy : MonoBehaviour 
{
	[HideInInspector]
	public UnityEvent eventBackClick = new UnityEvent();
	[HideInInspector]
	public UnityEvent eventLeftClick = new UnityEvent();
	[HideInInspector]
	public UnityEvent eventRightClick = new UnityEvent();
	[HideInInspector]
	public UnityEvent eventVideoClick = new UnityEvent();
	[HideInInspector]
	public UnityEvent eventGemsClick = new UnityEvent();
	[HideInInspector]
	public UnityEvent eventIAPClick = new UnityEvent();
	[HideInInspector]
	public UnityEvent eventSelectClick = new UnityEvent();
	[HideInInspector]
	public UnityEvent eventPlayClick = new UnityEvent();

	// ---

	public void onBackClick()
	{
		eventBackClick.Invoke();
	}

	public void onLeftClick()
	{
		eventLeftClick.Invoke();
	}

	public void onRightClick()
	{
		eventRightClick.Invoke();
	}

	public void onVideoClick()
	{
		 eventVideoClick.Invoke();
	}

	public void onGemsClick()
	{
		 eventGemsClick.Invoke();
	}

	public void onIAPClick()
	{
		 eventIAPClick.Invoke();
	}

	public void onSelectClick()
	{
		 eventSelectClick.Invoke();
	}

	public void onPlayClick()
	{
		 eventPlayClick.Invoke();
	}

	// ---

	public void onAnimInFinish()
	{
		// GetComponent<Animator>().enabled = false;
	}

	public void onAnimOutFinish()
	{
		AFArcade.ArtikFlowArcade.instance.setState(AFArcade.ArtikFlowArcade.State.START_SCREEN);
		gameObject.SetActive(false);
	}

}
