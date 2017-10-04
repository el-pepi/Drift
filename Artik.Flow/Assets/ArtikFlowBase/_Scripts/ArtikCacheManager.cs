using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AFBase{
/* This class is meant to manage the ads cache manually */
public class ArtikCacheManager : MonoBehaviour {
	
	public static ArtikCacheManager instance;

	public void Awake(){
		instance = this;
	}

	// Use this for initialization
	void Start(){
	}
	
	// Update is called once per frame
	void Update(){		
	}

	void OnApplicationQuit(){
		// This method assumes the manager will be destroyed only on application exit
		if( ArtikFlowBase.instance.configuration.cleanOnApplicationQuit ){
			instance.clearVungleAdsCache();
			instance.clearCache();
			#if UNITY_EDITOR
				Debug.Log("Cleaned ads cache on QUIT");
			#endif
		}
	}

	void OnApplicationPause(bool isPaused){
		if( isPaused && ArtikFlowBase.instance.configuration.cleanOnApplicationPause ){
			instance.clearVungleAdsCache();
			instance.clearCache();
			#if UNITY_EDITOR
				Debug.Log("Cleaned ads cache on PAUSE");
			#endif
		} else if( !isPaused && ArtikFlowBase.instance.configuration.cleanOnApplicationPause ){
			Ads.instance.FetchVideo();
			Ads.instance.FetchInterstitial();
		}
	}

	public void clearCache(){
		// Tries to clear the whole /cache directory in application path
		try{
			DirectoryInfo cacheDirectory = new DirectoryInfo(getCachePath());
			if( cacheDirectory.Exists ){
				foreach(FileInfo file in cacheDirectory.GetFiles()){
					try{
						file.Delete();
					}
					catch(Exception e){
						Debug.Log("[ ArtikCacheManager ] Exception: " + e.Message);
					}
				}
				foreach(DirectoryInfo subdirectory in cacheDirectory.GetDirectories()){
					try{
						subdirectory.Delete(true);
					}
					catch(Exception e){
						Debug.Log("[ ArtikCacheManager ] Exception: " + e.Message);
					}					
				}
			}
		}
		catch(Exception e){
			Debug.Log("[ ArtikCacheManager ] Exception: " + e.Message);
		}
	}

	public void clearUnityAdsCache(){
		// Tries to clear the UnityAdsCache directory in application path
		try{
			#if UNITY_ANDROID
				string unityAdsCachePath = Path.Combine(getCachePath(), "UnityAdsCache");
			#elif UNITY_IOS
				string unityAdsCachePath = Path.Combine(getCachePath(), "unityads");
			#else 
				string unityAdsCachePath = getCachePath();
			#endif
			if( Directory.Exists(unityAdsCachePath) ){
				Directory.Delete(unityAdsCachePath, true);
			}
		}
		catch(Exception e){
			Debug.Log("[ ArtikCacheManager ] Exception: " + e.Message);
		} 
	}

	public void clearUnityShaderCache(){
		// Tries to clear the UnityShaderCache directory in application path
		try{
			string unityShaderCachePath = Path.Combine(getCachePath(), "UnityShaderCache");
			if( Directory.Exists(unityShaderCachePath) ){
				Directory.Delete(unityShaderCachePath, true);
			}
		}
		catch(Exception e){
			Debug.Log("[ ArtikCacheManager ] Exception: " + e.Message);
		}
	}

	public void clearVungleAdsCache(){
		try{
			#if UNITY_ANDROID
				string vungleCachePath = Path.Combine(getFilesPath(), ".vungle");
			#elif UNITY_IOS
				string vungleCachePath = Path.Combine(getCachePath(), "vungle");
			#else
				string vungleCachePath = getFilesPath();
			#endif
			if( Directory.Exists(vungleCachePath) ){
				Directory.Delete(vungleCachePath, true);
			}
		}
		catch(Exception e){
			Debug.Log("[ ArtikCacheManager ] Exception: " + e.Message);
		}
	}

	public string getCachePath(){
		string path = Application.persistentDataPath;
		#if UNITY_ANDROID
			String[] pathParts = path.Split(Path.DirectorySeparatorChar);
			Array.Resize(ref pathParts, pathParts.Length - 1); // removes "/files"
			path = String.Join(Path.DirectorySeparatorChar.ToString(), pathParts);
			path = Path.Combine(path, "cache");
		#elif UNITY_IOS
			String[] pathParts = path.Split(Path.DirectorySeparatorChar);
			Array.Resize(ref pathParts, pathParts.Length - 1); // removes "/Documents"
			path = String.Join(Path.DirectorySeparatorChar.ToString(), pathParts);
			path = Path.Combine(path, "Library");
			path = Path.Combine(path, "Caches");
		#endif
		return path;
	}

	public string getFilesPath(){
		string path = Application.persistentDataPath;
		return path;
	}
}

}