using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeveloperMode : MonoBehaviour 
{
	public PlayerController playerControllerScript_P1;
	public GameController gameControllerScript;
	public TimescaleController timeScaleControllerScript;
	public LocalSceneLoader localSceneLoaderScript;
	public AudioController audioControllerScript;
	public SaveAndLoadScript saveAndLoadScript;
	public TargetFPS targetFramerateScript;
	public bool forceStarted;

	[Header ("Cheats")]
	public string CheatString;
	public string LastCheatName;
	public float CheatStringResetTimeRemaining = 1.0f;
	public float CheatStringResetDuration = 1.0f;
	public int maxCheatCharacters = 12;

	public bool useCheats;
	public bool allowCheats = false;
	public bool showCheats;

	public GameObject CheatsMenu;
	public Animator CheatsMenuAnim;
	public Animator CheatNotificationAnim;
	public TextMeshProUGUI CheatNotifcationText;
	public GameObject CheatSound;

	public Texture2D DoubleShotTexture;
	public Texture2D TripleShotTexture;

	[Header ("Debug Menu")]
	public bool showDebugMenu;
	public GameObject DebugMenu;
	public Animator DebugMenuAnim;

	void Start () 
	{
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();
	}

	void Update () 
	{
		if (useCheats == true) 
		{
			UpdateDebugMenu ();
		}

		if (allowCheats == true)
		{
			UpdateCheats ();
		}
	}

	void UpdateDebugMenu ()
	{
		if (playerControllerScript_P1.playerActions.DebugMenu.WasPressed) 
		{
			showDebugMenu = !showDebugMenu;

			if (showDebugMenu == true)
			{
				DebugMenuAnim.Play ("DebugMenuEnter");
				CheatsMenuAnim.Play ("CheatsMenuEnter");
			}

			if (showDebugMenu == false) 
			{
				DebugMenuAnim.Play ("DebugMenuExit");
				CheatsMenuAnim.Play ("CheatsMenuExit");
			}
		}
	}

	public void ClearCheatString ()
	{
		CheatString = "";
	}

	void UpdateCheats ()
	{
		if (CheatStringResetTimeRemaining > 0) 
		{
			CheatStringResetTimeRemaining -= Time.unscaledDeltaTime;
		}

		if (CheatStringResetTimeRemaining <= 0) 
		{
			ClearCheatString ();
		}

		foreach (char c in Input.inputString) 
		{		
			CheatStringResetTimeRemaining = CheatStringResetDuration;
			CheatString += c;

			if (CheatString.Contains (c.ToString() + c.ToString() + c.ToString())) 
			{
				ClearCheatString ();
			}
		}

		if (CheatString.Length > maxCheatCharacters) 
		{
			ClearCheatString ();
		}

		if (Input.GetKeyDown (KeyCode.Backspace)) 
		{
			ClearCheatString ();
		}

		if ((CheatString == "continuum" || 
			CheatString == "CONTINUUM" || 
			CheatString == "Continuum")
			&& allowCheats == true) 
		{
			useCheats = !useCheats;

			if (useCheats == true) 
			{
				Debug.Log ("Enabled cheats.");
				ShowCheatActivation ("CHEATS ON");
			}

			if (useCheats == false) 
			{
				Debug.Log ("Disabled cheats.");
				ShowCheatActivation ("CHEATS OFF");
			}
		}
			
		if (useCheats == true)
		{
			// Insert cheats here.
			if (CheatString == "start" || 
				CheatString == "Start" || 
				CheatString == "START") 
			{
				if (forceStarted == false) 
				{
					timeScaleControllerScript.SwitchInitialSequence ();
					playerControllerScript_P1.StartCoroutines ();
					ShowCheatNotification ("CHEAT ACTIVATED: FORCE START");
					forceStarted = true;
				}
			}

			if (CheatString == "restart" || 
				CheatString == "Restart" || 
				CheatString == "RESTART") 
			{
				localSceneLoaderScript.sceneLoaderScript.SceneName = SceneManager.GetActiveScene().name;
				localSceneLoaderScript.sceneLoaderScript.StartLoadSequence ();
				ShowCheatNotification ("");
			}

			if (CheatString == "fpsunlock" || 
				CheatString == "FPSUNLOCK") 
			{
				targetFramerateScript.SetTargetFramerate (-1);
				ShowCheatNotification ("CHEAT ACTIVATED: FPS: -1");
			}

			if (CheatString == "fps60" || 
				CheatString == "FPS60") 
			{
				targetFramerateScript.SetTargetFramerate (60);
				ShowCheatNotification ("CHEAT ACTIVATED: FPS: 60");
			}

			if (CheatString == "fps30" || 
				CheatString == "FPS30") 
			{
				targetFramerateScript.SetTargetFramerate (30);
				ShowCheatNotification ("CHEAT ACTIVATED: FPS: 30");
			}

			if (CheatString == "fps90" || 
				CheatString == "FPS90") 
			{
				targetFramerateScript.SetTargetFramerate (90);
				ShowCheatNotification ("CHEAT ACTIVATED: FPS: 90");
			}

			if (CheatString == "fps120" || 
				CheatString == "FPS120") 
			{
				targetFramerateScript.SetTargetFramerate (120);
				ShowCheatNotification ("CHEAT ACTIVATED: FPS: 120");
			}

			if (CheatString == "nexttrack" || 
				CheatString == "NEXTTRACK") 
			{
				audioControllerScript.NextTrack ();
				ShowCheatNotification ("CHEAT ACTIVATED: NEXT TRACK");
			}

			if (CheatString == "previoustrack" || 
				CheatString == "PREVIOUSTRACK") 
			{
				audioControllerScript.PreviousTrack ();
				ShowCheatNotification ("CHEAT ACTIVATED: PREVIOUS TRACK");
			}

			if (CheatString == "randomtrack" || 
				CheatString == "RANDOMTRACK") 
			{
				audioControllerScript.RandomTrack ();
				ShowCheatNotification ("CHEAT ACTIVATED: RANDOM TRACK");
			}

			if (CheatString == "savesettings" || 
				CheatString == "SAVESETTINGS") 
			{
				saveAndLoadScript.SaveSettingsData ();
				ShowCheatNotification ("CHEAT ACTIVATED: SETTINGS SAVE");
			}

			if (CheatString == "loadsettings" || 
				CheatString == "LOADSETTINGS") 
			{
				saveAndLoadScript.LoadSettingsData ();
				ShowCheatNotification ("CHEAT ACTIVATED: SETTINGS LOAD");
			}

			if (CheatString == "life" || 
				CheatString == "LIFE") 
			{
				gameControllerScript.Lives += 3;
				ShowCheatNotification ("CHEAT ACTIVATED: LIFE");
			}

			if (CheatString == "god" || 
				CheatString == "GOD") 
			{
				playerControllerScript_P1.playerCol.enabled = !playerControllerScript_P1.playerCol.enabled;

				if (playerControllerScript_P1.playerCol.enabled == true) 
				{
					ShowCheatNotification ("CHEAT ACTIVATED: GOD ON");
				}

				if (playerControllerScript_P1.playerCol.enabled == false) 
				{
					ShowCheatNotification ("CHEAT ACTIVATED: GOD OFF");
				}
			}

			if (CheatString == "chargeability" || 
				CheatString == "CHARGEABILITY") 
			{
				playerControllerScript_P1.CurrentAbilityTimeRemaining = playerControllerScript_P1.CurrentAbilityDuration;
				ShowCheatNotification ("CHEAT ACTIVATED: ABILITY CHARGED");
			}

			if (CheatString == "refreshability" || 
				CheatString == "REFRESHABILITY") 
			{
				playerControllerScript_P1.RefreshAbilityName ();
				ShowCheatNotification ("CHEAT ACTIVATED: ABILITY REFRESH");
			}

			if (CheatString == "spblock") 
			{
				gameControllerScript.SpawnBlock (true);
				ShowCheatNotification ("CHEAT ACTIVATED: SPAWN BLOCK");
			}

			if (CheatString == "sppow") 
			{
				gameControllerScript.SpawnPowerupPickup ();
				ShowCheatNotification ("CHEAT ACTIVATED: SPAWN POWERUP");
			}

			if (CheatString == "poweruptime") 
			{
				gameControllerScript.PowerupTimeRemaining = gameControllerScript.PowerupTimeDuration;
				ShowCheatNotification ("CHEAT ACTIVATED: POWERUP TIME REFRESH");
			}

			if (CheatString == "double" || 
				CheatString == "DOUBLE") 
			{
				playerControllerScript_P1.TripleShotIteration = PlayerController.shotIteration.Standard;
				playerControllerScript_P1.ShotType = PlayerController.shotType.Double;
				playerControllerScript_P1.NextTripleShotIteration = 0;

				// Apply tweaks to conditions based on which iteration the player is on.
				switch (playerControllerScript_P1.NextDoubleShotIteration) 
				{
				case 0:
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.DoubleShotFireRates [0];
					gameControllerScript.SetPowerupTime (20);
					break;
				case 1:
					gameControllerScript.SetPowerupTime (20);
					break;
				case 2:
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.DoubleShotFireRates [1];
					gameControllerScript.SetPowerupTime (20);
					break;
				case 3:
					gameControllerScript.SetPowerupTime (5);
					break;
				}
					
				// Sets double shot iteration (enum) as the next double shot iteration (int).
				if (playerControllerScript_P1.NextDoubleShotIteration < 4) 
				{
					playerControllerScript_P1.DoubleShotIteration = 
						(PlayerController.shotIteration)playerControllerScript_P1.NextDoubleShotIteration;
				}

				// Increases iteration count.
				if (playerControllerScript_P1.NextDoubleShotIteration < 4) 
				{
					playerControllerScript_P1.NextDoubleShotIteration += 1;
				}

				gameControllerScript.PowerupShootingImage_P1.texture = DoubleShotTexture;
				gameControllerScript.PowerupShootingImage_P1.color = new Color (1, 1, 1, 1);
				gameControllerScript.PowerupShootingText_P1.text = "" + playerControllerScript_P1.DoubleShotIteration.ToString ();

				ShowCheatNotification ("CHEAT ACTIVATED: DOUBLE SHOT: " + playerControllerScript_P1.DoubleShotIteration.ToString ());
			}

			if (CheatString == "triple" || 
				CheatString == "TRIPLE") 
			{
				playerControllerScript_P1.DoubleShotIteration = PlayerController.shotIteration.Standard;
				playerControllerScript_P1.NextDoubleShotIteration = 0;
				playerControllerScript_P1.ShotType = PlayerController.shotType.Triple;

				// Apply tweaks to conditions based on which iteration the player is on.
				switch (playerControllerScript_P1.NextTripleShotIteration) 
				{
				case 0:
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.TripleShotFireRates [0];
					gameControllerScript.SetPowerupTime (20);
					break;
				case 1:
					gameControllerScript.SetPowerupTime (20);
					break;
				case 2:
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.TripleShotFireRates [1];
					gameControllerScript.SetPowerupTime (20);
					break;
				case 3:
					gameControllerScript.SetPowerupTime (5);
					break;
				}

				// Sets double shot iteration (enum) as the next double shot iteration (int).
				if (playerControllerScript_P1.NextTripleShotIteration < 4) 
				{
					playerControllerScript_P1.TripleShotIteration = 
						(PlayerController.shotIteration)playerControllerScript_P1.NextTripleShotIteration;
				}

				// Increases iteration count.
				if (playerControllerScript_P1.NextTripleShotIteration < 4) 
				{
					playerControllerScript_P1.NextTripleShotIteration += 1;
				}

				gameControllerScript.PowerupShootingImage_P1.texture = TripleShotTexture;
				gameControllerScript.PowerupShootingImage_P1.color = new Color (1, 1, 1, 1);
				gameControllerScript.PowerupShootingText_P1.text = "" + playerControllerScript_P1.TripleShotIteration.ToString ();

				ShowCheatNotification ("CHEAT ACTIVATED: TRIPLE SHOT: " + playerControllerScript_P1.TripleShotIteration.ToString ());
			}

			if (CheatString == "clone") 
			{
				playerControllerScript_P1.Clone.SetActive (true);
				ShowCheatNotification ("CHEAT ACTIVATED: CLONE");
			}

			if (CheatString == "nextwave") 
			{
				gameControllerScript.Wave += 1;
				gameControllerScript.WaveText.text = "WAVE " + gameControllerScript.Wave;
				ShowCheatNotification ("CHEAT ACTIVATED: NEXT WAVE");
			}

			if (CheatString == "lastwave") 
			{
				if (gameControllerScript.Wave > 1) 
				{
					gameControllerScript.Wave -= 1;
				}

				gameControllerScript.WaveText.text = "WAVE " + gameControllerScript.Wave;
				ShowCheatNotification ("CHEAT ACTIVATED: LAST WAVE");
			}

			if (Input.GetKeyDown (KeyCode.Alpha7)) 
			{
				CheatString = LastCheatName;
			}
		}
	}

	void ShowCheatActivation (string cheatText)
	{
		CheatNotificationAnim.Play ("CheatNotification");
		CheatNotifcationText.text = cheatText;
		Instantiate (CheatSound, transform.position, Quaternion.identity);
		ClearCheatString ();
		Debug.Log (cheatText);
	}

	void ShowCheatNotification (string cheatText)
	{
		CheatNotificationAnim.StopPlayback ();
		CheatNotificationAnim.Play ("CheatNotification");
		CheatNotifcationText.text = cheatText;
		Instantiate (CheatSound, transform.position, Quaternion.identity);
		LastCheatName = CheatString;
		ClearCheatString ();
		Debug.Log (cheatText);
	}
}
