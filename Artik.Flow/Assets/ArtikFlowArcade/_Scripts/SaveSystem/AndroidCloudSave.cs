using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.SocialPlatforms;
using System;
using System.Collections.Generic;
using GooglePlayGames.BasicApi;
using UnityEngine.Events;

namespace AFArcade {

public static class AndroidCloudSave
{
	public class ByteArrayEvent : UnityEvent<byte[]> { }
	public static ByteArrayEvent eventOverwriteSavedGame = new ByteArrayEvent();

	static string FILENAME = "Main";

	static ISavedGameMetadata currentSnapshot;

	static bool busy;

	// Open

	public static void OpenSavedGame()
	{
#if UNITY_ANDROID
		if (busy)
			return;
		busy = true;

		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.OpenWithManualConflictResolution(FILENAME, DataSource.ReadCacheOrNetwork, false,
			(resolver, original, originalData, unmerged, unmergedData) =>
			{
				Debug.Log("============ Cloud Save open conflict! Resolving manually, OriginalTime: " + original.TotalTimePlayed + ", UnmergedTime: " + unmerged.TotalTimePlayed);
				if (original.TotalTimePlayed > unmerged.TotalTimePlayed)
					resolver.ChooseMetadata(original);
				else
					resolver.ChooseMetadata(unmerged);
			}, OnSavedGameOpened);
#endif
	}

	static void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
	{
#if UNITY_ANDROID
		Debug.Log("============ New cloud saveGame opened: " + status + ", GameTotalTimePlayed: " + game.TotalTimePlayed + ", PlayerPrefs last cloudsave: " + PlayerPrefs.GetFloat("LastCloudSave"));
		busy = false;

		if (status == SavedGameRequestStatus.Success)
		{
			currentSnapshot = game;
			LoadGameData(game);
		}
		else
		{
			// handle error
		}
#endif
	}

	// Load

	static void LoadGameData(ISavedGameMetadata game)
	{
#if UNITY_ANDROID
		if (game.TotalTimePlayed.TotalSeconds - PlayerPrefs.GetFloat("LastCloudSave") < 5f)
		{
			Debug.Log("============ Cloud save matches local save time, skip loading. SaveTotalSeconds: " + game.TotalTimePlayed.TotalSeconds + ", PlayerPrefs: " + PlayerPrefs.GetFloat("LastCloudSave"));
			return;
		}

		if(PlayerPrefs.GetFloat("SecondsPlayed") > game.TotalTimePlayed.TotalSeconds + 100f)
		{
			Debug.Log("============ Cloud load skipped! Attempted to load save with time: " + game.TotalTimePlayed.TotalSeconds + ", PlayerPrefs SecondsPlayed is: " + PlayerPrefs.GetFloat("SecondsPlayed"));
			return;
		}

		if (busy)
			return;
		busy = true;

		PlayerPrefs.SetFloat("SecondsPlayed", (float)game.TotalTimePlayed.TotalSeconds);
		PlayerPrefs.SetFloat("LastCloudSave", (float)game.TotalTimePlayed.TotalSeconds);

		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
#endif
	}

	static void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
	{
#if UNITY_ANDROID
		busy = false;

		if (status == SavedGameRequestStatus.Success)
		{
			eventOverwriteSavedGame.Invoke(data);
		}
		else
		{
			// handle error
		}
#endif
	}

	// Save

	public static void SaveGame(byte[] savedData, TimeSpan totalPlaytime)
	{
#if UNITY_ANDROID
		if (currentSnapshot == null)
		{
			Debug.Log("======= Error in AndroidSaveSystem.SaveGame(), no snapshot is open for saving.");
			return;
		}

		if (busy)
			return;
		busy = true;

		Debug.Log("============ Executing cloud save...");
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

		SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
		builder = builder
			.WithUpdatedPlayedTime(totalPlaytime)
			.WithUpdatedDescription("Saved game at " + DateTime.Now);

		SavedGameMetadataUpdate updatedMetadata = builder.Build();
		savedGameClient.CommitUpdate(currentSnapshot, updatedMetadata, savedData, OnSavedGameWritten);
#endif
	}

	static void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
	{
#if UNITY_ANDROID
		Debug.Log("============ OnSavedGameWritten: " + status);
		busy = false;

		if (status == SavedGameRequestStatus.Success)
		{
			// handle reading or writing of saved game.
			OpenSavedGame();
			PlayerPrefs.SetFloat("LastCloudSave", PlayerPrefs.GetFloat("SecondsPlayed"));
		}
		else
		{
			// handle error
		}
#endif
	}

}

}