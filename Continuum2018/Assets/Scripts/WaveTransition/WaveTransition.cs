using UnityEngine;

public class WaveTransition : MonoBehaviour 
{
	[Tooltip ("Wave Transition UI GameObject.")]
	public GameObject WaveTransitionUI;
	[Tooltip ("UI for soundtrack title.")]
	public GameObject SoundtrackUI;

	// Turn off wave transition UI based on wave number.
	public void DeactivateWaveTransition ()
	{
		GameController.Instance.IsInWaveTransition = false;

		if (GameController.Instance.Wave % 4 == 1 || GameController.Instance.Wave == 1) 
		{
			SoundtrackUI.SetActive (true);
			GameController.Instance.WaveTransitionUIStats.Play ("WaveTransitionUIStatsExit");
		}

		if (GameController.Instance.Wave % 4 != 1) 
		{
			SoundtrackUI.SetActive (false);
		}
	}

	// Turn on wave transition UI based on wave number.
	public void ActivateWaveTransitionUI ()
	{
		GameController.Instance.IsInWaveTransition = true;

		if (GameController.Instance.Wave % 4 == 1 || GameController.Instance.Wave != 1) 
		{
			SoundtrackUI.SetActive (true);
			GameController.Instance.WaveTransitionUIStats.Play ("WaveTransitionUIStats");
		}

		if (GameController.Instance.Wave % 4 != 1) 
		{
			SoundtrackUI.SetActive (false);
		}
	}
}