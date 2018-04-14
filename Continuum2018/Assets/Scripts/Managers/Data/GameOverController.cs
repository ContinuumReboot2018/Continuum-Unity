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
	}

	void Start ()
	{
		GetGameOverStats ();

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
