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
	public float TimeScaleView;
	public float FixedTimeStepView;
	public float MinimumTimeScale = 0.2f;
	public float MaximumTimeScale = 2.5f;

	[Header ("Time Manipulation")]
	public bool UpdateTargetTimeScale;
	public float TargetTimeScale = 1;
	public float TargetTimeScaleSmoothing = 10;
	public float TargetTimeScaleMult = 1;
	public float TargetTimeScaleAdd;
	public float TargetTimeScaleIncreaseRate = 0.01f;
	public Transform ReferencePoint;
	public float Distance;
	public Transform PlayerOne;
	public bool useTwoPlayers;
	public Transform PlayerTwo;

	public bool isInInitialSequence = true;

	public bool isInInitialCountdownSequence;
	public float InitialCountdownSequenceDuration = 3;
	public float InitialCountdownSequenceTimeRemaining = 3;

	[Header ("Override")]
	public bool isOverridingTimeScale;
	public float OverridingTimeScale;
	public float OverrideTimeScaleTimeRemaining;
	public float OverrideTimeScaleSmoothing = 10;

	public bool isEndSequence;
	public float EndSequenceInitialDelay;

	void Awake () 
	{
		Time.timeScale = MinimumTimeScale;
		Time.fixedDeltaTime = 0.005f;
	}

	void Update () 
	{
		CheckViewTimeProperties ();
		CheckTargetTimeScale ();
		CheckOverrideTimeScale ();

		if (localSceneLoaderScript.SceneLoadCommit == true) 
		{
			this.enabled = false;
			Time.timeScale = 1;
		}

		CheckInitialCountdownSequence ();
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
		if (useTwoPlayers == false &&
		    gameControllerScript.isPaused == false &&
		    Application.isFocused == true &&
			Application.isPlaying == true && 
			isEndSequence == false && 
			playerControllerScript_P1.isInCooldownMode == false) 
		{
			if (isOverridingTimeScale == false && isInInitialSequence == false && isInInitialCountdownSequence == false) 
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
			//Time.timeScale = Mathf.Clamp (Time.timeScale, 0, 1);
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
	}

	void CheckInitialCountdownSequence ()
	{
		if (isInInitialCountdownSequence == true) 
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
		}
	}

	public IEnumerator EndSequenceTimeScale ()
	{
		TargetTimeScale = 0.02f;
		yield return new WaitForSecondsRealtime (EndSequenceInitialDelay);
		TargetTimeScale = 1.5f;
		yield return new WaitForSecondsRealtime (EndSequenceInitialDelay);
		TargetTimeScale = 0;
		yield return new WaitForSecondsRealtime (EndSequenceInitialDelay);
		gameControllerScript.GameoverUI.SetActive (true);
		playerControllerScript_P1.cursorManagerScript.UnlockMouse ();
		playerControllerScript_P1.cursorManagerScript.ShowMouse ();
	}
}
