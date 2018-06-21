using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;

using TMPro;
using InControl;

public class LeaderboardEntryManager : MonoBehaviour 
{
	public LeaderboardDisplay leaderboardDisplayScript;
	public GameOverController gameOverControllerScript;
	public GameObject GameOverUI;
	public GameObject LeaderboardDisplay;
	public AudioSource SelectSound;
	public AudioSource ScrollSound;

	[Header ("Leaderboard Entry")]
	public float ScrollCooldown = 0.5f;
	private float nextScroll;
	[Space (10)]
	public int initialsId;
	public string NewName;
	public TextMeshProUGUI NewNameText;
	[Space (10)]
	public int characterId;
	public string currentCharacter;
	[Space (10)]
	public string allCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ _.01234567890";
	public int maxCharacters;
	[Space (10)]
	public Button NextButton;
	public Button PreviousButton;
	public Button UpButton;
	public Button DownButton;

	[Header ("Display Stats")]
	public TextMeshProUGUI HighScoreText;
	public TextMeshProUGUI WaveText;
	public TextMeshProUGUI PlacementText;
	public GameObject SavingText;

	public PlayerActions menuActions;
	public PointerEventData eventData;

	void Start ()
	{
		SavingText.SetActive (false);
		initialsId = 0;

		maxCharacters = allCharacters.Length;
		currentCharacter = allCharacters [characterId].ToString ();
		NewNameText.text = NewName + allCharacters [characterId].ToString ();
		CheckPlacement ();
		CheckFinalScore ();
		WaveText.text = "WAVE: " + gameOverControllerScript.gameControllerScript.Wave;

		CreateMenuActions ();
		eventData = new PointerEventData (EventSystem.current);
	}

	void CheckFinalScore ()
	{
		HighScoreText.text = gameOverControllerScript.FinalScore.ToString ("N0");
	}

	void Update ()
	{
		CheckForEntryInput ();
	}

	void CheckForEntryInput ()
	{
		if (menuActions.Shoot.WasPressed) 
		{
			NextButtonOnClick ();
			SelectSound.Play ();
		}

		if (menuActions.Back.WasPressed) 
		{
			PreviousButtonOnClick ();
			SelectSound.Play ();
		}

		if (menuActions.MoveUp.WasPressed) 
		{
			if (Time.unscaledTime > nextScroll) 
			{
				UpButtonClick ();
				ScrollSound.Play ();
				nextScroll = Time.unscaledTime + ScrollCooldown;
			}
		}

		if (menuActions.MoveDown.WasPressed) 
		{
			if (Time.unscaledTime > nextScroll) 
			{
				DownButtonClick ();
				ScrollSound.Play ();
				nextScroll = Time.unscaledTime + ScrollCooldown;
			}
		}
	}

	public void UpButtonClick ()
	{
		if (characterId >= 0) 
		{
			characterId--;
		}

		if (characterId < 0) 
		{
			characterId = maxCharacters - 1;
		}

		currentCharacter = allCharacters [characterId].ToString ();
		NewNameText.text = NewName + currentCharacter.ToString ();
	}

	public void DownButtonClick ()
	{
		if (characterId < maxCharacters) 
		{
			characterId++;
		}

		if (characterId >= maxCharacters) 
		{
			characterId = 0;
		}

		currentCharacter = allCharacters [characterId].ToString ();
		NewNameText.text = NewName + currentCharacter.ToString ();
	}

	public void NextButtonOnClick ()
	{
		if (initialsId <= 2)
		{
			NewName += currentCharacter;
			initialsId++;
			NewNameText.text = NewName + allCharacters [0].ToString ();
		}

		if (initialsId > 2) 
		{
			StartCoroutine (CheckLeaderboard ());
			NewNameText.text = NewName;
		}
			
		characterId = 0;
		currentCharacter = allCharacters [characterId].ToString ();
	}

	public void PreviousButtonOnClick ()
	{
		initialsId = 0;
		characterId = 0;
		currentCharacter = allCharacters [0].ToString ();
		NewName = "";
		NewNameText.text = currentCharacter;
	}

