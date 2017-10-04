using UnityEngine;
using System.Collections;

public class LeaderboardsExample : ExampleScreen 
{

	public override void Draw()
	{
		if (GUILayout.Button("Submit score"))
		{
			var leaderboardId = "3";
			var score = 42;
			PlayPhone.MyPlay.SubmitScore (leaderboardId, score);
			
			SetStatus("Score submited");
		}
	}
}
