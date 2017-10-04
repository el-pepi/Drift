using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using UnityEngine.SceneManagement;

/* -------------------------------------------------------------
*	ArtikFlowArcadeGame abstract class.
*	Zamaroht | January, 2016
*
*	This abstract class must be implemented by one GameObject in the game scene.
*	The game scene should then be referenced in the configuration file.
*	Further instructions in the documentation.
------------------------------------------------------------- */

namespace AFArcade {
	[System.Serializable]
	public abstract class ArtikFlowArcadeGame : MonoBehaviour
	{
	public int unlockThreshold = 0;
	public int unlockGems = 0;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }
	[System.Serializable]
	public class IntEvent : UnityEvent<int> { }
	[System.Serializable]
	public class StringIntEvent : UnityEvent<string, int> { }
	[System.Serializable]
	public class CameraEvent : UnityEvent<Camera> { }

	[HideInInspector]
	public UnityEvent eventResetReady;				// ()
	[HideInInspector]
	public UnityEvent eventFinish;					// ()
	[HideInInspector]
	public IntEvent eventAddCoins;			        // (int count=1)
	[HideInInspector]
	public IntEvent eventAddScore;					// (int count=1)
	[HideInInspector]
	public UnityEvent eventTakeScreenshot;			//
	[HideInInspector]
	public CameraEvent eventTakeScreenshotCamera;   //
	[HideInInspector]
	public StringIntEvent eventUsePowerup;          // Uses a specified amount of a certain powerup
	[HideInInspector]
	public UnityEvent eventNeedGems;
	[HideInInspector]
	public BoolEvent eventToggleSound;
	//[HideInInspector]
	//public IntEvent eventSetCurrentGamemode;		// Set a new gamemode ID to interact with the score system

	public abstract void reset(Character character);
	public abstract void play();
	public abstract void revive();
	public abstract void switchSound(bool state);

	public virtual void Awake()
	{
		eventResetReady = new UnityEvent();
		eventFinish = new UnityEvent();
		eventAddCoins = new IntEvent();
		eventAddScore = new IntEvent();
		eventTakeScreenshot = new UnityEvent();
		eventTakeScreenshotCamera = new CameraEvent();
		eventUsePowerup = new StringIntEvent();
		eventNeedGems = new UnityEvent();
		eventToggleSound = new BoolEvent();
		//eventSetCurrentGamemode = new IntEvent();

		if(ArtikFlowArcade.instance == null)
			SceneManager.LoadScene("ArtikFlowBase");
    }
	

		public virtual void OnChanged(){

		}
}

}