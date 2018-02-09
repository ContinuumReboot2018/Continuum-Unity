using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimescaleController : MonoBehaviour 
{
	public PlayerController playerControllerScript_P1;
	public GameController gameControllerScript;
	public LocalSceneLoader localSceneLoaderScript;
	public FPSCounter fpsCounterScript;

	[Header ("Read Only")]
	public float TimeScaleView; // Time.timeScale property.
	public float FixedTimeStepView; // Time.fixedDeltaTime property.
	public float MinimumTimeScale = 0.2f; // Time.timeScale should not go below this value.
	public float MaximumTimeScale = 2.5f; // Time.timeScale should not go above this value.

	[Header ("Time Manipulation")]
	public bool UpdateTargetTimeScale;
	public float TargetTimeScale = 1; // Time.timeScale will always try to transition to this value if UpdateTargetTimeScale is on.
	public float TargetTimeScaleSmoothing = 10; // How much smoothing is needed for Time.timeScale to be TargetTimeScale.
	public float TargetTimeScaleMult = 1; // Multiplier of how much the TargetTimeScale changes based on distance.
	public float TargetTimeScaleAdd; // How much Time.timeScale is added on top as a constant.
	public float TargetTimeScaleIncreaseRate = 0.01f; // The rate of increasing targetTimeScaleAdd constant.
	public Transform ReferencePoint; // Where the position is to calculate the distance from the player to this point.
	public float Distance; // Value as the Vector3.Distance from player and reference point.

	public Transform PlayerOne;
	public bool useTwoPlayers;
	public Transform PlayerTwo;

	public bool isInInitialSequence = true; // Set to true as the tutorial finishes but set to false when the first wave starts.
	public bool isInInitialCountdownSequence; // Time between end of tutorial and start of first wave transition.
	public float InitialCountdownSequenceDuration = 3; // How long the countdown is set to start the first wave.
	public float InitialCountdownSequenceTimeRemaining = 3; // How much time is remaining from the countdown.

	[Header ("Override")]
	public bool isOverridingTimeScale; // When OverrideTimeScaleRemaining is > 0, this will be true.
	public float OverridingTimeScale; // The value at which Time.timeScale will be when isOverridingTimeScale is true;
	public float OverrideTimeScaleTimeRemaining; // The amount of unscaled time left of overriding Time.timeScale.
	public float OverrideTimeScaleSmoothing = 10; // How quickly the current time scale moves towards the OverridingTimeScale.

	public bool isEndSequence; // Set to true when GameOver sequence happens.
	public float EndSequenceInitialDelay; // How long the initial part of the GaameOver sequence is delayed.

	// Doesn't override Time.timeScale directly, but gives the illusion of rewinding time;
	public bool isRewinding;
	public float RewindTimeRemaining;
	public float RewindDuration;

	void Awake () 
	{
		Time.timeScale = MinimumTimeScale; // Set Time.timeScale to slowest possible value.
		Time.fixedDeltaTime = 0.005f; // Setting initial fixed time step.
	}

	void Update () 
	{
		CheckViewTimeProperties ();
		CheckTargetTimeScale ();
		CheckOverrideTimeScale ();
		CheckSceneLoadCommit ();
		CheckRewindTimeState ();
	}

	void CheckViewTimeProperties ()
	{
		TimeScaleView = Time.timeScale;
		FixedTimeStepView = Time.fixedDeltaTime;
	}

	void CheckTargetTimeScale ()
	{
		if (UpdateTargetTimeScale == true && isEndSequence == false) 
		{
			if (isOverridingTimeScale == false) 
			{
				Time.timeScale = Mathf.Lerp (Time.timeScale, TargetTimeScale, TargetTimeScaleSmoothing * Time.unscaledDeltaTime);
				UpdateMainTargetTimeScale ();
			}

			if (isOverridingTimeScale == true) 
			{
				Time.timeScale = Mathf.Lerp (Time.timeScale, OverridingTimeScale, OverrideTimeScaleSmoothing * Time.unscaledDeltaTime);
			}
		} else {
			Time.timeScale = Mathf.Lerp (Time.timeScale, TargetTimeScale, TargetTimeScaleSmoothing * Time.unscaledDeltaTime);
		}
	}

	void UpdateMainTargetTimeScale ()
	{
		if (useTwoPlayers == false && gameControllerScript.isPaused == false && 
			isEndSequence == false && playerControllerScript_P1.isInCooldownMode == false) 
		{
			if (isOverridingTimeScale == false && isInInitialSequence == false && 
				isInInitialCountdownSequence == false && playerControllerScript_P1.tutorialManagerScript.tutorialComplete == true) 
			{
				Distance = PlayerOne.transform.position.y - ReferencePoint.position.y;

				TargetTimeScaleAdd += TargetTimeScaleIncreaseRate * Time.unscaledDeltaTime;
				TargetTimeScale = Mathf.Clamp (TargetTimeScaleMult * Distance + TargetTimeScaleAdd, MinimumTimeScale, MaximumTimeScale);
					
				// Updates fixed time step based on time scale.
				Time.fixedDeltaTime = Time.timeScale * 0.005f;
			}

			if (isOverridingTimeScale == true) 
			{
				if (Time.timeScale <= MinimumTimeScale) 
				{
					Time.fixedDeltaTime = Time.timeScale * 0.001f;
				}
			}
		} else 
		{
			Distance = PlayerOne.transform.position.y - ReferencePoint.position.y;
			TargetTimeScale = Mathf.Clamp (TargetTimeScaleMult * Distance, MinimumTimeScale, MaximumTimeScale);
		}
	}

	void CheckOverrideTimeScale ()
	{
		if (OverrideTimeScaleTimeRemaining > 0) 
		{
			if (gameControllerScript.isPaused == false && isInInitialSequence == false && isInInitialCountdownSequence == false) 
			{
				OverrideTimeScaleTimeRemaining -= Time.unscaledDeltaTime;
			}

			if (isOverridingTimeScale == false) 
			{
				isOverridingTimeScale = true;
			}

			playerControllerScript_P1.MovementX = 0;
			playerControllerScript_P1.MovementY = 0;
			playerControllerScript_P1.PlayerRb.velocity = Vector3.zero;
		}

		if (OverrideTimeScaleTimeRemaining <= 0) 
		{
			if (isOverridingTimeScale == true) 
			{
				isOverridingTimeScale = false;
			}
		}
	}

	public void SwitchInitialSequence ()
	{
		Time.timeScale = 1;
		isInInitialSequence = false;
		isInInitialCountdownSequence = true;
		StartCoroutine (CheckInitialCountdownSequence ());
	}

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
				gameControllerScript.CountScore = true;
				isInInitialCountdownSequence = false;
				gameControllerScript.NextLevel ();
				gameControllerScript.StartGame ();
			}

			yield return null;
		}
	}

	public IEnumerator EndSequenceTimeScale ()
	{
		TargetTimeScale = 0.02f;
		yield return new WaitForSecondsRealtime (EndSequenceInitialDelay);
		TargetTimeScale = 1f;
		yield return new WaitForSecondsRealtime (EndSequenceInitialDelay);
		TargetTimeScale = 0;
		yield return new WaitForSecondsRealtime (EndSequenceInitialDelay);
		gameControllerScript.GameoverUI.SetActive (true);
		playerControllerScript_P1.cursorManagerScript.UnlockMouse ();
		playerControllerScript_P1.cursorManagerScript.ShowMouse ();
	}

	public void SetRewindTime (bool _isRewinding, float rewindTime)
	{
		isRewinding = _isRewinding;
		RewindDuration = rewindTime;
		RewindTimeRemaining = RewindDuration;
	}

	void CheckRewindTimeState ()
	{
		if (RewindTimeRemaining > 0) {
			isRewinding = true;
			RewindTimeRemaining -= Time.unscaledDeltaTime;
		} else {
			isRewinding = false;
		}
	}

	void CheckSceneLoadCommit ()
	{
		// Checks to see if the scene loader wants to load another scene.
		if (localSceneLoaderScript.SceneLoadCommit == true) 
		{
			Time.timeScale = 0;
			this.enabled = false;
		}
	}
}
