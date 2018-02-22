﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI; // Access to Unity's UI system.
using TMPro; // Access to TextMeshPro components.
using UnityEngine.PostProcessing; // Allows access to Unity's Post Processing Stack.

public class GameController : MonoBehaviour 
{
	public PlayerController playerControllerScript_P1;		// Reference to the player controller.
	public TimescaleController timescaleControllerScript;	// Reference to the timescale controller.
	public AudioController audioControllerScript;			// Reference to the audio controller.
	public DeveloperMode developerModeScript;				// Reference to the developer mode for debug info.
	public CursorManager cursorManagerScript;				// Reference to the cursor state.
	public PostProcessingProfile ImageEffects;				// Reference to the post processing profile.
	public GameModifierManager gameModifier;				// Reference to the GameModifierManager Scriptable object.								// Main camera.

	[Header ("Game Stats")]
	public bool TrackStats = true;		// Track debug info.
	public float GameTime;				// Scaled time elapsed since the start of the first wave.
	public float RealTime;				// Unscaled time elapsed since the start of the first wave.
	public float TimeRatio;				// GameTime / RealTime, can be greater than 1.
	public float Distance;				// Distance between player and reference point.
	public int BlocksDestroyed;			// Counter for how many blocks were destroyed.
	public int BulletsShot;				// Counter for bullets instantiated.
	public float BlockShotAccuracy;		// Blocks destroyed / Bullets shot.

	[Header ("Waves")]
	public int Wave;								// Current wave number.
	public TextMeshProUGUI WaveText;				// Wave number text.
	public float WaveTimeIncreaseRate;				// How much longer each wave duration increases per wave.
	public float FirstWaveTimeDuration;				// How long the first wave should last.
	public float WaveTimeDuration;					// Max wave duration time.
	public float WaveTimeRemaining;					// Wave time remaining until end of wave.
	public RawImage WaveBackground;					// Background of wave text.

	[Header ("Wave Transition")]
	public bool IsInWaveTransition;					// Allows wave transition animation.
	public ParticleSystem WaveTransitionParticles;  // Cool wave transition particle effect.
	public Animator WaveTransitionAnim;				// Animator for wave transition.
	public TextMeshProUGUI WaveTransitionText;		// Text for wave transition to show wave number.
	public AudioSource WaveTransitionAudio;			// Warp sound.

	// This is for wave transition UI only.
	public TextMeshProUGUI SoundtrackText;			// New soundtrack text. (Appears every four waves).
	public TextMeshProUGUI GameTimeText;			// Display for game time in wave transition.
	public TextMeshProUGUI RealTimeText;			// Display for real time in wave transition.
	public TextMeshProUGUI TimeRatioText;			// Display for time ration in wave transition.
	public TextMeshProUGUI BlocksDestroyedText;		// Display for blocks destroyed in wave transition.
	public TextMeshProUGUI BulletsShotText;			// Display for bullets shot in wave transition.
	public TextMeshProUGUI AccuracyText;			// Display for accuracy in wave transition.

	[Header ("Scoring")]
	public bool CountScore;				// Allow score to be incremented.
	public float DisplayScore;			// Currently displayed score.
	public float CurrentScore;			// Current score updated.
	public float TargetScore;			// Score to smoothly transition towards.
	public float ScoreSmoothing;		// How much smoothing is applied to target score.
	public TextMeshProUGUI ScoreText;	// Score UI text.
	public RawImage ScoreBackground;	// Dark background for score area.

	[Header ("Lives")]
	public int Lives;					 // Counter for lives remaining (UI displays one less as one life is the player itself).
	public int MaxLives = 11;			 // Maximum lives the player can have at any one time.
	public TextMeshProUGUI LivesText;	 // UI counter for lives.
	public RawImage[] LifeImages;		 // Life images x3
	public RawImage LivesBackground;	 // Dark background for Lives.
	public GameObject LivesSpacing;		 // Lives spacing determined by amount of lives.
	public TextMeshProUGUI MaxLivesText; // When lives count reaches max lives, display this text.

	[Header ("Combo")]
	public int combo = 1;				// Current combo.
	public float comboDuration = 0.5f;	// How long the combo lasts before decrementing.
	public float comboTimeRemaining;	// Current time left of the current combo before it decremements.

	[Header ("Stacking")]
	public GameObject StackingObject; // Stack zones parent object.

	[Header ("Block Spawner")]
	public GameObject[] Blocks;			 // Block prefabs to spawn based on wave number.
	public float BlockSpawnRate;		 // How often to spawn blocks.
	private float NextBlockSpawn;		 // Time to elapse before next block spawn. Time.time > this.
	public float blockSpawnStartDelay; 	 // Start delay for blocks to spawn.
	public float[] BlockSpawnXPositions; // Fixed x positions to line up with stack zones.
	public float BlockSpawnYPosition; 	 // Starting height of block spawn y position.
	public float BlockSpawnZPosition;	 // Block spawn depth position.
	public float BlockSpawnIncreaseRate; // How much faster blocks spawn every wave.

	[Header ("Powerup General")]
	public float PowerupTimeRemaining;				// Current powerup time left.
	public float PowerupTimeDuration;				// Maximum powerup time.
	public Animator PowerupAnim;					// Powerup animator for background light.
	public AudioSource PowerupTimeRunningOutAudio;  // Sound to play when powerup time < 3 seconds.
	public AudioSource PowerupResetAudio;			// Sound to play when powerups reset.
	public GameObject[] PowerupPickups;				// List of powerup pickups to spawn. 
	public float powerupPickupTimeRemaining;		// How long the next powerup pickup will spawn.
	public Vector2 PowerupPickupSpawnRate;			// Range of time powerup pickups are spawned.
	public float powerupPickupSpawnModifier = 1;	// Gets value from game modifier for multiplier.
	private float NextPowerupPickupSpawn;			// Time to pass before next powerup spawns.
	public float PowerupPickupSpawnRangeX;			// Horizontal area to spawn powerup pickups.
	public float PowerupPickupSpawnY;				// Vertical area to spawn powerup pickups.

