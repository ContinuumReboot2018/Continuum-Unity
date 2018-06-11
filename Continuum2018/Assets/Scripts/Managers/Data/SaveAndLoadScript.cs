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
	public InitManager initManagerScript;
	public SettingsManager settingsManagerScript;
	public SceneLoader sceneLoaderScript;
	public PlayerController playerControllerScript_P1;
	public GameController gameControllerScript;

	public bool AllowLoading = true;
	public bool AllowSaving = true;

	public List<LeaderboardEntry> DefaultLeaderboard;

	// Live variables.
	[Header ("Player Data")]
	public string Username = "default";
	public int ExperiencePoints;

	[Header ("Leaderboards")]
	//public List<LeaderboardEntry> Leaderboard;
	public List<LeaderboardEntry> Leaderboard_ArcadeMode;
	public List<LeaderboardEntry> Leaderboard_BossRushMode;
	public List<LeaderboardEntry> Leaderboard_LuckyMode;
	public List<LeaderboardEntry> Leaderboard_FullyLoadedMode;
	public List<LeaderboardEntry> Leaderboard_ScavengerMode;
	public List<LeaderboardEntry> Leaderboard_HellMode;
	public List<LeaderboardEntry> Leaderboard_FastTrackMode;

	[Space (10)]
	public int SelectedAbility;
	public int SelectedSkin;
	public int MissionId;

	[Header ("Settings Data")]
	public PostProcessingProfile VisualSettings;
	public PostProcessingBehaviour VisualSettingsComponent;
	public Camera cam;
	public FastMobileBloom fastMobileBloomScript;

	[Space (10)]
	public int QualitySettingsIndex;
	public bool useHdr;
	public bool sunShaftsEnabled;

	public TargetFPS targetFramerateScript;
	public int targetframerate;

	public FPSCounter fpsCounterScript;
	public float averageFpsTimer;

	[Space (10)]
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
				settingsManagerScript = GameObject.Find ("SettingsManager").GetComponent<SettingsManager> ();
				targetFramerateScript = GameObject.Find ("TargetFPS").GetComponent<TargetFPS> ();

				cam = settingsManagerScript.cam;
				VisualSettingsComponent = cam.GetComponent<PostProcessingBehaviour> ();
				fastMobileBloomScript = cam.GetComponent<FastMobileBloom> ();

				CheckPlayerDataFile ();

				//LoadPlayerData ();
				//LoadSettingsData ();

				CheckUsername ();
			}
		}

		if (SceneManager.GetActiveScene ().name == "SinglePlayer") 
		{
			fpsCounterScript = GameObject.Find ("FPSCounter").GetComponent<FPSCounter> ();
		}
	}

	void FixedUpdate ()
	{
		// This allows the framerate to hitch without causing a quality settings change.
		if (fpsCounterScript != null && sceneLoaderScript.isLoading == false) 
		{
			if (fpsCounterScript.averageFps < 30)
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
			if (gameControllerScript != null) 
			{
				if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat") == true) 
				{
					ExperiencePoints += (int)Math.Round (gameControllerScript.TargetScore);
				}
			}

			if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat") == false) 
			{
				ExperiencePoints = 0;
			}
		#endif

		#if UNITY_EDITOR
			if (gameControllerScript != null) 
			{
				if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat") == true) 
				{
					ExperiencePoints += (int)Math.Round (gameControllerScript.TargetScore);
				}
			}

			if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat") == false) 
			{
				ExperiencePoints = 0;
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

		data.Leaderboard_ArcadeMode 		= Leaderboard_ArcadeMode;
		data.Leaderboard_BossRushMode 		= Leaderboard_BossRushMode;
		data.Leaderboard_LuckyMode 			= Leaderboard_LuckyMode;
		data.Leaderboard_FullyLoadedMode 	= Leaderboard_FullyLoadedMode;
		data.Leaderboard_ScavengerMode		= Leaderboard_ScavengerMode;
		data.Leaderboard_HellMode 			= Leaderboard_HellMode;
		data.Leaderboard_FastTrackMode 		= Leaderboard_FastTrackMode;

		data.SelectedAbility = SelectedAbility;
		data.SelectedSkin = SelectedSkin;
		data.MissionId = MissionId;
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

			CheckPlayerDataFile ();
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

			CheckPlayerDataFile ();
		}
		#endif
	}

	void CheckPlayerDataFile ()
	{
		#if !UNITY_EDITOR
		if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat") == false)
		{
			Debug.LogWarning ("Unable to load from " +
				Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat");

			//Leaderboard_ArcadeMode 		= new List<LeaderboardEntry> (10);
			//Leaderboard_BossRushMode 	= new List<LeaderboardEntry> (10);
			//Leaderboard_LuckyMode 		= new List<LeaderboardEntry> (10);
			//Leaderboard_FullyLoadedMode = new List<LeaderboardEntry> (10);
			//Leaderboard_ScavengerMode 	= new List<LeaderboardEntry> (10);
			//Leaderboard_HellMode 		= new List<LeaderboardEntry> (10);
			//Leaderboard_FastTrackMode 	= new List<LeaderboardEntry> (10);

			Leaderboard_ArcadeMode		= DefaultLeaderboard;
			Leaderboard_BossRushMode 	= DefaultLeaderboard;
			Leaderboard_LuckyMode 		= DefaultLeaderboard;
			Leaderboard_FullyLoadedMode = DefaultLeaderboard;
			Leaderboard_ScavengerMode 	= DefaultLeaderboard;
			Leaderboard_HellMode 		= DefaultLeaderboard;
			Leaderboard_FastTrackMode 	= DefaultLeaderboard;

			SavePlayerData ();

			Debug.Log ("Saved new player data to " +
				Application.persistentDataPath + "/" + Username + "_PlayerConfig.dat");
		}
		#endif

		#if UNITY_EDITOR
		if (File.Exists (Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat") == false)
		{
			Debug.LogWarning ("Unable to load from " +
			Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat");
			
			//Leaderboard_ArcadeMode 		= new List<LeaderboardEntry> (10);
			//Leaderboard_BossRushMode 	= new List<LeaderboardEntry> (10);
			//Leaderboard_LuckyMode 		= new List<LeaderboardEntry> (10);
			//Leaderboard_FullyLoadedMode = new List<LeaderboardEntry> (10);
			//Leaderboard_ScavengerMode	= new List<LeaderboardEntry> (10);
			//Leaderboard_HellMode 		= new List<LeaderboardEntry> (10);
			//Leaderboard_FastTrackMode 	= new List<LeaderboardEntry> (10);

			Leaderboard_ArcadeMode 		= DefaultLeaderboard;
			Leaderboard_BossRushMode 	= DefaultLeaderboard;
			Leaderboard_LuckyMode 		= DefaultLeaderboard;
			Leaderboard_FullyLoadedMode = DefaultLeaderboard;
			Leaderboard_ScavengerMode 	= DefaultLeaderboard;
			Leaderboard_HellMode 		= DefaultLeaderboard;
			Leaderboard_FastTrackMode 	= DefaultLeaderboard;

			SavePlayerData ();

			Debug.Log ("Saved new player data to " +
			Application.persistentDataPath + "/" + Username + "_PlayerConfig_Editor.dat");
		}
		#endif
	}

	// Sets variables in this script by getting data from save file. 
	void LoadPlayerDataContents (playerData data)
	{
		Username = data.Username;
		ExperiencePoints = data.ExperiencePoints;
		SelectedAbility = data.SelectedAbility;
		SelectedSkin = data.SelectedSkin;
		MissionId = data.MissionId;

		if (SceneManager.GetActiveScene ().name == "Menu")
		{
			Leaderboard_ArcadeMode 		= data.Leaderboard_ArcadeMode;
			Leaderboard_BossRushMode 	= data.Leaderboard_BossRushMode;
			Leaderboard_LuckyMode 		= data.Leaderboard_LuckyMode;
			Leaderboard_FullyLoadedMode = data.Leaderboard_FullyLoadedMode;
			Leaderboard_ScavengerMode	= data.Leaderboard_ScavengerMode;
			Leaderboard_HellMode 		= data.Leaderboard_HellMode;
			Leaderboard_FastTrackMode 	= data.Leaderboard_FastTrackMode;
			return;
		}
			
		if (SceneManager.GetActiveScene ().name != "Menu")
		{
			Leaderboard_ArcadeMode 		= data.Leaderboard_ArcadeMode;
			Leaderboard_BossRushMode 	= data.Leaderboard_BossRushMode;
			Leaderboard_LuckyMode 		= data.Leaderboard_LuckyMode;
			Leaderboard_FullyLoadedMode = data.Leaderboard_FullyLoadedMode;
			Leaderboard_ScavengerMode	= data.Leaderboard_ScavengerMode;
			Leaderboard_HellMode 		= data.Leaderboard_HellMode;
			Leaderboard_FastTrackMode 	= data.Leaderboard_FastTrackMode;
			return;
		}

		/*
		if (SceneManager.GetActiveScene ().name != "Menu") 
		{
			switch (MissionId)
			{
			case 0:
				data.Leaderboard_ArcadeMode.Capacity = 10;
				Leaderboard_ArcadeMode.Capacity = 10;
				Leaderboard_ArcadeMode = data.Leaderboard_ArcadeMode;
				break;
			case 1:
				// data.Leaderboard_ModsMode.Capacity = 10;
				// Leaderboard_ModsMode.Capacity = 10;
				// Leaderboard_ModsMode = data.Leaderboard_ArcadeMode;
				break;
			case 2:
				data.Leaderboard_BossRushMode.Capacity = 10;
				Leaderboard_BossRushMode.Capacity = 10;
				Leaderboard_BossRushMode = data.Leaderboard_BossRushMode;
				break;
			case 3:
				data.Leaderboard_LuckyMode.Capacity = 10;
				Leaderboard_LuckyMode.Capacity = 10;
				Leaderboard_LuckyMode = data.Leaderboard_LuckyMode;
				break;
			case 4:
				data.Leaderboard_FullyLoadedMode.Capacity = 10;
				Leaderboard_FullyLoadedMode.Capacity = 10;
				Leaderboard_FullyLoadedMode = data.Leaderboard_FullyLoadedMode;
				break;
			case 5:
				data.Leaderboard_ScavengerMode.Capacity = 10;
				Leaderboard_ScavengerMode.Capacity = 10;
				Leaderboard_ScavengerMode = data.Leaderboard_ScavengerMode;
				break;
			case 6:
				data.Leaderboard_HellMode.Capacity = 10;
				Leaderboard_HellMode.Capacity = 10;
				Leaderboard_HellMode = data.Leaderboard_HellMode;
				break;
			case 7:
				data.Leaderboard_FastTrackMode.Capacity = 10;
				Leaderboard_FastTrackMode.Capacity = 10;
				Leaderboard_FastTrackMode = data.Leaderboard_FastTrackMode;
				break;
			}
		}
		*/
	}

	// Puts new data into relevant scripts.
	public void StorePlayerDataInGame ()
	{

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
					VisualSettingsComponent.enabled = false;
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
					VisualSettingsComponent.enabled = true;
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

				if (File.Exists (Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat") == false) 
				{
					Debug.LogWarning ("Unable to load from " +
					Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat");

					SaveSettingsData ();

					Debug.Log ("Saved settings data to " +
					Application.persistentDataPath + "/" + Username + "_SettingsConfig.dat");
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

				if (File.Exists (Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat") == false) 
				{
					Debug.LogWarning ("Unable to load from " +
					Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat");

					SaveSettingsData ();

					Debug.Log ("Saved settings data to " +
					Application.persistentDataPath + "/" + Username + "_SettingsConfig_Editor.dat");
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
				VisualSettingsComponent.enabled = false;
			}

			useHdr = false;
			sunShaftsEnabled = false;
		}

		if (QualitySettingsIndex == 1) 
		{
			if (VisualSettingsComponent != null)
			{
				VisualSettingsComponent.enabled = true;
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
			targetFramerateScript.SetTargetFramerate (targetframerate);
		}
	}

	void CheckAndApplyQualitySettings ()
	{
		if (QualitySettingsIndex == 0) 
		{
			if (VisualSettingsComponent != null)
			{
				VisualSettingsComponent.enabled = false;
			}

			useHdr = false;
			sunShaftsEnabled = false;
		}

		if (QualitySettingsIndex == 1) 
		{
			if (VisualSettingsComponent != null)
			{
				VisualSettingsComponent.enabled = true;
			}

			useHdr = true;
			sunShaftsEnabled = true;
		}
			
		if (cam != null) 
		{
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

		//public List<LeaderboardEntry> Leaderboard;
		public List<LeaderboardEntry> Leaderboard_ArcadeMode;
		public List<LeaderboardEntry> Leaderboard_BossRushMode;
		public List<LeaderboardEntry> Leaderboard_LuckyMode;
		public List<LeaderboardEntry> Leaderboard_FullyLoadedMode;
		public List<LeaderboardEntry> Leaderboard_ScavengerMode;
		public List<LeaderboardEntry> Leaderboard_HellMode;
		public List<LeaderboardEntry> Leaderboard_FastTrackMode;

		public int SelectedAbility;
		public int SelectedSkin;
		public int MissionId;
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