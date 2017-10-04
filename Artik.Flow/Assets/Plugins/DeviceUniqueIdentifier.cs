using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;

public static class DeviceUniqueIdentifier
{
	static string identifier = null;

	public static string get()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		if (identifier == null)
			identifier = generateDeviceUniqueIdentifier();

		return identifier;
#else
		return SystemInfo.deviceUniqueIdentifier;
#endif
	}

	// Android only.
	// Skips the READ_PHONE_STATE permission by using ANDROID_ID, and defaulting to mac address in case of fail
	// instead of grabbing the IMEI as SystemInfo.deviceUniqueIdentifier does.
	static string generateDeviceUniqueIdentifier()
	{
		string id = "";
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");

		if (id == null)
			id = "";

		AndroidJavaClass settingsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
		string ANDROID_ID = settingsSecure.GetStatic<string>("ANDROID_ID");
		AndroidJavaObject contentResolver = activity.Call<AndroidJavaObject>("getContentResolver");
		id = settingsSecure.CallStatic<string>("getString", contentResolver, ANDROID_ID);
		if (id == null)
			id = "";

		if (id == "")
		{
			string mac = "00000000000000000000000000000000";
			try
			{
				StreamReader reader = new StreamReader("/sys/class/net/wlan0/address");
				mac = reader.ReadLine();
				reader.Close();
			}
			catch (Exception e) { }
			id = mac.Replace(":", "");
		}
		return getMd5Hash(id);
	}

	// Hash an input string and return the hash as
	// a 32 character hexadecimal string.
	static string getMd5Hash(string input)
	{
		if (input == "")
			return "";
		MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
		byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
		StringBuilder sBuilder = new StringBuilder();
		for (int i = 0; i < data.Length; i++)
			sBuilder.Append(data[i].ToString("x2"));
		return sBuilder.ToString();
	}

}
