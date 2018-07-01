using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.PostProcessing;

public class MenuInitializer : MonoBehaviour 
{
	public PostProcessingProfile postProcess;
	public float bloomIntensity;

	public AudioMixer SoundtrackAudioMix;
	public AudioMixer EffectsAudioMix;

	private float curFreq = 22000.0f;
	private float curEffectsFreq = 22000.0f;
	private float curRes = 0.0f;
	private float curEffectsRes = 0.0f;

	void Start () 
	{
		AudioListener.volume = SaveAndLoadScript.Instance.MasterVolume;

		SoundtrackAudioMix.SetFloat ("LowCutoffFrequency", curFreq);
		EffectsAudioMix.SetFloat ("LowCutoffFrequency", curEffectsFreq);
		SoundtrackAudioMix.SetFloat ("Resonance", curRes);
		EffectsAudioMix.SetFloat ("Resonance", curEffectsRes);

		var bloomsettings = postProcess.bloom.settings;
		bloomsettings.bloom.intensity = bloomIntensity;
		postProcess.bloom.settings = bloomsettings;
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
}