﻿using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

using System.Collections;

using TMPro;

public class TimescaleController : MonoBehaviour 
{
	public static TimescaleController Instance { get; private set; }

	public NoiseAndGrain 		noiseScript;
	public GameModifierManager 	gameModifier;
	public MenuManager 			gameOverMenuManager;
	public SimpleFollow 		CamSimpleFollow;

	[Header ("Read Only")]
	[Tooltip ("Time.timeScale property.")]
	public float TimeScaleView; 
	[Tooltip ("Time.fixedDeltaTime property.")]
	public float FixedTimeStepView; 
	[Tooltip ("Time.timeScale should not go below this value.")]
	public float MinimumTimeScale = 0.2f;
	[Tooltip ("Time.timeScale should not go above this value.")]
	public float MaximumTimeScale = 2.5f; 

	[Header ("Time Manipulation")]
	public timeCalc TimeCalculation;
	public enum timeCalc
	{
		Continuous,
		Discrete
	}

	public TimescalePreset timeScalePreset;

	[Tooltip ("Should the time scale update by calculating distance?")]
	public bool UpdateTargetTimeScale;
	[Tooltip ("Time.timeScale will always try to transition to this value if UpdateTargetTimeScale is on.")]
	public float TargetTimeScale = 1;
	[Tooltip ("How much smoothing is needed for Time.timeScale to be TargetTimeScale.")]
	public float TargetTimeScaleSmoothing = 10;
	[Tooltip ("Multiplier of how much the TargetTimeScale changes based on distance.")]
	public float TargetTimeScaleMult = 1;
	[Tooltip ("How much Time.timeScale is added on top as a constant.")]
	public float TargetTimeScaleAdd;
	[Tooltip ("The rate of increasing targetTimeScaleAdd constant.")]
	public float TargetTimeScaleIncreaseRate = 0.01f;
	[Tooltip ("Where the position is to calculate the distance from the player to this point.")]
	public Transform ReferencePoint;
	[Tooltip ("Value as the Vector3.Distance from player and reference point.")]
	public float Distance;
	[Tooltip ("Location of the Player 1.")]
	public Transform PlayerOne;
	[Tooltip ("Multiplayer mode?")]
	public bool useTwoPlayers;
	[Tooltip ("Location of Player 2.")]
	public Transform PlayerTwo;
	[Tooltip ("Set to true as the tutorial finishes but set to false when the first wave starts.")]
	public bool isInInitialSequence = true;
	[Tooltip ("Time between end of tutorial and start of first wave transition.")]
	public bool isInInitialCountdownSequence;
	[Tooltip ("How long the countdown is set to start the first wave.")]
	public float InitialCountdownSequenceDuration = 3;
	[Tooltip ("How much time is remaining from the countdown.")]
	public float InitialCountdownSequenceTimeRemaining = 3;

	[Header ("Override")]
	[Tooltip ("When OverrideTimeScaleRemaining is > 0, this will be true.")]
	public bool isOverridingTimeScale;
	[Tooltip ("The value at which Time.timeScale will be when isOverridingTimeScale is true")]
	public float OverridingTimeScale;
	[Tooltip ("The amount of unscaled time left of overriding Time.timeScale.")]
	public float OverrideTimeScaleTimeRemaining;
	[Tooltip ("How quickly the current time scale moves towards the OverridingTimeScale.")]
	public float OverrideTimeScaleSmoothing = 10;
	[Tooltip ("Set to true when GameOver sequence happens.")]
	public bool isEndSequence;
	[Tooltip ("How long the initial part of the GaameOver sequence is delayed.")]
	public float EndSequenceInitialDelay;

	[Header ("Rewinding")]
	[Tooltip ("Doesn't override Time.timeScale directly, but gives the illusion of rewinding time;")]
	public bool isRewinding;
	[Tooltip ("How long the rewinding lasts for in real time.")]
	public float RewindTimeRemaining;
	[Tooltip ("Max rewinding time in real time.")]
	public float RewindDuration;

	[Header ("UI")]
	public Animator WaveTransitionUI;

