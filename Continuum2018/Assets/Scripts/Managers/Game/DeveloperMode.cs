using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeveloperMode : MonoBehaviour 
{
	public PlayerController playerControllerScript_P1;
	public GameController gameControllerScript;
	public TimescaleController timeScaleControllerScript;
	public LocalSceneLoader localSceneLoaderScript;
	public AudioController audioControllerScript;
	public SaveAndLoadScript saveAndLoadScript;

	[Header ("Cheats")]
	public string CheatString;
	public float CheatStringResetTimeRemaining = 1.0f;
	public float CheatStringResetDuration = 1.0f;
	public int maxCheatCharacters = 12;

	public bool useCheats;
	public bool allowCheats = false;
	public bool showCheats;

	public GameObject CheatsMenu;
	public Animator CheatsMenuAnim;
	public Animator CheatNotificationAnim;
	public TextMeshProUGUI CheatNotifcationText;
	public AudioSource CheatSound;

	[Header ("Debug Menu")]
	public bool showDebugMenu;
	public GameObject DebugMenu;

	void Start () 
	{
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();
	}

	void Update () 
	{
		if (useCheats == true) 
		{
			UpdateDebugMenu ();
		}

		if (allowCheats == true)
		{
			UpdateCheats ();
		}
	}

	void UpdateDebugMenu ()
	{
		if (playerControllerScript_P1.playerActions.DebugMenu.WasPressed) 
		{
			showDebugMenu = !showDebugMenu;

			if (showDebugMenu == true)
			{
				DebugMenu.SetActive (true);
			}

			if (showDebugMenu == false) 
			{
				DebugMenu.SetActive (false);
			}
		}
	}

	public void ClearCheatString ()
	{
		CheatString = "";
	}

	void UpdateCheats ()
	{
		if (CheatStringResetTimeRemaining > 0) 
		{
			CheatStringResetTimeRemaining -= Time.unscaledDeltaTime;
		}

		if (CheatStringResetTimeRemaining <= 0) 
		{
			ClearCheatString ();
			CheatStringResetTimeRemaining = CheatStringResetDuration;
		}

		foreach (char c in Input.inputString) 
		{		
			CheatStringResetTimeRemaining = CheatStringResetDuration;
			CheatString += c;
		}

		if (CheatString.Length > maxCheatCharacters) 
		{
			ClearCheatString ();
		}

		if (Input.GetKeyDown (KeyCode.Backspace)) 
		{
			ClearCheatString ();
		}

		if (CheatString == "continuum" && allowCheats == true) 
		{
			useCheats = true;
			Debug.Log ("Enabled cheats.");
			ShowCheatActivation ("CHEATS ON");
		}

		if (useCheats == true)
		{
			// Insert cheats here.
			if (CheatString == "start") 
			{
				timeScaleControllerScript.SwitchInitialSequence ();
				playerControllerScript_P1.StartCoroutines ();
				ShowCheatNotification ("CHEAT ACTIVATED");
			}

			if (CheatString == "restart") 
			{
				localSceneLoaderScript.sceneLoaderScript.SceneName = SceneManager.GetActiveScene().name;
				localSceneLoaderScript.sceneLoaderScript.StartLoadSequence ();
				ShowCheatNotification ("CHEAT ACTIVATED");
			}

			if (CheatString == "nexttrack") 
			{
				audioControllerScript.NextTrack ();
				ShowCheatNotification ("CHEAT ACTIVATED");
			}

			if (CheatString == "previoustrack") 
			{
				audioControllerScript.PreviousTrack ();
				ShowCheatNotification ("CHEAT ACTIVATED");
			}

			if (CheatString == "randomtrack") 
			{
				audioControllerScript.RandomTrack ();
				ShowCheatNotification ("CHEAT ACTIVATED");
			}

			if (CheatString == "savesettings") 
			{
				saveAndLoadScript.SaveSettingsData ();
				ShowCheatNotification ("CHEAT ACTIVATED");
			}

			if (CheatString == "loadsettings") 
			{
				saveAndLoadScript.LoadSettingsData ();
				ShowCheatNotification ("CHEAT ACTIVATED");
			}
		}
	}

	void ShowCheatActivation (string cheatText)
	{
		CheatNotificationAnim.Play ("CheatNotification");
		CheatNotifcationText.text = cheatText;
		Instantiate (CheatSound);
		ClearCheatString ();
	}

	void ShowCheatNotification (string cheatText)
	{
		CheatNotificationAnim.Play ("CheatNotification");
		CheatNotifcationText.text = cheatText;
		Instantiate (CheatSound);
		ClearCheatString ();
	}
}
