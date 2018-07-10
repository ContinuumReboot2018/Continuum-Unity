using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Linq;

public class AudioController : MonoBehaviour 
{
	public static AudioController Instance { get; private set; }

	public PlayerController playerControllerScript_P1;

	[Tooltip ("If true, the music will update to volume and pitch.")]
	public bool updateVolumeAndPitches = true;

	[Tooltip ("Gets distance from TimescaleController.Instance.")]
	public float Distance;

	[Tooltip ("Audio mixer on soundtracks.")]
	public AudioMixer SoundtrackAudioMix;
	public AudioMixer EffectsAudioMix;

	[Tooltip ("Current low pass frequency from mixer.")]
	public float curFreq;
	public float curEffectsFreq;

	[Tooltip ("Target low pass frequency for mixer.")]
	public float TargetCutoffFreq;

	[Tooltip ("Low pass frequency smooth time.")]
	public float CutoffFreqSmoothing;

	[Tooltip ("Current low pass resonance from mixer.")]
	public float curRes;
	public float curEffectsRes;

	[Tooltip ("Target low pass resonance for mixer.")]
	public float TargetResonance;

	[Tooltip ("Low pass resonance smooth time.")]
	public float ResonanceSmoothing;

	// Distances to edit
	[Header ("Distance values")]

	public float[] DistanceValues;

	[Header ("Track Sequence")]

	[Tooltip ("Current track number referenced in array.")]
	public int TrackNumber;

	[Tooltip ("The track name based on track number.")]
	public string TrackName;

	[Tooltip ("List of all track names.")]
	public string[] TrackNames;

	[Tooltip ("Can change the way tracks are sequenced.")]
	public trackSequence TrackSequenceMode;
	public enum trackSequence
	{
		Sequential,
		Random,
	}

	public AudioSource[] LayerSources;

	[Tooltip ("Plays beat detection tracks.")]
	public AudioSource[] BeatDetectionTracks;

	[Header ("Soundtrack Library")]

	public List<AudioClipsByLayer> TracksByLayer;

	[Tooltip ("Beat detection tracks. Must show what tempo this is to synchronize with audio.")]
	public AudioClip[] BeatDetectionLayerOneTracks;
	public AudioClip[] BeatDetectionLayerTwoTracks;
	public AudioClip[] BeatDetectionLayerThreeTracks;
	public AudioClip[] BeatDetectionLayerFourTracks;

	[Header ("Beat Detection")]

	[Tooltip ("Increments by 1 every time a beat is detected.")]
	public int Beats;

	[Tooltip ("Divides beats by 4 and returns the remainder.")]
	public int BeatInBar;

	[Tooltip ("Calculates BPM.")]
	public float BeatsPerMinute;

	[Tooltip ("Scaled time since the audio track changed.")]
	public float TimeSinceTrackLoad;

	[Header ("Volume")]

	public float[] TargetVolumes;

	public List<VolumesByLayer> TrackVolumesByLayer;

	[Tooltip ("Layer three current volume lerps to this value.")]
	public float VolumeSmoothTime;

	[Header ("Pitch")]

	[Tooltip ("Current bass track pitch lerps to this value, other tracks synchronize automatically.")]
	public float BassTargetPitch;

	[Tooltip ("Pitch smoothing time.")]
	public float PitchSmoothTime;

	public List<float> TimePitchesByLayer;

	[Tooltip ("Is pitch being reversed?")]
	public bool ReversePitch;

	public AudioSource BigBossSoundtrack;

	void Awake ()
	{
		Instance = this;
	}

	void Start ()
	{
		TargetCutoffFreq = 22000; // Set target cutoff frequency to max value (22kHz).

		// Randomize track if on random mode.
		if (TrackSequenceMode == trackSequence.Random) 
		{
			TrackNumber = Random.Range (0, TracksByLayer.Count);
		}

		LoadTracks (); // Load the track by track number.
		InvokeRepeating ("CheckReversePitch", 0, 0.5f); // If in rewind, check for reversing the pitch.
		AudioListener.volume = SaveAndLoadScript.Instance.MasterVolume;
	}

	void Update ()
	{
		UpdateAudio (); // Method to update audio states.
		GetMasterLowPassValue (); // Update low pass filter cutoff frequency.
		GetMasterResonanceValue (); // Update low pass filter resonance amount.
		SetFilterEffectAmounts (); // Sets filter properties based on audio states.
		UpdateTimeSinceTrackLoad ();
	}
		
