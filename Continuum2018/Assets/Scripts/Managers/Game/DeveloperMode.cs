using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using InControl;
using TMPro;

public class DeveloperMode : MonoBehaviour 
{
	public static DeveloperMode Instance { get ; private set; }
	public bool OnlyInEditor = true;
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

		#if !UNITY_EDITOR
		if (OnlyInEditor)
		{
			this.enabled = false;
		}
		#endif
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
		if (InputManager.Devices.Count > 0)
		{
			if (InputManager.Devices [0].LeftBumper.WasPressed) 
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

		if (Input.GetKeyDown (KeyCode.Tab))
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
					TutorialManager.Instance.gameObject.SetActive (false);
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
					GameController.Instance.Lives += 1;
					GameController.Instance.MaxLivesText.text = "";

					RawImage NewLife = GameController.Instance.LifeImages [GameController.Instance.Lives - 2];

					NewLife.gameObject.SetActive (true);
					NewLife.enabled = true;
					NewLife.color = Color.white;
					NewLife.texture = NewLife.GetComponent<TextureSwapper> ().Textures [0];

					ShowCheatNotification ("CHEAT ACTIVATED: EXTRA LIFE");
				}

				// On full lives.
				if (GameController.Instance.Lives > (GameController.Instance.MaxLives - 1)) 
				{
					// Reverse loop down and deactivate lives objects up to icon #1.
					for (int i = 9; i > 0; i--) 
					{
						GameController.Instance.LifeImages [i].gameObject.SetActive (false);
						GameController.Instance.LifeImages [i].enabled = false;
					}

					// Enable max lives text.
					GameController.Instance.LivesText.gameObject.SetActive (true);
					GameController.Instance.LivesText.text = "x " + (GameController.Instance.MaxLives - 1);
					GameController.Instance.MaxLivesText.text = "MAX";
					Debug.Log ("Reached maximum lives.");

					ShowCheatNotification ("CHEAT ACTIVATED: MAX LIVES");
				}
					
