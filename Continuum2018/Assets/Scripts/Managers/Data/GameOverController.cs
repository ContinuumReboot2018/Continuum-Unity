using UnityEngine;
using UnityEngine.UI;
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

	void OnEnable ()
	{
		allowupdateentry = true;
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();
		CheckLeaderboard ();

		//InvokeRepeating ("UpdateFinalScoreText", 0, 1); // Don't do this when Time.timeScale == 0;

		StartCoroutine (UpdateFinalScoreText ());
	}

	void OnDisable ()
	{
		StopCoroutine (UpdateFinalScoreText ());
	}

	void Start ()
	{
		GetXpToAdd ();
	}

	void Update () 
	{
		if (FinalScoreText.gameObject.activeInHierarchy == true) 
		{
			CurrentScore = Mathf.Lerp (CurrentScore, FinalScore, ScoreSmoothing * Time.unscaledDeltaTime);
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

		CheckLeaderboard ();
	}

	void CheckLeaderboard ()
	{
		// Get final score figure.
		FinalScore = Mathf.RoundToInt (gameControllerScript.DisplayScore);

		Debug.Log ("Final score is: " + FinalScore);

		// Loop through all positions in leaderboard. Add one entry only when requirement is met. 
		for (int i = 0; i < saveAndLoadScript.Leaderboard.Count; i++) 
		{
			if (FinalScore > saveAndLoadScript.Leaderboard [i].score && allowupdateentry == true) 
			{
				place = i;
				LeaderboardEntryUI.SetActive (true);
				allowupdateentry = false;
				GameOverUI.SetActive (false);
				return;
			}
		}
			
		if (allowupdateentry == true) 
		{
			GameOverAnim.enabled = true;
			Debug.Log ("Not a new high score.");
			allowupdateentry = false;
		}
	}

	public void NewLeaderboardEntry (int position, string name)
	{
		place = position;

		LeaderboardEntry newLeaderboardEntry = new LeaderboardEntry (name, Mathf.RoundToInt (FinalScore), gameControllerScript.Wave);

		saveAndLoadScript.Leaderboard.Insert (position, newLeaderboardEntry);
		saveAndLoadScript.Leaderboard.RemoveAt (10);

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

	IEnumerator UpdateFinalScoreText ()
	{
		while (true) 
		{
			yield return new WaitForSecondsRealtime (1);

			if (FinalScoreText.gameObject.activeInHierarchy == true) 
			{
				FinalScoreText.text = CurrentScore + "";
			}

			yield return null;
		}
	}
}