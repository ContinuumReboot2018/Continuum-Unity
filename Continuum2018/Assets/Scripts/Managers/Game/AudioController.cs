using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
//using UnityEngine.Networking;

public class AudioController : MonoBehaviour 
{
	public PlayerController playerControllerScript_P1;
	public GameController gameControllerScript;
	public TimescaleController timescaleControllerScript;
	public SaveAndLoadScript saveAndLoadScript;

	[Tooltip ("If true, the music will update to volume and pitch.")]
	public bool updateVolumeAndPitches = true;
	[Tooltip ("Gets distance from timescaleControllerScript.")]
	public float Distance;

	//public UnityWebRequestMultimedia

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
	[Tooltip ("The distance at which any lower will give the lowest assigned audio pitch.")]
	public float BaseDistance;
	[Tooltip ("Higher = 1.0f, Lower = 0.75f.")]
	public float LowDistance;
	[Tooltip ("Higher = 1.25f, Lower = 1.0f.")]
	public float MediumDistance;
	[Tooltip ("The distance at which any higher will give the highest assigned audio pitch.")]
	public float HighDistance;

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

	// The soundtrack audio sources.
	[Tooltip ("Plays bass tracks.")]
	public AudioSource BassTrack;
	[Tooltip ("Plays layer one tracks.")]
	public AudioSource LayerOneTrack;
	[Tooltip ("Plays layer two tracks.")]
	public AudioSource LayerTwoTrack;
	[Tooltip ("Plays layer three tracks.")]
	public AudioSource LayerThreeTrack;
	[Tooltip ("Plays beat detection tracks.")]
	public AudioSource[] BeatDetectionTracks;

	[Header ("Soundtrack Library")]
	// A library of all each type of track.
	[Tooltip ("Bassdrums, main beat, bed.")]
	public AudioClip[] BassTracks;
	[Tooltip ("Bass synths, Pads.")]
	public AudioClip[] LayerOneTracks;
	[Tooltip ("Mains and lead synths.")]
	public AudioClip[] LayerTwoTracks; 
	[Tooltip ("Riffs, arps, all sorts of cool audio flourishes.")]
	public AudioClip[] LayerThreeTracks;
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
	[Tooltip ("Bass current volume lerps to this value.")]
	public float BaseTargetVolume;
	[Tooltip ("Layer one current volume lerps to this value.")]
	public float LayerOneTargetVolume;
	[Tooltip ("Layer two current volume lerps to this value.")]
	public float LayerTwoTargetVolume;
	[Tooltip ("Layer two current volume lerps to this value.")]
	public float LayerThreeTargetVolume;
	[Tooltip ("Layer three current volume lerps to this value.")]
	public float VolumeSmoothTime;
	[Tooltip ("Volume values based on distance value.")]
	public Vector4 BassVolume, LayerOneVolume, LayerTwoVolume, LayerThreeVolume;

	[Header ("Pitch")]
	[Tooltip ("Current bass track pitch lerps to this value, other tracks synchronize automatically.")]
	public float BassTargetPitch;
	[Tooltip ("Pitch smoothing time.")]
	public float PitchSmoothTime;
	[Tooltip ("Pitch values based on distance values.")]
	public Vector4 TimePitch;
	[Tooltip ("Is pitch being reversed?")]
	public bool ReversePitch;

	void Start ()
	{
		TargetCutoffFreq = 22000; // Set target cutoff frequency to max value (22kHz).
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();

		// Randomize track if on random mode.
		if (TrackSequenceMode == trackSequence.Random) 
		{
			TrackNumber = Random.Range (0, BassTracks.Length);
		}

		LoadTracks (); // Load the track by track number.
		InvokeRepeating ("CheckReversePitch", 0, 0.5f); // If in rewind, check for reversing the pitch.
		AudioListener.volume = saveAndLoadScript.MasterVolume;
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
		if (gameControllerScript.isPaused == false && 
			timescaleControllerScript.isInInitialCountdownSequence == false && 
			timescaleControllerScript.isInInitialSequence == false)
		{
			UpdateSoundtrackVolumeAndPitches (); // Update sound pitch based on distance.
		}

		if (timescaleControllerScript.isOverridingTimeScale == true) 
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
		ReversePitch = timescaleControllerScript.isRewinding;
	}

	// Gets current low pass cutoff frequency value.
	public float GetMasterLowPassValue ()
	{
		bool curFreqResult = SoundtrackAudioMix.GetFloat ("LowCutoffFrequency", out curFreq);

		if (curFreqResult) 
		{
			return curFreq;
		}

		else
		
		{
			return 0f;
		}
	}

	// Gets current low pass resonance value.
	public float GetMasterResonanceValue ()
	{
		bool curResResult = SoundtrackAudioMix.GetFloat ("Resonance", out curRes);

		if (curResResult) 
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

		if (curFreqEffectsResult) 
		{
			return curEffectsFreq;
		}

		else

		{
			return 0f;
		}
	}

