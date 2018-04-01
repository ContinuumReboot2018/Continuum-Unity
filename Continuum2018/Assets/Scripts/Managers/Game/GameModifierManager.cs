using UnityEngine;

[CreateAssetMenu(fileName = "New Game Modifier", menuName = "Game Modifier")]
public class GameModifierManager : ScriptableObject
{
	[Header ("Tutorial")]
	[Tooltip ("Tutorial mode.")]
	public bool Tutorial = false;

	[Header ("Powerups")]
	[Tooltip ("How powerups spawn.")]
	public powerupSpawnMode PowerupSpawn;
	public enum powerupSpawnMode
	{
		Normal = 0,
		Faster = 1,
		Slower = 2,
		Off = 3
	}

	[Header ("Bosses")]
	[Tooltip ("How bosses spawn.")]
	public bossSpawnMode BossSpawn;
	public enum bossSpawnMode
	{
		Normal = 0,
		BossesOnly = 1,
		NoBosses = 2
	}

	[Header ("Time")]
	[Tooltip ("How the speed of time increases.")]
	public timeIncreaseMode TimeIncreaseMode;
	public enum timeIncreaseMode 
	{
		Normal = 0,
		Slow = 1,
		Fast = 2,
		Off = 3
	}

	[Tooltip ("How long for until the game invokes a game over.")]
	public float TrialTime = -1;

	[Header ("Shooting modifiers")]
	[Tooltip ("Homing powerup stays on if enabled.")]
	public bool AlwaysHoming = false;
	[Tooltip ("Ricochet powerup stays on if enabled.")]
	public bool AlwaysRicochet = false;
	[Tooltip ("Rapidfire powerup stays on if enabled.")]
	public bool AlwaysRapidfire = false;
	[Tooltip ("Overdrive powerup stays on if enabled.")]
	public bool AlwaysOverdrive = false;

	[Header ("Other modifiers")]
	[Tooltip ("Lives to start with when game loads.")]
	public int StartingLives = 3;
	[Tooltip ("How fast blocks spawn every wave increasing by.")]
	public float blockSpawnRateMultiplier = 1;
	[Tooltip ("Allow blocks to stack?")]
	public bool stacking = true;
	[Tooltip ("Use overheating?")]
	public bool useOverheat = false;
}