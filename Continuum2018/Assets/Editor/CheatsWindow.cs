using UnityEngine;
using UnityEditor;

public class CheatsWindow : EditorWindow
{
	public DeveloperMode developerModeScript;
	Vector2 scrollPos;

	[MenuItem ("Window/Cheats")]
	public static void ShowCheatsEditorWindow ()
	{
		GetWindow<CheatsWindow> ("Cheats");
	}

	void OnGUI ()
	{
		developerModeScript = FindObjectOfType <DeveloperMode> ();



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
		}

		// Reset cheat string.
		if (GUILayout.Button ("Clear cheat string", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.ClearCheatString ();
		}

		// Force start.
		if (GUILayout.Button ("Force start", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.ForceStartCommand;
		}

		// Force restart.
		if (GUILayout.Button ("Force restart", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.ForceRestartCommand;
		}

		GUILayout.Label ("General cheats", EditorStyles.boldLabel);

		// Toggle God mode.
		if (GUILayout.Button ("Toggle God mode", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.ToggleGodmodeCommand;
		}

		// Force game over.
		if (GUILayout.Button ("Force game over", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.GameOverCommand;
		}

		// Add score.
		if (GUILayout.Button ("Add 1,000,000 score", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.AddScoreCommand;
		}

		// Add life.
		if (GUILayout.Button ("Add 3 lives", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.AddLifeCommand;
		}

		// Lose life.
		if (GUILayout.Button ("Lose a life", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.LoseLifeCommand;
		}

		// Next wave.
		if (GUILayout.Button ("Next wave", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.NextWaveCommand;
		}

		// Previous wave.
		if (GUILayout.Button ("Previous wave", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.PreviousWaveCommand;
		}

		// Do bonus round.
		if (GUILayout.Button ("Do bonus round", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.DoBonusRoundCommand;
		}

		GUILayout.Label ("Framerate cheats", EditorStyles.boldLabel);

		// FPS unlock.
		if (GUILayout.Button ("Unlock framerate", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.FpsUnlockCommand;
		}

		// FPS 20.
		if (GUILayout.Button ("FPS limit = 20", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.Fps20Command;
		}

		// FPS 30.
		if (GUILayout.Button ("FPS limit = 30", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.Fps30Command;
		}

		// FPS 60.
		if (GUILayout.Button ("FPS limit = 60", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.Fps60Command;
		}

		// FPS 90.
		if (GUILayout.Button ("FPS limit = 90", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.Fps90Command;
		}

		// FPS unlock.
		if (GUILayout.Button ("FPS limit = 120", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.Fps120Command;
		}

		GUILayout.Label ("Save cheats", EditorStyles.boldLabel);

		// Save settings.
		if (GUILayout.Button ("Save settings", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.SaveSettingsCommand;
		}

		// Load settings.
		if (GUILayout.Button ("Load settings", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.LoadSettingsCommand;
		}

		// Set high quality settings.
		if (GUILayout.Button ("High quality settings", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.SetQualitySettingsHigh;
		}

		// Set low quality settings.
		if (GUILayout.Button ("Low quality settings", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.SetQualitySettingsLow;
		}
			
		GUILayout.Label ("General powerup cheats", EditorStyles.boldLabel);

		// Reset powerup time.
		if (GUILayout.Button ("Reset powerup time", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.PowerupTimeCommand;
		}

		// Reset all powerups.
		if (GUILayout.Button ("Reset all powerups", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.ResetAllPowerupsCommand;
		}

		GUILayout.Label ("Shooting cheats", EditorStyles.boldLabel);
			
		// Standard shot.
		if (GUILayout.Button ("Standard shot", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.StandardShotCommand;
		}

		// Double shot.
		if (GUILayout.Button ("Double shot", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.DoubleShotCommand;
		}

		// Triple shot.
		if (GUILayout.Button ("Triple shot", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.TripleShotCommand;
		}

		// Ripple shot.
		if (GUILayout.Button ("Ripple shot", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.RippleShotCommand;
		}

		// Toggle overheat.
		if (GUILayout.Button ("Toggle overheat", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.UseOverheatCommand;
		}

		GUILayout.Label ("Other powerup cheats", EditorStyles.boldLabel);

		// Turret.
		if (GUILayout.Button ("Add turret", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.TurretCommand;
		}

		// Helix.
		if (GUILayout.Button ("Helix", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.HelixCommand;
		}

		// Slow time.
		if (GUILayout.Button ("Slow time", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.SlowTimeCommand;
		}

		GUILayout.Label ("Powerup modifier cheats", EditorStyles.boldLabel);

		// Homing.
		if (GUILayout.Button ("Homing", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.HomingCommand;
		}

		// Ricochet.
		if (GUILayout.Button ("Ricochet", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.RicochetCommand;
		}

		// Rapidfire.
		if (GUILayout.Button ("Rapidfire", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.RapidfireCommand;
		}

		// Overdrive.
		if (GUILayout.Button ("Overdrive", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.OverdriveCommand;
		}

		GUILayout.Label ("Ability cheats", EditorStyles.boldLabel);

		// Charge ability.
		if (GUILayout.Button ("Charge ability", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.ChargeAbilityCommand;
		}

		// Refresh ability.
		if (GUILayout.Button ("Refresh ability", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.RefreshAbilityCommand;
		}

		// Shield ability.
		if (GUILayout.Button ("Shield ability", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.ShieldCommand;
		}

		// Emp ability.
		if (GUILayout.Button ("EMP ability", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.EmpCommand;
		}

		// Vertical beam ability.
		if (GUILayout.Button ("Vertical beam ability", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.VerticalBeamCommand;
		}

		// Horizontal beam ability.
		if (GUILayout.Button ("Horizontal beam ability", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.HorizontalBeamCommand;
		}

		// Rewind time ability.
		if (GUILayout.Button ("Rewind time ability", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.RewindCommand;
		}

		// Mirror player ability.
		if (GUILayout.Button ("Mirror player ability", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.MirrorPlayerCommand;
		}

		GUILayout.Label ("Audio cheats", EditorStyles.boldLabel);

		// Next track.
		if (GUILayout.Button ("Next soundtrack", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.NextTrackCommand;
		}

		// Previous track.
		if (GUILayout.Button ("Previous soundtrack", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.PreviousTrackCommand;
		}

		// Next track.
		if (GUILayout.Button ("Random soundtrack", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.RandomTrackCommand;
		}

		GUILayout.Label ("Spawn cheats", EditorStyles.boldLabel);

		// Spawn a block.
		if (GUILayout.Button ("Spawn block prefab", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.SpawnBlockCommand;
		}

		// Spawn a powerup.
		if (GUILayout.Button ("Spawn powerup prefab", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.SpawnPowerupPickupCommand;
		}

		// Spawn a boss.
		if (GUILayout.Button ("Spawn mini boss prefab", style)) 
		{
			developerModeScript.ResetCheatStringTimer ();
			developerModeScript.CheatString = developerModeScript.SpawnMiniBossCommand;
		}
			
		GUILayout.EndScrollView ();
		GUILayout.EndVertical ();
	}
}