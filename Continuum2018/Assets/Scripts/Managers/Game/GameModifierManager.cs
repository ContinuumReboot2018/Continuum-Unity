using UnityEngine;

[CreateAssetMenu(fileName = "New Game Modifier", menuName = "Game Modifier")]
public class GameModifierManager : ScriptableObject
{
	[Header ("Tutorial")]
	public bool Tutorial = false;

	[Header ("Powerups")]
	public powerupSpawnMode PowerupSpawn;
	public enum powerupSpawnMode
	{
		Normal = 0,
		Faster = 1,
		Slower = 2,
		Off = 3
	}

	[Header ("Bosses")]
	public bossSpawnMode BossSpawn;
	public enum bossSpawnMode
	{
		Normal = 0,
		BossesOnly = 1,
		NoBosses = 2
	}

	[Header ("Time")]
	public timeIncreaseMode TimeIncreaseMode;
	public enum timeIncreaseMode 
	{
		Normal = 0,
		Slow = 1,
		Fast = 2,
		Off = 3
	}
	public float TrialTime = -1;

	[Header ("Shooting modifiers")]
	public bool AlwaysHoming = false;
	public bool AlwaysRicochet = false;
	public bool AlwaysRapidfire = false;
	public bool AlwaysOverdrive = false;

	[Header ("Other modifiers")]
	public int StartingLives = 3;
	public float blockSpawnRateMultiplier = 1;
}
