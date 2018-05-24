using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingScreenTextUpdate : MonoBehaviour
{
	public InitManager initManager;
	public TextMeshProUGUI loadingScreenText;

	void Start ()
	{
		if (loadingScreenText == null) 
		{
			initManager = GameObject.Find ("MANAGERS").GetComponent<InitManager> ();
			loadingScreenText = initManager.LoadingMissionText;
		}
	}

	public void UpdateMissionText (string missionText)
	{
		if (SceneManager.GetActiveScene ().name == "menu")
		{
			loadingScreenText.text = missionText;
		} 

		else 
		
		{
			loadingScreenText.text = "MAIN MENU";
		}
	}
}