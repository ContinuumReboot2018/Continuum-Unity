using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class AchievementManager : MonoBehaviour 
{
	public static AchievementManager Instance { get; private set; }

	public PlayerController playerControllerScript;

	[Header ("Achievement notification")]
	public Animator AchievementAnim;
	public RawImage AchievementImage;
	public TextMeshProUGUI AchievementTitle;
	public TextMeshProUGUI AchievementText;
	public AudioSource AchievementSound;

	[Header ("Achievement assets")]
	public Texture BonusScoreTexture;
	public Texture BonusComboTexture;
	public Texture BonusBlocksDestroyedTexture;
	public Texture BonusPowerupsCollectedTexture;
	public Texture BonusAbilityActivationsTexture;
	public Texture BonusWaveReachedTexture;
	public Texture BonusNonShootingTimeTexture;
	public Texture BonusRiskDistanceTimeTexture;
	public Texture BonusPowerupsInUseTexture;

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
	[Space (10)]
	public int PowerupsInUseToBonus = 4;
	public int PowerupsInUseIncreaseAmount = 2;
	private int TimesReachedPowerupsInUseToBonus;

	void Awake ()
	{
		Instance = this;
		// DontDestroyOnLoad (gameObject);
	}

	void Start ()
	{
		StartAchievementChecking ();
	}

	void StartAchievementChecking ()
	{
		StartCoroutine (CheckScoreCount ());
		StartCoroutine (CheckComboCount ());
		StartCoroutine (CheckBlocksDestroyedCount ());
		StartCoroutine (CheckPowerupsCollectedCount ());
		StartCoroutine (CheckAbilityActivationsCount ());
		StartCoroutine (CheckWaveToBonusRoundCount ());
		StartCoroutine (CheckNonShootingTimeCount ());
		StartCoroutine (CheckRiskDistanceTimeCount ());
		StartCoroutine (CheckPowerupsInUseCount ());
	}

	IEnumerator CheckScoreCount ()
	{
		yield return new WaitForSecondsRealtime (1);

		if (GameController.Instance.DisplayScore >= ScoreToBonusRound) 
		{
			GameController.Instance.doBonusRound = true;
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

		if (GameController.Instance.combo >= ComboToBonusRound) 
		{
			GameController.Instance.doBonusRound = true;
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

		if (GameController.Instance.BlocksDestroyed >= BlocksDestroyedToBonusRound) 
		{
			GameController.Instance.doBonusRound = true;
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

		if (GameController.Instance.totalPowerupsCollected >= PowerupsCollectedToBonusRound) 
		{
			GameController.Instance.doBonusRound = true;
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
			GameController.Instance.doBonusRound = true;
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

		if (GameController.Instance.Wave >= WaveToBonusRound) 
		{
			GameController.Instance.doBonusRound = true;
			TimesWaveToBonusRoundReached++;

			TriggerAchievementNotification (
				"Achievement",
				BonusWaveReachedTexture,
				"WAVE REACHER",
				("Reached wave " + GameController.Instance.Wave + "").ToString (),
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
			GameController.Instance.doBonusRound = true;
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
			GameController.Instance.doBonusRound = true;
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

	IEnumerator CheckPowerupsInUseCount ()
	{
		yield return new WaitForSecondsRealtime (1);

		if (playerControllerScript.powerupsInUse >= PowerupsInUseToBonus) 
		{
			GameController.Instance.doBonusRound = true;
			TimesReachedPowerupsInUseToBonus++;

			TriggerAchievementNotification (
				"Achievement",
				BonusPowerupsInUseTexture,
				"FEEL THE POWER",
				("Collected " + PowerupsInUseToBonus + " powerups before timer runs out").ToString (),
				TimesReachedPowerupsInUseToBonus
			);

			PowerupsInUseToBonus += PowerupsInUseIncreaseAmount;
		}

		StartCoroutine (CheckPowerupsInUseCount ());
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