	void Awake () 
	{
		Instance = this;

		Time.timeScale = MinimumTimeScale; // Set Time.timeScale to slowest possible value.
		Time.fixedDeltaTime = 0.005f; // Setting initial fixed time step.
	}

	public void OnStart ()
	{
		if (TimeCalculation == timeCalc.Continuous) 
		{
			InvokeRepeating ("UpdateTimeScalePreset", 0, 2);
		}

		if (PlayerController.PlayerTwoInstance == null)
		{
			PlayerController.PlayerOneInstance.transform.parent.GetComponent<PlayerParent> ().anim.enabled = true;
			PlayerController.PlayerOneInstance.transform.parent.GetComponent<PlayerParent> ().anim.Play ("PlayerEntry");
		}

		if (PlayerController.PlayerTwoInstance != null) 
		{
			useTwoPlayers = true;
			Debug.Log ("Two player mode active");

			PlayerController.PlayerOneInstance.transform.parent.GetComponent<PlayerParent> ().anim.enabled = true;
			PlayerController.PlayerOneInstance.transform.parent.GetComponent<PlayerParent> ().anim.Play ("PlayerEntryLeft");
			PlayerController.PlayerTwoInstance.transform.parent.GetComponent<PlayerParent> ().anim.enabled = true;
			PlayerController.PlayerTwoInstance.transform.parent.GetComponent<PlayerParent> ().anim.Play ("PlayerEntryRight");
		}
	}

	void UpdateTimeScalePreset ()
	{
		TargetTimeScaleMult = timeScalePreset.TimeScaleMult;
		TargetTimeScaleAdd = timeScalePreset.TimeScaleAdd;
		TargetTimeScaleIncreaseRate = timeScalePreset.TimeScaleIncreaseRate;
		TargetTimeScaleSmoothing = timeScalePreset.TimeScaleSmoothing;
	}

	void Update () 
	{
		CheckViewTimeProperties (); // Updates time variables to be displayed.
		CheckTargetTimeScale (); // Reads distance and applies target time scale.
		CheckOverrideTimeScale (); // Checks if it needs to override current time scale.
		CheckSceneLoadCommit (); // Checks if a scene is pending to load.
		CheckRewindTimeState (); // Checks for a rewinding.
	}

	void CheckViewTimeProperties ()
	{
		TimeScaleView = Time.timeScale; // Gets Time.timeScale property in Unity Time panel.
		FixedTimeStepView = Time.fixedDeltaTime; // Gets Fixed time step in Unity Time panel.
	}

	void CheckTargetTimeScale ()
	{
		if (UpdateTargetTimeScale == true && isEndSequence == false) 
		{
			// In normal circumstances.
			if (isOverridingTimeScale == false) 
			{
				// Smoothly interpolate the current Time.timeScale to the target time scale.
				Time.timeScale = Mathf.Lerp (Time.timeScale, TargetTimeScale, TargetTimeScaleSmoothing * Time.unscaledDeltaTime);
				UpdateMainTargetTimeScale ();
			}

			// When overriding time.
			if (isOverridingTimeScale == true) 
			{
				// Smoothly interpolate to the overriding time scale.
				Time.timeScale = Mathf.Lerp (Time.timeScale, OverridingTimeScale, OverrideTimeScaleSmoothing * Time.unscaledDeltaTime);
			}
		} 

		else 
		
		{
			// Just follow the target time scale.
			Time.timeScale = Mathf.Lerp (Time.timeScale, TargetTimeScale, TargetTimeScaleSmoothing * Time.unscaledDeltaTime);
		}
	}

