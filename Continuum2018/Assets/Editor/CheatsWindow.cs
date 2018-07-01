using UnityEngine;
using UnityEditor;

public class CheatsWindow : EditorWindow
{
	public DeveloperMode developerModeScript;
	Vector2 scrollPos;
	string lastCheat;

	[MenuItem ("Window/Cheats")]
	public static void ShowCheatsEditorWindow ()
	{
		GetWindow<CheatsWindow> ("Cheats");
	}

	void UpdateLastCheatButtonLabel ()
	{
		lastCheat = "Last cheat: " + developerModeScript.LastCheatName;
	}

	void UpdateCurrentCheatButtonLabel ()
	{
		lastCheat = "Last cheat: " + developerModeScript.CheatString;
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

		GUI.backgroundColor = new Color (0.75f, 0.75f, 0.75f, 1);
		GUILayout.BeginVertical ();

		scrollPos = GUILayout.BeginScrollView (
			scrollPos, 
			false, 
			true, 
			GUILayout.ExpandWidth (true), 
			GUILayout.ExpandHeight (true) 
		);

		GUILayout.Label ("Cheat list", EditorStyles.boldLabel);

		// Toggle cheats.
		if (GUILayout.Button ("Toggle cheats", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.ToggleCheatsCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		if (GUILayout.Button (lastCheat, style)) 
		{
			UpdateLastCheatButtonLabel ();
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.LastCheatName;
		}

		// Reset cheat string.
		if (GUILayout.Button ("Clear cheat string", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.ClearCheatString ();
			UpdateCurrentCheatButtonLabel ();
		}

		// Force start.
		if (GUILayout.Button ("Force start", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.ForceStartCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Force restart.
		if (GUILayout.Button ("Force restart", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.ForceRestartCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		GUILayout.Label ("General cheats", EditorStyles.boldLabel);

		// Toggle God mode.
		if (GUILayout.Button ("Toggle God mode", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.ToggleGodmodeCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Force game over.
		if (GUILayout.Button ("Force game over", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.GameOverCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Add score.
		if (GUILayout.Button ("Add 1,000,000 score", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.AddScoreCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Add life.
		if (GUILayout.Button ("Add 3 lives", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.AddLifeCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Lose life.
		if (GUILayout.Button ("Lose a life", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.LoseLifeCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Next wave.
		if (GUILayout.Button ("Next wave", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.NextWaveCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Previous wave.
		if (GUILayout.Button ("Previous wave", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.PreviousWaveCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Do bonus round.
		if (GUILayout.Button ("Do bonus round", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.DoBonusRoundCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		GUILayout.Label ("Framerate cheats", EditorStyles.boldLabel);

		// FPS unlock.
		if (GUILayout.Button ("Unlock framerate", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.FpsUnlockCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// FPS 20.
		if (GUILayout.Button ("FPS limit = 20", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.Fps20Command;
			UpdateCurrentCheatButtonLabel ();
		}

		// FPS 30.
		if (GUILayout.Button ("FPS limit = 30", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.Fps30Command;
			UpdateCurrentCheatButtonLabel ();
		}

		// FPS 60.
		if (GUILayout.Button ("FPS limit = 60", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.Fps60Command;
			UpdateCurrentCheatButtonLabel ();
		}

		// FPS 90.
		if (GUILayout.Button ("FPS limit = 90", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.Fps90Command;
			UpdateCurrentCheatButtonLabel ();
		}

		// FPS unlock.
		if (GUILayout.Button ("FPS limit = 120", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.Fps120Command;
			UpdateCurrentCheatButtonLabel ();
		}

		GUILayout.Label ("Save cheats", EditorStyles.boldLabel);

		// Save settings.
		if (GUILayout.Button ("Save settings", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.SaveSettingsCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Load settings.
		if (GUILayout.Button ("Load settings", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.LoadSettingsCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Set high quality settings.
		if (GUILayout.Button ("High quality settings", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.SetQualitySettingsHigh;
			UpdateCurrentCheatButtonLabel ();
		}

		// Set low quality settings.
		if (GUILayout.Button ("Low quality settings", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.SetQualitySettingsLow;
			UpdateCurrentCheatButtonLabel ();
		}
			
		GUILayout.Label ("General powerup cheats", EditorStyles.boldLabel);

		// Reset powerup time.
		if (GUILayout.Button ("Reset powerup time", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.PowerupTimeCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Reset all powerups.
		if (GUILayout.Button ("Reset all powerups", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.ResetAllPowerupsCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		GUILayout.Label ("Shooting cheats", EditorStyles.boldLabel);
			
		// Standard shot.
		if (GUILayout.Button ("Standard shot", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.StandardShotCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Double shot.
		if (GUILayout.Button ("Double shot", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.DoubleShotCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Triple shot.
		if (GUILayout.Button ("Triple shot", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.TripleShotCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Ripple shot.
		if (GUILayout.Button ("Ripple shot", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.RippleShotCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Toggle overheat.
		if (GUILayout.Button ("Toggle overheat", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.UseOverheatCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		GUILayout.Label ("Other powerup cheats", EditorStyles.boldLabel);

		// Turret.
		if (GUILayout.Button ("Add turret", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.TurretCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Helix.
		if (GUILayout.Button ("Helix", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.HelixCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Slow time.
		if (GUILayout.Button ("Slow time", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.SlowTimeCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		GUILayout.Label ("Powerup modifier cheats", EditorStyles.boldLabel);

		// Homing.
		if (GUILayout.Button ("Homing", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.HomingCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Ricochet.
		if (GUILayout.Button ("Ricochet", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.RicochetCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Rapidfire.
		if (GUILayout.Button ("Rapidfire", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.RapidfireCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Overdrive.
		if (GUILayout.Button ("Overdrive", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.OverdriveCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		GUILayout.Label ("Ability cheats", EditorStyles.boldLabel);

		// Charge ability.
		if (GUILayout.Button ("Charge ability", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.ChargeAbilityCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Refresh ability.
		if (GUILayout.Button ("Refresh ability", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.RefreshAbilityCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Shield ability.
		if (GUILayout.Button ("Shield ability", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.ShieldCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Emp ability.
		if (GUILayout.Button ("EMP ability", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.EmpCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Vertical beam ability.
		if (GUILayout.Button ("Vertical beam ability", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.VerticalBeamCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Horizontal beam ability.
		if (GUILayout.Button ("Horizontal beam ability", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.HorizontalBeamCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Rewind time ability.
		if (GUILayout.Button ("Rewind time ability", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.RewindCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Mirror player ability.
		if (GUILayout.Button ("Mirror player ability", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.MirrorPlayerCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		GUILayout.Label ("Audio cheats", EditorStyles.boldLabel);

		// Next track.
		if (GUILayout.Button ("Next soundtrack", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.NextTrackCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Previous track.
		if (GUILayout.Button ("Previous soundtrack", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.PreviousTrackCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Next track.
		if (GUILayout.Button ("Random soundtrack", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.RandomTrackCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		GUILayout.Label ("Spawn cheats", EditorStyles.boldLabel);

		// Spawn a block.
		if (GUILayout.Button ("Spawn block prefab", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.SpawnBlockCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Spawn a powerup.
		if (GUILayout.Button ("Spawn powerup prefab", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.SpawnPowerupPickupCommand;
			UpdateCurrentCheatButtonLabel ();
		}

		// Spawn a boss.
		if (GUILayout.Button ("Spawn mini boss prefab", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.SpawnMiniBossCommand;
			UpdateCurrentCheatButtonLabel ();
		}
			
		GUILayout.EndScrollView ();
		GUILayout.EndVertical ();
	}
}