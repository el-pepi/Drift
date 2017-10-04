using UnityEngine;
using System.Collections;
using System;

namespace PlayPhone
{
	public static class License
	{
		public static event Action OnSuccess;
		public static event Action<string> OnError;

		public static void Check()
		{
			Plugin.CheckLicense();
		}

		internal static void RaiseOnSuccess()
		{
			if (OnSuccess != null) 
			{
				OnSuccess();
			}
		}

		internal static void RaiseOnError(string error)
		{
			if (OnError != null)
			{
				OnError(error);
			}
		}
	}
}
