using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.PostProcessing;
using UnityStandardAssets.ImageEffects;
using UnityEngine.SceneManagement;

public class SaveAndLoadScript : MonoBehaviour 
{
	public SettingsManager settingsManagerScript;
	public PlayerController playerControllerScript_P1;
	public GameController gameControllerScript;

	public bool AllowLoading = true;
	public bool AllowSaving = true;

	public List<LeaderboardEntry> DefaultLeaderboard;

	// Live variables.
	[Header ("Player Data")]
	public string Username = "default";
	public int ExperiencePoints;
	public List<LeaderboardEntry> Leaderboard;

	[Header ("Settings Data")]
	public PostProcessingProfile VisualSettings;
	public PostProcessingBehaviour VisualSettingsComponent;
	public Camera cam;
	[Space (10)]
	public int QualitySettingsIndex;
	public bool useHdr;
	public bool sunShaftsEnabled;

	public TargetFPS targetFramerateScript;
	public int targetframerate;

	[Space (10)]
	[Range (0, 2)]
	public float ParticleEmissionMultiplier = 1;

	[Space (10)]
	public float MasterVolume;
	public float SoundtrackVolume;
	public float EffectsVolume;

	public playerData PlayerData;
	public settingsData SettingsData;

	void Start ()
	{
		if (SceneManager.GetActiveScene ().name != "Init")
		{
			if (AllowLoading == true) 
			{
				CheckPlayerDataFile ();

				settingsManagerScript = GameObject.Find ("SettingsManager").GetComponent<SettingsManager> ();
				
				cam = settingsManagerScript.cam;
				VisualSettingsComponent = cam.GetComponent<PostProcessingBehaviour> ();

				LoadPlayerData ();

				LoadSettingsData ();
				//Invoke ("LoadSettingsData", 0.1f);

				CheckUsername ();
			}
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
		if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat") == true) 
		{
			ExperiencePoints += (int)Math.Round (gameControllerScript.TargetScore);
		}

		if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat") == false) 
		{
			ExperiencePoints = 0;
		}
	}

	// Save PlayerData Main.
	public void SavePlayerData ()
	{
		if (AllowSaving == true) 
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

			Debug.Log (
				"Successfully saved to " +
				Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat"
			); 
		}
	}

	// Sets data.[variable] = [variable] from this script.
	void SetPlayerData (playerData data)
	{
		data.Username = Username;
		data.ExperiencePoints = ExperiencePoints;
		data.Leaderboard = Leaderboard;
	}

