using System.Collections;
using System.Collections.Generic;
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

	void QuitGame ()
	{
		Application.Quit ();
	}
}