	[Header ("Powerup List UI")]
	public int NextPowerupSlot_P1;		  // Powerup slot index.
	public RawImage[] PowerupImage_P1;	  // Array of powrup images for each slot. 
	public Texture2D StandardShotTexture; // Texture for default single standard shot.
	public RawImage HomingImage;		  // Image for homing.
	public RawImage HomingHex;			  // Hex background for homing.
	public RawImage RicochetImage;		  // Image for ricochet.
	public RawImage RicochetHex;		  // Hex background for ricochet.
	public RawImage RapidfireImage;		  // Image for rapidfire.
	public RawImage RapidfireHex;		  // Hex background for rapidfire.
	public RawImage OverdriveImage;		  // Image for overdrive.
	public RawImage OverdriveHex;		  // Hex background for overdrive.

	public GameObject PowerupPickupUI;	  // For other scripts to reference to spawn pickup UI.

	[Header ("Bosses")]
	public float BossSpawnDelay = 5; // Minimum amount of time for a boss to spawn.

	public GameObject[] MiniBosses;		 // List of minibosses to spawn at random.
	public Transform MiniBossSpawnPos;	 // Where to spawn the minibosses.
	public float MiniBossSpawnDelay = 4; // Start delay to spawn a boss after the wave ends.

	public GameObject[] BigBosses;		// Array of big bosses to spawn.
	public Transform BigBossSpawnPos;	// Where to spawn the spawned big boss.
	public float BigBossSpawnDelay = 4; // How long the delay is from the end of the wave to the big boss spawning.
	public int bossId;					// Boss id for the index in the big bosses array.

	[Header ("Pausing")]
	public bool isPaused;			// Checks whether game state is paused.
	public float PauseCooldown = 1; // How much time needs to pass before the player can pause and unpause again.
	[HideInInspector] 
	public float NextPauseCooldown;	// Timer for Pause cooldown.
	public bool canPause = true;	// Allow pausing or not.
	public GameObject PauseUI;		// UI for pause menu.
	public bool isInOtherMenu;		// Is the menu in the first pause menu or a sub menu?

	[Header ("Game Over")]
	public bool isGameOver;			// Did the player run out of lives?
	public GameObject GameoverUI;	// UI Game Over screen.

	[Header ("Camera")]
	public Camera MainCamera;			 // Main Camera GameObject.
	public float OrthSize = 10;			 // Current orthographic size of the camera.
	public float StartOrthSize = 10;	 // Starting orthographic size of the camera.
	private float OrthSizeVel;			 // Orthographic size smoothdamp amount.
	public float OrthSizeSmoothTime = 1; // Smmothing amount for orthographic change.

	[Header ("Visuals")]
	public bool isUpdatingImageEffects = true;	  // Allows updating image effects values.
	public bool isUpdatingParticleEffects = true; // Allows updating particle effect values.
	public float TargetDepthDistance;			  // For depth of field.

	[Header ("Star field")]
	public ParticleSystem StarFieldForeground;				  // Starfield foreground.
	public ParticleSystem StarFieldBackground;				  // Starfield background.
	public float StarFieldForegroundLifetimeMultipler = 0.1f; // Lifetime multiplier.
	public float StarFieldForegroundSimulationSpeed = 1;	  // How fast the particle simulation is.
	public float StarFieldBackgroundLifetimeMultipler = 0.1f; // Lifetime multiplier.
	public float StarFieldBackgroundSimulationSpeed = 1;	  // How fast the particle simulation is.

	// Debug info visuals in Debug UI menu.
	[Header ("Debug")]

	// Debug shooting stats.
	public TextMeshProUGUI P1_ShootingIterationRapid;
	public TextMeshProUGUI P1_ShootingIterationOverdrive;
	public TextMeshProUGUI P1_ShootingIterationRicochet;
	public TextMeshProUGUI P1_CurrentFireRate;
	[Space (10)]
	// Debug ability stats.
	public TextMeshProUGUI P1_Ability;
	public TextMeshProUGUI P1_AbilityTimeRemaining;
	public TextMeshProUGUI P1_AbilityTimeDuration;
	public TextMeshProUGUI P1_AbilityTimeProportion;
	[Space (10)]
	// Debug combo and wave stats.
	public TextMeshProUGUI WaveText_Debug;
	public TextMeshProUGUI WaveTimeRemainingText_Debug;
	public TextMeshProUGUI ComboText_Debug;
	[Space (10)]
	// Debug time stats.
	public TextMeshProUGUI DistanceText_Debug;
	public TextMeshProUGUI GameTimeText_Debug;
	public TextMeshProUGUI RealTimeText_Debug;
	public TextMeshProUGUI TimeRatioText_Debug;
	public TextMeshProUGUI TimeScaleText_Debug;
	public TextMeshProUGUI FixedTimeStepText_Debug;
	[Space (10)]
	// Debug score and block spawn stats.
	public TextMeshProUGUI TargetScoreText_Debug;
	public TextMeshProUGUI SpawnWaitText_Debug;
	[Space (10)]
	// Debug cheat stats.
	public TextMeshProUGUI CheatTimeRemainText_Debug;
	public TextMeshProUGUI CheatStringText_Debug;
	public TextMeshProUGUI LastCheatText_Debug;
	[Space (10)]
	// Debug powerup time stats.
	public TextMeshProUGUI PowerupTimeRemain_Debug;
	public TextMeshProUGUI AddedTimeText_Debug;
	[Space (10)]
	// Debug misc.
	public TextMeshProUGUI LivesText_Debug;
	[Space (10)]
	// Debug Audio Stats.
	public TextMeshProUGUI CurrentPitch_Debug;
	public TextMeshProUGUI TargetPitch_Debug;
	[Space (10)]
	// Debug wave transition stats.
	public TextMeshProUGUI BlocksDestroyedText_Debug;
	public TextMeshProUGUI BulletsShotText_Debug;
	public TextMeshProUGUI BlockShotAccuracyText_Debug;
	public TextMeshProUGUI RewindTimeRemainingText_Debug;
	public TextMeshProUGUI IsRewindingText_Debug;
	[Space (10)]
	// Debug modifiers.
	public TextMeshProUGUI Modifier_Tutorial_Debug;
	public TextMeshProUGUI Modifier_PowerupSpawn_Debug;
	public TextMeshProUGUI Modifier_BossSpawn_Debug;
	public TextMeshProUGUI Modifier_TimeIncrease_Debug;
	public TextMeshProUGUI Modifier_StartingLives_Debug;
	public TextMeshProUGUI Modifier_AlwaysHoming_Debug;
	public TextMeshProUGUI Modifier_AlwaysRicochet_Debug;
	public TextMeshProUGUI Modifier_AlwaysRapidfire_Debug;
	public TextMeshProUGUI Modifier_AlwaysOverdrive_Debug;
	public TextMeshProUGUI Modifier_TrialTime_Debug;
	public TextMeshProUGUI Modifier_BlockSpawnRateMult_Debug;

