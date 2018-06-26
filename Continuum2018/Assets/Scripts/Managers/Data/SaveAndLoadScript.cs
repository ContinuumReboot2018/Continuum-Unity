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
	public LeaderboardAsset[] Leaderboards;

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
			}
		}

		if (SceneManager.GetActiveScene ().name == "SinglePlayer") 
		{
			fpsCounterScript = GameObject.Find ("FPSCounter").GetComponent<FPSCounter> ();
		}
	}

	void FixedUpdate ()
	{
		#if !UNITY_EDITOR
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
		data.SelectedAbility = SelectedAbility;
		data.SelectedSkin = SelectedSkin;
		data.MissionId = MissionId;
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
		SelectedAbility = data.SelectedAbility;
		SelectedSkin = data.SelectedSkin;
		MissionId = data.MissionId;
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