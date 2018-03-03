﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FPSCounter : MonoBehaviour 
{
	public float frequency = 0.5f; // How often to update calculating fps.
	public TextMeshProUGUI FPSText;
	public bool showPrefix;
	public int FramesPerSec { get; protected set; }
	public float FramesPerSecB { get; protected set; }
	public bool toggle;
	public bool showFps;

	private void Start()
	{
		StartCoroutine (FPS ());

		if (showFps == true)
		{
			FPSText.enabled = true;
		}

		if (showFps == false)
		{
			FPSText.enabled = false;
		}
	}

	private IEnumerator FPS () 
	{
		for(;;)
		{
			// Capture current framerate.
			int lastFrameCount = Time.frameCount;

			float lastTime = Time.realtimeSinceStartup; // Get time since startup.

			// Sets how fast this updates.
			yield return new WaitForSeconds (frequency);

			// Get time span and frame count.
			float timeSpan = Time.realtimeSinceStartup - lastTime;
			int frameCount = Time.frameCount - lastFrameCount;
			
			// Display it.
			FramesPerSec = Mathf.RoundToInt(frameCount / timeSpan);
			FramesPerSecB = frameCount / timeSpan;
			FPSText.text = FramesPerSec.ToString() + "";

			if (showPrefix == true)
			{
				FPSText.text = "FPS: " + FramesPerSec.ToString() + " (" + ((1.0f/(FramesPerSecB)) * 1000.0f) + " ms)";
			}
		}
	}
}