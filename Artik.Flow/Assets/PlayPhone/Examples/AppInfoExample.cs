using UnityEngine;
using System.Collections;

public class AppInfoExample : ExampleScreen 
{
	public override void Draw()
	{
		if (GUILayout.Button("Rate Game"))
		{
			SetStatus("Rate Game is not implemented");
		}
		if (GUILayout.Button("Open Game Page"))
		{
			SetStatus("Open Game Page is not implemented");
		}
	}
}