	void UpdateAudio ()
	{
		if (GameController.Instance.isPaused == false && 
			TimescaleController.Instance.isInInitialCountdownSequence == false && 
			TimescaleController.Instance.isInInitialSequence == false)
		{
			UpdateSoundtrackVolumeAndPitches (); // Update sound pitch based on distance.
		}

		if (TimescaleController.Instance.isOverridingTimeScale == true) 
		{
			BassTargetPitch = Time.timeScale; // pitch based on Time.timeScale when time is overriding.
		}
	}

	void SetFilterEffectAmounts ()
	{
		// Sets low pass filter frequency cutoff value.
		float SmoothLowFreqVal = Mathf.Lerp (curFreq, TargetCutoffFreq, CutoffFreqSmoothing * Time.unscaledDeltaTime);
		SoundtrackAudioMix.SetFloat ("LowCutoffFrequency", SmoothLowFreqVal);
		EffectsAudioMix.SetFloat ("LowCutoffFrequency", SmoothLowFreqVal);

		// Sets low pass filter resonance value.
		float SmoothResVal = Mathf.Lerp (curRes, TargetResonance, ResonanceSmoothing * Time.unscaledDeltaTime);
		SoundtrackAudioMix.SetFloat ("Resonance", SmoothResVal);
		EffectsAudioMix.SetFloat ("Resonance", SmoothResVal);
	}

	public void SetTargetLowPassFreq (float lowPassFreq)
	{
		TargetCutoffFreq = lowPassFreq;
	}

	public void SetTargetResonance (float resAmt)
	{
		TargetResonance = resAmt;
	}

	// Reverses the pitch if rewinding.
	void CheckReversePitch ()
	{
		ReversePitch = TimescaleController.Instance.isRewinding;
	}

	// Gets current low pass cutoff frequency value.
	public float GetMasterLowPassValue ()
	{
		bool curFreqResult = SoundtrackAudioMix.GetFloat ("LowCutoffFrequency", out curFreq);

		if (curFreqResult && GameController.Instance.isPaused == false) 
		{
			return curFreq;
		}

		else
		
		{
			return 22000f;
		}
	}

	// Gets current low pass resonance value.
	public float GetMasterResonanceValue ()
	{
		bool curResResult = SoundtrackAudioMix.GetFloat ("Resonance", out curRes);

		if (curResResult && GameController.Instance.isPaused == false) 
		{
			return curRes;
		} 

		else 
		
		{
			return 0f;
		}
	}

	public float GetEffectsLowPassValue ()
	{
		bool curFreqEffectsResult = EffectsAudioMix.GetFloat ("LowCutoffFrequency", out curEffectsFreq);

		if (curFreqEffectsResult && GameController.Instance.isPaused == false) 
		{
			return curEffectsFreq;
		}

		else

		{
			return 22000f;
		}
	}

	// Gets current low pass resonance value.
	public float GetEffectsResonanceValue ()
	{
		bool curEffectsResResult = EffectsAudioMix.GetFloat ("Resonance", out curEffectsRes);

		if (curEffectsResResult && GameController.Instance.isPaused == false) 
		{
			return curEffectsRes;
		} 

		else 

		{
			return 0f;
		}
	}

	// Updates audio sources volume and pitch here.
	public void UpdateSoundtrackVolumeAndPitches ()
	{
		if (updateVolumeAndPitches == true) 
		{
			Distance = TimescaleController.Instance.Distance;

			UpdateSoundtracksVolume ();
			UpdateSoundtracksPitch ();

			UpdateTargetVolumes ();
			UpdateTargetPitches ();
		}
	}

	// Updates target volume values for the sountrack to interpolate towards.
	void UpdateTargetVolumes ()
	{
		for (int i = 0; i < DistanceValues.Length; i++)
		{
			if (Distance > DistanceValues [i]) 
			{
				TargetVolumes [i] = TrackVolumesByLayer [i].Volumes[i];
			}
		}
	}

	// Updates target pitch values for the sountrack to interpolate towards.
	void UpdateTargetPitches ()
	{
		for (int i = 0; i < DistanceValues.Length; i++) 
		{
			if (Distance > DistanceValues [i]) 
			{
				BassTargetPitch = TimePitchesByLayer [i] * (ReversePitch ? -1 : 1);
			}
		}
	}

