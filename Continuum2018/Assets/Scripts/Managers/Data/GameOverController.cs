using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;

using TMPro;

public class GameOverController : MonoBehaviour 
{
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

	void OnEnable ()
	{
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();
		allowupdateentry = true;
		CheckLeaderboard ();
		GetGameOverStats ();
	}

	void Start ()
	{
		InvokeRepeating ("UpdateFinalScoreText", 0, 0.5f);
		GetXpToAdd ();
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

	void CheckLeaderboard ()
	{
		// Get final score figure.
		FinalScore = Mathf.RoundToInt (gameControllerScript.DisplayScore);

		Debug.Log ("Final score is: " + FinalScore);

		switch (saveAndLoadScript.MissionId) 
		{
		case 0:
			// Loop through all positions in leaderboard. Add one entry only when requirement is met. 
			for (int i = 0; i < saveAndLoadScript.Leaderboard_ArcadeMode.Count; i++) 
			{
				if (FinalScore > saveAndLoadScript.Leaderboard_ArcadeMode [i].score && allowupdateentry == true) 
				{
					place = i;
					LeaderboardEntryUI.SetActive (true);
					allowupdateentry = false;
					GameOverUI.SetActive (false);
					UpdateFinalScoreText ();
					return;
				}
			}
			break;
		case 1:
			/*
			// Loop through all positions in leaderboard. Add one entry only when requirement is met. 
			for (int i = 0; i < saveAndLoadScript.Leaderboard_ModsMode.Count; i++) 
			{
				if (FinalScore > saveAndLoadScript.Leaderboard_ModsMode [i].score && allowupdateentry == true) 
				{
					place = i;
					LeaderboardEntryUI.SetActive (true);
					allowupdateentry = false;
					GameOverUI.SetActive (false);
					UpdateFinalScoreText ();
					return;
				}
			}
			*/
			break;
		case 2:
			// Loop through all positions in leaderboard. Add one entry only when requirement is met. 
			for (int i = 0; i < saveAndLoadScript.Leaderboard_BossRushMode.Count; i++) 
			{
				if (FinalScore > saveAndLoadScript.Leaderboard_BossRushMode [i].score && allowupdateentry == true) 
				{
					place = i;
					LeaderboardEntryUI.SetActive (true);
					allowupdateentry = false;
					GameOverUI.SetActive (false);
					UpdateFinalScoreText ();
					return;
				}
			}
			break;
		case 3:
			// Loop through all positions in leaderboard. Add one entry only when requirement is met. 
			for (int i = 0; i < saveAndLoadScript.Leaderboard_LuckyMode.Count; i++) 
			{
				if (FinalScore > saveAndLoadScript.Leaderboard_LuckyMode [i].score && allowupdateentry == true) 
				{
					place = i;
					LeaderboardEntryUI.SetActive (true);
					allowupdateentry = false;
					GameOverUI.SetActive (false);
					UpdateFinalScoreText ();
					return;
				}
			}
			break;
		case 4:
			// Loop through all positions in leaderboard. Add one entry only when requirement is met. 
			for (int i = 0; i < saveAndLoadScript.Leaderboard_FullyLoadedMode.Count; i++) 
			{
				if (FinalScore > saveAndLoadScript.Leaderboard_FullyLoadedMode [i].score && allowupdateentry == true) 
				{
					place = i;
					LeaderboardEntryUI.SetActive (true);
					allowupdateentry = false;
					GameOverUI.SetActive (false);
					UpdateFinalScoreText ();
					return;
				}
			}
			break;
		case 5:
			// Loop through all positions in leaderboard. Add one entry only when requirement is met. 
			for (int i = 0; i < saveAndLoadScript.Leaderboard_ScavengerMode.Count; i++) 
			{
				if (FinalScore > saveAndLoadScript.Leaderboard_ScavengerMode [i].score && allowupdateentry == true) 
				{
					place = i;
					LeaderboardEntryUI.SetActive (true);
					allowupdateentry = false;
					GameOverUI.SetActive (false);
					UpdateFinalScoreText ();
					return;
				}
			}
			break;
		case 6:
			// Loop through all positions in leaderboard. Add one entry only when requirement is met. 
			for (int i = 0; i < saveAndLoadScript.Leaderboard_HellMode.Count; i++) 
			{
				if (FinalScore > saveAndLoadScript.Leaderboard_HellMode [i].score && allowupdateentry == true) 
				{
					place = i;
					LeaderboardEntryUI.SetActive (true);
					allowupdateentry = false;
					GameOverUI.SetActive (false);
					UpdateFinalScoreText ();
					return;
				}
			}
			break;
		case 7:
			// Loop through all positions in leaderboard. Add one entry only when requirement is met. 
			for (int i = 0; i < saveAndLoadScript.Leaderboard_FastTrackMode.Count; i++) 
			{
				if (FinalScore > saveAndLoadScript.Leaderboard_FastTrackMode [i].score && allowupdateentry == true) 
				{
					place = i;
					LeaderboardEntryUI.SetActive (true);
					allowupdateentry = false;
					GameOverUI.SetActive (false);
					UpdateFinalScoreText ();
					return;
				}
			}
			break;
		}

		/*
		// Loop through all positions in leaderboard. Add one entry only when requirement is met. 
		for (int i = 0; i < saveAndLoadScript.Leaderboard.Count; i++) 
		{
			if (FinalScore > saveAndLoadScript.Leaderboard [i].score && allowupdateentry == true) 
			{
				place = i;
				LeaderboardEntryUI.SetActive (true);
				allowupdateentry = false;
				GameOverUI.SetActive (false);
				UpdateFinalScoreText ();
				return;
			}
		}
		*/
					
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
		place = position;

		LeaderboardEntry newLeaderboardEntry = new LeaderboardEntry (name, Mathf.RoundToInt (FinalScore), gameControllerScript.Wave);

		switch (saveAndLoadScript.MissionId)
		{
		case 0:
			saveAndLoadScript.Leaderboard_ArcadeMode.Insert (position, newLeaderboardEntry);
			saveAndLoadScript.Leaderboard_ArcadeMode.RemoveAt (10);
			break;
		case 1:
			//saveAndLoadScript.Leaderboard_ModsMode.Insert (position, newLeaderboardEntry);
			//saveAndLoadScript.Leaderboard_ModsMode.RemoveAt (10);
			break;
		case 2:
			saveAndLoadScript.Leaderboard_BossRushMode.Insert (position, newLeaderboardEntry);
			saveAndLoadScript.Leaderboard_BossRushMode.RemoveAt (10);
			break;
		case 3:
			saveAndLoadScript.Leaderboard_LuckyMode.Insert (position, newLeaderboardEntry);
			saveAndLoadScript.Leaderboard_LuckyMode.RemoveAt (10);
			break;
		case 4:
			saveAndLoadScript.Leaderboard_FullyLoadedMode.Insert (position, newLeaderboardEntry);
			saveAndLoadScript.Leaderboard_FullyLoadedMode.RemoveAt (10);
			break;
		case 5:
			saveAndLoadScript.Leaderboard_ScavengerMode.Insert (position, newLeaderboardEntry);
			saveAndLoadScript.Leaderboard_ScavengerMode.RemoveAt (10);
			break;
		case 6:
			saveAndLoadScript.Leaderboard_HellMode.Insert (position, newLeaderboardEntry);
			saveAndLoadScript.Leaderboard_HellMode.RemoveAt (10);
			break;
		case 7:
			saveAndLoadScript.Leaderboard_FastTrackMode.Insert (position, newLeaderboardEntry);
			saveAndLoadScript.Leaderboard_FastTrackMode.RemoveAt (10);
			break;
		}

		//saveAndLoadScript.Leaderboard.Insert (position, newLeaderboardEntry);
		//saveAndLoadScript.Leaderboard.RemoveAt (10);

		Debug.Log (
			"Place: " + (position + 1).ToString () + 
			", Final Score: " + FinalScore +  
			", Wave: " + gameControllerScript.Wave
		);

		saveAndLoadScript.SavePlayerData ();
	}
		
	void GetXpToAdd ()
	{
		CurrentXP = Mathf.RoundToInt (FinalScore);
		saveAndLoadScript.ExperiencePoints += CurrentXP;
		saveAndLoadScript.SavePlayerData ();
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