	IEnumerator CheckLeaderboard ()
	{
		SavingText.SetActive (true);

		yield return new WaitForSecondsRealtime (2);

		// This allows the scores and waves to be updated correctly.
		gameOverControllerScript.NewLeaderboardEntry (gameOverControllerScript.place, NewName); 

		/*
		switch (gameOverControllerScript.saveAndLoadScript.MissionId) 
		{
		case 0:
			gameOverControllerScript.saveAndLoadScript.Leaderboard_ArcadeMode [gameOverControllerScript.place].name = NewName;
			gameOverControllerScript.saveAndLoadScript.Leaderboard_ArcadeMode [gameOverControllerScript.place].score = Mathf.RoundToInt (gameOverControllerScript.FinalScore);
			gameOverControllerScript.saveAndLoadScript.Leaderboard_ArcadeMode [gameOverControllerScript.place].wave = gameOverControllerScript.gameControllerScript.Wave;
			break;
		case 1:
			break;
		case 2:
			gameOverControllerScript.saveAndLoadScript.Leaderboard_BossRushMode [gameOverControllerScript.place].name = NewName;
			gameOverControllerScript.saveAndLoadScript.Leaderboard_BossRushMode [gameOverControllerScript.place].score = Mathf.RoundToInt (gameOverControllerScript.FinalScore);
			gameOverControllerScript.saveAndLoadScript.Leaderboard_BossRushMode [gameOverControllerScript.place].wave = gameOverControllerScript.gameControllerScript.Wave;
			break;
		case 3:
			gameOverControllerScript.saveAndLoadScript.Leaderboard_LuckyMode [gameOverControllerScript.place].name = NewName;
			gameOverControllerScript.saveAndLoadScript.Leaderboard_LuckyMode [gameOverControllerScript.place].score = Mathf.RoundToInt (gameOverControllerScript.FinalScore);
			gameOverControllerScript.saveAndLoadScript.Leaderboard_LuckyMode [gameOverControllerScript.place].wave = gameOverControllerScript.gameControllerScript.Wave;
			break;
		case 4:
			gameOverControllerScript.saveAndLoadScript.Leaderboard_FullyLoadedMode [gameOverControllerScript.place].name = NewName;
			gameOverControllerScript.saveAndLoadScript.Leaderboard_FullyLoadedMode [gameOverControllerScript.place].score = Mathf.RoundToInt (gameOverControllerScript.FinalScore);
			gameOverControllerScript.saveAndLoadScript.Leaderboard_FullyLoadedMode [gameOverControllerScript.place].wave = gameOverControllerScript.gameControllerScript.Wave;
			break;
		case 5:
			gameOverControllerScript.saveAndLoadScript.Leaderboard_ScavengerMode [gameOverControllerScript.place].name = NewName;
			gameOverControllerScript.saveAndLoadScript.Leaderboard_ScavengerMode [gameOverControllerScript.place].score = Mathf.RoundToInt (gameOverControllerScript.FinalScore);
			gameOverControllerScript.saveAndLoadScript.Leaderboard_ScavengerMode [gameOverControllerScript.place].wave = gameOverControllerScript.gameControllerScript.Wave;
			break;
		case 6:
			gameOverControllerScript.saveAndLoadScript.Leaderboard_HellMode [gameOverControllerScript.place].name = NewName;
			gameOverControllerScript.saveAndLoadScript.Leaderboard_HellMode [gameOverControllerScript.place].score = Mathf.RoundToInt (gameOverControllerScript.FinalScore);
			gameOverControllerScript.saveAndLoadScript.Leaderboard_HellMode [gameOverControllerScript.place].wave = gameOverControllerScript.gameControllerScript.Wave;
			break;
		case 7:
			gameOverControllerScript.saveAndLoadScript.Leaderboard_FastTrackMode [gameOverControllerScript.place].name = NewName;
			gameOverControllerScript.saveAndLoadScript.Leaderboard_FastTrackMode [gameOverControllerScript.place].score = Mathf.RoundToInt (gameOverControllerScript.FinalScore);
			gameOverControllerScript.saveAndLoadScript.Leaderboard_FastTrackMode [gameOverControllerScript.place].wave = gameOverControllerScript.gameControllerScript.Wave;
			break;
		}*/

		leaderboardDisplayScript.UpdateLeaderboard ();
		gameOverControllerScript.saveAndLoadScript.SavePlayerData ();

		GameOverUI.SetActive (true);

		LeaderboardDisplay.SetActive (true);
		leaderboardDisplayScript.enabled = true;

		SavingText.SetActive (false);
		gameOverControllerScript.LeaderboardEntryUI.SetActive (false);
	}

	void CheckPlacement ()
	{
		switch (gameOverControllerScript.place)
		{
		case 0:
			PlacementText.text = "1st";
			break;
		case 1:
			PlacementText.text = "2nd";
			break;
		case 2:
			PlacementText.text = "3rd";
			break;
		case 3:
			PlacementText.text = "4th";
			break;
		case 4:
			PlacementText.text = "5th";
			break;
		case 5:
			PlacementText.text = "6th";
			break;
		case 6:
			PlacementText.text = "7th";
			break;
		case 7:
			PlacementText.text = "8th";
			break;
		case 8:
			PlacementText.text = "9th";
			break;
		case 9:
			PlacementText.text = "10th";
			break;
		}
	}

	void CreateMenuActions ()
	{
		menuActions = new PlayerActions ();

		menuActions.Shoot.AddDefaultBinding (Key.Return);
		menuActions.Shoot.AddDefaultBinding (InputControlType.Action1);

		menuActions.MoveUp.AddDefaultBinding (Key.UpArrow);
		menuActions.MoveUp.AddDefaultBinding (InputControlType.LeftStickUp);

		menuActions.MoveDown.AddDefaultBinding (Key.DownArrow);
		menuActions.MoveDown.AddDefaultBinding (InputControlType.LeftStickDown);

		menuActions.Back.AddDefaultBinding (Key.Escape);
		menuActions.Back.AddDefaultBinding (InputControlType.Action2);
	}
}