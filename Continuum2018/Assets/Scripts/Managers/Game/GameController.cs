using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PostProcessing;
using System.Diagnostics;

public class GameController : MonoBehaviour 
{
	public PlayerController playerControllerScript_P1;
	public TimescaleController timescaleControllerScript;
	public AudioController audioControllerScript;
	public DeveloperMode developerModeScript;
	public CursorManager cursorManagerScript;
	public PostProcessingProfile ImageEffects;
	public ObjectPooler BlockObjectPooler;
	public GameModifierManager gameModifier;

	[Header ("Game Stats")]
	public bool TrackStats = true;
	public float GameTime;
	public float RealTime;
	public float TimeRatio;
	public float Distance;
	public int BlocksDestroyed;
	public int BulletsShot;
	public float BlockShotAccuracy;
	public AudioSource BassTrack;
	public float TrialTime = -1;

	[Header ("Waves")]
	public int Wave;
	public float WaveTimeIncreaseRate;
	public float FirstWaveTimeDuration;
	public float WaveTimeDuration;
	public float WaveTimeRemaining;
	public bool IsInWaveTransition;
	public TextMeshProUGUI WaveText;
	public Animator WaveAnim;
	public RawImage WaveBackground;
	public ParticleSystem WaveTransitionParticles;
	public Animator WaveTransitionAnim;
	public TextMeshProUGUI WaveTransitionText;
	public AudioSource NextLevelAudio;
	public Slider WaveTimeSlider;
	public TextMeshProUGUI WaveTimeRemainText;
	public TextMeshProUGUI SoundtrackText;

	// This is for wave transition UI only.
	public TextMeshProUGUI GameTimeText;
	public TextMeshProUGUI RealTimeText;
	public TextMeshProUGUI TimeRatioText;
	public TextMeshProUGUI BlocksDestroyedText;
	public TextMeshProUGUI BulletsShotText;
	public TextMeshProUGUI AccuracyText;

	[Header ("Scoring")]
	public bool CountScore;
	public float DisplayScore;
	public float CurrentScore;
	public float TargetScore;
	public float ScoreSmoothing;
	public TextMeshProUGUI ScoreText;
	public Animator ScoreAnim;
	public RawImage ScoreBackground;

	[Header ("Lives")]
	public int Lives;
	public int MaxLives = 11;
	public TextMeshProUGUI LivesText;
	public Animator LivesAnim;
	public RawImage[] LifeImages;
	public RawImage LivesBackground;
	public GameObject LivesSpacing;
	public TextMeshProUGUI MaxLivesText;

	[Header ("Combo")]
	public int combo = 1;
	public float comboDuration = 0.5f;
	public float comboTimeRemaining;

	[Header ("Block Spawner")]
	public GameObject[] Blocks;
	public float BlockSpawnRate;
	private float NextBlockSpawn;
	public float blockSpawnStartDelay = 2;
	public float[] BlockSpawnXPositions;
	public float BlockSpawnYPosition;
	public float BlockSpawnZPosition;
	public float BlockSpawnIncreaseRate;

	[Header ("Powerup General")]
	public float PowerupTimeRemaining;
	public float PowerupTimeDuration;
	public int MaxSimultaneousPowerups = 4;
	public Animator PowerupAnim;
	public AudioSource PowerupTimeRunningOutAudio;
	public AudioSource PowerupResetAudio;
	public GameObject[] PowerupPickups;
	public float powerupPickupTimeRemaining;
	public Vector2 PowerupPickupSpawnRate;
	public float powerupPickupSpawnModifier = 1;
	private float NextPowerupPickupSpawn;
	public float PowerupPickupSpawnRangeX;
	public float PowerupPickupSpawnY;

	[Header ("Powerup List UI")]
	public int NextPowerupSlot_P1;
	public RawImage[] PowerupImage_P1;
	public Texture2D StandardShotTexture;
	public TextMeshProUGUI[] PowerupText_P1;
	public RawImage OverdriveImage;
	public RawImage RapidfireImage;
	public RawImage RicochetImage;
	public RawImage HomingImage;

	public GameObject PowerupPickupUI;

	[Header ("BossSpawner")]
	public GameObject[] MiniBosses;
	public Transform MiniBossSpawnPos;
	public float MiniBossSpawnDelay = 4;

	public GameObject[] BigBosses;
	public Transform BigBossSpawnPos;
	public float BigBossSpawnDelay = 4;
	public int bossId;
	public float BossSpawnDelay = 5;

	[Header ("Pausing")]
	public bool isPaused;
	public float PauseCooldown = 1;
	public bool canPause = true;
	public GameObject PauseUI;
	[HideInInspector]
	public float NextPauseCooldown;
	public bool isInOtherMenu;

	[Header ("Game Over")]
	public bool isGameOver;
	public GameObject GameoverUI;

	[Header ("Camera")]
	public Camera MainCamera;
	public float OrthSize = 10;
	public float StartOrthSize = 10;
	private float OrthSizeVel;
	public float OrthSizeSmoothTime = 1;

	[Header ("Visuals")]
	public bool isUpdatingImageEffects = true;
	public bool isUpdatingParticleEffects = true;

	public float StarFieldForegroundLifetimeMultipler = 0.1f;
	public float StarFieldForegroundSimulationSpeed = 1;
	public float StarFieldBackgroundLifetimeMultipler = 0.1f;
	public float StarFieldBackgroundSimulationSpeed = 1;

	public ParticleSystem StarFieldForeground;
	public ParticleSystem StarFieldBackground;

