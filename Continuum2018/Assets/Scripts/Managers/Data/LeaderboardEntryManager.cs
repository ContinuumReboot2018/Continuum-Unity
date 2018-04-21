using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;

using TMPro;

public class LeaderboardEntryManager : MonoBehaviour 
{
	public LeaderboardDisplay leaderboardDisplayScript;
	public GameOverController gameOverControllerScript;
	public GameObject LeaderboardDisplay;

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
		menuActions = new PlayerActions ();
		eventData = new PointerEventData (EventSystem.current);
		maxCharacters = allCharacters.Length;
		currentCharacter = allCharacters [characterId].ToString ();
		NewNameText.text = NewName + allCharacters [characterId].ToString ();
	
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

		HighScoreText.text = "" + gameOverControllerScript.FinalScore;
		WaveText.text = "WAVE: " + gameOverControllerScript.gameControllerScript.Wave;
	}

	void Update ()
	{
		CheckForEntryInput ();
	}

	void CheckForEntryInput ()
	{
		if (menuActions.Shoot.WasPressed) 
		{
			ExecuteEvents.Execute (
				NextButton.gameObject, 
				eventData, 
				ExecuteEvents.pointerClickHandler
			);
		}

		if (menuActions.Back.WasPressed) 
		{
			ExecuteEvents.Execute (
				PreviousButton.gameObject, 
				eventData, 
				ExecuteEvents.pointerClickHandler
			);
		}

		if (menuActions.MoveUp.Value > 0) 
		{
			if (Time.unscaledTime > nextScroll) 
			{
				ExecuteEvents.Execute (
					UpButton.gameObject, 
					eventData, 
					ExecuteEvents.pointerClickHandler
				);

				nextScroll = Time.unscaledTime + ScrollCooldown;
			}
		}

		if (menuActions.MoveDown.Value > 0) 
		{
			if (Time.unscaledTime > nextScroll) 
			{
				ExecuteEvents.Execute (
					DownButton.gameObject, 
					eventData, 
					ExecuteEvents.pointerClickHandler
				);

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
		gameOverControllerScript.saveAndLoadScript.SavePlayerData ();

		yield return new WaitForSecondsRealtime (2);

		gameOverControllerScript.saveAndLoadScript.Leaderboard [gameOverControllerScript.place].name = NewName;
		gameOverControllerScript.NewLeaderboardEntry (gameOverControllerScript.place, NewName);

		LeaderboardDisplay.SetActive (true);

		SavingText.SetActive (false);
		gameOverControllerScript.LeaderboardEntryUI.SetActive (false);
	}
}