	void Awake () 
	{
		// Hide and lock the mouse.
		cursorManagerScript.HideMouse ();
		cursorManagerScript.LockMouse ();

		// Clear and reset UI for score, waves, and lives.
		ClearMainUI (); 
	}

	void ClearMainUI ()
	{
		// Clear score stuff.
		ScoreText.text = "";
		ScoreBackground.enabled = false;

		// Clear lives stuff.
		LivesText.text = "";
		MaxLivesText.text = "";
		LivesBackground.enabled = false;

		// Clear wave info stuff.
		Wave = 1;
		WaveTimeDuration = FirstWaveTimeDuration;
		WaveText.text = "";
		playerControllerScript_P1.WaveAnim.enabled = false;
		WaveBackground.enabled = false;
		IsInWaveTransition = false;
	}

	public void ClearPowerupUI ()
	{
		// Reset powerup modifiers.
		HomingImage.enabled = gameModifier.AlwaysHoming;
		HomingHex.enabled = gameModifier.AlwaysHoming;
		RicochetImage.enabled = gameModifier.AlwaysRicochet;
		RicochetHex.enabled = gameModifier.AlwaysRicochet;
		RapidfireImage.enabled = gameModifier.AlwaysRapidfire;
		RapidfireHex.enabled = gameModifier.AlwaysRapidfire;
		OverdriveImage.enabled = gameModifier.AlwaysOverdrive;
		OverdriveHex.enabled = gameModifier.AlwaysOverdrive;

		// Reset all powerup textures in list.
		foreach (RawImage powerupImage in PowerupImage_P1) 
		{
			powerupImage.texture = null;
			powerupImage.color = new Color (0, 0, 0, 0);
		}

		// Reset inital powerup slot index.
		NextPowerupSlot_P1 = 1;
	}

	void Start ()
	{
		InvokeRepeating ("UpdateBlockSpawnTime", 0, 1); // Refreshes block spawn time.
		InvokeRepeating ("UpdateLives", 0, 1); // Refreshes UI for lives.
		ClearPowerupUI (); // Clears powerup UI from list.
		SetGameModifiers (); // Applies game modifiers.

		UnityEngine.Debug.Log ("Camera aspect ratio = " + Camera.main.aspect.ToString ());
		InvokeRepeating ("SetStartOrthSize", 0, 1); // Checks orthographic size based on screen ratio.

		// Invokes a game over if the trial time is greater than 0. (Set to -1 just to be safe to avoid this).
		if (gameModifier.TrialTime > 0) 
		{
			playerControllerScript_P1.Invoke ("GameOver", gameModifier.TrialTime);
		}
	}

	// Timescale controller calls this initially after the countdown.
	public void StartGame ()
	{
		CurrentScore = 0;
		TargetScore = 0;
		DisplayScore = 0;

		TrackStats = true;

		WaveTransitionParticles.Play (true);
		WaveTransitionAnim.Play ("WaveTransition");
		IsInWaveTransition = true;
		WaveTimeRemaining = WaveTimeDuration;
		playerControllerScript_P1.AbilityUIHexes.Play ("HexesFadeIn");
		bossId = 0; // Reset boss ID.

		if (gameModifier.BossSpawn != GameModifierManager.bossSpawnMode.BossesOnly)
		{
			StartCoroutine (StartBlockSpawn ());
		}

		if (gameModifier.BossSpawn == GameModifierManager.bossSpawnMode.BossesOnly) 
		{
			if (Wave % 4 != 0)
			{
				Invoke("SpawnMiniBossObject", BossSpawnDelay);
			}

			if (Wave % 4 == 0) 
			{
				Invoke("SpawnBigBossObject", BossSpawnDelay);
			}
		}

		playerControllerScript_P1.ScoreAnim.enabled = true;
		playerControllerScript_P1.LivesAnim.gameObject.SetActive (true);
		playerControllerScript_P1.LivesAnim.enabled = true;
		playerControllerScript_P1.WaveAnim.enabled = true;
		ScoreBackground.enabled = true;
		LivesBackground.enabled = true;
		WaveBackground.enabled = true;

		BlockSpawnRate -= (BlockSpawnIncreaseRate * gameModifier.blockSpawnRateMultiplier);
		UnityEngine.Debug.Log ("Block spawn rate: " + BlockSpawnRate);

		Lives = gameModifier.StartingLives;
		UpdateLives ();
	}

	void Update ()
	{
		UpdateGameStats ();
		UpdateTimeStats ();
		CheckCombo ();
		CheckPowerupTime ();
		CheckWaveTime ();
		UpdateScoreIncrements ();
		UpdateStarFieldParticleEffects ();
		//UpdateImageEffects ();
		LevelTimer ();
		PowerupSpawner ();
	}

	void UpdateStarFieldParticleEffects ()
	{
		if (isUpdatingParticleEffects == true) 
		{
			var StarFieldForegroundTrailModule = StarFieldForeground.trails;
			StarFieldForegroundTrailModule.lifetime = new ParticleSystem.MinMaxCurve (0, StarFieldForegroundLifetimeMultipler * Time.timeScale);

			var StarFieldForegroundMainModule = StarFieldForeground.main;
			StarFieldForegroundMainModule.simulationSpeed = StarFieldForegroundSimulationSpeed * Time.timeScale;

			var StarFieldBackgroundTrailModule = StarFieldBackground.trails;
			StarFieldBackgroundTrailModule.lifetime = new ParticleSystem.MinMaxCurve (0, StarFieldBackgroundLifetimeMultipler * Time.timeScale);

			var StarFieldBackgroundMainModule = StarFieldBackground.main;
			StarFieldBackgroundMainModule.simulationSpeed = StarFieldBackgroundSimulationSpeed * Time.timeScale;
		}
	}

