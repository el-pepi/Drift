using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlayPhone
{
	public static class MyPlay
	{
		public static void UnlockAchievement(string achievementId)
		{
			var dict = new Dictionary<string, object>()
			{ 
				{ Consts.HASH_VALUES_ACHIEVEMENT_ID, achievementId} 
			};
			Plugin.DoAction (Consts.PSGN_ADD_ACHIEVEMENT, dict);
		}

		public static void SubmitScore(string leaderboardId, int score)
		{
			var dict = new Dictionary<string, object>() 
			{ 
				{ Consts.HASH_VALUES_LEADERBOARD_ID, leaderboardId} ,
				{ Consts.HASH_VALUES_LEADERBOARD_SCORE, score} 
			};
			Plugin.DoAction (Consts.PSGN_ADD_LEADERBOARD_SCORE, dict);
		}

		public static void ShowDashboard()
		{
			Plugin.DoAction (Consts.PSGN_DASHBOARD_PROFILE);
		}
	}
}
