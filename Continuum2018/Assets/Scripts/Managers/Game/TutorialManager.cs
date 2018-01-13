using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
	public PlayerController playerControllerScript_P1;
	public TimescaleController timeScaleControllerScript;
	public GameController gameControllerScript;
	public bool tutorialComplete;
	public tutorialPhase TutorialPhase;
	public enum tutorialPhase
	{
		Movement = 0,
		Blocks = 1,
		Info = 2
	}

	public GameObject[] Blocks;

	public GameObject MovementObject;
	public GameObject ControlsObject;
	public GameObject BlocksObject;
	public GameObject InfoObject;

	void Start ()
	{
		tutorialComplete = false;
		MovementObject.SetActive (false);
		//ControlsObject.SetActive (true);
		BlocksObject.SetActive (false);
		InfoObject.SetActive (false);
		TutorialPhase = tutorialPhase.Movement;
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

	public void TurnOffTutorial ()
	{
		tutorialComplete = true;
		TutorialPhase = tutorialPhase.Info;
		InfoObject.SetActive (false);
		ControlsObject.SetActive (false);
		BlocksObject.SetActive (false);
		InfoObject.SetActive (false);
		Debug.Log ("Tutorial: Complete! Starting game.");
		timeScaleControllerScript.SwitchInitialSequence ();
		playerControllerScript_P1.StartCoroutines ();
		this.gameObject.SetActive (false);
	}
}
