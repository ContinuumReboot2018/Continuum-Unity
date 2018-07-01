using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;
using System.Collections.Generic;

using TMPro;

public class GameOverController : MonoBehaviour 
{
	public static GameOverController Instance { get; private set; }

	public LeaderboardEntry newLeaderboardEntry;
	private bool allowupdateentry;

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

	void Awake ()
	{
		Instance = this;
		// DontDestroyOnLoad (gameObject);
	}

	void Start ()
	{
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

		if (GameController.Instance.playerControllerScript_P1.playerActions.Shoot.IsPressed) 
		{
			if (GameOverUI.activeInHierarchy == true && 
				LeaderboardDisplay.Instance.UI.gameObject.activeInHierarchy == false) 
			{
				// Execute the OnClick event for the continue button.
				ExecuteEvents.Execute (
					ContinueButton.gameObject, 
					eventData, 
					ExecuteEvents.pointerClickHandler
				);
			}

			if (LeaderboardDisplay.Instance.UI.gameObject.activeInHierarchy == true) 
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
				(int)GameController.Instance.GameTime / 60, 
				(int)GameController.Instance.GameTime % 60
			);

		TotalRealTimeText.text = 
			"Total Real Time: " + string.Format (
				"{0}:{1:00}", 
				(int)GameController.Instance.RealTime / 60, 
				(int)GameController.Instance.RealTime % 60
			);	
		
		TimeRatioText.text = 
			"Time Ratio: " + System.Math.Round (GameController.Instance.TimeRatio, 2);

		TotalBulletsShotText.text = 
			"Total Bullets Shot: " + GameController.Instance.BulletsShot;

		TotalBlockDestroyedText.text = 
			"Total Blocks Destroyed: " + GameController.Instance.BlocksDestroyed;

		AccuracyText.text = 
			"Accuracy: " + (System.Math.Round((GameController.Instance.BlockShotAccuracy * 100), 2)) + "%";

		UpdateFinalScoreText ();
	}

	public void CheckLeaderboard ()
	{
		// Get final score figure.
		FinalScore = Mathf.RoundToInt (GameController.Instance.DisplayScore);

		Debug.Log ("Final score is: " + FinalScore);

		// Allows a leaderboard entry UI to activate.
		switch (SaveAndLoadScript.Instance.MissionId) 
		{
		case 0:
			// Loop through all positions in leaderboard. Add one entry only when requirement is met. 
			for (int i = 0; i < SaveAndLoadScript.Instance.Leaderboard_Arcade.Count; i++) 
			{
				if (FinalScore > SaveAndLoadScript.Instance.Leaderboard_Arcade [i].score && allowupdateentry == true) 
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
			for (int i = 0; i < SaveAndLoadScript.Instance.Leaderboard_BossRush.Count; i++) 
			{
				if (FinalScore > SaveAndLoadScript.Instance.Leaderboard_BossRush [i].score && allowupdateentry == true) 
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
			for (int i = 0; i < SaveAndLoadScript.Instance.Leaderboard_Lucky.Count; i++) 
			{
				if (FinalScore > SaveAndLoadScript.Instance.Leaderboard_Lucky [i].score && allowupdateentry == true) 
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
			for (int i = 0; i < SaveAndLoadScript.Instance.Leaderboard_FullyLoaded.Count; i++) 
			{
				if (FinalScore > SaveAndLoadScript.Instance.Leaderboard_FullyLoaded [i].score && allowupdateentry == true) 
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
			for (int i = 0; i < SaveAndLoadScript.Instance.Leaderboard_Scavenger.Count; i++) 
			{
				if (FinalScore > SaveAndLoadScript.Instance.Leaderboard_Scavenger [i].score && allowupdateentry == true) 
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
			for (int i = 0; i < SaveAndLoadScript.Instance.Leaderboard_Hell.Count; i++) 
			{
				if (FinalScore > SaveAndLoadScript.Instance.Leaderboard_Hell [i].score && allowupdateentry == true) 
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
			for (int i = 0; i < SaveAndLoadScript.Instance.Leaderboard_FastTrack.Count; i++) 
			{
				if (FinalScore > SaveAndLoadScript.Instance.Leaderboard_FastTrack [i].score && allowupdateentry == true) 
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
			", Wave: " + GameController.Instance.Wave
		);

		place = position;
		newLeaderboardEntry = new LeaderboardEntry (name, Mathf.RoundToInt (FinalScore), GameController.Instance.Wave);

		switch (SaveAndLoadScript.Instance.MissionId) 
		{
		case 0:
			SaveAndLoadScript.Instance.Leaderboard_Arcade.Insert (position, newLeaderboardEntry);
			SaveAndLoadScript.Instance.Leaderboard_Arcade.RemoveAt (10);
			break;
		case 1:
			// mods
			break;
		case 2:
			SaveAndLoadScript.Instance.Leaderboard_BossRush.Insert (position, newLeaderboardEntry);
			SaveAndLoadScript.Instance.Leaderboard_BossRush.RemoveAt (10);
			break;
		case 3:
			SaveAndLoadScript.Instance.Leaderboard_Lucky.Insert (position, newLeaderboardEntry);
			SaveAndLoadScript.Instance.Leaderboard_Lucky.RemoveAt (10);
			break;
		case 4:
			SaveAndLoadScript.Instance.Leaderboard_FullyLoaded.Insert (position, newLeaderboardEntry);
			SaveAndLoadScript.Instance.Leaderboard_FullyLoaded.RemoveAt (10);
			break;
		case 5:
			SaveAndLoadScript.Instance.Leaderboard_Scavenger.Insert (position, newLeaderboardEntry);
			SaveAndLoadScript.Instance.Leaderboard_Scavenger.RemoveAt (10);
			break;
		case 6:
			SaveAndLoadScript.Instance.Leaderboard_Hell.Insert (position, newLeaderboardEntry);
			SaveAndLoadScript.Instance.Leaderboard_Hell.RemoveAt (10);
			break;
		case 7:
			SaveAndLoadScript.Instance.Leaderboard_FastTrack.Insert (position, newLeaderboardEntry);
			SaveAndLoadScript.Instance.Leaderboard_FastTrack.RemoveAt (10);
			break;
		}
			
		return;
	}
		
	public void GetXpToAdd ()
	{
		CurrentXP = Mathf.RoundToInt (FinalScore);
		SaveAndLoadScript.Instance.ExperiencePoints += CurrentXP;
	}

	public void GetBlocksDestroyedToAdd ()
	{
		SaveAndLoadScript.Instance.blocksDestroyed += GameController.Instance.BlocksDestroyed;
	}

	public void UpdateFinalScoreText ()
	{
		CurrentScore = GameController.Instance.DisplayScore;

		if (FinalScoreText.gameObject.activeInHierarchy == true) 
		{
			FinalScoreText.text = CurrentScore + "";
		}

		FinalScoreText.text = CurrentScore.ToString ("N0");
	}
}