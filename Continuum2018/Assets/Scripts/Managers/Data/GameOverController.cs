using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;
using System.Collections.Generic;

using TMPro;

public class GameOverController : MonoBehaviour 
{
	public LeaderboardEntry newLeaderboardEntry;
	private bool allowupdateentry;

	public SaveAndLoadScript saveAndLoadScript;
	public GameController gameControllerScript;
	public DeveloperMode developerModeScript;
	public LeaderboardDisplay leaderboardDisplay;

	[Header ("Stats")]
	public string myName;
	public float CurrentScore = 0;
	public float ScoreSmoothing = 20;
	public int place;

	[Header ("Stats UI")]
	public GameObject LeaderboardEntryUI;
	public GameObject GameOverUI;
	public Animator GameOverAnim;
	public TextMeshProUGUI TotalGameTimeText;
	public TextMeshProUGUI TotalRealTimeText;
	public TextMeshProUGUI TimeRatioText;
	public TextMeshProUGUI TotalBulletsShotText;
	public TextMeshProUGUI TotalBlockDestroyedText;
	public TextMeshProUGUI AccuracyText;

	[Header ("Level Up UI")]
	public TextMeshProUGUI FinalScoreText;
	public TextMeshProUGUI HighScoreLabel;

	[Header ("Level Calculation")]
	public float FinalScore;
	public int CurrentXP;
	public AudioSource XPIncreaseSound;

	public GameObject LeaderboardUI;
	public Button LeaderboardCloseButton;
	public Button ContinueButton;
	public PointerEventData eventData;

	void Start ()
	{
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();
		allowupdateentry = true;
		InvokeRepeating ("UpdateFinalScoreText", 0, 0.5f);
		//GetXpToAdd ();
		//GetGameOverStats ();
		eventData = new PointerEventData (EventSystem.current);
	}

	void Update () 
	{
		if (FinalScoreText.gameObject.activeInHierarchy == true) 
		{
			CurrentScore = Mathf.Lerp (CurrentScore, FinalScore, ScoreSmoothing * Time.unscaledDeltaTime);
		}

		if (gameControllerScript.playerControllerScript_P1.playerActions.Shoot.IsPressed) 
		{
			if (GameOverUI.activeInHierarchy == true && 
				leaderboardDisplay.UI.gameObject.activeInHierarchy == false) 
			{
				// Execute the OnClick event for the continue button.
				ExecuteEvents.Execute (
					ContinueButton.gameObject, 
					eventData, 
					ExecuteEvents.pointerClickHandler
				);
			}

			if (leaderboardDisplay.UI.gameObject.activeInHierarchy == true) 
			{
				ExecuteEvents.Execute (
					LeaderboardCloseButton.gameObject, 
					eventData, 
					ExecuteEvents.pointerClickHandler
				);
			}
		}
	}

	public void GetGameOverStats ()
	{
		TotalGameTimeText.text = 
			"Total Game Time: " + string.Format (
				"{0}:{1:00}", 
				(int)gameControllerScript.GameTime / 60, 
				(int)gameControllerScript.GameTime % 60
			);

		TotalRealTimeText.text = 
			"Total Real Time: " + string.Format (
				"{0}:{1:00}", 
				(int)gameControllerScript.RealTime / 60, 
				(int)gameControllerScript.RealTime % 60
			);	
		
		TimeRatioText.text = 
			"Time Ratio: " + System.Math.Round (gameControllerScript.TimeRatio, 2);

		TotalBulletsShotText.text = 
			"Total Bullets Shot: " + gameControllerScript.BulletsShot;

		TotalBlockDestroyedText.text = 
			"Total Blocks Destroyed: " + gameControllerScript.BlocksDestroyed;

		AccuracyText.text = 
			"Accuracy: " + (System.Math.Round((gameControllerScript.BlockShotAccuracy * 100), 2)) + "%";

		UpdateFinalScoreText ();
	}

