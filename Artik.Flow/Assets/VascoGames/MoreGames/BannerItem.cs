using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VascoGames.MoreGames
{
	public class BannerItem : MonoBehaviour {
	
	    public string GameURL;
	    public Text NameField;
	    public RawImage Face;
	    public RawImage Icon;
	    public Text MessageText;
	    public Image InstallButtonImage;
		public Text InstallButtonText;
	    
	    [HideInInspector()]
	    public string bundleID;
	
	
	    public void OnClicked()
	    {
	#if UNITY_EDITOR && UNITY_ANDROID
	        Application.OpenURL(@"https://play.google.com/store/apps/details?id=" + bundleID);
	#else
	        Application.OpenURL(GameURL);
	
	#endif
	    }
		 
	}
}