using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveTransition : MonoBehaviour 
{
	public GameController gameControllerScript;
	public GameObject WaveTransitionUI;
	public GameObject SoundtrackUI;

	void Start ()
	{
		WaveTransitionUI.SetActive (false);
		SoundtrackUI.SetActive (false);
	}

	public void DeactivateWaveTransition ()
	{
		gameControllerScript.IsInWaveTransition = false;

		if (gameControllerScript.Wave % 5 == 1 || gameControllerScript.Wave == 1) 
		{
			WaveTransitionUI.SetActive (false);
			SoundtrackUI.SetActive (false);
		}
	}

	public void ActivateWaveTransitionUI ()
	{
		gameControllerScript.IsInWaveTransition = true;

		if (gameControllerScript.Wave % 5 == 1 || gameControllerScript.Wave == 1)
		{
			WaveTransitionUI.SetActive (true);
			SoundtrackUI.SetActive (true);
		}
	}
}
