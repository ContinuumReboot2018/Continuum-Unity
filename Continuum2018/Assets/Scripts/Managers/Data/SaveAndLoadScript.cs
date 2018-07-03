using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.PostProcessing;

using UnityStandardAssets.ImageEffects;

public class SaveAndLoadScript : MonoBehaviour 
{
	public static SaveAndLoadScript Instance { get; private set; }

	public PlayerController playerControllerScript_P1;
	[Space (10)]
	public bool AllowLoading = true;
	public bool AllowSaving = true;

	// Live variables.
	[Header ("Player Data")]
	public string Username = "default";
	public int ExperiencePoints;
	public int blocksDestroyed;

	[Space (10)]
	public int SelectedAbility;
	public int SelectedSkin;
	public int MissionId;

	[Space (10)]
	public List<LeaderboardEntry> DefaultLeaderboard_Arcade;
	public List<LeaderboardEntry> DefaultLeaderboard_BossRush;
	public List<LeaderboardEntry> DefaultLeaderboard_Lucky;
	public List<LeaderboardEntry> DefaultLeaderboard_FullyLoaded;
	public List<LeaderboardEntry> DefaultLeaderboard_Scavenger;
	public List<LeaderboardEntry> DefaultLeaderboard_Hell;
	public List<LeaderboardEntry> DefaultLeaderboard_FastTrack;

	[Header ("Leaderboards")]
	public List<LeaderboardEntry> Leaderboard_Arcade;
	public List<LeaderboardEntry> Leaderboard_BossRush;
	public List<LeaderboardEntry> Leaderboard_Lucky;
	public List<LeaderboardEntry> Leaderboard_FullyLoaded;
	public List<LeaderboardEntry> Leaderboard_Scavenger;
	public List<LeaderboardEntry> Leaderboard_Hell;
	public List<LeaderboardEntry> Leaderboard_FastTrack;

	[Header ("Settings Data")]
	public PostProcessingProfile VisualSettings;
	public PostProcessingBehaviour VisualSettingsComponent;
	public Camera cam;
	public FastMobileBloom fastMobileBloomScript;
	[Space (10)]
	public int QualitySettingsIndex;
	public bool useHdr;
	public bool sunShaftsEnabled;
	[Space (10)]
	public int targetframerate;
	public float averageFpsTimer;
	[Space (10)]
	public float ParticleEmissionMultiplier = 1;
	[Space (10)]
	public float MasterVolume;
	public float SoundtrackVolume;
	public float EffectsVolume;

	public playerData PlayerData;
	public settingsData SettingsData;

	void Awake ()
	{
		Instance = this;
		// DontDestroyOnLoad (gameObject);
	}

	void Start ()
	{
		if (SceneManager.GetActiveScene ().name != "Init")
		{
			if (AllowLoading == true) 
			{
				cam = SettingsManager.Instance.cam;
				VisualSettingsComponent = cam.GetComponent<PostProcessingBehaviour> ();
				fastMobileBloomScript = cam.GetComponent<FastMobileBloom> ();

				CheckAndApplyQualitySettings ();
			}
		}
	}

	void FixedUpdate ()
	{
		#if !UNITY_EDITOR
		// This allows the framerate to hitch without causing a quality settings change.
		if (FPSCounter.Instance != null && SceneLoader.Instance.isLoading == false) 
		{
			if (FPSCounter.Instance.averageFps < 30)
			{
				averageFpsTimer += Time.fixedDeltaTime;

				if (averageFpsTimer > 10) 
				{
					if (QualitySettingsIndex != 0)
					{
						QualitySettingsIndex = 0;
						Application.targetFrameRate = -1;

						if (Screen.width > 1280 || Screen.height > 720) 
						{
							Screen.SetResolution (1280, 720, Screen.fullScreen);
						}

						SaveSettingsData ();
						LoadSettingsData ();
						Debug.Log ("Average FPS too low, falling back to lower quality.");
						averageFpsTimer = 0;
						return;
					}
				}
			} 

			else 
			
			{
				if (averageFpsTimer != 0) 
				{
					averageFpsTimer = 0;
				}
			}
		}
		#endif
	}
		
	void CheckUsername ()
	{
		if (Username == null || Username == "") 
		{
			Username = "default";
		}

		if (Username == "default") 
		{
			Debug.Log (
				"Username is " 
				+ Username + 
				". Consider changing your username in the menu. " +
				"You may not have created a local profile yet."
			);
		}
	}
		
