using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SaveAndLoadScript))]
public class SaveWindow : Editor
{
	public override void OnInspectorGUI ()
	{
		SaveAndLoadScript savescript = (SaveAndLoadScript)target;

		var style = new GUIStyle (GUI.skin.button);
		style.normal.textColor = Color.black;
		style.fixedWidth = 200;
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

		if (GUILayout.Button ("Load Settings Data", style)) 
		{
			savescript.LoadSettingsData ();
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

		GUI.backgroundColor = Color.grey;

		if (GUILayout.Button ("Reset Leaderboards", style)) 
		{
			savescript.ResetAllLeaderboards ();
		}

		GUI.backgroundColor = Color.white;
		DrawDefaultInspector ();
	}
}