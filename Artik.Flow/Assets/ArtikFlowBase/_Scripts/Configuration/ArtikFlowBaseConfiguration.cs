using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AFBase {

public class ArtikFlowBaseConfiguration : ScriptableObject 
{
	[HideInInspector]
	public string ARTIK_HEYZAP_ID = "f12890a3baaffe96f7417c7cb922e0f2";		// Hardcoded, it's the same in every game

	public enum StoreTarget
	{
		DEFAULT_STORE,
		PLAYPHONE,
		FRENCH_PREMIUM,
	}

	[Header("ArtikFlow Base")]
	[Tooltip("The name of the game main scene")]
	public string gameScene = "main";

	[Header("Build Target")]
	[Tooltip("Sets the build target")]
	public StoreTarget storeTarget = StoreTarget.DEFAULT_STORE;
	[Tooltip("In case of FRENCH_PREMIUM build, activate the Try&Buy system")]
	public bool FRENCH_PREMIUM_tryNBuy = false;
	[Tooltip("Try N Buy ID")]
	public string FRENCH_PREMIUM_tryNBuyId;

	[Header("Game Services")]
	[Tooltip("Leaderboard IDs for Google Play Games")]
	public string[] GoogleLeaderboards = new string[] {
		"CgkImLDfp70HEAIQAQ",
	};
	[Tooltip("Leaderboard IDs for Game Center")]
	public string[] iOSLeaderboards = new string[] {
		"artikflow.ranking",
	};
	[Tooltip("Leaderboard IDs for Playphone")]
	public string[] playphoneLeaderboards = new string[] {
		"11000",
	};
	[Tooltip("Achievement IDs for Google Play Games")]
	public string[] GoogleAchievements = new string[] {
		"CgkImLDfp70HEAIQAg",
		"CgkImLDfp70HEAIQAw",
		"CgkImLDfp70HEAIQBA",
		"CgkImLDfp70HEAIQBQ",
		"CgkImLDfp70HEAIQBg",
	};
	[Tooltip("Achievement IDs for Game Center")]
	public string[] iOSAchievements = new string[] {
		"artikflow.achievement1",
		"artikflow.achievement2",
		"artikflow.achievement3",
		"artikflow.achievement4",
		"artikflow.achievement5",
	};
	[Tooltip("Achievement IDs for Playphone")]
	public string[] playphoneAchievements = new string[] {
		"12000",
		"12001",
		"12002",
		"12003",
		"12004",
	};

	[Header("FlowButtons")]
	[Tooltip("URL to forward in the rate panel, Android. % gets replaced with bundle id.")]
	public string Android_StarUrl = "https://play.google.com/store/apps/details?id=%";
	[Tooltip("URL to forward in the rate panel, Playphone. % gets replaced with bundle id.")]
	public string Playphone_StarUrl = "psgn://getgames.info/?jump_to=gamedetails&id=16543";
	[Tooltip("URL to forward in the rate panel, iOS. % gets replaced with bundle id.")]
	public string iOS_StarUrl = "https://itunes.apple.com/app/id%";
	[Tooltip("ID of the app in the itunes store.")]
	public string iOS_StoreId;

	// [Header("Language")]
	[Tooltip("The reference font to be used everywhere by NGUI")]
	public UIFont[] referenceFonts;
	[Tooltip("The NGUI bitmap font to be used if there aren't strange characters")]
	public UIFont[] bitmapFonts;
	[Tooltip("The NGUI truetype font to be used if the language contains strange characters")]
	public UIFont[] dynamicFonts;

	[Header("IAPs")]
	[Tooltip("Products purchasable from in-game, 'noads' product is used by ArtikFlow")]
	public ArtikProduct[] products;

	[Header("Language")]
	[Tooltip("Force a language to be used. Keep empty to ignore this setting. Use the language code.")]
	public string forceLanguage = "";

	[Header("Google Analytics")]
	public string androidTrackingCode;
	public string iosTrackingCode;

	[Header("Permissions")]
	public bool forceStoragePermission = false;

	[Header("Cache Manager")]
	public bool cleanOnApplicationPause = false;
	public bool cleanOnApplicationQuit = false;

	// --- Methods to call from custom inspector ---

#if UNITY_EDITOR

	// Store update

	public void storeUpdated()		
	{
		PluginImporter[] importers = PluginImporter.GetAllImporters();

		foreach(PluginImporter plugin in importers)
		{
			if(plugin.assetPath.Contains("playphone") || plugin.assetPath.Contains("PSGN"))
				plugin.SetCompatibleWithPlatform(BuildTarget.Android, (storeTarget == StoreTarget.PLAYPHONE) );

			if (plugin.assetPath.Contains("pxinapp"))
				plugin.SetCompatibleWithPlatform(BuildTarget.Android, (storeTarget == StoreTarget.FRENCH_PREMIUM && FRENCH_PREMIUM_tryNBuy));
		}

	}

	// Storage permission update

	public void storagePermissionUpdated()
	{
		Debug.Log("Updating manifest files...");
		updateManifestsRecursively(Application.dataPath + "/Plugins/Android/");
		Debug.Log("Finished!");
	}

	void updateManifestsRecursively(string path)
	{
		foreach(string directory in Directory.GetDirectories(path))
			updateManifestsRecursively(directory);

		foreach(string file in Directory.GetFiles(path))
		{
			if(file.EndsWith("/AndroidManifest.xml") || file.EndsWith("\\AndroidManifest.xml"))
				updateManifestFile(file);
		}

	}

	void updateManifestFile(string filePath)
	{
		string noPermission_line = "EXTERNAL_STORAGE\" android:maxSdkVersion=\"18\" />";
		string permission_line = "EXTERNAL_STORAGE\" />";

		string[] lines = File.ReadAllLines(filePath);
		for(int i = 0; i < lines.Length; i ++)
		{
			if(forceStoragePermission && lines[i].Contains(noPermission_line))
			{
				Debug.Log("---- File: " + filePath);
				Debug.Log("Fr: " + lines[i]);
				lines[i] = lines[i].Replace(noPermission_line, permission_line);
				Debug.Log("To: " + lines[i]);
			}
			else if(!forceStoragePermission && lines[i].Contains(permission_line))
			{
				Debug.Log("---- File: " + filePath);
				Debug.Log("Fr: " + lines[i]);
				lines[i] = lines[i].Replace(permission_line, noPermission_line);
				Debug.Log("To: " + lines[i]);
			}
				
		}
		File.WriteAllLines(filePath, lines);
	}

#endif

}

}