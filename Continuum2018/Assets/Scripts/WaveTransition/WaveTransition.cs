using UnityEngine;

public class WaveTransition : MonoBehaviour 
{
	[Tooltip ("Reference to Game Controller.")]
	public GameController gameControllerScript;
	[Tooltip ("Wave Transition UI GameObject.")]
	public GameObject WaveTransitionUI;
	[Tooltip ("UI for soundtrack title.")]
	public GameObject SoundtrackUI;

	// Turn off wave transition UI based on wave number.
	public void DeactivateWaveTransition ()
	{
		gameControllerScript.IsInWaveTransition = false;

		if (gameControllerScript.Wave % 4 == 1 || gameControllerScript.Wave == 1) 
		{
			SoundtrackUI.SetActive (true);
			gameControllerScript.WaveTransitionUIStats.Play ("WaveTransitionUIStatsExit");
		}

		if (gameControllerScript.Wave % 4 != 1) 
		{
			SoundtrackUI.SetActive (false);
		}
	}

	// Turn on wave transition UI based on wave number.
	public void ActivateWaveTransitionUI ()
	{
		gameControllerScript.IsInWaveTransition = true;

		if (gameControllerScript.Wave % 4 == 1 || gameControllerScript.Wave != 1) 
		{
			SoundtrackUI.SetActive (true);
			gameControllerScript.WaveTransitionUIStats.Play ("WaveTransitionUIStats");
		}

		if (gameControllerScript.Wave % 4 != 1) 
		{
			SoundtrackUI.SetActive (false);
		}
	}
}