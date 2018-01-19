using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFPS : MonoBehaviour 
{
	public int targetFramerate = 60;
	public int RefreshRate;
	public bool useScreenRefreshRate;

	void Start ()
	{
		if (useScreenRefreshRate == true) 
		{
			Application.targetFrameRate = Screen.currentResolution.refreshRate;
		} 
		else 
		{
			Application.targetFrameRate = RefreshRate; 
		}
	}

	public void SetTargetFramerate (int framerate)
	{
		Application.targetFrameRate = framerate; 
	}
}
