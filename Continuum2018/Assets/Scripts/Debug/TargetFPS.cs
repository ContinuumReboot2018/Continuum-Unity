using UnityEngine;

public class TargetFPS : MonoBehaviour 
{
	public SaveAndLoadScript saveAndLoadScript;

	[Tooltip ("Game will try to render as fast as the target. Will try to run as fast as possible if set to a negative number.")]
	public int targetFramerate = 60; 
	public int currentTargetFramerate;
	[Tooltip ("Use screen refresh rate and match framerate.")]
	public bool useScreenRefreshRate;

	void Start ()
	{
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();
		saveAndLoadScript.targetFramerateScript = this;

		if (useScreenRefreshRate == true) 
		{
			Application.targetFrameRate = Screen.currentResolution.refreshRate;
		} 

		InvokeRepeating ("UpdateCurrentTargetFramerate", 0, 1);
	}

	void UpdateCurrentTargetFramerate ()
	{
		currentTargetFramerate = Application.targetFrameRate;
	}

	// Set target framerate by other scripts.
	public void SetTargetFramerate (int framerate)
	{
		Application.targetFrameRate = framerate; 
		saveAndLoadScript.targetframerate = framerate;
		Debug.Log ("Target framerate set to " + framerate + " FPS.");
	}
}