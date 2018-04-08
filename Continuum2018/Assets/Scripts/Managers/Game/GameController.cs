using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI; // Access to Unity's UI system.
using TMPro; // Access to TextMeshPro components.
using UnityEngine.PostProcessing; // Allows access to Unity's Post Processing Stack.
using InControl;

public class GameController : MonoBehaviour 
{
	public PlayerController playerControllerScript_P1;		// Reference to the player controller.
	public TimescaleController timescaleControllerScript;	// Reference to the timescale controller.
	public AudioController audioControllerScript;			// Reference to the audio controller.
	public DeveloperMode developerModeScript;				// Reference to the developer mode for debug info.
	public CursorManager cursorManagerScript;				// Reference to the cursor state.
	public PostProcessingProfile ImageEffects;				// Reference to the post processing profile.
	public GameModifierManager gameModifier;				// Reference to the GameModifierManager Scriptable object.								// Main camera.
	public static List<InputDevice> playerDevices; 

	[Header ("Game Stats")]
	[Tooltip ("Track debug info.")]
	public bool TrackStats = true;
	[Tooltip ("Scaled time elapsed since the start of the first wave.")]
	public float GameTime;
	[Tooltip ("Unscaled time elapsed since the start of the first wave.")]
	public float RealTime;
	[Tooltip ("GameTime / RealTime, can be greater than 1.")]
	public float TimeRatio;
	[Tooltip ("Distance between player and reference point.")]
	public float Distance;
	[Tooltip ("Counter for how many blocks were destroyed.")]
	public int BlocksDestroyed;
	[Tooltip ("Counter for bullets instantiated.")]
	public int BulletsShot;
	[Tooltip ("Blocks destroyed / Bullets shot.")]
	public float BlockShotAccuracy;

	[Header ("Waves")]
	[Tooltip ("Current wave number.")]
	public int Wave;
	[Tooltip ("Wave number text.")]
	public TextMeshProUGUI WaveText;
	[Tooltip ("How much longer each wave duration increases per wave.")]
	public float WaveTimeIncreaseRate;
	[Tooltip ("How long the first wave should last.")]
	public float FirstWaveTimeDuration;
	[Tooltip (" Max wave duration time.")]
	public float WaveTimeDuration;
	[Tooltip ("Wave time remaining until end of wave.")]
	public float WaveTimeRemaining;
	[Tooltip ("Background of wave text.")]
	public RawImage WaveBackground;

	[Header ("Wave Transition")]
	[Tooltip ("Allows wave transition animation.")]
	public bool IsInWaveTransition;
	[Tooltip ("Cool wave transition particle effect.")]
	public ParticleSystem WaveTransitionParticles;
	[Tooltip ("Animator for wave transition.")]
	public Animator WaveTransitionAnim;
	[Tooltip ("Text for wave transition to show wave number.")]
	public TextMeshProUGUI WaveTransitionText;
	[Tooltip ("Warp sound.")]
	public AudioSource WaveTransitionAudio;

	// This is for wave transition UI only.
	[Tooltip ("New soundtrack text. (Appears every four waves).")]
	public TextMeshProUGUI SoundtrackText;
	[Tooltip ("Display for game time in wave transition.")]
	public TextMeshProUGUI GameTimeText;
	[Tooltip ("Display for real time in wave transition.")]
	public TextMeshProUGUI RealTimeText;
	[Tooltip ("Display for time ratio in wave transition.")]
	public TextMeshProUGUI TimeRatioText;
	[Tooltip ("Display for blocks destroyed in wave transition.")]
	public TextMeshProUGUI BlocksDestroyedText;
	[Tooltip ("Display for bullets shot in wave transition.")]
	public TextMeshProUGUI BulletsShotText;
	[Tooltip ("Display for accuracy in wave transition.")]
	public TextMeshProUGUI AccuracyText;

	[Header ("Scoring")]
	[Tooltip ("Allow score to be incremented.")]
	public bool CountScore;
	[Tooltip ("Currently displayed score.")]
	public float DisplayScore;
	[Tooltip ("Current score updated.")]
	public float CurrentScore;
	[Tooltip ("Score to smoothly transition towards.")]
	public float TargetScore;
	[Tooltip ("How much smoothing is applied to target score.")]
	public float ScoreSmoothing;
	[Tooltip ("Score UI text.")]
	public TextMeshProUGUI ScoreText;
	[Tooltip ("Dark background for score area.")]
	public RawImage ScoreBackground;

	[Header ("Lives")]
	[Tooltip ("Counter for lives remaining (UI displays one less as one life is the player itself).")]
	public int Lives;
	[Tooltip ("Maximum lives the player can have at any one time.")]
	public int MaxLives = 11;
	[Tooltip ("UI counter for lives.")]
	public TextMeshProUGUI LivesText;
	[Tooltip ("Life images x3.")]
	public RawImage[] LifeImages;
	[Tooltip ("Dark background for Lives.")]
	public RawImage LivesBackground;
	[Tooltip ("Lives spacing determined by amount of lives.")]
	public GameObject LivesSpacing;
	[Tooltip ("When lives count reaches max lives, display this text.")]
	public TextMeshProUGUI MaxLivesText;

	[Header ("Combo")]
	[Tooltip ("Current combo.")]
	public int pointsThisCombo;
	public TextMeshProUGUI PointsThisComboText;
	public Animator PointsThisComboAnim;
	public int combo = 1;
	[Tooltip ("How long the combo lasts before decrementing.")]
	public float comboDuration = 0.5f;
	[Tooltip ("Current time left of the current combo before it decremements.")]
	public float comboTimeRemaining;

	[Header ("Stacking")]
	[Tooltip ("Stack zones parent object.")]
	public GameObject StackingObject;

