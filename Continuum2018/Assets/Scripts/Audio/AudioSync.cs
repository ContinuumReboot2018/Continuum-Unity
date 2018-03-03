using UnityEngine;

public class AudioSync : MonoBehaviour 
{
	public AudioSource master; // The AudioSource to follow.
	public AudioSource slave; // The following AudioSource.

	// Syncronizes audio source timing with another audio source.
	void LateUpdate () 
	{
		// Checks if either audio source is playing.
		if (slave.isPlaying == true || master.isPlaying == true) 
		{
			slave.timeSamples = master.timeSamples; // Match time samples.
		}
	}
}