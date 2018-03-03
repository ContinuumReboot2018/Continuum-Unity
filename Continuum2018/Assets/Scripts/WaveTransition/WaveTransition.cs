using UnityEngine;

public class WaveTransition : MonoBehaviour 
{
	public GameController gameControllerScript; // Reference to Game Controller.
	public GameObject WaveTransitionUI; // Wave Transition UI GameObject.
	public GameObject SoundtrackUI; // UI for soundtrack title.

	void Start ()
	{
		// Disable UI on start.
		WaveTransitionUI.SetActive (false);
		SoundtrackUI.SetActive (false);
	}

	// Turn off wave transition UI based on wave number.
	public void DeactivateWaveTransition ()
	{
		gameControllerScript.IsInWaveTransition = false;

		if (gameControllerScript.Wave % 4 == 1 || gameControllerScript.Wave == 1) 
		{
			WaveTransitionUI.SetActive (false);
			SoundtrackUI.SetActive (false);
		}
	}

	// Turn on wave transition UI based on wave number.
	public void ActivateWaveTransitionUI ()
	{
		gameControllerScript.IsInWaveTransition = true;

		if (gameControllerScript.Wave % 4 == 1 || gameControllerScript.Wave == 1)
		{
			WaveTransitionUI.SetActive (true);
			SoundtrackUI.SetActive (true);
		}
	}
}