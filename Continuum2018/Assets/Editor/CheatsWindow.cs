using UnityEngine;
using UnityEditor;

public class CheatsWindow : EditorWindow
{
	public DeveloperMode developerModeScript;
	Vector2 scrollPos;
	string lastCheat;

	void OnApplicationQuit ()
	{
		ClearLastCheatStringLabel ();
	}

	void Start ()
	{
		ClearLastCheatStringLabel ();
	}

	[MenuItem ("Window/Cheats")]
	public static void ShowCheatsEditorWindow ()
	{
		GetWindow<CheatsWindow> ("Cheats");
	}

	void UpdateLastCheatButtonLabel ()
	{
		lastCheat = "Last cheat: " + DeveloperMode.Instance.LastCheatName;
	}

	void UpdateCurrentCheatButtonLabel ()
	{
		lastCheat = "Last cheat: " + DeveloperMode.Instance.CheatString;
	}

	void ClearLastCheatStringLabel ()
	{
		lastCheat = "Last cheat: ";
	}

	void OnGUI ()
	{
		if (developerModeScript == null) 
		{
			developerModeScript = FindObjectOfType <DeveloperMode> ();
		}

		var style = new GUIStyle (GUI.skin.button);
		style.normal.textColor = Color.black;
		style.stretchWidth = true;
		style.alignment = TextAnchor.MiddleCenter;

		GUILayout.BeginVertical ();

		GUI.backgroundColor = new Color (1f, 1f, 1f, 1); // Grey background.

		// Toggle cheats.
		if (GUILayout.Button ("Toggle cheats", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.ToggleCheatsCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		if (GUILayout.Button (lastCheat, style)) 
		{
			UpdateLastCheatButtonLabel ();
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.LastCheatName;
		}

		// Reset cheat string.
		if (GUILayout.Button ("Clear cheat string", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.ClearCheatString ();
			UpdateCurrentCheatButtonLabel ();
		}

		GUILayout.Label ("General cheats", EditorStyles.boldLabel);

		GUI.backgroundColor = new Color (1f, 0.9f, 0.0f, 1); // Yellow background.

		// Toggle God mode.
		if (GUILayout.Button ("Toggle God mode", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.ToggleGodmodeCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		GUI.backgroundColor = new Color (1.0f, 1.0f, 1.0f, 1); // White background.

		scrollPos = GUILayout.BeginScrollView (
			scrollPos, 
			false, 
			true, 
			GUILayout.ExpandWidth (true), 
			GUILayout.ExpandHeight (true) 
		);
			
		GUI.backgroundColor = new Color (1.0f, 0.5f, 0.75f, 1); // Grey background.

		// Force game over.
		if (GUILayout.Button ("Force game over", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.GameOverCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Force start.
		if (GUILayout.Button ("Force start", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.ForceStartCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Force restart.
		if (GUILayout.Button ("Force restart", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.ForceRestartCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Add score.
		if (GUILayout.Button ("Add 1,000,000 score", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.AddScoreCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Add life.
		if (GUILayout.Button ("Add 3 lives", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.AddLifeCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Lose life.
		if (GUILayout.Button ("Lose a life", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.LoseLifeCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Next wave.
		if (GUILayout.Button ("Next wave", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.NextWaveCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Previous wave.
		if (GUILayout.Button ("Previous wave", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.PreviousWaveCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Do bonus round.
		if (GUILayout.Button ("Do bonus round", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.DoBonusRoundCommand;
			UpdateCurrentCheatButtonLabel ();
		}
			
		GUILayout.Label ("Save cheats", EditorStyles.boldLabel);

		GUI.backgroundColor = new Color (0.0f, 0.8f, 1.0f, 1); // Dark grey background.

		// Save settings.
		if (GUILayout.Button ("Save settings", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.SaveSettingsCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Load settings.
		if (GUILayout.Button ("Load settings", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.LoadSettingsCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Set high quality settings.
		if (GUILayout.Button ("High quality settings", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.SetQualitySettingsHigh;
			UpdateCurrentCheatButtonLabel ();
		}

		// Set low quality settings.
		if (GUILayout.Button ("Low quality settings", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.SetQualitySettingsLow;
			UpdateCurrentCheatButtonLabel ();
		}
			
		GUILayout.Label ("General powerup cheats", EditorStyles.boldLabel);

		GUI.backgroundColor = new Color (0.5f, 0.75f, 1.0f, 1); // Pale blue background.

		// Reset powerup time.
		if (GUILayout.Button ("Reset powerup time", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.PowerupTimeCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Reset all powerups.
		if (GUILayout.Button ("Reset all powerups", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.ResetAllPowerupsCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		GUILayout.Label ("Shooting cheats", EditorStyles.boldLabel);
			
		// Standard shot.
		if (GUILayout.Button ("Standard shot", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.StandardShotCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Double shot.
		if (GUILayout.Button ("Double shot", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.DoubleShotCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Triple shot.
		if (GUILayout.Button ("Triple shot", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.TripleShotCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Ripple shot.
		if (GUILayout.Button ("Ripple shot", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.RippleShotCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Toggle overheat.
		if (GUILayout.Button ("Toggle overheat", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.UseOverheatCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		GUILayout.Label ("Other powerup cheats", EditorStyles.boldLabel);

		// Turret.
		if (GUILayout.Button ("Add turret", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.TurretCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Helix.
		if (GUILayout.Button ("Helix", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.HelixCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Slow time.
		if (GUILayout.Button ("Slow time", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.SlowTimeCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		GUILayout.Label ("Powerup modifier cheats", EditorStyles.boldLabel);

		GUI.backgroundColor = new Color (0.5f, 1f, 0.0f, 1); // Lime background.

		// Homing.
		if (GUILayout.Button ("Homing", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.HomingCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		GUI.backgroundColor = new Color (0.0f, 1f, 0.0f, 1); // Green background.

		// Ricochet.
		if (GUILayout.Button ("Ricochet", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.RicochetCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		GUI.backgroundColor = new Color (0.0f, 1.0f, 1.0f, 1); // Aqua background.

		// Rapidfire.
		if (GUILayout.Button ("Rapidfire", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.RapidfireCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		GUI.backgroundColor = new Color (0.75f, 0.4f, 0.75f, 1); // Purple background.

		// Overdrive.
		if (GUILayout.Button ("Overdrive", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.OverdriveCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		GUILayout.Label ("Ability cheats", EditorStyles.boldLabel);

		GUI.backgroundColor = new Color (0.75f, 0.9f, 1.0f, 1); // Pale blue background.

		// Charge ability.
		if (GUILayout.Button ("Charge ability", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.ChargeAbilityCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Refresh ability.
		if (GUILayout.Button ("Refresh ability", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.RefreshAbilityCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Shield ability.
		if (GUILayout.Button ("Shield ability", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.ShieldCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Emp ability.
		if (GUILayout.Button ("EMP ability", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.EmpCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Vertical beam ability.
		if (GUILayout.Button ("Vertical beam ability", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.VerticalBeamCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Horizontal beam ability.
		if (GUILayout.Button ("Horizontal beam ability", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.HorizontalBeamCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Rewind time ability.
		if (GUILayout.Button ("Rewind time ability", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.RewindCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Mirror player ability.
		if (GUILayout.Button ("Mirror player ability", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.MirrorPlayerCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		GUILayout.Label ("Audio cheats", EditorStyles.boldLabel);

		GUI.backgroundColor = new Color (1.0f, 0.75f, 0.0f, 1); // Orange background.

		// Next track.
		if (GUILayout.Button ("Next soundtrack", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.NextTrackCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Previous track.
		if (GUILayout.Button ("Previous soundtrack", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.PreviousTrackCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Next track.
		if (GUILayout.Button ("Random soundtrack", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.RandomTrackCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		GUILayout.Label ("Spawn cheats", EditorStyles.boldLabel);

		GUI.backgroundColor = new Color (0.9f, 1f, 0.9f, 1); // Pale green background.

		// Spawn a block.
		if (GUILayout.Button ("Spawn block prefab", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.SpawnBlockCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Spawn a powerup.
		if (GUILayout.Button ("Spawn powerup prefab", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.SpawnPowerupPickupCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Spawn a boss.
		if (GUILayout.Button ("Spawn mini boss prefab", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.SpawnMiniBossCommand;
			UpdateCurrentCheatButtonLabel ();
		}
			
		GUILayout.Label ("Framerate cheats", EditorStyles.boldLabel);

		GUI.backgroundColor = new Color (0.75f, 0.75f, 0.75f, 1); // Dark grey background.

		// FPS unlock.
		if (GUILayout.Button ("Unlock framerate", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.FpsUnlockCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// FPS 20.
		if (GUILayout.Button ("FPS limit = 20", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.Fps20Command;
			UpdateCurrentCheatButtonLabel ();
		}

		// FPS 30.
		if (GUILayout.Button ("FPS limit = 30", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.Fps30Command;
			UpdateCurrentCheatButtonLabel ();
		}

		// FPS 60.
		if (GUILayout.Button ("FPS limit = 60", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.Fps60Command;
			UpdateCurrentCheatButtonLabel ();
		}

		// FPS 90.
		if (GUILayout.Button ("FPS limit = 90", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.Fps90Command;
			UpdateCurrentCheatButtonLabel ();
		}

		// FPS 120.
		if (GUILayout.Button ("FPS limit = 120", style)) 
		{
			DeveloperMode.Instance.ResetCheatStringTimer ();
			DeveloperMode.Instance.CheatString = DeveloperMode.Instance.Fps120Command;
			UpdateCurrentCheatButtonLabel ();
		}

		GUILayout.EndScrollView ();
		GUILayout.EndVertical ();
	}
}