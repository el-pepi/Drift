#if UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AFArcade {

[ExecuteInEditMode]
public class ArtikTools : EditorWindow
{
	string playerPrefButton = "Reset Player Prefabs";
	string deleteSerialized = "Delete Serialized Data";

	// Add menu item named "My Window" to the Window menu
	[MenuItem("Window/Artik Tools/Herramientas")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(ArtikTools));
	}

	[MenuItem("Window/Artik Tools/Delete Saved Data")]
	public static void DeleteSerializeData()
	{
		//Show existing window instance. If one doesn't exist, make one.
		if (EditorUtility.DisplayDialog(
 "Delete Serialized Data",
 "This deletes all player pref and serialized data, if anything goes wrong you can always blame Facu and hope everything will be ok. This cannot be undone. Facu's presence neither.",
 "Yes",
 "Never, I love my saved game!"))
		{
			DeleteSaveGame("Main");
			PlayerPrefs.DeleteAll();
		}
	}

	void OnGUI()
	{
		GUILayout.Label("Borrar Player Prefs", EditorStyles.boldLabel);
		if (GUILayout.Button(playerPrefButton))
		{
			PlayerPrefs.DeleteAll();
			playerPrefButton = "Reseted PlayerPrefs";
		}

		GUILayout.Label("Borrar datos serializados", EditorStyles.boldLabel);
		if (GUILayout.Button(deleteSerialized))
		{
			DeleteSaveGame("Main");
			deleteSerialized = "Datos serializados borrados";
		}

	}

	public static bool DeleteSaveGame(string name)
	{
		try
		{
			File.Delete(GetSavePath(name));
		}
		catch (Exception e)
		{
			Debug.Log("Exception1: " + e);
			return false;
		}
		return true;
	}

	private static string GetSavePath(string name)
	{
		return Path.Combine(Application.persistentDataPath, "save.artik");
		// return Path.Combine(Application.persistentDataPath, name + ".artiks");
	}
}

}

#endif