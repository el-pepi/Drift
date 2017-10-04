using UnityEngine;
using System.Collections;
using VascoGames.Common;
using System;
using UnityEngine.Events;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

namespace VascoGames.MoreGames
{
	public class MoreGamesManager : Singleton<MoreGamesManager>
	{
	    public const bool DefaultSendMoreGamesVersion = true;
	    public const int Landscape = 0;
	    public const int Portrait = 1;
	
	    delegate void docCallback(string message, string data);
	    public string AndroidPublisherID;
	    public string iOSPubliserID;
	
	    public string AndroidMadsAddress = "androidha.vascogames.com";
	    public string iOSMadsAddress = "madsiosnew.vascogames.com";
	
	    public bool UseHWBackButtonToClose = false;
	    public bool UseHWBackButtonToShow = false;
	    public bool AlwaysShowPreloader = false;
	    
	    [Header("If enabled, it sends the moregamesversion=1 to the backend server, if you get an error with incorrect url you can disable this.")]
	    public bool SendMoreGamesVersion = DefaultSendMoreGamesVersion;
	
	    [Header("GameObject References")]
	
	    public GameObject ContainerLandscape;
	    public GameObject ContainerPortrait;
	    public MoreGamesContent[] Layouts;
	    public GameObject PreLoader;
	    public GameObject NoItemsLandscape;
	    public GameObject NoInternetLandscape;
	    public GameObject NoItemsPortrait;
	    public GameObject NoInternetPortrait;
	    public Text[] UpdateMessage;
	    public Texture2D FallbackImg;
	    public Texture2D FallbackIconImg;
	    [SerializeField] CanvasScaler canvasScaler;
	
	    private string bundleID = "";
		private MoreGamesContent currentLayout;
		
		private List<AdBannerData> banners = new List<AdBannerData>();
		private List<AdBannerData> topBanners = new List<AdBannerData>();
		//---------------------------
		private XDocument doc = null;
		
		private bool docIsDownloading = false;
		private bool initCall = true;
		private bool bannersShown = false;
        private bool isShowing = false;
        private ScreenOrientationType currentScreenOrientation;

        [Header("Callbacks")]
	    [SerializeField]
	    public UnityEvent OnShow;
	    [SerializeField]
	    public UnityEvent OnClose;
	    [SerializeField]
		public UnityEvent onDocProcessed;
	    [SerializeField]
		public UnityEvent onDocLoadError;
	
	    public UnityEvent ShowEventHandler
	    {
	        get
	        {
	            if (OnShow == null) OnShow = new UnityEvent();
	            return OnShow;
	        }
	    }
	
	    public UnityEvent CloseEventHandler
	    {
	        get
	        {
	            if (OnClose == null) OnClose = new UnityEvent();
	            return OnClose;
	        }
	    }
	
	    public UnityEvent DocProcessedEventHandler
	    {
	        get
	        {
	            if (onDocProcessed == null) onDocProcessed = new UnityEvent();
	            return onDocProcessed;
	        }
	    }
	
	    public UnityEvent DocLoadErrorEventHandler
	    {
	        get
	        {
	            if (onDocLoadError == null) onDocLoadError = new UnityEvent();
	            return onDocLoadError;
	        }
	    }
		#region Public
	    public void Load()
	    {
	        StartCoroutine(DownloadData(doc, OnDocData));
	    }
	    
		/// <summary>
		/// Loads and displays the banner data.
		/// </summary>
		public void DisplayBanners()
		{
			PreLoader.SetActive(false);
			
			if(currentScreenOrientation == ScreenOrientationType.Landscape)
			{			
				ContainerLandscape.SetActive(true);			
				currentLayout = Layouts[Landscape];
				currentLayout.Reset();
			}		
			else
			{
				ContainerPortrait.SetActive(true);
				currentLayout = Layouts[Portrait];
				currentLayout.Reset();
			}		
			
			if (BannerCount == 0) return;
			
			int offset = 0;
			int topOffset = 0;
			
			for (int i = 0; i < BannerCount; i++)
			{				
				if (i == 0 && topBanners.Count > 0)
				{
					if (currentLayout.currentItems == 0)
					{
						if (!string.IsNullOrEmpty(topBanners[i].UpdateMessage))
						{
							for (int j = 0; j < UpdateMessage.Length; j++)
							{
								UpdateMessage[j].text = topBanners[i].UpdateMessage;
							}
						}
					}
					currentLayout.PopulateNextItem(topBanners[i]);
					offset = 1; // offset for the next normal banner so it starts at 0
				}
				else
				{
					if ((banners.Count == 0 || i-offset >= banners.Count) && (i - topOffset) < topBanners.Count)
					{
						if (currentLayout.currentItems == 0)
						{
							if (!string.IsNullOrEmpty(topBanners[i - topOffset].UpdateMessage))
							{
								for (int j = 0; j < UpdateMessage.Length; j++)
								{
									UpdateMessage[j].text = topBanners[i - topOffset].UpdateMessage;
								}
							}
						}
						currentLayout.PopulateNextItem(topBanners[i - topOffset]);
					}
					else if (i - offset < banners.Count)
					{
						if (currentLayout.currentItems == 0)
						{
							if (!string.IsNullOrEmpty(banners[i - offset].UpdateMessage))
							{
								for (int j = 0; j < UpdateMessage.Length; j++)
								{
									UpdateMessage[j].text = banners[i - offset].UpdateMessage;
								}
							}
						}
						
						currentLayout.PopulateNextItem(banners[i - offset]);
						topOffset++;
					}
				}
			}
			StartCoroutine(RescaleCanvas());
		}
		
