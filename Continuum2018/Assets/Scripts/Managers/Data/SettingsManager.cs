using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SettingsManager : MonoBehaviour 
{
	public SaveAndLoadScript saveAndLoadScript;
	public string HighEndPresetText = "High";
	public string LowEndPresetText = "Low";
	public Slider QualityPresetSlider;
	public TextMeshProUGUI QualityPresetText;

	public Slider MasterVolumeSlider;
	public TextMeshProUGUI MasterVolumeValueText;
	public Slider SoundtrackVolumeSlider;
	public TextMeshProUGUI SoundtrackVolumeValueText;
	public Slider EffectsVolumeSlider;
	public TextMeshProUGUI EffectsVolumeValueText;

	void Start () 
	{
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();
	}
		
	public void SetQualityIndex ()
	{
		saveAndLoadScript.QualitySettingsIndex = (int)QualityPresetSlider.value;
		Debug.Log ("Quality Settings index set to: " + saveAndLoadScript.QualitySettingsIndex);
	}

	public void UpdateQualitySettingsIndexText ()
	{
		if (QualityPresetSlider.value == 0) 
		{
			QualityPresetText.text = LowEndPresetText;
		}

		if (QualityPresetSlider.value == 1) 
		{
			QualityPresetText.text = HighEndPresetText;
		}
	}

	public void UpdateMasterVolumeText ()
	{
		MasterVolumeValueText.text = "" + System.Math.Round (MasterVolumeSlider.value, 2);
	}

	public void UpdateSoundtrackVolumeText ()
	{
		SoundtrackVolumeValueText.text = "" + System.Math.Round (SoundtrackVolumeSlider.value, 2);
	}

	public void EffectsMasterVolumeText ()
	{
		EffectsVolumeValueText.text = "" + System.Math.Round (EffectsVolumeSlider.value, 2);
	}

	// These sliders are to be called on "OnEndEdit" in the button script.
	public void UpdateMasterVolumeSlider ()
	{
		saveAndLoadScript.MasterVolume = MasterVolumeSlider.value;
		AudioListener.volume = MasterVolumeSlider.value;
		Debug.Log ("Master Volume value = " + saveAndLoadScript.MasterVolume);
	}

	public void UpdateSoundtrackVolumeSlider ()
	{
		saveAndLoadScript.SoundtrackVolume = SoundtrackVolumeSlider.value;
		Debug.Log ("Soundtrack Volume value = " + saveAndLoadScript.SoundtrackVolume);
	}

	public void UpdateEffectsVolumeSlider ()
	{
		saveAndLoadScript.EffectsVolume = EffectsVolumeSlider.value;
		Debug.Log ("Effects Volume value = " + saveAndLoadScript.EffectsVolume);
	}

	public void ApplySettings ()
	{
		saveAndLoadScript.SaveSettingsData ();
		saveAndLoadScript.LoadSettingsData ();
	}

	public void RevertSettings ()
	{
		saveAndLoadScript.LoadSettingsData ();

		QualityPresetSlider.value = saveAndLoadScript.QualitySettingsIndex;
		MasterVolumeSlider.value = saveAndLoadScript.MasterVolume;
		SoundtrackVolumeSlider.value = saveAndLoadScript.SoundtrackVolume;
		EffectsVolumeSlider.value = saveAndLoadScript.EffectsVolume;

		UpdateQualitySettingsIndexText ();
		UpdateMasterVolumeText ();
		UpdateSoundtrackVolumeText ();
		EffectsMasterVolumeText ();
	}
}
