using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace PlayPhone
{
	public static class Expansions
	{
		public static event Action<Expansion[]> OnServerExpansions;
		public static event Action<string> OnServerExpansionsError;
		public static event Action<Expansion> OnExpansion;
		public static event Action<string> OnExpansionError;
		public static event Action<Expansion[]> OnExpansionsToUpdate;
		public static event Action<string> OnExpansionsToUpdateError;

		private static readonly Dictionary<string, List<Expansion>> downloadingExpansions = new Dictionary<string, List<Expansion>>();

		#region Public methods

		public static void GetServerExpansions(string type, bool downloadIfAbsent)
		{
			Plugin.GetServerExpansions(type, downloadIfAbsent);
		}

		public static Expansion[] GetLocalExpansions(string type, bool downloadIfAbsent)
		{
			return Plugin.GetLocalExpansions(type, downloadIfAbsent);;
		}

		public static Expansion GetLocalExpansion(string type, string name, int version)
		{
			return Plugin.GetLocalExpansion(type, name, version);
		}

		public static Expansion GetExpansion(string id, bool downloadIfAbsent)
		{
			return Plugin.GetExpansion(id, downloadIfAbsent);
		}

		public static Expansion GetExpansion(string type, string name, bool downloadIfAbsent)
		{
			return Plugin.GetExpansion(type, name, downloadIfAbsent);
		}

		public static Expansion GetExpansion(string type, string name, int version, bool downloadIfAbsent)
		{
			return Plugin.GetExpansion(type, name, version, downloadIfAbsent);
		}

		public static void GetExpansionsToUpdate(params string[] types)
		{
			Plugin.GetExpansionsToUpdate(types);
		}

		public static bool DeleteExpansion(Expansion expansion)
		{
			return Plugin.DeleteExpansion(expansion);
		}

		internal static void DownloadExpansion(Expansion expansion)
		{
			List<Expansion> expansions;
			if (!downloadingExpansions.TryGetValue(expansion.Id, out expansions))
			{
				expansions = new List<Expansion>();
				downloadingExpansions[expansion.Id] = expansions;
			}

			expansions.Add(expansion);

			Plugin.DownloadExpansion(expansion);
		}

		internal static void DownloadExpansionUnpackToDefaultLocation (Expansion expansion)
		{
			List<Expansion> expansions;
			if (!downloadingExpansions.TryGetValue(expansion.Id, out expansions))
			{
				expansions = new List<Expansion>();
				downloadingExpansions[expansion.Id] = expansions;
			}
			
			expansions.Add(expansion);

			Plugin.DownloadExpansionUnpackToDefaultLocation (expansion);
		}

		#endregion

		#region Callback internal methods

		internal static void RaiseServerExpansions(Expansion[] expansions)
		{
			if (OnServerExpansions != null)
			{
				OnServerExpansions(expansions);
			}
		}

		internal static void RaiseServerExpansionsError(string error)
		{
			if (OnServerExpansionsError != null)
			{
				OnServerExpansionsError(error);
			}
		}

		internal static void RaiseExpansion(Expansion expansion)
		{
			if (OnExpansion != null)
			{
				OnExpansion(expansion);
			}
		}

		internal static void RaiseExpansionError(string error)
		{
			if (OnExpansionError != null)
			{
				OnExpansionError(error);
			}
		}

		internal static void RaiseExpansionsToUpdate(Expansion[] expansions)
		{
			if (OnExpansionsToUpdate != null)
			{
				OnExpansionsToUpdate(expansions);
			}
		}

		internal static void RaiseExpansionsToUpdateError(string error)
		{
			if (OnExpansionsToUpdateError != null)
			{
				OnExpansionsToUpdateError(error);
			}
		}

		internal static void RaiseExpansionDownloaded(string id, bool success)
		{
			List<String> allKeys = downloadingExpansions.Keys.ToList();
			foreach(String key in allKeys){
				
				List<Expansion> expansions;
				if (downloadingExpansions.TryGetValue(key, out expansions))
				{
					foreach (var expansion in expansions)
					{
						expansion.RaiseDownloaded(success);
					}
				}
				downloadingExpansions.Remove(key);
			}
		}

		internal static void RaiseExpansionProgress(string id, long downloaded, long left)
		{

			List<String> allKeys = downloadingExpansions.Keys.ToList();
			foreach(String key in allKeys){

				List<Expansion> expansions;
				if (downloadingExpansions.TryGetValue(key, out expansions))
				{
					foreach (var expansion in expansions)
					{
						expansion.RaiseProgress(downloaded, left);
					}
				}
			}
		}

		#endregion
	}
}
