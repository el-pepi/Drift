using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;

namespace PlayPhone
{	
	public static class Plugin
	{
		#region Callbacks
		public static event Action OnInit;
		public static event Action<string> OnInitError;
		public static event Action OnUserChanged;
		public static event Action<string> OnUserChangedError;
		public static event Action<string> OnLeaderboardsData;
		public static event Action<string> OnInAppProducts;
		public static event Action<string> OnLeaderboardsDataError;
		public static event Action<string> OnInAppProductsError;
		public static event Action<string> OnLaunchScreen;


		internal static void RaiseOnLaunchScreen(string screen)
		{
			if (OnLaunchScreen != null)
			{
				OnLaunchScreen(screen);
			}
		}

		internal static void RaiseOnLeaderboards(string json)
		{
			if (OnLeaderboardsData != null)
			{
				OnLeaderboardsData(json);
			}
		}
		
		internal static void RaiseOnInAppProducts(string json)
		{
			if (OnInAppProducts != null)
			{
				OnInAppProducts(json);
			}
		}

		internal static void RaiseOnLeaderboardsError(string json)
		{
			if (OnLeaderboardsDataError != null)
			{
				OnLeaderboardsDataError(json);
			}
		}
		
		internal static void RaiseOnInAppProductsError(string json)
		{
			if (OnInAppProductsError != null)
			{
				OnInAppProductsError(json);
			}
		}


		internal static void RaiseOnInitError(string error)
		{
			if (OnInitError != null) {
				OnInitError(error);
			}
		}

		internal static void RaiseOnInit()
		{
			if (OnInit != null) {
				OnInit ();
			}
		}

		internal static void RaiseUserChanged()
		{
			if (OnUserChanged != null) {
				OnUserChanged();
			}
		}

		internal static void RaiseUserChangedError(string error)
		{
			if (OnUserChangedError != null) {
				OnUserChangedError(error);
			}
		}

		#endregion

		#region Logging
		public static bool Verbose = true;
		
		private static void DebugLog(string message)
		{
			if (!Verbose)
				return;
			
			Debug.Log("[PlayPhone.Plugin] " + message);
		}
		#endregion
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		private static AndroidJavaObject _androidAdapterJavaObject;
		private static GameObject _androidAdapterGameObject;
		private static PlayPhoneAndroidCallbacks _androidCallbacksComponent;
		
		private static void InitAndroidAdapterIfNeeded()
		{
			if (_androidAdapterJavaObject != null ||
				_androidAdapterGameObject != null ||
				_androidCallbacksComponent != null)
			{
				return;
			}
			
			_androidAdapterGameObject = new GameObject("PlayPhoneAdapter");
			GameObject.DontDestroyOnLoad(_androidAdapterGameObject);
			_androidCallbacksComponent = _androidAdapterGameObject.AddComponent<PlayPhoneAndroidCallbacks>();
			
			_androidAdapterJavaObject = new AndroidJavaObject("com.playphone.psgn.unityadapter.UnityAdapter", _androidAdapterGameObject.name);
		}
		
		private static AndroidJavaObject AndroidAdapter
		{
			get
			{
				InitAndroidAdapterIfNeeded();
				return _androidAdapterJavaObject;
			}
		}
		
		private static PlayPhoneAndroidCallbacks Callbacks
		{
			get 
			{
				InitAndroidAdapterIfNeeded();
				return _androidCallbacksComponent;
			}
		}

		#region Public Interface
		
		public static void Init()
		{
			InitAndroidAdapterIfNeeded();
		}

		public static void ShowIcon()
		{
			InitAndroidAdapterIfNeeded ();
			AndroidAdapter.Call ("showIcon", true);
		}

		public static void HideIcon()
		{
			InitAndroidAdapterIfNeeded ();
			AndroidAdapter.Call ("showIcon", false);
		}

		public static void GetLeaderboardData(int id)
		{
			AndroidAdapter.Call("getLeaderboardsData", id,"");
		}
		
		public static void GetInAppProductsData()
		{
			AndroidAdapter.Call("getInAppProductsData","");
		}

