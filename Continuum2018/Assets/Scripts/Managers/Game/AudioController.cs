using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour 
{
	public PlayerController playerControllerScript_P1;
	public GameController gameControllerScript;
	public TimescaleController timescaleControllerScript;
	public SaveAndLoadScript saveAndLoadScript;

	public bool updateVolumeAndPitches = true;
	public float Distance;

	public AudioMixer AudioMix;
	public float curFreq;
	public float TargetCutoffFreq;
	public float CutoffFreqSmoothing;
	public float curRes;
	public float TargetResonance;
	public float ResonanceSmoothing;

	// Distances to edit
	[Header ("Distance values")]
	public float BaseDistance;
	public float LowDistance;
	public float MediumDistance;
	public float HighDistance;

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

	public Vector4 TimePitch;

	[Header ("StereoUI")]
	public AudioSourceLoudnessTester bassLoudness;
	public AudioSourceLoudnessTester layerOneLoudness;
	public AudioSourceLoudnessTester layerTwoLoudness;
	public AudioSourceLoudnessTester layerThreeLoudness;
	public float CurrentBassLoudness;
	public float CurrentLayerOneLoudness;
	public float CurrentLayerTwoLoudness;
	public float CurrentLayerThreeLoudness;
	public Image StereoImageL;
	public Image StereoImageR;
	public float LoudnessSmoothing = 0.1f;

	void Awake ()
	{
		TargetCutoffFreq = 22000;
	}

	void Start ()
	{
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();
		//TrackNumber = 3;
		LoadTracks ();
	}

	void Update ()
	{
		GetMasterLowPassValue ();
		GetMasterResonanceValue ();

		if (gameControllerScript.isPaused == false && 
			timescaleControllerScript.isInInitialCountdownSequence == false && 
			timescaleControllerScript.isInInitialSequence == false)
		{
			UpdateSoundtrackVolumeAndPitches ();
			//UpdateStereoUI ();
		}

		if (timescaleControllerScript.isOverridingTimeScale == true) 
		{
			BassTargetPitch = Time.timeScale;
		}

		float SmoothLowFreqVal = Mathf.Lerp (curFreq, TargetCutoffFreq, CutoffFreqSmoothing * Time.unscaledDeltaTime);
		AudioMix.SetFloat ("LowCutoffFrequency", SmoothLowFreqVal);

		float SmoothResVal = Mathf.Lerp (curRes, TargetResonance, ResonanceSmoothing * Time.unscaledDeltaTime);
		AudioMix.SetFloat ("Resonance", SmoothResVal);
	}

	public float GetMasterLowPassValue ()
	{
		bool curFreqResult = AudioMix.GetFloat ("LowCutoffFrequency", out curFreq);

		if (curFreqResult) {
			return curFreq;
		} else {
			return 0f;
		}
	}

	public float GetMasterResonanceValue ()
	{
		bool curResResult = AudioMix.GetFloat ("Resonance", out curRes);

		if (curResResult) {
			return curRes;
		} else {
			return 0f;
		}
	}

	void UpdateStereoUI ()
	{
		CurrentBassLoudness = bassLoudness.clipLoudness;
		CurrentLayerOneLoudness = layerOneLoudness.clipLoudness;
		CurrentLayerTwoLoudness = layerTwoLoudness.clipLoudness;
		CurrentLayerThreeLoudness = layerThreeLoudness.clipLoudness;

		// For all clips
		//float AverageLoudness = 0.25f * (CurrentBassLoudness + CurrentLayerOneLoudness + CurrentLayerTwoLoudness + CurrentLayerThreeLoudness);

		// For bass clip
		float AverageLoudness = 0.5f * (CurrentBassLoudness + CurrentLayerOneLoudness);

		StereoImageL.fillAmount = Mathf.Lerp (StereoImageL.fillAmount, AverageLoudness, LoudnessSmoothing * Time.deltaTime);
		StereoImageR.fillAmount = Mathf.Lerp (StereoImageR.fillAmount, AverageLoudness, LoudnessSmoothing * Time.deltaTime);
	}

	public void UpdateSoundtrackVolumeAndPitches ()
	{
		if (updateVolumeAndPitches == true) 
		{
			Distance = timescaleControllerScript.Distance;

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

		// Medium distance.
		if (Distance >= LowDistance && Distance < MediumDistance) 
		{
			BaseTargetVolume = BassVolume.z;
			LayerOneTargetVolume = LayerOneVolume.z;
			LayerTwoTargetVolume = LayerTwoVolume.z;
			LayerThreeTargetVolume = LayerThreeVolume.z;
		}

		// High distance.
		if (Distance >= MediumDistance && Distance < HighDistance) 
		{
			BaseTargetVolume = BassVolume.w;
			LayerOneTargetVolume = LayerOneVolume.w;
			LayerTwoTargetVolume = LayerTwoVolume.w;
			LayerThreeTargetVolume = LayerThreeVolume.w;
		}

		// Top distance.
		if (Distance >= HighDistance) 
		{
			BaseTargetVolume = 1;
			LayerOneTargetVolume = 1;
			LayerTwoTargetVolume = 1;
			LayerThreeTargetVolume = 1;
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

		// Medium distance.
		if (Distance >= LowDistance && Distance < MediumDistance) 
		{
			BassTargetPitch = TimePitch.z;
		}

		// High distance.
		if (Distance >= MediumDistance && Distance < HighDistance) 
		{
			BassTargetPitch = TimePitch.w;
		}

		// Top distance.
		if (Distance >= HighDistance) 
		{
			BassTargetPitch = 1.5f;
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
		TrackName = TrackNames [TrackNumber];
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

	public void StopAllSoundtracks ()
	{
		BassTrack.Pause ();
		LayerOneTrack.Pause ();
		LayerTwoTrack.Pause ();
		LayerThreeTrack.Pause ();
	}
}
