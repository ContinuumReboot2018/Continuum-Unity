﻿using UnityEngine;
using TMPro;

public class GameModifierReceiver : MonoBehaviour 
{
	public static GameModifierReceiver Instance;

	public GameModifierManager gameModifierManagerScript;
	public GameModifierManager loadedModifierManagerScript;
	public GameModifierManager[] gameModifiers;
	public TextMeshProUGUI missionText;

	[Space (10)]
	public TextMeshProUGUI TutorialStatusText;
	public TextMeshProUGUI PowerupModeStatusText;
	public TextMeshProUGUI BossModeStatusText;
	public TextMeshProUGUI TimeIncreaseModeStatusText;
	public TextMeshProUGUI AlwaysHomingStatusText;
	public TextMeshProUGUI AlwaysRicochetStatusText;
	public TextMeshProUGUI AlwaysRapidfireStatusText;
	public TextMeshProUGUI AlwaysOverdriveStatusText;
	public TextMeshProUGUI StartingLivesStatusText;
	public TextMeshProUGUI BlockSpawnRateStatusText;
	public TextMeshProUGUI StackingStatusText;
	public TextMeshProUGUI OverheatStatusText;
	public TextMeshProUGUI BonusRoundsStatusText;
	public TextMeshProUGUI StartingWaveStatusText;

	void Awake ()
	{
		Instance = this;
	}

	void Start ()
	{
		loadedModifierManagerScript = gameModifiers [SaveAndLoadScript.Instance.MissionId];
		CheckModifierStates ();
		missionText.text = "";
	}

	public void SetMultiplayerModeModifier (bool Multiplayer)
	{
		loadedModifierManagerScript.Multiplayer = Multiplayer;
	}

	// The below methods will be accessed by UI in the main menu.

	public void SetTutorialState ()
	{
		gameModifierManagerScript.Tutorial = !gameModifierManagerScript.Tutorial;
		Debug.Log ("MODIFIER: Tutorial: " + gameModifierManagerScript.Tutorial.ToString ());
	}

	public void SetPowerupMode ()
	{
		if (gameModifierManagerScript.PowerupSpawn < (GameModifierManager.powerupSpawnMode)3) 
		{
			gameModifierManagerScript.PowerupSpawn += 1;
		} 

		else 
		
		{
			gameModifierManagerScript.PowerupSpawn = (GameModifierManager.powerupSpawnMode)0;
		}

		Debug.Log ("MODIFIER: Powerup spawning speed: " + gameModifierManagerScript.PowerupSpawn.ToString ());
	}

	public void SetPowerupModeDown ()
	{
		if (gameModifierManagerScript.PowerupSpawn > (GameModifierManager.powerupSpawnMode)1) 
		{
			gameModifierManagerScript.PowerupSpawn -= 1;
		} 

		else 

		{
			gameModifierManagerScript.PowerupSpawn = (GameModifierManager.powerupSpawnMode)3;
		}

		Debug.Log ("MODIFIER: Powerup spawning speed: " + gameModifierManagerScript.PowerupSpawn.ToString ());
	}

	public void SetBossSpawnMode ()
	{
		if (gameModifierManagerScript.BossSpawn < (GameModifierManager.bossSpawnMode)2) 
		{
			gameModifierManagerScript.BossSpawn += 1;
		} 

		else 
		
		{
			gameModifierManagerScript.BossSpawn = (GameModifierManager.bossSpawnMode)0;
		}

		Debug.Log ("MODIFIER: Boss spawning mode: " + gameModifierManagerScript.BossSpawn.ToString ());
	}

	public void SetBossSpawnModeDown ()
	{
		if (gameModifierManagerScript.BossSpawn > (GameModifierManager.bossSpawnMode)1) 
		{
			gameModifierManagerScript.BossSpawn -= 1;
		} 

		else 

		{
			gameModifierManagerScript.BossSpawn = (GameModifierManager.bossSpawnMode)3;
		}

		Debug.Log ("MODIFIER: Boss spawning mode: " + gameModifierManagerScript.BossSpawn.ToString ());
	}

	public void SetTimeIncreaseMode ()
	{
		if (gameModifierManagerScript.TimeIncreaseMode < (GameModifierManager.timeIncreaseMode)3) 
		{
			gameModifierManagerScript.TimeIncreaseMode += 1;
		} 

		else 

		{
			gameModifierManagerScript.TimeIncreaseMode = (GameModifierManager.timeIncreaseMode)0;
		}
			
		Debug.Log ("MODIFIER: Timescale increase speed: " + gameModifierManagerScript.TimeIncreaseMode.ToString ());
	}

