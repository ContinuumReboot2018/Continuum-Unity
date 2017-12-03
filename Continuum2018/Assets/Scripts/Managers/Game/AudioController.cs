using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour 
{
	public PlayerController playerControllerScript_P1;
	public GameController gameControllerScript;
	public TimescaleController timescaleControllerScript;
	public SaveAndLoadScript saveAndLoadScript;

	public bool updateVolumeAndPitches = true;
	public float Distance;

	// Distances to edit
	[Header ("Distance values")]
	public float BaseDistance = 5.5f;
	public float LowDistance = 11.16f;
	public float HighDistance = 13.96f;

	[Header ("Track Sequence")]
	public int TrackNumber;
	public string TrackName;
	public string[] TrackNames;
	public trackSequence TrackSequenceMode;
	public enum trackSequence
	{
		Sequential,
		Random,
		Repeat
	}

	// The soundtrack audio sources.
	public AudioSource BassTrack;
	public AudioSource LayerOneTrack;
	public AudioSource LayerTwoTrack;
	public AudioSource LayerThreeTrack;

	[Header ("Soundtrack Library")]
	// A library of all each type of track.
	public AudioClip[] BassTracks;
	public AudioClip[] LayerOneTracks;
	public AudioClip[] LayerTwoTracks;
	public AudioClip[] LayerThreeTracks;

	[Header ("Volume")]
	public float BaseTargetVolume;
	public float LayerOneTargetVolume;
	public float LayerTwoTargetVolume;
	public float LayerThreeTargetVolume;
	public float VolumeSmoothTime;

	public Vector4 BassVolume, LayerOneVolume, LayerTwoVolume, LayerThreeVolume;

	[Header ("Pitch")]
	public float BassTargetPitch;
	public float PitchSmoothTime;

	public Vector4 TimePitch = new Vector4 (0.25f, 1.0f, 1.25f, 1.5f);

	void Start ()
	{
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();
		//RandomTrack ();
		TrackNumber = 5;
		LoadTracks ();
	}

	void Update () 
	{
		Distance = timescaleControllerScript.Distance;

		if (updateVolumeAndPitches == true) 
		{
			UpdateSoundtracksVolume ();
			UpdateSoundtracksPitch ();

			UpdateTargetVolumes ();
			UpdateTargetPitches ();
		}
	}

	void UpdateTargetVolumes ()
	{
		// Base distance.
		if (Distance < BaseDistance) 
		{
			BaseTargetVolume = BassVolume.x;
			LayerOneTargetVolume = LayerOneVolume.x;
			LayerTwoTargetVolume = LayerTwoVolume.x;
			LayerThreeTargetVolume = LayerThreeVolume.x;
		}

		// Low distance.
		if (Distance >= BaseDistance && Distance < LowDistance) 
		{
			BaseTargetVolume = BassVolume.y;
			LayerOneTargetVolume = LayerOneVolume.y;
			LayerTwoTargetVolume = LayerTwoVolume.y;
			LayerThreeTargetVolume = LayerThreeVolume.y;
		}

		// High distance.
		if (Distance >= LowDistance && Distance < HighDistance) 
		{
			BaseTargetVolume = BassVolume.z;
			LayerOneTargetVolume = LayerOneVolume.z;
			LayerTwoTargetVolume = LayerTwoVolume.z;
			LayerThreeTargetVolume = LayerThreeVolume.z;
		}

		// Top distance.
		if (Distance >= HighDistance) 
		{
			BaseTargetVolume = BassVolume.w;
			LayerOneTargetVolume = LayerOneVolume.w;
			LayerTwoTargetVolume = LayerTwoVolume.w;
			LayerThreeTargetVolume = LayerThreeVolume.w;
		}
	}

	void UpdateTargetPitches ()
	{
		// Base distance.
		if (Distance < BaseDistance) 
		{
			BassTargetPitch = TimePitch.x;
		}

		// Low distance.
		if (Distance >= BaseDistance && Distance < LowDistance) 
		{
			BassTargetPitch = TimePitch.y;
		}

		// High distance.
		if (Distance >= LowDistance && Distance < HighDistance) 
		{
			BassTargetPitch = TimePitch.z;
		}

		// Top distance.
		if (Distance >= HighDistance) 
		{
			BassTargetPitch = TimePitch.w;
		}
	}

	void UpdateSoundtracksVolume ()
	{
		// Updates target volumes.
		BassTrack.volume = Mathf.Lerp (BassTrack.volume, BaseTargetVolume + (saveAndLoadScript.SoundtrackVolume - 1), VolumeSmoothTime * Time.unscaledDeltaTime);
		LayerOneTrack.volume = Mathf.Lerp (LayerOneTrack.volume, LayerOneTargetVolume + (saveAndLoadScript.SoundtrackVolume - 1), VolumeSmoothTime * Time.unscaledDeltaTime);
		LayerTwoTrack.volume = Mathf.Lerp (LayerTwoTrack.volume, LayerTwoTargetVolume + (saveAndLoadScript.SoundtrackVolume - 1), VolumeSmoothTime * Time.unscaledDeltaTime);
		LayerThreeTrack.volume = Mathf.Lerp (LayerThreeTrack.volume, LayerThreeTargetVolume + (saveAndLoadScript.SoundtrackVolume - 1), VolumeSmoothTime * Time.unscaledDeltaTime);
	}

	void UpdateSoundtracksPitch ()
	{
		// Updates target pitch for bass track. 
		// We only need tp update the bass track as the other layers of audio have a pitch sync script taking care of this.
		BassTrack.pitch = Mathf.Lerp (BassTrack.pitch, BassTargetPitch, PitchSmoothTime * Time.unscaledDeltaTime);
	}

	void UpdateTrackNames ()
	{
		TrackName = TrackNames [TrackNumber];
	}

	void LoadTracks ()
	{
		BassTrack.clip = BassTracks [TrackNumber];
		LayerOneTrack.clip = LayerOneTracks [TrackNumber];
		LayerTwoTrack.clip = LayerTwoTracks [TrackNumber];
		LayerThreeTrack.clip = LayerThreeTracks [TrackNumber];

		BassTrack.Play ();
		LayerOneTrack.Play ();
		LayerTwoTrack.Play ();
		LayerThreeTrack.Play ();
	}

	public void NextTrack ()
	{
		if (TrackSequenceMode == trackSequence.Sequential) 
		{
			if (TrackNumber < BassTracks.Length) 
			{
				TrackNumber += 1;
			}

			if (TrackNumber >= BassTracks.Length) 
			{
				TrackNumber = 0;
			}
		}

		if (TrackSequenceMode == trackSequence.Random) 
		{
			TrackNumber = Random.Range (0, BassTracks.Length);
		}

		LoadTracks ();
	}

	public void PreviousTrack ()
	{
		if (TrackSequenceMode == trackSequence.Sequential) 
		{
			if (TrackNumber <= 0) 
			{
				TrackNumber = BassTracks.Length;
			}

			if (TrackNumber > 0) 
			{
				TrackNumber -= 1;
			}
		}

		if (TrackSequenceMode == trackSequence.Random) 
		{
			TrackNumber = Random.Range (0, BassTracks.Length);
		}

		LoadTracks ();
	}

	public void RandomTrack ()
	{
		TrackNumber = Random.Range (0, BassTracks.Length);
		LoadTracks ();
	}
}
