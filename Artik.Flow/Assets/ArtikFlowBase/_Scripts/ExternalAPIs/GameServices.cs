using UnityEngine;
using System.Collections;
using GooglePlayGames.BasicApi;
using UnityEngine.Events;

namespace AFBase {
	
public class GameServices : MonoBehaviour
{
	public static GameServices instance;

	public class BoolEvent : UnityEvent<bool> { };
	[HideInInspector]
	public BoolEvent eventAuthFinish = new BoolEvent();

	public enum POSTLOGIN_ACTION {
		NONE,
		SHOW_LEADERBOARDS,
		SHOW_ACHIEVEMENTS,
	}
	private POSTLOGIN_ACTION postloginAction;

	private string[] GoogleLeaderboards;		// ArtikFlowConfiguration
	private string[] iOSLeaderboards;			// ArtikFlowConfiguration
	private string[] playphoneLeaderboards;     // ArtikFlowConfiguration
	private string[] GoogleAchievements;		// ArtikFlowConfiguration
	private string[] iOSAchievements;			// ArtikFlowConfiguration
	private string[] playphoneAchievements;     // ArtikFlowConfiguration

	void Awake()
	{
		instance = this;
	}

	void Start ()
	{
		GoogleLeaderboards = ArtikFlowBase.instance.configuration.GoogleLeaderboards;
		iOSLeaderboards = ArtikFlowBase.instance.configuration.iOSLeaderboards;
		playphoneLeaderboards = ArtikFlowBase.instance.configuration.playphoneLeaderboards;
		GoogleAchievements = ArtikFlowBase.instance.configuration.GoogleAchievements;
		iOSAchievements = ArtikFlowBase.instance.configuration.iOSAchievements;
		playphoneAchievements = ArtikFlowBase.instance.configuration.playphoneAchievements;

		Invoke("tryAuthenticate", 0.5f);
	}

	void tryAuthenticate()
	{
		if(ArtikFlowBase.instance.configuration.storeTarget == ArtikFlowBaseConfiguration.StoreTarget.DEFAULT_STORE)
		{
			postloginAction = POSTLOGIN_ACTION.NONE;

#if UNITY_ANDROID
			PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
				// enables saving game progress.
				.EnableSavedGames()
				.Build();

			GooglePlayGames.PlayGamesPlatform.InitializeInstance(config);
			// recommended for debugging:
			GooglePlayGames.PlayGamesPlatform.DebugLogEnabled = true;
			// Activate the Google Play Games platform
			GooglePlayGames.PlayGamesPlatform.Activate();

			bool silent = PlayerPrefs.GetInt("GPGS_FirstLogin", 0) == 1;
			GooglePlayGames.PlayGamesPlatform.Instance.Authenticate(onLogin, silent);
			PlayerPrefs.SetInt("GPGS_FirstLogin", 1);
#else
			Social.localUser.Authenticate(onLogin);
#endif
		}
		else if(ArtikFlowBase.instance.configuration.storeTarget == ArtikFlowBaseConfiguration.StoreTarget.PLAYPHONE)
		{
			// :)
		}
		else
		{
			// :D
		}
	}

	// ------------- Listeners
	void onLogin(bool success)
	{
		Debug.Log(success ? "Authentication successful" : "Authentication failed");

		if(postloginAction == POSTLOGIN_ACTION.SHOW_LEADERBOARDS)
			Social.ShowLeaderboardUI();
		else if(postloginAction == POSTLOGIN_ACTION.SHOW_ACHIEVEMENTS)
			Social.ShowAchievementsUI();

		eventAuthFinish.Invoke(success);
	}