	public void SetTimeIncreaseModeDown ()
	{
		if (gameModifierManagerScript.TimeIncreaseMode > (GameModifierManager.timeIncreaseMode)1) 
		{
			gameModifierManagerScript.TimeIncreaseMode -= 1;
		} 

		else 

		{
			gameModifierManagerScript.TimeIncreaseMode = (GameModifierManager.timeIncreaseMode)3;
		}

		Debug.Log ("MODIFIER: Timescale increase speed: " + gameModifierManagerScript.TimeIncreaseMode.ToString ());
	}

	public void SetAlwaysHoming ()
	{
		gameModifierManagerScript.AlwaysHoming = !gameModifierManagerScript.AlwaysHoming;
		Debug.Log ("MODIFIER: Always homing: " + gameModifierManagerScript.AlwaysHoming.ToString ());
	}

	public void SetAlwaysRicochet ()
	{
		gameModifierManagerScript.AlwaysRicochet = !gameModifierManagerScript.AlwaysRicochet;
		Debug.Log ("MODIFIER: Always ricochet: " + gameModifierManagerScript.AlwaysRicochet.ToString ());
	}

	public void SetAlwaysRapidFire ()
	{
		gameModifierManagerScript.AlwaysRapidfire = !gameModifierManagerScript.AlwaysRapidfire;
		Debug.Log ("MODIFIER: Always rapidfire: " + gameModifierManagerScript.AlwaysRapidfire.ToString ());
	}

	public void SetAlwaysOverdrive ()
	{
		gameModifierManagerScript.AlwaysOverdrive = !gameModifierManagerScript.AlwaysOverdrive;
		Debug.Log ("MODIFIER: Always overdrive: " + gameModifierManagerScript.AlwaysOverdrive.ToString ());
	}

	public void SetStartingLives ()
	{
		if (gameModifierManagerScript.StartingLives < 10)
		{
			gameModifierManagerScript.StartingLives += 1;
		} 

		else 
		
		{
			gameModifierManagerScript.StartingLives = 1;
		}

		Debug.Log ("MODIFIER: Starting lives: " + gameModifierManagerScript.StartingLives.ToString () + " LIVES");
	}

	public void SetStartingLivesDown ()
	{
		if (gameModifierManagerScript.StartingLives > 1)
		{
			gameModifierManagerScript.StartingLives -= 1;
		} 

		else 

		{
			gameModifierManagerScript.StartingLives = 10;
		}

		Debug.Log ("MODIFIER: Starting lives: " + gameModifierManagerScript.StartingLives.ToString () + " LIVES");
	}

	public void SetBlockSpawnRate ()
	{
		if (gameModifierManagerScript.blockSpawnRateMultiplier < 10) 
		{
			gameModifierManagerScript.blockSpawnRateMultiplier += 1;
		} 

		else 
		
		{
			gameModifierManagerScript.blockSpawnRateMultiplier = 1;
		}

		Debug.Log ("MODIFIER: Block spawn rate multiplier: " + gameModifierManagerScript.blockSpawnRateMultiplier.ToString ());
	}

	public void SetBlockSpawnRateDown ()
	{
		if (gameModifierManagerScript.blockSpawnRateMultiplier > 1) 
		{
			gameModifierManagerScript.blockSpawnRateMultiplier -= 1;
		} 

		else 

		{
			gameModifierManagerScript.blockSpawnRateMultiplier = 10;
		}

		Debug.Log ("MODIFIER: Block spawn rate multiplier: " + gameModifierManagerScript.blockSpawnRateMultiplier.ToString ());
	}


	public void SetStackingMode ()
	{
		gameModifierManagerScript.stacking = !gameModifierManagerScript.stacking;
		Debug.Log ("MODIFIER: Stacking: " + gameModifierManagerScript.stacking.ToString ());
	}

	public void SetOverheatMode ()
	{
		gameModifierManagerScript.useOverheat = !gameModifierManagerScript.useOverheat;
		Debug.Log ("MODIFIER: Use overheat: " + gameModifierManagerScript.useOverheat.ToString ());
	}

	public void SetBonusRoundsMode ()
	{
		gameModifierManagerScript.bonusRounds = !gameModifierManagerScript.bonusRounds;
		Debug.Log ("MODIFIER: Bonus rounds: " + gameModifierManagerScript.bonusRounds.ToString ());
	}

