using UnityEngine;
using System.Collections;

public class ExamplesMenu : MonoBehaviour
{
	// Example Demo Secret Key          f068097e5ecd1271d34ce1d59773176f8a85ec4d
	// Example Demo Bundle Identifier   com.playphone.sdk2sample 
	public string Status { get; set; }

	private MyPlayExample myPlayExample;
	private BillingExample billingExample;
	private PlayerDataExample playerDataExample;
	private AchievementsExample achievementsExample;
	private LeaderboardsExample leaderboardsExample;
	private LicenseExample licenseExample;
	private ExpansionsExample expansionsExample;
	private ExampleScreen currentScreen;

	public GUISkin customSkin;

	void Awake()
	{
		myPlayExample = GetComponent<MyPlayExample>();
		billingExample = GetComponent<BillingExample>();
		playerDataExample = GetComponent<PlayerDataExample>();
		leaderboardsExample = GetComponent<LeaderboardsExample>();
		achievementsExample = GetComponent<AchievementsExample>();
		licenseExample = GetComponent<LicenseExample>();
		expansionsExample = GetComponent<ExpansionsExample>();
	}



	/// <summary>
	/// /*/*/*Required Initialization Code.*/*/*/
	/// </summary>
	void Start ()
	{
		Status = "Initializing...";
		PlayPhone.Plugin.IncrementTracking();
		PlayPhone.Plugin.OnInit += () => 
		{
			Status = "Successfuly initialized";
			PlayPhone.Plugin.ShowIcon();
			PlayPhone.Plugin.GetLaunchScreen();
		};
		PlayPhone.Plugin.OnInitError += (error) =>
		{
			Status = "Initialization failed: " + error;
		};

		PlayPhone.Plugin.OnLaunchScreen += (screen) =>
		{   
			if (PlayPhone.Consts.PSGN_LAUNCH_SCREEN_OFFERS == screen) {
				currentScreen = billingExample;
			}
		};

		
		PlayPhone.Plugin.Init();
		PlayPhone.Plugin.DoLaunchAction(false);
	}

	void OnApplicationQuit() {
		PlayPhone.Plugin.DecrementTracking();
	}	
	/// <summary>
	/// /*/*/*Required Initialization Code.*/*/*/
	/// </summary>
	/// 

	void OnGUI()
	{
		GUI.skin = customSkin;
		float dh = (Screen.height - 20) / 12;
		GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));
		GUI.skin.button.fixedHeight = dh;
		GUI.skin.button.fixedWidth = Screen.width - 20;
		GUI.skin.textField.wordWrap = true;
		GUI.skin.textField.fixedWidth = Screen.width - 20;
		GUI.skin.textField.fixedHeight = dh * 0.6f;
		GUI.skin.label.alignment = TextAnchor.MiddleLeft;

		GUILayout.Label ("Status: " + Status, GUILayout.MaxHeight(dh * 0.7f));

		if (currentScreen == null)
		{
			if (GUILayout.Button("Billing")) 
			{
				currentScreen = billingExample;
			}
			if (GUILayout.Button("DataStorage")) 
			{
				currentScreen = playerDataExample;
			}
			if (GUILayout.Button("Leaderboards")) 
			{
				currentScreen = leaderboardsExample;
			}
			if (GUILayout.Button("Achievements")) 
			{
				currentScreen = achievementsExample;
			}
			if (GUILayout.Button("License")) 
			{
				currentScreen = licenseExample;
			}
			if (GUILayout.Button("Expansions")) 
			{
				currentScreen = expansionsExample;
			}

			if (GUILayout.Button("Exit"))
			{
				Application.Quit();
			}
		} 
		else 
		{
			currentScreen.Draw();

			if (GUILayout.Button("Back")) 
			{
				currentScreen = null;
			}
		}

		GUILayout.EndArea();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape)) 
		{
			currentScreen = null;
		}
	}
}