	void UpdateGameStats ()
	{
		if (TrackStats == true && isPaused == false) 
		{
			// Put debug stuff here.
			if (developerModeScript.DebugMenu.activeInHierarchy == true)
			{
				WaveText_Debug.text = 
					"Wave: " + Wave;
				TargetScoreText_Debug.text = 
					"Target Score: " + Mathf.Round (TargetScore);
				ComboText_Debug.text = 
					"Combo: " + combo; 
				DistanceText_Debug.text = 
					"Distance: " + Math.Round (Distance, 2);
				GameTimeText_Debug.text = 
					"Game Time: " + string.Format ("{0}:{1:00}", (int)GameTime / 60, (int)GameTime % 60);
				RealTimeText_Debug.text = 
					"Real Time: " + string.Format ("{0}:{1:00}", (int)RealTime / 60, (int)RealTime % 60);
				TimeRatioText_Debug.text = 
					"Time Ratio: " + Math.Round (TimeRatio, 2);
				TargetPitch_Debug.text = 
					"Target Pitch: " + audioControllerScript.BassTargetPitch;
				CurrentPitch_Debug.text = 
					"Current Pitch: " + Math.Round (audioControllerScript.BassTrack.pitch, 4);
				TimeScaleText_Debug.text = 
					"Time.timeScale: " + Math.Round (Time.timeScale, 2);
				FixedTimeStepText_Debug.text = 
					"Time.fixedDeltaTime: " + Math.Round (Time.fixedDeltaTime, 5);
				SpawnWaitText_Debug.text = 
					"Spawn Rate: " + BlockSpawnRate;
				WaveTimeRemainingText_Debug.text = 
					"Wave Time Remain: " + Math.Round (WaveTimeRemaining, 1) + " s";
				P1_CurrentFireRate.text = 
					"P1 Fire Rate: " + playerControllerScript_P1.CurrentFireRate;
				P1_Ability.text = 
					"P1 Ability: " + playerControllerScript_P1.AbilityName;
				P1_AbilityTimeRemaining.text = 
					"P1 Ability Remain: " + Math.Round (playerControllerScript_P1.CurrentAbilityTimeRemaining, 2);
				P1_AbilityTimeDuration.text = 
					"P1 Max Ability Time: " + playerControllerScript_P1.CurrentAbilityDuration;
				P1_AbilityTimeProportion.text = 
					"P1 Ability Fill: " + Math.Round (playerControllerScript_P1.AbilityTimeAmountProportion, 2);
				CheatTimeRemainText_Debug.text = 
					"Cheat Time Remain: " + Math.Round (developerModeScript.CheatStringResetTimeRemaining, 1);
				CheatStringText_Debug.text = 
					"Cheat Input Field: " + developerModeScript.CheatString;
				LastCheatText_Debug.text = 
					"Last Cheat: " + developerModeScript.LastCheatName;
				PowerupTimeRemain_Debug.text = 
					"Powerup Time Remain: " + Math.Round (PowerupTimeRemaining, 1);
				P1_ShootingIterationRapid.text = 
					"Rapid Fire: " + (playerControllerScript_P1.isInRapidFire ? "ON" : "OFF");
				P1_ShootingIterationOverdrive.text = 
					"Overdrive: " + (playerControllerScript_P1.isInOverdrive ? "ON" : "OFF");
				P1_ShootingIterationRicochet.text = 
					"Ricochet: " + (playerControllerScript_P1.isRicochet ? "ON" : "OFF");
				AddedTimeText_Debug.text = 
					"Added Time: " + System.Math.Round((timescaleControllerScript.TargetTimeScaleAdd), 3);
				LivesText_Debug.text = 
					"Lives: " + Lives;
				BlocksDestroyedText_Debug.text = 
					"Blocks Destroyed: " + BlocksDestroyed;
				BulletsShotText_Debug.text = 
					"Bullets Shot: " + BulletsShot;

				BlockShotAccuracy = Mathf.Clamp (BlocksDestroyed / (BulletsShot + Mathf.Epsilon), 0, 10000);
				BlockShotAccuracyText_Debug.text = 
					"Block Shot Accuracy: " + (System.Math.Round((BlockShotAccuracy * 100), 2)) + "%";

				RewindTimeRemainingText_Debug.text = 
					"Rewind Time Remain: " + System.Math.Round(timescaleControllerScript.RewindTimeRemaining, 2);
				IsRewindingText_Debug.text = 
					"Is Rewinding: " + (timescaleControllerScript.isRewinding ? "ON" : "OFF");

				Modifier_Tutorial_Debug.text = "Use Tutorial: " + (gameModifier.Tutorial ? "ON" : "OFF");
				Modifier_PowerupSpawn_Debug.text = "Powerup Spawn Mode: " + (gameModifier.PowerupSpawn.ToString ());
				Modifier_BossSpawn_Debug.text = "Boss Spawn Mode: " + (gameModifier.BossSpawn.ToString ());
				Modifier_TimeIncrease_Debug.text = "Time Increase Mode: " + (gameModifier.TimeIncreaseMode.ToString ());
				Modifier_StartingLives_Debug.text = "Starting Lives: " + (gameModifier.StartingLives);
				Modifier_AlwaysHoming_Debug.text = "Always Homing: " + (gameModifier.AlwaysHoming ? "ON" : "OFF");
				Modifier_AlwaysRicochet_Debug.text = "Always Ricochet: " + (gameModifier.AlwaysRicochet ? "ON" : "OFF");
				Modifier_AlwaysRapidfire_Debug.text = "Always rapidfire: " + (gameModifier.AlwaysRapidfire ? "ON" : "OFF");
				Modifier_AlwaysOverdrive_Debug.text = "Always overdrive: " + (gameModifier.AlwaysOverdrive ? "ON" : "OFF");
				Modifier_TrialTime_Debug.text = "Trial Time: " + (gameModifier.TrialTime);
				Modifier_BlockSpawnRateMult_Debug.text = "Block Spawn Rate Mult: " + gameModifier.blockSpawnRateMultiplier;
			}
		}
	}