	// Load PlayerData main.
	public void LoadPlayerData ()
	{
		if (AllowLoading == true)
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

			CheckPlayerDataFile ();
		}
	}

	void CheckPlayerDataFile ()
	{
		if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat") == false)
		{
			Debug.LogWarning ("Unable to load from " +
				Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat");

			Leaderboard = new List<LeaderboardEntry> (10);

			Leaderboard = DefaultLeaderboard;

			SavePlayerData ();

			Debug.Log ("Saved new player data to " +
				Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat");
		}
	}

	// Sets variables in this script by getting data from save file. 
	void LoadPlayerDataContents (playerData data)
	{
		Username = data.Username;
		ExperiencePoints = data.ExperiencePoints;
		data.Leaderboard.Capacity = 10;
		Leaderboard.Capacity = 10;
		Leaderboard = new List<LeaderboardEntry> (10);
		Leaderboard = data.Leaderboard;
	}

	// Puts new data into relevant scripts.
	void StorePlayerDataInGame ()
	{

	}

	// Gets variables from this script = variables in other scripts.
	void GetSettingsData ()
	{
		if (File.Exists (Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat") == true) 
		{
			QualitySettings.SetQualityLevel (QualitySettingsIndex);

			if (QualitySettingsIndex == 0) 
			{
				VisualSettingsComponent.enabled = false;
				sunShaftsEnabled = false;
				useHdr = false;
				ParticleEmissionMultiplier = 0.25f;
			}

			if (QualitySettingsIndex == 1) 
			{
				VisualSettingsComponent.enabled = true;
				sunShaftsEnabled = true;
				useHdr = true;
				ParticleEmissionMultiplier = 1.0f;
			}

			MasterVolume = Mathf.Clamp (AudioListener.volume, 0, 1);

			targetframerate = Application.targetFrameRate;
		}
	}

	// Save Settings Main.
	public void SaveSettingsData ()
	{
		if (AllowSaving == true) 
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

			Debug.Log (
				"Successfully saved to " +
				Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat"
			); 
		}
	}

	// Sets data.[variable] = [variable] from this script.
	void SetSettingsData (settingsData data)
	{
		data.QualitySettingsIndex = QualitySettingsIndex;

		if (data.QualitySettingsIndex == 0) 
		{
			data.useHdr = false;
			data.sunShaftsEnabled = false;
		}

		if (data.QualitySettingsIndex == 1) 
		{
			data.useHdr = true;
			data.sunShaftsEnabled = true;
		}

		data.ParticleEmissionMultiplier = ParticleEmissionMultiplier;

		data.MasterVolume 	  = Mathf.Clamp (MasterVolume, 	   0, 1);
		data.SoundtrackVolume = Mathf.Clamp (SoundtrackVolume, 0, 1);
		data.EffectsVolume 	  = Mathf.Clamp (EffectsVolume,    0, 1);

		data.targetframerate = targetframerate;
	}

	public void LoadSettingsData ()
	{
		if (AllowLoading == true)
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
				Debug.LogWarning ("Unable to load from " +
				Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat");

				SaveSettingsData ();

				Debug.Log ("Saved settings data to " +
				Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat");
			}
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
		}

		if (QualitySettingsIndex == 1) 
		{
			VisualSettingsComponent.enabled = true;
			useHdr = true;
			sunShaftsEnabled = true;
		}

		ParticleEmissionMultiplier = data.ParticleEmissionMultiplier;

		MasterVolume = data.MasterVolume;
		SoundtrackVolume = data.SoundtrackVolume;
		EffectsVolume = data.EffectsVolume;

		targetframerate = data.targetframerate;
	}

	// Puts new data into relevant scripts.
	void StoreSettingsDataInGame ()
	{
		QualitySettings.SetQualityLevel (QualitySettingsIndex);
		CheckAndApplyQualitySettings ();
		AudioListener.volume = Mathf.Clamp (MasterVolume, 0, 1);

		if (targetframerate < 30 && targetframerate >= 0) 
		{
			targetframerate = -1;
		}

		if (targetframerate >= 30 || targetframerate <= -1) 
		{
			targetFramerateScript.SetTargetFramerate (targetframerate);
		}
	}

	void CheckAndApplyQualitySettings ()
	{
		if (QualitySettingsIndex == 0) 
		{
			VisualSettingsComponent.enabled = false;
			useHdr = false;
			sunShaftsEnabled = false;
		}

		if (QualitySettingsIndex == 1) 
		{
			VisualSettingsComponent.enabled = true;
			useHdr = true;
			sunShaftsEnabled = true;
		}
			
		cam.allowHDR = useHdr;
		cam.GetComponent<SunShafts> ().enabled = sunShaftsEnabled;
	}

	// Variables stored in data files.
	[Serializable]
	public class playerData
	{
		public string Username;
		public int ExperiencePoints;
		public List<LeaderboardEntry> Leaderboard;
	}

	[Serializable]
	public class settingsData
	{
		public int QualitySettingsIndex;
		public bool useHdr;
		public bool sunShaftsEnabled;

		[Range (0, 2)]
		public float ParticleEmissionMultiplier = 1;

		public int targetframerate;

		public float MasterVolume;
		public float SoundtrackVolume;
		public float EffectsVolume;
	}
}