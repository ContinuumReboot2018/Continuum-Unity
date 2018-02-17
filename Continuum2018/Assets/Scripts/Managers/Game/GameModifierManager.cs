using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Modifier", menuName = "Game Modifier")]
public class GameModifierManager : ScriptableObject
{
	//public SaveAndLoadScript saveAndLoadScript;
	//public GameModifierReceiver gameModifierReceiverScript;

	public bool Tutorial = false;

	public powerupSpawnMode PowerupSpawn;
	public enum powerupSpawnMode
	{
		Normal = 0,
		Faster = 1,
		Off = 2
	}

	public bossSpawnMode BossSpawn;
	public enum bossSpawnMode
	{
		Normal = 0,
		BossesOnly = 1,
		NoBosses = 2
	}

	public int StartingLives = 3;
	public bool AlwaysRapidfire = false;
	public float TrialTime = -1;
}
