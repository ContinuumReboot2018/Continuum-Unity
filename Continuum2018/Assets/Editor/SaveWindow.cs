using UnityEngine;
using UnityEditor;

public class SaveWindow : EditorWindow
{
	static SaveAndLoadScript savescript;

	[MenuItem ("Window/Save Editor")]
	public static void ShowSaveEditorWindow ()
	{
		GetWindow<SaveWindow> ("Save Editor");
	}

	[MenuItem ("Window/Open save folder location")]
	public static void ShowSaveLocationWindow ()
	{
		string savefolder = Application.persistentDataPath + "/";
		Application.OpenURL (savefolder);
		Debug.Log ("Opened save folder at " + savefolder);
	}

	void OnGUI ()
	{
		savescript = FindObjectOfType<SaveAndLoadScript> ();
		//SaveAndLoadScript savescript = (SaveAndLoadScript)FindObjectOfType (typeof(SaveAndLoadScript));

		GUILayout.Label ("Add/Remove your save files here.", EditorStyles.boldLabel);

		var style = new GUIStyle (GUI.skin.button);
		style.normal.textColor = Color.black;
		style.stretchWidth = true;
		style.alignment = TextAnchor.MiddleCenter;

		GUI.backgroundColor = new Color (0.75f, 0.75f, 1, 1);

		if (GUILayout.Button ("Save Player Data", style)) 
		{
			savescript.SavePlayerData ();
		}

		if (GUILayout.Button ("Load Player Data", style)) 
		{
			savescript.LoadPlayerData ();
		}

		GUI.backgroundColor = new Color (0.4f, 0.9f, 1, 1);

		if (GUILayout.Button ("Save Settings Data", style)) 
		{
			savescript.SaveSettingsData ();
		}

		if (Application.isPlaying == true) 
		{
			if (GUILayout.Button ("Load Settings Data", style)) 
			{
				savescript.LoadSettingsData ();
			}
		}

		GUI.backgroundColor = new Color (0.5f, 1, 0.5f, 1);

		if (GUILayout.Button ("Create Player Data", style)) 
		{
			savescript.SavePlayerData ();
		}

		if (GUILayout.Button ("Create Settings Data", style)) 
		{
			savescript.SaveSettingsData ();
		}

		GUI.backgroundColor = new Color (1, 0.5f, 0.5f, 1);

		if (GUILayout.Button ("Delete Player Data (Editor)", style)) 
		{
			savescript.DeletePlayerDataEditor ();
		}

		if (GUILayout.Button ("Delete Player Data", style)) 
		{
			savescript.DeletePlayerDataMain ();
		}

		if (GUILayout.Button ("Delete Settings Data (Editor)", style)) 
		{
			savescript.DeleteSettingsDataEditor ();
		}

		if (GUILayout.Button ("Delete Settings Data", style)) 
		{
			savescript.DeleteSettingsDataMain ();
		}

		GUI.backgroundColor = new Color (0.75f, 0.75f, 0.75f, 1);

		if (GUILayout.Button ("Reset Leaderboards", style)) 
		{
			savescript.ResetAllLeaderboards ();
		}

		GUI.backgroundColor = Color.white;
	}
}