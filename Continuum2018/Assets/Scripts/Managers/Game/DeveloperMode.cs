using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;

public class DeveloperMode : MonoBehaviour 
{
	public static DeveloperMode Instance { get ; private set; }

	public PlayerController playerControllerScript_P1;

	public bool forceStarted; // Has the tutorial been skipped via force start command?

	[Header ("Cheats")]
	public string CheatString; // Current cheat string field value.
	public string LastCheatName; // Logs last cheat name, referenced when player loads last cheat again.
	public float CheatStringResetTimeRemaining = 1.0f; // Current time remaining before the cheat string is reset.
	public float CheatStringResetDuration = 1.0f; // How long for the cheat string can remain idle until it is reset.
	public int maxCheatCharacters = 12; // Cheat character limit.
	public bool useCheats; // Toggles if cheats are being used.
	public bool allowCheats = false; // Toggles if cheats are allowed.
	public bool showCheats; // Toggles if cheats are being shown or not.
	public bool isGod; // Toggles if god mode is on or off.
	[Space (10)]
	public int AddScoreAmount = 100000; // Score added when addscore cheat is activated.

	[Header ("Cheat Commands")]
	// General cheats.
	public string ToggleCheatsCommand = "continuum"; // Toggles cheats ON or OFF.
	public string ForceStartCommand = "start"; // Force starts the wave sequence and skips the tutorial.
	public string ForceRestartCommand = "restart"; // Loads the scene again, taken to scene transition UI.
	[Space (10)]
	// FPS testing cheats.
	public string FpsUnlockCommand = "fpsunlock"; // Unlocks framerate.
	public string Fps20Command = "fps20"; // Target framerate set to 20.
	public string Fps30Command = "fps30"; // Target framerate set to 30.
	public string Fps60Command = "fps60"; // Target framerate set to 60.
	public string Fps90Command = "fps90"; // Target framerate set to 90.
	public string Fps120Command = "fps120"; // Target framerate set to 120.
	[Space (10)]
	// Audio cheats.
	public string NextTrackCommand = "nexttrack"; // Audio manager to load the next soundtrack.
	public string PreviousTrackCommand = "previoustrack"; // Audio manager to load the previous soundtrack.
	public string RandomTrackCommand = "randomtrack"; // Audio manager to load a random soundtrack.
	[Space (10)]
	// Saving / loading cheats.
	public string SaveSettingsCommand = "savesettings"; // Saves app settings to a file.
	public string LoadSettingsCommand = "loadsettings"; // Loads app settings to a file.
	[Space (10)]
	// Spawning cheats.
	public string SpawnBlockCommand = "spblock"; // Spawns a random block.
	public string SpawnPowerupPickupCommand = "sppow"; // Spawns a random powerup.
	public string SpawnMiniBossCommand = "spminiboss"; // Spawns a random mini boss.
	[Space (10)]
	// Powerup cheats.
	public string PowerupTimeCommand = "poweruptime"; // Replenishes powerup time.
	public string ResetAllPowerupsCommand = "resetpow"; // Resets all powerups.
	[Space (10)]
	// Shooting powerup cheats.
	public string StandardShotCommand = "standard"; // Player shooting to standard shot.
	public string DoubleShotCommand = "double"; // Player shooting to double shot.
	public string TripleShotCommand = "triple"; // Player shooting to triple shot.
	public string RippleShotCommand = "ripple"; // Player shooting to ripple shot.
	[Space (10)]
	// Other powerup cheats.
	public string TurretCommand = "Turret"; // Player gets  a turret (Maximum 4).
	public string HelixCommand = "helix"; // Player gets a helix.
	public string SlowTimeCommand = "slowtime"; // Slows time down temporarily.
	[Space (10)]
	// Powerup modifier cheats.
	public string RapidfireCommand = "rapid"; // Toggles rapid fire mode ON or OFF.
	public string OverdriveCommand = "overdrive"; // Toggle overdrive mode ON or OFF.
	public string RicochetCommand = "ricochet"; // Toggle ricochet mode ON or OFF.
	public string HomingCommand = "homing"; // Toggle hominh mode ON or OFF.
	[Space (10)]
	// Ability cheats.
	public string ChargeAbilityCommand = "chargeability"; // Charges ability bar and sets to ready.
	public string RefreshAbilityCommand = "refreshability"; // Refreshes ability name and stats.
	public string ShieldCommand = "shield"; // Set ability to shield and turn on the shield + ability timer.
	public string VerticalBeamCommand = "vbeam"; // Set ability to vertical beam and turn on the shield + ability timer.
	public string HorizontalBeamCommand = "hbeam"; // Set ability to horizontal and turn on the shield + ability timer.
	public string EmpCommand = "emp"; // Set ability to emp blast and turn on the shield + ability timer.
	public string RewindCommand = "rewind"; // All GameObjects with a TimeBody script should rewind position and rotation for some time.
	public string MirrorPlayerCommand = "mirror"; // Enables a clone which mirrors the player.
	[Space (10)]
	// Misc cheats.
	public string AddScoreCommand = "addscore"; // Adds current score by AddScoreAmount value.
	public string LoseLifeCommand = "loselife"; // Loses a life.
	public string GameOverCommand = "gameover"; // Force a game over regardless of how many lives remain.
	public string ToggleGodmodeCommand = "god"; // Toggle god mode ON or OFF.
	public string AddLifeCommand = "life"; // Adds lives to player.
	public string NextWaveCommand = "nextwave"; // Wave number increases.
	public string PreviousWaveCommand = "lastwave"; // Wave number decreases.
	public string UseOverheatCommand = "useoverheat"; // Allows overheating or not.
	public string DoBonusRoundCommand = "dobonus"; // Sets up bonus round.
	public string SetQualitySettingsHigh = "highqual"; // Sets quality settings to high.
	public string SetQualitySettingsLow = "lowqual"; // Sets quality settings to low.