	// Updates volume by reading targets and lerping.
	void UpdateSoundtracksVolume ()
	{
		// Updates target volumes.
		// We need to compensate for how the mmixer volume is displayed in dB (-80 to 0).

		for (int i = 0; i < LayerSources.Length; i++)
		{
			LayerSources [i].volume = Mathf.Lerp (
				LayerSources [i].volume,
				TargetVolumes [i] + (1 + (float)System.Math.Round (0.0125f * SaveAndLoadScript.Instance.SoundtrackVolume, 1)),
				VolumeSmoothTime * Time.unscaledDeltaTime
			);
		}
	}

	// Updates pitch by reading targets and lerping.
	void UpdateSoundtracksPitch ()
	{
		// Updates target pitch for bass track. 
		// We only need to update the bass track as the other layers of audio have a pitch sync script taking care of this.
		LayerSources[0].pitch = Mathf.Lerp (
			LayerSources[0].pitch, 
			BassTargetPitch, 
			PitchSmoothTime * Time.unscaledDeltaTime
		);
	}

	// Updates the current track name string value.
	void UpdateTrackNames ()
	{
		TrackName = TrackNames [TrackNumber];
	}

	// Replaces audio clips in the specified audio source by index.
	public void LoadTracks ()
	{
		TrackName = TrackNames [TrackNumber];

		// Assigns clips to audio sources.

		for (int i = 0; i < LayerSources.Length; i++) 
		{
			LayerSources [i].clip = TracksByLayer [i].clips [TrackNumber];
			LayerSources [i].Play ();
		}

		// Loop through beat detection tracks, assign beat detection clip.
		for (int i = 0; i < BeatDetectionTracks.Length; i++)
		{
			switch (i) 
			{
			case 0:
				BeatDetectionTracks [i].clip = BeatDetectionLayerOneTracks [i];
				break;
			case 1:
				BeatDetectionTracks [i].clip = BeatDetectionLayerTwoTracks [i];
				break;
			case 2:
				BeatDetectionTracks [i].clip = BeatDetectionLayerThreeTracks [i];
				break;
			case 3:
				BeatDetectionTracks [i].clip = BeatDetectionLayerFourTracks [i];
				break;
			}
				
			BeatDetectionTracks [i].Play ();
		}

		// Reset beat amounts.
		Beats = 1;
		TimeSinceTrackLoad = 0;
	}

	// Increase the soundtrack index and replace the audio of it. 
	public void NextTrack ()
	{
		// Increase by sequential order.
		if (TrackSequenceMode == trackSequence.Sequential) 
		{
			if (TrackNumber < TracksByLayer[0].clips.Count) 
			{
				TrackNumber += 1;
			}

			if (TrackNumber >= TracksByLayer[0].clips.Count) 
			{
				TrackNumber = 0;
			}
		}

		// Randomize.
		if (TrackSequenceMode == trackSequence.Random) 
		{
			TrackNumber = Random.Range (0, TracksByLayer[0].clips.Count);
		}

		LoadTracks ();
	}

	// Decrease the soundtrack index and replace the audio of it. 
	public void PreviousTrack ()
	{
		// Increase by sequencial order.
		if (TrackSequenceMode == trackSequence.Sequential) 
		{
			if (TrackNumber <= 0) 
			{
				TrackNumber = TracksByLayer[0].clips.Count;
			}

			if (TrackNumber > 0) 
			{
				TrackNumber -= 1;
			}
		}

		// Randomize.
		if (TrackSequenceMode == trackSequence.Random) 
		{
			//TrackNumber = Random.Range (0, BassTracks.Length);
			TrackNumber = Random.Range (0, TracksByLayer[0].clips.Count);
		}

		LoadTracks ();
	}

	// Set a random track.
	public void RandomTrack ()
	{
		//TrackNumber = Random.Range (0, BassTracks.Length);
		TrackNumber = Random.Range (0, TracksByLayer[0].clips.Count);
		LoadTracks ();
	}

	// Pause all currently playing soundtrack audio sources.
	public void StopAllSoundtracks ()
	{
		for (int i = 0; i < LayerSources.Length; i++) 
		{
			LayerSources [i].Pause ();
		}

		foreach (AudioSource beatdetection in BeatDetectionTracks) 
		{
			beatdetection.Pause ();
		}
	}

	public void FadeOutSceneAudio ()
	{
		SetTargetLowPassFreq (0);
		SetTargetResonance (3);
	}

	void UpdateTimeSinceTrackLoad ()
	{
		TimeSinceTrackLoad += Time.deltaTime;

		if (GameController.Instance.Lives <= 0) 
		{
			if (LayerSources[0].isPlaying == true) 
			{
				StopAllSoundtracks ();
			}
		}
	}
}