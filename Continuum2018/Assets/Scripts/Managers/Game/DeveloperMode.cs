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
	public TutorialManager tutorialManagerScript;

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
	[Space (10)]
	// Misc cheats.
	public string AddScoreCommand = "addscore"; // Adds current score by AddScoreAmount value.
	public string LoseLifeCommand = "loselife"; // Loses a life.
	public string GameOverCommand = "gameover"; // Force a game over regardless of how many lives remain.
	public string ToggleGodmodeCommand = "god"; // Toggle god mode ON or OFF.
	public string AddLifeCommand = "life"; // Adds lives to player.
	public string NextWaveCommand = "nextwave"; // Wave number increases.
	public string PreviousWaveCommand = "lastwave"; // Wave number decreases.

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

	void Start () 
	{
		// Find the saving script.
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();
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
		// CheatInputText.text = LastCheatName;
	}

	void UpdateCheats ()
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
			if (CheatString.Contains (c.ToString () + c.ToString () + c.ToString ()))
			{
				ClearCheatString ();
			}

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
		if (CheatString == ToggleCheatsCommand
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

		// Cheat time remain is 0, ignore checking for cheats.
		if (CheatStringResetTimeRemaining <= 0) 
		{
			ClearCheatString ();
			//CheatInputText.text = ">_ ";
			return;
		}
		
		if (useCheats == true) 
		{
			// Insert cheats here.

			if (CheatString == ForceStartCommand) 
			{
				if (forceStarted == false) 
				{
					tutorialManagerScript.TurnOffTutorial ();
					ShowCheatNotification ("CHEAT ACTIVATED: FORCE START");
					forceStarted = true;
				}
			}

			if (CheatString == ForceRestartCommand) 
			{
				localSceneLoaderScript.sceneLoaderScript.SceneName = SceneManager.GetActiveScene ().name;
				localSceneLoaderScript.sceneLoaderScript.StartLoadSequence ();
				ShowCheatNotification ("");
			}

			if (CheatString == FpsUnlockCommand) 
			{
				targetFramerateScript.SetTargetFramerate (-1);
				ShowCheatNotification ("CHEAT ACTIVATED: FPS: -1");
			}

			if (CheatString == Fps60Command) 
			{
				targetFramerateScript.SetTargetFramerate (60);
				ShowCheatNotification ("CHEAT ACTIVATED: FPS: 60");
			}

			if (CheatString == Fps30Command) 
			{
				targetFramerateScript.SetTargetFramerate (30);
				ShowCheatNotification ("CHEAT ACTIVATED: FPS: 30");
			}

			if (CheatString == Fps90Command) {
				targetFramerateScript.SetTargetFramerate (90);
				ShowCheatNotification ("CHEAT ACTIVATED: FPS: 90");
			}

			if (CheatString == Fps120Command) 
			{
				targetFramerateScript.SetTargetFramerate (120);
				ShowCheatNotification ("CHEAT ACTIVATED: FPS: 120");
			}

			if (CheatString == NextTrackCommand) 
			{
				audioControllerScript.NextTrack ();
				ShowCheatNotification ("CHEAT ACTIVATED: NEXT TRACK");
			}

			if (CheatString == PreviousTrackCommand) {
				audioControllerScript.PreviousTrack ();
				ShowCheatNotification ("CHEAT ACTIVATED: PREVIOUS TRACK");
			}

			if (CheatString == RandomTrackCommand)
			{
				audioControllerScript.RandomTrack ();
				ShowCheatNotification ("CHEAT ACTIVATED: RANDOM TRACK");
			}

			if (CheatString == SaveSettingsCommand)
			{
				saveAndLoadScript.SaveSettingsData ();
				ShowCheatNotification ("CHEAT ACTIVATED: SETTINGS SAVE");
			}

			if (CheatString == LoadSettingsCommand)
			{
				saveAndLoadScript.LoadSettingsData ();
				ShowCheatNotification ("CHEAT ACTIVATED: SETTINGS LOAD");
			}

			if (CheatString == AddLifeCommand) 
			{
				if (gameControllerScript.Lives < gameControllerScript.MaxLives) 
				{
					gameControllerScript.Lives += 3;
					gameControllerScript.Lives = Mathf.Clamp (gameControllerScript.Lives, 0, gameControllerScript.MaxLives);
					GameObject powerupPickupUI = Instantiate (gameControllerScript.PowerupPickupUI, playerControllerScript_P1.playerCol.transform.position, Quaternion.identity);
					powerupPickupUI.GetComponentInChildren<RawImage> ().texture = AddlifeTexture;
					powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (1f, 1f, 1, 1);
					gameControllerScript.MaxLivesText.text = "";
					ShowCheatNotification ("CHEAT ACTIVATED: EXTRA LIFE");
				}

				if (gameControllerScript.Lives >= gameControllerScript.MaxLives) 
				{
					gameControllerScript.MaxLivesText.text = "MAX";
					Debug.Log ("Reached maximum lives.");
					ShowCheatNotification ("CHEAT ACTIVATED: MAX LIVES");
				}

				// Caps maximum lives.
				gameControllerScript.Lives = Mathf.Clamp (gameControllerScript.Lives, 0, gameControllerScript.MaxLives);
				gameControllerScript.UpdateLives ();
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
				gameControllerScript.SpawnBlock (true);
				ShowCheatNotification ("CHEAT ACTIVATED: SPAWN BLOCK");
			}
				
			if (CheatString == SpawnMiniBossCommand) 
			{
				gameControllerScript.SpawnMiniBossObject ();
				ShowCheatNotification ("CHEAT ACTIVATED: SPAWN MINI BOSS");
			}

			if (CheatString == SpawnPowerupPickupCommand)
			{
				gameControllerScript.SpawnPowerupPickup ();
				ShowCheatNotification ("CHEAT ACTIVATED: SPAWN POWERUP");
			}

			if (CheatString == PowerupTimeCommand)
			{
				gameControllerScript.PowerupTimeRemaining = gameControllerScript.PowerupTimeDuration;
				ShowCheatNotification ("CHEAT ACTIVATED: POWERUP TIME REFRESH");
			}

			if (CheatString == DoubleShotCommand) 
			{
				gameControllerScript.SetPowerupTime (20);

				playerControllerScript_P1.ShotType = PlayerController.shotType.Double;

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

				gameControllerScript.PowerupImage_P1[0].texture = DoubleShotTexture;
				gameControllerScript.PowerupImage_P1[0].color = new Color (1, 1, 1, 1);
				gameControllerScript.PowerupImage_P1[0].gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");

				GameObject powerupPickupUI = Instantiate (gameControllerScript.PowerupPickupUI, playerControllerScript_P1.playerCol.transform.position, Quaternion.identity);
				powerupPickupUI.GetComponentInChildren<RawImage> ().texture = DoubleShotTexture;
				powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (0.3f, 0.7f, 1, 1);

				ShowCheatNotification ("CHEAT ACTIVATED: DOUBLE SHOT");
			}

			if (CheatString == TripleShotCommand)
			{
				gameControllerScript.SetPowerupTime (20);

				playerControllerScript_P1.ShotType = PlayerController.shotType.Triple;

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

				gameControllerScript.PowerupImage_P1[0].texture = TripleShotTexture;
				gameControllerScript.PowerupImage_P1[0].color = new Color (1, 1, 1, 1);
				gameControllerScript.PowerupImage_P1[0].gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");

				GameObject powerupPickupUI = Instantiate (gameControllerScript.PowerupPickupUI, playerControllerScript_P1.playerCol.transform.position, Quaternion.identity);
				powerupPickupUI.GetComponentInChildren<RawImage> ().texture = TripleShotTexture;
				powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (0.435f, 0.717f, 1, 1);

				ShowCheatNotification ("CHEAT ACTIVATED: TRIPLE SHOT");
			}

		if (CheatString == RippleShotCommand)
		{
			gameControllerScript.SetPowerupTime (20);

			playerControllerScript_P1.ShotType = PlayerController.shotType.Ripple;

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

			gameControllerScript.PowerupImage_P1[0].texture = RippleShotTexture;
			gameControllerScript.PowerupImage_P1[0].color = new Color (1, 1, 1, 1);
			gameControllerScript.PowerupImage_P1[0].gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");

			GameObject powerupPickupUI = Instantiate (gameControllerScript.PowerupPickupUI, playerControllerScript_P1.playerCol.transform.position, Quaternion.identity);
			powerupPickupUI.GetComponentInChildren<RawImage> ().texture = RippleShotTexture;
			powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (0.31f, 0.372f, 1, 1);

			ShowCheatNotification ("CHEAT ACTIVATED: RIPPLE SHOT");
		}

			if (CheatString == TurretCommand)
			{
				if (playerControllerScript_P1.nextTurretSpawn < 4) 
				{
					gameControllerScript.SetPowerupTime (20);

					GameObject Turret = playerControllerScript_P1.Turrets [playerControllerScript_P1.nextTurretSpawn];
					Turret.SetActive (true);
					Turret.GetComponent<Turret> ().playerControllerScript = playerControllerScript_P1;

					gameControllerScript.PowerupImage_P1 [gameControllerScript.NextPowerupSlot_P1].texture = TurretTexture;
					gameControllerScript.PowerupImage_P1 [gameControllerScript.NextPowerupSlot_P1].color = Color.white;
					gameControllerScript.PowerupImage_P1 [gameControllerScript.NextPowerupSlot_P1].gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");

					gameControllerScript.NextPowerupSlot_P1 += 1;
					playerControllerScript_P1.nextTurretSpawn += 1;

					GameObject powerupPickupUI = Instantiate (gameControllerScript.PowerupPickupUI, playerControllerScript_P1.playerCol.transform.position, Quaternion.identity);
					powerupPickupUI.GetComponentInChildren<RawImage> ().texture = TurretTexture;
					powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (0.31f, 0.372f, 1, 1);

					ShowCheatNotification ("CHEAT ACTIVATED: Turret");
				}
			}

			if (CheatString == NextWaveCommand) 
			{
				gameControllerScript.Wave += 1;
				gameControllerScript.WaveText.text = "WAVE " + gameControllerScript.Wave;
				gameControllerScript.BlockSpawnRate -= gameControllerScript.BlockSpawnIncreaseRate;
				ShowCheatNotification ("CHEAT ACTIVATED: NEXT WAVE");
			}
			
			if (CheatString == PreviousWaveCommand)
			{
				if (gameControllerScript.Wave > 1)
				{
					gameControllerScript.Wave -= 1;
					gameControllerScript.BlockSpawnRate += gameControllerScript.BlockSpawnIncreaseRate;
				}

				gameControllerScript.WaveText.text = "WAVE " + gameControllerScript.Wave;
				ShowCheatNotification ("CHEAT ACTIVATED: LAST WAVE");
			}

			if (CheatString == RapidfireCommand) 
			{
				// Toggles rapid fire mode.
				playerControllerScript_P1.isInRapidFire = !playerControllerScript_P1.isInRapidFire;

				if (playerControllerScript_P1.isInRapidFire == true)
				{
					gameControllerScript.SetPowerupTime (20);
					gameControllerScript.RapidfireImage.enabled = true;
					gameControllerScript.RapidfireHex.enabled = true;
					gameControllerScript.RapidfireImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");

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

					GameObject powerupPickupUI = Instantiate (gameControllerScript.PowerupPickupUI, playerControllerScript_P1.playerCol.transform.position, Quaternion.identity);
					powerupPickupUI.GetComponentInChildren<RawImage> ().texture = RapidfireTexture;
					powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (0.207f, 0.866f, 1, 1);

					ShowCheatNotification ("CHEAT ACTIVATED: RAPIDFIRE: " + playerControllerScript_P1.ShotType.ToString().ToUpper ());
				}

				if (playerControllerScript_P1.isInRapidFire == false)
				{
					gameControllerScript.RapidfireImage.enabled = false;
					gameControllerScript.RapidfireHex.enabled = false;
					ShowCheatNotification ("CHEAT ACTIVATED: RAPIDFIRE OFF");
				}
			}

			if (CheatString == OverdriveCommand) 
			{
				// Toggles overdrive mode.
				playerControllerScript_P1.isInOverdrive = !playerControllerScript_P1.isInOverdrive;

				if (playerControllerScript_P1.isInOverdrive == true) 
				{
					gameControllerScript.SetPowerupTime (20);					
					gameControllerScript.OverdriveImage.enabled = true;
					gameControllerScript.OverdriveHex.enabled = true;
					gameControllerScript.OverdriveImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");

					playerControllerScript_P1.StandardShotIteration = PlayerController.shotIteration.Overdrive;
					playerControllerScript_P1.DoubleShotIteration = PlayerController.shotIteration.Overdrive;
					playerControllerScript_P1.TripleShotIteration = PlayerController.shotIteration.Overdrive;
					playerControllerScript_P1.RippleShotIteration = PlayerController.shotIteration.Overdrive;

					GameObject powerupPickupUI = Instantiate (gameControllerScript.PowerupPickupUI, playerControllerScript_P1.playerCol.transform.position, Quaternion.identity);
					powerupPickupUI.GetComponentInChildren<RawImage> ().texture = OverdriveTexture;
					powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (0.898f, 0.25f, 1, 1);

					ShowCheatNotification ("CHEAT ACTIVATED: OVERDRIVE MODE ON");
				}

				if (playerControllerScript_P1.isInOverdrive == false)
				{
					gameControllerScript.OverdriveImage.enabled = false;
					gameControllerScript.OverdriveHex.enabled = false;
					ShowCheatNotification ("CHEAT ACTIVATED: OVERDRIVE MODE OFF");
				}
			}

			if (CheatString == RicochetCommand) 
			{
				// Toggles ricochet mode.
				playerControllerScript_P1.isRicochet = !playerControllerScript_P1.isRicochet;

				if (playerControllerScript_P1.isRicochet == true) 
				{
					gameControllerScript.SetPowerupTime (20);
					playerControllerScript_P1.EnableRicochetObject ();

					if (playerControllerScript_P1.DoubleShotIteration != PlayerController.shotIteration.Overdrive) {
						playerControllerScript_P1.DoubleShotIteration = PlayerController.shotIteration.Enhanced;
					}

					if (playerControllerScript_P1.TripleShotIteration != PlayerController.shotIteration.Overdrive) {
						playerControllerScript_P1.TripleShotIteration = PlayerController.shotIteration.Enhanced;
					}

					if (playerControllerScript_P1.RippleShotIteration != PlayerController.shotIteration.Overdrive) {
						playerControllerScript_P1.RippleShotIteration = PlayerController.shotIteration.Enhanced;
					}

					if (playerControllerScript_P1.StandardShotIteration != PlayerController.shotIteration.Overdrive) {
						playerControllerScript_P1.StandardShotIteration = PlayerController.shotIteration.Enhanced;
					}

					playerControllerScript_P1.isRicochet = true;
					gameControllerScript.RicochetImage.enabled = true;
					gameControllerScript.RicochetHex.enabled = true;
					gameControllerScript.RicochetImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");

					playerControllerScript_P1.EnableRicochetObject ();

					GameObject powerupPickupUI = Instantiate (gameControllerScript.PowerupPickupUI, playerControllerScript_P1.playerCol.transform.position, Quaternion.identity);
					powerupPickupUI.GetComponentInChildren<RawImage> ().texture = RicochetTexture;
					powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (0.25f, 1, 0.565f, 1);

					ShowCheatNotification ("CHEAT ACTIVATED: RICOCHET MODE ON");
				}

				if (playerControllerScript_P1.isRicochet == false) 
				{
					gameControllerScript.RicochetImage.enabled = false;
					gameControllerScript.RicochetHex.enabled = false;
					ShowCheatNotification ("CHEAT ACTIVATED: RICOCHET MODE OFF");
				}
			}

			if (CheatString == HomingCommand) 
			{
				// Toggles homing mode.
				playerControllerScript_P1.isHoming = !playerControllerScript_P1.isHoming;

				if (playerControllerScript_P1.isHoming == true) 
				{
					gameControllerScript.SetPowerupTime (20);
					gameControllerScript.HomingImage.enabled = true;
					gameControllerScript.HomingHex.enabled = true;
					gameControllerScript.HomingImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");

					// TODO: When not in overdrive.
					//playerControllerScript_P1.DoubleShotIteration = PlayerController.shotIteration.Enhanced;
					//playerControllerScript_P1.TripleShotIteration = PlayerController.shotIteration.Enhanced;
					//playerControllerScript_P1.RippleShotIteration = PlayerController.shotIteration.Overdrive;
					//playerControllerScript_P1.StandardShotIteration = PlayerController.shotIteration.Enhanced;

					GameObject powerupPickupUI = Instantiate (
						gameControllerScript.PowerupPickupUI, 
						playerControllerScript_P1.playerCol.transform.position, 
						Quaternion.identity
					);

					powerupPickupUI.GetComponentInChildren<RawImage> ().texture = HomingTexture;
					powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (0.57f, 1, 0.277f, 1);

					ShowCheatNotification ("CHEAT ACTIVATED: HOMING MODE ON");
				}

				if (playerControllerScript_P1.isHoming == false) 
				{
					gameControllerScript.HomingImage.enabled = false;
					gameControllerScript.HomingHex.enabled = false;
					ShowCheatNotification ("CHEAT ACTIVATED: HOMING MODE OFF");
				}
			}

			if (CheatString == HelixCommand) 
			{
				gameControllerScript.SetPowerupTime (20);

				if (playerControllerScript_P1.Helix.activeInHierarchy == false)
				{
					playerControllerScript_P1.Helix.SetActive (true);

					gameControllerScript.PowerupImage_P1 [gameControllerScript.NextPowerupSlot_P1].texture = HelixTexture;
					gameControllerScript.PowerupImage_P1 [gameControllerScript.NextPowerupSlot_P1].color = Color.white;
					gameControllerScript.PowerupImage_P1 [gameControllerScript.NextPowerupSlot_P1].gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");

					gameControllerScript.NextPowerupSlot_P1 += 1;
				}

				GameObject powerupPickupUI = Instantiate (gameControllerScript.PowerupPickupUI, playerControllerScript_P1.playerCol.transform.position, Quaternion.identity);
				powerupPickupUI.GetComponentInChildren<RawImage> ().texture = HelixTexture;
				powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (1f, 0.278f, 0.561f, 1);

				ShowCheatNotification ("CHEAT ACTIVATED: HELIX");
			}

			if (CheatString == StandardShotCommand) 
			{
				gameControllerScript.PowerupImage_P1 [0].texture = null;
				gameControllerScript.PowerupImage_P1 [0].color = new Color (0, 0, 0, 0);

				playerControllerScript_P1.ShotType = PlayerController.shotType.Standard;
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.StandardFireRate;
				playerControllerScript_P1.isInOverdrive = false;
				playerControllerScript_P1.isInRapidFire = false;

				ShowCheatNotification ("CHEAT ACTIVATED: STANDARD SHOT");
			}

			if (CheatString == ResetAllPowerupsCommand) 
			{
				gameControllerScript.PowerupTimeRemaining = 0;
				playerControllerScript_P1.ResetPowerups ();

				ShowCheatNotification ("CHEAT ACTIVATED: RESET POWERUPS");
			}

			if (CheatString == ShieldCommand) 
			{
				playerControllerScript_P1.Ability = PlayerController.ability.Shield;
				playerControllerScript_P1.CurrentAbilityTimeRemaining = playerControllerScript_P1.CurrentAbilityDuration;
				playerControllerScript_P1.RefreshAbilityName ();
				playerControllerScript_P1.RefreshAbilityImage ();
				playerControllerScript_P1.ActivateAbility ();
				playerControllerScript_P1.CurrentAbilityState = PlayerController.abilityState.Active;

				ShowCheatNotification ("CHEAT ACTIVATED: SHIELD");
			}

			if (CheatString == VerticalBeamCommand) 
			{
				playerControllerScript_P1.Ability = PlayerController.ability.VerticalBeam;
				playerControllerScript_P1.RefreshAbilityName ();
				playerControllerScript_P1.RefreshAbilityImage ();
				playerControllerScript_P1.CurrentAbilityTimeRemaining = playerControllerScript_P1.CurrentAbilityDuration;
				playerControllerScript_P1.ActivateAbility ();
				playerControllerScript_P1.CurrentAbilityState = PlayerController.abilityState.Active;

				ShowCheatNotification ("CHEAT ACTIVATED: VERTICAL BEAM");
			}

			if (CheatString == HorizontalBeamCommand) 
			{
				playerControllerScript_P1.Ability = PlayerController.ability.HorizontalBeam;
				playerControllerScript_P1.RefreshAbilityName ();
				playerControllerScript_P1.RefreshAbilityImage ();
				playerControllerScript_P1.CurrentAbilityTimeRemaining = playerControllerScript_P1.CurrentAbilityDuration;
				playerControllerScript_P1.ActivateAbility ();
				playerControllerScript_P1.CurrentAbilityState = PlayerController.abilityState.Active;

				ShowCheatNotification ("CHEAT ACTIVATED: HORIZONTAL BEAM");
			}

			if (CheatString == EmpCommand) 
			{
				playerControllerScript_P1.Ability = PlayerController.ability.Emp;
				playerControllerScript_P1.RefreshAbilityName ();
				playerControllerScript_P1.RefreshAbilityImage ();
				playerControllerScript_P1.CurrentAbilityTimeRemaining = playerControllerScript_P1.CurrentAbilityDuration;
				playerControllerScript_P1.ActivateAbility ();
				playerControllerScript_P1.CurrentAbilityState = PlayerController.abilityState.Active;

				ShowCheatNotification ("CHEAT ACTIVATED: EMP");
			}

			if (CheatString == RewindCommand) 
			{
				playerControllerScript_P1.Ability = PlayerController.ability.Rewind;
				playerControllerScript_P1.RefreshAbilityName ();
				playerControllerScript_P1.RefreshAbilityImage ();
				playerControllerScript_P1.CurrentAbilityTimeRemaining = playerControllerScript_P1.CurrentAbilityDuration;
				playerControllerScript_P1.ActivateAbility ();
				playerControllerScript_P1.CurrentAbilityState = PlayerController.abilityState.Active;
			
				ShowCheatNotification ("CHEAT ACTIVATED: REWIND TIME");
			}
				
			if (Input.GetKeyDown (KeyCode.Alpha7))
			{
				CheatString = LastCheatName;
			}

			if (CheatString == AddScoreCommand) 
			{
				gameControllerScript.TargetScore += AddScoreAmount;
				ShowCheatNotification ("CHEAT ACTIVATED: ADD SCORE");
			}

			if (CheatString == LoseLifeCommand) 
			{
				gameControllerScript.Lives -= 1;
				gameControllerScript.UpdateLives ();
				ShowCheatNotification ("CHEAT ACTIVATED: LOSE LIFE");
			}

			if (CheatString == GameOverCommand)
			{
				playerControllerScript_P1.GameOver ();
				Debug.Log ("Player forced game over.");
				ShowCheatNotification ("CHEAT ACTIVATED: FORCE GAME OVER");
			}
		}
	}

	// What happens when cheats get enabled.
	void ShowCheatActivation (string cheatText)
	{
		LastCheatName = CheatString;
		CheatInputText.text = LastCheatName;
		CheatNotificationAnim.Play ("CheatNotification");
		CheatNotifcationText.text = cheatText;

		Instantiate (CheatSound, transform.position, Quaternion.identity);

		ClearCheatString ();

		timeScaleControllerScript.OverrideTimeScaleTimeRemaining = 0.5f;
		timeScaleControllerScript.OverridingTimeScale = 0.2f;

		playerControllerScript_P1.NextFire = 0;
		playerControllerScript_P1.DoubleShotNextFire = 0;
		playerControllerScript_P1.TripleShotNextFire = 0;
		playerControllerScript_P1.RippleShotNextFire = 0;

		Debug.Log (cheatText);
		Invoke ("DisableCheatConsole", 0.5f);
	}

	// Cheat string was matched.
	void ShowCheatNotification (string cheatText)
	{
		CheatNotificationAnim.StopPlayback ();
		CheatNotificationAnim.Play ("CheatNotification");
		CheatNotifcationText.text = cheatText;

		playerControllerScript_P1.CheckPowerupImageUI ();

		Instantiate (CheatSound, transform.position, Quaternion.identity);

		LastCheatName = CheatString;
		ClearCheatString ();

		timeScaleControllerScript.OverrideTimeScaleTimeRemaining = 0.5f;
		timeScaleControllerScript.OverridingTimeScale = 0.2f;

		playerControllerScript_P1.NextFire = 0;
		playerControllerScript_P1.DoubleShotNextFire = 0;
		playerControllerScript_P1.TripleShotNextFire = 0;
		playerControllerScript_P1.RippleShotNextFire = 0;

		Debug.Log (cheatText);
		Invoke ("DisableCheatConsole", 0.5f);
	}

	// Turn off cheat console.
	void DisableCheatConsole ()
	{
		CheatConsole.SetActive (false);
	}
}