	[Header ("UI and Animations")]
	public GameObject CheatsMenu; // Cheat menu for viewing possible cheats.
	public Animator CheatsMenuAnim; // Animator for cheat menu.
	public Animator CheatNotificationAnim; // Animator for cheat notification UI.
	public TextMeshProUGUI CheatNotifcationText; // Text for cheat noticiation UI.
	public GameObject CheatSound; // Sound to play when a cheat is activated.
	[Space (10)]
	public Texture2D DoubleShotTexture; // Double shot display texture.
	public Texture2D TripleShotTexture; // Triple shot display texture.
	public Texture2D RippleShotTexture; // Ripple shot display texture.
	public Texture2D TurretTexture; // Turret display texture.
	public Texture2D HelixTexture; // Helix display texture.
	public Texture2D SlowTimeTexture; // Slow time texture.
	public Texture2D RicochetTexture; // Ricochet texture.
	public Texture2D RapidfireTexture; // Rapidfire texture.
	public Texture2D OverdriveTexture; // Overdrive texture.
	public Texture2D HomingTexture; // Homing texture.
	public Texture2D AddlifeTexture; // Add life texture.

	[Header ("Debug Menu")]
	public bool showDebugMenu; // Toggles whether debug menu is visible.
	public GameObject DebugMenu; // Debug menu object for viewing stats.
	public Animator DebugMenuAnim; // Animator for the debug menu object.

	[Header ("Cheat Console")]
	public GameObject CheatConsole; // Visual input keycodes by user.
	public TextMeshProUGUI CheatInputText; // Current input visual text.

	void Awake ()
	{
		Instance = this;
		// DontDestroyOnLoad (gameObject);
	}

	// Check for cheat input by keyboard.
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

	// Shows/hides debug and cheat menus.
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

	// Resets cheat string.
	public void ClearCheatString ()
	{
		CheatString = "";
	}

	public void ResetCheatStringTimer ()
	{
		CheatStringResetTimeRemaining = CheatStringResetDuration;
	}

