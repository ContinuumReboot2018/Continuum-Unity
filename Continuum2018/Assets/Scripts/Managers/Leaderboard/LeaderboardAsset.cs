using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Leaderboard Asset", menuName = "Leaderboard/Leaderboard Asset", order = 3)]
public class LeaderboardAsset : ScriptableObject
{
	public List<LeaderboardEntry> leaderboard;
}