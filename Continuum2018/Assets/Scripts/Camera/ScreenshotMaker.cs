using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotMaker : MonoBehaviour 
{
	public string ScreenshotName = "Screenshot.png";
	public int ScreenShotsMade;
	public int SuperSizeFactor = 4;
	public int GlobalTime;

	void Start ()
	{
		ScreenShotsMade = 0;
		GlobalTime = 0;
		ScreenshotName = "Screenshot_" + ScreenShotsMade + "_" + GlobalTime +".png";
	}

	void Update ()
	{
		GlobalTime += 1;
		
		if (Input.GetKeyDown (KeyCode.Alpha9)) 
		{
			ScreenCapture.CaptureScreenshot (ScreenshotName, SuperSizeFactor);
			ScreenShotsMade += 1;
			ScreenshotName = "Screenshot_" + ScreenShotsMade + "_" + GlobalTime +".png";
			Debug.Log ("Captured screenshot");
		}

		if (Input.GetKey (KeyCode.Alpha0)) 
		{
			if (Time.timeScale != 0) 
			{
				Time.timeScale = 0;
			}
		}

		if (Input.GetKeyUp (KeyCode.Alpha0)) 
		{
			Time.timeScale = 1;
		}
	}
}