	public void UpdateCheats ()
	{
		// Timer for cheats.
		if (CheatStringResetTimeRemaining > 0) 
		{
			CheatStringResetTimeRemaining -= Time.unscaledDeltaTime;
		}

		foreach (char c in Input.inputString) 
		{		
			// Reset cheat timer for each keystroke.
			CheatStringResetTimeRemaining = CheatStringResetDuration;
			CheatString += c; // Add character to cheat string.

			// Reset cheat string by triple duplicate of same character.
			//if (CheatString.Contains (c.ToString () + c.ToString () + c.ToString ()))
			//{
			//	ClearCheatString ();
			//}

			// Reset cheat string by backquote character.
			if (CheatString.Contains ("`")) 
			{
				ClearCheatString ();
			}
		}

		// Reset cheat string by character length.
		if (CheatString.Length > maxCheatCharacters) 
		{
			ClearCheatString ();
		}

		// Reset cheat string by backspacing.
		if (Input.GetKeyDown (KeyCode.Backspace)) 
		{
			ClearCheatString ();
		}

		// Toggle cheat activation.
		if (CheatString.Contains (ToggleCheatsCommand)
		   && allowCheats == true)
		{
			useCheats = !useCheats;

			if (useCheats == true) 
			{
				ShowCheatActivation ("CHEATS ON");
			}

			if (useCheats == false)
			{
				ShowCheatActivation ("CHEATS OFF");
			}
		}

		// Cheat time remain is 0, ignore checking for cheats.
		if (CheatStringResetTimeRemaining <= 0) 
		{
			ClearCheatString ();
			return;
		}
		
		if (useCheats == true) 
		{
			// Insert cheats here.

			if (CheatString == ForceStartCommand) 
			{
				if (forceStarted == false) 
				{
					TutorialManager.Instance.TurnOffTutorial (true);
					ShowCheatNotification ("CHEAT ACTIVATED: FORCE START");
					forceStarted = true;
				}
			}

			if (CheatString == ForceRestartCommand) 
			{
				AudioController.Instance.FadeOutSceneAudio ();
				SceneLoader.Instance.SceneName = SceneManager.GetActiveScene ().name;
				SceneLoader.Instance.StartLoadSequence ();
				ShowCheatNotification ("");
			}

			if (CheatString == FpsUnlockCommand) 
			{
				TargetFPS.Instance.SetTargetFramerate (-1);
				ShowCheatNotification ("CHEAT ACTIVATED: FPS: -1");
			}

			if (CheatString == Fps60Command) 
			{
				TargetFPS.Instance.SetTargetFramerate (60);
				ShowCheatNotification ("CHEAT ACTIVATED: FPS: 60");
			}

			if (CheatString == Fps30Command) 
			{
				TargetFPS.Instance.SetTargetFramerate (30);
				ShowCheatNotification ("CHEAT ACTIVATED: FPS: 30");
			}

			if (CheatString == Fps20Command) 
			{
				TargetFPS.Instance.SetTargetFramerate (20);
				ShowCheatNotification ("CHEAT ACTIVATED: FPS: 20");
			}

			if (CheatString == Fps90Command) 
			{
				TargetFPS.Instance.SetTargetFramerate (90);
				ShowCheatNotification ("CHEAT ACTIVATED: FPS: 90");
			}

			if (CheatString == Fps120Command) 
			{
				TargetFPS.Instance.SetTargetFramerate (120);
				ShowCheatNotification ("CHEAT ACTIVATED: FPS: 120");
			}

			if (CheatString == NextTrackCommand) 
			{
				AudioController.Instance.NextTrack ();
				ShowCheatNotification ("CHEAT ACTIVATED: NEXT TRACK");
			}

			if (CheatString == PreviousTrackCommand) 
			{
				AudioController.Instance.PreviousTrack ();
				ShowCheatNotification ("CHEAT ACTIVATED: PREVIOUS TRACK");
			}

			if (CheatString == RandomTrackCommand)
			{
				AudioController.Instance.RandomTrack ();
				ShowCheatNotification ("CHEAT ACTIVATED: RANDOM TRACK");
			}

			if (CheatString == SaveSettingsCommand)
			{
				SaveAndLoadScript.Instance.SaveSettingsData ();
				ShowCheatNotification ("CHEAT ACTIVATED: SETTINGS SAVE");
			}

			if (CheatString == LoadSettingsCommand)
			{
				SaveAndLoadScript.Instance.LoadSettingsData ();
				ShowCheatNotification ("CHEAT ACTIVATED: SETTINGS LOAD");
			}

			if (CheatString == AddLifeCommand) 
			{
				if (GameController.Instance.Lives < GameController.Instance.MaxLives) 
				{
					//GameController.Instance.Lives += 3;
					GameController.Instance.Lives = Mathf.Clamp (GameController.Instance.Lives, 0, GameController.Instance.MaxLives);
					GameObject powerupPickupUI = Instantiate (GameController.Instance.PowerupPickupUI, playerControllerScript_P1.playerCol.transform.position, Quaternion.identity);
					powerupPickupUI.GetComponentInChildren<RawImage> ().texture = AddlifeTexture;
					powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (1f, 1f, 1, 1);
					GameController.Instance.MaxLivesText.text = "";

					if (GameController.Instance.Lives <= 7) 
					{
						GameController.Instance.LifeImages [GameController.Instance.Lives - 2].gameObject.SetActive (true);
						GameController.Instance.LifeImages [GameController.Instance.Lives - 2].enabled = true;
						GameController.Instance.LifeImages [GameController.Instance.Lives - 2].color = Color.white;
						GameController.Instance.LifeImages [GameController.Instance.Lives - 2].GetComponent<Animator> ().SetTrigger ("LifeImageEnter");
						GameController.Instance.LifeImages [GameController.Instance.Lives - 2].GetComponent<Animator> ().SetBool ("Hidden", false);

						GameController.Instance.LifeImages [GameController.Instance.Lives - 1].gameObject.SetActive (true);
						GameController.Instance.LifeImages [GameController.Instance.Lives - 1].enabled = true;
						GameController.Instance.LifeImages [GameController.Instance.Lives - 1].color = Color.white;
						GameController.Instance.LifeImages [GameController.Instance.Lives - 1].GetComponent<Animator> ().SetTrigger ("LifeImageEnter");
						GameController.Instance.LifeImages [GameController.Instance.Lives - 1].GetComponent<Animator> ().SetBool ("Hidden", false);

						GameController.Instance.LifeImages [GameController.Instance.Lives].gameObject.SetActive (true);
						GameController.Instance.LifeImages [GameController.Instance.Lives].enabled = true;
						GameController.Instance.LifeImages [GameController.Instance.Lives].color = Color.white;
						GameController.Instance.LifeImages [GameController.Instance.Lives].GetComponent<Animator> ().SetTrigger ("LifeImageEnter");
						GameController.Instance.LifeImages [GameController.Instance.Lives].GetComponent<Animator> ().SetBool ("Hidden", false);

						GameController.Instance.LifeImages [GameController.Instance.Lives + 1].gameObject.SetActive (true);
						GameController.Instance.LifeImages [GameController.Instance.Lives + 1].enabled = true;
						GameController.Instance.LifeImages [GameController.Instance.Lives + 1].color = Color.white;
						GameController.Instance.LifeImages [GameController.Instance.Lives + 1].GetComponent<Animator> ().SetTrigger ("LifeImageEnter");
						GameController.Instance.LifeImages [GameController.Instance.Lives + 1].GetComponent<Animator> ().SetBool ("Hidden", false);
					}

					GameController.Instance.Lives += 3;

					//GameController.Instance.UpdateLives ();
					ShowCheatNotification ("CHEAT ACTIVATED: EXTRA LIFE");
				}

				if (GameController.Instance.Lives >= GameController.Instance.MaxLives) 
				{
					GameController.Instance.MaxLivesText.text = "MAX";
					Debug.Log ("Reached maximum lives.");
					ShowCheatNotification ("CHEAT ACTIVATED: MAX LIVES");
				}

				GameController.Instance.Lives = Mathf.Clamp (GameController.Instance.Lives, 0, GameController.Instance.MaxLives);
				GameController.Instance.LivesAnim.SetTrigger ("UpdateLives");
			}

			if (CheatString == ToggleGodmodeCommand) 
			{
				isGod = !isGod;

				if (isGod == true)
				{
					playerControllerScript_P1.playerCol.enabled = false;
					playerControllerScript_P1.playerTrigger.enabled = false;
					ShowCheatNotification ("CHEAT ACTIVATED: GOD ON");
				}

				if (isGod == false) 
				{
					playerControllerScript_P1.playerCol.enabled = true;
					playerControllerScript_P1.playerTrigger.enabled = true;
					ShowCheatNotification ("CHEAT ACTIVATED: GOD OFF");
				}
			}

			if (CheatString == ChargeAbilityCommand) 
			{
				playerControllerScript_P1.CurrentAbilityTimeRemaining = playerControllerScript_P1.CurrentAbilityDuration;
				playerControllerScript_P1.AbilityCompletion.Play ("AbilityComplete");
				playerControllerScript_P1.AbilityCompletionTexture.texture = playerControllerScript_P1.AbilityImage.texture;
				playerControllerScript_P1.AbilityCompletionText.text = PlayerController.ParseByCase(playerControllerScript_P1.Ability.ToString ());

				if (playerControllerScript_P1.timeIsSlowed == false) 
				{
					TimescaleController.Instance.OverrideTimeScaleTimeRemaining += 1f;
					TimescaleController.Instance.OverridingTimeScale = 0.3f;
				}

				playerControllerScript_P1.CurrentAbilityState = PlayerController.abilityState.Ready;
				ShowCheatNotification ("CHEAT ACTIVATED: ABILITY CHARGED");
			}

			if (CheatString == RefreshAbilityCommand) 
			{
				playerControllerScript_P1.RefreshAbilityName ();
				playerControllerScript_P1.RefreshAbilityImage ();
				ShowCheatNotification ("CHEAT ACTIVATED: ABILITY REFRESH");
			}

			if (CheatString == SpawnBlockCommand)
			{
				GameController.Instance.SpawnBlock (true);
				ShowCheatNotification ("CHEAT ACTIVATED: SPAWN BLOCK");
			}
				
			if (CheatString == SpawnMiniBossCommand) 
			{
				GameController.Instance.SpawnMiniBossObject ();
				ShowCheatNotification ("CHEAT ACTIVATED: SPAWN MINI BOSS");
			}

			if (CheatString == SpawnPowerupPickupCommand)
			{
				GameController.Instance.SpawnPowerupPickup ();
				ShowCheatNotification ("CHEAT ACTIVATED: SPAWN POWERUP");
			}

			if (CheatString == PowerupTimeCommand)
			{
				GameController.Instance.PowerupTimeRemaining = GameController.Instance.PowerupTimeDuration;
				ShowCheatNotification ("CHEAT ACTIVATED: POWERUP TIME REFRESH");
			}

			if (CheatString == DoubleShotCommand) 
			{
				if (playerControllerScript_P1.ShotType == PlayerController.shotType.Standard) 
				{
					playerControllerScript_P1.AddParticleActiveEffects ();
				}

				GameController.Instance.SetPowerupTime (20);

				playerControllerScript_P1.ShotType = PlayerController.shotType.Double;
				playerControllerScript_P1.CurrentShootingHeatCost = playerControllerScript_P1.DoubleShootingHeatCost;

				if (playerControllerScript_P1.isInOverdrive == true) 
				{
					playerControllerScript_P1.DoubleShotIteration = PlayerController.shotIteration.Overdrive;
				}

				if (playerControllerScript_P1.isInRapidFire == false) 
				{
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.DoubleShotFireRates [0];
				}

				if (playerControllerScript_P1.isInRapidFire == true) 
				{
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.DoubleShotFireRates [1];
				}
					
				GameController.Instance.PowerupImage_P1[0].texture = DoubleShotTexture;
				GameController.Instance.PowerupImage_P1[0].color = new Color (1, 1, 1, 1);
				GameController.Instance.PowerupImage_P1[0].gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");

				GameObject powerupPickupUI = Instantiate (GameController.Instance.PowerupPickupUI, playerControllerScript_P1.playerCol.transform.position, Quaternion.identity);
				powerupPickupUI.GetComponentInChildren<RawImage> ().texture = DoubleShotTexture;
				powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (0.3f, 0.7f, 1, 1);

				ShowCheatNotification ("CHEAT ACTIVATED: DOUBLE SHOT");
			}

			if (CheatString == TripleShotCommand)
			{
				if (playerControllerScript_P1.ShotType == PlayerController.shotType.Standard) 
				{
					playerControllerScript_P1.AddParticleActiveEffects ();
				}

				GameController.Instance.SetPowerupTime (20);

				playerControllerScript_P1.ShotType = PlayerController.shotType.Triple;
				playerControllerScript_P1.CurrentShootingHeatCost = playerControllerScript_P1.TripleShootingHeatCost;

				if (playerControllerScript_P1.isInOverdrive == true) 
				{
					playerControllerScript_P1.TripleShotIteration = PlayerController.shotIteration.Overdrive;
				}

				if (playerControllerScript_P1.isInRapidFire == false) 
				{
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.TripleShotFireRates [0];
				}

				if (playerControllerScript_P1.isInRapidFire == true) 
				{
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.TripleShotFireRates [1];
				}

				GameController.Instance.PowerupImage_P1[0].texture = TripleShotTexture;
				GameController.Instance.PowerupImage_P1[0].color = new Color (1, 1, 1, 1);
				GameController.Instance.PowerupImage_P1[0].gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");

				GameObject powerupPickupUI = Instantiate (GameController.Instance.PowerupPickupUI, playerControllerScript_P1.playerCol.transform.position, Quaternion.identity);
				powerupPickupUI.GetComponentInChildren<RawImage> ().texture = TripleShotTexture;
				powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (0.435f, 0.717f, 1, 1);

				ShowCheatNotification ("CHEAT ACTIVATED: TRIPLE SHOT");
			}

		if (CheatString == RippleShotCommand)
		{
			if (playerControllerScript_P1.ShotType == PlayerController.shotType.Standard) 
			{
				playerControllerScript_P1.AddParticleActiveEffects ();
			}

			GameController.Instance.SetPowerupTime (20);

			playerControllerScript_P1.ShotType = PlayerController.shotType.Ripple;
			playerControllerScript_P1.CurrentShootingHeatCost = playerControllerScript_P1.RippleShootingHeatCost;

			if (playerControllerScript_P1.isInOverdrive == true || playerControllerScript_P1.isHoming == true) 
			{
				playerControllerScript_P1.RippleShotIteration = PlayerController.shotIteration.Overdrive;
			}

			if (playerControllerScript_P1.isInRapidFire == false) 
			{
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.RippleShotFireRates [0];
			}

			if (playerControllerScript_P1.isInRapidFire == true) 
			{
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.RippleShotFireRates [1];
			}

			GameController.Instance.PowerupImage_P1[0].texture = RippleShotTexture;
			GameController.Instance.PowerupImage_P1[0].color = new Color (1, 1, 1, 1);
			GameController.Instance.PowerupImage_P1[0].gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");

			GameObject powerupPickupUI = Instantiate (GameController.Instance.PowerupPickupUI, playerControllerScript_P1.playerCol.transform.position, Quaternion.identity);
			powerupPickupUI.GetComponentInChildren<RawImage> ().texture = RippleShotTexture;
			powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (0.31f, 0.372f, 1, 1);

			ShowCheatNotification ("CHEAT ACTIVATED: RIPPLE SHOT");
		}

			if (CheatString == TurretCommand)
			{
				if (playerControllerScript_P1.nextTurretSpawn < 4) 
				{
					GameController.Instance.SetPowerupTime (20);

					GameObject Turret = playerControllerScript_P1.Turrets [playerControllerScript_P1.nextTurretSpawn];
					Turret.SetActive (true);
					Turret.GetComponent<Turret> ().playerControllerScript = playerControllerScript_P1;

					UpdatePowerupImages (GameController.Instance.NextPowerupSlot_P1, TurretTexture, Color.white);

					GameController.Instance.NextPowerupSlot_P1 += 1;
					playerControllerScript_P1.nextTurretSpawn += 1;

					GameObject powerupPickupUI = Instantiate (GameController.Instance.PowerupPickupUI, playerControllerScript_P1.playerCol.transform.position, Quaternion.identity);
					powerupPickupUI.GetComponentInChildren<RawImage> ().texture = TurretTexture;
					powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (0.31f, 0.372f, 1, 1);

					ShowCheatNotification ("CHEAT ACTIVATED: POWERUP - TURRET");
					playerControllerScript_P1.AddParticleActiveEffects ();
				}
			}

			if (CheatString == NextWaveCommand) 
			{
				StopCoroutine (GameController.Instance.StartBlockSpawn ());
				GameController.Instance.StartNewWave ();

				GameController.Instance.WaveText.text = "WAVE " + GameController.Instance.Wave;

				ShowCheatNotification ("CHEAT ACTIVATED: NEXT WAVE");
			}
			
			if (CheatString == PreviousWaveCommand)
			{
				if (GameController.Instance.Wave > 1)
				{
					StopCoroutine (GameController.Instance.StartBlockSpawn ());
					GameController.Instance.StartPreviousWave ();
					GameController.Instance.BlockSpawnRate += GameController.Instance.BlockSpawnIncreaseRate;
				}

				GameController.Instance.WaveText.text = "WAVE " + GameController.Instance.Wave;
				ShowCheatNotification ("CHEAT ACTIVATED: LAST WAVE");
			}

			if (CheatString == RapidfireCommand) 
			{
				// Toggles rapid fire mode.
				playerControllerScript_P1.isInRapidFire = !playerControllerScript_P1.isInRapidFire;

				if (playerControllerScript_P1.isInRapidFire == true)
				{
					GameController.Instance.SetPowerupTime (20);
					GameController.Instance.RapidfireImage.transform.SetSiblingIndex (-GameController.Instance.NextPowerupShootingSlot_P1 + 3);
					GameController.Instance.NextPowerupShootingSlot_P1 += 1;
					GameController.Instance.RapidfireImage.enabled = true;
					GameController.Instance.RapidfireHex.enabled = true;
					GameController.Instance.RapidfireImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");

					switch (playerControllerScript_P1.ShotType) 
					{
					case PlayerController.shotType.Double:
						playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.DoubleShotFireRates [1];
						break;
					case PlayerController.shotType.Triple:
						playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.TripleShotFireRates [1];
						break;
					case PlayerController.shotType.Ripple:
						playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.RippleShotFireRates [1];
						break;
					case PlayerController.shotType.Standard:
						playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.DoubleShotFireRates [1];
						break;
					}

					GameObject powerupPickupUI = Instantiate (GameController.Instance.PowerupPickupUI, playerControllerScript_P1.playerCol.transform.position, Quaternion.identity);
					powerupPickupUI.GetComponentInChildren<RawImage> ().texture = RapidfireTexture;
					powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (0.207f, 0.866f, 1, 1);

					ShowCheatNotification ("CHEAT ACTIVATED: RAPIDFIRE: ON");
					playerControllerScript_P1.AddParticleActiveEffects ();
				}

				if (playerControllerScript_P1.isInRapidFire == false)
				{
					if (GameController.Instance.NextPowerupShootingSlot_P1 > 0)
					{
						GameController.Instance.NextPowerupShootingSlot_P1 -= 1;
					}

					GameController.Instance.RapidfireImage.enabled = false;
					GameController.Instance.RapidfireHex.enabled = false;
					ShowCheatNotification ("CHEAT ACTIVATED: RAPIDFIRE OFF");
				}
			}

			if (CheatString == OverdriveCommand) 
			{
				// Toggles overdrive mode.
				playerControllerScript_P1.isInOverdrive = !playerControllerScript_P1.isInOverdrive;

				if (playerControllerScript_P1.isInOverdrive == true) 
				{
					GameController.Instance.SetPowerupTime (20);
					GameController.Instance.OverdriveImage.transform.SetSiblingIndex (-GameController.Instance.NextPowerupShootingSlot_P1 + 3);
					GameController.Instance.NextPowerupShootingSlot_P1 += 1;
					GameController.Instance.OverdriveImage.enabled = true;
					GameController.Instance.OverdriveHex.enabled = true;
					GameController.Instance.OverdriveImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");

					playerControllerScript_P1.StandardShotIteration = PlayerController.shotIteration.Overdrive;
					playerControllerScript_P1.DoubleShotIteration = PlayerController.shotIteration.Overdrive;
					playerControllerScript_P1.TripleShotIteration = PlayerController.shotIteration.Overdrive;
					playerControllerScript_P1.RippleShotIteration = PlayerController.shotIteration.Overdrive;

					GameObject powerupPickupUI = Instantiate (GameController.Instance.PowerupPickupUI, playerControllerScript_P1.playerCol.transform.position, Quaternion.identity);
					powerupPickupUI.GetComponentInChildren<RawImage> ().texture = OverdriveTexture;
					powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (0.898f, 0.25f, 1, 1);

					ShowCheatNotification ("CHEAT ACTIVATED: OVERDRIVE MODE ON");
					playerControllerScript_P1.AddParticleActiveEffects ();
				}

				if (playerControllerScript_P1.isInOverdrive == false)
				{
					if (GameController.Instance.NextPowerupShootingSlot_P1 > 0)
					{
						GameController.Instance.NextPowerupShootingSlot_P1 -= 1;
					}

					GameController.Instance.OverdriveImage.enabled = false;
					GameController.Instance.OverdriveHex.enabled = false;
					ShowCheatNotification ("CHEAT ACTIVATED: OVERDRIVE MODE OFF");
				}
			}

			if (CheatString == RicochetCommand) 
			{
				// Toggles ricochet mode.
				playerControllerScript_P1.isRicochet = !playerControllerScript_P1.isRicochet;

				if (playerControllerScript_P1.isRicochet == true) 
				{
					GameController.Instance.SetPowerupTime (20);
					playerControllerScript_P1.EnableRicochetObject ();

					if (playerControllerScript_P1.DoubleShotIteration != PlayerController.shotIteration.Overdrive) 
					{
						playerControllerScript_P1.DoubleShotIteration = PlayerController.shotIteration.Enhanced;
					}

					if (playerControllerScript_P1.TripleShotIteration != PlayerController.shotIteration.Overdrive) 
					{
						playerControllerScript_P1.TripleShotIteration = PlayerController.shotIteration.Enhanced;
					}

					if (playerControllerScript_P1.RippleShotIteration != PlayerController.shotIteration.Overdrive) 
					{
						playerControllerScript_P1.RippleShotIteration = PlayerController.shotIteration.Enhanced;
					}

					if (playerControllerScript_P1.StandardShotIteration != PlayerController.shotIteration.Overdrive)
					{
						playerControllerScript_P1.StandardShotIteration = PlayerController.shotIteration.Enhanced;
					}

					playerControllerScript_P1.isRicochet = true;
					GameController.Instance.RicochetImage.transform.SetSiblingIndex (-GameController.Instance.NextPowerupShootingSlot_P1 + 3);
					GameController.Instance.NextPowerupShootingSlot_P1 += 1;
					GameController.Instance.RicochetImage.enabled = true;
					GameController.Instance.RicochetHex.enabled = true;
					GameController.Instance.RicochetImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");

					playerControllerScript_P1.EnableRicochetObject ();

					GameObject powerupPickupUI = Instantiate (GameController.Instance.PowerupPickupUI, playerControllerScript_P1.playerCol.transform.position, Quaternion.identity);
					powerupPickupUI.GetComponentInChildren<RawImage> ().texture = RicochetTexture;
					powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (0.25f, 1, 0.565f, 1);

					ShowCheatNotification ("CHEAT ACTIVATED: RICOCHET MODE ON");
					playerControllerScript_P1.AddParticleActiveEffects ();
				}

				if (playerControllerScript_P1.isRicochet == false) 
				{
					if (GameController.Instance.NextPowerupShootingSlot_P1 > 0)
					{
						GameController.Instance.NextPowerupShootingSlot_P1 -= 1;
					}

					GameController.Instance.RicochetImage.enabled = false;
					GameController.Instance.RicochetHex.enabled = false;
					ShowCheatNotification ("CHEAT ACTIVATED: RICOCHET MODE OFF");
				}
			}

			if (CheatString == HomingCommand) 
			{
				// Toggles homing mode.
				playerControllerScript_P1.isHoming = !playerControllerScript_P1.isHoming;

				if (playerControllerScript_P1.isHoming == true) 
				{
					GameController.Instance.SetPowerupTime (20);
					GameController.Instance.HomingImage.transform.SetSiblingIndex (-GameController.Instance.NextPowerupShootingSlot_P1 + 3);
					GameController.Instance.NextPowerupShootingSlot_P1 += 1;
					GameController.Instance.HomingImage.enabled = true;
					GameController.Instance.HomingHex.enabled = true;
					GameController.Instance.HomingImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");

					GameObject powerupPickupUI = Instantiate (
						GameController.Instance.PowerupPickupUI, 
						playerControllerScript_P1.playerCol.transform.position, 
						Quaternion.identity
					);

					powerupPickupUI.GetComponentInChildren<RawImage> ().texture = HomingTexture;
					powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (0.57f, 1, 0.277f, 1);

					ShowCheatNotification ("CHEAT ACTIVATED: HOMING MODE ON");
					playerControllerScript_P1.AddParticleActiveEffects ();
				}

				if (playerControllerScript_P1.isHoming == false) 
				{

					if (GameController.Instance.NextPowerupShootingSlot_P1 > 0)
					{
						GameController.Instance.NextPowerupShootingSlot_P1 -= 1;
					}

					GameController.Instance.HomingImage.enabled = false;
					GameController.Instance.HomingHex.enabled = false;
					ShowCheatNotification ("CHEAT ACTIVATED: HOMING MODE OFF");
				}
			}

			if (CheatString == HelixCommand) 
			{
				GameController.Instance.SetPowerupTime (20);

				if (playerControllerScript_P1.Helix.activeInHierarchy == false)
				{
					playerControllerScript_P1.Helix.SetActive (true);
					UpdatePowerupImages (GameController.Instance.NextPowerupSlot_P1, HelixTexture, Color.white);
					GameController.Instance.NextPowerupSlot_P1 += 1;
					playerControllerScript_P1.AddParticleActiveEffects ();
				}

				GameObject powerupPickupUI = Instantiate (GameController.Instance.PowerupPickupUI, playerControllerScript_P1.playerCol.transform.position, Quaternion.identity);
				powerupPickupUI.GetComponentInChildren<RawImage> ().texture = HelixTexture;
				powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (1f, 0.278f, 0.561f, 1);

				ShowCheatNotification ("CHEAT ACTIVATED: POWERUP - HELIX");
			}

			if (CheatString == SlowTimeCommand) 
			{
				if (playerControllerScript_P1.timeIsSlowed == false) 
				{
					GameController.Instance.SetPowerupTime (20);

					if (GameController.Instance.VhsAnim.GetCurrentAnimatorStateInfo (0).IsName ("Slow") == false)
					{
						GameController.Instance.VhsAnim.SetTrigger ("Slow");
					}

					playerControllerScript_P1.timeIsSlowed = true;

					TimescaleController.Instance.OverridingTimeScale = 0.3f;
					TimescaleController.Instance.OverrideTimeScaleTimeRemaining += 20;
					TimescaleController.Instance.isOverridingTimeScale = true;

					UpdatePowerupImages (GameController.Instance.NextPowerupSlot_P1, SlowTimeTexture, Color.white);

					ShowCheatNotification ("CHEAT ACTIVATED: POWERUP - SLOW TIME");
				}
			}

			if (CheatString == StandardShotCommand) 
			{
				GameController.Instance.PowerupImage_P1 [0].texture = null;
				GameController.Instance.PowerupImage_P1 [0].color = new Color (0, 0, 0, 0);

				playerControllerScript_P1.ShotType = PlayerController.shotType.Standard;
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.StandardFireRate;
				playerControllerScript_P1.isInOverdrive = false;
				playerControllerScript_P1.isInRapidFire = false;

				ShowCheatNotification ("CHEAT ACTIVATED: STANDARD SHOT");
			}

			if (CheatString == ResetAllPowerupsCommand) 
			{
				GameController.Instance.PowerupTimeRemaining = 0;
				playerControllerScript_P1.ResetPowerups ();
				playerControllerScript_P1.CurrentShootingHeatCost = playerControllerScript_P1.StandardShootingHeatCost;

				ShowCheatNotification ("CHEAT ACTIVATED: RESET POWERUPS");
			}

			if (CheatString == ShieldCommand) 
			{
				playerControllerScript_P1.Ability = PlayerController.ability.Shield;
				playerControllerScript_P1.RefreshAbilityName ();
				playerControllerScript_P1.RefreshAbilityImage ();

				ShowCheatNotification ("CHEAT ACTIVATED: ABILITY - SHIELD");
			}

			if (CheatString == VerticalBeamCommand) 
			{
				playerControllerScript_P1.Ability = PlayerController.ability.VerticalBeam;
				playerControllerScript_P1.RefreshAbilityName ();
				playerControllerScript_P1.RefreshAbilityImage ();

				ShowCheatNotification ("CHEAT ACTIVATED: ABILITY - VERTICAL BEAM");
			}

			if (CheatString == HorizontalBeamCommand) 
			{
				playerControllerScript_P1.Ability = PlayerController.ability.HorizontalBeam;
				playerControllerScript_P1.RefreshAbilityName ();
				playerControllerScript_P1.RefreshAbilityImage ();

				ShowCheatNotification ("CHEAT ACTIVATED: ABILITY - HORIZONTAL BEAM");
			}

			if (CheatString == EmpCommand) 
			{
				playerControllerScript_P1.Ability = PlayerController.ability.Emp;
				playerControllerScript_P1.RefreshAbilityName ();
				playerControllerScript_P1.RefreshAbilityImage ();

				ShowCheatNotification ("CHEAT ACTIVATED: ABILITY - EMP");
			}

			if (CheatString == RewindCommand) 
			{
				playerControllerScript_P1.Ability = PlayerController.ability.Rewind;
				playerControllerScript_P1.RefreshAbilityName ();
				playerControllerScript_P1.RefreshAbilityImage ();
	
				ShowCheatNotification ("CHEAT ACTIVATED: ABILITY - REWIND TIME");
			}

			if (CheatString == MirrorPlayerCommand) 
			{
				playerControllerScript_P1.Ability = PlayerController.ability.Mirror;
				playerControllerScript_P1.RefreshAbilityName ();
				playerControllerScript_P1.RefreshAbilityImage ();

				ShowCheatNotification ("CHEAT ACTIVATED: ABILITY - MIRROR PLAYER");
			}
				
			if (Input.GetKeyDown (KeyCode.Alpha7))
			{
				CheatString = LastCheatName;
			}

			if (CheatString == AddScoreCommand) 
			{
				GameController.Instance.TargetScore += AddScoreAmount;
				ShowCheatNotification ("CHEAT ACTIVATED: ADD SCORE");
			}

			if (CheatString == LoseLifeCommand) 
			{
				GameController.Instance.Lives -= 1;
				GameController.Instance.UpdateLives ();
				ShowCheatNotification ("CHEAT ACTIVATED: LOSE LIFE");
			}

			if (CheatString == GameOverCommand)
			{
				playerControllerScript_P1.GameOver ();
				//Debug.Log ("Player forced game over.");
				ShowCheatNotification ("CHEAT ACTIVATED: FORCE GAME OVER");
			}

			if (CheatString == UseOverheatCommand) 
			{
				playerControllerScript_P1.useOverheat = !playerControllerScript_P1.useOverheat;

				if (playerControllerScript_P1.useOverheat == true) 
				{
					ShowCheatNotification ("CHEAT ACTIVATED: OVERHEAT ON");
				}

				if (playerControllerScript_P1.useOverheat == false) 
				{
					playerControllerScript_P1.Overheated = false;
					playerControllerScript_P1.CurrentShootingHeat = 0;
					playerControllerScript_P1.CurrentShootingCooldown = 0;
					ShowCheatNotification ("CHEAT ACTIVATED: OVERHEAT OFF");
				}
			}

			if (CheatString == DoBonusRoundCommand) 
			{
				GameController.Instance.doBonusRound = true;

				ShowCheatNotification ("CHEAT ACTIVATED: BONUS ROUND ON");
			}

			if (CheatString == SetQualitySettingsHigh) 
			{
				if (SaveAndLoadScript.Instance.QualitySettingsIndex != 1)
				{
					SaveAndLoadScript.Instance.QualitySettingsIndex = 1;
					Application.targetFrameRate = -1;

					if (Screen.width < 1920 || Screen.height < 1080) 
					{
						Screen.SetResolution (1920, 1080, Screen.fullScreen);
					}

					SaveAndLoadScript.Instance.SaveSettingsData ();
					SaveAndLoadScript.Instance.LoadSettingsData ();
					ShowCheatNotification ("CHEAT ACTIVATED: HIGH QUALITY SETTINGS");
				}
			}

			if (CheatString == SetQualitySettingsLow) 
			{
				if (SaveAndLoadScript.Instance.QualitySettingsIndex != 0)
				{
					SaveAndLoadScript.Instance.QualitySettingsIndex = 0;
					Application.targetFrameRate = -1;

					if (Screen.width > 1280 || Screen.height > 720) 
					{
						Screen.SetResolution (1280, 720, Screen.fullScreen);
					}

					SaveAndLoadScript.Instance.SaveSettingsData ();
					SaveAndLoadScript.Instance.LoadSettingsData ();
					ShowCheatNotification ("CHEAT ACTIVATED: LOW QUALITY SETTINGS");
				}
			}
		}
	}

