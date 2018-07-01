using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;

using TMPro;
using InControl;

public class LeaderboardEntryManager : MonoBehaviour 
{
	public static LeaderboardEntryManager Instance { get; private set; }

	public GameObject GameOverUI;
	public GameObject LeaderboardDisplayObject;
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

	void Awake ()
	{
		Instance = this;
		// DontDestroyOnLoad (gameObject);
	}

	void Start ()
	{
		SavingText.SetActive (false);
		initialsId = 0;

		maxCharacters = allCharacters.Length;
		currentCharacter = allCharacters [characterId].ToString ();
		NewNameText.text = NewName + allCharacters [characterId].ToString ();
		CheckPlacement ();
		CheckFinalScore ();
		WaveText.text = "WAVE: " + GameController.Instance.Wave;

		CreateMenuActions ();
		eventData = new PointerEventData (EventSystem.current);
	}

	void CheckFinalScore ()
	{
		HighScoreText.text = GameOverController.Instance.FinalScore.ToString ("N0");
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
		GameOverController.Instance.NewLeaderboardEntry (GameOverController.Instance.place, NewName); 

		int missionId = SaveAndLoadScript.Instance.MissionId;

		switch (missionId) 
		{
		case 0:
			SaveAndLoadScript.Instance.Leaderboard_Arcade [GameOverController.Instance.place].name = NewName;
			SaveAndLoadScript.Instance.Leaderboard_Arcade [GameOverController.Instance.place].score = Mathf.RoundToInt (GameOverController.Instance.FinalScore);
			SaveAndLoadScript.Instance.Leaderboard_Arcade [GameOverController.Instance.place].wave = GameController.Instance.Wave;
			break;
		case 1:
			// mods.
			break;
		case 2:
			SaveAndLoadScript.Instance.Leaderboard_BossRush [GameOverController.Instance.place].name = NewName;
			SaveAndLoadScript.Instance.Leaderboard_BossRush [GameOverController.Instance.place].score = Mathf.RoundToInt (GameOverController.Instance.FinalScore);
			SaveAndLoadScript.Instance.Leaderboard_BossRush [GameOverController.Instance.place].wave = GameController.Instance.Wave;
			break;
		case 3:
			SaveAndLoadScript.Instance.Leaderboard_Lucky [GameOverController.Instance.place].name = NewName;
			SaveAndLoadScript.Instance.Leaderboard_Lucky [GameOverController.Instance.place].score = Mathf.RoundToInt (GameOverController.Instance.FinalScore);
			SaveAndLoadScript.Instance.Leaderboard_Lucky [GameOverController.Instance.place].wave = GameController.Instance.Wave;
			break;
		case 4:
			SaveAndLoadScript.Instance.Leaderboard_FullyLoaded [GameOverController.Instance.place].name = NewName;
			SaveAndLoadScript.Instance.Leaderboard_FullyLoaded [GameOverController.Instance.place].score = Mathf.RoundToInt (GameOverController.Instance.FinalScore);
			SaveAndLoadScript.Instance.Leaderboard_FullyLoaded [GameOverController.Instance.place].wave = GameController.Instance.Wave;
			break;
		case 5:
			SaveAndLoadScript.Instance.Leaderboard_Scavenger [GameOverController.Instance.place].name = NewName;
			SaveAndLoadScript.Instance.Leaderboard_Scavenger [GameOverController.Instance.place].score = Mathf.RoundToInt (GameOverController.Instance.FinalScore);
			SaveAndLoadScript.Instance.Leaderboard_Scavenger [GameOverController.Instance.place].wave = GameController.Instance.Wave;
			break;
		case 6:
			SaveAndLoadScript.Instance.Leaderboard_Hell [GameOverController.Instance.place].name = NewName;
			SaveAndLoadScript.Instance.Leaderboard_Hell [GameOverController.Instance.place].score = Mathf.RoundToInt (GameOverController.Instance.FinalScore);
			SaveAndLoadScript.Instance.Leaderboard_Hell [GameOverController.Instance.place].wave = GameController.Instance.Wave;
			break;
		case 7:
			SaveAndLoadScript.Instance.Leaderboard_FastTrack [GameOverController.Instance.place].name = NewName;
			SaveAndLoadScript.Instance.Leaderboard_FastTrack [GameOverController.Instance.place].score = Mathf.RoundToInt (GameOverController.Instance.FinalScore);
			SaveAndLoadScript.Instance.Leaderboard_FastTrack [GameOverController.Instance.place].wave = GameController.Instance.Wave;
			break;
		}
			
		LeaderboardDisplay.Instance.UpdateLeaderboard ();
		SaveAndLoadScript.Instance.SavePlayerData ();

		GameOverUI.SetActive (true);

		LeaderboardDisplayObject.SetActive (true);
		LeaderboardDisplay.Instance.enabled = true;

		SavingText.SetActive (false);
		GameOverController.Instance.LeaderboardEntryUI.SetActive (false);
	}

	void CheckPlacement ()
	{
		switch (GameOverController.Instance.place)
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