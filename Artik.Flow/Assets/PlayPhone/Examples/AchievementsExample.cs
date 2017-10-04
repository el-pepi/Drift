using UnityEngine;
using System;

public class AchievementsExample : ExampleScreen
{
	public override void Draw()
	{
		if (GUILayout.Button("Unlock achievement with Id 5"))
		{
			var achievementId = "5";
			PlayPhone.MyPlay.UnlockAchievement (achievementId);
			
			SetStatus("Achievement unlocked");
		}
		if (GUILayout.Button("Unlock achievement with Id 6"))
		{
			var achievementId = "6";
			PlayPhone.MyPlay.UnlockAchievement (achievementId);
			
			SetStatus("Achievement unlocked");
		}
		if (GUILayout.Button("Unlock achievement with Id 7"))
		{
			var achievementId = "7";
			PlayPhone.MyPlay.UnlockAchievement (achievementId);
			
			SetStatus("Achievement unlocked");
		}
	}
}