	void UpdatePowerupImages (int index, Texture2D powerupTex, Color powerupCol)
	{
		GameController.Instance.PowerupImage_P1 [index].gameObject.SetActive (true);
		GameController.Instance.PowerupImage_P1 [index].texture = powerupTex;
		GameController.Instance.PowerupImage_P1 [index].color = powerupCol;
		GameController.Instance.PowerupImage_P1 [index].gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");
	}

	// What happens when cheats get enabled.
	void ShowCheatActivation (string cheatText)
	{
		// Gives new next fire amount so the player can keep on firing.
		float nextfire = 
			Time.time + 
			(playerControllerScript_P1.CurrentFireRate / (playerControllerScript_P1.FireRateTimeMultiplier * Time.timeScale));
		
		LastCheatName = CheatString;
		CheatInputText.text = LastCheatName;
		CheatNotificationAnim.Play ("CheatNotification");
		CheatNotifcationText.text = cheatText;

		Instantiate (CheatSound, transform.position, Quaternion.identity);

		ClearCheatString ();

		if (playerControllerScript_P1.timeIsSlowed == false)
		{
			TimescaleController.Instance.OverrideTimeScaleTimeRemaining += 0.5f;
			TimescaleController.Instance.OverridingTimeScale = 0.2f;
		}

		playerControllerScript_P1.NextFire = nextfire;
		playerControllerScript_P1.DoubleShotNextFire = nextfire;
		playerControllerScript_P1.TripleShotNextFire = nextfire;
		playerControllerScript_P1.RippleShotNextFire = nextfire;

		Debug.Log (cheatText);
		Invoke ("DisableCheatConsole", 0.5f);
	}

