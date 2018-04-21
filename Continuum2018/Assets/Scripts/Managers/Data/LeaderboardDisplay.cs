using UnityEngine;
using TMPro;

public class LeaderboardDisplay : MonoBehaviour 
{
	private SaveAndLoadScript saveAndLoadScript;

	public TextMeshProUGUI[] Names;
	public TextMeshProUGUI[] Scores;
	public TextMeshProUGUI[] Waves;

	void Start ()
	{
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();
		//InvokeRepeating ("UpdateLeaderboard", 0, 0.5f);
		UpdateLeaderboard ();
	}

	public void UpdateLeaderboard ()
	{
		for (int i = 0; i < saveAndLoadScript.Leaderboard.Count; i++)
		{
			Names  [i].text = saveAndLoadScript.Leaderboard [i].name.ToString ();
			Scores [i].text = saveAndLoadScript.Leaderboard [i].score.ToString ();
			Waves  [i].text = saveAndLoadScript.Leaderboard [i].wave.ToString ();
		}
	}
}