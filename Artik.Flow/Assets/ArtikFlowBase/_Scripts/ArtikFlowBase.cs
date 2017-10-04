using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/* -------------------------------------------------------------
*	ArtikFlowBase main script.
*	Zamaroht | January, 2017
*
*	Base ArtikFlow class. Takes care of the synchronization with the
*	selected ArtikFlow frontend scene.
------------------------------------------------------------- */

namespace AFBase {
	
public class ArtikFlowBase : MonoBehaviour 
{
	public const string BASE_VERSION = "v1.0.1.14 2017-06-13";
	
	public static ArtikFlowBase instance;
	
	[HideInInspector]
	public UnityEvent eventSplashHidden = new UnityEvent();

	public ArtikFlowBaseConfiguration configuration { get; private set; }   // Configuration file
	ArtikFlowGamemode artikFlowGamemode;

	void Awake()
	{
		if (instance != null)
			throw new Exception("[ERROR] There is more than one ArtikFlowBase instance in the game!");

		instance = this;

		ConfigurationReference reference = GameObject.FindObjectOfType<ConfigurationReference>();
		if (reference == null || reference.configuration == null)
			throw new Exception("[ERROR] No ArtikFlowBaseConfiguration is set in the 'Configuration' GameObject.");
		else
			configuration = reference.configuration;
	}

	IEnumerator Start()
	{
		SplashScreen.instance.showSplash();
		AsyncOperation async;

		// Unload additional scenes
#if UNITY_EDITOR
		List<string> scenesToUnload = new List<string>();
        for (int i = 0; i < SceneManager.sceneCount; i ++)
		{
			if(SceneManager.GetSceneAt(i).name != "ArtikFlowBase")
				scenesToUnload.Add(SceneManager.GetSceneAt(i).name);
		}

		foreach(string s in scenesToUnload)
		{
			async = SceneManager.UnloadSceneAsync(s);
			yield return async;
		}
#endif

		// Load the correct ArtikFlowGamemode scene depending on the gotten configuration file
		string sceneName = configuration.GetType().Name;
		sceneName = sceneName.Substring(0, sceneName.Length - "Configuration".Length);
		print("[INFO] Attempting to load scene: " + sceneName + "...");
		async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		yield return async;

		artikFlowGamemode = GameObject.FindObjectOfType<ArtikFlowGamemode>();
		if(artikFlowGamemode == null)
			throw new Exception("[ERROR] No valid ArtikFlowGamemode has been found in the scene.");

		artikFlowGamemode.eventInitialized.AddListener(() => 
		{
			Application.targetFrameRate = 60;
			SplashScreen.instance.hideSplash(() => {
				eventSplashHidden.Invoke();
			});
		});
	}

}

}