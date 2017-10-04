using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayPhone.MiniJSON;

public class PlayerDataExample : ExampleScreen
{
	private string playerId = string.Empty;
	private string gameLevel = string.Empty;
	private string currentPlayerGuid;

	void Start()
	{
		PlayPhone.PlayerData.OnFriends += (json) => {
			SetStatus("Friends=" + json);
		};
		PlayPhone.PlayerData.OnCurrentPlayerData += (json) => {
			var dict = (Dictionary<string, object>)Json.Deserialize(json);
			currentPlayerGuid = (string)dict["guid"];
			SetStatus("Current=" + json);
		};
		PlayPhone.PlayerData.OnPlayerData += (json) => {
			SetStatus("Player=" + json);
		};
		PlayPhone.PlayerData.OnRemoteGamePlayerData += (json) => {
			SetStatus("RemoteGamePlayer=" + json);
		};


		PlayPhone.Plugin.OnInAppProducts += (json) => {
			SetStatus("OnInAppProducts=" + json);
		};
		PlayPhone.Plugin.OnInAppProductsError += (json) => {
			SetStatus("OnInAppProductsError=" + json);
		};

		PlayPhone.Plugin.OnLeaderboardsData += (json) => {
			SetStatus("LeaderboardsData=" + json);
		};
		PlayPhone.Plugin.OnLeaderboardsDataError += (json) => {
			SetStatus("LeaderboardsData=" + json);
		};
	}

	public override void Draw()
	{
		//
		// Dashboards
		//
		GUILayout.BeginHorizontal();
		var w = GUI.skin.button.fixedWidth;
		GUI.skin.button.fixedWidth = w / 2;

//		if (GUILayout.Button("Show Profile Dashboard"))
//		{
//			PlayPhone.PlayerData.ShowProfileDashboard ();
//		}
//		if (GUILayout.Button("Show Friends Dashboard"))
//		{
//			PlayPhone.PlayerData.ShowFriendsDashboard ();
//		}

		if (GUILayout.Button("Get Leaderboard Data"))
		{
			PlayPhone.Plugin.GetLeaderboardData(3);
		}

		if (GUILayout.Button("Get In App Products Data"))
		{
			PlayPhone.Plugin.GetInAppProductsData();
		}




		GUI.skin.button.fixedWidth = w;
		GUILayout.EndHorizontal();

		GUILayout.Space(10);

		//
		// Player data and firends
		//
		if (GUILayout.Button("Get Current Player Data"))
		{
			SetStatus("Loading current player data...");
			PlayPhone.PlayerData.GetCurrentPlayerData();
		}
		if (GUILayout.Button("Get Friends Data"))
		{
			SetStatus("Loading friends data...");
			PlayPhone.PlayerData.GetFriendsData();
		}
		GUILayout.Space(10);

		//
		// Game data (Cloud storage)
		//
		if (GUILayout.Button("Get Game Player Data"))
		{
			SetStatus("GamePlayerData=" + PlayPhone.PlayerData.GetGamePlayerData());
		}
		if (GUILayout.Button("Get Remote Player Data"))
		{
			SetStatus("Loading remote game player data...");
			PlayPhone.PlayerData.GetRemoteGamePlayerData();
		}

		GUILayout.BeginHorizontal();
		GUILayout.Label("Game level", GUILayout.Width(150));
		gameLevel = GUILayout.TextField(gameLevel);
		GUILayout.EndHorizontal();

		if (GUILayout.Button("Set Game Player Data"))
		{
			var dict = new Dictionary<string, object>() {
				{"level", gameLevel},
			};
			string json = Json.Serialize(dict);
			PlayPhone.PlayerData.SetGamePlayerData(json);
			SetStatus("Successfully set game player data (try to get game player data)");
		}

		GUILayout.Space(10);
	}
}
