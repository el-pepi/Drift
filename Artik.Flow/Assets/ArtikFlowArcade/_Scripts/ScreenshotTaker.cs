using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace AFArcade {

public class ScreenshotTaker : MonoBehaviour
{
	public delegate void RawScreenshotTakenListener();

	public static ScreenshotTaker instance;

	Camera captureCamera;

	Texture2D rawScreenshotImage;
	Texture2D processedScreenshot;
	RawImage canvasScreenshotTexture;
	Text scoreText;

	public enum ScreenshotState	{
		REQUESTED,
		RAW_TAKEN,
		PROCESSED,
	}
	ScreenshotState screenshotState = ScreenshotState.PROCESSED;

	void Awake()
	{
		instance = this;

		captureCamera = GetComponent<Camera>();
		Invoke("initialize", 0.5f);
	}

	void Start()
	{
		scoreText = transform.Find("Canvas").Find("Text_Score").GetComponent<Text>();
		canvasScreenshotTexture = transform.Find("Canvas").Find("ScreenshotTexture").GetComponent<RawImage>();

		ArtikFlowArcade.instance.eventScoreUpdate.AddListener(onScoreUpdate);
	}

	void initialize()
	{
		// Set logo
		Texture2D logo = ArtikFlowArcade.instance.configuration.gameLogoTexture;
        Sprite spriteLogo = Sprite.Create(logo as Texture2D, new Rect(0f, 0f, logo.width, logo.height), Vector2.zero);
		Image canvasImage = transform.Find("Canvas").Find("ImageLogo").GetComponent<Image>();
		canvasImage.sprite = spriteLogo;
		canvasImage.GetComponent<RectTransform>().sizeDelta = new Vector2(
			canvasImage.GetComponent<RectTransform>().sizeDelta.x,
			((float) logo.height / (float) logo.width) * canvasImage.GetComponent<RectTransform>().sizeDelta.x
		);

		Texture2D icon = ArtikFlowArcade.instance.configuration.icon;
		Sprite spriteIcon = Sprite.Create(icon as Texture2D, new Rect(0f, 0f, icon.width, icon.height), Vector2.zero);
		transform.Find("Canvas").Find("Icon").GetComponent<Image>().sprite = spriteIcon;
		transform.Find("Canvas").Find("Icon").GetComponent<Image>().color = Color.white;

		transform.Find("Canvas").Find("Text_BeatMyScore").GetComponent<Text>().text = Language.get("Screenshot.BeatMyScore");

		captureCamera.enabled = false;
	}

	void onScoreUpdate(int score)
	{
		scoreText.text = "" + score;
	}

	/*
	void OnPostRender()
	{
		if(screenshotState == ScreenshotState.REQUESTED)		// Step 1
		{
			RenderTexture original = RenderTexture.active;
			RenderTexture.active = null;

			rawScreenshotImage = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
			rawScreenshotImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
			rawScreenshotImage.Apply();

			canvasScreenshotTexture.texture = rawScreenshotImage;
			canvasScreenshotTexture.SetNativeSize();
			float ratio = canvasScreenshotTexture.rectTransform.sizeDelta.y / canvasScreenshotTexture.rectTransform.sizeDelta.x;
			canvasScreenshotTexture.rectTransform.sizeDelta = new Vector2(512, 512 * ratio);

			RenderTexture.active = original;

			screenshotState = ScreenshotState.RAW_TAKEN;
		}
		else if(screenshotState == ScreenshotState.RAW_TAKEN)	// Step 2
		{
			RenderTexture original = RenderTexture.active;

			captureCamera.enabled = true;
			captureCamera.Render();
			RenderTexture.active = captureCamera.targetTexture;
			processedScreenshot = new Texture2D(RenderTexture.active.width, RenderTexture.active.height, TextureFormat.RGB24, false);
			processedScreenshot.ReadPixels(new Rect(0, 0, RenderTexture.active.width, RenderTexture.active.height), 0, 0);
			processedScreenshot.Apply();

			captureCamera.enabled = false;
			RenderTexture.active = original;

			screenshotState = ScreenshotState.PROCESSED;
		}
	}
	*/

	/*
	Texture2D processImage()
	{
		captureCamera.enabled = true;

		RenderTexture.active = captureCamera.targetTexture;

		Texture2D image = new Texture2D(RenderTexture.active.width, RenderTexture.active.height, TextureFormat.RGB24, false);
		image.ReadPixels(new Rect(0, 0, RenderTexture.active.width, RenderTexture.active.height), 0, 0);
		image.Apply();

		captureCamera.enabled = false;

		return image;
	}
	*/

	// API

	public void takeScreenshot(RawScreenshotTakenListener listener = null)
	{
		/*
		captureCamera.clearFlags = gameCamera.clearFlags;
		captureCamera.backgroundColor = new Color(gameCamera.backgroundColor.r, gameCamera.backgroundColor.g, gameCamera.backgroundColor.b, 1f);
		captureCamera.cullingMask = gameCamera.cullingMask | LayerMask.NameToLayer("ScreenshotUI");
		captureCamera.orthographic = gameCamera.orthographic;

		captureCamera.transform.position = gameCamera.transform.position;
		captureCamera.transform.rotation = gameCamera.transform.rotation;

		captureCamera.enabled = true;
		*/

		screenshotState = ScreenshotState.REQUESTED;
		StartCoroutine(doTheScreenie(listener));
	}

	IEnumerator doTheScreenie(RawScreenshotTakenListener listener)
	{
		RenderTexture raw_screen_rt = new RenderTexture(512, 512, 24);

		yield return new WaitForEndOfFrame();

		// Step 1: Take the raw screenshot

		RenderTexture original = RenderTexture.active;
		rawScreenshotImage = new Texture2D(512, 512, TextureFormat.RGB24, false);	// to-do: Optimize to read only a square in the center
		
		foreach(Camera cam in Camera.allCameras)
		{
			if (cam.cullingMask == 1 << LayerMask.NameToLayer("ArtikFlowUI") || cam.targetTexture != null)
				continue;

			cam.targetTexture = raw_screen_rt;
			cam.Render();
			cam.targetTexture = null;
		}

		RenderTexture.active = raw_screen_rt;
		rawScreenshotImage.ReadPixels(new Rect(0, 0, 512, 512), 0, 0);
		rawScreenshotImage.Apply();

		if (listener != null)
			listener();

		canvasScreenshotTexture.texture = rawScreenshotImage;
		canvasScreenshotTexture.rectTransform.sizeDelta = new Vector2(512, 512);
		
		screenshotState = ScreenshotState.RAW_TAKEN;

		// Step 2: Embed it into the Canvas, and take a second screenie there
		
		captureCamera.enabled = true;
		captureCamera.targetTexture.DiscardContents();
		RenderTexture.active = captureCamera.targetTexture;
		captureCamera.Render();

		processedScreenshot = new Texture2D(RenderTexture.active.width, RenderTexture.active.height, TextureFormat.RGB24, false);
		processedScreenshot.ReadPixels(new Rect(0, 0, RenderTexture.active.width, RenderTexture.active.height), 0, 0);
		processedScreenshot.Apply();

		captureCamera.enabled = false;
		RenderTexture.active = original;

		screenshotState = ScreenshotState.PROCESSED;
	}

	/// <summary>
	/// WARNING: Don't call inmediatly after takeScreenshot(). Wait a frame or two at least.
	/// </summary>
	/// <returns>The last screenshot taken</returns>
	public Texture2D getLastScreenshot()
	{
		if (screenshotState == ScreenshotState.PROCESSED)
			return processedScreenshot;
		else
		{
			Debug.Log("WARNING: Screenshot requested but not yet ready! Wait a few frames before calling this function.");
			return null;
		}
	}

}

}