		#endregion

		#region Internal Interface

		internal static void logProperties()
	    {
	    	AndroidAdapter.Call("logProperties");
	    }

	    internal static String getProperties()
	    {
	    	return AndroidAdapter.Call<String>("getProperties");
	    }

		internal static String getPlatformId()
		{
			return AndroidAdapter.Call<String>("getPlatformId");
		}

		internal static void DoLaunchAction(bool billingReady){
			AndroidAdapter.Call("doLaunchAction", billingReady);
		}

		internal static void GetLaunchScreen()
		{
			AndroidAdapter.Call("getAndClearLaunchAction","");
		}

		internal static void DoAction(string action)
		{
			AndroidAdapter.Call<bool>("doAction", action);
		}

		internal static void DoAction(string action, Dictionary<string, object> values)
		{
			AndroidAdapter.Call<bool>("doAction", action, DictionaryToJavaHashMap(values));
		}

		internal static void DoAction(string action, Dictionary<string, object> values, string chainedAction)
		{
			AndroidAdapter.Call<bool>("doAction", action, DictionaryToJavaHashMap(values), chainedAction);
		}

		internal static void GetData(string action, Dictionary<string, object> values, string taskId = "")
		{
			AndroidAdapter.Call("getData", action, DictionaryToJavaHashMap(values), taskId);
		}

		internal static void GetCurrentPlayerData()
		{
			AndroidAdapter.Call("getCurrentPlayerData", "");
		}

		internal static void GetFriendsData()
		{
			AndroidAdapter.Call("getFriendsData", "");
		}

		internal static void GetPlayerData(string playerId)
		{
			AndroidAdapter.Call("getPlayerData", playerId, "");
		}

		internal static void SetGamePlayerData(string json)
		{
			AndroidAdapter.Call<bool>("setGamePlayerData", json);
		}

		internal static string GetGamePlayerData()
		{
			return AndroidAdapter.Call<string>("getGamePlayerData");
		}

		internal static void GetRemoteGamePlayerData()
		{
			AndroidAdapter.Call("getRemoteGamePlayerData", "");
		}

		internal static void IncrementTracking()
		{
			AndroidAdapter.Call("onResume");
		}

		internal static void DecrementTracking()
		{
			AndroidAdapter.Call("onPause");
		}

		#region Subscriptions

		internal static void GetSubscriptions()
		{
			AndroidAdapter.Call("getSubscriptions");
		}

		internal static string GetResultSubscriptions(long messageId)
		{
			return AndroidAdapter.Call<string>("getResultSubscriptions", messageId);
		}

		#endregion

		internal static void CheckLicense()
		{
			AndroidAdapter.Call("checkLicense", "");
		}

		internal static bool IsFacebookConnected()
		{
			return AndroidAdapter.Call<bool>("isFacebookConnected");
		}

		#region Expansions

		internal static void GetServerExpansions(string type, bool downloadIfAbsent)
		{
			AndroidAdapter.Call("getServerExpansions", type, downloadIfAbsent);
		}

		internal static Expansion[] GetLocalExpansions(string type, bool downloadIfAbsent)
		{
			var messageId = AndroidAdapter.Call<long>("getLocalExpansions", type, downloadIfAbsent);
			var expansions = GetReceivedExpansions(messageId);
			ClearResult(messageId);
			return expansions;
		}

		internal static Expansion GetLocalExpansion(string type, string name, int version)
		{
			var javaExpansion = AndroidAdapter.Call<AndroidJavaObject>("getLocalExpansion", type, name, version);
			return BuildExpansionFromJavaObject(javaExpansion);
		}

		internal static Expansion GetExpansion(string id, bool downloadIfAbsent)
		{
			var intId = int.Parse(id);
			var javaExpansion = AndroidAdapter.Call<AndroidJavaObject>("getExpansion", intId, downloadIfAbsent);
			return BuildExpansionFromJavaObject(javaExpansion);
		}

