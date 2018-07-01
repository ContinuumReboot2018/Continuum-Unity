using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;
using UnityStandardAssets.ImageEffects;
using UnityEngine.Audio;

using TMPro;

public class SettingsManager : MonoBehaviour 
{
	public static SettingsManager Instance { get; private set; }

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

	void Awake ()
	{
		Instance = this;
		// DontDestroyOnLoad (gameObject);
	}

	void Start ()
	{
		SaveAndLoadScript.Instance.VisualSettingsComponent = postProcessingBehaviourComponent;
		SaveAndLoadScript.Instance.cam = cam;

		UpdateVisuals ();
		UpdateVolumeTextValues ();
	}

	public void SaveSettings ()
	{
		SaveAndLoadScript.Instance.SaveSettingsData ();
	}

	public void LoadSettings ()
	{
		SaveAndLoadScript.Instance.LoadSettingsData ();
	}

	void Update ()
	{
		GetSoundtrackVolumeValue ();
		GetEffectsVolumeValue ();
	}

	// VISUALS

	public void OnQualitySettingsButtonClick (int QualityIndex)
	{
		SaveAndLoadScript.Instance.QualitySettingsIndex = QualityIndex;
		Debug.Log ("Quality Settings index set to: " + SaveAndLoadScript.Instance.QualitySettingsIndex);
	}

	public void UpdateVisuals ()
	{
		// Low visual quality settings.
		if (SaveAndLoadScript.Instance.QualitySettingsIndex == 0) 
		{
			QualitySettings.SetQualityLevel (0);

			SaveAndLoadScript.Instance.ParticleEmissionMultiplier = 0.25f;

			cam.allowHDR = false;
			SaveAndLoadScript.Instance.useHdr = false;

			SaveAndLoadScript.Instance.sunShaftsEnabled = false;
			cam.GetComponent<SunShafts> ().enabled = false;

			postProcessingBehaviourComponent.enabled = false;
			fastMobileBloomScript.enabled = true;
			volLightRend.enabled = false;
		}

		// High visual quality settings.
		if (SaveAndLoadScript.Instance.QualitySettingsIndex == 1) 
		{
			QualitySettings.SetQualityLevel (1);

			SaveAndLoadScript.Instance.ParticleEmissionMultiplier = 1f;

			cam.allowHDR = true;
			SaveAndLoadScript.Instance.useHdr = true;

			SaveAndLoadScript.Instance.sunShaftsEnabled = true;
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
			SaveAndLoadScript.Instance.MasterVolume += 0.1f;
			AudioListener.volume = SaveAndLoadScript.Instance.MasterVolume;
			UpdateVolumeTextValues ();
		}
	}

	public void MasterVolumeDownOnClick ()
	{
		if (AudioListener.volume > 0) 
		{
			SaveAndLoadScript.Instance.MasterVolume -= 0.1f;
			AudioListener.volume = SaveAndLoadScript.Instance.MasterVolume;
			UpdateVolumeTextValues ();
		}
	}
		
	public void SoundtrackVolumeUpOnClick ()
	{
		SaveAndLoadScript.Instance.SoundtrackVolume += 8f;
		UpdateSoundtrackVol ();
	}

	public void SoundtrackVolumeDownOnClick ()
	{
		SaveAndLoadScript.Instance.SoundtrackVolume -= 8f;
		UpdateSoundtrackVol ();
	}

	void UpdateSoundtrackVol ()
	{
		SaveAndLoadScript.Instance.SoundtrackVolume = Mathf.Clamp (SaveAndLoadScript.Instance.SoundtrackVolume, -80, 0);
		curSoundtrackVol = SaveAndLoadScript.Instance.SoundtrackVolume;
		SoundtrackVolMix.SetFloat ("SoundtrackVolume", curSoundtrackVol);
		UpdateVolumeTextValues ();
	}

	public void EffectsVolumeUpOnClick ()
	{
		SaveAndLoadScript.Instance.EffectsVolume += 8f;
		UpdateEffectsVol ();
	}

	public void EffectsVolumeDownOnClick ()
	{
		SaveAndLoadScript.Instance.EffectsVolume -= 8f;
		UpdateEffectsVol ();
	}

	void UpdateEffectsVol ()
	{
		SaveAndLoadScript.Instance.EffectsVolume = Mathf.Clamp (SaveAndLoadScript.Instance.EffectsVolume, -80, 0);
		curEffectsVol = SaveAndLoadScript.Instance.EffectsVolume;
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
			SaveAndLoadScript.Instance.MasterVolume, 1).ToString ();
		
		SoundtrackVolumeValueText.text = (1 +
			System.Math.Round ((0.0125f * SaveAndLoadScript.Instance.SoundtrackVolume), 1)
		).ToString ();
		
		EffectsVolumeValueText.text = (1 +
			System.Math.Round ((0.0125f * SaveAndLoadScript.Instance.EffectsVolume), 1)
		).ToString ();
	}
		

	// Saving and applying settings.

	public void ApplySettings ()
	{
		SaveAndLoadScript.Instance.SaveSettingsData ();
		SaveAndLoadScript.Instance.LoadSettingsData ();
	}

	public void RevertSettings ()
	{
		SaveAndLoadScript.Instance.LoadSettingsData ();
		RefreshSettings ();
	}

	public void RefreshSettings ()
	{
		UpdateVolumeTextValues ();
	}
}