using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

/* Instructions: 

1. Attach script to gameObject with an audio source
2. Set values

 */

public class AudioSourcePitchByTimescale : MonoBehaviour 
{
	public float startingPitch = 1.0f;
	public float multiplierPitch = 1.0f;
	public float minimumPitch = 0.0001f;
	public float maximumPitch = 20.0f;
	public float addPitch;
	public bool reachedMaxPitch;
	public bool dontUseStartPitch;

	private AudioSource Audio;
	
	void Start () 
	{
		FindAudioSource ();

		if (dontUseStartPitch == false) 
		{
			Audio.pitch = startingPitch; // Gives value to audio pitch
		}

		if (dontUseStartPitch == true) 
		{
			Audio.pitch = Time.timeScale; // Sets starting pitch based on Time.timeScale.
		}
	}
	

	void Update ()
	{
		Audio.pitch = (Time.timeScale * multiplierPitch * startingPitch) + addPitch;

		if (Audio.pitch < 0) 
		{
			Debug.LogWarning ("Audio pitch is " + Audio.pitch + ".");
		}

		Audio.pitch = Mathf.Clamp (Audio.pitch, minimumPitch, maximumPitch);

		// Gives maximum pitch to audio source before reaching it
		if (Audio.pitch > maximumPitch)
		{
			if (reachedMaxPitch == false)
			{
				Audio.pitch = maximumPitch;
				Debug.Log ("Reached maximum audio pitch.");
				reachedMaxPitch = true;
			}

			Audio.pitch = maximumPitch;
		}
	}

	void FindAudioSource ()
	{
		Audio = GetComponent<AudioSource> ();
	}
}
