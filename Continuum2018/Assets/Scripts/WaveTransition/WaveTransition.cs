using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveTransition : MonoBehaviour 
{
	public GameController gameControllerScript;
	public GameObject WaveTransitionUI;

	public void DeactivateWaveTransition ()
	{
		gameControllerScript.IsInWaveTransition = false;
		WaveTransitionUI.SetActive (false);
	}

	public void ActivateWaveTransitionUI ()
	{
		gameControllerScript.IsInWaveTransition = true;
		WaveTransitionUI.SetActive (true);
	}
}