	void UpdateScoreIncrements ()
	{
		if (CountScore == true) 
		{
			// Adds score over time.
			//TargetScore += ScoreRate * Time.deltaTime * ScoreMult * Time.timeScale;

			CurrentScore = Mathf.Lerp (CurrentScore, TargetScore, ScoreSmoothing * Time.unscaledDeltaTime);
			DisplayScore = Mathf.Round (CurrentScore);

			if (playerControllerScript_P1.tutorialManagerScript.tutorialComplete == true)
			{
				if (DisplayScore <= 0) 
				{
					ScoreText.text = "0";
				}

				if (DisplayScore > 0 && DisplayScore < 1000) 
				{
					ScoreText.text = DisplayScore.ToString ("###");
				}

				if (DisplayScore >= 1000 && DisplayScore < 10000) 
				{
					ScoreText.text = DisplayScore.ToString ("# ###");
				}

				if (DisplayScore >= 10000 && DisplayScore < 100000) 
				{
					ScoreText.text = DisplayScore.ToString ("## ###");
				}

				if (DisplayScore >= 100000 && DisplayScore < 1000000) 
				{
					ScoreText.text = DisplayScore.ToString ("### ###");
				}

				if (DisplayScore >= 1000000 && DisplayScore < 10000000) 
				{
					ScoreText.text = DisplayScore.ToString ("# ### ###");
				}

				if (DisplayScore >= 10000000 && DisplayScore < 100000000) 
				{
					ScoreText.text = DisplayScore.ToString ("## ### ###");
				}

				if (DisplayScore >= 100000000 && DisplayScore < 1000000000) 
				{
					ScoreText.text = DisplayScore.ToString ("### ### ###");
				}

				if (DisplayScore >= 1000000000 && DisplayScore < 10000000000) 
				{
					ScoreText.text = DisplayScore.ToString ("# ### ### ###");
				}

				if (DisplayScore >= 10000000000 && DisplayScore < 100000000000) 
				{
					ScoreText.text = DisplayScore.ToString ("## ### ### ###");
				}

				if (DisplayScore >= 100000000000 && DisplayScore < 1000000000000) 
				{
					ScoreText.text = DisplayScore.ToString ("### ### ### ###");
				}

				if (DisplayScore >= 1000000000000 && DisplayScore < 10000000000000) 
				{
					ScoreText.text = DisplayScore.ToString ("# ### ### ### ###");
				}
			}
		}
	}

	public void UpdateLives ()
	{
		if (timescaleControllerScript.isInInitialSequence == true || timescaleControllerScript.isInInitialCountdownSequence == true) 
		{
			if (LivesText.gameObject.activeSelf == true) 
			{
				LifeImages [0].enabled = false;
				LifeImages [1].enabled = false;
				LifeImages [2].enabled = false;
				LivesText.gameObject.SetActive (false);
				LivesText.text = "";
			}
		}

		if (timescaleControllerScript.isInInitialSequence == false && timescaleControllerScript.isInInitialCountdownSequence == false) 
		{
			// Check how many life images are supposed to be there
			switch (Lives) 
			{
			case 0:
				if (LivesText.gameObject.activeSelf == true) 
				{
					LifeImages [0].enabled = false;
					LifeImages [1].enabled = false;
					LifeImages [2].enabled = false;
					LivesSpacing.SetActive (false);
					LivesText.gameObject.SetActive (false);
					LivesText.text = "";
					MaxLivesText.text = "";
				}
				break;
			case 1:
				LifeImages [0].enabled = false;
				LifeImages [1].enabled = false;
				LifeImages [2].enabled = false;
				LivesSpacing.SetActive (false);
				LivesText.gameObject.SetActive (false);
				LivesText.text = "";
				MaxLivesText.text = "";
				break;
			case 2:
				LifeImages [0].enabled = true;
				LifeImages [1].enabled = false;
				LifeImages [2].enabled = false;
				LivesSpacing.SetActive (false);
				LivesText.gameObject.SetActive (false);
				LivesText.text = "";
				MaxLivesText.text = "";
				break;
			case 3:
				LifeImages [0].enabled = true;
				LifeImages [1].enabled = true;
				LifeImages [2].enabled = false;
				LivesSpacing.SetActive (false);
				LivesText.gameObject.SetActive (false);
				LivesText.text = "";
				MaxLivesText.text = "";
				break;
			case 4:
				LifeImages [0].enabled = true;
				LifeImages [1].enabled = true;
				LifeImages [2].enabled = true;
				LivesSpacing.SetActive (false);
				LivesText.gameObject.SetActive (false);
				LivesText.text = "";
				MaxLivesText.text = "";
				break;
			case 10:
				MaxLivesText.text = "";
				break;
			case 11:
				MaxLivesText.text = "MAX";
				break;
			}

			if (Lives > 4) 
			{
				LifeImages [0].enabled = true;
				LifeImages [1].enabled = false;
				LifeImages [2].enabled = false;
				LivesSpacing.SetActive (true);
				LivesText.gameObject.SetActive (true);
				LivesText.text = "x " + (Lives - 1);
			}
		}
	}

	// Combo time remaining variable is timed and decreases based on what combo it is already on.
	void CheckCombo ()
	{
		if (comboTimeRemaining > 0 && isGameOver == false) 
		{
			comboTimeRemaining -= Time.unscaledDeltaTime * 0.5f * combo;
		}

		// Decrements a combo when the timer runs out and resets.
		if (comboTimeRemaining < 0) 
		{
			if (combo > 1) 
			{
				combo -= 1;
				comboTimeRemaining = comboDuration;
			}
		}
	}

	void CheckPowerupTime ()
	{
		if (PowerupTimeRemaining > 0 && isPaused == false && isGameOver == false) 
		{
			PowerupTimeRemaining -= Time.unscaledDeltaTime;

			if (PowerupTimeRemaining < 3) 
			{
				if (PowerupAnim.GetCurrentAnimatorStateInfo (0).IsName ("PowerupTimeRunningOut") == false) 
				{
					if (PowerupTimeRunningOutAudio.isPlaying == false) 
					{
						PowerupTimeRunningOutAudio.Play ();
					}
					PowerupAnim.Play ("PowerupTimeRunningOut");
					playerControllerScript_P1.TurretSpinSpeed = playerControllerScript_P1.TurretSpinSpeedFaster;
				}
			}
		}

		if (PowerupTimeRemaining < 0) 
		{
			playerControllerScript_P1.powerupsInUse = 0;
			PowerupTimeRemaining = 0;
			PowerupAnim.StopPlayback ();
			PowerupResetAudio.Play ();
			// Reset all powerups for all players.
			playerControllerScript_P1.ResetPowerups ();
		}
	}

	public void SetPowerupTime (float Duration)
	{
		PowerupTimeRemaining += Duration;
		PowerupTimeRemaining = Mathf.Clamp (PowerupTimeRemaining, 0, PowerupTimeDuration);
	}