	// Gets current low pass resonance value.
	public float GetEffectsResonanceValue ()
	{
		bool curEffectsResResult = EffectsAudioMix.GetFloat ("Resonance", out curEffectsRes);

		if (curEffectsResResult) 
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
			Distance = timescaleControllerScript.Distance;

			UpdateSoundtracksVolume ();
			UpdateSoundtracksPitch ();

			UpdateTargetVolumes ();
			UpdateTargetPitches ();
		}
	}

	// Updates target volume values for the sountrack to interpolate towards.
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

	// Updates target pitch values for the sountrack to interpolate towards.
	void UpdateTargetPitches ()
	{
		// Base distance.
		if (Distance < BaseDistance) 
		{
			BassTargetPitch = TimePitch.x * (ReversePitch ? -1 : 1);
		}

		// Low distance.
		if (Distance >= BaseDistance && Distance < LowDistance) 
		{
			BassTargetPitch = TimePitch.y * (ReversePitch ? -1 : 1);
		}

		// Medium distance.
		if (Distance >= LowDistance && Distance < MediumDistance) 
		{
			BassTargetPitch = TimePitch.z * (ReversePitch ? -1 : 1);
		}

		// High distance.
		if (Distance >= MediumDistance && Distance < HighDistance) 
		{
			BassTargetPitch = TimePitch.w * (ReversePitch ? -1 : 1);
		}

		// Top distance.
		if (Distance >= HighDistance) 
		{
			BassTargetPitch = 1.5f * (ReversePitch ? -1 : 1);
		}
	}

	// Updates volume by reading targets and lerping.
	void UpdateSoundtracksVolume ()
	{
		// Updates target volumes.
		// We need to compensate for how the mmixer volume is displayed in dB (-80 to 0).

		BassTrack.volume = Mathf.Lerp (
			BassTrack.volume, 
			BaseTargetVolume + (1 + (float)System.Math.Round (0.0125f * saveAndLoadScript.SoundtrackVolume, 1)), 
			VolumeSmoothTime * Time.unscaledDeltaTime
		);

		LayerOneTrack.volume = Mathf.Lerp (
			LayerOneTrack.volume, 
			LayerOneTargetVolume + (1 + (float)System.Math.Round (0.0125f * saveAndLoadScript.SoundtrackVolume, 1)), 
			VolumeSmoothTime * Time.unscaledDeltaTime
		);

		LayerTwoTrack.volume = Mathf.Lerp (
			LayerTwoTrack.volume, 
			LayerTwoTargetVolume + (1 + (float)System.Math.Round (0.0125f * saveAndLoadScript.SoundtrackVolume, 1)), 
			VolumeSmoothTime * Time.unscaledDeltaTime
		);

		LayerThreeTrack.volume = Mathf.Lerp (
			LayerThreeTrack.volume, 
			LayerThreeTargetVolume + (1 + (float)System.Math.Round (0.0125f * saveAndLoadScript.SoundtrackVolume, 1)), 
			VolumeSmoothTime * Time.unscaledDeltaTime
		);
	}

	// Updates pitch by reading targets and lerping.
	void UpdateSoundtracksPitch ()
	{
		// Updates target pitch for bass track. 
		// We only need to update the bass track as the other layers of audio have a pitch sync script taking care of this.
		BassTrack.pitch = Mathf.Lerp (
			BassTrack.pitch, 
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
		// Assigns clips to audio sources.
		TrackName 			 = TrackNames 		[TrackNumber];
		BassTrack.clip 		 = BassTracks 		[TrackNumber];
		LayerOneTrack.clip	 = LayerOneTracks 	[TrackNumber];
		LayerTwoTrack.clip 	 = LayerTwoTracks 	[TrackNumber];
		LayerThreeTrack.clip = LayerThreeTracks [TrackNumber];

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

		// Plays all audio sources.
		BassTrack.Play ();
		LayerOneTrack.Play ();
		LayerTwoTrack.Play ();
		LayerThreeTrack.Play ();

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
			if (TrackNumber < BassTracks.Length) 
			{
				TrackNumber += 1;
			}

			if (TrackNumber >= BassTracks.Length) 
			{
				TrackNumber = 0;
			}
		}

		// Randomize.
		if (TrackSequenceMode == trackSequence.Random) 
		{
			TrackNumber = Random.Range (0, BassTracks.Length);
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
				TrackNumber = BassTracks.Length;
			}

			if (TrackNumber > 0) 
			{
				TrackNumber -= 1;
			}
		}

		// Randomize.
		if (TrackSequenceMode == trackSequence.Random) 
		{
			TrackNumber = Random.Range (0, BassTracks.Length);
		}

		LoadTracks ();
	}

	// Set a random track.
	public void RandomTrack ()
	{
		TrackNumber = Random.Range (0, BassTracks.Length);
		LoadTracks ();
	}

	// Pause all currently playing soundtrack audio sources.
	public void StopAllSoundtracks ()
	{
		BassTrack.Pause ();
		LayerOneTrack.Pause ();
		LayerTwoTrack.Pause ();
		LayerThreeTrack.Pause ();

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

		if (gameControllerScript.Lives <= 0) 
		{
			if (BassTrack.isPlaying == true) 
			{
				StopAllSoundtracks ();
			}
		}
	}
}