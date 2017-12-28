using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFPS : MonoBehaviour 
{
	public int targetFramerate = 60;
	public int RefreshRate;

	void Start ()
	{
		RefreshRate = Screen.currentResolution.refreshRate;
		Application.targetFrameRate = RefreshRate; 
	}

	public void SetTargetFramerate (int framerate)
	{
		Application.targetFrameRate = framerate; 
	}
}