		public void GoToPublisherPage()
		{
			#if UNITY_ANDROID
			Application.OpenURL("market://search?q=pub:" + AndroidPublisherID);
			#elif UNITY_IOS
			Application.OpenURL("itms://itunes.apple.com/developer/" + iOSPubliserID);
			#endif		
		}
		
		public void Show()
		{
			if (OnShow != null) OnShow.Invoke();
			
			PreLoader.SetActive(true);
			
			if (doc != null && !hasNetwork())
			{
				DisplayNoInternetPage();
			}
			else if (doc != null)
			{
				if (AlwaysShowPreloader) DOVirtual.DelayedCall(1, ShowBanners, true);
				else ShowBanners();
			}
			else if (doc == null && !docIsDownloading)
			{
				Load();
			}       
		}
		
		public void Close()
		{
			if (OnClose != null) OnClose.Invoke();
			ContainerLandscape.SetActive(false);
			ContainerPortrait.SetActive(false);
            isShowing = false;
        }
	    #endregion
	    
	    #region Private
		protected override void Awake()
		{
			base.Awake();
			currentLayout = Layouts[0];
		}
	    
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if(currentScreenOrientation == ScreenOrientationType.Landscape)
				{
					if(!ContainerLandscape.activeSelf && UseHWBackButtonToShow)
					{
						Show();
					}			
					else if(ContainerLandscape.activeSelf && UseHWBackButtonToShow)
					{
						Close();
					}
				}
				else
				{
					if(!ContainerPortrait.activeSelf && UseHWBackButtonToShow)
					{
						Show();
					}			
					else if(ContainerPortrait.activeSelf && UseHWBackButtonToShow)
					{
						Close();
					}
				}		
			}      
		}
	
	    private void OnDocData(string message, string data)
	    {
	        if (!string.IsNullOrEmpty(message))
	        {
	            if (onDocLoadError != null) onDocLoadError.Invoke();
	            DisplayNoInternetPage();
	        }
	        else
	        {
	            data = data.Replace("&", "&amp;");
	
	            PreLoader.SetActive(false);
	            doc = XDocument.Parse(data);
	            DocToObjects();
	
	            if (initCall)
	            {
	                initCall = false;
	                Show();
	            }
	        }
	    }
	
	    private void DisplayNoInternetPage()
	    {
	    	if(currentScreenOrientation == ScreenOrientationType.Landscape)
	    	{
				ContainerLandscape.SetActive(true);
				NoInternetLandscape.SetActive(true);			
				currentLayout = Layouts[Landscape];
	    	}
	    	else
	    	{
				ContainerPortrait.SetActive(true);
				NoInternetPortrait.SetActive(true);
				currentLayout = Layouts[Portrait];			
	    	}    
	        
			currentLayout.gameObject.SetActive(false);
	        PreLoader.SetActive(false);
	    }
	    
		private ScreenOrientationType DetectScreenOrientation()
	    {
	    	#if UNITY_EDITOR
			switch(UnityEditor.PlayerSettings.defaultInterfaceOrientation)
			{
				case UnityEditor.UIOrientation.LandscapeLeft:
					return ScreenOrientationType.Landscape;
				case UnityEditor.UIOrientation.LandscapeRight:
					return ScreenOrientationType.Landscape;
				case UnityEditor.UIOrientation.Portrait:
					return ScreenOrientationType.Portrait;
				case UnityEditor.UIOrientation.PortraitUpsideDown:
					return ScreenOrientationType.Portrait;
				default:
					return ScreenOrientationType.Landscape;
			}   
			#else
			Debug.Log("DetectScreenOrientation - Screen.orientation : " + Screen.orientation);
	    	switch(Screen.orientation)
	    	{
				case ScreenOrientation.LandscapeLeft:
					return ScreenOrientationType.Landscape;
						
				case ScreenOrientation.LandscapeRight:
					return ScreenOrientationType.Landscape;	
							
				case ScreenOrientation.Portrait:
					return ScreenOrientationType.Portrait;
	
				case ScreenOrientation.PortraitUpsideDown:
					return ScreenOrientationType.Portrait;
	
				default:
				Debug.Log("DetectScreenOrientation - Input.deviceOrientation : " + Input.deviceOrientation);
						switch(Input.deviceOrientation)
						{
							case DeviceOrientation.LandscapeLeft:
								return ScreenOrientationType.Landscape;
							case DeviceOrientation.LandscapeRight:
								return ScreenOrientationType.Landscape; 
							case DeviceOrientation.Portrait:
								return ScreenOrientationType.Portrait; 
							case DeviceOrientation.PortraitUpsideDown:
								return ScreenOrientationType.Portrait; 
							default:
								return ScreenOrientationType.Landscape;
						}						
	    	}
	#endif
	    }
	
		private IEnumerator DownloadData(XDocument xml, docCallback OnDocCallback)
	    {
	#if UNITY_ANDROID
	        var address = AndroidMadsAddress;
	#elif UNITY_IOS
	        var address = iOSMadsAddress;
	#else
	        var address = "";
	#endif
	
	        // Get the bundleID
	#if UNITY_EDITOR
	        bundleID = UnityEditor.PlayerSettings.bundleIdentifier;
	#elif UNITY_ANDROID
			string[] pathFolders = Application.persistentDataPath.Split("/"[0]);
			for (int i = 0; i < pathFolders.Length; i++)
			{
				if (pathFolders[i].StartsWith("com."))
					bundleID = pathFolders[i];
			}
	#else
			// This won't work on iOS for unity 4.X, find a fix if needed
			//bundleID = Application.bundleID;
	#endif
	        string url = GenerateUrl(address);
	
	        docIsDownloading = true;
	
	        WWW xmldoc = new WWW(url);
	        yield return xmldoc;
	
	        docIsDownloading = false;
	        OnDocCallback(xmldoc.error, string.IsNullOrEmpty(xmldoc.error) ? xmldoc.text : "");
	    }
	
	    private string GenerateUrl(string address)
	    {
	        return SendMoreGamesVersion ? "http://" + address + "/loadmoregames.php?bid=" + bundleID + "&moregamesversion=1" : "http://" + address + "/loadmoregames.php?bid=" + bundleID;
	    }
	
		private void DocToObjects()
	    {
			bool updateMessageSet = false;
	        foreach (XElement item in doc.Descendants("banner"))
	        {
	            string bundle = item.Element("packed").Value;
	            string name = item.Element("name").Value;
	
	            if (IsInstalled(bundle)) continue;
	
	            AdBannerData banner = new AdBannerData();
	            banner.BundleID = bundle;
	            string[] namesplit = name.Split('|');
	            banner.Name = namesplit[0];
	            banner.MarketLink = item.Element("link").Value;
	            Debug.Log("MarketLink : " + banner.MarketLink);
	            banner.IconURL = item.Element("icon").Value;
	            banner.ImageURL = item.Element("image").Value;
				banner.ButtonColor = ColorFromHexString(item.Element("color").Value);
				banner.ButtonTextColor = ColorFromHexString(item.Element("tcolor").Value);
	            int.TryParse(item.Element("adid").Value, out banner.AdID);
	            int isFeatured;
	            int.TryParse(item.Element("featured").Value, out isFeatured);
	            banner.IsFeatured = isFeatured != 0;
	            banner.InstallMessage = item.Element("btext").Value;
				banner.UpdateMessage = item.Element("text").Value;
	            if (banner.IsFeatured) topBanners.Add(banner);
	            else banners.Add(banner);
	        }
	    }
	
		private Color ColorFromHexString(string colorString)
		{
			if (!string.IsNullOrEmpty(colorString) && colorString.Length == 6)
			{
				return HexToRGB(colorString);
			}
			return new Color32(0x68, 0x9F, 0x38, 0xFF);
		}
		
		private Color HexToRGB(string colorString)
		{
			if (colorString.Length == 6)
			{
				float red = Convert.ToInt32(colorString[0].ToString() + colorString[1].ToString(), 16) / 255f;
				float green = Convert.ToInt32(colorString[2].ToString() + colorString[3].ToString(), 16) / 255f;
				float blue = Convert.ToInt32(colorString[4].ToString() + colorString[5].ToString(), 16) / 255f;
				return new Color(red, green, blue, 1.0f);
			}
			return Color.white;
		}
	
	    /// <summary>
	    /// Checks if one of the games is installed after loading the data from the server.
	    /// If so it will be removed from showing.
	    /// </summary>
		private void ValidateBanners()
	    {
	        List<AdBannerData> invalidItems = new List<AdBannerData>();
	        List<AdBannerData> invalidTopItems = new List<AdBannerData>();
	
	        foreach (var item in banners)
	        {
	            if (IsInstalled(item.BundleID))
	                invalidItems.Add(item);
	        }
	
	        foreach (var item in topBanners)
	        {
	            if (IsInstalled(item.BundleID))
	                invalidTopItems.Add(item);
	        }
	
	        foreach (var item in invalidItems)
	        {
	            banners.Remove(item);
	        }
	
	        foreach (var item in invalidTopItems)
	        {
	            topBanners.Remove(item);
	        }
	
	        invalidItems = null;
	        invalidTopItems = null;
	    }
	
		private void SetLayoutDisplay()
	    {
	        int numberOfBanners = BannerCount;
	        int numberOfLayouts = Layouts.Length;
	        int layout = Mathf.Min(numberOfBanners, numberOfLayouts);
	
	        currentLayout.ResetScrollPosition();

	        if (numberOfBanners != 0)
	        {
	            currentLayout.gameObject.SetActive(false);
	            
				if(currentScreenOrientation == ScreenOrientationType.Landscape)
				{
					currentLayout = Layouts[Landscape];
				}
				else
				{
					currentLayout = Layouts[Portrait];
				}
				   			  
	            currentLayout.gameObject.SetActive(true);
	        }
	        else
	        {
	            currentLayout.gameObject.SetActive(false);

				if(currentScreenOrientation == ScreenOrientationType.Landscape) {
					NoItemsLandscape.gameObject.SetActive(true);
				} else {
					NoItemsPortrait.gameObject.SetActive(true);
				}
	        }
	    }
	
		private int BannerCount
	    {
	        get
	        {
	            return banners.Count + topBanners.Count;
	        }
	    }
	    
		private IEnumerator RescaleCanvas()
		{
			canvasScaler.enabled = false;
			yield return null;
			canvasScaler.enabled = true;
		}
		
		private bool IsInstalled(string bundleID)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			
			AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");
			
			try
			{
				packageManager.Call<AndroidJavaObject>("getPackageInfo", bundleID, packageManager.GetStatic<int>("GET_ACTIVITIES"));
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
			#else
			return false;
			#endif
		}
		
		private bool hasNetwork()
		{
			System.Net.WebClient client = null;
			System.IO.Stream stream = null;
			try
			{
				client = new System.Net.WebClient();
				stream = client.OpenRead("http://www.google.com");
				return true;
			}
			#pragma warning disable
			catch (Exception e)
			{
				#pragma warning restore
				return false;
			}
			finally
			{
				if (client != null) { client.Dispose(); }
				if (stream != null) { stream.Dispose(); }
			}
		}
		
		private void ShowBanners()
		{
            currentScreenOrientation = DetectScreenOrientation();

            if (!bannersShown)
			{
				ValidateBanners();
				SetLayoutDisplay();
				DisplayBanners();
				bannersShown = true;
			}
			else
			{
				SetLayoutDisplay();
				PreLoader.SetActive(false);

				if(currentScreenOrientation == ScreenOrientationType.Landscape)
				{
					ContainerLandscape.SetActive(true);
				}
				else
				{
					ContainerPortrait.SetActive(true);
				}
			}

            isShowing = true;
        }
	    #endregion
	}
	
	public struct AdBannerData
	{
	    public string BundleID;
	    public string Name;
	    public string MarketLink;
	    public string IconURL;
	    public string ImageURL;
	    public int AdID;
	    public bool IsFeatured;
	    public string InstallMessage;
	    public Color ButtonColor;
		public Color ButtonTextColor;
		public string UpdateMessage;
	}
	
	public enum ScreenOrientationType
	{
		Landscape,
		Portrait,
	}
}