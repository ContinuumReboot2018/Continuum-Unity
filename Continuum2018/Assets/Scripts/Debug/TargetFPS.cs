using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFPS : MonoBehaviour 
{
	public int targetFramerate = 60;

	void Start ()
	{
		Application.targetFrameRate = targetFramerate; 
	}

	public void SetTargetFramerate (int framerate)
	{
		Application.targetFrameRate = framerate; 
	}
}
