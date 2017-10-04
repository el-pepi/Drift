using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;  
using System.Xml;
using System.Xml.Serialization;

public class vg_moregames : MonoBehaviour {
	public Material tempmat;
	private string blink;
	private string spotid;
	private string adid;
	Texture2D bannerimg;
	public Image instImage;
	public GameObject instcanvas;
	// Use this for initialization
	void Start () {
		StartCoroutine(loadbanner());
		
	}
	
	private bool isAppInstalled(string bundleID) {
		#if UNITY_EDITOR || !UNITY_EDITOR
		return false;
		#endif
		#if UNITY_ANDROID
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

	void OnClick ()
	{
		print ("click");
		clicked();
	}

	public void clicked() {
		StartCoroutine(makeclick());
	}
	
	
	public void closenow() {
		StartCoroutine(removenowdelay());
	}
	
	IEnumerator removenowdelay()
	{
		yield return new WaitForSeconds(0.05f);
//		instcanvas.SetActive(false);
		Destroy(gameObject);
		
	}
	
	IEnumerator makeclick()
	{
		
		WWW www = new WWW(vg_interstitial.houseadslinkbanner + "/loadbanner.php?clicked=1&spotid=" + spotid + "&adid=" + adid + "&bid=" + vg_interstitial.GBundleId + "&deviceid=" + DeviceUniqueIdentifier.get());
		yield return www;
		print ("click " + blink);
		Application.OpenURL(blink);
		StartCoroutine(loadbanner(1));
		
		
	}
	
	IEnumerator impressionok()
	{
		
		WWW www = new WWW(vg_interstitial.houseadslinkbanner + "/loadbanner.php?impressieok=1&spotid=" + spotid + "&adid=" + adid + "&bid=" + vg_interstitial.GBundleId + "&deviceid=" + DeviceUniqueIdentifier.get());
		yield return www;
	}
	
	public IEnumerator loadbanner(int start = 0)
	{
		Debug.Log("Load banner for " + vg_interstitial.GBundleId);
		WWW www = new WWW(vg_interstitial.houseadslinkbanner + "/loadbanner.php?load=" + start + "&bid=" + vg_interstitial.GBundleId + "&deviceid=" + DeviceUniqueIdentifier.get());
		yield return www;
		
		XmlDocument	doc= new XmlDocument();
		doc.LoadXml(www.text);
		XmlNodeList bannerinfo = doc.SelectNodes("banner");
		if(bannerinfo[0].SelectSingleNode("packed").InnerText == "zero") {
			//StartCoroutine(installcheck());
		}
		else if(!isAppInstalled(bannerinfo[0].SelectSingleNode("packed").InnerText)) {
			
			WWW wwwimg = new WWW(bannerinfo[0].SelectSingleNode("image").InnerText);
			bannerimg = new Texture2D(300, 250, TextureFormat.RGB24, false);
			
			yield return wwwimg;
			//www.LoadImageIntoTexture(bannerimg);
			wwwimg.LoadImageIntoTexture(bannerimg);
			Sprite imagespr = Sprite.Create(bannerimg, new Rect(0, 0, bannerimg.width, bannerimg.height), new Vector2(0.5f, 0.5f));
			
			instImage.sprite = imagespr;
			//instcanvas.SetActive(true);
		
			spotid = bannerinfo[0].SelectSingleNode("spotid").InnerText;
			adid = bannerinfo[0].SelectSingleNode("adid").InnerText;
			blink = bannerinfo[0].SelectSingleNode("link").InnerText;
			
			System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
			int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
			PlayerPrefs.SetInt("vginstshowtimeout" , cur_time + (int.Parse(bannerinfo[0].SelectSingleNode("showtimeout").InnerText) * 60));
			
			
			
			
			//showtimeout
			StartCoroutine(impressionok());
			//tempmat.mainTexture = bannerimg;
			
		} else {
			yield return new WaitForSeconds(1);
			StartCoroutine(loadbanner(start + 1));
		}
		
	}
}
