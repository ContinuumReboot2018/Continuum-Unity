using UnityEngine;
using System.Collections;

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

	void Awake ()
	{
		Audio = GetComponent<AudioSource> ();
	}

	void Start () 
	{
		Audio.pitch = dontUseStartPitch ? startingPitch : Time.timeScale; // Gives value to audio pitch.
	}

	void Update ()
	{
		Audio.pitch = (Time.timeScale * multiplierPitch * startingPitch) + addPitch;
		Audio.pitch = Mathf.Clamp (Audio.pitch, minimumPitch, maximumPitch);
	}
}