	[Header ("Block Spawner")]
	[Tooltip ("Block prefabs to spawn based on wave number.")]
	public GameObject[] Blocks;
	[Tooltip ("How often to spawn blocks.")]
	public float BlockSpawnRate;
	[Tooltip ("Time to elapse before next block spawn. Time.time > this.")]
	private float NextBlockSpawn;
	[Tooltip ("Start delay for blocks to spawn.")]
	public float blockSpawnStartDelay;
	[Tooltip ("Fixed x positions to line up with stack zones.")]
	public float[] BlockSpawnXPositions;
	[Tooltip ("Starting height of block spawn y position.")]
	public float BlockSpawnYPosition;
	[Tooltip ("Block spawn depth position.")]
	public float BlockSpawnZPosition;
	[Tooltip ("How much faster blocks spawn every wave.")]
	public float BlockSpawnIncreaseRate;
	[Tooltip ("The maximum points any block can have awarded.")]
	public int MaximumBlockPoints = 10000;

	[Header ("Powerup General")]
	[Tooltip ("Current powerup time left.")]
	public float PowerupTimeRemaining;
	[Tooltip ("Maximum powerup time.")]
	public float PowerupTimeDuration;
	[Tooltip ("Powerup animator for background light.")]
	public Animator PowerupAnim;
	[Tooltip ("Sound to play when powerup time < 3 seconds.")]
	public AudioSource PowerupTimeRunningOutAudio; 
	[Tooltip ("Sound to play when powerups reset.")]
	public AudioSource PowerupResetAudio;
	[Tooltip ("List of powerup pickups to spawn.")]
	public GameObject[] PowerupPickups;	
	[Tooltip ("How long the next powerup pickup will spawn.")]
	public float powerupPickupTimeRemaining;
	[Tooltip ("Range of time powerup pickups are spawned.")]
	public Vector2 PowerupPickupSpawnRate;
	[Tooltip ("Gets value from game modifier for multiplier.")]
	public float powerupPickupSpawnModifier = 1;
	[Tooltip ("Time to pass before next powerup spawns.")]
	private float NextPowerupPickupSpawn;
	[Tooltip ("Horizontal area to spawn powerup pickups.")]
	public float PowerupPickupSpawnRangeX;
	[Tooltip ("Vertical area to spawn powerup pickups.")]
	public float PowerupPickupSpawnY;

	[Header ("Powerup List UI")]
	[Tooltip ("Powerup slot index.")]
	public int NextPowerupSlot_P1;
	[Tooltip ("Powerup slot index.")]
	public int NextPowerupShootingSlot_P1;	
	[Tooltip ("Array of powrup images for each slot. ")]
	public RawImage[] PowerupImage_P1;
	[Tooltip ("Texture for default single standard shot.")]
	public Texture2D StandardShotTexture; 
	[Tooltip ("Image for homing.")]
	public RawImage HomingImage;
	[Tooltip ("Hex background for homing.")]
	public RawImage HomingHex;
	[Tooltip ("Image for ricochet.")]
	public RawImage RicochetImage;	
	[Tooltip ("Hex background for ricochet.")]
	public RawImage RicochetHex;
	[Tooltip ("Image for rapidfire.")]
	public RawImage RapidfireImage;	
	[Tooltip ("Hex background for rapidfire.")]
	public RawImage RapidfireHex;
	[Tooltip ("Image for overdrive.")]
	public RawImage OverdriveImage;
	[Tooltip ("Hex background for overdrive.")]
	public RawImage OverdriveHex;
	[Tooltip ("For other scripts to reference to spawn pickup UI.")]
	public GameObject PowerupPickupUI;

	[Header ("Bosses")]
	[Tooltip ("Minimum amount of time for a boss to spawn.")]
	public float BossSpawnDelay = 5;
	[Tooltip ("List of minibosses to spawn at random.")]
	public GameObject[] MiniBosses;
	[Tooltip ("Where to spawn the minibosses.")]
	public Transform MiniBossSpawnPos;
	[Tooltip ("Start delay to spawn a boss after the wave ends.")]
	public float MiniBossSpawnDelay = 4;
	[Tooltip ("Array of big bosses to spawn.")]
	public GameObject[] BigBosses;
	[Tooltip ("Where to spawn the spawned big boss.")]
	public Transform BigBossSpawnPos;
	[Tooltip ("How long the delay is from the end of the wave to the big boss spawning.")]
	public float BigBossSpawnDelay = 4; 
	[Tooltip ("Boss ID for the index in the big bosses array.")]
	public int bossId;

	[Header ("Pausing")]
	[Tooltip ("Checks whether game state is paused.")]
	public bool isPaused;
	[Tooltip ("How much time needs to pass before the player can pause and unpause again.")]
	public float PauseCooldown = 1;
	[HideInInspector] 
	public float NextPauseCooldown;
	[Tooltip ("Allow pausing or not.")]
	public bool canPause = true;
	[Tooltip ("UI for pause menu.")]
	public GameObject PauseUI;
	[Tooltip ("Is the menu in the first pause menu or a sub menu?")]
	public bool isInOtherMenu;

	[Header ("Game Over")]
	[Tooltip ("Did the player run out of lives?")]
	public bool isGameOver;
	[Tooltip ("UI Game Over screen.")]
	public GameObject GameoverUI;

	[Header ("Camera")]
	[Tooltip ("Main Camera GameObject.")]
	public Camera MainCamera;
	[Tooltip ("Current orthographic size of the camera.")]
	public float OrthSize = 10;
	[Tooltip ("Starting orthographic size of the camera.")]
	public float StartOrthSize = 10;
	private float OrthSizeVel;
	[Tooltip ("Smoothing amount for orthographic change.")]
	public float OrthSizeSmoothTime = 1;

	[Header ("Visuals")]
	[Tooltip ("Allows updating image effects values.")]
	public bool isUpdatingImageEffects = true;
	[Tooltip ("Allows updating particle effect values.")]
	public bool isUpdatingParticleEffects = true;
	[Tooltip ("For depth of field.")]
	public float TargetDepthDistance;

