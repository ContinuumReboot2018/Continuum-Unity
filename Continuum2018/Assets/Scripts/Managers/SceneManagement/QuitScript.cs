using System.Collections;
using UnityEngine;

public class QuitScript : MonoBehaviour 
{
	private float delayQuitTime;

	public void DelayAndQuit (float delay)
	{
		SaveAndLoadScript.Instance.SavePlayerData ();
		SaveAndLoadScript.Instance.SaveSettingsData ();
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