	public void SetStartingWave ()
	{
		if (gameModifierManagerScript.startingWave < 10) 
		{
			gameModifierManagerScript.startingWave += 1;
		} 

		else 

		{
			gameModifierManagerScript.startingWave = 1;
		}

		Debug.Log ("MODIFIER: Starting wave: " + gameModifierManagerScript.startingWave.ToString ());
	}

	public void SetStartingWaveDown ()
	{
		if (gameModifierManagerScript.startingWave > 1) 
		{
			gameModifierManagerScript.startingWave -= 1;
		} 

		else 

		{
			gameModifierManagerScript.startingWave = 10;
		}

		Debug.Log ("MODIFIER: Starting wave: " + gameModifierManagerScript.startingWave.ToString ());
	}

	public void SetMissionId (int mission)
	{
		SaveAndLoadScript.Instance.MissionId = mission;
		Debug.Log ("MODIFIER: Set mission ID to: " + SaveAndLoadScript.Instance.MissionId);
		SaveAndLoadScript.Instance.SavePlayerData ();
	}

	public void SetTrialTime (float trialTime)
	{
		gameModifierManagerScript.TrialTime = trialTime;
	}

	public void UpdateMissionText (GameModifierManager missionRules)
	{
		missionText.text = missionRules.MissionDescription;
	}
		
	public void CheckModifierStates ()
	{
		TutorialStatusText.text = gameModifierManagerScript.Tutorial ? "ON" : "OFF";

		switch (gameModifierManagerScript.PowerupSpawn) 
		{
		case GameModifierManager.powerupSpawnMode.Normal:
			PowerupModeStatusText.text = "NORMAL";
			break;
		case GameModifierManager.powerupSpawnMode.Faster:
			PowerupModeStatusText.text = "FASTER";
			break;
		case GameModifierManager.powerupSpawnMode.Slower:
			PowerupModeStatusText.text = "SLOWER";
			break;
		case GameModifierManager.powerupSpawnMode.Off:
			PowerupModeStatusText.text = "OFF";
			break;
		}

		switch (gameModifierManagerScript.BossSpawn) 
		{
		case GameModifierManager.bossSpawnMode.Normal:
			BossModeStatusText.text = "NORMAL";
			break;
		case GameModifierManager.bossSpawnMode.BossesOnly:
			BossModeStatusText.text = "BOSS RUSH";
			break;
		case GameModifierManager.bossSpawnMode.NoBosses:
			BossModeStatusText.text = "NO BOSSES";
			break;
		}
			
		switch (gameModifierManagerScript.TimeIncreaseMode) 
		{
		case GameModifierManager.timeIncreaseMode.Normal:
			TimeIncreaseModeStatusText.text = "NORMAL";
			break;
		case GameModifierManager.timeIncreaseMode.Slow:
			TimeIncreaseModeStatusText.text = "SLOW";
			break;
		case GameModifierManager.timeIncreaseMode.Fast:
			TimeIncreaseModeStatusText.text = "FAST";
			break;
		case GameModifierManager.timeIncreaseMode.Off:
			TimeIncreaseModeStatusText.text = "OFF";
			break;
		}

		AlwaysHomingStatusText.text = gameModifierManagerScript.AlwaysHoming ? "ON" : "OFF";
		AlwaysRicochetStatusText.text = gameModifierManagerScript.AlwaysRicochet ? "ON" : "OFF";
		AlwaysRapidfireStatusText.text = gameModifierManagerScript.AlwaysRapidfire ? "ON" : "OFF";
		AlwaysOverdriveStatusText.text = gameModifierManagerScript.AlwaysOverdrive ? "ON" : "OFF";
		StartingLivesStatusText.text = gameModifierManagerScript.StartingLives.ToString ();
		BlockSpawnRateStatusText.text = "x" + gameModifierManagerScript.blockSpawnRateMultiplier.ToString ();
		StackingStatusText.text = gameModifierManagerScript.stacking ? "ON" : "OFF";
		OverheatStatusText.text = gameModifierManagerScript.useOverheat ? "ON" : "OFF";
		BonusRoundsStatusText.text = gameModifierManagerScript.bonusRounds ? "ON" : "OFF";
		StartingWaveStatusText.text = gameModifierManagerScript.startingWave.ToString ();
	}
}