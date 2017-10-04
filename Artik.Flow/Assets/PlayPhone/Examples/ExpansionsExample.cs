using UnityEngine;
using System.Linq;
using System.Collections;
using PlayPhone;

public class ExpansionsExample : ExampleScreen 
{
	private const string ExpansionType = "com.playphone.expansion_example2";
	private Expansion[] serverExpansions;
	private Expansion[] localExpansions;
	private Expansion downloadingExpansion;
	private Texture2D icon;

	void Start()
	{
		Expansions.OnServerExpansions += (expansions) => {
			serverExpansions = expansions;
			ShowExpansions("server", expansions);
		};
		Expansions.OnExpansion += (expansion) => {
			SetStatus(string.Format("{0}(id={1}) downloaded", expansion.Name, expansion.Id));
		};
		Expansions.OnServerExpansionsError += (error) => {
			SetStatus("Unable to get server expansions: " + error);
		};
		Expansions.OnExpansionError += (error) => {
			SetStatus("Unable to download expansion: " + error);
		};
	}

	public override void Draw()
	{
		if (GUILayout.Button("Get Server Expansions"))
		{
			SetStatus("Getting server expansions...");
			Expansions.GetServerExpansions(ExpansionType, false);
		}
		if (GUILayout.Button("Get Local Expansions"))
		{
			localExpansions = Expansions.GetLocalExpansions(ExpansionType, false);
			ShowExpansions("local", localExpansions);
		}
		if (GUILayout.Button("Download expansion"))
		{
			DownloadFirstServerExpansion();
		}
		if (GUILayout.Button("Download expansion with status"))
		{
			DownloadFirstServerExpansionWithProgress();
		}
		if (GUILayout.Button("Delete first local expansion"))
		{
			DeleteFirstLocalExpansion();
		}

		if (GUILayout.Button("Load first local expansion's icon"))
		{
			GetFirstLocalIcon();
		}
		if (icon != null)
		{
			GUILayout.Button(icon);
		}
		if (GUILayout.Button("Get first local expansion's stream"))
		{
			GetFirstLocalStream();
		}
	}

	private void DownloadFirstServerExpansion()
	{
		if (serverExpansions != null && serverExpansions.Length > 0)
		{
			Expansion expansion = serverExpansions.FirstOrDefault(e => !e.IsDownloaded);
			if (expansion == null)
			{
				SetStatus("All server expansions downloaded");
			}
			else
			{
				var localExpansion = Expansions.GetExpansion(expansion.Id, true);
				if (localExpansion != null && localExpansion.IsDownloaded)
				{
					SetStatus(string.Format("{0}(id={1}) is already downloaded", localExpansion.Name, localExpansion.Id));
				}
				else
				{
					SetStatus(string.Format("Downloading {0}(id={1}) ...", expansion.Name, expansion.Id));
				}
			}
		}
		else
		{
			SetStatus("No available server expansions. Try to get server expansions.");
		}
	}

	private void DownloadFirstServerExpansionWithProgress()
	{
		if (serverExpansions != null && serverExpansions.Length > 0)
		{
			if (downloadingExpansion != null)
			{
				SetStatus("Another expansion is downloading. Please wait");
			}
			else
			{
				downloadingExpansion = serverExpansions.FirstOrDefault(e => !e.IsDownloaded);
				if (downloadingExpansion == null)
				{
					SetStatus("All server expansions downloaded");
				}
				else
				{
					SetStatus("Downloading started...");
					downloadingExpansion.OnDownloaded += OnExpansionDownloaded;
					downloadingExpansion.OnProgress += OnExpansionProgress;
					downloadingExpansion.Download();
				}
			}
		}
		else
		{
			SetStatus("No available server expansions. Try to get server expansions.");
		}
	}

	private void OnExpansionDownloaded(bool success)
	{
		SetStatus("Expansion downloaded: " + success);

		downloadingExpansion.IsDownloaded = success;
		downloadingExpansion.OnProgress -= OnExpansionProgress;
		downloadingExpansion.OnDownloaded -= OnExpansionDownloaded;
		downloadingExpansion = null;
	}

	private void OnExpansionProgress(long downloaded, long left)
	{
		SetStatus(string.Format("Expansion downloading: {0}/{1}", downloaded, left));
	}

	private void DeleteFirstLocalExpansion()
	{
		if (localExpansions != null && localExpansions.Length > 0)
		{
			var expansion = localExpansions[0];
			if (Expansions.DeleteExpansion(expansion))
			{
				SetStatus(string.Format("Succesfully deleted {0}(id={1})", expansion.Name, expansion.Id));
				localExpansions = Expansions.GetLocalExpansions(ExpansionType, false);
			}
			else
			{
				SetStatus(string.Format("Unable to delete {0}(id={1})", expansion.Name, expansion.Id));
			}
		}
		else
		{
			SetStatus("No available local expansions. Try to get local expansions.");
		}
	}

	private void ShowExpansions(string tag, Expansion[] expansions)
	{
		if (expansions == null)
		{
			SetStatus("No expansions");
		}

		var names = string.Join(", ", (from e in expansions 
			select string.Format("{0}(id={1} v={2}]", e.Name, e.Id, e.Version)).ToArray());
		SetStatus(string.Format("{0} {1} expansions: {2}", expansions.Length, tag, names));
	}

	private void GetFirstLocalIcon()
	{
		if (localExpansions != null && localExpansions.Length > 0)
		{
			var expansion = localExpansions[0];
			var texture = expansion.GetIcon();
			if (texture != null)
			{
				icon = texture;
				SetStatus(string.Format("Icon size {0}x{1}", texture.width,  texture.height));
			}
			else
			{
				SetStatus("No icon found");
			}
		}
	}

	private void GetFirstLocalStream()
	{
		if (localExpansions != null && localExpansions.Length > 0)
		{
			var expansion = localExpansions[0];
			if (expansion.ContentList != null && expansion.ContentList.Length > 0)
			{
				var content = expansion.ContentList.Where(c => c.Contains("about.str")).FirstOrDefault();
				if (content == null)
				{
					content = expansion.ContentList[0];
				}

				var stream = expansion.GetInputStream(content);
				if (stream != null)
				{
					byte[] buffer = new byte[256];
					var count = stream.Read(buffer, 0, 255);
					SetStatus(string.Format("Read {0} bytes from {1}. First byte is {2}", count, content, buffer[0]));
					stream.Dispose();
				}
				else
				{
					SetStatus("Stream not found");
				}
			}
			else
			{
				SetStatus("Content not found");
			}
		}
	}
}
