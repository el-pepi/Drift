using UnityEngine;
using System.Collections;
using System;
using System.IO;

namespace PlayPhone
{
	public class Expansion
	{
		public class Usage
		{
			public Expansion Expansion { get; private set; }
			public string PackageName { get; set; }
			public long Time { get; set; }

			public Usage(Expansion expansion)
			{
				Expansion = expansion;
			}
		}

		public string Id { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string ReadableLabel { get; set; }
		public string UserData { get; set; }
		public string Description { get; set; }
		public string Developer { get; set; }
		public string InstallerPackageName { get; set; }
		public string[] ContentList { get; set; }
		public int Version { get; set; }
		public long LastUpdate { get; set; }
		public long Size { get; set; }
		public bool IsDownloaded { get; set; }
		public bool IsServerSide { get; set; }
		public Usage[] Usages { get; set; }

		public event Action<bool> OnDownloaded;
		public event Action<long, long> OnProgress;

		public Expansion()
		{}

		public void Download()
		{
			Expansions.DownloadExpansion(this);
		}

		public void DownloadUnpackToDefaultLocation()
		{
			Expansions.DownloadExpansionUnpackToDefaultLocation(this);
		}

		public Texture2D GetIcon()
		{
			var bytes = Plugin.GetExpansionIconBytes(this);
			if (bytes == null)
			{
				return null;
			}
			// The size might be changed
			var texture = new Texture2D(10, 10);
			texture.LoadImage(bytes);
			return texture;
		}

		public Stream GetInputStream(string relativePath)
		{
			return Plugin.GetExpansionInputStream(this, relativePath);
		}

		internal void RaiseDownloaded(bool success)
		{
			if (OnDownloaded != null)
			{
				OnDownloaded(success);
			}
		}

		internal void RaiseProgress(long downloaded, long left)
		{
			if (OnProgress != null)
			{
				OnProgress(downloaded, left);
			}
		}
	}
}