	[Header ("Star field")]
	[Tooltip ("Starfield foreground.")]
	public ParticleSystem StarFieldForeground;
	[Tooltip ("Starfield background.")]
	public ParticleSystem StarFieldBackground;
	[Tooltip ("Lifetime multiplier.")]
	public float StarFieldForegroundLifetimeMultipler = 0.1f;
	[Tooltip ("How fast the particle simulation is.")]
	public float StarFieldForegroundSimulationSpeed = 1;
	[Tooltip ("Lifetime multiplier.")]
	public float StarFieldBackgroundLifetimeMultipler = 0.1f;
	[Tooltip ("How fast the particle simulation is.")]
	public float StarFieldBackgroundSimulationSpeed = 1;

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
	public TextMeshProUGUI PointsThisComboText_Debug;
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
	public TextMeshProUGUI Beats_Debug;
	public TextMeshProUGUI BeatInBar_Debug;
	public TextMeshProUGUI BeatsPerMinute_Debug;
	public TextMeshProUGUI TimeSinceTrackLoad_Debug;
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
	public TextMeshProUGUI Modifier_Stacking_Debug;
	public TextMeshProUGUI Modifier_UseOverheat_Debug;
	[Space (10)]
	// Other modifiers.
	public TextMeshProUGUI OverheatTimeText_Debug;
	public TextMeshProUGUI OverheatStateText_Debug;
	public TextMeshProUGUI CameraOrthographicSizeText_Debug;
	public TextMeshProUGUI PlayerMovementVector_Debug;
	public TextMeshProUGUI CurrentShootingHeatCostText_Debug;
	public TextMeshProUGUI LastImpactPointText_Debug;
	public TextMeshProUGUI CooldownTimeRemainingText_Debug;
	public TextMeshProUGUI CurrentAbilityStateText_Debug;

	void Awake () 
	{
		// Hide and lock the mouse.
		cursorManagerScript.VisibleTimerRemain = 0;
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

		PowerupImage_P1 [0].texture = StandardShotTexture;
			
		// Reset all powerup textures in list.
		foreach (RawImage powerupImage in PowerupImage_P1) 
		{
			if (powerupImage != PowerupImage_P1 [0]) 
			{
				powerupImage.gameObject.SetActive (false);
			}

			powerupImage.texture = null;
			powerupImage.color = new Color (0, 0, 0, 0);
			playerControllerScript_P1.CheckPowerupImageUI ();
		}

		// Reset inital powerup slot index.
		NextPowerupSlot_P1 = 1;
		NextPowerupShootingSlot_P1 = 0;
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

		powerupPickupTimeRemaining = 10;
	}

	// Timescale controller calls this initially after the countdown.
	public void StartGame ()
	{
		ResetScore (); // Resets score display and value.

		TrackStats = true;

		// Set wave transition in motion.
		PlayWaveTransitionVisuals ();
		WaveTimeRemaining = WaveTimeDuration;

		playerControllerScript_P1.AbilityUIHexes.Play ("HexesFadeIn"); // Fade in hexes.
		bossId = 0; // Reset boss ID.

		CheckBossSpawnMode (); // Checks mode from game modifier to set boss spawn mode.

		// Allow score animators and UI.
		playerControllerScript_P1.ScoreAnim.enabled = true;
		playerControllerScript_P1.LivesAnim.gameObject.SetActive (true);
		playerControllerScript_P1.LivesAnim.enabled = true;
		playerControllerScript_P1.WaveAnim.enabled = true;

		ScoreBackground.enabled = true;
		LivesBackground.enabled = true;
		WaveBackground.enabled = true;

		// Step down block spawn rate.
		BlockSpawnRate -= (BlockSpawnIncreaseRate * gameModifier.blockSpawnRateMultiplier);
		UnityEngine.Debug.Log ("Block spawn rate: " + BlockSpawnRate);

		// Set starting lives.
		Lives = gameModifier.StartingLives;
	}

	// Checks mode from game modifier to set boss spawn mode.
	void CheckBossSpawnMode ()
	{
		// Mode for normal and no bosses.
		if (gameModifier.BossSpawn != GameModifierManager.bossSpawnMode.BossesOnly)
		{
			StartCoroutine (StartBlockSpawn ());
		}

		// Mode for bosses only.
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
	}

	void Update ()
	{
		// Update game and time stats.
		UpdateGameStats ();
		UpdateTimeStats ();

		CheckCombo (); 						// Check current combo amount and timer.
		CheckPowerupTime (); 				// Check current powerup time and timer.
		LevelTimer (); 						// Timer for wave time.
		CheckWaveTime (); 					// Stuff to do when the wave time > 0.
		UpdateScoreIncrements (); 			// Refreshes score.
		UpdateStarFieldParticleEffects ();  // Updates particle effects.
	
		PowerupSpawner ();					// Timer for powerup spawning.
		CheckResetCombo ();
	}

	void CheckResetCombo ()
	{
		if (combo == 1) 
		{
			if (pointsThisCombo != 0) 
			{
				pointsThisCombo = 0;
				PointsThisComboAnim.SetTrigger ("PointsComboFade");
			}
		}
	}