	// Detect dsiatnce, apply time.
	void UpdateMainTargetTimeScale ()
	{
		if (GameController.Instance.isPaused == false && isEndSequence == false)
		{
			if (isOverridingTimeScale == false && isInInitialSequence == false && 
				isInInitialCountdownSequence == false && TutorialManager.Instance.tutorialComplete == true) 
			{
				if (useTwoPlayers == false && PlayerController.PlayerOneInstance.isInCooldownMode == false) 
				{
					float DistanceVector = Mathf.Clamp ( 
						Mathf.Abs (
							PlayerOne.transform.position.y -  
							ReferencePoint.transform.position.y
						),
						0,
						20
					);

					Distance = DistanceVector; 
				}

				if (useTwoPlayers == true && PlayerController.PlayerOneInstance.isInCooldownMode == false && 
					PlayerController.PlayerTwoInstance.isInCooldownMode == false) 
				{
					/*// Average out the distance between players.
					float AverageDistanceVector = Vector3.Distance (
						0.5f * (PlayerController.PlayerOneInstance.transform.position + PlayerController.PlayerTwoInstance.transform.position), 
						ReferencePoint.transform.position
					);

					Distance = AverageDistanceVector;*/

					float DistanceA = Vector3.Distance (
						PlayerOne.transform.position, 
						ReferencePoint.transform.position
					);

					float DistanceB = Vector3.Distance (
						PlayerTwo.transform.position, 
						ReferencePoint.transform.position
					);
						
					if (DistanceA > DistanceB) 
					{
						Distance = DistanceA;
					}

					if (DistanceB > DistanceA) 
					{
						Distance = DistanceB;
					}
				}

				// Checks for game modifier time increasing mode over real time.
				switch (gameModifier.TimeIncreaseMode)
				{
					// Increase minimum time scale normally.
					case GameModifierManager.timeIncreaseMode.Normal:
						TargetTimeScaleAdd += (TargetTimeScaleIncreaseRate * Time.unscaledDeltaTime);
						break;
					// Increase minimum time scale faster.
					case GameModifierManager.timeIncreaseMode.Fast:
						TargetTimeScaleAdd += (TargetTimeScaleIncreaseRate * Time.unscaledDeltaTime * 2);
						break;
					// Increase minimum time scale slower.
					case GameModifierManager.timeIncreaseMode.Slow:
						TargetTimeScaleAdd += (TargetTimeScaleIncreaseRate * Time.unscaledDeltaTime * 0.5f);
						break;
					// Dont increase minimum time scale normally.
					case GameModifierManager.timeIncreaseMode.Off:
						break;
				}
					
				// Updates fixed time step based on time scale. (Current period: 1/200 of a second, 200Hz).
				// Physics updates must be this fast to maintain accuracy.
				Time.fixedDeltaTime = Time.timeScale * 0.005f; // gets called 200 times per second.
				//Time.maximumParticleDeltaTime = Time.timeScale * 0.005f;
				Time.maximumParticleDeltaTime = 0.05f;
			}

			// When overriding time scale.
			if (isOverridingTimeScale == true) 
			{
				if (Time.timeScale <= MinimumTimeScale) 
				{
					// Updates fixed time step based on time scale. (Current period: 1/1000 of a second, 1000Hz).
					Time.fixedDeltaTime = Time.timeScale * 0.001f;
					Time.maximumParticleDeltaTime = Time.timeScale * 0.001f;
				}
			}
		} 

		// Time is not overriding. 
		if (isOverridingTimeScale == false 
			&& isInInitialSequence == false && isInInitialCountdownSequence == false) 
		{
			if (useTwoPlayers == false && PlayerController.PlayerOneInstance.isInCooldownMode == false) 
			{
				float DistanceVector = Vector3.Distance (PlayerOne.transform.position, ReferencePoint.transform.position);
				Distance = DistanceVector;
			}

			if (TimeCalculation == timeCalc.Continuous) 
			{
				// Set TargetTimeScale with multiplier, distance, minimum timescale, clamp to min and max values.
				TargetTimeScale = Mathf.Clamp (
					TargetTimeScaleMult * Distance + TargetTimeScaleAdd, 
					MinimumTimeScale, 
					MaximumTimeScale
				);

				TargetTimeScale = Mathf.Clamp (TargetTimeScaleMult * Distance, MinimumTimeScale, MaximumTimeScale);
			}

			if (TimeCalculation == timeCalc.Discrete) 
			{
				// Get pitch value from bass track in audio controller and allow manipulation.
				TargetTimeScale = Mathf.Clamp (
					//AudioController.Instance.BassTrack.pitch + TargetTimeScaleAdd - 0.3f, 
					AudioController.Instance.LayerSources[0].pitch + TargetTimeScaleAdd, 
					MinimumTimeScale, 
					MaximumTimeScale
				);
			}
		}
	}

