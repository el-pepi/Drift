using UnityEngine;
using System.Collections;
using System;

namespace PlayPhone
{
	public static class PlayerData	
	{
		public static event Action OnRemoteData;
		public static event Action OnRemoteDataError;

		public static event Action<string> OnFriends;
		public static event Action<string> OnFriendsError;
		public static event Action<string> OnCurrentPlayerData;
		public static event Action<string> OnCurrentPlayerDataError;
		public static event Action<string> OnPlayerData;
		public static event Action<string> OnPlayerDataError;
		public static event Action<string> OnRemoteGamePlayerData;
		public static event Action<string> OnRemoteGamePlayerDataError;

		public static void ShowProfileDashboard ()
		{
			Plugin.DoAction (Consts.PSGN_DASHBOARD);
		}

		public static void ShowFriendsDashboard ()
		{
			Plugin.DoAction (Consts.PSGN_DASHBOARD_FRIENDS);
		}

		public static void SetGamePlayerData(string json)
		{
			Plugin.SetGamePlayerData(json);
		}

		public static string GetGamePlayerData()
		{
			return Plugin.GetGamePlayerData();
		}

		public static void GetRemoteGamePlayerData()
		{
			Plugin.GetRemoteGamePlayerData();
		}

		public static void GetCurrentPlayerData()
		{
			Plugin.GetCurrentPlayerData();
		}

		public static void GetPlayerData(string playerId)
		{
			Plugin.GetPlayerData(playerId);
		}

		public static void GetFriendsData()
		{
			Plugin.GetFriendsData();
		}

		#region Riase internal methods


		internal static void RaiseOnFriends(string json)
		{
			if (OnFriends != null)
			{
				OnFriends(json);
			}
		}

		internal static void RaiseOnCurrentPlayerData(string json)
		{
			if (OnCurrentPlayerData != null)
			{
				OnCurrentPlayerData(json);
			}
		}

		internal static void RaiseOnPlayerData(string json)
		{
			if (OnPlayerData != null)
			{
				OnPlayerData(json);
			}
		}

		internal static void RaiseOnRemoteGamePlayerData(string json)
		{
			if (OnRemoteGamePlayerData != null)
			{
				OnRemoteGamePlayerData(json);
			}
		}

		internal static void RaiseOnFriendsError(string error)
		{
			if (OnFriendsError != null)
			{
				OnFriendsError(error);
			}
		}

		internal static void RaiseOnCurrentPlayerDataError(string error)
		{
			if (OnCurrentPlayerDataError != null)
			{
				OnCurrentPlayerDataError(error);
			}
		}

		internal static void RaiseOnPlayerDataError(string error)
		{
			if (OnPlayerDataError != null)
			{
				OnPlayerDataError(error);
			}
		}

		internal static void RaiseOnRemoteGamePlayerDataError(string error)
		{
			if (OnRemoteGamePlayerDataError != null)
			{
				OnRemoteGamePlayerDataError(error);
			}
		}

		#endregion
	}
}