				GameController.Instance.Lives = Mathf.Clamp (GameController.Instance.Lives, 0, GameController.Instance.MaxLives);
			}

			if (CheatString == ToggleGodmodeCommand) 
			{
				isGod = !isGod;

				if (isGod == true)
				{
					PlayerController.PlayerOneInstance.playerCol.enabled = false;
					PlayerController.PlayerOneInstance.playerTrigger.enabled = false;

					ShowCheatNotification ("CHEAT ACTIVATED: GOD ON");
				}

				if (isGod == false) 
				{
					PlayerController.PlayerOneInstance.playerCol.enabled = true;
					PlayerController.PlayerOneInstance.playerTrigger.enabled = true;

					ShowCheatNotification ("CHEAT ACTIVATED: GOD OFF");
				}
			}

			if (CheatString == ChargeAbilityCommand) 
			{
				GameController.Instance.CurrentAbilityTimeRemaining = GameController.Instance.CurrentAbilityDuration;

				PlayerController.PlayerOneInstance.AbilityCompletion.Play ("AbilityComplete");
				PlayerController.PlayerOneInstance.AbilityCompletionTexture.texture = PlayerController.PlayerOneInstance.AbilityImage.texture;
				PlayerController.PlayerOneInstance.AbilityCompletionText.text = PlayerController.ParseByCase (PlayerController.PlayerOneInstance.Ability.ToString ());

				if (PlayerController.PlayerOneInstance.timeIsSlowed == false) 
				{
					TimescaleController.Instance.OverrideTimeScaleTimeRemaining += 1f;
					TimescaleController.Instance.OverridingTimeScale = 0.3f;
				}

				PlayerController.PlayerOneInstance.CurrentAbilityState = PlayerController.abilityState.Ready;

				ShowCheatNotification ("CHEAT ACTIVATED: ABILITY CHARGED");
			}

			if (CheatString == RefreshAbilityCommand) 
			{
				PlayerController.PlayerOneInstance.RefreshAbilityName ();
				PlayerController.PlayerOneInstance.RefreshAbilityImage ();

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
				if (PlayerController.PlayerOneInstance.ShotType == PlayerController.shotType.Standard) 
				{
					PlayerController.PlayerOneInstance.AddParticleActiveEffects ();
				}

				GameController.Instance.SetPowerupTime (20);

				PlayerController.PlayerOneInstance.ShotType = PlayerController.shotType.Double;
				PlayerController.PlayerOneInstance.CurrentShootingHeatCost = PlayerController.PlayerOneInstance.DoubleShootingHeatCost;

				if (PlayerController.PlayerOneInstance.isInOverdrive == true) 
				{
					PlayerController.PlayerOneInstance.DoubleShotIteration = PlayerController.shotIteration.Overdrive;
				}

				if (PlayerController.PlayerOneInstance.isInRapidFire == false) 
				{
					PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.DoubleShotFireRates [0];
				}

				if (PlayerController.PlayerOneInstance.isInRapidFire == true) 
				{
					PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.DoubleShotFireRates [1];
				}
					
				GameController.Instance.PowerupImage_P1[0].texture = DoubleShotTexture;
				GameController.Instance.PowerupImage_P1[0].color = new Color (1, 1, 1, 1);
				GameController.Instance.PowerupImage_P1[0].gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");

				GameObject powerupPickupUI = Instantiate (
					GameController.Instance.PowerupPickupUI, 
					PlayerController.PlayerOneInstance.playerCol.transform.position, 
					Quaternion.identity
				);

				powerupPickupUI.GetComponentInChildren<RawImage> ().texture = DoubleShotTexture;
				powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (0.3f, 0.7f, 1, 1);

				ShowCheatNotification ("CHEAT ACTIVATED: DOUBLE SHOT");
			}

			if (CheatString == TripleShotCommand)
			{
				if (PlayerController.PlayerOneInstance.ShotType == PlayerController.shotType.Standard) 
				{
					PlayerController.PlayerOneInstance.AddParticleActiveEffects ();
				}

				GameController.Instance.SetPowerupTime (20);

				PlayerController.PlayerOneInstance.ShotType = PlayerController.shotType.Triple;
				PlayerController.PlayerOneInstance.CurrentShootingHeatCost = PlayerController.PlayerOneInstance.TripleShootingHeatCost;

				if (PlayerController.PlayerOneInstance.isInOverdrive == true) 
				{
					PlayerController.PlayerOneInstance.TripleShotIteration = PlayerController.shotIteration.Overdrive;
				}

				if (PlayerController.PlayerOneInstance.isInRapidFire == false) 
				{
					PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.TripleShotFireRates [0];
				}

				if (PlayerController.PlayerOneInstance.isInRapidFire == true) 
				{
					PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.TripleShotFireRates [1];
				}

				GameController.Instance.PowerupImage_P1[0].texture = TripleShotTexture;
				GameController.Instance.PowerupImage_P1[0].color = new Color (1, 1, 1, 1);
				GameController.Instance.PowerupImage_P1[0].gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");

				GameObject powerupPickupUI = Instantiate (GameController.Instance.PowerupPickupUI, PlayerController.PlayerOneInstance.playerCol.transform.position, Quaternion.identity);
				powerupPickupUI.GetComponentInChildren<RawImage> ().texture = TripleShotTexture;
				powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (0.435f, 0.717f, 1, 1);

				ShowCheatNotification ("CHEAT ACTIVATED: TRIPLE SHOT");
			}

		if (CheatString == RippleShotCommand)
		{
			if (PlayerController.PlayerOneInstance.ShotType == PlayerController.shotType.Standard) 
			{
				PlayerController.PlayerOneInstance.AddParticleActiveEffects ();
			}

			GameController.Instance.SetPowerupTime (20);

			PlayerController.PlayerOneInstance.ShotType = PlayerController.shotType.Ripple;
			PlayerController.PlayerOneInstance.CurrentShootingHeatCost = PlayerController.PlayerOneInstance.RippleShootingHeatCost;

			if (PlayerController.PlayerOneInstance.isInOverdrive == true || PlayerController.PlayerOneInstance.isHoming == true) 
			{
				PlayerController.PlayerOneInstance.RippleShotIteration = PlayerController.shotIteration.Overdrive;
			}

			if (PlayerController.PlayerOneInstance.isInRapidFire == false) 
			{
				PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.RippleShotFireRates [0];
			}

			if (PlayerController.PlayerOneInstance.isInRapidFire == true) 
			{
				PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.RippleShotFireRates [1];
			}

			GameController.Instance.PowerupImage_P1[0].texture = RippleShotTexture;
			GameController.Instance.PowerupImage_P1[0].color = new Color (1, 1, 1, 1);
			GameController.Instance.PowerupImage_P1[0].gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");

			GameObject powerupPickupUI = Instantiate (GameController.Instance.PowerupPickupUI, PlayerController.PlayerOneInstance.playerCol.transform.position, Quaternion.identity);
			powerupPickupUI.GetComponentInChildren<RawImage> ().texture = RippleShotTexture;
			powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (0.31f, 0.372f, 1, 1);

			ShowCheatNotification ("CHEAT ACTIVATED: RIPPLE SHOT");
		}

			if (CheatString == TurretCommand)
			{
				if (PlayerController.PlayerOneInstance.nextTurretSpawn < 4) 
				{
					GameController.Instance.SetPowerupTime (20);

					GameObject Turret = PlayerController.PlayerOneInstance.Turrets [PlayerController.PlayerOneInstance.nextTurretSpawn];
					Turret.SetActive (true);
					Turret.GetComponent<Turret> ().playerControllerScript = PlayerController.PlayerOneInstance;

					UpdatePowerupImages (GameController.Instance.NextPowerupSlot_P1, TurretTexture, Color.white);

					GameController.Instance.NextPowerupSlot_P1 += 1;
					PlayerController.PlayerOneInstance.nextTurretSpawn += 1;

					GameObject powerupPickupUI = Instantiate (GameController.Instance.PowerupPickupUI, PlayerController.PlayerOneInstance.playerCol.transform.position, Quaternion.identity);
					powerupPickupUI.GetComponentInChildren<RawImage> ().texture = TurretTexture;
					powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (0.31f, 0.372f, 1, 1);

					ShowCheatNotification ("CHEAT ACTIVATED: POWERUP - TURRET");
					PlayerController.PlayerOneInstance.AddParticleActiveEffects ();
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
				PlayerController.PlayerOneInstance.isInRapidFire = !PlayerController.PlayerOneInstance.isInRapidFire;

				if (PlayerController.PlayerOneInstance.isInRapidFire == true)
				{
					GameController.Instance.SetPowerupTime (20);
					GameController.Instance.RapidfireImage.transform.SetSiblingIndex (-GameController.Instance.NextPowerupShootingSlot_P1 + 3);
					GameController.Instance.NextPowerupShootingSlot_P1 += 1;
					GameController.Instance.RapidfireImage.enabled = true;
					GameController.Instance.RapidfireHex.enabled = true;
					GameController.Instance.RapidfireImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");

					switch (PlayerController.PlayerOneInstance.ShotType) 
					{
					case PlayerController.shotType.Double:
						PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.DoubleShotFireRates [1];
						break;
					case PlayerController.shotType.Triple:
						PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.TripleShotFireRates [1];
						break;
					case PlayerController.shotType.Ripple:
						PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.RippleShotFireRates [1];
						break;
					case PlayerController.shotType.Standard:
						PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.DoubleShotFireRates [1];
						break;
					}

					GameObject powerupPickupUI = Instantiate (GameController.Instance.PowerupPickupUI, PlayerController.PlayerOneInstance.playerCol.transform.position, Quaternion.identity);
					powerupPickupUI.GetComponentInChildren<RawImage> ().texture = RapidfireTexture;
					powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (0.207f, 0.866f, 1, 1);

					ShowCheatNotification ("CHEAT ACTIVATED: RAPIDFIRE: ON");
					PlayerController.PlayerOneInstance.AddParticleActiveEffects ();
				}

				if (PlayerController.PlayerOneInstance.isInRapidFire == false)
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
				PlayerController.PlayerOneInstance.isInOverdrive = !PlayerController.PlayerOneInstance.isInOverdrive;

				if (PlayerController.PlayerOneInstance.isInOverdrive == true) 
				{
					GameController.Instance.SetPowerupTime (20);
					GameController.Instance.OverdriveImage.transform.SetSiblingIndex (-GameController.Instance.NextPowerupShootingSlot_P1 + 3);
					GameController.Instance.NextPowerupShootingSlot_P1 += 1;
					GameController.Instance.OverdriveImage.enabled = true;
					GameController.Instance.OverdriveHex.enabled = true;
					GameController.Instance.OverdriveImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");

					PlayerController.PlayerOneInstance.StandardShotIteration = PlayerController.shotIteration.Overdrive;
					PlayerController.PlayerOneInstance.DoubleShotIteration = PlayerController.shotIteration.Overdrive;
					PlayerController.PlayerOneInstance.TripleShotIteration = PlayerController.shotIteration.Overdrive;
					PlayerController.PlayerOneInstance.RippleShotIteration = PlayerController.shotIteration.Overdrive;

					GameObject powerupPickupUI = Instantiate (
						GameController.Instance.PowerupPickupUI, 
						PlayerController.PlayerOneInstance.playerCol.transform.position, 
						Quaternion.identity
					);

					powerupPickupUI.GetComponentInChildren<RawImage> ().texture = OverdriveTexture;
					powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (0.898f, 0.25f, 1, 1);

					ShowCheatNotification ("CHEAT ACTIVATED: OVERDRIVE MODE ON");
					PlayerController.PlayerOneInstance.AddParticleActiveEffects ();
				}

				if (PlayerController.PlayerOneInstance.isInOverdrive == false)
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
				PlayerController.PlayerOneInstance.isRicochet = !PlayerController.PlayerOneInstance.isRicochet;

				if (PlayerController.PlayerOneInstance.isRicochet == true) 
				{
					GameController.Instance.SetPowerupTime (20);
					PlayerController.PlayerOneInstance.EnableRicochetObject ();

					if (PlayerController.PlayerOneInstance.DoubleShotIteration != PlayerController.shotIteration.Overdrive) 
					{
						PlayerController.PlayerOneInstance.DoubleShotIteration = PlayerController.shotIteration.Enhanced;
					}

					if (PlayerController.PlayerOneInstance.TripleShotIteration != PlayerController.shotIteration.Overdrive) 
					{
						PlayerController.PlayerOneInstance.TripleShotIteration = PlayerController.shotIteration.Enhanced;
					}

					if (PlayerController.PlayerOneInstance.RippleShotIteration != PlayerController.shotIteration.Overdrive) 
					{
						PlayerController.PlayerOneInstance.RippleShotIteration = PlayerController.shotIteration.Enhanced;
					}

					if (PlayerController.PlayerOneInstance.StandardShotIteration != PlayerController.shotIteration.Overdrive)
					{
						PlayerController.PlayerOneInstance.StandardShotIteration = PlayerController.shotIteration.Enhanced;
					}

					PlayerController.PlayerOneInstance.isRicochet = true;
						
					GameController.Instance.RicochetImage.transform.SetSiblingIndex (-GameController.Instance.NextPowerupShootingSlot_P1 + 3);
					GameController.Instance.NextPowerupShootingSlot_P1 += 1;
					GameController.Instance.RicochetImage.enabled = true;
					GameController.Instance.RicochetHex.enabled = true;
					GameController.Instance.RicochetImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");

					GameObject powerupPickupUI = Instantiate (
						GameController.Instance.PowerupPickupUI,
						PlayerController.PlayerOneInstance.playerCol.transform.position, 
						Quaternion.identity
					);

					powerupPickupUI.GetComponentInChildren<RawImage> ().texture = RicochetTexture;
					powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (0.25f, 1, 0.565f, 1);

					ShowCheatNotification ("CHEAT ACTIVATED: RICOCHET MODE ON");
					PlayerController.PlayerOneInstance.AddParticleActiveEffects ();
				}

				if (PlayerController.PlayerOneInstance.isRicochet == false) 
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

				PlayerController.PlayerOneInstance.isHoming = !PlayerController.PlayerOneInstance.isHoming;

				if (PlayerController.PlayerOneInstance.isHoming == true) 
				{
					GameController.Instance.SetPowerupTime (20);
					GameController.Instance.HomingImage.transform.SetSiblingIndex (-GameController.Instance.NextPowerupShootingSlot_P1 + 3);
					GameController.Instance.NextPowerupShootingSlot_P1 += 1;
					GameController.Instance.HomingImage.enabled = true;
					GameController.Instance.HomingHex.enabled = true;
					GameController.Instance.HomingImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");

					GameObject powerupPickupUI = Instantiate (
						GameController.Instance.PowerupPickupUI, 
						PlayerController.PlayerOneInstance.playerCol.transform.position, 
						Quaternion.identity
					);

					powerupPickupUI.GetComponentInChildren<RawImage> ().texture = HomingTexture;
					powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (0.57f, 1, 0.277f, 1);

					ShowCheatNotification ("CHEAT ACTIVATED: HOMING MODE ON");
					PlayerController.PlayerOneInstance.AddParticleActiveEffects ();
				}

				if (PlayerController.PlayerOneInstance.isHoming == false) 
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

				if (PlayerController.PlayerOneInstance.Helix.activeInHierarchy == false)
				{
					PlayerController.PlayerOneInstance.Helix.SetActive (true);

					UpdatePowerupImages (GameController.Instance.NextPowerupSlot_P1, HelixTexture, Color.white);
					GameController.Instance.NextPowerupSlot_P1 += 1;

					PlayerController.PlayerOneInstance.AddParticleActiveEffects ();
				}

				GameObject powerupPickupUI = Instantiate (GameController.Instance.PowerupPickupUI, PlayerController.PlayerOneInstance.playerCol.transform.position, Quaternion.identity);

				powerupPickupUI.GetComponentInChildren<RawImage> ().texture = HelixTexture;
				powerupPickupUI.GetComponentInChildren<RawImage> ().color = new Color (1f, 0.278f, 0.561f, 1);

				ShowCheatNotification ("CHEAT ACTIVATED: POWERUP - HELIX");
			}

			if (CheatString == SlowTimeCommand) 
			{
				if (PlayerController.PlayerOneInstance.timeIsSlowed == false) 
				{
					GameController.Instance.SetPowerupTime (20);

					if (GameController.Instance.VhsAnim.GetCurrentAnimatorStateInfo (0).IsName ("Slow") == false)
					{
						GameController.Instance.VhsAnim.SetTrigger ("Slow");
					}

					PlayerController.PlayerOneInstance.timeIsSlowed = true;

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

				PlayerController.PlayerOneInstance.ShotType = PlayerController.shotType.Standard;
				PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.StandardFireRate;
				PlayerController.PlayerOneInstance.isInOverdrive = false;
				PlayerController.PlayerOneInstance.isInRapidFire = false;

				ShowCheatNotification ("CHEAT ACTIVATED: STANDARD SHOT");
			}

			if (CheatString == ResetAllPowerupsCommand) 
			{
				GameController.Instance.PowerupTimeRemaining = 0;

				PlayerController.PlayerOneInstance.ResetPowerups ();
				PlayerController.PlayerOneInstance.CurrentShootingHeatCost = PlayerController.PlayerOneInstance.StandardShootingHeatCost;

				ShowCheatNotification ("CHEAT ACTIVATED: RESET POWERUPS");
			}

			if (CheatString == ShieldCommand) 
			{
				PlayerController.PlayerOneInstance.Ability = PlayerController.ability.Shield;
				PlayerController.PlayerOneInstance.RefreshAbilityName ();
				PlayerController.PlayerOneInstance.RefreshAbilityImage ();
					
				ShowCheatNotification ("CHEAT ACTIVATED: ABILITY - SHIELD");
			}

			if (CheatString == VerticalBeamCommand) 
			{
				PlayerController.PlayerOneInstance.Ability = PlayerController.ability.VerticalBeam;
				PlayerController.PlayerOneInstance.RefreshAbilityName ();
				PlayerController.PlayerOneInstance.RefreshAbilityImage ();

				ShowCheatNotification ("CHEAT ACTIVATED: ABILITY - VERTICAL BEAM");
			}

			if (CheatString == HorizontalBeamCommand) 
			{
				PlayerController.PlayerOneInstance.Ability = PlayerController.ability.HorizontalBeam;
				PlayerController.PlayerOneInstance.RefreshAbilityName ();
				PlayerController.PlayerOneInstance.RefreshAbilityImage ();

				ShowCheatNotification ("CHEAT ACTIVATED: ABILITY - HORIZONTAL BEAM");
			}

			if (CheatString == EmpCommand) 
			{
				PlayerController.PlayerOneInstance.Ability = PlayerController.ability.Emp;
				PlayerController.PlayerOneInstance.RefreshAbilityName ();
				PlayerController.PlayerOneInstance.RefreshAbilityImage ();

				ShowCheatNotification ("CHEAT ACTIVATED: ABILITY - EMP");
			}

			if (CheatString == RewindCommand) 
			{
				PlayerController.PlayerOneInstance.Ability = PlayerController.ability.Rewind;
				PlayerController.PlayerOneInstance.RefreshAbilityName ();
				PlayerController.PlayerOneInstance.RefreshAbilityImage ();
	
				ShowCheatNotification ("CHEAT ACTIVATED: ABILITY - REWIND TIME");
			}

			if (CheatString == MirrorPlayerCommand) 
			{
				PlayerController.PlayerOneInstance.Ability = PlayerController.ability.Mirror;
				PlayerController.PlayerOneInstance.RefreshAbilityName ();
				PlayerController.PlayerOneInstance.RefreshAbilityImage ();

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
				GameController.Instance.LivesText.text = "";
				GameController.Instance.MaxLivesText.text = "";

				// Enable all objects up to life count.
				for (int i = 0; i < GameController.Instance.Lives; i++) 
				{
					GameController.Instance.LifeImages [i].gameObject.SetActive (true);
					GameController.Instance.LifeImages [i].enabled = true;
				}
					
				// Turn off all icons above it.
				for (int i = GameController.Instance.Lives; i < GameController.Instance.MaxLives; i++) 
				{
					GameController.Instance.LifeImages [i - 1].gameObject.SetActive (false);
					GameController.Instance.LifeImages [i - 1].enabled = false;
				}
					
				GameController.Instance.Lives = Mathf.Clamp (GameController.Instance.Lives, 0, GameController.Instance.MaxLives);
				ShowCheatNotification ("CHEAT ACTIVATED: LOSE LIFE");
			}

			if (CheatString == GameOverCommand)
			{
				PlayerController.PlayerOneInstance.GameOver ();
				ShowCheatNotification ("CHEAT ACTIVATED: FORCE GAME OVER");
			}

			if (CheatString == UseOverheatCommand) 
			{
				PlayerController.PlayerOneInstance.useOverheat = !PlayerController.PlayerOneInstance.useOverheat;

				if (PlayerController.PlayerOneInstance.useOverheat == true) 
				{
					ShowCheatNotification ("CHEAT ACTIVATED: OVERHEAT ON");
				}

				if (PlayerController.PlayerOneInstance.useOverheat == false) 
				{
					PlayerController.PlayerOneInstance.Overheated = false;
					PlayerController.PlayerOneInstance.CurrentShootingHeat = 0;
					PlayerController.PlayerOneInstance.CurrentShootingCooldown = 0;
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
		RawImage powerupImage = GameController.Instance.PowerupImage_P1 [index];
		
		powerupImage.gameObject.SetActive (true);
		powerupImage.texture = powerupTex;
		powerupImage.color = powerupCol;
		powerupImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");
	}

	// What happens when cheats get enabled.
	void ShowCheatActivation (string cheatText)
	{
		// Gives new next fire amount so the player can keep on firing.
		float nextfire = 
			Time.time + 
			(PlayerController.PlayerOneInstance.CurrentFireRate / (PlayerController.PlayerOneInstance.FireRateTimeMultiplier * Time.timeScale));
		
		LastCheatName = CheatString;
		CheatInputText.text = LastCheatName;
		CheatNotificationAnim.Play ("CheatNotification");
		CheatNotifcationText.text = cheatText;

		Instantiate (CheatSound, transform.position, Quaternion.identity);

		ClearCheatString ();

		if (PlayerController.PlayerOneInstance.timeIsSlowed == false)
		{
			TimescaleController.Instance.OverrideTimeScaleTimeRemaining += 0.5f;
			TimescaleController.Instance.OverridingTimeScale = 0.2f;
		}

		PlayerController.PlayerOneInstance.NextFire = nextfire;
		PlayerController.PlayerOneInstance.DoubleShotNextFire = nextfire;
		PlayerController.PlayerOneInstance.TripleShotNextFire = nextfire;
		PlayerController.PlayerOneInstance.RippleShotNextFire = nextfire;

		Debug.Log (cheatText);
		Invoke ("DisableCheatConsole", 0.5f);
	}

	// Cheat string was matched.
	void ShowCheatNotification (string cheatText)
	{
		// Gives new next fire amount so the player can keep on firing.
		float nextfire = 
			Time.time + 
			(PlayerController.PlayerOneInstance.CurrentFireRate / (PlayerController.PlayerOneInstance.FireRateTimeMultiplier * Time.timeScale));
		
		CheatNotificationAnim.StopPlayback ();
		CheatNotificationAnim.Play ("CheatNotification");
		CheatNotifcationText.text = cheatText;

		PlayerController.PlayerOneInstance.CheckPowerupImageUI ();

		Instantiate (CheatSound, transform.position, Quaternion.identity);

		LastCheatName = CheatString;
		//Invoke ("ClearCheatString", 0.1f);
		ClearCheatString ();

		if (TimescaleController.Instance.OverrideTimeScaleTimeRemaining < 0.5f && PlayerController.PlayerOneInstance.timeIsSlowed) 
		{
			TimescaleController.Instance.OverrideTimeScaleTimeRemaining += 0.5f;
		}

		if (TimescaleController.Instance.OverridingTimeScale < 0.2f && PlayerController.PlayerOneInstance.timeIsSlowed)
		{
			TimescaleController.Instance.OverridingTimeScale = 0.2f;
		}

		PlayerController.PlayerOneInstance.NextFire = nextfire ;
		PlayerController.PlayerOneInstance.DoubleShotNextFire = nextfire ;
		PlayerController.PlayerOneInstance.TripleShotNextFire = nextfire ;
		PlayerController.PlayerOneInstance.RippleShotNextFire = nextfire ;

		Debug.Log (cheatText);
		Invoke ("DisableCheatConsole", 0.5f);
	}

	// Turn off cheat console.
	void DisableCheatConsole ()
	{
		CheatConsole.SetActive (false);
	}
}