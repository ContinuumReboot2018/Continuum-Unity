using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PostProcessing;

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

	[Header ("Scoring")]
	public bool CountScore;
	public float DisplayScore;
	public float CurrentScore;
	public float TargetScore;
	public float ScoreSmoothing;
	public TextMeshProUGUI ScoreText;
	public Animator ScoreAnim;

	//public int ScoreMult;
	//public float ScoreRate;

	[Header ("Combo")]
	public int combo = 1;
	public float comboDuration = 0.5f;
	public float comboTimeRemaining;

	[Header ("Block Spawner")]
	public GameObject[] Blocks;
	public float BlockSpawnRate;
	private float NextBlockSpawn;
	public float[] BlockSpawnXPositions;
	public float BlockSpawnYPosition;
	public float BlockSpawnZPosition;

	[Header ("Pausing")]
	public bool isPaused;
	public float PauseCooldown = 1;
	public bool canPause = true;
	public GameObject PauseUI;
	[HideInInspector]
	public float NextPauseCooldown;
	public bool isInOtherMenu;

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
	public ParticleSystem StarFieldForeground;

	[Header ("Debug")]
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


	void Start () 
	{
		ScoreText.text = "0";
		SetStartOrthSize ();
		cursorManagerScript.HideMouse ();
		cursorManagerScript.LockMouse ();
		Wave = 1;
		WaveTimeDuration = FirstWaveTimeDuration;
		StartCoroutines ();
	}

	public void StartCoroutines ()
	{
		ScoreText.text = "";
		StartCoroutine (UpdateImageEffects ());
		StartCoroutine (UpdateStarFieldparticleEffectTrail ());
	}

	// Timescale controller calls this initially after the countdown.
	public void StartGame ()
	{
		StartCoroutine (LevelTimer ());
		StartCoroutine (StartBlockSpawn ());
	}

	void Update ()
	{
		UpdateGameStats ();
		UpdateTimeStats ();
		CheckCombo ();
	}

	IEnumerator UpdateStarFieldparticleEffectTrail ()
	{
		isUpdatingParticleEffects = true;
		while (isUpdatingParticleEffects == true) 
		{
			var StarFieldForegroundTrailModule = StarFieldForeground.trails;
			StarFieldForegroundTrailModule.lifetime = new ParticleSystem.MinMaxCurve (0, StarFieldForegroundLifetimeMultipler * Time.timeScale);

			var StarFieldForegroundMainModule = StarFieldForeground.main;
			StarFieldForegroundMainModule.simulationSpeed = StarFieldForegroundSimulationSpeed * Time.timeScale;

			yield return null;
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
				DistanceText_Debug.text = "Distance: " + System.Math.Round (Distance, 2);

				GameTimeText_Debug.text = "Game Time: " + string.Format ("{0}:{1:00}", (int)GameTime / 60, (int)GameTime % 60);
				RealTimeText_Debug.text = "Real Time: " + string.Format ("{0}:{1:00}", (int)RealTime / 60, (int)RealTime % 60);
				TimeRatioText_Debug.text = "Time Ratio: " + System.Math.Round (TimeRatio, 2);

				TargetPitch_Debug.text = "Target Pitch: " + audioControllerScript.BassTargetPitch;
				CurrentPitch_Debug.text = "Current Pitch: " + System.Math.Round (BassTrack.pitch, 4);

				TimeScaleText_Debug.text = "TimeScale: " + System.Math.Round (Time.timeScale, 2);
				FixedTimeStepText_Debug.text = "FixedTimeStep: " + System.Math.Round (Time.fixedDeltaTime * 2, 5);

				SpawnWaitText_Debug.text = "Spawn Rate: " + BlockSpawnRate;
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

	void CheckCombo ()
	{
		if (comboTimeRemaining > 0) 
		{
			comboTimeRemaining -= Time.unscaledDeltaTime * (0.05f * combo);
		}

		if (comboTimeRemaining < 0) 
		{
			if (combo > 1) 
			{
				combo -= 1;
				comboTimeRemaining = comboDuration;
			}
		}
	}

	void UpdateTimeStats ()
	{
		Distance = timescaleControllerScript.Distance;
		GameTime += Time.deltaTime;
		RealTime += Time.unscaledDeltaTime;
		TimeRatio = GameTime / RealTime;
	}

	public void ResetScore ()
	{
		TargetScore = 0;
		CurrentScore = 0;
		DisplayScore = 0;
		ScoreText.text = "" + DisplayScore;
	}

	IEnumerator UpdateImageEffects ()
	{
		isUpdatingImageEffects = true;
		while (isUpdatingImageEffects == true)
		{
			if (isPaused == true) 
			{
				var motionblursettings = ImageEffects.motionBlur.settings;
				motionblursettings.frameBlending = 0;
				ImageEffects.motionBlur.settings = motionblursettings;
			}

			if (isPaused == false)
			{
				if (timescaleControllerScript.isInInitialSequence == false && timescaleControllerScript.isInInitialCountdownSequence == false)
				{
					var ImageEffectsMotionBlurModuleSettings = ImageEffects.motionBlur.settings;
					ImageEffectsMotionBlurModuleSettings.frameBlending = Mathf.Clamp (-0.12f * timescaleControllerScript.Distance + 1.18f, 0, 1); 
					ImageEffects.motionBlur.settings = ImageEffectsMotionBlurModuleSettings;
				}

				if (timescaleControllerScript.isInInitialSequence == true || timescaleControllerScript.isInInitialCountdownSequence == true) 
				{
					var ImageEffectsMotionBlurModuleSettings = ImageEffects.motionBlur.settings;
					ImageEffectsMotionBlurModuleSettings.frameBlending = 0; 
					ImageEffects.motionBlur.settings = ImageEffectsMotionBlurModuleSettings;
				}
			}

			yield return null;
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
				PauseUI.SetActive (false);
				cursorManagerScript.LockMouse ();
				cursorManagerScript.HideMouse ();
				audioControllerScript.updateVolumeAndPitches = true;

				if (timescaleControllerScript.isInInitialSequence == false && 
					timescaleControllerScript.isInInitialCountdownSequence == false) 
				{
					CountScore = true;
				}

				timescaleControllerScript.isOverridingTimeScale = false;
				timescaleControllerScript.OverrideTimeScaleTimeRemaining = 0;
			}

			NextPauseCooldown = Time.unscaledTime + PauseCooldown;
		}
	}

	public void InvokeUnpause ()
	{
		if (isInOtherMenu == false) 
		{
			isPaused = false;
			PauseUI.SetActive (false);
			cursorManagerScript.LockMouse ();
			cursorManagerScript.HideMouse ();
			audioControllerScript.updateVolumeAndPitches = true;

			if (timescaleControllerScript.isInInitialSequence == false && 
				timescaleControllerScript.isInInitialCountdownSequence == false) 
			{
				CountScore = true;
			}

			timescaleControllerScript.isOverridingTimeScale = false;
			timescaleControllerScript.OverrideTimeScaleTimeRemaining = 0;

			NextPauseCooldown = Time.unscaledTime + PauseCooldown;
		}
	}

	public void SetPauseOtherMenu (bool otherMenu)
	{
		isInOtherMenu = otherMenu;
	}

	void SetStartOrthSize ()
	{
		//MainCamera.orthographicSize = StartOrthSize;
	}

	void CheckOrthSize ()
	{
		//OrthSize = -0.27f * (timeScaleControllerScript.Distance) + 10;
		//OrthSize = 4 * Mathf.Sin (0.17f * timeScaleControllerScript.Distance) + 8;

		//MainCamera.orthographicSize = Mathf.SmoothDamp (MainCamera.orthographicSize, OrthSize, ref OrthSizeVel, OrthSizeSmoothTime * Time.deltaTime);
	}

	public IEnumerator LevelTimer ()
	{
		while (WaveTimeRemaining > 0) 
		{
			WaveTimeRemaining -= Time.deltaTime;
			UpdateScoreIncrements ();
			yield return null;
		}
	}

	public void NextLevel ()
	{
		WaveTimeDuration += WaveTimeIncreaseRate;
		WaveTimeRemaining = WaveTimeDuration;
	}

	IEnumerator StartBlockSpawn ()
	{
		int StartXPosId = Random.Range (0, BlockSpawnXPositions.Length);
		int NextXPosId = StartXPosId;

		while (WaveTimeRemaining > 0) 
		{
			if (Time.time > NextBlockSpawn)
			{
				GameObject Block = Blocks [Random.Range (0, Blocks.Length)];

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
				}

				Vector3 SpawnPos = new Vector3 (BlockSpawnXPositions[NextXPosId], BlockSpawnYPosition, BlockSpawnZPosition);
				Vector3 SpawnPosRand = new Vector3 (BlockSpawnXPositions[Random.Range (0, BlockSpawnXPositions.Length)], BlockSpawnYPosition, BlockSpawnZPosition);
				Instantiate (Block, SpawnPos, Quaternion.identity);
				Instantiate (Block, SpawnPosRand, Quaternion.identity);
				NextBlockSpawn = Time.time + BlockSpawnRate;
			}
			yield return null;
		}
	}
}
