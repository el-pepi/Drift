using UnityEngine;
using System.Collections;
using PlayPhone;

public class LicenseExample : ExampleScreen 
{
	void Start () 
	{
		License.OnSuccess += () => {
			SetStatus("Success");
		};
		License.OnError += (error) => {
			SetStatus("Error: " + error);
		};
	}

	public override void Draw()
	{
		if (GUILayout.Button("Check License"))
		{
			SetStatus("Checking...");
			License.Check();
		}
	}
}
