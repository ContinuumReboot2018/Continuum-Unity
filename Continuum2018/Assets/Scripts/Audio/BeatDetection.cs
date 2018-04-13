using UnityEngine;

public class BeatDetection : MonoBehaviour 
{
	public AudioController audioControllerScript;
	public AudioProcessor processor;
	public bool isLayerOne;

	void Start ()
	{
		audioControllerScript = GameObject.Find ("AudioController").GetComponent<AudioController> ();

		//Select the instance of AudioProcessor and pass a reference to this object.
		//AudioProcessor processor = FindObjectOfType<AudioProcessor> ();

		processor = GetComponent<AudioProcessor> ();
		processor.onBeat.AddListener (onOnbeatDetected);
		processor.onSpectrum.AddListener (onSpectrum);
	}

	// This event will be called every time a beat is detected.
	// Change the threshold parameter in the inspector to adjust the sensitivity.
	void onOnbeatDetected ()
	{
		if (isLayerOne == true)
		{
			//Debug.Log ("Beat!");
			audioControllerScript.Beats += 1;
			audioControllerScript.BeatInBar = (audioControllerScript.Beats % 4) + 1;
			GetBeatsPerMinute ();
		}
	}

	//This event will be called every frame while music is playing.
	void onSpectrum (float[] spectrum)
	{
		//The spectrum is logarithmically averaged to 12 bands.
		for (int i = 0; i < spectrum.Length; ++i)
		{
			Vector3 start = new Vector3 (i, 0, 0);
			Vector3 end = new Vector3 (i, spectrum [i], 0);
			Debug.DrawLine (start, end);
		}
	}

	public void GetBeatsPerMinute ()
	{
		audioControllerScript.BeatsPerMinute = (
			audioControllerScript.Beats / 
			((audioControllerScript.TimeSinceTrackLoad / 60)) * 
			audioControllerScript.BassTrack.pitch
		);
	}
}