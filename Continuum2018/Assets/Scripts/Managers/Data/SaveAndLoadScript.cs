using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.PostProcessing;

public class SaveAndLoadScript : MonoBehaviour 
{
	// Live variables.
	[Header ("Player Data")]
	public string Username = "default";
	public int ExperiencePoints;

	[Header ("Settings Data")]
	public PostProcessingProfile VisualSettings;

	public playerData PlayerData;
	public settingsData SettingsData;

	void Start () 
	{
		//SavePlayerData ();
		LoadPlayerData ();
		//SaveSettingsData ();
		LoadSettingsData ();
		CheckUsername ();
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
		SetSettingsData ();

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
	void SetSettingsData ()
	{
		
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

	}

	// Puts new data into relevant scripts.
	void StoreSettingsDataInGame ()
	{

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
		
	}
}
