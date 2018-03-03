using UnityEngine;

public class AudioSourcePitchByTimescale : MonoBehaviour 
{
	public float currentPitchView; // The current pitch of the audio source. (Read only.)
	public bool dontUseStartPitch; // Don't use the starting pitch.
	public float startingPitch = 1.0f; // The pitch to start off.
	public float multiplierPitch = 1.0f; // Multiply by this amount to get amplitude of change in pitch.
	public float minimumPitch = 0.0001f; // Specify a minimum pitch.
	public float maximumPitch = 20.0f; // Specify a maximum pitch.
	public float addPitch; // Specify to offset pitch if needed.

	private AudioSource Audio; // Reference to audio source.

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
		Audio.pitch = (Time.timeScale * multiplierPitch * startingPitch) + addPitch; // Sets pitch based on Time.timeScale and modifiers.
		Audio.pitch = Mathf.Clamp (Audio.pitch, minimumPitch, maximumPitch); // Clamps pitch to min and max values.
		currentPitchView = Audio.pitch; // Update current pitch attribute.
	}
}