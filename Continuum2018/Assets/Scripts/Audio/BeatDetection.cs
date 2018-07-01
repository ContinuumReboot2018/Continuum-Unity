using UnityEngine;

public class BeatDetection : MonoBehaviour 
{
	public AudioProcessor processor;
	public bool isLayerOne;

	void Start ()
	{
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
			AudioController.Instance.Beats += 1;
			AudioController.Instance.BeatInBar = (AudioController.Instance.Beats % 4) + 1;
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
		AudioController.Instance.BeatsPerMinute = (
			AudioController.Instance.Beats / 
			((AudioController.Instance.TimeSinceTrackLoad / 60)) * 
			AudioController.Instance.BassTrack.pitch
		);
	}
}