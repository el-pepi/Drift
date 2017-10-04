using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Text;

namespace VascoGames.MoreGames
{
	public class MoreGamesContent : MonoBehaviour
	{
	    public delegate void OnTextureDataLoaded(Texture2D texture);
	
	    public GameObject LoadingPrefab;
	    public GameObject SmallGamePrefab;
	    public RectTransform SmallGameContent;
	    public List<BannerItem> Items = new List<BannerItem>();
	    public ScrollRect ScrollRect;
		[SerializeField] private RectTransform ContentRect;
	    private List<GameObject> smallGames = new List<GameObject>();
	    int position = 0;
	
	    List<Coroutine> routines = new List<Coroutine>();
	    List<Tween> tweens = new List<Tween>();
	    List<GameObject> loadingIcons = new List<GameObject>();
	    /*private List<string> cachedGames = new List<string>();
	    private const int CACHE_EXPIRE_DAYS = 14;
	
	    private void Awake()
	    {
	        GetCachedGames();
	    }
	
	    private void GetCachedGames()
	    {
	        if (Directory.Exists(Application.persistentDataPath + "/mgcache.txt"))
	        {
	            var gameListByte = File.ReadAllBytes(Application.persistentDataPath + "/mgcache.txt");
	            string gamesString = Encoding.UTF8.GetString(gameListByte);
	            string[] gamesArray = gamesString.Split(new string[] { "\n" }, StringSplitOptions.None);
	            cachedGames = gamesArray.ToList();
	        }
	
	        for (int i = cachedGames.Count - 1; i >= 0; i--)
	            CheckCachedDate(cachedGames[i]);
	
	        SaveCachedGames();
	    }
	
	    private void SaveCachedGames()
	    {
	        StringBuilder sb = new StringBuilder();
	        for (int i = cachedGames.Count - 1; i >= 0; i--)
	        {
	            sb.Append(cachedGames[i]);
	            sb.Append("\n");
	        }
	        File.WriteAllBytes(Application.persistentDataPath + "/mgcache.txt", Encoding.UTF8.GetBytes(sb.ToString()));
	    }
	
	    private void CheckCachedDate(string bundleID)
	    {
	        if (!Directory.Exists(Application.persistentDataPath + "/mgcache-img-" + bundleID + ".png"))
	        {
	            Debug.Log("cached image not found");
	            cachedGames.Remove(bundleID);
	            return;
	        }
	
	        DateTime writeTime = File.GetLastWriteTime(Application.persistentDataPath + "/mgcache-img-" + bundleID + ".png");
	        DateTime now = DateTime.Now;
	        TimeSpan elapsed = now.Subtract(writeTime);
	        double days = elapsed.TotalDays;
	
	        if (elapsed.TotalDays > CACHE_EXPIRE_DAYS)
	        {
	            File.Delete(Application.persistentDataPath + "/mgcache-img-" + bundleID + ".png");
	            cachedGames.Remove(bundleID);
	        }
	    }*/
	
		private void OnEnable()
		{
			if (ContentRect != null)
			{
				// Hack, the content size fitter is doing some funky shit when you close and open the more games window
				// Reset the left and right offset to 0
				ContentRect.offsetMin = new Vector2(0.0f, ContentRect.offsetMin.y);
				ContentRect.offsetMax = new Vector2(0.0f, ContentRect.offsetMax.y);
			}
		}
	
	    public int currentItems = 0;
	    public void PopulateNextItem(AdBannerData data)
	    {
	        if (currentItems > 0)
	        {
	            GameObject game = Instantiate(SmallGamePrefab) as GameObject;
	            game.transform.SetParent(SmallGameContent);
	            RectTransform rect = game.GetComponent<RectTransform>();
	            rect.anchoredPosition = new Vector2(0f, 0f);
	            rect.localScale = new Vector3(1.0f, 1.0f, 1.0f);
	            smallGames.Add(game);
	            Items.Add(game.GetComponent<BannerItem>());
	        }
	        currentItems++;
	        /*if (cachedGames.Contains(data.BundleID))
	        {
	            routines.Add(StartCoroutine(GetCacheImage(data.BundleID, data.ImageURL, Items[position])));
	        }*/
	
	        if (Items[currentItems - 1].Face != null)
	            routines.Add(StartCoroutine(GetImage(data.ImageURL, Items[currentItems - 1])));
	        routines.Add(StartCoroutine(GetIcon(data.IconURL, Items[currentItems - 1])));
	        Items[currentItems - 1].GameURL = data.MarketLink;
	        Items[currentItems - 1].NameField.text = data.Name;
	        if (data.IsFeatured && Items[position].MessageText != null) {
	            Items[currentItems - 1].MessageText.text = data.InstallMessage;
	        }
	        else if(currentItems - 1 == 0 && Items[currentItems - 1].MessageText != null)
	        {
	            Items[currentItems - 1].MessageText.text = "Play these games!";
	        }
			if (Items[currentItems - 1].InstallButtonImage != null && data.ButtonColor.a != 0.0f)
				Items[currentItems - 1].InstallButtonImage.color = data.ButtonColor;
	
			if (Items[currentItems - 1].InstallButtonText != null)
			{
				if (data.ButtonTextColor.a != 0.0f)
					Items[currentItems - 1].InstallButtonText.color = data.ButtonTextColor;
				if (!string.IsNullOrEmpty(data.InstallMessage))
					Items[currentItems - 1].InstallButtonText.text = data.InstallMessage;
			}
	
	
	        Items[currentItems - 1].bundleID = data.BundleID;
	
	        position++;
	    }
	
