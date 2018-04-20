using UnityEngine;

[System.Serializable]
public class LeaderboardEntry
{
	public string name;
	public int score;
	public int wave;

	public LeaderboardEntry (string _name, int _score, int _wave)
	{
		name = _name;
		score = _score;
		wave = _wave;
	}
}