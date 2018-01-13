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

	[Header ("Game Stats")]
	public bool TrackStats = true;
	public float GameTime;
	public float RealTime;
	public float TimeRatio;
	public float Distance;
	public AudioSource BassTrack;

	[Header ("Waves")]
	public int Wave;
	public float WaveTimeIncreaseRate;
	public float FirstWaveTimeDuration;
	public float WaveTimeDuration;
	public float WaveTimeRemaining;
	public TextMeshProUGUI WaveText;
	public Animator WaveAnim;
	public RawImage WaveBackground;
	public ParticleSystem WaveTransitionParticles;
	public Animator WaveTransitionAnim;
	public TextMeshProUGUI WaveTransitionText;
	public AudioSource NextLevelAudio;

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
	public TextMeshProUGUI LivesText;
	public Animator LivesAnim;
	public RawImage LifeOne, LifeTwo, LifeThree;
	public RawImage LivesBackground;
	public GameObject LivesSpacing;

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
	public float PowerupPickupSpawnRate;
	private float NextPowerupPickupSpawn;
	public float PowerupPickupSpawnRangeX;
	public float PowerupPickupSpawnY;

	// UI for powerup list
	public int NextAvailablePowerupSlot_P1;
	public RawImage PowerupShootingImage_P1;
	public TextMeshProUGUI PowerupShootingText_P1;

	public bool PowerupOneOccupied_P1;
	public RawImage PowerupOneImage_P1;
	public TextMeshProUGUI PowerupOneText_P1;

	public bool PowerupTwoOccupied_P1;
	public RawImage PowerupTwoImage_P1;
	public TextMeshProUGUI PowerupTwoText_P1;

	public bool PowerupThreeOccupied_P1;
	public RawImage PowerupThreeImage_P1;
	public TextMeshProUGUI PowerupThreeText_P1;

	// UI for powerup list
	public int NextAvailablePowerupSlot_P2;
	public RawImage PowerupShootingImage_P2;
	public TextMeshProUGUI PowerupShootingText_P2;

	public bool PowerupOneOccupied_P2;
	public RawImage PowerupOneImage_P2;
	public TextMeshProUGUI PowerupOneText_P2;

	public bool PowerupTwoOccupied_P2;
	public RawImage PowerupTwoImage_P2;
	public TextMeshProUGUI PowerupTwoText_P2;

	public bool PowerupThreeOccupied_P2;
	public RawImage PowerupThreeImage_P2;
	public TextMeshProUGUI PowerupThreeText_P2;

	[Header ("BossSpawner")]
	public GameObject[] MiniBosses;
	public Transform MiniBossSpawnPos;
	public float bossSpawnDelay = 4;

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
	public TextMeshProUGUI P1_DoubleShotIteration;
	public TextMeshProUGUI P1_DoubleShotIterationNumber;
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

	void Awake () 
	{
		StartPowerupUI ();
		ScoreText.text = "";
		ScoreBackground.enabled = false;
		TrackStats = false;

		Lives = 3;
		LivesText.text = "";
		LivesBackground.enabled = false;

		Wave = 1;
		WaveTimeDuration = FirstWaveTimeDuration;
		WaveText.text = "";
		WaveAnim.enabled = false;
		WaveBackground.enabled = false;

		SetStartOrthSize ();
		cursorManagerScript.HideMouse ();
		cursorManagerScript.LockMouse ();
	}

	void StartPowerupUI ()
	{
		PowerupShootingImage_P1.texture = null;
		PowerupShootingImage_P1.color = new Color (0, 0, 0, 0);
		PowerupShootingText_P1.text = " ";

		PowerupOneImage_P1.texture = null;
		PowerupOneImage_P1.color = new Color (0, 0, 0, 0);
		PowerupOneText_P1.text = " ";

		PowerupTwoImage_P1.texture = null;
		PowerupTwoImage_P1.color = new Color (0, 0, 0, 0);
		PowerupTwoText_P1.text = " ";

		PowerupThreeImage_P1.texture = null;
		PowerupThreeImage_P1.color = new Color (0, 0, 0, 0);
		PowerupThreeText_P1.text = " ";


		PowerupShootingImage_P2.texture = null;
		PowerupShootingImage_P2.color = new Color (0, 0, 0, 0);
		PowerupShootingText_P2.text = " ";

		PowerupOneImage_P2.texture = null;
		PowerupOneImage_P2.color = new Color (0, 0, 0, 0);
		PowerupOneText_P2.text = " ";

		PowerupTwoImage_P2.texture = null;
		PowerupTwoImage_P2.color = new Color (0, 0, 0, 0);
		PowerupTwoText_P2.text = " ";

		PowerupThreeImage_P2.texture = null;
		PowerupThreeImage_P2.color = new Color (0, 0, 0, 0);
		PowerupThreeText_P2.text = " ";
	}

	void Start ()
	{
		StartCoroutines ();
	}

	public void StartCoroutines ()
	{
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
		StartCoroutine (LevelTimer ());
		StartCoroutine (StartBlockSpawn ());
		StartCoroutine (PowerupSpawner ());
		ScoreAnim.enabled = true;
		LivesAnim.enabled = true;
		WaveAnim.enabled = true;
		ScoreBackground.enabled = true;
		LivesBackground.enabled = true;
		WaveBackground.enabled = true;

	}

	void Update ()
	{
		UpdateGameStats ();
		UpdateLives ();
		UpdateTimeStats ();
		CheckCombo ();
		CheckPowerupTime ();
		UpdateScoreIncrements ();
		UpdateBlockSpawnTime ();
		UpdateStarFieldparticleEffectTrail ();
		UpdateImageEffects ();
		//CheckOrthSize ();

		if (WaveTimeRemaining < 0) 
		{
			StartCoroutine (SpawnMiniBoss ());
			WaveTimeRemaining = 0;
		}
	}

	void UpdateStarFieldparticleEffectTrail ()
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
				WaveText_Debug.text = "Wave: " + Wave;
				TargetScoreText_Debug.text = "Target Score: " + Mathf.Round (TargetScore);
				ComboText_Debug.text = "Combo: " + combo; 
				DistanceText_Debug.text = "Distance: " + Math.Round (Distance, 2);
				GameTimeText_Debug.text = "Game Time: " + string.Format ("{0}:{1:00}", (int)GameTime / 60, (int)GameTime % 60);
				RealTimeText_Debug.text = "Real Time: " + string.Format ("{0}:{1:00}", (int)RealTime / 60, (int)RealTime % 60);
				TimeRatioText_Debug.text = "Time Ratio: " + Math.Round (TimeRatio, 2);
				TargetPitch_Debug.text = "Target Pitch: " + audioControllerScript.BassTargetPitch;
				CurrentPitch_Debug.text = "Current Pitch: " + Math.Round (BassTrack.pitch, 4);
				TimeScaleText_Debug.text = "Time Scale: " + Math.Round (Time.timeScale, 2);
				FixedTimeStepText_Debug.text = "Fixed Time Step: " + Math.Round (Time.fixedDeltaTime, 5);
				SpawnWaitText_Debug.text = "Spawn Rate: " + BlockSpawnRate;
				WaveTimeRemainingText_Debug.text = "Wave Time Remain: " + Math.Round (WaveTimeRemaining, 1) + " s";
				P1_CurrentFireRate.text = "P1 Fire Rate: " + playerControllerScript_P1.CurrentFireRate;
				P1_Ability.text = "P1 Ability: " + playerControllerScript_P1.AbilityName;
				P1_AbilityTimeRemaining.text = "P1 Ability Remain: " + Math.Round (playerControllerScript_P1.CurrentAbilityTimeRemaining, 2);
				P1_AbilityTimeDuration.text = "P1 Max Ability Time: " + playerControllerScript_P1.CurrentAbilityDuration;
				P1_AbilityTimeProportion.text = "P1 Ability Fill: " + Math.Round (playerControllerScript_P1.AbilityTimeAmountProportion, 2);
				P1_DoubleShotIteration.text = "P1 Double Shot Stage: " + playerControllerScript_P1.DoubleShotIteration.ToString ();
				P1_DoubleShotIterationNumber.text = "P1 Double Shot Iteration: " + playerControllerScript_P1.NextDoubleShotIteration;
				CheatTimeRemainText_Debug.text = "Cheat Time Remain: " + Math.Round (developerModeScript.CheatStringResetTimeRemaining, 1);
				CheatStringText_Debug.text = "Cheat Input Field: " + developerModeScript.CheatString;
				LastCheatText_Debug.text = "Last Cheat: " + developerModeScript.LastCheatName;
				PowerupTimeRemain_Debug.text = "Powerup Time Remain: " + Math.Round (PowerupTimeRemaining, 1);
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
			ScoreText.text = DisplayScore.ToString ("00000000");
		}
	}

	void UpdateLives ()
	{
		if (timescaleControllerScript.isInInitialSequence == true || timescaleControllerScript.isInInitialCountdownSequence == true) 
		{
			if (LivesText.gameObject.activeSelf == true) 
			{
				LifeOne.gameObject.SetActive (false);
				LifeTwo.gameObject.SetActive (false);
				LifeThree.gameObject.SetActive (false);

				LivesText.gameObject.SetActive (false);
				LivesText.text = "";
			}
		}

		if (timescaleControllerScript.isInInitialSequence == false && timescaleControllerScript.isInInitialCountdownSequence == false) {
			// Check how many life images are supposed to be there
			switch (Lives) {
			case 0:
				if (LivesText.gameObject.activeSelf == true) 
				{
					LifeOne.gameObject.SetActive (false);
					LifeTwo.gameObject.SetActive (false);
					LifeThree.gameObject.SetActive (false);
					LivesSpacing.SetActive (false);
					LivesText.gameObject.SetActive (false);
					LivesText.text = "";
				}
				break;
			case 1:
				LivesText.gameObject.SetActive (false);
				LifeOne.gameObject.SetActive (false);
				LifeTwo.gameObject.SetActive (false);
				LifeThree.gameObject.SetActive (false);
				LivesSpacing.SetActive (false);
				//LivesBackground.enabled = false;
				LivesText.text = "";
				break;
			case 2:
				LivesText.gameObject.SetActive (false);
				LifeOne.gameObject.SetActive (true);
				LifeTwo.gameObject.SetActive (false);
				LifeThree.gameObject.SetActive (false);
				LivesSpacing.SetActive (false);
				//LivesBackground.enabled = true;
				LivesText.text = "";
				break;
			case 3:
				LivesText.gameObject.SetActive (false);
				LifeOne.gameObject.SetActive (true);
				LifeTwo.gameObject.SetActive (true);
				LifeThree.gameObject.SetActive (false);
				LivesSpacing.SetActive (false);
				//LivesBackground.enabled = true;
				LivesText.text = "";
				break;
			case 4:
				LivesText.gameObject.SetActive (false);
				LifeOne.gameObject.SetActive (true);
				LifeTwo.gameObject.SetActive (true);
				LifeThree.gameObject.SetActive (true);
				LivesSpacing.SetActive (false);
				//LivesBackground.enabled = true;
				LivesText.text = "";
				break;
			}

			if (Lives > 4) 
			{
				LivesText.gameObject.SetActive (true);
				LifeOne.gameObject.SetActive (true);
				LifeTwo.gameObject.SetActive (false);
				LifeThree.gameObject.SetActive (false);
				LivesSpacing.SetActive (true);
				//LivesBackground.enabled = true;
				LivesText.text = "" + Lives;
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
				}
			}
		}

		if (PowerupTimeRemaining < 0) 
		{
			playerControllerScript_P1.powerupsInUse = 0;
			PowerupTimeRemaining = 0;
			PowerupAnim.StopPlayback ();
			PowerupResetAudio.Play ();
			// Reset all powerups for both players.
			playerControllerScript_P1.ResetPowerups ();
		}
	}

	public void SetPowerupTime (float Duration)
	{
		PowerupTimeDuration = Duration;
		PowerupTimeRemaining = PowerupTimeDuration;
	}

	void UpdateTimeStats ()
	{
		if (isPaused == false && TrackStats == true) 
		{
			Distance = timescaleControllerScript.Distance;
			GameTime += Time.deltaTime;
			RealTime += Time.unscaledDeltaTime;
			TimeRatio = GameTime / RealTime;
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

		MainCamera.orthographicSize = Mathf.SmoothDamp (MainCamera.orthographicSize, OrthSize, ref OrthSizeVel, OrthSizeSmoothTime * Time.deltaTime);
	}

	public IEnumerator LevelTimer ()
	{
		while (WaveTimeRemaining > 0) 
		{
			if (playerControllerScript_P1.isInCooldownMode == false && isPaused == false && isGameOver == false) 
			{
				WaveTimeRemaining -= Time.deltaTime;
			}
			yield return null;
		}
	}

	public void NextLevel ()
	{
		if (Wave > 1) 
		{
			BlockSpawnRate -= BlockSpawnIncreaseRate;
		}

		WaveTimeDuration += WaveTimeIncreaseRate;
		WaveTimeRemaining = WaveTimeDuration;
		WaveTransitionParticles.Play (true);
		WaveTransitionAnim.Play ("WaveTransition");
		WaveTransitionText.text = "WAVE " + Wave;
		playerControllerScript_P1.camShakeScript.shakeAmount = 0.4f;
		playerControllerScript_P1.camShakeScript.shakeDuration = 3.7f;
		playerControllerScript_P1.camShakeScript.Shake ();
		playerControllerScript_P1.Vibrate (0.6f, 0.6f, 3);
		NextLevelAudio.Play ();
		StartCoroutine (LevelTimer ());
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
		if (anyBlock == false) {
			if (isGameOver == false) {
				if (Wave == 1) {
					GameObject Block = Blocks [UnityEngine.Random.Range (0, 1)];
					Vector3 SpawnPosRand = new Vector3 (BlockSpawnXPositions [UnityEngine.Random.Range (0, BlockSpawnXPositions.Length)], BlockSpawnYPosition, BlockSpawnZPosition);
					Instantiate (Block, SpawnPosRand, Quaternion.identity);
				}

				if (Wave == 2) {
					GameObject Block = Blocks [UnityEngine.Random.Range (0, 2)];
					Vector3 SpawnPosRand = new Vector3 (BlockSpawnXPositions [UnityEngine.Random.Range (0, BlockSpawnXPositions.Length)], BlockSpawnYPosition, BlockSpawnZPosition);
					Instantiate (Block, SpawnPosRand, Quaternion.identity);
				}

				if (Wave == 3) {
					GameObject Block = Blocks [UnityEngine.Random.Range (0, 3)];
					Vector3 SpawnPosRand = new Vector3 (BlockSpawnXPositions [UnityEngine.Random.Range (0, BlockSpawnXPositions.Length)], BlockSpawnYPosition, BlockSpawnZPosition);
					Instantiate (Block, SpawnPosRand, Quaternion.identity);
				}

				if (Wave == 4) {
					GameObject Block = Blocks [UnityEngine.Random.Range (0, 4)];
					Vector3 SpawnPosRand = new Vector3 (BlockSpawnXPositions [UnityEngine.Random.Range (0, BlockSpawnXPositions.Length)], BlockSpawnYPosition, BlockSpawnZPosition);
					Instantiate (Block, SpawnPosRand, Quaternion.identity);
				}

				if (Wave >= 5) {
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

	IEnumerator PowerupSpawner ()
	{
		while (isGameOver == false) 
		{
			yield return new WaitForSecondsRealtime (UnityEngine.Random.Range (PowerupPickupSpawnRate, PowerupPickupSpawnRate * 2));
			SpawnPowerupPickup ();
			yield return null;
		}
	}

	public void SpawnPowerupPickup ()
	{
		GameObject PowerupPickup = PowerupPickups[UnityEngine.Random.Range (0, PowerupPickups.Length)];
		Vector3 PowerupPickupSpawnPos = new Vector3 (
			UnityEngine.Random.Range (-PowerupPickupSpawnRangeX, PowerupPickupSpawnRangeX), 
			UnityEngine.Random.Range (-PowerupPickupSpawnY, PowerupPickupSpawnY), 
			-2.5f);
		Instantiate (PowerupPickup, PowerupPickupSpawnPos, Quaternion.identity);
	}

	public IEnumerator SpawnMiniBoss ()
	{
		yield return new WaitForSeconds (bossSpawnDelay);
		GameObject MiniBoss = MiniBosses [UnityEngine.Random.Range (0, MiniBosses.Length)];
		Instantiate (MiniBoss, MiniBossSpawnPos.position, MiniBossSpawnPos.rotation);
	}

	public void StartNewWave ()
	{
		StartCoroutine (GoToNextWave ());
	}

	public IEnumerator GoToNextWave ()
	{
		yield return new WaitForSecondsRealtime (5);
		Wave += 1;
		NextLevel ();
		StartCoroutine (StartBlockSpawn ());
	}
}
