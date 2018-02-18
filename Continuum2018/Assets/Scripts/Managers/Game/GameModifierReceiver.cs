using UnityEngine;

public class GameModifierReceiver : MonoBehaviour 
{
	public GameModifierManager gameModifierManagerScript;

	// The below methods will be accessed by UI in the main menu.

	public void SetTutorialState (bool tutorial)
	{
		gameModifierManagerScript.Tutorial = tutorial;
		Debug.Log ("MODIFIER: Tutorial: " + tutorial.ToString ());
	}

	public void SetPowerupMode (int powerupMode)
	{
		gameModifierManagerScript.PowerupSpawn = (GameModifierManager.powerupSpawnMode)powerupMode;
		Debug.Log ("MODIFIER: Powerup spawning mode: " + gameModifierManagerScript.PowerupSpawn.ToString ());
	}

	public void SetBossSpawnMode (int bossMode)
	{
		gameModifierManagerScript.BossSpawn = (GameModifierManager.bossSpawnMode)bossMode;
		Debug.Log ("MODIFIER: Boss spawning mode: " + gameModifierManagerScript.BossSpawn.ToString ());
	}

	public void SetTimeIncreaseMode (int timeIncreaseId)
	{
		gameModifierManagerScript.TimeIncreaseMode = (GameModifierManager.timeIncreaseMode)timeIncreaseId;
		Debug.Log ("MODIFIER: Time increase mode: " + gameModifierManagerScript.TimeIncreaseMode.ToString ());
	}

	public void SetStartingLives (int startingLives)
	{
		gameModifierManagerScript.StartingLives = startingLives;
		Debug.Log ("MODIFIER: Starting lives: " + startingLives.ToString ());
	}

	public void SetAlwaysHoming (bool alwaysHoming)
	{
		gameModifierManagerScript.AlwaysHoming = alwaysHoming;
		Debug.Log ("MODIFIER: Always homing: " + alwaysHoming.ToString ());
	}

	public void SetAlwaysRicochet (bool alwaysRicochet)
	{
		gameModifierManagerScript.AlwaysRicochet = alwaysRicochet;
		Debug.Log ("MODIFIER: Always ricochet: " + alwaysRicochet.ToString ());
	}

	public void SetAlwaysRapidFire (bool alwaysRapidFire)
	{
		gameModifierManagerScript.AlwaysRapidfire = alwaysRapidFire;
		Debug.Log ("MODIFIER: Always rapidfire: " + alwaysRapidFire.ToString ());
	}

	public void SetAlwaysOverdrive (bool alwaysOverdrive)
	{
		gameModifierManagerScript.AlwaysOverdrive = alwaysOverdrive;
		Debug.Log ("MODIFIER: Always overdrive: " + alwaysOverdrive.ToString ());
	}

	public void SetTrialTime (float trialTime)
	{
		gameModifierManagerScript.TrialTime = trialTime;
	}

	public void SetBlockSpawnRate (int BlockSpawnMult)
	{
		gameModifierManagerScript.blockSpawnRateMultiplier = BlockSpawnMult;
		Debug.Log ("MODIFIER: Block spawn multiplier: " + BlockSpawnMult.ToString ());
	}
}