	// What to do when override time scale is active.
	void CheckOverrideTimeScale ()
	{
		if (PlayerController.PlayerOneInstance.timeIsSlowed == true) 
		{
			OverrideTimeScaleTimeRemaining = GameController.Instance.PowerupTimeRemaining;
			GameController.Instance.isUpdatingImageEffects = false;

			var saturationSettings = GameController.Instance.ImageEffects.colorGrading.settings;
			saturationSettings.basic.saturation = Mathf.Lerp (saturationSettings.basic.saturation, 0.2f, Time.unscaledDeltaTime);
			GameController.Instance.ImageEffects.colorGrading.settings = saturationSettings;
		}

		if (OverrideTimeScaleTimeRemaining <= 0) 
		{
			if (isOverridingTimeScale == true) 
			{
				isOverridingTimeScale = false;
			}
				
			PlayerController.PlayerOneInstance.SmoothFollowTime = 15;

			if (PlayerController.PlayerTwoInstance != null)
			{
				PlayerController.PlayerTwoInstance.SmoothFollowTime = 15;
			}

			return;
		}

		// Time is overriding.
		if (OverrideTimeScaleTimeRemaining > 0) 
		{
			// Normal overriding curcumstances.
			if (GameController.Instance.isPaused == false && 
				isInInitialSequence == false && 
				isInInitialCountdownSequence == false) 
			{
				OverrideTimeScaleTimeRemaining -= Time.unscaledDeltaTime; // Decrease override time remaining unscaled.
			}

			// Keep overriding time scale false if theres no time for it.
			if (isOverridingTimeScale == false)
			{
				isOverridingTimeScale = true;
			}
				
			// If game is not in a wave transition.
			if (WaveTransitionUI.GetCurrentAnimatorStateInfo (0).IsName ("WaveTransition") == false) 
			{
				if (isOverridingTimeScale == false) 
				{
					isOverridingTimeScale = true;
				}

				// Decrease sensitivity of player movement.
				PlayerController.PlayerOneInstance.MovementX *= Time.timeScale;
				PlayerController.PlayerOneInstance.MovementY *= Time.timeScale;
				PlayerController.PlayerOneInstance.SmoothFollowTime = 2 / Time.timeScale;

				if (PlayerController.PlayerTwoInstance != null)
				{
					PlayerController.PlayerTwoInstance.MovementX *= Time.timeScale;
					PlayerController.PlayerTwoInstance.MovementY *= Time.timeScale;
					PlayerController.PlayerTwoInstance.SmoothFollowTime = 2 / Time.timeScale;
				}

				return;
			}

			// Check state of wave transition UI.
			if (WaveTransitionUI.GetCurrentAnimatorStateInfo (0).IsName ("WaveTransition") == true
				&& GameController.Instance.isPaused == false) 
			{
				if (isOverridingTimeScale == true) 
				{
					isOverridingTimeScale = false;
				}

				PlayerController.PlayerOneInstance.SmoothFollowTime = 15;

				if (PlayerController.PlayerTwoInstance != null) 
				{
					PlayerController.PlayerTwoInstance.SmoothFollowTime = 15;
				}

				return;
			}
		}
	}

	// Give initial delay between turorial and start of first wave. 
	public void SwitchInitialSequence ()
	{
		Time.timeScale = 1; // Hard reset Time.timeScale.
		isInInitialSequence = false;
		isInInitialCountdownSequence = true;
		StartCoroutine (CheckInitialCountdownSequence ());
	}

	// Timer to start first wave.
	IEnumerator CheckInitialCountdownSequence ()
	{
		while (isInInitialCountdownSequence == true) 
		{
			if (InitialCountdownSequenceTimeRemaining > 0) 
			{
				InitialCountdownSequenceTimeRemaining -= Time.unscaledDeltaTime;
			}

			if (InitialCountdownSequenceTimeRemaining <= 0) 
			{
				Time.timeScale = 1;
				isInInitialCountdownSequence = false;
				GameController.Instance.CountScore = true;
				GameController.Instance.NextLevel ();
				GameController.Instance.StartGame ();
			}

			yield return null;
		}
	}

