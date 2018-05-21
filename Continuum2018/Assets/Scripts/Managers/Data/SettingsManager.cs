using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;
using UnityStandardAssets.ImageEffects;
using UnityEngine.Audio;

using TMPro;

public class SettingsManager : MonoBehaviour 
{
	public SaveAndLoadScript saveAndLoadScript;
	public PostProcessingBehaviour postProcessingBehaviourComponent;
	public FastMobileBloom fastMobileBloomScript;
	public VolumetricLightRenderer volLightRend;

	[Header ("Visual Settings")]
	public Camera cam;

	public Button HighEndQualityButton;
	public Button LowEndQualityButton;

	[Header ("Audio Settings")]
	public Button MasterVolumeButtonUp;
	public Button MasterVolumeButtonDown;
	public TextMeshProUGUI MasterVolumeValueText;

	public AudioMixer SoundtrackVolMix;
	private float curSoundtrackVol;
	public Button SoundtrackVolumeButtonUp;
	public Button SoundtrackVolumeButtonDown;
	public TextMeshProUGUI SoundtrackVolumeValueText;

	public AudioMixer EffectsVolMix;
	private float curEffectsVol;
	public Button EffectsVolumeButtonUp;
	public Button EffectsVolumeButtonDown;
	public TextMeshProUGUI EffectsVolumeValueText;

	void Start ()
	{
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();
		saveAndLoadScript.settingsManagerScript = this; // Assign itself to save and load script.
		saveAndLoadScript.VisualSettingsComponent = postProcessingBehaviourComponent;
		saveAndLoadScript.cam = cam;

		UpdateVisuals ();
		UpdateVolumeTextValues ();
	}

	public void SaveSettings ()
	{
		saveAndLoadScript.SaveSettingsData ();
	}

	public void LoadSettings ()
	{
		saveAndLoadScript.LoadSettingsData ();
	}

	void Update ()
	{
		GetSoundtrackVolumeValue ();
		GetEffectsVolumeValue ();
	}

	// VISUALS

	public void OnQualitySettingsButtonClick (int QualityIndex)
	{
		saveAndLoadScript.QualitySettingsIndex = QualityIndex;
		Debug.Log ("Quality Settings index set to: " + saveAndLoadScript.QualitySettingsIndex);
	}

	public void UpdateVisuals ()
	{
		// Low visual quality settings.
		if (saveAndLoadScript.QualitySettingsIndex == 0) 
		{
			QualitySettings.SetQualityLevel (0);

			saveAndLoadScript.ParticleEmissionMultiplier = 0.25f;

			cam.allowHDR = false;
			saveAndLoadScript.useHdr = false;

			saveAndLoadScript.sunShaftsEnabled = false;
			cam.GetComponent<SunShafts> ().enabled = false;

			postProcessingBehaviourComponent.enabled = false;
			fastMobileBloomScript.enabled = true;
			volLightRend.enabled = false;
		}

		// High visual quality settings.
		if (saveAndLoadScript.QualitySettingsIndex == 1) 
		{
			QualitySettings.SetQualityLevel (1);

			saveAndLoadScript.ParticleEmissionMultiplier = 1f;

			cam.allowHDR = true;
			saveAndLoadScript.useHdr = true;

			saveAndLoadScript.sunShaftsEnabled = true;
			cam.GetComponent<SunShafts> ().enabled = true;

			postProcessingBehaviourComponent.enabled = true;
			fastMobileBloomScript.enabled = false;
			volLightRend.enabled = true;
		}
	}


	// AUDIO

	public void MasterVolumeUpOnClick ()
	{
		if (AudioListener.volume < 1) 
		{
			saveAndLoadScript.MasterVolume += 0.1f;
			AudioListener.volume = saveAndLoadScript.MasterVolume;
			UpdateVolumeTextValues ();
		}
	}

	public void MasterVolumeDownOnClick ()
	{
		if (AudioListener.volume > 0) 
		{
			saveAndLoadScript.MasterVolume -= 0.1f;
			AudioListener.volume = saveAndLoadScript.MasterVolume;
			UpdateVolumeTextValues ();
		}
	}
		
	public void SoundtrackVolumeUpOnClick ()
	{
		saveAndLoadScript.SoundtrackVolume += 8f;
		UpdateSoundtrackVol ();
	}

	public void SoundtrackVolumeDownOnClick ()
	{
		saveAndLoadScript.SoundtrackVolume -= 8f;
		UpdateSoundtrackVol ();
	}

	void UpdateSoundtrackVol ()
	{
		saveAndLoadScript.SoundtrackVolume = Mathf.Clamp (saveAndLoadScript.SoundtrackVolume, -80, 0);
		curSoundtrackVol = saveAndLoadScript.SoundtrackVolume;
		SoundtrackVolMix.SetFloat ("SoundtrackVolume", curSoundtrackVol);
		UpdateVolumeTextValues ();
	}

	public void EffectsVolumeUpOnClick ()
	{
		saveAndLoadScript.EffectsVolume += 8f;
		UpdateEffectsVol ();
	}

	public void EffectsVolumeDownOnClick ()
	{
		saveAndLoadScript.EffectsVolume -= 8f;
		UpdateEffectsVol ();
	}

	void UpdateEffectsVol ()
	{
		saveAndLoadScript.EffectsVolume = Mathf.Clamp (saveAndLoadScript.EffectsVolume, -80, 0);
		curEffectsVol = saveAndLoadScript.EffectsVolume;
		EffectsVolMix.SetFloat ("EffectsVolume", curEffectsVol);
		UpdateVolumeTextValues ();
	}

	// Gets current soundtrack volume from mixer.
	public float GetSoundtrackVolumeValue ()
	{
		bool curVolResult = SoundtrackVolMix.GetFloat ("SoundtrackVolume", out curSoundtrackVol);

		if (curVolResult) 
		{
			return curSoundtrackVol;
		} 

		else 

		{
			return 0f;
		}
	}

	// Gets current effects volume from mixer.
	public float GetEffectsVolumeValue ()
	{
		bool curVolResult = EffectsVolMix.GetFloat ("EffectsVolume", out curEffectsVol);

		if (curVolResult) 
		{
			return curEffectsVol;
		} 

		else 

		{
			return 0f;
		}
	}

	void UpdateVolumeTextValues ()
	{
		MasterVolumeValueText.text = System.Math.Round (
			saveAndLoadScript.MasterVolume, 1).ToString ();
		
		SoundtrackVolumeValueText.text = (1 +
			System.Math.Round ((0.0125f * saveAndLoadScript.SoundtrackVolume), 1)
		).ToString ();
		
		EffectsVolumeValueText.text = (1 +
			System.Math.Round ((0.0125f * saveAndLoadScript.EffectsVolume), 1)
		).ToString ();
	}
		

	// Saving and applying settings.

	public void ApplySettings ()
	{
		saveAndLoadScript.SaveSettingsData ();
		saveAndLoadScript.LoadSettingsData ();
	}

	public void RevertSettings ()
	{
		saveAndLoadScript.LoadSettingsData ();
		RefreshSettings ();
	}

	public void RefreshSettings ()
	{
		UpdateVolumeTextValues ();
	}
}