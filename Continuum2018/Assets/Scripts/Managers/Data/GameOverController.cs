using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour 
{
	public SaveAndLoadScript saveAndLoadScript;
	public GameController gameControllerScript;
	public DeveloperMode developerModeScript;

	[Header ("Stats UI")]
	public TextMeshProUGUI TotalGameTimeText;
	public TextMeshProUGUI TotalRealTimeText;
	public TextMeshProUGUI TimeRatioText;
	public TextMeshProUGUI TotalBulletsShotText;
	public TextMeshProUGUI TotalBlockDestroyedText;
	public TextMeshProUGUI AccuracyText;

	[Header ("Level Up UI")]
	public TextMeshProUGUI FinalScoreText;
	public TextMeshProUGUI HighScoreLabel;
	//public Slider LevelSlider;
	//public TextMeshProUGUI CurrentLevelLabel;
	//public TextMeshProUGUI NexLevelLabel;
	//public TextMeshProUGUI CurrentXPNeededText;
	//public TextMeshProUGUI NextLevelXPNeededText;
	//public TextMeshProUGUI NextLevelXPRemainText;
	//public TextMeshProUGUI LevelUpText;

	[Header ("Level Calculation")]
	public float FinalScore;
	//public float xpIncreaseSpeed = 1;
	public int CurrentXP;
	//public int CurrentLevelXP;
	//public int[] NextLevelXP;
	//public int[] IncreaseRates;

	public AudioSource XPIncreaseSound;

	void Awake ()
	{
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();
		GetGameOverStats ();
	}

	void Start ()
	{
		//Invoke ("GetGameOverStats", 1);

		//StartCoroutine (GetGameOverStats ());

		//LevelUpText.enabled = false;
		//RefreshLevelLabels ();
		//GetNewLevelAmounts ();
		GetXpToAdd ();
	}

	void Update () 
	{
		//SetLevelSliderValue ();

		if (CurrentXP < 0) 
		{
			CurrentXP = 0;
		}
	}

	void GetGameOverStats ()
	//IEnumerator GetGameOverStats ()
	{
		//yield return new WaitForSecondsRealtime (1);

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

		CheckLeaderboard ();
		//saveAndLoadScript.SavePlayerData ();
	}

	void CheckLeaderboard ()
	{
		FinalScore = gameControllerScript.DisplayScore;
		int _FinalScore = Mathf.RoundToInt (FinalScore);
		Debug.Log ("Final score is: " + _FinalScore);
		bool allowupdateentry = true;

		if (_FinalScore > saveAndLoadScript.Leaderboard [0].score && allowupdateentry == true) 
		{
			// This is a new high score!
			LeaderboardEntry firstplace = new LeaderboardEntry (saveAndLoadScript.Username, Mathf.RoundToInt (FinalScore), gameControllerScript.Wave);
			firstplace.name = saveAndLoadScript.Username;
			firstplace.score = Mathf.RoundToInt (FinalScore);
			firstplace.wave = gameControllerScript.Wave;

			saveAndLoadScript.Leaderboard.Insert (0, firstplace);
			saveAndLoadScript.Leaderboard.RemoveAt (10);
			Debug.Log ("1st place! Final score: " + FinalScore +  ", Wave: " + gameControllerScript.Wave);
			allowupdateentry = false;
			return;
		}

		if (_FinalScore > saveAndLoadScript.Leaderboard [1].score && allowupdateentry == true) 
		{
			LeaderboardEntry secondplace = new LeaderboardEntry (saveAndLoadScript.Username, Mathf.RoundToInt (FinalScore), gameControllerScript.Wave);
			secondplace.name = saveAndLoadScript.Username;
			secondplace.score = Mathf.RoundToInt (FinalScore);
			secondplace.wave = gameControllerScript.Wave;

			saveAndLoadScript.Leaderboard.Insert (1, secondplace);
			saveAndLoadScript.Leaderboard.RemoveAt (10);
			Debug.Log ("2nd place! Final score: " + FinalScore +  ", Wave: " + gameControllerScript.Wave);
			allowupdateentry = false;
			return;
		}

		if (_FinalScore > saveAndLoadScript.Leaderboard [2].score && allowupdateentry == true) 
		{
			LeaderboardEntry thirdplace = new LeaderboardEntry (saveAndLoadScript.Username, Mathf.RoundToInt (FinalScore), gameControllerScript.Wave);
			thirdplace.name = saveAndLoadScript.Username;
			thirdplace.score = Mathf.RoundToInt (FinalScore);
			thirdplace.wave = gameControllerScript.Wave;

			saveAndLoadScript.Leaderboard.Insert (2, thirdplace);
			saveAndLoadScript.Leaderboard.RemoveAt (10);
			Debug.Log ("3rd place! Final score: " + FinalScore +  ", Wave: " + gameControllerScript.Wave);
			allowupdateentry = false;
			return;
		}

		if (_FinalScore > saveAndLoadScript.Leaderboard [3].score && allowupdateentry == true) 
		{
			LeaderboardEntry fourthplace = new LeaderboardEntry (saveAndLoadScript.Username, Mathf.RoundToInt (FinalScore), gameControllerScript.Wave);
			fourthplace.name = saveAndLoadScript.Username;
			fourthplace.score = Mathf.RoundToInt (FinalScore);
			fourthplace.wave = gameControllerScript.Wave;

			saveAndLoadScript.Leaderboard.Insert (3, fourthplace);
			saveAndLoadScript.Leaderboard.RemoveAt (10);
			Debug.Log ("4th place! Final score: " + FinalScore +  ", Wave: " + gameControllerScript.Wave);
			allowupdateentry = false;
			return;
		}

		if (_FinalScore > saveAndLoadScript.Leaderboard [4].score && allowupdateentry == true) 
		{
			LeaderboardEntry fifthplace = new LeaderboardEntry (saveAndLoadScript.Username, Mathf.RoundToInt (FinalScore), gameControllerScript.Wave);
			fifthplace.name = saveAndLoadScript.Username;
			fifthplace.score = Mathf.RoundToInt (FinalScore);
			fifthplace.wave = gameControllerScript.Wave;

			saveAndLoadScript.Leaderboard.Insert (4, fifthplace);
			saveAndLoadScript.Leaderboard.RemoveAt (10);
			Debug.Log ("5th place! Final score: " + FinalScore +  ", Wave: " + gameControllerScript.Wave);
			allowupdateentry = false;
			return;
		}

		if (_FinalScore > saveAndLoadScript.Leaderboard [5].score && allowupdateentry == true) 
		{
			LeaderboardEntry sixthplace = new LeaderboardEntry (saveAndLoadScript.Username, Mathf.RoundToInt (FinalScore), gameControllerScript.Wave);
			sixthplace.name = saveAndLoadScript.Username;
			sixthplace.score = Mathf.RoundToInt (FinalScore);
			sixthplace.wave = gameControllerScript.Wave;

			saveAndLoadScript.Leaderboard.Insert (5, sixthplace);
			saveAndLoadScript.Leaderboard.RemoveAt (10);
			Debug.Log ("6th place! Final score: " + FinalScore +  ", Wave: " + gameControllerScript.Wave);
			allowupdateentry = false;
			return;
		}

		if (_FinalScore > saveAndLoadScript.Leaderboard [6].score && allowupdateentry == true) 
		{
			LeaderboardEntry seventhplace = new LeaderboardEntry (saveAndLoadScript.Username, Mathf.RoundToInt (FinalScore), gameControllerScript.Wave);
			seventhplace.name = saveAndLoadScript.Username;
			seventhplace.score = Mathf.RoundToInt (FinalScore);
			seventhplace.wave = gameControllerScript.Wave;

			saveAndLoadScript.Leaderboard.Insert (6, seventhplace);
			saveAndLoadScript.Leaderboard.RemoveAt (10);
			Debug.Log ("7th place! Final score: " + FinalScore +  ", Wave: " + gameControllerScript.Wave);
			allowupdateentry = false;
			return;
		}

		if (_FinalScore > saveAndLoadScript.Leaderboard [7].score && allowupdateentry == true) 
		{
			LeaderboardEntry eighthplace = new LeaderboardEntry (saveAndLoadScript.Username, Mathf.RoundToInt (FinalScore), gameControllerScript.Wave);
			eighthplace.name = saveAndLoadScript.Username;
			eighthplace.score = Mathf.RoundToInt (FinalScore);
			eighthplace.wave = gameControllerScript.Wave;

			saveAndLoadScript.Leaderboard.Insert (7, eighthplace);
			saveAndLoadScript.Leaderboard.RemoveAt (10);
			Debug.Log ("8th place! Final score: " + FinalScore +  ", Wave: " + gameControllerScript.Wave);
			allowupdateentry = false;
			return;
		}

		if (_FinalScore > saveAndLoadScript.Leaderboard [8].score && allowupdateentry == true) 
		{
			LeaderboardEntry ninthplace = new LeaderboardEntry (saveAndLoadScript.Username, Mathf.RoundToInt (FinalScore), gameControllerScript.Wave);
			ninthplace.name = saveAndLoadScript.Username;
			ninthplace.score = Mathf.RoundToInt (FinalScore);
			ninthplace.wave = gameControllerScript.Wave;

			saveAndLoadScript.Leaderboard.Insert (8, ninthplace);
			saveAndLoadScript.Leaderboard.RemoveAt (10);
			Debug.Log ("9th place! Final score: " + FinalScore +  ", Wave: " + gameControllerScript.Wave);
			allowupdateentry = false;
			return;
		}

		if (_FinalScore > saveAndLoadScript.Leaderboard [9].score && allowupdateentry == true) 
		{
			LeaderboardEntry tenthplace = new LeaderboardEntry (saveAndLoadScript.Username, Mathf.RoundToInt (FinalScore), gameControllerScript.Wave);
			tenthplace.name = saveAndLoadScript.Username;
			tenthplace.score = Mathf.RoundToInt (FinalScore);
			tenthplace.wave = gameControllerScript.Wave;

			saveAndLoadScript.Leaderboard.Insert (9, tenthplace);
			saveAndLoadScript.Leaderboard.RemoveAt (10);
			Debug.Log ("10th place! Final score: " + FinalScore +  ", Wave: " + gameControllerScript.Wave);
			allowupdateentry = false;
			return;
		}

		Debug.Log ("Not a new high score.");
	}

	void LevelUp ()
	{
		//saveAndLoadScript.Level += 1;
		//GetNewLevelAmounts ();
		//RefreshLevelLabels ();
		//LevelUpText.enabled = true;
	}

	/*
	void RefreshLevelLabels ()
	{
		CurrentLevelLabel.text = "Lv." + saveAndLoadScript.Level;
		NexLevelLabel.text = "Lv." + (saveAndLoadScript.Level + 1);

		CurrentXPNeededText.text = "" + LevelSlider.minValue;
		NextLevelXPNeededText.text = "" + LevelSlider.maxValue;
	}

	void GetNewLevelAmounts ()
	{
		LevelSlider.minValue = NextLevelXP [saveAndLoadScript.Level];
		LevelSlider.maxValue = NextLevelXP [saveAndLoadScript.Level + 1];
	}*/

	void GetXpToAdd ()
	{
		FinalScore = gameControllerScript.DisplayScore;
		FinalScoreText.text = "" + FinalScore;
		CurrentXP = Mathf.RoundToInt (FinalScore);
		saveAndLoadScript.ExperiencePoints += CurrentXP;
		saveAndLoadScript.SavePlayerData ();
	}

	/*
	void SetLevelSliderValue ()
	{
		NextLevelXPRemainText.text = "" + CurrentXP;
		if (LevelSlider.value < LevelSlider.maxValue) 
		{
			if (CurrentXP > 0) 
			{
				LevelSlider.value += IncreaseRates [saveAndLoadScript.Level];
				CurrentXP -= IncreaseRates [saveAndLoadScript.Level];
			}
		}

		if (LevelSlider.value >= LevelSlider.maxValue) 
		{
			LevelUp ();
		}
	}*/
}