	void UpdateTimeStats ()
	{
		if (isPaused == false && TrackStats == true) 
		{
			Distance = timescaleControllerScript.Distance;
			GameTime += Time.deltaTime;
			RealTime += Time.unscaledDeltaTime;
			TimeRatio = GameTime / RealTime;

			GameTimeText.text = "GAME TIME: " + 
				(GameTime / 60 / 60).ToString ("00") + " " + // To hours. 
				(GameTime/60).ToString("00") + "\' " + // To minutes.
				(GameTime % 60).ToString("00") + "\""; // To seconds.

			RealTimeText.text = "REAL TIME: " + 
				(RealTime / 60 / 60).ToString ("00") + " " + // To hours. 
				(RealTime/60).ToString("00") + "\' " + // To minutes.
				(RealTime % 60).ToString("00") + "\""; // To seconds.

			TimeRatioText.text = "TIME RATIO: " + System.Math.Round((GameTime / RealTime), 2).ToString ("0.00") + "";

			BlocksDestroyedText.text = "BLOCKS DESTROYED: " + BlocksDestroyed;
			BulletsShotText.text = "BULLETS SHOT: " + BulletsShot;
			AccuracyText.text = "ACCURACY: " + System.Math.Round(BlockShotAccuracy * 100, 2) + "%";
		}
	}

	public void ResetScore ()
	{
		TargetScore = 0;
		CurrentScore = 0;
		DisplayScore = 0;
		ScoreText.text = "" + DisplayScore;
	}

	void UpdateImageEffects ()
	{
		if (isUpdatingImageEffects == true)
		{
			if (isPaused == true) 
			{
				//var motionblursettings = ImageEffects.motionBlur.settings;
				//motionblursettings.frameBlending = 0;
				//ImageEffects.motionBlur.settings = motionblursettings;
			}

			if (isPaused == false)
			{
				if (timescaleControllerScript.isInInitialSequence == false && timescaleControllerScript.isInInitialCountdownSequence == false)
				{
					//var ImageEffectsMotionBlurModuleSettings = ImageEffects.motionBlur.settings;
					//ImageEffectsMotionBlurModuleSettings.frameBlending = Mathf.Clamp (-0.12f * timescaleControllerScript.Distance + 1.18f, 0, 1); 
					//ImageEffects.motionBlur.settings = ImageEffectsMotionBlurModuleSettings;

					//var ImageEffectsDofModuleSettings = ImageEffects.depthOfField.settings;
					//ImageEffectsDofModuleSettings.focusDistance = Mathf.Clamp (
						//Mathf.Lerp (ImageEffectsDofModuleSettings.focusDistance, TargetDepthDistance, Time.deltaTime), 0.1f, 100); 
					//ImageEffects.depthOfField.settings = ImageEffectsDofModuleSettings;
				}

				if (timescaleControllerScript.isInInitialSequence == true || timescaleControllerScript.isInInitialCountdownSequence == true) 
				{
					//var ImageEffectsMotionBlurModuleSettings = ImageEffects.motionBlur.settings;
					//ImageEffectsMotionBlurModuleSettings.frameBlending = 0; 
					//ImageEffects.motionBlur.settings = ImageEffectsMotionBlurModuleSettings;
				}
			}
		}
	}

	public void CheckPause ()
	{
		if (isInOtherMenu == false)
		{
			isPaused = !isPaused;

			// Stop updating required scripts.
			if (isPaused) 
			{
				PauseUI.SetActive (true);
				cursorManagerScript.UnlockMouse ();
				cursorManagerScript.ShowMouse ();

				audioControllerScript.updateVolumeAndPitches = false;
				audioControllerScript.BassTrack.pitch = 0;
				CountScore = false;
				timescaleControllerScript.isOverridingTimeScale = true;
				timescaleControllerScript.OverridingTimeScale = 0;
				timescaleControllerScript.OverrideTimeScaleTimeRemaining = 0.1f;
			}

			// Restart updating required scripts.
			if (!isPaused) 
			{
				UnPauseGame ();
			}

			NextPauseCooldown = Time.unscaledTime + PauseCooldown;
		}
	}

	public void InvokeUnpause ()
	{
		if (isInOtherMenu == false) 
		{
			isPaused = false;
			UnPauseGame ();
			NextPauseCooldown = Time.unscaledTime + PauseCooldown;
		}
	}

	void UnPauseGame ()
	{
		TargetDepthDistance = 100;
		playerControllerScript_P1.UsePlayerFollow = true;
		playerControllerScript_P1.canShoot = true;
		PauseUI.SetActive (false);
		cursorManagerScript.LockMouse ();
		cursorManagerScript.HideMouse ();
		audioControllerScript.updateVolumeAndPitches = true;
		audioControllerScript.BassTrack.pitch = 1;

		if (timescaleControllerScript.isInInitialSequence == false && 
			timescaleControllerScript.isInInitialCountdownSequence == false) 
		{
			CountScore = true;
		}

		timescaleControllerScript.isOverridingTimeScale = false;
		timescaleControllerScript.OverrideTimeScaleTimeRemaining = 0;
	}

	public void SetPauseOtherMenu (bool otherMenu)
	{
		isInOtherMenu = otherMenu;
	}

	public void SetStartOrthSize ()
	{
		// 16:9 ratio.
		if (MainCamera.aspect > 1.6f) 
		{
			MainCamera.orthographicSize = 12.0f;
		}

		// 16:10 ratio.
		if (MainCamera.aspect <= 1.6f) 
		{
			MainCamera.orthographicSize = 13.35f;
		}

		playerControllerScript_P1.lensScript.ratio = 1 / MainCamera.aspect;
	}

	void CheckOrthSize ()
	{
		OrthSize = -0.27f * (timescaleControllerScript.Distance) + 10;
		OrthSize = 4 * Mathf.Sin (0.17f * timescaleControllerScript.Distance) + 8;

		MainCamera.orthographicSize = Mathf.SmoothDamp (
			MainCamera.orthographicSize, 
			OrthSize, 
			ref OrthSizeVel, 
			OrthSizeSmoothTime * Time.deltaTime
		);
	}

	public void LevelTimer ()
	{
		if (WaveTimeRemaining > 0 && IsInWaveTransition == false) 
		{
			if (playerControllerScript_P1.isInCooldownMode == false && isPaused == false && isGameOver == false) 
			{
				WaveTimeRemaining -= Time.deltaTime;
			}
		}
	}

