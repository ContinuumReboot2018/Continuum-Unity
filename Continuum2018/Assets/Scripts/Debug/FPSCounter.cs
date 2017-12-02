/* **************************************************************************
 * FPS COUNTER
 * **************************************************************************
 * Written by: Annop "Nargus" Prapasapong
 * Created: 7 June 2012
 * *************************************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class FPSCounter : MonoBehaviour 
{
	public float frequency = 0.5f;
	public TextMeshProUGUI FPSText;
	public bool showPrefix;
	public int FramesPerSec { get; protected set; }
	public float FramesPerSecB { get; protected set; }
	public bool toggle;
	public bool showFps;

	private void Start()
	{
		StartCoroutine(FPS());

		if (showFps == true)
		{
			FPSText.enabled = true;
		}

		if (showFps == false)
		{
			FPSText.enabled = false;
		}
	}

	void Update ()
	{
		/*if (Input.GetKeyDown (KeyCode.Tab)) 
		{
			if (toggle == true)
			{
				if (showFps == false)
				{
					FPSText.enabled = true;

				}

				if (showFps == true) 
				{
					FPSText.enabled = false;
				}

				showFps = !showFps;
			}
		}*/
	}

	private IEnumerator FPS() 
	{
		for(;;)
		{
			// Capture frame-per-second
			int lastFrameCount = Time.frameCount;

			float lastTime = Time.realtimeSinceStartup;

			// Sets how fast this updates
			yield return new WaitForSeconds(frequency);

			float timeSpan = Time.realtimeSinceStartup - lastTime;
			int frameCount = Time.frameCount - lastFrameCount;
			
			// Display it
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