	public float TargetDepthDistance;

	[Header ("Debug")]
	public TextMeshProUGUI P1_ShootingIterationRapid;
	public TextMeshProUGUI P1_ShootingIterationOverdrive;
	public TextMeshProUGUI P1_ShootingIterationRicochet;
	public TextMeshProUGUI P1_CurrentFireRate;
	public TextMeshProUGUI P1_Ability;
	public TextMeshProUGUI P1_AbilityTimeRemaining;
	public TextMeshProUGUI P1_AbilityTimeDuration;
	public TextMeshProUGUI P1_AbilityTimeProportion;
	public TextMeshProUGUI WaveText_Debug;
	public TextMeshProUGUI ComboText_Debug;
	public TextMeshProUGUI CurrentPitch_Debug;
	public TextMeshProUGUI TargetPitch_Debug;
	public TextMeshProUGUI DistanceText_Debug;
	public TextMeshProUGUI GameTimeText_Debug;
	public TextMeshProUGUI RealTimeText_Debug;
	public TextMeshProUGUI TimeRatioText_Debug;
	public TextMeshProUGUI TimeScaleText_Debug;
	public TextMeshProUGUI FixedTimeStepText_Debug;
	public TextMeshProUGUI TargetScoreText_Debug;
	public TextMeshProUGUI SpawnWaitText_Debug;
	public TextMeshProUGUI WaveTimeRemainingText_Debug;
	public TextMeshProUGUI CheatTimeRemainText_Debug;
	public TextMeshProUGUI CheatStringText_Debug;
	public TextMeshProUGUI LastCheatText_Debug;
	public TextMeshProUGUI PowerupTimeRemain_Debug;
	public TextMeshProUGUI AddedTimeText_Debug;
	public TextMeshProUGUI LivesText_Debug;
	public TextMeshProUGUI BlocksDestroyedText_Debug;
	public TextMeshProUGUI BulletsShotText_Debug;
	public TextMeshProUGUI BlockShotAccuracyText_Debug;
	public TextMeshProUGUI RewindTimeRemainingText_Debug;
	public TextMeshProUGUI IsRewindingText_Debug;

	void Awake () 
	{
		ClearPowerupUI ();
		ScoreText.text = "";
		ScoreBackground.enabled = false;

		LivesAnim.gameObject.SetActive (false);
		//Lives = 3;
		LivesText.text = "";
		MaxLivesText.text = "";
		LivesBackground.enabled = false;

		Wave = 1;
		WaveTimeDuration = FirstWaveTimeDuration;
		WaveText.text = "";
		WaveAnim.enabled = false;
		WaveBackground.enabled = false;
		IsInWaveTransition = true;
		bossId = 0;
		powerupPickupTimeRemaining = UnityEngine.Random.Range (PowerupPickupSpawnRate.x, PowerupPickupSpawnRate.y);

		SetStartOrthSize ();
		cursorManagerScript.HideMouse ();
		cursorManagerScript.LockMouse ();
	}

	public void ClearPowerupUI ()
	{
		OverdriveImage.enabled = false;

		if (gameModifier.AlwaysRapidfire == false) 
		{
			RapidfireImage.enabled = false;
		}

		RicochetImage.enabled = false;
		HomingImage.enabled = false;

		foreach (RawImage powerupImage in PowerupImage_P1) 
		{
			powerupImage.texture = null;
			powerupImage.color = new Color (0, 0, 0, 0);
		}

		foreach (TextMeshProUGUI powerupText in PowerupText_P1)
		{
			powerupText.text = String.Empty;
		}

		NextPowerupSlot_P1 = 1;
	}

	void Start ()
	{
		InvokeRepeating ("UpdateBlockSpawnTime", 0, 1);
		InvokeRepeating ("UpdateLives", 0, 1);
		SetGameModifiers ();

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

		ScoreAnim.enabled = true;
		LivesAnim.gameObject.SetActive (true);
		LivesAnim.enabled = true;
		WaveAnim.enabled = true;
		ScoreBackground.enabled = true;
		LivesBackground.enabled = true;
		WaveBackground.enabled = true;

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
					"Current Pitch: " + Math.Round (BassTrack.pitch, 4);
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
				ScoreText.text = DisplayScore.ToString ("00000000");
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

			TimeRatioText.text = "AVERAGE TIME SCALE: " + System.Math.Round((GameTime / RealTime), 2).ToString ("0.00") + "";

			BlocksDestroyedText.text = "BLOCKS DESTROYED: " + BlocksDestroyed;
			BulletsShotText.text = "BULLETS SHOT: " + BulletsShot;
			AccuracyText.text = "ACCURACY: " + System.Math.Round(BlockShotAccuracy * 100, 2) + "%";
			WaveTimeRemainText.text = "WAVE TIME: " + System.Math.Round (WaveTimeRemaining, 0);
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

	void SetStartOrthSize ()
	{
		MainCamera.orthographicSize = StartOrthSize;
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
			BlockSpawnRate -= BlockSpawnIncreaseRate;
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

		NextLevelAudio.Play ();
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
		case GameModifierManager.powerupSpawnMode.Off:
			powerupPickupSpawnModifier = Mathf.Infinity;
			break;
		}

		Lives = gameModifier.StartingLives;
		playerControllerScript_P1.isInRapidFire = gameModifier.AlwaysRapidfire;

		if (playerControllerScript_P1.isInRapidFire == true) 
		{
			playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.TripleShotFireRates [1];
			RapidfireImage.enabled = true;
		}
	}
}
