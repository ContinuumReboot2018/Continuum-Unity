using UnityEngine;

public class TargetFPS : MonoBehaviour 
{
	// Game will try to render as fast as the target.
	// Will try to run as fast as possible if set to a negative number.
	public int targetFramerate = 60; 
	[Space (10)]
	public bool useScreenRefreshRate; // Use screen refresh rate and match framerate.

	void Start ()
	{
		if (useScreenRefreshRate == true) 
		{
			Application.targetFrameRate = Screen.currentResolution.refreshRate;
		} 
	}

	// Set target framerate by other scripts.
	public void SetTargetFramerate (int framerate)
	{
		Application.targetFrameRate = framerate; 
	}
}