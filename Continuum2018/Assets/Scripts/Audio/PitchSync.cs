using UnityEngine;

public class PitchSync : MonoBehaviour 
{
	public AudioSource master; // The AudioSource to follow.
	public AudioSource slave; // The following AudioSource.

	// Syncronizes audio source timing with another audio source.
	void LateUpdate () 
	{
		// Checks if either audio source is playing.
		if (slave.isPlaying == true || master.isPlaying == true)
		{
			slave.pitch = master.pitch; // Match pitches.
		}
	}
}