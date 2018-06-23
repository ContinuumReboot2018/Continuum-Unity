using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class AchievementManager : MonoBehaviour 
{
	public PlayerController playerControllerScript;
	public GameController gameControllerScript;
	public SaveAndLoadScript saveAndLoadScript;

	[Header ("Achievement notification")]
	public Animator AchievementAnim;
	public RawImage AchievementImage;
	public TextMeshProUGUI AchievementTitle;
	public TextMeshProUGUI AchievementText;

	[Header ("Achievement assets")]
	public Texture BonusScoreTexture;
	public Texture BonusComboTexture;
	public Texture BonusBlocksDestroyedTexture;
	public Texture BonusPowerupsCollectedTexture;
	public Texture BonusAbilityActivationsTexture;
	public Texture BonusWaveReachedTexture;
	public Texture BonusNonShootingTimeTexture;
	public Texture BonusRiskDistanceTimeTexture;
	public AudioSource AchievementSound;

	[Header ("Achievement requirements")]
	public int ScoreToBonusRound = 2000;
	public int ScoreToBonusRoundMultiplier = 10;
	private int TimesScoredToBonus;
	[Space (10)]
	public int ComboToBonusRound = 20000;
	public int ComboToBonusRoundMultiplier = 5;
	private int TimesComboToBonus;
	[Space (10)]
	public int BlocksDestroyedToBonusRound = 500;
	public int BlocksDestroyedToBonusRoundMultiplier = 2;
	private int TimesBlocksDestroyedToBonus;
	[Space (10)]
	public int PowerupsCollectedToBonusRound = 5;
	public int PowerupsCollectedToBonusRoundMultiplier = 2;
	private int TimesPowerupsCollectedToBonus;
	[Space (10)]
	public int AbilitiesActivatedToBonusRound = 1;
	public int AbilitiesActivatedToBonusRoundMultiplier = 2;
	private int TimesAbilitiesActivatedToBonus;
	[Space (10)]
	public int WaveToBonusRound = 5;
	public int WaveToBonusRoundIncreaseAmount = 5;
	private int TimesWaveToBonusRoundReached;
	[Space (10)]
	public int NextNonShootingTimeToBonus = 30;
	public int NonShootingTimeToBonusIncreaseAmount = 30;
	private int TimesNonShootingToBonus;
	[Space (10)]
	public int NextRiskDistanceTimeToBonus = 10;
	public int NextRiskDistanceTimeToBonusMultiplier = 2;
	private int TimesRiskDistanceToBonus;

	void Start ()
	{
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();
		StartCoroutine (CheckScoreCount ());
		StartCoroutine (CheckComboCount ());
		StartCoroutine (CheckBlocksDestroyedCount ());
		StartCoroutine (CheckPowerupsCollectedCount ());
		StartCoroutine (CheckAbilityActivationsCount ());
		StartCoroutine (CheckWaveToBonusRoundCount ());
		StartCoroutine (CheckNonShootingTimeCount ());
		StartCoroutine (CheckRiskDistanceTimeCount ());
	}

	IEnumerator CheckScoreCount ()
	{
		yield return new WaitForSecondsRealtime (1);

		if (gameControllerScript.DisplayScore >= ScoreToBonusRound) 
		{
			gameControllerScript.doBonusRound = true;
			TimesScoredToBonus++;

			TriggerAchievementNotification (
				"Achievement", 
				BonusScoreTexture, 
				"BIG SCORE", 
				("Scored " + ScoreToBonusRound + " or more points").ToString(),
				TimesScoredToBonus
			);

			ScoreToBonusRound *= ScoreToBonusRoundMultiplier;
		}

		StartCoroutine (CheckScoreCount ());
	}

	IEnumerator CheckComboCount ()
	{
		yield return new WaitForSecondsRealtime (1);

		if (gameControllerScript.combo >= ComboToBonusRound) 
		{
			gameControllerScript.doBonusRound = true;
			TimesComboToBonus++;

			TriggerAchievementNotification (
				"Achievement", 
				BonusComboTexture, 
				"BIG COMBO", 
				("Scored combo of " + ComboToBonusRound + " or more").ToString(),
				TimesComboToBonus
			);

			ComboToBonusRound *= ComboToBonusRoundMultiplier;
		}

		StartCoroutine (CheckComboCount ());
	}

	IEnumerator CheckBlocksDestroyedCount ()
	{
		yield return new WaitForSecondsRealtime (1);

		if (gameControllerScript.BlocksDestroyed >= BlocksDestroyedToBonusRound) 
		{
			gameControllerScript.doBonusRound = true;
			TimesBlocksDestroyedToBonus++;

			TriggerAchievementNotification (
				"Achievement",
				BonusBlocksDestroyedTexture,
				"BLOCK DESTROYER",
				("Destroyed " + BlocksDestroyedToBonusRound + " or more blocks").ToString (),
				TimesBlocksDestroyedToBonus
			);

			BlocksDestroyedToBonusRound *= BlocksDestroyedToBonusRoundMultiplier;
		}

		StartCoroutine (CheckBlocksDestroyedCount ());
	}

	IEnumerator CheckPowerupsCollectedCount ()
	{
		yield return new WaitForSecondsRealtime (1);

		if (gameControllerScript.totalPowerupsCollected >= PowerupsCollectedToBonusRound) 
		{
			gameControllerScript.doBonusRound = true;
			TimesPowerupsCollectedToBonus++;

			TriggerAchievementNotification (
				"Achievement",
				BonusPowerupsCollectedTexture,
				"POWERUP COLLECTOR",
				("Collected " + PowerupsCollectedToBonusRound + " or more powerups").ToString (),
				TimesPowerupsCollectedToBonus
			);

			PowerupsCollectedToBonusRound *= PowerupsCollectedToBonusRoundMultiplier;
		}

		StartCoroutine (CheckPowerupsCollectedCount ());
	}

	IEnumerator CheckAbilityActivationsCount ()
	{
		yield return new WaitForSecondsRealtime (1);

		if (playerControllerScript.AbilityActivations >= AbilitiesActivatedToBonusRound) 
		{
			gameControllerScript.doBonusRound = true;
			TimesAbilitiesActivatedToBonus++;

			TriggerAchievementNotification (
				"Achievement",
				BonusAbilityActivationsTexture,
				"ABILITY ACTIVATOR",
				("Activated your ability " + AbilitiesActivatedToBonusRound + " or more times").ToString (),
				TimesAbilitiesActivatedToBonus
			);

			AbilitiesActivatedToBonusRound *= AbilitiesActivatedToBonusRoundMultiplier;
		}

		StartCoroutine (CheckAbilityActivationsCount ());
	}

	IEnumerator CheckWaveToBonusRoundCount ()
	{
		yield return new WaitForSecondsRealtime (1);

		if (gameControllerScript.Wave >= WaveToBonusRound) 
		{
			gameControllerScript.doBonusRound = true;
			TimesWaveToBonusRoundReached++;

			TriggerAchievementNotification (
				"Achievement",
				BonusWaveReachedTexture,
				"WAVE REACHER",
				("Reached wave " + gameControllerScript.Wave + "").ToString (),
				TimesWaveToBonusRoundReached
			);

			WaveToBonusRound += WaveToBonusRoundIncreaseAmount;
		}

		StartCoroutine (CheckWaveToBonusRoundCount ());
	}

	IEnumerator CheckNonShootingTimeCount ()
	{
		yield return new WaitForSecondsRealtime (1);

		if (playerControllerScript.NonShootingTime >= NextNonShootingTimeToBonus) 
		{
			gameControllerScript.doBonusRound = true;
			TimesNonShootingToBonus++;

			TriggerAchievementNotification (
				"Achievement",
				BonusNonShootingTimeTexture,
				"SURVIVOR",
				("Survived without shooting for " + NextNonShootingTimeToBonus + " seconds").ToString (),
				TimesNonShootingToBonus
			);

			NextNonShootingTimeToBonus += NonShootingTimeToBonusIncreaseAmount;
		}

		StartCoroutine (CheckNonShootingTimeCount ());
	}

	IEnumerator CheckRiskDistanceTimeCount ()
	{
		yield return new WaitForSecondsRealtime (1);

		if (playerControllerScript.RiskDistanceTime >= NextRiskDistanceTimeToBonus) 
		{
			gameControllerScript.doBonusRound = true;
			TimesRiskDistanceToBonus++;

			TriggerAchievementNotification (
				"Achievement",
				BonusRiskDistanceTimeTexture,
				"GOT TO GO FAST",
				("Survived at maximum speed for " + NextRiskDistanceTimeToBonus + " seconds").ToString (),
				TimesRiskDistanceToBonus
			);

			NextRiskDistanceTimeToBonus *= NextRiskDistanceTimeToBonusMultiplier;
		}

		StartCoroutine (CheckRiskDistanceTimeCount ());
	}

	public void TriggerAchievementNotification 
		(string AnimationTriggerName, Texture AchievementTexture, string AchievementTitleName, string AchievementTextName, int suffix)
	{
		AchievementAnim.SetTrigger (AnimationTriggerName);
		AchievementImage.texture = AchievementTexture;
		AchievementTitle.text = AchievementTitleName;
		AchievementText.text = AchievementTextName;

		AchievementSound.Play ();
	}
}