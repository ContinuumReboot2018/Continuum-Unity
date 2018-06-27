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
			for (int i = 0; i < saveAndLoadScript.Leaderboard_Arcade.Count; i++) 
			{
				if (FinalScore > saveAndLoadScript.Leaderboard_Arcade [i].score && allowupdateentry == true) 
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
			for (int i = 0; i < saveAndLoadScript.Leaderboard_BossRush.Count; i++) 
			{
				if (FinalScore > saveAndLoadScript.Leaderboard_BossRush [i].score && allowupdateentry == true) 
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
			for (int i = 0; i < saveAndLoadScript.Leaderboard_Lucky.Count; i++) 
			{
				if (FinalScore > saveAndLoadScript.Leaderboard_Lucky [i].score && allowupdateentry == true) 
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
			for (int i = 0; i < saveAndLoadScript.Leaderboard_FullyLoaded.Count; i++) 
			{
				if (FinalScore > saveAndLoadScript.Leaderboard_FullyLoaded [i].score && allowupdateentry == true) 
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
			for (int i = 0; i < saveAndLoadScript.Leaderboard_Scavenger.Count; i++) 
			{
				if (FinalScore > saveAndLoadScript.Leaderboard_Scavenger [i].score && allowupdateentry == true) 
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
			for (int i = 0; i < saveAndLoadScript.Leaderboard_Hell.Count; i++) 
			{
				if (FinalScore > saveAndLoadScript.Leaderboard_Hell [i].score && allowupdateentry == true) 
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
			for (int i = 0; i < saveAndLoadScript.Leaderboard_FastTrack.Count; i++) 
			{
				if (FinalScore > saveAndLoadScript.Leaderboard_FastTrack [i].score && allowupdateentry == true) 
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

		switch (saveAndLoadScript.MissionId) 
		{
		case 0:
			saveAndLoadScript.Leaderboard_Arcade.Insert (position, newLeaderboardEntry);
			saveAndLoadScript.Leaderboard_Arcade.RemoveAt (10);
			break;
		case 1:
			// mods
			break;
		case 2:
			saveAndLoadScript.Leaderboard_BossRush.Insert (position, newLeaderboardEntry);
			saveAndLoadScript.Leaderboard_BossRush.RemoveAt (10);
			break;
		case 3:
			saveAndLoadScript.Leaderboard_Lucky.Insert (position, newLeaderboardEntry);
			saveAndLoadScript.Leaderboard_Lucky.RemoveAt (10);
			break;
		case 4:
			saveAndLoadScript.Leaderboard_FullyLoaded.Insert (position, newLeaderboardEntry);
			saveAndLoadScript.Leaderboard_FullyLoaded.RemoveAt (10);
			break;
		case 5:
			saveAndLoadScript.Leaderboard_Scavenger.Insert (position, newLeaderboardEntry);
			saveAndLoadScript.Leaderboard_Scavenger.RemoveAt (10);
			break;
		case 6:
			saveAndLoadScript.Leaderboard_Hell.Insert (position, newLeaderboardEntry);
			saveAndLoadScript.Leaderboard_Hell.RemoveAt (10);
			break;
		case 7:
			saveAndLoadScript.Leaderboard_FastTrack.Insert (position, newLeaderboardEntry);
			saveAndLoadScript.Leaderboard_FastTrack.RemoveAt (10);
			break;
		}

		//saveAndLoadScript.Leaderboards [saveAndLoadScript.MissionId].leaderboard.Insert (position, newLeaderboardEntry);
		//saveAndLoadScript.Leaderboards [saveAndLoadScript.MissionId].leaderboard.RemoveAt (10);
	
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