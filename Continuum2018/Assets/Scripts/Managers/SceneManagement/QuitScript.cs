using System.Collections;
using UnityEngine;

public class QuitScript : MonoBehaviour 
{
	public SaveAndLoadScript saveAndLoadScript;
	private float delayQuitTime;

	void Start ()
	{
		// Find the saving script.
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();
	}

	public void DelayAndQuit (float delay)
	{
		saveAndLoadScript.SavePlayerData ();
		saveAndLoadScript.SaveSettingsData ();
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