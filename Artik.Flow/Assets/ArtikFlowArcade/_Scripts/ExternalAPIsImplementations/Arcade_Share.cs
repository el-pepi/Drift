using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AFBase;
using System.IO;

namespace AFArcade {

public class Arcade_Share : MonoBehaviour 
{
	public static Arcade_Share instance;
	private string text;
	
	void Awake()
	{
		instance = this;
	}

	void Start () 
	{
		text = ArtikFlowArcade.instance.configuration.sharingText;
		text = text.Replace("*", Application.productName.Replace(" ", ""));
	}

	// -----

	// Twitter

	public void shareTwitterScore()
	{
		string formattedText = text.Replace("%", ArtikFlowArcade.instance.getScore().ToString());
		Share.instance.shareTwitterText(formattedText);
	}

	public void shareTwitterImageScore(Texture2D image)
	{
		string formattedText = text.Replace("%", ArtikFlowArcade.instance.getScore().ToString());
		Share.instance.shareTwitterImage(image, formattedText);
	}

	public void shareTwitterScreenshotScore()
	{
		string formattedText = text.Replace("%", ArtikFlowArcade.instance.getScore().ToString());
		Share.instance.shareTwitterScreenshot(formattedText);
	}

	// Facebook

	public void shareFacebookImageScore(Texture2D image)
	{
		string formattedText = text.Replace("%", ArtikFlowArcade.instance.getScore().ToString());
		Share.instance.shareFacebookImage(image);
	}

	public void shareFacebookScreenshotScore()
	{
		string formattedText = text.Replace("%", ArtikFlowArcade.instance.getScore().ToString());
		Share.instance.shareFacebookScreenshot();
	}

	// Native

	public void shareNativeScore()
	{
		string formattedText = text.Replace("%", ArtikFlowArcade.instance.getScore().ToString());
		Share.instance.shareNativeText(formattedText);
	}

	public void shareNativeImageScore(Texture2D image)
	{
		string formattedText = text.Replace("%", ArtikFlowArcade.instance.getScore().ToString());



			#if UNITY_ANDROID
			Texture2D screenTexture = new Texture2D(1080, 1080,TextureFormat.RGB24,true);
			screenTexture.Apply();

			byte[] dataToSave = image.EncodeToPNG();

			string destination = Path.Combine(Application.persistentDataPath,"screen" + ".png");
			Debug.Log(destination);
			File.WriteAllBytes(destination, dataToSave);

			AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
			AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
			intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
			AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
			AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse","file://" + destination);
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), formattedText);
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "Share");
			intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
			AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

			currentActivity.Call("startActivity", intentObject);
			#else

		Share.instance.shareNativeImage(image, formattedText);
			#endif
	}

	public void shareNativeScreenshotScore()
	{
		string formattedText = text.Replace("%", ArtikFlowArcade.instance.getScore().ToString());
		Share.instance.shareNativeScreenshot(formattedText);
	}

}

}