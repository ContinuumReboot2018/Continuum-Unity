using System.Collections;
using UnityEngine;

public class QuitScript : MonoBehaviour 
{
	private float delayQuitTime;

	public void DelayAndQuit (float delay)
	{
		delayQuitTime = delay;
		StartCoroutine (DelayQuitApp ());
	}

	IEnumerator DelayQuitApp ()
	{
		yield return new WaitForSecondsRealtime (delayQuitTime);
		QuitGame ();
	}

	public void QuitGame ()
	{
		Application.Quit ();
	}
}