	// Updates the background particles.
	void UpdateStarFieldParticleEffects ()
	{
		if (isUpdatingParticleEffects == true) 
		{
			var StarFieldForegroundTrailModule = StarFieldForeground.trails;
			StarFieldForegroundTrailModule.lifetime = new ParticleSystem.MinMaxCurve (
				0, StarFieldForegroundLifetimeMultipler * Time.timeScale
			);

			var StarFieldForegroundMainModule = StarFieldForeground.main;
			StarFieldForegroundMainModule.simulationSpeed = StarFieldForegroundSimulationSpeed * Time.timeScale;
			StarFieldForegroundMainModule.startSize = new ParticleSystem.MinMaxCurve (0.01f, 0.02f * Time.timeScale);

			var StarFieldForegroundRenderer = StarFieldForeground.GetComponent<ParticleSystemRenderer> ();
			//StarFieldForegroundRenderer.renderMode = ParticleSystemRenderMode.Stretch;
			StarFieldForegroundRenderer.velocityScale = 0.1f * Time.timeScale;


			var StarFieldForegroundEmissionModule = StarFieldForeground.emission;
			StarFieldForegroundEmissionModule.rateOverTime = Time.timeScale * 10;

			/*var StarFieldBackgroundTrailModule = StarFieldBackground.trails;
			StarFieldBackgroundTrailModule.lifetime = new ParticleSystem.MinMaxCurve (
				0, StarFieldBackgroundLifetimeMultipler * Time.timeScale
			);*/

			var StarFieldBackgroundMainModule = StarFieldBackground.main;
			StarFieldBackgroundMainModule.simulationSpeed = StarFieldBackgroundSimulationSpeed * Time.timeScale;
			StarFieldBackgroundMainModule.startSize = new ParticleSystem.MinMaxCurve (0.01f, 0.02f * Time.timeScale);

			var StarFieldBackgroundEmissionModule = StarFieldBackground.emission;
			StarFieldBackgroundEmissionModule.rateOverTime = Time.timeScale * 40;

			var StarFieldBackgroundRenderer = StarFieldBackground.GetComponent<ParticleSystemRenderer> ();
			//StarFieldBackgroundRenderer.renderMode = ParticleSystemRenderMode.Stretch;
			StarFieldBackgroundRenderer.velocityScale = 0.1f * Time.timeScale;

		}
	}

	// Updates debug values and gets current values for the current game state.
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
				PointsThisComboText_Debug.text = 
					"Points this combo: " + pointsThisCombo;
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
					"P1 Ability Fill: " + Math.Round (playerControllerScript_P1.AbilityTimeAmountProportion, 6);
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
				BlockShotAccuracy = 
					Mathf.Clamp (BlocksDestroyed / (BulletsShot + Mathf.Epsilon), 0, 10000);
				BlockShotAccuracyText_Debug.text = 
					"Block Shot Accuracy: " + (System.Math.Round((BlockShotAccuracy * 100), 2)) + "%";
				RewindTimeRemainingText_Debug.text = 
					"Rewind Time Remain: " + System.Math.Round(timescaleControllerScript.RewindTimeRemaining, 2);
				IsRewindingText_Debug.text = 
					"Is Rewinding: " + (timescaleControllerScript.isRewinding ? "ON" : "OFF");
				

				// Modifier debug values.
				Modifier_Tutorial_Debug.text = 
					"Use Tutorial: " + (gameModifier.Tutorial ? "ON" : "OFF");
				Modifier_PowerupSpawn_Debug.text = 
					"Powerup Spawn Mode: " + (gameModifier.PowerupSpawn.ToString ());
				Modifier_BossSpawn_Debug.text = 
					"Boss Spawn Mode: " + (gameModifier.BossSpawn.ToString ());
				Modifier_TimeIncrease_Debug.text = 
					"Time Increase Mode: " + (gameModifier.TimeIncreaseMode.ToString ());
				Modifier_StartingLives_Debug.text = 
					"Starting Lives: " + (gameModifier.StartingLives);
				Modifier_AlwaysHoming_Debug.text = 
					"Always Homing: " + (gameModifier.AlwaysHoming ? "ON" : "OFF");
				Modifier_AlwaysRicochet_Debug.text = 
					"Always Ricochet: " + (gameModifier.AlwaysRicochet ? "ON" : "OFF");
				Modifier_AlwaysRapidfire_Debug.text = 
					"Always rapidfire: " + (gameModifier.AlwaysRapidfire ? "ON" : "OFF");
				Modifier_AlwaysOverdrive_Debug.text = 
					"Always overdrive: " + (gameModifier.AlwaysOverdrive ? "ON" : "OFF");
				Modifier_TrialTime_Debug.text = 
					"Trial Time: " + (gameModifier.TrialTime);
				Modifier_BlockSpawnRateMult_Debug.text = 
					"Block Spawn Rate Mult: " + gameModifier.blockSpawnRateMultiplier;
				Modifier_Stacking_Debug.text = 
					"Stacking: " + (gameModifier.stacking ? "ON" : "OFF");
				Modifier_UseOverheat_Debug.text = 
					"Use Overheat: " + (gameModifier.useOverheat ? "ON" : "OFF");

				OverheatTimeText_Debug.text = 
					"Overheat proportion: " + System.Math.Round (playerControllerScript_P1.CurrentShootingHeat, 2);
				OverheatStateText_Debug.text = 
					"Overheated: " + (playerControllerScript_P1.Overheated ? "ON" : "OFF");
				CameraOrthographicSizeText_Debug.text = 
					"Orthographic size: " + System.Math.Round (Camera.main.orthographicSize, 2);
				PlayerMovementVector_Debug.text = 
					"Player move input: " + new Vector2 (playerControllerScript_P1.MovementX, playerControllerScript_P1.MovementY);
				CurrentShootingHeatCostText_Debug.text = 
					"Current shooting heat cost: " + playerControllerScript_P1.CurrentShootingHeatCost;
				LastImpactPointText_Debug.text = 
					"Last impact point: " + playerControllerScript_P1.ImpactPoint.ToString ();
				CooldownTimeRemainingText_Debug.text = 
					"Cooldown time remain: " + System.Math.Round (playerControllerScript_P1.cooldownTimeRemaining, 2);
				CurrentAbilityStateText_Debug.text = 
					"P1 Ability State: " + playerControllerScript_P1.CurrentAbilityState.ToString ();

