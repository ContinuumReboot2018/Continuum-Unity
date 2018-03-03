using UnityEngine;

public class AudioSourceLoudnessTester : MonoBehaviour 
{
	public AudioSource audioSource; // Which AudioSource component to look at.
	[Range (0, 1)]
	public float clipLoudness; // Current loudness of the clip (Returns a rang ebetween 0 and 1).

	public float updateStep = 0.1f; // How fast each update cycle is calculated.
	public int sampleDataLength = 1024; // How many samples.
	private float[] clipSampleData; // Samples array.
	private float currentUpdateTime = 0f;

	void OnEnable () 
	{
		// Checks for array.
		if (!audioSource)
		{
			Debug.LogError(GetType() + ".Awake: There was no AudioSource assigned.");
			this.enabled = false;
		}

		clipSampleData = new float[sampleDataLength]; // Creates new sample data array.
	}
		
	void Update ()
	{
		// AudioSource must be playing.
		if (audioSource.isPlaying == true)
		{
			currentUpdateTime += Time.deltaTime; // Increase update time.

			// Update time must be grater than update step/
			if (currentUpdateTime >= updateStep) 
			{
				currentUpdateTime = 0f; // Reset update time.

				// I read 1024 samples, which is about 80 ms on a 44khz stereo clip, 
				// beginning at the current sample position of the clip.
				audioSource.clip.GetData (clipSampleData, audioSource.timeSamples); 

				clipLoudness = 0f; // Reset clip loudness reading.

				foreach (var sample in clipSampleData) 
				{
					clipLoudness += Mathf.Abs (sample);
				}

				clipLoudness /= sampleDataLength; // ClipLoudness is what you are looking for.
			}
		}
	}
}