using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.PostProcessing;
using UnityStandardAssets.ImageEffects;
using UnityEngine.SceneManagement;

public class SaveAndLoadScript : MonoBehaviour 
{
	public SettingsManager settingsManagerScript;
	public PlayerController playerControllerScript;
	public GameController gameControllerScript;

	// Live variables.
	[Header ("Player Data")]
	public string Username = "default";
	public int ExperiencePoints;

	[Header ("Settings Data")]
	public PostProcessingProfile VisualSettings;
	public PostProcessingBehaviour VisualSettingsComponent;
	public Camera cam;
	public int QualitySettingsIndex;
	public bool useHdr;
	public bool sunShaftsEnabled;
	public bool noiseGrainEnabled;

	public float MasterVolume;
	public float SoundtrackVolume;
	public float EffectsVolume;

	public playerData PlayerData;
	public settingsData SettingsData;

	void Awake () 
	{
		if (SceneManager.GetActiveScene ().name != "Init")
		{
			settingsManagerScript = GameObject.Find ("SettingsManager").GetComponent<SettingsManager> ();
			playerControllerScript = GameObject.Find ("PlayerController").GetComponent<PlayerController> ();
			gameControllerScript = GameObject.Find ("GameController").GetComponent<GameController> ();
			cam = settingsManagerScript.cam;
			VisualSettingsComponent = cam.GetComponent<PostProcessingBehaviour> ();
			LoadPlayerData ();
			LoadSettingsData ();
			CheckUsername ();
		}
	}

	void CheckUsername ()
	{
		if (Username == null || Username == "") 
		{
			Username = "default";
		}

		if (Username == "default") 
		{
			Debug.Log ("Username is " + Username + ". Consider changing your username in the menu. " +
				"You may not have created a local profile yet.");
		}
	}
		
	// Gets variables from this script = variables in other scripts.
	void GetPlayerData ()
	{
		ExperiencePoints += (int)Math.Round(gameControllerScript.TargetScore);
	}

	// Save PlayerData Main.
	public void SavePlayerData ()
	{
		// Refer to GetPlayerData.
		GetPlayerData ();

		// Creates new save file.
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat");

		// Does the saving
		playerData data = new playerData ();
		SetPlayerData (data);

		// Serializes and closes the file.
		bf.Serialize (file, data);
		file.Close ();

		Debug.Log 
		(
			"Successfully saved to " +
			Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat"
		); 
	}

	// Sets data.[variable] = [variable] from this script.
	void SetPlayerData (playerData data)
	{
		data.Username = Username;
		data.ExperiencePoints = ExperiencePoints;
	}

