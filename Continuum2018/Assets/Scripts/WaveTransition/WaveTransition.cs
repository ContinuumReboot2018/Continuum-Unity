using UnityEngine;

public class WaveTransition : MonoBehaviour 
{
	[Tooltip ("Reference to Game Controller.")]
	public GameController gameControllerScript;
	[Tooltip ("Wave Transition UI GameObject.")]
	public GameObject WaveTransitionUI;
	[Tooltip ("UI for soundtrack title.")]
	public GameObject SoundtrackUI;

	void Start ()
	{
		// Disable UI on start.
		//WaveTransitionUI.SetActive (false);
		//SoundtrackUI.SetActive (false);
	}

	// Turn off wave transition UI based on wave number.
	public void DeactivateWaveTransition ()
	{
		gameControllerScript.IsInWaveTransition = false;

		if (gameControllerScript.Wave % 4 == 1 || gameControllerScript.Wave == 1) 
		{
			//WaveTransitionUI.SetActive (false);
			//SoundtrackUI.SetActive (false);
			gameControllerScript.WaveTransitionUIStats.Play ("WaveTransitionUIStatsExit");
		}
	}

	// Turn on wave transition UI based on wave number.
	public void ActivateWaveTransitionUI ()
	{
		gameControllerScript.IsInWaveTransition = true;

		if (gameControllerScript.Wave % 4 == 1 || gameControllerScript.Wave == 1)
		{
			gameControllerScript.WaveTransitionUIStats.Play ("WaveTransitionUIStats");
			//WaveTransitionUI.SetActive (true);
			//SoundtrackUI.SetActive (true);
		}
	}
}