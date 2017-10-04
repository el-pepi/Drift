using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;  
using System.Xml;
using System.Xml.Serialization;

public class vg_owninterstitial : MonoBehaviour {
	Texture2D bannerimg;
	public Image instImage;
	public GameObject instcanvas;
	private string blink;
	private string spotid;
	private string adid;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(gameObject);
	//	instcanvas.SetActive(false);

		if(Screen.orientation == ScreenOrientation.Landscape) {
			instImage.rectTransform.sizeDelta = new Vector2(1024 * 0.96f,768 * 0.96f);
			instcanvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1024,768);
			//instcanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
			//0.62
		} else {
			instImage.rectTransform.sizeDelta = new Vector2(768 * 0.96f,1024 * 0.96f);
			instcanvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(768,1024);
			//instcanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
		}
		instcanvas.SetActive(false);

		StartCoroutine(loadbanner());
	}

	private bool isAppInstalled(string bundleID) {
		#if UNITY_EDITOR
		return false;
		#elif UNITY_IOS
		return false;
		#elif UNITY_ANDROID
		AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");
		Debug.Log(" ********LaunchOtherApp " + bundleID);
		AndroidJavaObject launchIntent = null;
		//if the app is installed, no errors. Else, doesn't get past next line
		try{
			launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage",bundleID);
			//        
			//        ca.Call("startActivity",launchIntent);
		}catch(Exception ex){
			Debug.Log("exception"+ex.Message);
		}
		if(launchIntent == null)
			return false;
		return true;
		#endif
	}
	
	public void clicked() {
		print ("click");
		StartCoroutine(makeclick());
	}


	public void closenow() {
		Time.timeScale = 1f;
		StartCoroutine(removenowdelay());
	}

	IEnumerator removenowdelay()
	{
		yield return new WaitForSeconds(0.05f);
		instcanvas.SetActive(false);
		Destroy(gameObject);

	}

	IEnumerator makeclick()
	{
		
		WWW www = new WWW(vg_interstitial.houseadslink + "/loadbannerown.php?clicked=1&spotid=" + spotid + "&bid=" + vg_interstitial.GBundleId + "&deviceid=" + DeviceUniqueIdentifier.get());
		yield return www;
		
		Application.OpenURL(blink);
		closenow();
		
		
	}

	IEnumerator installcheck()
	{
		
		WWW www = new WWW(vg_interstitial.houseadslink + "/loadbannerown.php?installcheck=1&spotid=" + spotid + "&bid=" + vg_interstitial.GBundleId + "&deviceid=" + DeviceUniqueIdentifier.get());
		yield return www;
		closenow();
	}

	IEnumerator impressionok()
	{
		
		WWW www = new WWW(vg_interstitial.houseadslink + "/loadbannerown.php?impressieok=1&spotid=" + spotid + "&bid=" + vg_interstitial.GBundleId + "&deviceid=" + DeviceUniqueIdentifier.get());
		yield return www;
	}
	
	public IEnumerator loadbanner(int start = 0)
	{

		WWW www = new WWW(vg_interstitial.houseadslink + "/loadbannerown.php?load=" + start + "&bid=" + vg_interstitial.GBundleId + "&deviceid=" + DeviceUniqueIdentifier.get());
		yield return www;

		XmlDocument	doc= new XmlDocument();
		doc.LoadXml(www.text);
		XmlNodeList bannerinfo = doc.SelectNodes("banner");
		if(bannerinfo[0].SelectSingleNode("packed").InnerText == "zero") {
			StartCoroutine(installcheck());
		}
		else if(!isAppInstalled(bannerinfo[0].SelectSingleNode("packed").InnerText)) {

			WWW wwwimg = new WWW(bannerinfo[0].SelectSingleNode("image").InnerText);
			bannerimg = new Texture2D(300, 250, TextureFormat.RGB24, false);
			
			yield return wwwimg;
			//www.LoadImageIntoTexture(bannerimg);
			wwwimg.LoadImageIntoTexture(bannerimg);
			Sprite imagespr = Sprite.Create(bannerimg, new Rect(0, 0, bannerimg.width, bannerimg.height), new Vector2(0.5f, 0.5f));

			instImage.sprite = imagespr;
			instcanvas.SetActive(true);
			Time.timeScale = 0f;
			spotid = bannerinfo[0].SelectSingleNode("spotid").InnerText;
			adid = bannerinfo[0].SelectSingleNode("adid").InnerText;
			blink = bannerinfo[0].SelectSingleNode("link").InnerText;

				//showtimeout
			StartCoroutine(impressionok());
			//tempmat.mainTexture = bannerimg;

		} else {
			yield return new WaitForSeconds(0.2f);
			StartCoroutine(loadbanner(start + 1));
		}

	}
}
