using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverController : MonoBehaviour 
{
	public string myName;

	public SaveAndLoadScript saveAndLoadScript;
	public GameController gameControllerScript;
	public DeveloperMode developerModeScript;
	public LeaderboardDisplay leaderboardDisplay;
	public GameObject LeaderboardEntryUI;
	public Animator GameOverAnim;

	public float CurrentScore = 0;
	public float ScoreSmoothing = 20;
	public int place;

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

	[Header ("Level Calculation")]
	public float FinalScore;
	public int CurrentXP;

	public AudioSource XPIncreaseSound;

	void Awake ()
	{
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();
		CheckLeaderboard ();
		//GetGameOverStats ();
	}

	void Start ()
	{
		GetXpToAdd ();
	}

	void Update () 
	{
		if (CurrentXP < 0) 
		{
			CurrentXP = 0;
		}

		if (FinalScoreText.gameObject.activeInHierarchy == true) 
		{
			UpdateFinalScoreText ();
		}
	}

	void GetGameOverStats ()
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

		CheckLeaderboard ();
	}

	void CheckLeaderboard ()
	{
		// Get final score figure.
		FinalScore = gameControllerScript.DisplayScore;
		int _FinalScore = Mathf.RoundToInt (FinalScore);
		Debug.Log ("Final score is: " + _FinalScore);

		bool allowupdateentry = true;

		// Loop through all positions in leaderboard. Add one entry only when requirement is met. 
		for (int i = 0; i < saveAndLoadScript.Leaderboard.Count; i++) 
		{
			if (_FinalScore > saveAndLoadScript.Leaderboard [i].score && allowupdateentry == true) 
			{
				LeaderboardEntryUI.SetActive (true);
				allowupdateentry = false;
				return;
			}
		}
			
		GameOverAnim.enabled = true;
		Debug.Log ("Not a new high score.");
	}

	public void NewLeaderboardEntry (int position, string name)
	{
		place = position;

		LeaderboardEntry newLeaderboardEntry = new LeaderboardEntry (name, Mathf.RoundToInt (FinalScore), gameControllerScript.Wave);
		newLeaderboardEntry.name = name;
		newLeaderboardEntry.score = Mathf.RoundToInt (FinalScore);
		newLeaderboardEntry.wave = gameControllerScript.Wave;

		saveAndLoadScript.Leaderboard.Insert (position, newLeaderboardEntry);
		saveAndLoadScript.Leaderboard.RemoveAt (10);

		Debug.Log (
			"Position: " + (position + 1).ToString () + 
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

	void UpdateFinalScoreText ()
	{
		CurrentScore = Mathf.Lerp (CurrentScore, FinalScore, ScoreSmoothing * Time.unscaledDeltaTime);
		FinalScoreText.text = CurrentScore + "";
	}
}