	// ------------- Game services
	public void ReportScore(int boardID, long score)
	{
		if (!gameObject.activeInHierarchy)
			return;

		if(ArtikFlowBase.instance.configuration.storeTarget == ArtikFlowBaseConfiguration.StoreTarget.DEFAULT_STORE)
		{
#if UNITY_ANDROID
			if(boardID < 0 || boardID >= GoogleLeaderboards.Length)
			{
				Debug.LogWarning("[WARNING] Attempting to submit score to invalid leaderboard id: " + boardID);
				return;
			}

			string board = GoogleLeaderboards[boardID];
#elif UNITY_IOS
			if(boardID < 0 || boardID >= iOSLeaderboards.Length)
			{
				Debug.LogWarning("[WARNING] Attempting to submit score to invalid leaderboard id: " + boardID);
				return;
			}

			string board = iOSLeaderboards[boardID];
#endif

			if (Social.localUser.authenticated)
			{
				Social.ReportScore(score, board, success => {
					Debug.Log(success ? "Score reported successfully" : "Failed to report score");
				});
			}
		}
		else if(ArtikFlowBase.instance.configuration.storeTarget == ArtikFlowBaseConfiguration.StoreTarget.PLAYPHONE)
		{
			if(boardID < 0 || boardID >= playphoneLeaderboards.Length)
			{
				Debug.LogWarning("[WARNING] Attempting to submit score to invalid leaderboard id: " + boardID);
				return;
			}

			string board = playphoneLeaderboards[boardID];

			PlayPhone.MyPlay.SubmitScore(board, (int) score);
		}
		else
		{
			// :)
		}

	}

	public void ReportProgress(int achievementID, double progress)
	{
		if (!gameObject.activeInHierarchy)
			return;

		if (ArtikFlowBase.instance.configuration.storeTarget == ArtikFlowBaseConfiguration.StoreTarget.DEFAULT_STORE)
		{
#if UNITY_ANDROID
			string achievement = GoogleAchievements[achievementID];
#elif UNITY_IOS
			string achievement = iOSAchievements[achievementID];
#endif

			if (Social.localUser.authenticated)
			{
				Social.ReportProgress(achievement, progress, success =>
				{
					Debug.Log(success ? "Achievement progress reported successfully" : "Failed to report achievement progress");
				});
			}
		}
		else if (ArtikFlowBase.instance.configuration.storeTarget == ArtikFlowBaseConfiguration.StoreTarget.PLAYPHONE)
		{
			string achievement = playphoneAchievements[achievementID];

			PlayPhone.MyPlay.UnlockAchievement(achievement);
		}
		else
		{
			// :)
		}

    }

	public void ShowLeaderboardUI()
	{
		if (!gameObject.activeInHierarchy)
			return;

		if (ArtikFlowBase.instance.configuration.storeTarget == ArtikFlowBaseConfiguration.StoreTarget.DEFAULT_STORE)
		{
			if (Social.localUser.authenticated)
			{
				Social.ShowLeaderboardUI();
			}
			else
			{
				postloginAction = POSTLOGIN_ACTION.SHOW_LEADERBOARDS;
				Social.localUser.Authenticate(onLogin);
			}
		}
		else if (ArtikFlowBase.instance.configuration.storeTarget == ArtikFlowBaseConfiguration.StoreTarget.PLAYPHONE)
		{
			PlayPhone.MyPlay.ShowDashboard();
		}
		else
		{
			// :(
		}
					
	}

	public void ShowAchievementsUI()
	{
		if (!gameObject.activeInHierarchy)
			return;

		if (ArtikFlowBase.instance.configuration.storeTarget == ArtikFlowBaseConfiguration.StoreTarget.DEFAULT_STORE)
		{
			if (Social.localUser.authenticated)
			{
				Social.ShowAchievementsUI();
			}
			else
			{
				postloginAction = POSTLOGIN_ACTION.SHOW_ACHIEVEMENTS;
				Social.localUser.Authenticate(onLogin);
			}
		}
		else if (ArtikFlowBase.instance.configuration.storeTarget == ArtikFlowBaseConfiguration.StoreTarget.PLAYPHONE)
		{
			PlayPhone.MyPlay.ShowDashboard();
		}
		else
		{
			// :(
		}

	}

}

}