				Beats_Debug.text = 
					"Beats: " + audioControllerScript.Beats;
				BeatInBar_Debug.text = 
					"Beat in bar: " + audioControllerScript.BeatInBar;
				BeatsPerMinute_Debug.text = 
					"Beats per minute: " + System.Math.Round (audioControllerScript.BeatsPerMinute, 3);
				TimeSinceTrackLoad_Debug.text = 
					"Time since last track load: " + System.Math.Round (audioControllerScript.TimeSinceTrackLoad, 2);
			}
		}
	}

	// Updates score and represents it as a string to the score text.
	void UpdateScoreIncrements ()
	{
		if (CountScore == true) 
		{
			// Adds score over time.
			//TargetScore += ScoreRate * Time.deltaTime * ScoreMult * Time.timeScale;

			// Smooths the current score to the displayed score.
			CurrentScore = Mathf.Lerp (CurrentScore, TargetScore, ScoreSmoothing * Time.unscaledDeltaTime);
			DisplayScore = Mathf.Floor (CurrentScore); // Rounds down to nearest integer. 

			// Formats score text string based on value.
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

	// Refreshes lives UI and amount by repeated invoke.
	public void UpdateLives ()
	{
		// When in first wave transition.
		if (timescaleControllerScript.isInInitialSequence == true || timescaleControllerScript.isInInitialCountdownSequence == true) 
		{
			if (LivesText.gameObject.activeSelf == true) 
			{
				LifeImages [0].enabled = false;
				LifeImages [1].enabled = false;
				LifeImages [2].enabled = false;
				LifeImages [3].enabled = false;
				LifeImages [4].enabled = false;
				LifeImages [5].enabled = false;
				LifeImages [6].enabled = false;
				LifeImages [7].enabled = false;
				LifeImages [8].enabled = false;
				LifeImages [9].enabled = false;
				LivesText.gameObject.SetActive (false);
				LivesText.text = "";
			}
		}

		// For every wave after that.
		if (timescaleControllerScript.isInInitialSequence == false && 
			timescaleControllerScript.isInInitialCountdownSequence == false) 
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
					LifeImages [3].enabled = false;
					LifeImages [4].enabled = false;
					LifeImages [5].enabled = false;
					LifeImages [6].enabled = false;
					LifeImages [7].enabled = false;
					LifeImages [8].enabled = false;
					LifeImages [9].enabled = false;
				}
				break;
			case 1:
				LifeImages [0].enabled = false;
				LifeImages [1].enabled = false;
				LifeImages [2].enabled = false;
				LifeImages [3].enabled = false;
				LifeImages [4].enabled = false;
				LifeImages [5].enabled = false;
				LifeImages [6].enabled = false;
				LifeImages [7].enabled = false;
				LifeImages [8].enabled = false;
				LifeImages [9].enabled = false;
				break;
			case 2:
				LifeImages [0].enabled = true;
				LifeImages [1].enabled = false;
				LifeImages [2].enabled = false;
				LifeImages [3].enabled = false;
				LifeImages [4].enabled = false;
				LifeImages [5].enabled = false;
				LifeImages [6].enabled = false;
				LifeImages [7].enabled = false;
				LifeImages [8].enabled = false;
				LifeImages [9].enabled = false;
				break;
			case 3:
				LifeImages [0].enabled = true;
				LifeImages [1].enabled = true;
				LifeImages [2].enabled = false;
				LifeImages [3].enabled = false;
				LifeImages [4].enabled = false;
				LifeImages [5].enabled = false;
				LifeImages [6].enabled = false;
				LifeImages [7].enabled = false;
				LifeImages [8].enabled = false;
				LifeImages [9].enabled = false;
				break;
			case 4:
				LifeImages [0].enabled = true;
				LifeImages [1].enabled = true;
				LifeImages [2].enabled = true;
				LifeImages [3].enabled = false;
				LifeImages [4].enabled = false;
				LifeImages [5].enabled = false;
				LifeImages [6].enabled = false;
				LifeImages [7].enabled = false;
				LifeImages [8].enabled = false;
				LifeImages [9].enabled = false;
				break;
			case 5:
				LifeImages [0].enabled = true;
				LifeImages [1].enabled = true;
				LifeImages [2].enabled = true;
				LifeImages [3].enabled = true;
				LifeImages [4].enabled = false;
				LifeImages [5].enabled = false;
				LifeImages [6].enabled = false;
				LifeImages [7].enabled = false;
				LifeImages [8].enabled = false;
				LifeImages [9].enabled = false;
				break;
			case 6:
				LifeImages [0].enabled = true;
				LifeImages [1].enabled = true;
				LifeImages [2].enabled = true;
				LifeImages [3].enabled = true;
				LifeImages [4].enabled = true;
				LifeImages [5].enabled = false;
				LifeImages [6].enabled = false;
				LifeImages [7].enabled = false;
				LifeImages [8].enabled = false;
				LifeImages [9].enabled = false;
				break;
			case 7:
				LifeImages [0].enabled = true;
				LifeImages [1].enabled = true;
				LifeImages [2].enabled = true;
				LifeImages [3].enabled = true;
				LifeImages [4].enabled = true;
				LifeImages [5].enabled = true;
				LifeImages [6].enabled = false;
				LifeImages [7].enabled = false;
				LifeImages [8].enabled = false;
				LifeImages [9].enabled = false;
				break;
			case 8:
				LifeImages [0].enabled = true;
				LifeImages [1].enabled = true;
				LifeImages [2].enabled = true;
				LifeImages [3].enabled = true;
				LifeImages [4].enabled = true;
				LifeImages [5].enabled = true;
				LifeImages [6].enabled = true;
				LifeImages [7].enabled = false;
				LifeImages [8].enabled = false;
				LifeImages [9].enabled = false;
				break;
			case 9:
				LifeImages [0].enabled = true;
				LifeImages [1].enabled = true;
				LifeImages [2].enabled = true;
				LifeImages [3].enabled = true;
				LifeImages [4].enabled = true;
				LifeImages [5].enabled = true;
				LifeImages [6].enabled = true;
				LifeImages [7].enabled = true;
				LifeImages [8].enabled = false;
				LifeImages [9].enabled = false;
				break;
			case 10:
				LifeImages [0].enabled = true;
				LifeImages [1].enabled = true;
				LifeImages [2].enabled = true;
				LifeImages [3].enabled = true;
				LifeImages [4].enabled = true;
				LifeImages [5].enabled = true;
				LifeImages [6].enabled = true;
				LifeImages [7].enabled = true;
				LifeImages [8].enabled = true;
				LifeImages [9].enabled = false;
				break;
			case 11:
				LifeImages [0].enabled = true;
				LifeImages [1].enabled = true;
				LifeImages [2].enabled = true;
				LifeImages [3].enabled = true;
				LifeImages [4].enabled = true;
				LifeImages [5].enabled = true;
				LifeImages [6].enabled = true;
				LifeImages [7].enabled = true;
				LifeImages [8].enabled = true;
				LifeImages [9].enabled = true;
				break;
			}

			// Only show one life icon and show numerical text next to it.
			if (Lives >= MaxLives) 
			{
				LifeImages [0].enabled = true;
				LifeImages [1].enabled = false;
				LifeImages [2].enabled = false;
				LifeImages [3].enabled = false;
				LifeImages [4].enabled = false; 
				LifeImages [5].enabled = false;
				LifeImages [6].enabled = false;
				LifeImages [7].enabled = false;
				LifeImages [8].enabled = false;
				LifeImages [9].enabled = false;
				LivesSpacing.SetActive (true);
				LivesText.gameObject.SetActive (true);
				LivesText.text = "x " + (Lives - 1);
				MaxLivesText.text = "MAX";
			}

			if (Lives < MaxLives) 
			{
				LivesSpacing.SetActive (false);
				LivesText.gameObject.SetActive (false);
				LivesText.text = "";
				MaxLivesText.text = "";
			}
		}
	}

	// Combo time remaining variable is timed and decreases based on what combo it is already on.
	void CheckCombo ()
	{
		// The higher the combo, the faster the combo timer decreases.
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

	// Timer for powerups.
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

		// Reset all powrups when timer runs out.
		if (PowerupTimeRemaining < 0) 
		{
			playerControllerScript_P1.powerupsInUse = 0;
			PowerupTimeRemaining = 0;
			PowerupAnim.StopPlayback ();
			PowerupResetAudio.Play ();
			playerControllerScript_P1.ResetPowerups ();
		}
	}

	// Other scripts call this to set powerup time duration.
	public void SetPowerupTime (float Duration)
	{
		PowerupTimeRemaining += Duration;
		PowerupTimeRemaining = Mathf.Clamp (PowerupTimeRemaining, 0, PowerupTimeDuration);
	}

	// Update time stats for wave transitions and debug menu.
	void UpdateTimeStats ()
	{
		if (isPaused == false && TrackStats == true) 
		{
			Distance = timescaleControllerScript.Distance; // Gets distance from Time Scale Controller.
			GameTime += Time.deltaTime; // Increases time scaled.
			RealTime += Time.unscaledDeltaTime; // Increases time unscaled.

			// Sets time ration based on scaled/unscaled time.
			// Ratio > 1, player is performing above average.
			// Ratio == 1, player is average.
			// Ratio < 1. player is below average
			TimeRatio = GameTime / RealTime; 

			// Updates game time text.
			GameTimeText.text = "GAME TIME: " + 
				(GameTime / 60 / 60).ToString ("00") + " " + // To hours. 
				(GameTime/60).ToString("00") + "\' " + // To minutes.
				(GameTime % 60).ToString("00") + "\""; // To seconds.

			// Updates real time text.
			RealTimeText.text = "REAL TIME: " + 
				(RealTime / 60 / 60).ToString ("00") + " " + // To hours. 
				(RealTime/60).ToString("00") + "\' " + // To minutes.
				(RealTime % 60).ToString("00") + "\""; // To seconds.

			// Updates time ratio text.
			TimeRatioText.text = "TIME RATIO: " + System.Math.Round((GameTime / RealTime), 2).ToString ("0.00") + "";

			// Updates blocks destroyed text.
			BlocksDestroyedText.text = "BLOCKS DESTROYED: " + BlocksDestroyed;

			// Updates bullets shot text.
			BulletsShotText.text = "BULLETS SHOT: " + BulletsShot;

			// Updates accuracy text.
			// Accuracy can be > 100%.
			AccuracyText.text = "ACCURACY: " + System.Math.Round(BlockShotAccuracy * 100, 2) + "%";
		}
	}

	// Resets all score values.
	public void ResetScore ()
	{
		TargetScore = 0;
		CurrentScore = 0;
		DisplayScore = 0;
		ScoreText.text = "" + DisplayScore;
	}

	// Updates image effects (noise, bloom, etc).
	void UpdateImageEffects ()
	{
		/*
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
		}*/

	}

	// Tracks pause state.
	public void CheckPause ()
	{
		if (isInOtherMenu == false)
		{
			isPaused = !isPaused; // Toggles pausing.

			// Stop updating required scripts.
			if (isPaused) 
			{
				PauseUI.SetActive (true);

				playerControllerScript_P1.pauseManagerScript.MainPauseMenuOnEnter (0);
				playerControllerScript_P1.pauseManagerScript.menuButtons.buttonIndex = 0;

				// Sets mouse cursor states.
				//cursorManagerScript.UnlockMouse ();
				//cursorManagerScript.ShowMouse ();

				// Sets audio values for pause.
				audioControllerScript.updateVolumeAndPitches = false;
				//audioControllerScript.BassTrack.pitch = 0;
				audioControllerScript.SetTargetLowPassFreq (500);
				audioControllerScript.SetTargetResonance (2f);

				// Stops counting score.
				CountScore = false;

				// Overrides time scale.
				timescaleControllerScript.isOverridingTimeScale = true;
				timescaleControllerScript.OverridingTimeScale = 0;
				timescaleControllerScript.OverrideTimeScaleTimeRemaining = 0.1f;
			}

			// Restart updating required scripts.
			if (!isPaused) 
			{
				UnPauseGame ();
			}

			NextPauseCooldown = Time.unscaledTime + PauseCooldown; // Timer for next pause/unpause input to be read.
		}
	}

	// Prepares resuming the game.
	public void InvokeUnpause ()
	{
		if (isInOtherMenu == false) 
		{
			isPaused = false;
			UnPauseGame ();
			NextPauseCooldown = Time.unscaledTime + PauseCooldown; // Timer for next pause/unpause input to be read.
		}
	}

	// Unpauses the game.
	void UnPauseGame ()
	{
		// Allow player to move and shoot.
		playerControllerScript_P1.UsePlayerFollow = true;
		playerControllerScript_P1.canShoot = true;

		// Turn off the pause UI.
		PauseUI.SetActive (false);

		// Set mouse cursor states.
		cursorManagerScript.LockMouse ();
		cursorManagerScript.HideMouse ();

		// Set audio controller values to resumed state.
		audioControllerScript.updateVolumeAndPitches = true;
		audioControllerScript.BassTrack.pitch = 1;
		audioControllerScript.SetTargetLowPassFreq (22000);
		audioControllerScript.SetTargetResonance (1);

		// Allow counting score if not in initial transition.
		if (timescaleControllerScript.isInInitialSequence == false && 
			timescaleControllerScript.isInInitialCountdownSequence == false) 
		{
			CountScore = true;
		}

		// Stops overriding time scale.
		timescaleControllerScript.isOverridingTimeScale = false;
		timescaleControllerScript.OverrideTimeScaleTimeRemaining = 0;
	}

	// Tracks if not in main pause menu.
	public void SetPauseOtherMenu (bool otherMenu)
	{
		isInOtherMenu = otherMenu;
	}

	// Checks for screen's ratio, sets camera orthographic size based on it.
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

		// Updates lens ratio from lens script based on screen ratio.
		playerControllerScript_P1.lensScript.ratio = 1 / MainCamera.aspect;
	}

	// Wave time remaining timer.
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

	// Preparen for next wave.
	public void NextLevel ()
	{
		if (Wave > 1) 
		{
			BlockSpawnRate -= (BlockSpawnIncreaseRate * gameModifier.blockSpawnRateMultiplier); // Decrease block spawn time.
			UnityEngine.Debug.Log ("Block spawn rate: " + BlockSpawnRate);
		}

		// Update wave timer.
		WaveTimeDuration += WaveTimeIncreaseRate;
		WaveTimeRemaining = WaveTimeDuration;
		PlayWaveTransitionVisuals (); // Trigger wave transition.
		WaveTransitionText.text = "WAVE " + Wave;
		WaveTransitionAudio.Play ();

		// Shake the camera and vibrate the controller.
		playerControllerScript_P1.camShakeScript.ShakeCam (0.6f, 3.7f, 99);
		#if !PLATFORM_STANDALONE_OSX && !PLATFORM_ANDROID
		playerControllerScript_P1.Vibrate (0.6f, 0.6f, 3);
		#endif
	}

	// Plays cool particles when going to the next wave.
	void PlayWaveTransitionVisuals ()
	{
		WaveTransitionParticles.Play (true);
		WaveTransitionAnim.Play ("WaveTransition");
		IsInWaveTransition = true;
	}

	// Spawn blocks in the wave.
	IEnumerator StartBlockSpawn ()
	{
		yield return new WaitForSeconds (blockSpawnStartDelay); // Give an initial start delay in new wave.
		WaveText.text = "WAVE " + Wave; // Update wave text.

		//int StartXPosId = Random.Range (0, BlockSpawnXPositions.Length);
		//int NextXPosId = StartXPosId;

		while (WaveTimeRemaining > 0) // Wave time must be greater than 0 to keep spawning blocks.
		{
			if (Time.time > NextBlockSpawn && 
				playerControllerScript_P1.isInCooldownMode == false && 
				isPaused == false)
			{
				SpawnBlock (false); // Spawns a block based on wave number.

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

				NextBlockSpawn = Time.time + BlockSpawnRate; // Add time for when next block spawns.
			}
			yield return null;
		}
	}

	// Spawn block based on wave number.
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

	// Block spawn rate increases over time or by wave.
	void UpdateBlockSpawnTime ()
	{
		/*if (isPaused == false && Lives > 0 && WaveTimeRemaining > 0) 
		{
			// Can update the block spawn increasing over time.
			//BlockSpawnRate -= BlockSpawnIncreaseRate * Time.unscaledDeltaTime;
		}*/

		// Clamps the spawn rate to a min/max value so it doesnt cause performance issues.
		BlockSpawnRate = Mathf.Clamp (BlockSpawnRate, 0.0333f, 10);
	}

	// Timer for powerp spawning.
	void PowerupSpawner ()
	{
		if (isGameOver == false && 
			isPaused == false && 
			playerControllerScript_P1.tutorialManagerScript.tutorialComplete == true) 
		{
			// PowerupPickupTimeRemaining is scaled.
			powerupPickupTimeRemaining -= Time.deltaTime;

			if (powerupPickupTimeRemaining <= 0) 
			{
				SpawnPowerupPickup ();
			}
		}
	}

	// Spawn powerup at a random position on the screen and reset the timer with new values.
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

	// Give delay to spawn a mini boss, then spawn it.
	public IEnumerator SpawnMiniBoss ()
	{
		yield return new WaitForSeconds (MiniBossSpawnDelay);
		SpawnMiniBossObject ();
	}

	// Spawns a random mini boss from mini boss array. 
	public void SpawnMiniBossObject ()
	{
		GameObject MiniBoss = MiniBosses [UnityEngine.Random.Range (0, MiniBosses.Length)];
		Instantiate (MiniBoss, MiniBossSpawnPos.position, MiniBossSpawnPos.rotation);
		UnityEngine.Debug.Log ("Spawned a mini boss.");
	}

	// Give delay to spawn a big boss, then spawn it.
	public IEnumerator SpawnBigBoss ()
	{
		yield return new WaitForSeconds (BigBossSpawnDelay);
		SpawnBigBossObject ();

		// Increase boss spawn ID.
		if (bossId < BigBosses.Length) 
		{
			bossId += 1;
		}

		// Reset boss spawn ID if reached the end.
		if (bossId >= BigBosses.Length) 
		{
			bossId = 0;
		}
	}

	// Spawns a big boss from big boss array using spawn ID. 
	public void SpawnBigBossObject ()
	{
		//GameObject BigBoss = BigBosses [UnityEngine.Random.Range (0, bossId)];
		GameObject BigBoss = BigBosses [bossId];
		Instantiate (BigBoss, BigBossSpawnPos.position, BigBossSpawnPos.rotation);
		UnityEngine.Debug.Log ("Oh snap! We spawned a big boss!");
	}

	// Timer for when wave time runs out.
	void CheckWaveTime ()
	{
		// What happens when wave timer runs out.
		if (WaveTimeRemaining < 0) 
		{
			// Wave / 4 has remainders = normal wave.
			if (Wave % 4 != 0)
			{
				// Spawn a miniboss as usual in normal mode.
				if (gameModifier.BossSpawn == GameModifierManager.bossSpawnMode.Normal)
				{
					StartCoroutine (SpawnMiniBoss ());
				}

				// Go to next wave, skip bosses entirely.
				if (gameModifier.BossSpawn == GameModifierManager.bossSpawnMode.NoBosses) 
				{
					StartNewWave ();
					IsInWaveTransition = true;
				}

				// Wave after big boss wave, clear soundtrack text display.
				if (Wave % 4 != 1)
				{
					SoundtrackText.text = "";
				}
			}

			// Wave / 4 divides equally = big boss time.
			if (Wave % 4 == 0)
			{
				audioControllerScript.StopAllSoundtracks (); // Stop all soundtracks.
				// TODO: Play boss soundtrack.

				// Spawn a big boss in normal mode.
				if (gameModifier.BossSpawn == GameModifierManager.bossSpawnMode.Normal)
				{
					StartCoroutine (SpawnBigBoss ());
				}

				// Go to next wave, skip bosses entirely.
				if (gameModifier.BossSpawn == GameModifierManager.bossSpawnMode.NoBosses) 
				{
					StartNewWave ();
					IsInWaveTransition = true;
				}

				SoundtrackText.text = ""; // Clear sounstrack text display.
			}

			WaveTimeRemaining = 0; // Reset wave time remaining.
		}

		// For every wave after a major boss fight.
		if (Wave % 4 == 1 || Wave == 1) 
		{
			SoundtrackText.text = audioControllerScript.TrackName + ""; // Display new soundtrack name.
		}
	}

	// Prepare next wave.
	public void StartNewWave ()
	{
		if (IsInvoking ("IncrementWaveNumber") == false) 
		{
			Invoke ("IncrementWaveNumber", 0);
		}

		StartCoroutine (GoToNextWave ());
	}

	// Update wave number.
	void IncrementWaveNumber ()
	{
		Wave += 1;
		UnityEngine.Debug.Log ("Starting wave: " + Wave + ".");
	}

	// Give delay for wave and prepare essential stuff.
	public IEnumerator GoToNextWave ()
	{
		yield return new WaitForSecondsRealtime (5);
		NextLevel ();

		// When wave is after a multiple of 4.
		if (Wave % 4 == 1) 
		{
			audioControllerScript.NextTrack (); // Set audio controller to next track.
			audioControllerScript.LoadTracks (); // Play loaded tracks.
			UnityEngine.Debug.Log ("New soundtrack loaded. Soundtrack: " + audioControllerScript.TrackName);
		}

		// Go straight to block spawning.
		if (gameModifier.BossSpawn != GameModifierManager.bossSpawnMode.BossesOnly) 
		{
			StartCoroutine (StartBlockSpawn ());
		}

		// Go straight to boss spawning based on wave number.
		if (gameModifier.BossSpawn == GameModifierManager.bossSpawnMode.BossesOnly)
		{
			// Normal wave.
			if (Wave % 4 != 0)
			{
				if (IsInvoking ("SpawnMiniBossObject") == false)
				{
					Invoke ("SpawnMiniBossObject", BossSpawnDelay);
					yield return null;
				}
			}

			// Multiple of 4 wave.
			if (Wave % 4 == 0) 
			{
				if (IsInvoking ("SpawnBigBossObject") == false) 
				{
					Invoke ("SpawnBigBossObject", BossSpawnDelay);
					yield return null;
				}
			}
		}

		StopCoroutine (GoToNextWave ()); // Go to the next wave.
	}

	// Reads and sets Game Modifiers from scriptable object.
	public void SetGameModifiers ()
	{
		// Allows/skips tutorial.
		if (gameModifier.Tutorial == false) 
		{
			playerControllerScript_P1.tutorialManagerScript.TurnOffTutorial ();
		}

		// Sets how powerups should spawn in the game.
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

		// Starting shooting modifier conditions.
		HomingImage.enabled = playerControllerScript_P1.isHoming;
		RicochetImage.enabled = playerControllerScript_P1.isRicochet;

		if (playerControllerScript_P1.isInRapidFire == true) 
		{
			playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.TripleShotFireRates [1];
			RapidfireImage.enabled = true;
		}

		OverdriveImage.enabled = playerControllerScript_P1.isInOverdrive;
	
		Lives = gameModifier.StartingLives;
		playerControllerScript_P1.isHoming = gameModifier.AlwaysHoming;
		playerControllerScript_P1.isRicochet = gameModifier.AlwaysRicochet;
		playerControllerScript_P1.isInRapidFire = gameModifier.AlwaysRapidfire;
		playerControllerScript_P1.isInOverdrive = gameModifier.AlwaysOverdrive;
		StackingObject.SetActive (gameModifier.stacking);
	}
}