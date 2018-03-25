using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Networking;

public class AudioController : MonoBehaviour 
{
	public PlayerController playerControllerScript_P1;
	public GameController gameControllerScript;
	public TimescaleController timescaleControllerScript;
	public SaveAndLoadScript saveAndLoadScript;
	public SimpleBeatDetection beatDetector;
	public SubbandBeatDetection beatDetectorSubBand;

	public bool updateVolumeAndPitches = true; // If true, the music will update to volume and pitch.
	public float Distance; // Gets distance from timescaleControllerScript.

	//public UnityWebRequestMultimedia

	public AudioMixer AudioMix; // Audio mixer on soundtracks.
	public float curFreq; // Current low pass frequency from mixer.
	public float TargetCutoffFreq; // Target low pass frequency for mixer.
	public float CutoffFreqSmoothing; // Low pass frequency smooth time.
	public float curRes; // Current low pass resonance from mixer.
	public float TargetResonance; // Target low pass resonance for mixer.
	public float ResonanceSmoothing; // Low pass resonance smooth time.

	// Distances to edit
	[Header ("Distance values")]
	public float BaseDistance; // The distance at which any lower will give the lowest assigned audio pitch.
	public float LowDistance; // Higher = 1.0f, Lower = 0.75f.
	public float MediumDistance; // Higher = 1.25f, Lower = 1.0f.
	public float HighDistance; // The distance at which any higher will give the highest assigned audio pitch.

	[Header ("Track Sequence")]
	public int TrackNumber; // Current track number referenced in array.
	public string TrackName; // The track name based on track number.
	public string[] TrackNames; // List of all track names.
	public trackSequence TrackSequenceMode; // Can change the way tracks are sequenced.
	public enum trackSequence
	{
		Sequential,
		Random,
	}

	// The soundtrack audio sources.
	public AudioSource BassTrack; // Plays bass tracks.
	public AudioSource LayerOneTrack; // Plays layer one tracks.
	public AudioSource LayerTwoTrack; // Plays layer two tracks.
	public AudioSource LayerThreeTrack; // Plays layer three tracks.

	[Header ("Soundtrack Library")]
	// A library of all each type of track.
	public AudioClip[] BassTracks; // Bassdrums, main beat, bed.
	public AudioClip[] LayerOneTracks; // Bass synths, Pads.
	public AudioClip[] LayerTwoTracks; // Mains and lead synths.
	public AudioClip[] LayerThreeTracks; // Riffs, arps, all sorts of cool audio flourishes.

	[Header ("Volume")]
	public float BaseTargetVolume; // Bass current volume lerps to this value.
	public float LayerOneTargetVolume; // Layer one current volume lerps to this value.
	public float LayerTwoTargetVolume; // Layer two current volume lerps to this value.
	public float LayerThreeTargetVolume; // Layer three current volume lerps to this value.
	public float VolumeSmoothTime;

	public Vector4 BassVolume, LayerOneVolume, LayerTwoVolume, LayerThreeVolume; // Volume values based on distance value.

	[Header ("Pitch")]
	public float BassTargetPitch; // Current bass track pitch lerps to this value, other tracks synchronize automatically.
	public float PitchSmoothTime; // Pitch smoothing time.

	public Vector4 TimePitch; // Pitch values based on distance values.
	public bool ReversePitch;

	void Start ()
	{
		TargetCutoffFreq = 22000; // Set target cutoff frequency to max value (22kHz).
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();
		LoadTracks (); // Load the track by track number.
		InvokeRepeating ("CheckReversePitch", 0, 0.5f); // If in rewind, check for reversing the pitch.
		beatDetector.OnBeat += OnBeat;
	}

	void OnBeat ()
	{
		Debug.Log ("Beat");
	}

	void Update ()
	{
		UpdateAudio (); // Method to update audio states.
		GetMasterLowPassValue (); // Update low pass filter cutoff frequency.
		GetMasterResonanceValue (); // Update low pass filter resonance amount.
		SetFilterEffectAmounts (); // Sets filter properties based on audio states.
	}
		
	void UpdateAudio ()
	{
		if (gameControllerScript.isPaused == false && 
			timescaleControllerScript.isInInitialCountdownSequence == false && 
			timescaleControllerScript.isInInitialSequence == false)
		{
			UpdateSoundtrackVolumeAndPitches (); // Update sound pitch based on distance.
			//UpdateStereoUI ();
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
		AudioMix.SetFloat ("LowCutoffFrequency", SmoothLowFreqVal);

		// Sets low pass filter resonance value.
		float SmoothResVal = Mathf.Lerp (curRes, TargetResonance, ResonanceSmoothing * Time.unscaledDeltaTime);
		AudioMix.SetFloat ("Resonance", SmoothResVal);
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
		bool curFreqResult = AudioMix.GetFloat ("LowCutoffFrequency", out curFreq);

		if (curFreqResult) {
			return curFreq;
		} else {
			return 0f;
		}
	}

	// Gets current low pass resonance value.
	public float GetMasterResonanceValue ()
	{
		bool curResResult = AudioMix.GetFloat ("Resonance", out curRes);

		if (curResResult) {
			return curRes;
		} else {
			return 0f;
		}
	}

	/*void UpdateStereoUI ()
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
	}*/

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
		BassTrack.volume = Mathf.Lerp (
			BassTrack.volume, 
			BaseTargetVolume + (saveAndLoadScript.SoundtrackVolume - 1), 
			VolumeSmoothTime * Time.unscaledDeltaTime
		);

		LayerOneTrack.volume = Mathf.Lerp (
			LayerOneTrack.volume, 
			LayerOneTargetVolume + (saveAndLoadScript.SoundtrackVolume - 1), 
			VolumeSmoothTime * Time.unscaledDeltaTime
		);

		LayerTwoTrack.volume = Mathf.Lerp (
			LayerTwoTrack.volume, 
			LayerTwoTargetVolume + (saveAndLoadScript.SoundtrackVolume - 1), 
			VolumeSmoothTime * Time.unscaledDeltaTime
		);

		LayerThreeTrack.volume = Mathf.Lerp (
			LayerThreeTrack.volume, 
			LayerThreeTargetVolume + (saveAndLoadScript.SoundtrackVolume - 1), 
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
	}

	public void FadeOutSceneAudio ()
	{
		SetTargetLowPassFreq (0);
		SetTargetResonance (3);
	}
}