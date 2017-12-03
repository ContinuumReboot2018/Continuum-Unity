﻿using System.Collections;
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
	public CursorManager cursorManagerScript;
	public PostProcessingProfile ImageEffects;

	[Header ("Game Stats")]
	public bool TrackStats = true;
	public float GameTime;
	public float RealTime;
	public float TimeRatio;
	public TextMeshProUGUI GameTimeText_Debug;
	public TextMeshProUGUI RealTimeText_Debug;
	public TextMeshProUGUI TimeRatioText_Debug;
	public float Distance;
	public TextMeshProUGUI DistanceText;

	[Header ("Scoring")]
	public bool CountScore;
	public float DisplayScore;
	public float CurrentScore;
	public float TargetScore;
	public float ScoreSmoothing;
	public TextMeshProUGUI ScoreText;

	public int ScoreMult;
	public float ScoreRate;

	[Header ("Pausing")]
	public bool isPaused;
	public float PauseCooldown = 1;
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
	public float StarFieldForegroundLifetimeMultipler = 0.1f;
	public float StarFieldForegroundSimulationSpeed = 1;
	public ParticleSystem StarFieldForeground;

	void Start () 
	{
		ScoreText.text = "0";
		SetStartOrthSize ();
		cursorManagerScript.HideMouse ();
		cursorManagerScript.LockMouse ();
	}

	void Update () 
	{
		UpdateGameStats ();
		UpdateScore ();
		CheckOrthSize ();
		UpdateStarFieldparticleEffectTrail ();	
		UpdateImageEffects ();
	}

	void UpdateStarFieldparticleEffectTrail ()
	{
		var StarFieldForegroundTrailModule = StarFieldForeground.trails;
		StarFieldForegroundTrailModule.lifetime = new ParticleSystem.MinMaxCurve (0, StarFieldForegroundLifetimeMultipler * Time.timeScale);

		var StarFieldForegroundMainModule = StarFieldForeground.main;
		StarFieldForegroundMainModule.simulationSpeed = StarFieldForegroundSimulationSpeed * Time.timeScale;
	}

	void UpdateGameStats ()
	{
		if (TrackStats == true && isPaused == false) 
		{
			Distance = timescaleControllerScript.Distance;
			GameTime += Time.deltaTime;
			RealTime += Time.unscaledDeltaTime;
			TimeRatio = GameTime / RealTime;

			DistanceText.text = "Distance: " + System.Math.Round (Distance, 2);
			GameTimeText_Debug.text = "Game Time: " + string.Format ("{0}:{1:00}", (int)GameTime / 60, (int)GameTime % 60);
			RealTimeText_Debug.text = "Real Time: " + string.Format ("{0}:{1:00}", (int)RealTime / 60, (int)RealTime % 60);
			TimeRatioText_Debug.text = "Time Ratio: " + System.Math.Round (TimeRatio, 2);
		}
	}

	void UpdateScore ()
	{
		if (CountScore == true) 
		{
			TargetScore += ScoreRate * Time.deltaTime * ScoreMult * Time.timeScale;

			CurrentScore = Mathf.Lerp (CurrentScore, TargetScore, ScoreSmoothing * Time.unscaledDeltaTime);
			DisplayScore = Mathf.Round (CurrentScore);
			ScoreText.text = "" + DisplayScore;
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
				CountScore = true;
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
			CountScore = true;
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
}