	public void NextLevel ()
	{
		if (Wave > 1) 
		{
			BlockSpawnRate -= (BlockSpawnIncreaseRate * gameModifier.blockSpawnRateMultiplier);
			UnityEngine.Debug.Log ("Block spawn rate: " + BlockSpawnRate);
		}

		WaveTimeDuration += WaveTimeIncreaseRate;
		WaveTimeRemaining = WaveTimeDuration;
		WaveTransitionParticles.Play (true);
		WaveTransitionAnim.Play ("WaveTransition");
		WaveTransitionText.text = "WAVE " + Wave;
		IsInWaveTransition = true;

		playerControllerScript_P1.camShakeScript.ShakeCam (0.4f, 3.7f, 99);
		playerControllerScript_P1.Vibrate (0.6f, 0.6f, 3);

		WaveTransitionAudio.Play ();
	}

	IEnumerator StartBlockSpawn ()
	{
		yield return new WaitForSeconds (blockSpawnStartDelay);
		WaveText.text = "WAVE " + Wave;

		//int StartXPosId = Random.Range (0, BlockSpawnXPositions.Length);
		//int NextXPosId = StartXPosId;

		while (WaveTimeRemaining > 0) 
		{
			if (Time.time > NextBlockSpawn && playerControllerScript_P1.isInCooldownMode == false && isPaused == false)
			{
				SpawnBlock (false);

				/*
				// Creates a stream of blocks.
				float RandomRange = Random.Range (0, 1.0f);

				// Move left.
				if (RandomRange < 0.33f) 
				{
					if (NextXPosId > 0) 
					{
						NextXPosId -= 1;
					}

					if (NextXPosId <= 0) 
					{
						NextXPosId = 0;
					}
				}

				// Go straight.
				if (RandomRange >= 0.33f && RandomRange < 0.67f) 
				{
				}

				// Move right.
				if (RandomRange >= 0.67f) 
				{
					if (NextXPosId < BlockSpawnXPositions.Length - 1) 
					{
						NextXPosId += 1;
					}

					if (NextXPosId >= BlockSpawnXPositions.Length) 
					{
						
					}
				}*/

				//Vector3 SpawnPos = new Vector3 (BlockSpawnXPositions[NextXPosId], BlockSpawnYPosition, BlockSpawnZPosition);
				//Instantiate (Block, SpawnPos, Quaternion.identity);

				NextBlockSpawn = Time.time + BlockSpawnRate;
			}
			yield return null;
		}
	}

	public void SpawnBlock (bool anyBlock)
	{
		if (anyBlock == false) 
		{
			if (isGameOver == false) 
			{
				if (Wave == 1) 
				{
					GameObject Block = Blocks [UnityEngine.Random.Range (0, 1)];
					Vector3 SpawnPosRand = new Vector3 (BlockSpawnXPositions [UnityEngine.Random.Range (0, BlockSpawnXPositions.Length)], BlockSpawnYPosition, BlockSpawnZPosition);
					Instantiate (Block, SpawnPosRand, Quaternion.identity);
				}

				if (Wave == 2) 
				{
					GameObject Block = Blocks [UnityEngine.Random.Range (0, 2)];
					Vector3 SpawnPosRand = new Vector3 (BlockSpawnXPositions [UnityEngine.Random.Range (0, BlockSpawnXPositions.Length)], BlockSpawnYPosition, BlockSpawnZPosition);
					Instantiate (Block, SpawnPosRand, Quaternion.identity);
				}

				if (Wave == 3) 
				{
					GameObject Block = Blocks [UnityEngine.Random.Range (0, 3)];
					Vector3 SpawnPosRand = new Vector3 (BlockSpawnXPositions [UnityEngine.Random.Range (0, BlockSpawnXPositions.Length)], BlockSpawnYPosition, BlockSpawnZPosition);
					Instantiate (Block, SpawnPosRand, Quaternion.identity);
				}

				if (Wave == 4) 
				{
					GameObject Block = Blocks [UnityEngine.Random.Range (0, 4)];
					Vector3 SpawnPosRand = new Vector3 (BlockSpawnXPositions [UnityEngine.Random.Range (0, BlockSpawnXPositions.Length)], BlockSpawnYPosition, BlockSpawnZPosition);
					Instantiate (Block, SpawnPosRand, Quaternion.identity);
				}

				if (Wave >= 5 && Wave < 9) 
				{
					GameObject Block = Blocks [UnityEngine.Random.Range (0, 6)];
					Vector3 SpawnPosRand = new Vector3 (BlockSpawnXPositions [UnityEngine.Random.Range (0, BlockSpawnXPositions.Length)], BlockSpawnYPosition, BlockSpawnZPosition);
					Instantiate (Block, SpawnPosRand, Quaternion.identity);
				}

				if (Wave >= 9) 
				{
					GameObject Block = Blocks [UnityEngine.Random.Range (0, Blocks.Length)];
					Vector3 SpawnPosRand = new Vector3 (BlockSpawnXPositions [UnityEngine.Random.Range (0, BlockSpawnXPositions.Length)], BlockSpawnYPosition, BlockSpawnZPosition);
					Instantiate (Block, SpawnPosRand, Quaternion.identity);
				}
			}
		}

		if (anyBlock == true) 
		{
			if (isGameOver == false) 
			{
				GameObject Block = Blocks [UnityEngine.Random.Range (0, Blocks.Length)];
				Vector3 SpawnPosRand = new Vector3 (BlockSpawnXPositions [UnityEngine.Random.Range (0, BlockSpawnXPositions.Length)], BlockSpawnYPosition, BlockSpawnZPosition);
				Instantiate (Block, SpawnPosRand, Quaternion.identity);
			}
		}
	}

	void UpdateBlockSpawnTime ()
	{
		if (isPaused == false && Lives > 0 && WaveTimeRemaining > 0) 
		{
			//BlockSpawnRate -= BlockSpawnIncreaseRate * Time.unscaledDeltaTime;
		}

		BlockSpawnRate = Mathf.Clamp (BlockSpawnRate, 0.0333f, 10);
	}

	void PowerupSpawner ()
	{
		if (isGameOver == false && isPaused == false && playerControllerScript_P1.tutorialManagerScript.tutorialComplete == true) 
		{
			//powerupPickupTimeRemaining -= Time.unscaledDeltaTime;
			powerupPickupTimeRemaining -= Time.deltaTime;

			if (powerupPickupTimeRemaining <= 0) 
			{
				SpawnPowerupPickup ();
			}
		}
	}

