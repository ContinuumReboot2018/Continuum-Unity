using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LeaderboardDisplay : MonoBehaviour 
{
	private SaveAndLoadScript saveAndLoadScript;

	public TextMeshProUGUI[] Names;
	public TextMeshProUGUI[] Scores;
	public TextMeshProUGUI[] Waves;

	private int menuLeaderboardId;

	void Start ()
	{
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();

		if (SceneManager.GetActiveScene ().name != "Menu")
		{
			UpdateLeaderboard ();
		}
	}

	void UpdateLeaderboardDelayed ()
	{
		switch (menuLeaderboardId)
		{
		case 0:
			for (int i = 0; i < saveAndLoadScript.Leaderboard_ArcadeMode.Count; i++) 
			{
				Names [i].text = saveAndLoadScript.Leaderboard_ArcadeMode [i].name.ToString ();
				Scores [i].text = saveAndLoadScript.Leaderboard_ArcadeMode [i].score.ToString ();
				Waves [i].text = saveAndLoadScript.Leaderboard_ArcadeMode [i].wave.ToString ();
			}

			Debug.Log ("Opened arcade leaderboard display.");
			break;
		case 1:
			/*
			for (int i = 0; i < saveAndLoadScript.Leaderboard_ModsMode.Count; i++)
			{
				Names  [i].text = saveAndLoadScript.Leaderboard_ModsMode [i].score.ToString ();
				Waves  [i].text = saveAndLoadScript.Leaderboard_ModsMode [i].wave.ToString ();
			}
			*/
			break;
		case 2:
			for (int i = 0; i < saveAndLoadScript.Leaderboard_BossRushMode.Count; i++)
			{
				Names  [i].text = saveAndLoadScript.Leaderboard_BossRushMode [i].name.ToString ();
				Scores [i].text = saveAndLoadScript.Leaderboard_BossRushMode [i].score.ToString ();
				Waves  [i].text = saveAndLoadScript.Leaderboard_BossRushMode [i].wave.ToString ();
			}
			break;
		case 3:
			for (int i = 0; i < saveAndLoadScript.Leaderboard_LuckyMode.Count; i++)
			{
				Names  [i].text = saveAndLoadScript.Leaderboard_LuckyMode [i].name.ToString ();
				Scores [i].text = saveAndLoadScript.Leaderboard_LuckyMode [i].score.ToString ();
				Waves  [i].text = saveAndLoadScript.Leaderboard_LuckyMode [i].wave.ToString ();
			}
			break;
		case 4:
			for (int i = 0; i < saveAndLoadScript.Leaderboard_FullyLoadedMode.Count; i++)
			{
				Names  [i].text = saveAndLoadScript.Leaderboard_FullyLoadedMode [i].name.ToString ();
				Scores [i].text = saveAndLoadScript.Leaderboard_FullyLoadedMode [i].score.ToString ();
				Waves  [i].text = saveAndLoadScript.Leaderboard_FullyLoadedMode [i].wave.ToString ();
			}
			break;
		case 5:
			for (int i = 0; i < saveAndLoadScript.Leaderboard_ScavengerMode.Count; i++)
			{
				Names  [i].text = saveAndLoadScript.Leaderboard_ScavengerMode [i].name.ToString ();
				Scores [i].text = saveAndLoadScript.Leaderboard_ScavengerMode [i].score.ToString ();
				Waves  [i].text = saveAndLoadScript.Leaderboard_ScavengerMode [i].wave.ToString ();
			}
			break;
		case 6:
			for (int i = 0; i < saveAndLoadScript.Leaderboard_HellMode.Count; i++)
			{
				Names  [i].text = saveAndLoadScript.Leaderboard_HellMode [i].name.ToString ();
				Scores [i].text = saveAndLoadScript.Leaderboard_HellMode [i].score.ToString ();
				Waves  [i].text = saveAndLoadScript.Leaderboard_HellMode [i].wave.ToString ();
			}
			break;
		case 7:
			for (int i = 0; i < saveAndLoadScript.Leaderboard_FastTrackMode.Count; i++)
			{
				Names  [i].text = saveAndLoadScript.Leaderboard_FastTrackMode [i].name.ToString ();
				Scores [i].text = saveAndLoadScript.Leaderboard_FastTrackMode [i].score.ToString ();
				Waves  [i].text = saveAndLoadScript.Leaderboard_FastTrackMode [i].wave.ToString ();
			}
			break;
		}
	}

	public void UpdateLeaderboardMenu (int leaderboardMenu)
	{
		menuLeaderboardId = leaderboardMenu;

		Invoke ("UpdateLeaderboardDelayed", 0.03f);
	}

	public void UpdateLeaderboard ()
	{
		switch (saveAndLoadScript.MissionId) 
		{
		case 0:
			for (int i = 0; i < saveAndLoadScript.Leaderboard_ArcadeMode.Count; i++)
			{
				Names  [i].text = saveAndLoadScript.Leaderboard_ArcadeMode [i].name.ToString ();
				Scores [i].text = saveAndLoadScript.Leaderboard_ArcadeMode [i].score.ToString ();
				Waves  [i].text = saveAndLoadScript.Leaderboard_ArcadeMode [i].wave.ToString ();
			}
			break;
		case 1:
			/*
			for (int i = 0; i < saveAndLoadScript.Leaderboard_ModsMode.Count; i++)
			{
				Names  [i].text = saveAndLoadScript.Leaderboard_ModsMode [i].score.ToString ();
				Waves  [i].text = saveAndLoadScript.Leaderboard_ModsMode [i].wave.ToString ();
			}
			*/
			break;
		case 2:
			for (int i = 0; i < saveAndLoadScript.Leaderboard_BossRushMode.Count; i++)
			{
				Names  [i].text = saveAndLoadScript.Leaderboard_BossRushMode [i].name.ToString ();
				Scores [i].text = saveAndLoadScript.Leaderboard_BossRushMode [i].score.ToString ();
				Waves  [i].text = saveAndLoadScript.Leaderboard_BossRushMode [i].wave.ToString ();
			}
			break;
		case 3:
			for (int i = 0; i < saveAndLoadScript.Leaderboard_LuckyMode.Count; i++)
			{
				Names  [i].text = saveAndLoadScript.Leaderboard_LuckyMode [i].name.ToString ();
				Scores [i].text = saveAndLoadScript.Leaderboard_LuckyMode [i].score.ToString ();
				Waves  [i].text = saveAndLoadScript.Leaderboard_LuckyMode [i].wave.ToString ();
			}
			break;
		case 4:
			for (int i = 0; i < saveAndLoadScript.Leaderboard_FullyLoadedMode.Count; i++)
			{
				Names  [i].text = saveAndLoadScript.Leaderboard_FullyLoadedMode [i].name.ToString ();
				Scores [i].text = saveAndLoadScript.Leaderboard_FullyLoadedMode [i].score.ToString ();
				Waves  [i].text = saveAndLoadScript.Leaderboard_FullyLoadedMode [i].wave.ToString ();
			}
			break;
		case 5:
			for (int i = 0; i < saveAndLoadScript.Leaderboard_ScavengerMode.Count; i++)
			{
				Names  [i].text = saveAndLoadScript.Leaderboard_ScavengerMode [i].name.ToString ();
				Scores [i].text = saveAndLoadScript.Leaderboard_ScavengerMode [i].score.ToString ();
				Waves  [i].text = saveAndLoadScript.Leaderboard_ScavengerMode [i].wave.ToString ();
			}
			break;
		case 6:
			for (int i = 0; i < saveAndLoadScript.Leaderboard_HellMode.Count; i++)
			{
				Names  [i].text = saveAndLoadScript.Leaderboard_HellMode [i].name.ToString ();
				Scores [i].text = saveAndLoadScript.Leaderboard_HellMode [i].score.ToString ();
				Waves  [i].text = saveAndLoadScript.Leaderboard_HellMode [i].wave.ToString ();
			}
			break;
		case 7:
			for (int i = 0; i < saveAndLoadScript.Leaderboard_FastTrackMode.Count; i++)
			{
				Names  [i].text = saveAndLoadScript.Leaderboard_FastTrackMode [i].name.ToString ();
				Scores [i].text = saveAndLoadScript.Leaderboard_FastTrackMode [i].score.ToString ();
				Waves  [i].text = saveAndLoadScript.Leaderboard_FastTrackMode [i].wave.ToString ();
			}
			break;
		}
	}
}