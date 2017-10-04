using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fpsIndicator : MonoBehaviour {
	float deltaTime = 0.0f;
	public Texture2D t;
	GUIStyle style = new GUIStyle();
	Rect rect = new Rect(10, 10, 300, 40);
	
	void Start()
	{
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = 20;
		style.normal.textColor = new Color(1.0f, 0.8f, 0.2f, 1.0f);
		style.fontStyle = FontStyle.Bold;
		style.normal.background = t;
	}

	void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.05f; //adds differences, thus shows tendencies better
		//deltaTime = Time.deltaTime;
	}

	void OnGUI()
	{
		//int w = Screen.width, h = Screen.height;

		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label(rect, text, style);
	}
}
