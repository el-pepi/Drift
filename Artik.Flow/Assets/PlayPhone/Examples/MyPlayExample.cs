using UnityEngine;
using System;

public class MyPlayExample : ExampleScreen
{
	public override void Draw()
	{
		if (GUILayout.Button("Unlock achievement"))
		{
			var achievementId = "5";
			PlayPhone.MyPlay.UnlockAchievement (achievementId);

			SetStatus("Achievement unlocked");
		}
		if (GUILayout.Button("Submit score"))
		{
			var leaderboardId = "3";
			var score = 42;
			PlayPhone.MyPlay.SubmitScore (leaderboardId, score);

			SetStatus("Score submited");
		}
	}
}