	// In Game Over state, sequence for end game.
	public IEnumerator EndSequenceTimeScale ()
	{
		var SaturationSettings = GameController.Instance.ImageEffects.colorGrading.settings;
		SaturationSettings.basic.saturation = 1;
		GameController.Instance.ImageEffects.colorGrading.settings = SaturationSettings;

		PlayerController.PlayerOneInstance.GlitchEffect.Play ("CameraGlitchOn");

		TargetTimeScale = 0.02f;
		yield return new WaitForSecondsRealtime (EndSequenceInitialDelay);
		TargetTimeScale = 1f;
		yield return new WaitForSecondsRealtime (EndSequenceInitialDelay);
		TargetTimeScale = 0;
		yield return new WaitForSecondsRealtime (EndSequenceInitialDelay);

		SaturationSettings.basic.saturation = 1;
		GameController.Instance.ImageEffects.colorGrading.settings = SaturationSettings;

		PlayerController.PlayerOneInstance.GlitchEffect.Play ("CameraGlitchDefault");

		GameOverController.Instance.transform.parent.gameObject.SetActive (true);
		GameOverController.Instance.enabled = true;
		GameOverController.Instance.CheckLeaderboard ();
		CamSimpleFollow.enabled = false;

		gameOverMenuManager.menuButtons.buttonIndex = 0;
		gameOverMenuManager.MenuOnEnter (gameOverMenuManager.menuButtons.buttonIndex);

		// Show mouse and allow control for player.
		CursorManager.Instance.UnlockMouse ();
		CursorManager.Instance.ShowMouse ();

		#if !UNITY_ANDROID && !PLATFORM_WEBGL
		PlayerController.PlayerOneInstance.Vibrate (0, 0, 0);

		if (PlayerController.PlayerTwoInstance != null) 
		{
			PlayerController.PlayerTwoInstance.Vibrate (0, 0, 0);
		}
		#endif
	}

	// Rewinding functionality to be called by ability.
	public void SetRewindTime (bool _isRewinding, float rewindTime)
	{
		isRewinding = _isRewinding;
		RewindDuration = rewindTime;
		RewindTimeRemaining = RewindDuration;

		PlayerController.PlayerOneInstance.InvincibleParticles.Play ();
		PlayerController.PlayerOneInstance.InvincibleParticles.Play ();

		if (PlayerController.PlayerTwoInstance != null) 
		{
			PlayerController.PlayerTwoInstance.InvincibleParticles.Play ();
			PlayerController.PlayerTwoInstance.InvincibleParticles.Play ();
		}
	}

	// Timer for rewind state.
	void CheckRewindTimeState ()
	{
		if (RewindTimeRemaining <= 0 && isRewinding == true) 
		{
			isRewinding = false;
			noiseScript.enabled = false;
			PlayerController.PlayerOneInstance.InvincibleCollider.enabled = false;
			PlayerController.PlayerOneInstance.InvincibleParticles.Stop (true, ParticleSystemStopBehavior.StopEmitting);

			if (PlayerController.PlayerTwoInstance != null) 
			{
				PlayerController.PlayerTwoInstance.InvincibleCollider.enabled = false;
				PlayerController.PlayerTwoInstance.InvincibleParticles.Stop (true, ParticleSystemStopBehavior.StopEmitting);
			}
			return;
		}

		if (RewindTimeRemaining > 0) 
		{
			isRewinding = true;
			RewindTimeRemaining -= Time.unscaledDeltaTime;
			noiseScript.enabled = true;
			PlayerController.PlayerOneInstance.InvincibleCollider.enabled = true;

			if (PlayerController.PlayerTwoInstance != null)
			{
				PlayerController.PlayerTwoInstance.InvincibleCollider.enabled = true;
			}
		} 
	}

	// Check for a scene pending to load.
	// If scene wants to load, bail this script.
	void CheckSceneLoadCommit ()
	{
		// Checks to see if the scene loader wants to load another scene.
		if (LocalSceneLoader.Instance.SceneLoadCommit == true) 
		{
			Time.timeScale = 0;
			this.enabled = false;
		}
	}
}