	public void CheckLeaderboard ()
	{
		// Get final score figure.
		FinalScore = Mathf.RoundToInt (gameControllerScript.DisplayScore);

		Debug.Log ("Final score is: " + FinalScore);

		// Allows a leaderboard entry UI to activate.
		switch (saveAndLoadScript.MissionId) 
		{
		case 0:
			// Loop through all positions in leaderboard. Add one entry only when requirement is met. 
			for (int i = 0; i < saveAndLoadScript.Leaderboards[0].leaderboard.Count; i++) 
			{
				if (FinalScore > saveAndLoadScript.Leaderboards[0].leaderboard [i].score && allowupdateentry == true) 
				{
					place = i;
					LeaderboardEntryUI.SetActive (true);
					allowupdateentry = false;
					GameOverUI.SetActive (false);
					UpdateFinalScoreText ();
				}
			}
			break;
		case 2:
			// Loop through all positions in leaderboard. Add one entry only when requirement is met. 
			for (int i = 0; i < saveAndLoadScript.Leaderboards[2].leaderboard.Count; i++) 
			{
				if (FinalScore > saveAndLoadScript.Leaderboards[2].leaderboard [i].score && allowupdateentry == true) 
				{
					place = i;
					LeaderboardEntryUI.SetActive (true);
					allowupdateentry = false;
					GameOverUI.SetActive (false);
					UpdateFinalScoreText ();
				}
			}
			break;
		case 3:
			// Loop through all positions in leaderboard. Add one entry only when requirement is met. 
			for (int i = 0; i < saveAndLoadScript.Leaderboards[3].leaderboard.Count; i++) 
			{
				if (FinalScore > saveAndLoadScript.Leaderboards[3].leaderboard [i].score && allowupdateentry == true) 
				{
					place = i;
					LeaderboardEntryUI.SetActive (true);
					allowupdateentry = false;
					GameOverUI.SetActive (false);
					UpdateFinalScoreText ();
				}
			}
			break;
		case 4:
			// Loop through all positions in leaderboard. Add one entry only when requirement is met. 
			for (int i = 0; i < saveAndLoadScript.Leaderboards[4].leaderboard.Count; i++) 
			{
				if (FinalScore > saveAndLoadScript.Leaderboards[4].leaderboard [i].score && allowupdateentry == true) 
				{
					place = i;
					LeaderboardEntryUI.SetActive (true);
					allowupdateentry = false;
					GameOverUI.SetActive (false);
					UpdateFinalScoreText ();
				}
			}
			break;
		case 5:
			// Loop through all positions in leaderboard. Add one entry only when requirement is met. 
			for (int i = 0; i < saveAndLoadScript.Leaderboards[5].leaderboard.Count; i++) 
			{
				if (FinalScore > saveAndLoadScript.Leaderboards[5].leaderboard [i].score && allowupdateentry == true) 
				{
					place = i;
					LeaderboardEntryUI.SetActive (true);
					allowupdateentry = false;
					GameOverUI.SetActive (false);
					UpdateFinalScoreText ();
				}
			}
			break;
		case 6:
			// Loop through all positions in leaderboard. Add one entry only when requirement is met. 
			for (int i = 0; i < saveAndLoadScript.Leaderboards[6].leaderboard.Count; i++) 
			{
				if (FinalScore > saveAndLoadScript.Leaderboards[6].leaderboard [i].score && allowupdateentry == true) 
				{
					place = i;
					LeaderboardEntryUI.SetActive (true);
					allowupdateentry = false;
					GameOverUI.SetActive (false);
					UpdateFinalScoreText ();
				}
			}
			break;
		case 7:
			// Loop through all positions in leaderboard. Add one entry only when requirement is met. 
			for (int i = 0; i < saveAndLoadScript.Leaderboards[7].leaderboard.Count; i++) 
			{
				if (FinalScore > saveAndLoadScript.Leaderboards[7].leaderboard [i].score && allowupdateentry == true) 
				{
					place = i;
					LeaderboardEntryUI.SetActive (true);
					allowupdateentry = false;
					GameOverUI.SetActive (false);
					UpdateFinalScoreText ();
				}
			}
			break;
		}
					
		// Gets called on enable if theres no high score being added.
		if (allowupdateentry == true) 
		{
			GameOverUI.SetActive (true);
			GameOverAnim.enabled = true;
			GetGameOverStats ();
			Debug.Log ("Not a new high score.");
			allowupdateentry = false;
			UpdateFinalScoreText ();
		}
	}

	public void NewLeaderboardEntry (int position, string name)
	{
		Debug.Log (
			"Place: " + (position + 1).ToString () + 
			", Final Score: " + FinalScore +  
			", Wave: " + gameControllerScript.Wave
		);

		place = position;
		newLeaderboardEntry = new LeaderboardEntry (name, Mathf.RoundToInt (FinalScore), gameControllerScript.Wave);

		saveAndLoadScript.Leaderboards [saveAndLoadScript.MissionId].leaderboard.Insert (position, newLeaderboardEntry);
		saveAndLoadScript.Leaderboards [saveAndLoadScript.MissionId].leaderboard.RemoveAt (10);
	
		return;
	}
		
	void GetXpToAdd ()
	{
		CurrentXP = Mathf.RoundToInt (FinalScore);
		saveAndLoadScript.ExperiencePoints += CurrentXP;
	}

	public void UpdateFinalScoreText ()
	{
		CurrentScore = gameControllerScript.DisplayScore;

		if (FinalScoreText.gameObject.activeInHierarchy == true) 
		{
			FinalScoreText.text = CurrentScore + "";
		}

		FinalScoreText.text = CurrentScore.ToString ("N0");
	}
}