	public void SpawnPowerupPickup ()
	{
		GameObject PowerupPickup = PowerupPickups[UnityEngine.Random.Range (0, PowerupPickups.Length)];

		Vector3 PowerupPickupSpawnPos = new Vector3 (
			UnityEngine.Random.Range (-PowerupPickupSpawnRangeX, PowerupPickupSpawnRangeX), 
			UnityEngine.Random.Range (-PowerupPickupSpawnY, PowerupPickupSpawnY), 
			-2.5f
		);

		Instantiate (PowerupPickup, PowerupPickupSpawnPos, Quaternion.identity);

		powerupPickupTimeRemaining = UnityEngine.Random.Range (
			PowerupPickupSpawnRate.x * powerupPickupSpawnModifier, 
			PowerupPickupSpawnRate.y * powerupPickupSpawnModifier
		);
	}

	public IEnumerator SpawnMiniBoss ()
	{
		yield return new WaitForSeconds (MiniBossSpawnDelay);
		SpawnMiniBossObject ();
	}

	public void SpawnMiniBossObject ()
	{
		GameObject MiniBoss = MiniBosses [UnityEngine.Random.Range (0, MiniBosses.Length)];
		Instantiate (MiniBoss, MiniBossSpawnPos.position, MiniBossSpawnPos.rotation);
		UnityEngine.Debug.Log ("Spawned a mini boss.");
	}

	public IEnumerator SpawnBigBoss ()
	{
		yield return new WaitForSeconds (BigBossSpawnDelay);
		SpawnBigBossObject ();

		if (bossId <= BigBosses.Length) 
		{
			bossId += 1;
		}

		if (bossId > BigBosses.Length) 
		{
			bossId = 0;
		}
	}

	public void SpawnBigBossObject ()
	{
		GameObject BigBoss = BigBosses [UnityEngine.Random.Range (0, bossId)];
		Instantiate (BigBoss, BigBossSpawnPos.position, BigBossSpawnPos.rotation);
		UnityEngine.Debug.Log ("Oh snap! We spawned a big boss!");
	}

	void CheckWaveTime ()
	{
		if (WaveTimeRemaining < 0) 
		{
			if (Wave % 4 != 0)
			{
				if (gameModifier.BossSpawn == GameModifierManager.bossSpawnMode.Normal)
				{
					StartCoroutine (SpawnMiniBoss ());
				}

				if (gameModifier.BossSpawn == GameModifierManager.bossSpawnMode.NoBosses) 
				{
					StartNewWave ();
					IsInWaveTransition = true;
				}

				if (Wave % 4 != 1)
				{
					SoundtrackText.text = "";
				}
			}

			if (Wave % 4 == 0)
			{
				audioControllerScript.StopAllSoundtracks ();

				if (gameModifier.BossSpawn == GameModifierManager.bossSpawnMode.Normal)
				{
					StartCoroutine (SpawnBigBoss ());
				}

				if (gameModifier.BossSpawn == GameModifierManager.bossSpawnMode.NoBosses) 
				{
					StartNewWave ();
					IsInWaveTransition = true;
				}

				SoundtrackText.text = "";
			}

			WaveTimeRemaining = 0;
		}

		// For every wave after a major boss fight.
		if (Wave % 4 == 1 || Wave == 1) 
		{
			SoundtrackText.text = audioControllerScript.TrackName + "";
		}
	}

	public void StartNewWave ()
	{
		if (IsInvoking ("IncrementWaveNumber") == false) 
		{
			Invoke ("IncrementWaveNumber", 0);
		}

		StartCoroutine (GoToNextWave ());
	}

	void IncrementWaveNumber ()
	{
		Wave += 1;
		UnityEngine.Debug.Log ("Wave number increased to " + Wave + ".");
	}

	public IEnumerator GoToNextWave ()
	{
		yield return new WaitForSecondsRealtime (5);
		NextLevel ();

		if (Wave % 4 == 1) 
		{
			audioControllerScript.NextTrack ();
			audioControllerScript.LoadTracks ();
			UnityEngine.Debug.Log ("New soundtrack loaded.");
			UnityEngine.Debug.Log ("Soundtrack: " + audioControllerScript.TrackName);
		}

		if (gameModifier.BossSpawn != GameModifierManager.bossSpawnMode.BossesOnly) 
		{
			StartCoroutine (StartBlockSpawn ());
		}

		if (gameModifier.BossSpawn == GameModifierManager.bossSpawnMode.BossesOnly)
		{
			if (Wave % 4 != 0)
			{
				if (IsInvoking ("SpawnMiniBossObject") == false)
				{
					Invoke ("SpawnMiniBossObject", BossSpawnDelay);
					yield return null;
				}
			}

			if (Wave % 4 == 0) 
			{
				if (IsInvoking ("SpawnBigBossObject") == false) 
				{
					Invoke ("SpawnBigBossObject", BossSpawnDelay);
					yield return null;
				}
			}
		}

		StopCoroutine (GoToNextWave ());
	}

	public void SetGameModifiers ()
	{
		if (gameModifier.Tutorial == false) 
		{
			playerControllerScript_P1.tutorialManagerScript.TurnOffTutorial ();
		}

		switch (gameModifier.PowerupSpawn) 
		{
		case GameModifierManager.powerupSpawnMode.Normal:
			powerupPickupSpawnModifier = 1;
			break;
		case GameModifierManager.powerupSpawnMode.Faster:
			powerupPickupSpawnModifier = 0.4f;
			break;
		case GameModifierManager.powerupSpawnMode.Slower:
			powerupPickupSpawnModifier = 5f;
			break;
		case GameModifierManager.powerupSpawnMode.Off:
			powerupPickupSpawnModifier = Mathf.Infinity;
			break;
		}
			
		if (playerControllerScript_P1.isHoming == true)
		{
			HomingImage.enabled = true;
		}

		if (playerControllerScript_P1.isRicochet == true)
		{
			RicochetImage.enabled = true;
		}

		if (playerControllerScript_P1.isInRapidFire == true) 
		{
			playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.TripleShotFireRates [1];
			RapidfireImage.enabled = true;
		}

		if (playerControllerScript_P1.isInOverdrive == true)
		{
			OverdriveImage.enabled = true;
		}

		Lives = gameModifier.StartingLives;
		playerControllerScript_P1.isHoming = gameModifier.AlwaysHoming;
		playerControllerScript_P1.isRicochet = gameModifier.AlwaysRicochet;
		playerControllerScript_P1.isInRapidFire = gameModifier.AlwaysRapidfire;
		playerControllerScript_P1.isInOverdrive = gameModifier.AlwaysOverdrive;
		StackingObject.SetActive (gameModifier.stacking);
	}
}