	// Load PlayerData main.
	public void LoadPlayerData ()
	{
		if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat") == true) 
		{
			// Opens the save data.
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat", FileMode.Open);

			// Processes the save data into memory.
			playerData data = (playerData)bf.Deserialize (file);
			file.Close ();

			LoadPlayerDataContents (data);
			StorePlayerDataInGame ();

			Debug.Log ("Successfully loaded from " +
				Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat");
		}

		if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat") == false) 
		{
			Debug.Log ("Unable to load from " +
				Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat");
		}
	}

	// Sets variables in this script by getting data from save file. 
	void LoadPlayerDataContents (playerData data)
	{
		Username = data.Username;
		ExperiencePoints = data.ExperiencePoints;
	}

	// Puts new data into relevant scripts.
	void StorePlayerDataInGame ()
	{

	}

	// Gets variables from this script = variables in other scripts.
	void GetSettingsData ()
	{
		QualitySettingsIndex = (int)settingsManagerScript.QualityPresetSlider.value;
		QualitySettings.SetQualityLevel (QualitySettingsIndex);

		if (QualitySettingsIndex == 0) 
		{
			VisualSettingsComponent.enabled = false;
			sunShaftsEnabled = false;
			noiseGrainEnabled = false;
			useHdr = false;
		}

		if (QualitySettingsIndex == 1) 
		{
			VisualSettingsComponent.enabled = true;
			sunShaftsEnabled = true;
			noiseGrainEnabled = true;
			useHdr = true;
		}

		MasterVolume 	  = Mathf.Clamp (AudioListener.volume, 	   0, 1);
		SoundtrackVolume  = settingsManagerScript.SoundtrackVolumeSlider.value;
		EffectsVolume 	  = settingsManagerScript.EffectsVolumeSlider.value;
	}

	// Save Settings Main.
	public void SaveSettingsData ()
	{
		// Refer to GetSettingsData.
		GetSettingsData ();

		// Creates new save file.
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat");

		// Does the saving
		settingsData data = new settingsData ();
		SetSettingsData (data);

		// Serializes and closes the file.
		bf.Serialize (file, data);
		file.Close ();

		Debug.Log 
		(
			"Successfully saved to " +
			Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat"
		); 
	}

	// Sets data.[variable] = [variable] from this script.
	void SetSettingsData (settingsData data)
	{
		data.QualitySettingsIndex = QualitySettingsIndex;

		if (data.QualitySettingsIndex == 0) 
		{
			data.useHdr = false;
			data.sunShaftsEnabled = false;
			data.noiseGrainEnabled = false;
		}

		if (data.QualitySettingsIndex == 1) 
		{
			data.useHdr = true;
			data.sunShaftsEnabled = true;
			data.noiseGrainEnabled = true;
		}

		data.MasterVolume 	  = Mathf.Clamp (MasterVolume, 	   0, 1);
		data.SoundtrackVolume = Mathf.Clamp (SoundtrackVolume, 0, 1);
		data.EffectsVolume 	  = Mathf.Clamp (EffectsVolume,    0, 1);
	}

	public void LoadSettingsData ()
	{
		if (File.Exists (Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat") == true) 
		{
			// Opens the save data.
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat", FileMode.Open);

			// Processes the save data into memory.
			settingsData data = (settingsData)bf.Deserialize (file);
			file.Close ();

			LoadSettingsDataContents (data);
			StoreSettingsDataInGame ();

			Debug.Log ("Successfully loaded from " +
				Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat");
		}

		if (File.Exists (Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat") == false) 
		{
			Debug.Log ("Unable to load from " +
				Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat");
		}
	}

	// Sets variables in this script by getting data from save file. 
	void LoadSettingsDataContents (settingsData data)
	{
		QualitySettingsIndex = data.QualitySettingsIndex;

		if (QualitySettingsIndex == 0) 
		{
			VisualSettingsComponent.enabled = false;
			useHdr = false;
			sunShaftsEnabled = false;
			noiseGrainEnabled = false;
		}

		if (QualitySettingsIndex == 1) 
		{
			VisualSettingsComponent.enabled = true;
			useHdr = true;
			sunShaftsEnabled = true;
			noiseGrainEnabled = true;
		}
			
		MasterVolume = data.MasterVolume;
		SoundtrackVolume = data.SoundtrackVolume;
		EffectsVolume = data.EffectsVolume;
	}

	// Puts new data into relevant scripts.
	void StoreSettingsDataInGame ()
	{
		QualitySettings.SetQualityLevel (QualitySettingsIndex);
		CheckAndApplyQualitySettings ();
		AudioListener.volume = Mathf.Clamp (MasterVolume, 0, 1);
	}

	void CheckAndApplyQualitySettings ()
	{
		if (QualitySettingsIndex == 0) 
		{
			VisualSettingsComponent.enabled = false;
			useHdr = false;
			sunShaftsEnabled = false;
			noiseGrainEnabled = false;
		}

		if (QualitySettingsIndex == 1) 
		{
			VisualSettingsComponent.enabled = true;
			useHdr = true;
			sunShaftsEnabled = true;
			noiseGrainEnabled = true;
		}
			
		cam.allowHDR = useHdr;
		cam.GetComponent<SunShafts> ().enabled = sunShaftsEnabled;
		cam.GetComponent<NoiseAndGrain> ().enabled = noiseGrainEnabled;
	}

	// Variables stored in data files.
	[Serializable]
	public class playerData
	{
		public string Username;
		public int ExperiencePoints;
	}

	[Serializable]
	public class settingsData
	{
		public int QualitySettingsIndex;
		public bool useHdr;
		public bool sunShaftsEnabled;
		public bool noiseGrainEnabled;

		public float MasterVolume;
		public float SoundtrackVolume;
		public float EffectsVolume;
	}
}
