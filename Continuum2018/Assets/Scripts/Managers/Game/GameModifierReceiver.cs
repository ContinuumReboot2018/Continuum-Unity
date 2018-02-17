using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModifierReceiver : MonoBehaviour 
{
	public GameModifierManager gameModifierManagerScript;

	public void Start ()
	{
		//gameModifierManagerScript = GameObject.Find ("GameModifierManager").GetComponent<GameModifierManager> ();
		//gameModifierManagerScript.gameModifierReceiverScript = this;
	}

	// The below methods will be accessed by UI in the main menu.

	public void SetTutorialState (bool tutorial)
	{
		gameModifierManagerScript.Tutorial = tutorial;
	}

	public void SetPowerupMode (int powerupMode)
	{
		gameModifierManagerScript.PowerupSpawn = (GameModifierManager.powerupSpawnMode)powerupMode;
	}

	public void SetBossSpawnMode (int bossMode)
	{
		gameModifierManagerScript.BossSpawn = (GameModifierManager.bossSpawnMode)bossMode;
	}

	public void SetStartingLives (int startingLives)
	{
		gameModifierManagerScript.StartingLives = startingLives;
	}

	public void SetAlwaysRapidFire (bool alwaysRapidFire)
	{
		gameModifierManagerScript.AlwaysRapidfire = alwaysRapidFire;
	}

	public void SetTrialTime (float trialTime)
	{
		gameModifierManagerScript.TrialTime = trialTime;
	}
}