	// Gets variables from this script = variables in other scripts.
	void GetPlayerData ()
	{
		#if !UNITY_EDITOR
		if (GameController.Instance != null) 
		{
			if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat") == true) 
			{
				ExperiencePoints += (int)Math.Round (GameController.Instance.TargetScore);
				blocksDestroyed += GameController.Instance.BlocksDestroyed;
			}
		}

		if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat") == false) 
		{
			ExperiencePoints = 0;
			blocksDestroyed = 0;
			ResetAllLeaderboards ();
		}
		#endif

		#if UNITY_EDITOR
		if (GameController.Instance != null) 
		{
			if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat") == true) 
			{
				ExperiencePoints += (int)Math.Round (GameController.Instance.TargetScore);
				blocksDestroyed += GameController.Instance.BlocksDestroyed;
			}
		}

		if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat") == false) 
		{
			ExperiencePoints = 0;
			blocksDestroyed = 0;
			ResetAllLeaderboards ();
		}
		#endif
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

			#if !UNITY_EDITOR
				FileStream file = File.Create (Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat");

				Debug.Log (
					"Successfully saved to " +
					Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat"
				); 
			#endif

			#if UNITY_EDITOR
				FileStream file = File.Create (Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat");

				Debug.Log (
					"Successfully saved to " +
					Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat"
				); 
			#endif

			// Does the saving
			playerData data = new playerData ();
			SetPlayerData (data);

			// Serializes and closes the file.
			bf.Serialize (file, data);
			file.Close ();
		}
	}

	// Sets data.[variable] = [variable] from this script.
	void SetPlayerData (playerData data)
	{
		data.Username = Username;
		data.ExperiencePoints = ExperiencePoints;
		data.blocksDestroyed = blocksDestroyed;

		data.SelectedAbility = SelectedAbility;
		data.SelectedSkin = SelectedSkin;
		data.MissionId = MissionId;

		data.Leaderboard_Arcade = Leaderboard_Arcade;
		data.Leaderboard_BossRush = Leaderboard_BossRush;
		data.Leaderboard_Lucky = Leaderboard_Lucky;
		data.Leaderboard_FullyLoaded = Leaderboard_FullyLoaded;
		data.Leaderboard_Scavenger = Leaderboard_Scavenger;
		data.Leaderboard_Hell = Leaderboard_Hell;
		data.Leaderboard_FastTrack = DefaultLeaderboard_FastTrack;

		PlayerData.Leaderboard_Arcade = Leaderboard_Arcade;
		PlayerData.Leaderboard_BossRush = Leaderboard_BossRush;
		PlayerData.Leaderboard_Lucky = Leaderboard_Lucky;
		PlayerData.Leaderboard_FullyLoaded = Leaderboard_FullyLoaded;
		PlayerData.Leaderboard_Scavenger = Leaderboard_Scavenger;
		PlayerData.Leaderboard_Hell = Leaderboard_Hell;
		PlayerData.Leaderboard_FastTrack = Leaderboard_FastTrack;
	}

	public void DeletePlayerDataEditor ()
	{
		if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat") == true)
		{
			File.Delete (Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat");
			Debug.Log ("Successfully deleted file " +
			Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat");
		}
	}

	public void DeletePlayerDataMain ()
	{
		if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat") == true) 
		{
			File.Delete (Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat");
			Debug.Log ("Successfully deleted file " +
			Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat");
		}
	}

	public void DeleteSettingsDataEditor ()
	{
		if (File.Exists (Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat") == true) 
		{
			File.Delete (Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat");
			Debug.Log ("Successfully deleted file " +
			Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat");
		}
	}

	public void DeleteSettingsDataMain ()
	{
		if (File.Exists (Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat") == true)
		{
			File.Delete (Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat");
			Debug.Log ("Successfully deleted file " +
			Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat");
		}
	}

	public void ResetAllLeaderboards ()
	{
		Leaderboard_Arcade = DefaultLeaderboard_Arcade;
		Leaderboard_BossRush = DefaultLeaderboard_BossRush;
		Leaderboard_Lucky = DefaultLeaderboard_Lucky;
		Leaderboard_FullyLoaded = DefaultLeaderboard_FullyLoaded;
		Leaderboard_Scavenger = DefaultLeaderboard_Scavenger;
		Leaderboard_Hell = DefaultLeaderboard_Hell;
		Leaderboard_FastTrack = DefaultLeaderboard_FastTrack;

		PlayerData.Leaderboard_Arcade = Leaderboard_Arcade;
		PlayerData.Leaderboard_BossRush = Leaderboard_BossRush;
		PlayerData.Leaderboard_Lucky = Leaderboard_Lucky;
		PlayerData.Leaderboard_FullyLoaded = Leaderboard_FullyLoaded;
		PlayerData.Leaderboard_Scavenger = Leaderboard_Scavenger;
		PlayerData.Leaderboard_Hell = Leaderboard_Hell;
		PlayerData.Leaderboard_FastTrack = Leaderboard_FastTrack;

		Debug.Log ("Leaderboards have been reset.");
	}
		
	// Load PlayerData main.
	public void LoadPlayerData ()
	{
		#if !UNITY_EDITOR
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

			else
			
			{
				SavePlayerData ();
			}
		}
		#endif

		#if UNITY_EDITOR
		if (AllowLoading == true)
		{
			if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat") == true) 
			{
				// Opens the save data.
				BinaryFormatter bf = new BinaryFormatter ();
				FileStream file = File.Open (Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat", FileMode.Open);

				// Processes the save data into memory.
				playerData data = (playerData)bf.Deserialize (file);
				file.Close ();

				LoadPlayerDataContents (data);
				StorePlayerDataInGame ();

				Debug.Log ("Successfully loaded from " +
				Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat");
			}

			else
			
			{
				SavePlayerData ();
			}
		}
		#endif
	}

	// Sets variables in this script by getting data from save file. 
	void LoadPlayerDataContents (playerData data)
	{
		Username = data.Username;
		ExperiencePoints = data.ExperiencePoints;
		blocksDestroyed = data.blocksDestroyed;

		SelectedAbility = data.SelectedAbility;
		SelectedSkin = data.SelectedSkin;
		MissionId = data.MissionId;

		Leaderboard_Arcade = data.Leaderboard_Arcade;
		Leaderboard_BossRush = data.Leaderboard_BossRush;
		Leaderboard_Lucky = data.Leaderboard_Lucky;
		Leaderboard_FullyLoaded = data.Leaderboard_FullyLoaded;
		Leaderboard_Scavenger = data.Leaderboard_Scavenger;
		Leaderboard_Hell = data.Leaderboard_Hell;
		Leaderboard_FastTrack = data.Leaderboard_FastTrack;

		PlayerData.Leaderboard_Arcade = Leaderboard_Arcade;
		PlayerData.Leaderboard_BossRush = Leaderboard_BossRush;
		PlayerData.Leaderboard_Lucky = Leaderboard_Lucky;
		PlayerData.Leaderboard_FullyLoaded = Leaderboard_FullyLoaded;
		PlayerData.Leaderboard_Scavenger = Leaderboard_Scavenger;
		PlayerData.Leaderboard_Hell = Leaderboard_Hell;
		PlayerData.Leaderboard_FastTrack = Leaderboard_FastTrack;
	}
		
	// Puts new data into relevant scripts.
	public void StorePlayerDataInGame ()
	{
		if (GameModifierReceiver.Instance != null) 
		{
			// Sets modifier manager scirpt to change
			GameModifierReceiver.Instance.loadedModifierManagerScript = GameModifierReceiver.Instance.gameModifiers [MissionId];
		}
	}
		
	// Gets variables from this script = variables in other scripts.
	void GetSettingsData ()
	{
		if (File.Exists (Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat") == true
		 || File.Exists (Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat") == true) 
		{
			QualitySettings.SetQualityLevel (QualitySettingsIndex);

			if (QualitySettingsIndex == 0) 
			{
				if (VisualSettingsComponent != null)
				{
					if (VisualSettingsComponent.enabled == true) 
					{
						VisualSettingsComponent.enabled = false;
						InitManager.Instance.postProcess.enabled = false;
						Debug.Log ("Turned off visual settings component.");
					}
				}

				if (fastMobileBloomScript != null) 
				{
					fastMobileBloomScript.enabled = true;
				}

				sunShaftsEnabled = false;
				useHdr = false;
				ParticleEmissionMultiplier = 0.25f;
			}

			if (QualitySettingsIndex == 1) 
			{
				if (VisualSettingsComponent != null)
				{
					if (VisualSettingsComponent.enabled == true)
					{
						VisualSettingsComponent.enabled = true;
						InitManager.Instance.postProcess.enabled = true;
						Debug.Log ("Turned on visual settings component.");
					}
				}

				if (fastMobileBloomScript != null) 
				{
					fastMobileBloomScript.enabled = false;
				}

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
			//StreamWriter bf = new StreamWriter ();

			#if !UNITY_EDITOR

				FileStream file = File.Create (Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat");
				
				Debug.Log (
					"Successfully saved to " +
					Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat"
				); 
			#endif

			#if UNITY_EDITOR
				FileStream file = File.Create (Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat");

				Debug.Log (
					"Successfully saved to " +
					Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat"
				); 
			#endif


			#if UNITY_STANDALONE_OSX
				cam.GetComponent<VolumetricLightRenderer> ().enabled = false;
			#endif

			#if PLATFORM_STANDALONE_OSX
				cam.GetComponent<VolumetricLightRenderer> ().enabled = false;
			#endif

			// Does the saving
			settingsData data = new settingsData ();
			SetSettingsData (data);

			// Serializes and closes the file.
			bf.Serialize (file, data);
			file.Close ();
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

			if (VisualSettingsComponent != null)
			{
				VisualSettingsComponent.enabled = false;
			}
		}

		if (data.QualitySettingsIndex == 1) 
		{
			data.useHdr = true;
			data.sunShaftsEnabled = true;

			if (VisualSettingsComponent != null)
			{
				VisualSettingsComponent.enabled = false;
			}
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
			#if !UNITY_EDITOR
			if (File.Exists (Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat") == true) 
			{
				// Opens the save data.
				BinaryFormatter bf = new BinaryFormatter ();

				FileStream file = File.Open (Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat", FileMode.Open);

				Debug.Log ("Successfully loaded from " +
				Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat");

				// Processes the save data into memory.
				settingsData data = (settingsData)bf.Deserialize (file);
				file.Close ();

				LoadSettingsDataContents (data);
				StoreSettingsDataInGame ();
			}
			#endif

			#if UNITY_EDITOR
			if (File.Exists (Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat") == true) 
			{
				// Opens the save data.
				BinaryFormatter bf = new BinaryFormatter ();

				FileStream file = File.Open (Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat", FileMode.Open);

				Debug.Log ("Successfully loaded from " +
				Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat");

				// Processes the save data into memory.
				settingsData data = (settingsData)bf.Deserialize (file);
				file.Close ();

				LoadSettingsDataContents (data);
				StoreSettingsDataInGame ();
			}
			#endif
		}
	}

	// Sets variables in this script by getting data from save file. 
	void LoadSettingsDataContents (settingsData data)
	{
		QualitySettingsIndex = data.QualitySettingsIndex;

		if (QualitySettingsIndex == 0) 
		{
			if (VisualSettingsComponent != null)
			{
				if (VisualSettingsComponent.enabled == true) 
				{
					VisualSettingsComponent.enabled = false;
					InitManager.Instance.postProcess.enabled = false;
					Debug.Log ("Turned off visual settings component.");
				}
			}

			useHdr = false;
			sunShaftsEnabled = false;
		}

		if (QualitySettingsIndex == 1) 
		{
			if (VisualSettingsComponent != null)
			{
				if (VisualSettingsComponent.enabled == false) 
				{
					VisualSettingsComponent.enabled = true;
					InitManager.Instance.postProcess.enabled = true;
					Debug.Log ("Turned on visual settings component.");
				}
			}

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
			TargetFPS.Instance.SetTargetFramerate (targetframerate);
		}
	}

	void CheckAndApplyQualitySettings ()
	{
		if (QualitySettingsIndex == 0) 
		{
			if (VisualSettingsComponent != null)
			{
				if (VisualSettingsComponent.enabled == true)
				{
					VisualSettingsComponent.enabled = false;
					InitManager.Instance.postProcess.enabled = false;
					//Debug.Log ("Turned off visual settings component.");
				}
			}

			useHdr = false;
			sunShaftsEnabled = false;
		}

		if (QualitySettingsIndex == 1) 
		{
			if (VisualSettingsComponent != null)
			{
				if (VisualSettingsComponent.enabled == false)
				{
					VisualSettingsComponent.enabled = true;
					InitManager.Instance.postProcess.enabled = true;
					//Debug.Log ("Turned on visual settings component.");
				}
			}

			useHdr = true;
			sunShaftsEnabled = true;
		}
			
		if (cam != null) 
		{
			// Apply camera specific settings here.
			cam.allowHDR = useHdr;
			cam.GetComponent<SunShafts> ().enabled = sunShaftsEnabled;
		}
	}

	// Variables stored in data files.
	[Serializable]
	public class playerData
	{
		public string Username;
		public int ExperiencePoints;
		public int blocksDestroyed;

		public int SelectedAbility;
		public int SelectedSkin;
		public int MissionId;

		public List<LeaderboardEntry> Leaderboard_Arcade;
		public List<LeaderboardEntry> Leaderboard_BossRush;
		public List<LeaderboardEntry> Leaderboard_Lucky;
		public List<LeaderboardEntry> Leaderboard_FullyLoaded;
		public List<LeaderboardEntry> Leaderboard_Scavenger;
		public List<LeaderboardEntry> Leaderboard_Hell;
		public List<LeaderboardEntry> Leaderboard_FastTrack;
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