	    public void ResetScrollPosition()
	    {
	        if (ScrollRect != null)
	        {
	            ScrollRect.verticalNormalizedPosition = 1.0f;
	            ScrollRect.horizontalNormalizedPosition = 1.0f;
	        }
	    }
	
	    public void Reset()
	    {
	        position = 0;
	
	        currentItems = 0;
	        foreach (GameObject go in smallGames)
	        {
	            Destroy(go);
	        }
	        
	        foreach(var tween in tweens)
	        {
	            tween.Kill();
	        }
	
	        tweens.Clear();
	
	        foreach(var loading in loadingIcons)
	        {
	            DestroyImmediate(loading);
	        }
	
	        loadingIcons.Clear();
	
	        foreach(var routine in routines)
	        {
	            StopCoroutine(routine);
	        }
	
	        foreach (var item in Items)
	        {
	            if (item.Face != null)
	                item.Face.color = new Color(1, 1, 1, 0);
	            if (item.Icon != null)
	                item.Icon.color = new Color(1, 1, 1, 0);
	            if (item.Face != null)
	                item.Face.texture = null;
	            if (item.Icon != null)
	                item.Icon.texture = null;
	        }
	    }
	
	    IEnumerator GetImage(string url, BannerItem item)
	    {
	
	        GameObject loading = Instantiate(LoadingPrefab) as GameObject;
	        loading.transform.SetParent(item.transform, false);
	        loading.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360));
	
	        loadingIcons.Add(loading);
	
	        WWW img = new WWW(url);
	
	        yield return img;
	
	        
	        tweens.Add(loading.GetComponent<CanvasGroup>().DOFade(0, 1).OnComplete(() => Destroy(loading)).SetUpdate(true));
	        item.Face.GetComponent<TweenIn>().Animate();
	
	        if (string.IsNullOrEmpty(img.error)) { 
	            item.Face.texture = img.texture;
	
	            //File.WriteAllBytes(Application.persistentDataPath + "/mgcache-img-" + item.bundleID + ".png", img.texture.EncodeToPNG());
	        }
	        else
	        {
	            item.Face.texture = MoreGamesManager.Instance.FallbackImg;
	        }
	    }
	
	    /*IEnumerator GetCacheImage(string bundleID, string url, BannerItem item)
	    {
	        yield return null;
	        Texture2D tex = new Texture2D(2,2);
	
	        DateTime writeTime = File.GetLastWriteTime(Application.persistentDataPath + "/mgcache-img-" + item.bundleID + ".png");
	        DateTime now = DateTime.Now;
	        TimeSpan elapsed = now.Subtract(writeTime);
	        double days = elapsed.TotalDays;
	
	        if (elapsed.TotalDays < 14 && tex.LoadImage(File.ReadAllBytes(Application.persistentDataPath + "/mgcache-img-" + item.bundleID + ".png")))
	        {
	            item.Face.GetComponent<TweenIn>().Animate();
	            tex.Apply();
	            item.Face.texture = tex;
	        }
	        else
	        {
	            File.Delete(Application.persistentDataPath + "/mgcache-img-" + item.bundleID + ".png");
	            GetImage(url, item);
	        }
	    }*/
	
	
	    IEnumerator GetIcon(string url, BannerItem item)
	    {
	       
	        WWW img = new WWW(url);
	
	        yield return img;
	
	        Texture2D container = new Texture2D(img.texture.width, img.texture.height, TextureFormat.ARGB4444, false);
	        img.LoadImageIntoTexture(container);
	
	        if (string.IsNullOrEmpty(img.error))
	        {
	            item.Icon.texture = img.texture;
	        }
	        else
	        {
	            item.Icon.texture = MoreGamesManager.Instance.FallbackIconImg;
	        }
	        item.Icon.GetComponent<TweenIn>().Animate();
	
	    }
	
	
	    Texture2D LoadFromCache(string id)
	    {
	        return null;
	    }
	}
}