	// Cheat string was matched.
	void ShowCheatNotification (string cheatText)
	{
		// Gives new next fire amount so the player can keep on firing.
		float nextfire = 
			Time.time + 
			(playerControllerScript_P1.CurrentFireRate / (playerControllerScript_P1.FireRateTimeMultiplier * Time.timeScale));
		
		CheatNotificationAnim.StopPlayback ();
		CheatNotificationAnim.Play ("CheatNotification");
		CheatNotifcationText.text = cheatText;

		playerControllerScript_P1.CheckPowerupImageUI ();

		Instantiate (CheatSound, transform.position, Quaternion.identity);

		LastCheatName = CheatString;
		//Invoke ("ClearCheatString", 0.1f);
		ClearCheatString ();

		if (TimescaleController.Instance.OverrideTimeScaleTimeRemaining < 0.5f && playerControllerScript_P1.timeIsSlowed) 
		{
			TimescaleController.Instance.OverrideTimeScaleTimeRemaining += 0.5f;
		}

		if (TimescaleController.Instance.OverridingTimeScale < 0.2f && playerControllerScript_P1.timeIsSlowed)
		{
			TimescaleController.Instance.OverridingTimeScale = 0.2f;
		}

		playerControllerScript_P1.NextFire = nextfire ;
		playerControllerScript_P1.DoubleShotNextFire = nextfire ;
		playerControllerScript_P1.TripleShotNextFire = nextfire ;
		playerControllerScript_P1.RippleShotNextFire = nextfire ;

		Debug.Log (cheatText);
		Invoke ("DisableCheatConsole", 0.5f);
	}

	// Turn off cheat console.
	void DisableCheatConsole ()
	{
		CheatConsole.SetActive (false);
	}
}