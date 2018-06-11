using UnityEngine;
using UnityEngine.UI;
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
	public AudioSource AchievementSound;

	[Header ("Achievement requirements")]
	public int ScoreToBonusRound = 2000;
	public int ScoreToBonusRoundMultiplier = 10;
	private int TimesScoredToBonus;
	[Space (10)]
	public int ComboToBonusRound = 20000;
	public int ComboToBonusRoundMultiplier = 5;
	private int TimesComboToBonus;

	void Start ()
	{
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();
		InvokeRepeating ("CheckScoreCount", 0, 0.5f);
		InvokeRepeating ("CheckComboCount", 0, 1f);
	}

	void CheckScoreCount ()
	{
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
	}

	void CheckComboCount ()
	{
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