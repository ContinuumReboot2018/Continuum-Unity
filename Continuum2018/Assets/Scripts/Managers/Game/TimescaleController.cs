using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityStandardAssets.ImageEffects;

public class TimescaleController : MonoBehaviour 
{
	public PlayerController 	playerControllerScript_P1;
	public GameController 		gameControllerScript;
	public LocalSceneLoader 	localSceneLoaderScript;
	public FPSCounter 			fpsCounterScript;
	public NoiseAndGrain 		noiseScript;
	public GameModifierManager 	gameModifier;

	[Header ("Read Only")]
	public float TimeScaleView; 			// Time.timeScale property.
	public float FixedTimeStepView; 		// Time.fixedDeltaTime property.
	public float MinimumTimeScale = 0.2f;	// Time.timeScale should not go below this value.
	public float MaximumTimeScale = 2.5f; 	// Time.timeScale should not go above this value.

	[Header ("Time Manipulation")]
	public bool UpdateTargetTimeScale;
	public float TargetTimeScale = 1; 					// Time.timeScale will always try to transition to this value if UpdateTargetTimeScale is on.
	public float TargetTimeScaleSmoothing = 10; 		// How much smoothing is needed for Time.timeScale to be TargetTimeScale.
	public float TargetTimeScaleMult = 1; 				// Multiplier of how much the TargetTimeScale changes based on distance.
	public float TargetTimeScaleAdd; 					// How much Time.timeScale is added on top as a constant.
	public float TargetTimeScaleIncreaseRate = 0.01f; 	// The rate of increasing targetTimeScaleAdd constant.
	public Transform ReferencePoint; 					// Where the position is to calculate the distance from the player to this point.
	public float Distance; 								// Value as the Vector3.Distance from player and reference point.

	public Transform PlayerOne;
	public bool useTwoPlayers;
	public Transform PlayerTwo;

	public bool isInInitialSequence = true; 				// Set to true as the tutorial finishes but set to false when the first wave starts.
	public bool isInInitialCountdownSequence; 				// Time between end of tutorial and start of first wave transition.
	public float InitialCountdownSequenceDuration = 3;		// How long the countdown is set to start the first wave.
	public float InitialCountdownSequenceTimeRemaining = 3; // How much time is remaining from the countdown.

	[Header ("Override")]
	public bool isOverridingTimeScale; 			  // When OverrideTimeScaleRemaining is > 0, this will be true.
	public float OverridingTimeScale;			  // The value at which Time.timeScale will be when isOverridingTimeScale is true;
	public float OverrideTimeScaleTimeRemaining;  // The amount of unscaled time left of overriding Time.timeScale.
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
		if (useTwoPlayers == false && gameControllerScript.isPaused == false && 
			isEndSequence == false && playerControllerScript_P1.isInCooldownMode == false) 
		{
			if (isOverridingTimeScale == false && isInInitialSequence == false && 
				isInInitialCountdownSequence == false && playerControllerScript_P1.tutorialManagerScript.tutorialComplete == true) 
			{
				// Gets vertical distance from player to reference point.
				// Distance = PlayerOne.transform.position.y - ReferencePoint.position.y;
				// Get distance but only use Y component.
				float DistanceVector = Vector3.Distance (PlayerOne.transform.position, ReferencePoint.transform.position);
				Distance = DistanceVector;

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

				// Set TargetTimeScale with multiplier, distance, minimum timescale, clamp to min and max values.
				TargetTimeScale = Mathf.Clamp (TargetTimeScaleMult * Distance + TargetTimeScaleAdd, MinimumTimeScale, MaximumTimeScale);
					
				// Updates fixed time step based on time scale. (Current period: 1/200 of a second, 200Hz).
				// Physics updates must be this fast to maintain accuracy.
				Time.fixedDeltaTime = Time.timeScale * 0.005f;
				//Time.fixedDeltaTime = Time.timeScale * 0.01f;
			}

			// When overriding time scale.
			if (isOverridingTimeScale == true) 
			{
				if (Time.timeScale <= MinimumTimeScale) 
				{
					// Updates fixed time step based on time scale. (Current period: 1/1000 of a second, 1000Hz).
					Time.fixedDeltaTime = Time.timeScale * 0.001f;
				}
			}
		} 

		else 
		
		{
			// Gets vertical distance from player to reference point. (Default).
			// Distance = PlayerOne.transform.position.y - ReferencePoint.position.y;
			// Get distance but only use Y component.
			float DistanceVector = Vector3.Distance (PlayerOne.transform.position, ReferencePoint.transform.position);
			Distance = DistanceVector;
			TargetTimeScale = Mathf.Clamp (TargetTimeScaleMult * Distance, MinimumTimeScale, MaximumTimeScale);
		}
	}

	// What to do when override time scale is active.
	void CheckOverrideTimeScale ()
	{
		if (OverrideTimeScaleTimeRemaining <= 0) 
		{
			if (isOverridingTimeScale == true) 
			{
				isOverridingTimeScale = false;
			}

			return;
		}

		if (OverrideTimeScaleTimeRemaining > 0) 
		{
			// Normal overriding curcumstances.
			if (gameControllerScript.isPaused == false && 
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

			playerControllerScript_P1.MovementX = 0;
			playerControllerScript_P1.MovementY = 0;
			playerControllerScript_P1.PlayerRb.velocity = Vector3.zero;
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
				gameControllerScript.CountScore = true;
				isInInitialCountdownSequence = false;
				gameControllerScript.NextLevel ();
				gameControllerScript.StartGame ();
			}

			yield return null;
		}
	}

	// In Game Over state, sequence for end game.
	public IEnumerator EndSequenceTimeScale ()
	{
		TargetTimeScale = 0.02f;
		yield return new WaitForSecondsRealtime (EndSequenceInitialDelay);
		TargetTimeScale = 1f;
		yield return new WaitForSecondsRealtime (EndSequenceInitialDelay);
		TargetTimeScale = 0;
		yield return new WaitForSecondsRealtime (EndSequenceInitialDelay);

		gameControllerScript.GameoverUI.SetActive (true);

		// Show mouse and allow control for player.
		playerControllerScript_P1.cursorManagerScript.UnlockMouse ();
		playerControllerScript_P1.cursorManagerScript.ShowMouse ();
	}

	// Rewinding functionality to be called by ability.
	public void SetRewindTime (bool _isRewinding, float rewindTime)
	{
		isRewinding = _isRewinding;
		RewindDuration = rewindTime;
		RewindTimeRemaining = RewindDuration;
	}

	// Timer for rewind state.
	void CheckRewindTimeState ()
	{
		if (RewindTimeRemaining <= 0) 
		{
			isRewinding = false;
			noiseScript.enabled = false;
			playerControllerScript_P1.InvincibleCollider.enabled = false;
			playerControllerScript_P1.InvincibleMesh.enabled = false;
			//playerControllerScript_P1.InvincibleMeshAnim.Play ("InvincibleMeshFlash");
			return;
		}

		if (RewindTimeRemaining > 0) 
		{
			isRewinding = true;
			RewindTimeRemaining -= Time.unscaledDeltaTime;
			noiseScript.enabled = true;
			playerControllerScript_P1.InvincibleCollider.enabled = true;
			playerControllerScript_P1.InvincibleMesh.enabled = true;
			playerControllerScript_P1.InvincibleMeshAnim.Play ("InvincibleMeshFlash");
		} 

		else 
		
		{
			isRewinding = false;
			noiseScript.enabled = false;
		}
	}

	// Check for a scene pending to load.
	// If scene wants to load, bail this script.
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
