using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DriftScreenshotTaker : MonoBehaviour {

	public static DriftScreenshotTaker instance;

	public RenderTexture renderTexture;
	public Text beatMyScoreText;
	public Text scoreText;
	//public Texture2D teee;

	//public byte[] bi;
	Texture2D texture;

	void Start () {
		instance = this;
		texture = new Texture2D(512,512, TextureFormat.ARGB32,false);
		beatMyScoreText.text = Language.get ("Screenshot.BeatMyScore");
	}

	public Texture2D GetPic(){
		//return teee;
		scoreText.text = AFArcade.ArtikFlowArcade.instance.getScore ().ToString();
		RenderTexture.active = GetComponent<Camera>().targetTexture;
		//Render();
		GetComponent<Camera>().Render();
		texture.ReadPixels(new Rect(0,0,512,512),0,0);
		texture.Apply();
		//bi = texture.EncodeToPNG();
		return texture;
	}
}
