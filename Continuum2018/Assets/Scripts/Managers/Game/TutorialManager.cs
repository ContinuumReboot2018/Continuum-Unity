using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
	public PlayerController playerControllerScript_P1; // Reference to Player Controller.
	public GameController gameControllerScript; // Reference to Game Controller.
	public TimescaleController timeScaleControllerScript; // Reference to Timescale Controller.

	[Header ("Tutorial Stats")]
	public bool tutorialComplete; // Has the tutorial been completed?
	public tutorialPhase TutorialPhase; // Current tutorial phase.
	public enum tutorialPhase
	{
		Movement = 0,
		Blocks = 1,
		Info = 2
	}

	public GameObject[] Blocks; // Array of blocks for the playr to destroy.

	public GameObject MovementObject; // Movement section.
	public GameObject ControlsObject; // Controls section.
	public GameObject BlocksObject; // Blocks section.
	public GameObject InfoObject; // More info section.

	void Start ()
	{
		// Set all objects to false.
		tutorialComplete = false;
		MovementObject.SetActive (false);
		ControlsObject.SetActive (false);
		BlocksObject.SetActive (false);
		InfoObject.SetActive (false);

		TutorialPhase = tutorialPhase.Movement; // Set to first tutorial sequence.
		StartCoroutine (DeactivateMovementDelay ());
	}

	void Update ()
	{
		if (TutorialPhase == tutorialPhase.Blocks)
		{
			if (Blocks[0] == null && 
				Blocks[1] == null && 
				Blocks[2] == null && 
				Blocks[3] == null)
			{
				TurnOffPhaseTemp ();
				TutorialPhase = tutorialPhase.Info;
			}
		}
	}

	void TurnOffPhaseTemp ()
	{
		switch (TutorialPhase) 
		{
		case tutorialPhase.Movement:
			MovementObject.SetActive (false);
			ControlsObject.SetActive (false);
			break;
		case tutorialPhase.Blocks:
			BlocksObject.SetActive (false);
			break;
		case tutorialPhase.Info:
			InfoObject.SetActive (true);
			break;
		}

		Invoke ("MoveToNextPhase", 1);
	}

	void MoveToNextPhase ()
	{
		if (TutorialPhase != tutorialPhase.Info) 
		{
			TutorialPhase += 1;
		}

		switch (TutorialPhase) 
		{
		case tutorialPhase.Movement:
			MovementObject.SetActive (true);
			ControlsObject.SetActive (true);
			BlocksObject.SetActive (false);
			InfoObject.SetActive (false);
			Debug.Log ("Tutorial: Movement phase.");
			break;
		case tutorialPhase.Blocks:
			MovementObject.SetActive (false);
			ControlsObject.SetActive (false);
			BlocksObject.SetActive (true);
			InfoObject.SetActive (false);
			Debug.Log ("Tutorial: Blocks phase.");
			break;
		case tutorialPhase.Info:
			MovementObject.SetActive (false);
			BlocksObject.SetActive (false);
			InfoObject.SetActive (true);
			Debug.Log ("Tutorial: Info phase.");
			break;
		}
	}

	IEnumerator DeactivateMovementDelay ()
	{
		yield return new WaitForSecondsRealtime (5);
		MovementObject.SetActive (true);
		yield return new WaitForSecondsRealtime (5);
		TurnOffPhaseTemp ();
	}

	public void TurnOffTutorial (bool wasSkipped)
	{
		tutorialComplete = true;
		TutorialPhase = tutorialPhase.Info;
		InfoObject.SetActive (false);
		ControlsObject.SetActive (false);
		BlocksObject.SetActive (false);
		MovementObject.SetActive (false);

		if (wasSkipped == false) 
		{
			Debug.Log ("Completed tutorial. Starting waves.");
		}

		if (wasSkipped == true) 
		{
			Debug.Log ("Skipped tutorial. Starting waves.");
		}

		timeScaleControllerScript.SwitchInitialSequence ();
		playerControllerScript_P1.StartCoroutines ();

		foreach (GameObject block in Blocks) 
		{
			Destroy (block);
		}

		this.gameObject.SetActive (false);
		return;
	}
}