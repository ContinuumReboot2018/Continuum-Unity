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
	public TutorialManager tutorialManagerScript;
	public bool forceStarted;

	[Header ("Cheats")]
	//public GameObject CheatConsole;
	//public TextMeshProUGUI CheatInputText;
	public string CheatString;
	public string LastCheatName;
	public float CheatStringResetTimeRemaining = 1.0f;
	public float CheatStringResetDuration = 1.0f;
	public int maxCheatCharacters = 12;

	public bool useCheats;
	public bool allowCheats = false;
	public bool showCheats;
	public bool isGod;
	//public bool cheatInProcess;

	[Header ("Cheat Commands")]
	public string ToggleCheatsCommand = "continuum";
	public string ForceStartCommand = "start";
	public string ForceRestartCommand = "restart";

	public string FpsUnlockCommand = "fpsunlock";
	public string Fps30Command = "fps30";
	public string Fps60Command = "fps60";
	public string Fps90Command = "fps90";
	public string Fps120Command = "fps120";

	public string NextTrackCommand = "nexttrack";
	public string PreviousTrackCommand = "previoustrack";
	public string RandomTrackCommand = "randomtrack";

	public string SaveSettingsCommand = "savesettings";
	public string LoadSettingsCommand = "loadsettings";

	public string ToggleGodmodeCommand = "god";
	public string AddLifeCommand = "life";

	public string ChargeAbilityCommand = "chargeability";
	public string RefreshAbilityCommand = "refreshability";

	public string SpawnBlockCommand = "spblock";
	public string SpawnPowerupPickupCommand = "sppow";

	public string PowerupTimeCommand = "poweruptime";

	public string DoubleShotCommand = "double";
	public string TripleShotCommand = "triple";
	public string RippleShotCommand = "ripple";
	public string CloneCommand = "clone";
	public string HelixCommand = "helix";

	public string NextWaveCommand = "nextwave";
	public string PreviousWaveCommand = "lastwave";

	public string RapidfireCommand = "rapid";
	public string OverdriveCommand = "overdrive";
	public string RicochetCommand = "ricochet";
	public string StandardShotCommand = "standard";
	public string ResetAllPowerupsCommand = "resetpow";

	public string ShieldCommand = "shield";
	public string VerticalBeamCommand = "vbeam";

	public string AddScoreCommand = "addscore";
	public string LoseLifeCommand = "loselife";
	public string GameOverCommand = "gameover";

	[Header ("UI and Animations")]
	public GameObject CheatsMenu;
	public Animator CheatsMenuAnim;
	public Animator CheatNotificationAnim;
	public TextMeshProUGUI CheatNotifcationText;
	public GameObject CheatSound;

	public Texture2D DoubleShotTexture;
	public Texture2D TripleShotTexture;
	public Texture2D RippleShotTexture;

	public Texture2D RapidfireTexture;
	public Texture2D OverdriveTexture;

	public Texture2D CloneTexture;
	public Texture2D HelixTexture;

	public Color RapidFireColor;
	public Color OverdriveColor;

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
		// CheatInputText.text = LastCheatName;
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
			//CheatInputText.text = ">_ ";
		}

		foreach (char c in Input.inputString) 
		{		
			CheatStringResetTimeRemaining = CheatStringResetDuration;
			CheatString += c;

			//CheatInputText.text = "> " + CheatString;

			if (CheatString.Contains (c.ToString () + c.ToString () + c.ToString ()))
			{
				ClearCheatString ();
				//CheatInputText.text = ">_ ";
			}

			if (CheatString.Contains ("`")) 
			{
				ClearCheatString ();
			}
		}

		if (CheatString.Length > maxCheatCharacters) 
		{
			ClearCheatString ();
			//CheatInputText.text = ">_ ";
		}

		if (Input.GetKeyDown (KeyCode.Backspace)) 
		{
			ClearCheatString ();
			//CheatInputText.text = ">_ ";
		}

		if (CheatString == ToggleCheatsCommand
		   && allowCheats == true)
		{
			useCheats = !useCheats;

			if (useCheats == true) {
				Debug.Log ("Enabled cheats.");
				ShowCheatActivation ("CHEATS ON");
			}

			if (useCheats == false) {
				Debug.Log ("Disabled cheats.");
				ShowCheatActivation ("CHEATS OFF");
			}
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

			if (CheatString == AddLifeCommand) {
				gameControllerScript.Lives += 3;
				ShowCheatNotification ("CHEAT ACTIVATED: LIFE");
			}

			if (CheatString == ToggleGodmodeCommand) 
			{
				isGod = !isGod;
				//playerControllerScript_P1.playerCol.enabled = !playerControllerScript_P1.playerCol.enabled;

				if (isGod == true)
				{
					playerControllerScript_P1.playerCol.enabled = false;
					ShowCheatNotification ("CHEAT ACTIVATED: GOD ON");
				}

				if (isGod == false) 
				{
					playerControllerScript_P1.playerCol.enabled = true;
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
				ShowCheatNotification ("CHEAT ACTIVATED: ABILITY REFRESH");
			}

			if (CheatString == SpawnBlockCommand)
			{
				gameControllerScript.SpawnBlock (true);
				ShowCheatNotification ("CHEAT ACTIVATED: SPAWN BLOCK");
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
				playerControllerScript_P1.ShotType = PlayerController.shotType.Double;

				if (playerControllerScript_P1.isInOverdrive == true) 
				{
					playerControllerScript_P1.DoubleShotIteration = PlayerController.shotIteration.Overdrive;
				}

				if (playerControllerScript_P1.isInRapidFire == true) 
				{
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.DoubleShotFireRates [1];
				}

				gameControllerScript.PowerupImage_P1[0].texture = DoubleShotTexture;
				gameControllerScript.PowerupImage_P1[0].color = new Color (1, 1, 1, 1);
				gameControllerScript.PowerupText_P1[0].text = "";

				ShowCheatNotification ("CHEAT ACTIVATED: DOUBLE SHOT");
			}

			if (CheatString == TripleShotCommand)
			{
				playerControllerScript_P1.ShotType = PlayerController.shotType.Triple;

				if (playerControllerScript_P1.isInOverdrive == true) 
				{
					playerControllerScript_P1.TripleShotIteration = PlayerController.shotIteration.Overdrive;
				}

				if (playerControllerScript_P1.isInRapidFire == true) 
				{
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.TripleShotFireRates [1];
				}

				gameControllerScript.PowerupImage_P1[0].texture = TripleShotTexture;
				gameControllerScript.PowerupImage_P1[0].color = new Color (1, 1, 1, 1);
				gameControllerScript.PowerupText_P1[0].text = "";

				ShowCheatNotification ("CHEAT ACTIVATED: TRIPLE SHOT");
			}

		if (CheatString == RippleShotCommand)
		{
			playerControllerScript_P1.ShotType = PlayerController.shotType.Ripple;

			if (playerControllerScript_P1.isInOverdrive == true) 
			{
				playerControllerScript_P1.RippleShotIteration = PlayerController.shotIteration.Overdrive;
			}

			if (playerControllerScript_P1.isInRapidFire == true) 
			{
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.RippleShotFireRates [1];
			}

			gameControllerScript.PowerupImage_P1[0].texture = RippleShotTexture;
			gameControllerScript.PowerupImage_P1[0].color = new Color (1, 1, 1, 1);
			gameControllerScript.PowerupText_P1[0].text = "";

			ShowCheatNotification ("CHEAT ACTIVATED: RIPPLE SHOT");
		}

			if (CheatString == CloneCommand)
			{
				if (playerControllerScript_P1.nextCloneSpawn < 4) 
				{
					gameControllerScript.SetPowerupTime (20);

					GameObject clone = playerControllerScript_P1.Clones [playerControllerScript_P1.nextCloneSpawn];
					clone.SetActive (true);
					clone.GetComponent<ClonePlayer> ().playerControllerScript = playerControllerScript_P1;

					gameControllerScript.PowerupText_P1 [gameControllerScript.NextPowerupSlot_P1].text = "";
					gameControllerScript.PowerupImage_P1 [gameControllerScript.NextPowerupSlot_P1].texture = CloneTexture;
					gameControllerScript.PowerupImage_P1 [gameControllerScript.NextPowerupSlot_P1].color = Color.white;

					gameControllerScript.NextPowerupSlot_P1 += 1;
					playerControllerScript_P1.nextCloneSpawn += 1;

					ShowCheatNotification ("CHEAT ACTIVATED: CLONE");
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
				gameControllerScript.SetPowerupTime (20);

				if (playerControllerScript_P1.isInRapidFire == false)
				{
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

					gameControllerScript.PowerupImage_P1 [gameControllerScript.NextPowerupSlot_P1].texture = RapidfireTexture;
					gameControllerScript.PowerupImage_P1 [gameControllerScript.NextPowerupSlot_P1].color = RapidFireColor;
					gameControllerScript.PowerupText_P1 [gameControllerScript.NextPowerupSlot_P1].text = "";
					//gameControllerScript.NextPowerupSlot_P1 += 1;
					gameControllerScript.RapidfireImage.enabled = true;
					playerControllerScript_P1.isInRapidFire = true;
				}

				ShowCheatNotification ("CHEAT ACTIVATED: RAPIDFIRE: " + playerControllerScript_P1.ShotType.ToString().ToUpper ());
			}

			if (CheatString == OverdriveCommand) 
			{
				gameControllerScript.SetPowerupTime (20);

				if (playerControllerScript_P1.isInOverdrive == false) 
				{
					playerControllerScript_P1.StandardShotIteration = PlayerController.shotIteration.Overdrive;
					playerControllerScript_P1.DoubleShotIteration = PlayerController.shotIteration.Overdrive;
					playerControllerScript_P1.TripleShotIteration = PlayerController.shotIteration.Overdrive;
					playerControllerScript_P1.RippleShotIteration = PlayerController.shotIteration.Overdrive;

					if (playerControllerScript_P1.ShotType != PlayerController.shotType.Standard)
					{
						gameControllerScript.PowerupImage_P1 [gameControllerScript.NextPowerupSlot_P1].texture = OverdriveTexture;
						gameControllerScript.PowerupImage_P1 [gameControllerScript.NextPowerupSlot_P1].color = OverdriveColor;
						gameControllerScript.PowerupText_P1 [gameControllerScript.NextPowerupSlot_P1].text = "";
						//gameControllerScript.NextPowerupSlot_P1 += 1;
					}

					gameControllerScript.OverdriveImage.enabled = true;
					playerControllerScript_P1.isInOverdrive = true;
				}

				ShowCheatNotification ("CHEAT ACTIVATED: OVERDRIVE MODE");
			}

			if (CheatString == RicochetCommand) 
			{
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
				ShowCheatNotification ("CHEAT ACTIVATED: RICOCHET MODE");
			}

			if (CheatString == HelixCommand) 
			{
				if (playerControllerScript_P1.Helix.activeInHierarchy == false)
				{
					playerControllerScript_P1.Helix.SetActive (true);
					gameControllerScript.PowerupText_P1 [gameControllerScript.NextPowerupSlot_P1].text = "";
					gameControllerScript.PowerupImage_P1 [gameControllerScript.NextPowerupSlot_P1].texture = HelixTexture;
					gameControllerScript.PowerupImage_P1 [gameControllerScript.NextPowerupSlot_P1].color = Color.white;

					gameControllerScript.NextPowerupSlot_P1 += 1;
				}
				ShowCheatNotification ("CHEAT ACTIVATED: HELIX");
			}

			if (CheatString == StandardShotCommand) 
			{
				gameControllerScript.PowerupImage_P1 [0].texture = null;
				gameControllerScript.PowerupImage_P1 [0].color = new Color (0, 0, 0, 0);
				gameControllerScript.PowerupText_P1 [0].text = "";
				playerControllerScript_P1.ShotType = PlayerController.shotType.Standard;
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.StandardFireRate;
				ShowCheatNotification ("CHEAT ACTIVATED: STANDARD SHOT");
				playerControllerScript_P1.isInOverdrive = false;
				playerControllerScript_P1.isInRapidFire = false;
			}

			if (CheatString == ResetAllPowerupsCommand) 
			{
				gameControllerScript.PowerupTimeRemaining = 0;
				playerControllerScript_P1.ResetPowerups ();
				ShowCheatNotification ("CHEAT ACTIVATED: RESET POWERUPS");
			}

			if (CheatString == ShieldCommand) 
			{
				playerControllerScript_P1.AbilityName = "shield";
				playerControllerScript_P1.Ability = PlayerController.ability.Shield;
				playerControllerScript_P1.CurrentAbilityTimeRemaining = playerControllerScript_P1.CurrentAbilityDuration;
				playerControllerScript_P1.ActivateAbility ();
				ShowCheatNotification ("CHEAT ACTIVATED: SHIELD");
			}

			if (CheatString == VerticalBeamCommand) 
			{
				playerControllerScript_P1.AbilityName = "verticalbeam";
				playerControllerScript_P1.Ability = PlayerController.ability.VerticalBeam;
				playerControllerScript_P1.CurrentAbilityTimeRemaining = playerControllerScript_P1.CurrentAbilityDuration;
				playerControllerScript_P1.ActivateAbility ();
				ShowCheatNotification ("CHEAT ACTIVATED: VERTICAL BEAM");
			}
				
			if (Input.GetKeyDown (KeyCode.Alpha7))
			{
				CheatString = LastCheatName;
			}

			if (CheatString == AddScoreCommand) 
			{
				gameControllerScript.TargetScore += 100000;
				ShowCheatNotification ("CHEAT ACTIVATED: ADD SCORE");
			}

			if (CheatString == LoseLifeCommand) 
			{
				gameControllerScript.Lives -= 1;
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

	void ShowCheatActivation (string cheatText)
	{
		LastCheatName = CheatString;
		//CheatInputText.text = LastCheatName;
		CheatNotificationAnim.Play ("CheatNotification");
		CheatNotifcationText.text = cheatText;
		Instantiate (CheatSound, transform.position, Quaternion.identity);
		ClearCheatString ();
		Debug.Log (cheatText);
		Invoke ("DisableCheatConsole", 0.5f);
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
		Invoke ("DisableCheatConsole", 0.5f);
	}

	void DisableCheatConsole ()
	{
	}
}