		internal static Expansion GetExpansion(string type, string name, bool downloadIfAbsent)
		{
			var javaExpansion = AndroidAdapter.Call<AndroidJavaObject>("getExpansion", type, name, downloadIfAbsent);
			return BuildExpansionFromJavaObject(javaExpansion);
		}

		internal static Expansion GetExpansion(string type, string name, int version, bool downloadIfAbsent)
		{
			var javaExpansion = AndroidAdapter.Call<AndroidJavaObject>("getExpansion", type, name, version, downloadIfAbsent);
			return BuildExpansionFromJavaObject(javaExpansion);
		}

		internal static void GetExpansionsToUpdate(params string[] types)
		{
			AndroidAdapter.Call("getExpansionsToUpdate", types);
		}

		internal static bool DeleteExpansion(Expansion expansion)
		{
			var javaExpansion = AndroidAdapter.Call<AndroidJavaObject>("findExpansion", expansion.Id);
			return AndroidAdapter.Call<bool>("deleteExpansion", javaExpansion);
		}

		internal static Expansion[] GetReceivedExpansions(long messageId)
		{
			var count = AndroidAdapter.Call<int>("getReceivedExpansionsCount", messageId);
			Expansion[] expansions = new Expansion[count];

			var pointer = AndroidJNI.PushLocalFrame(count + 20);
			for (int i = 0; i < count; i++)
			{
				var javaExpansion = AndroidAdapter.Call<AndroidJavaObject>("getReceivedExpansion", messageId, i);
				expansions[i] = BuildExpansionFromJavaObject(javaExpansion);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			return expansions;
		}

		internal static void DownloadExpansion(Expansion expansion)
		{
			var javaExpansion = AndroidAdapter.Call<AndroidJavaObject>("findExpansion", expansion.Id);
			AndroidAdapter.Call("downloadExpansion", javaExpansion);
		}

		internal static void DownloadExpansionUnpackToDefaultLocation(Expansion expansion)
		{
			var javaExpansion = AndroidAdapter.Call<AndroidJavaObject>("findExpansion", expansion.Id);
			AndroidAdapter.Call("downloadExpansionUnpackToDefaultLocation", javaExpansion);
		}

		internal static ExpansionStream GetExpansionInputStream(Expansion expansion, string relativePath)
		{
			var javaExpansion = AndroidAdapter.Call<AndroidJavaObject>("findExpansion", expansion.Id);
			var javaStream = javaExpansion.Call<AndroidJavaObject>("getInputStream", relativePath);
			return new ExpansionStream(javaStream);
		}

		internal static byte[] GetExpansionIconBytes(Expansion expansion)
		{
			var javaExpansion = AndroidAdapter.Call<AndroidJavaObject>("findExpansion", expansion.Id);
			var jBytes = AndroidAdapter.Call<AndroidJavaObject>("getExpansionIconBytes", javaExpansion);
			if (jBytes.GetRawObject().ToInt32() == 0)
			{
				return null;
			}
			return AndroidJNIHelper.ConvertFromJNIArray<byte[]>(jBytes.GetRawObject());
		}

		#endregion

		internal static string GetResultJson(long messageId)
		{
			return AndroidAdapter.Call<string>("getResultJson", messageId);
		}

		internal static T GetResultMapValue<T>(long messageId, string key)
		{
			object result = null;
			if (typeof(T) == typeof(string))
			{
				result = AndroidAdapter.Call<string>("getResultMapValue", messageId, key);
			}
			else if (typeof(T) == typeof(int))
			{
				var i = AndroidAdapter.Call<string>("getResultMapValue", messageId, key);
				result = int.Parse(i);
			}
			else
			{
				throw new NotSupportedException("Only 'string' and 'int' supported");
			}

			return (T)result;
		}

		internal static void ClearResult(long messageId)
		{
			AndroidAdapter.Call("clearResult", messageId);
		}

		internal static void OnApplicationPause(bool paused)
		{
			if (paused)
			{
				AndroidAdapter.Call("onPause");
			}
			else
			{
				AndroidAdapter.Call("onResume");
			}
		}

		private static AndroidJavaObject DictionaryToJavaHashMap (Dictionary<string, object> dict) 
		{
			AndroidJavaObject hashMap = new AndroidJavaObject("java.util.HashMap");

			IntPtr putMethod = AndroidJNIHelper.GetMethodID(hashMap.GetRawClass(), "put","(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
			object[] args = new object[2];
			foreach (KeyValuePair<string, object> kvp in dict) 
			{
				using (AndroidJavaObject k = new AndroidJavaObject("java.lang.String", kvp.Key))
				{
					string androidClassName = "java.lang.Object";
					if (kvp.Value is string)
					{
						androidClassName = "java.lang.String";
					}
					else if (kvp.Value is int)
					{
						androidClassName = "java.lang.Integer";
					}
					else if (kvp.Value is bool)
					{
						androidClassName = "java.lang.Boolean";
					}
					else
					{
						throw new NotSupportedException(kvp.Value.GetType().Name);
					}

					using (AndroidJavaObject v = new AndroidJavaObject(androidClassName, kvp.Value))
					{
						args[0] = k;
						args[1] = v;
						AndroidJNI.CallObjectMethod(hashMap.GetRawObject(),
							putMethod, AndroidJNIHelper.CreateJNIArgArray(args));
					}
				}
			}
			return hashMap;
		}

		private static Expansion BuildExpansionFromJavaObject(AndroidJavaObject javaExpansion)
		{
			if (javaExpansion == null || javaExpansion.GetRawObject().ToInt32() == 0) {
				return null;
			}

			Expansion expansion = new Expansion();
			expansion.Id = javaExpansion.Call<int>("getExpansionID").ToString();
			expansion.Name = javaExpansion.Call<string>("getName");
			expansion.Type = javaExpansion.Call<string>("getType");
			expansion.Description = javaExpansion.Call<string>("getDescription");
			expansion.Developer = javaExpansion.Call<string>("getDeveloper");
			expansion.InstallerPackageName = javaExpansion.Call<string>("getInstallerPackageName");
			expansion.LastUpdate = javaExpansion.Call<long>("getLastUpdate");
			expansion.ReadableLabel = javaExpansion.Call<string>("getReadableLabel");
			expansion.Size = javaExpansion.Call<long>("getSize");
			expansion.UserData = javaExpansion.Call<string>("getUserData");
			expansion.Version = javaExpansion.Call<int>("getVersion");
			expansion.IsDownloaded = javaExpansion.Call<bool>("isAlreadyDownloaded");
			expansion.IsServerSide = javaExpansion.Call<bool>("isServerSide");

			expansion.ContentList = LoadExpansionContentList(javaExpansion);
			expansion.Usages = LoadExpansionUsages(expansion, javaExpansion);

			return expansion;
		}

		private static string[] LoadExpansionContentList(AndroidJavaObject javaExpansion)
		{
			var count = AndroidAdapter.Call<int>("getExpansionContentListCount", javaExpansion);
			var contentList = new string[count];

			AndroidJNI.PushLocalFrame(count + 10);

			for (int i = 0; i < count; i++)
			{
				contentList[i] = AndroidAdapter.Call<string>("getExpansionContent", javaExpansion, i);
			}

			AndroidJNI.PopLocalFrame(IntPtr.Zero);

			return contentList;
		}

		private static Expansion.Usage[] LoadExpansionUsages(Expansion expansion, AndroidJavaObject javaExpansion)
		{
			var count = AndroidAdapter.Call<int>("getExpansionUsagesCount", javaExpansion);
			var usages = new Expansion.Usage[count];

			AndroidJNI.PushLocalFrame(count + 10);

			for (int i = 0; i < count; i++)
			{
				var javaUsage = AndroidAdapter.Call<AndroidJavaObject>("getExpansionUsage", javaExpansion, i);

				var usage = new Expansion.Usage(expansion);
				usage.PackageName = javaUsage.Call<string>("getPackageName");
				usage.Time = javaUsage.Call<long>("getTime");

				usages[i] = usage;
			}

			AndroidJNI.PopLocalFrame(IntPtr.Zero);

			return usages;
		}

		private static Expansion[] BuildExpansionArrayFromJavaObject(AndroidJavaObject[] javaExpansions)
		{
			var count = javaExpansions == null ? 0 : javaExpansions.Length;
			var expansions = new Expansion[count];
			for (int i = 0; i < count; i++)
			{
				expansions[i] = BuildExpansionFromJavaObject(javaExpansions[i]);
			}
			return expansions;
		}

		#endregion
#endif
		#if !UNITY_ANDROID || UNITY_EDITOR
		#region Public Interface

		public static void Init()
		{
		}

		public static void ShowIcon()
		{
		}

		public static void HideIcon()
		{
		}

		public static void GetLeaderboardData(int id)
		{
		}
		
		public static void GetInAppProductsData()
		{
		}

		#endregion

		#region Internal Interface

		internal static void logProperties()
	    {
	    }

	    internal static String getProperties()
	    {
	    	return null;
	    }
		
		internal static String getPlatformId()
		{
			return null;
		}

		internal static void DoAction(string action)
		{
		}

		internal static void DoAction(string action, Dictionary<string, object> values)
		{
		}

		internal static void DoAction(string action, Dictionary<string, object> values, string chainedAction)
		{
		}

		internal static void DoLaunchAction(bool billingReady){
		}

		
		internal static void GetLaunchScreen()
		{
		}

		internal static void GetData(string Action, Dictionary<string, object> values, string taskId = "")
		{
		}

		internal static void GetCurrentPlayerData()
		{
		}

		internal static void GetFriendsData()
		{
		}

		internal static void GetPlayerData(string playerId)
		{
		}

		internal static void SetGamePlayerData(string json)
		{
		}

		internal static string GetGamePlayerData()
		{
			return string.Empty;
		}

		internal static void GetRemoteGamePlayerData()
		{}

		internal static void IncrementTracking()
		{
		}
		
		internal static void DecrementTracking()
		{
		}

		#region Subscriptions

		internal static void GetSubscriptions()
		{
		}

		internal static string GetResultSubscriptions(long messsageId)
		{
			return string.Empty;
		}

		#endregion

		internal static void CheckLicense()
		{
		}

		internal static bool IsFacebookConnected()
		{
			return false;
		}

		#region Expansions

		internal static void GetServerExpansions(string type, bool downloadIfAbsent)
		{
		}

		internal static Expansion[] GetLocalExpansions(string type, bool downloadIfAbsent)
		{
			return new Expansion[0];
		}

		internal static Expansion GetLocalExpansion(string type, string name, int version)
		{
			return null;
		}

		internal static Expansion GetExpansion(string id, bool downloadIfAbsent)
		{
			return null;
		}

		internal static Expansion GetExpansion(string type, string name, bool downloadIfAbsent)
		{
			return null;
		}

		internal static Expansion GetExpansion(string type, string name, int version, bool downloadIfAbsent)
		{
			return null;
		}

		internal static void GetExpansionsToUpdate(params string[] types)
		{}

		internal static bool DeleteExpansion(Expansion expansion)
		{
			return false;
		}

		internal static Expansion[] GetReceivedExpansions(long messsageId)
		{
			return new Expansion[0];
		}

		internal static void DownloadExpansion(Expansion expansion)
		{
		}

		internal static void DownloadExpansionUnpackToDefaultLocation(Expansion expansion)
		{
		}

		internal static ExpansionStream GetExpansionInputStream(Expansion expansion, string relativePath)
		{
			return null;
		}

		internal static byte[] GetExpansionIconBytes(Expansion expansion)
		{
			return null;
		}

		#endregion

		internal static string GetResultJson(long messsageId)
		{
			return string.Empty;
		}

		internal static T GetResultMapValue<T>(long messsageId, string key)
		{
			return default(T);
		}

		internal static void ClearResult(long messageId)
		{
		}

		internal static void OnApplicationPause(bool paused)
		{
		}

		#endregion

#endif
	}
}
