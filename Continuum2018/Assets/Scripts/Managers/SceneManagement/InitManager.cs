using UnityEngine;
using UnityEngine.PostProcessing;
using TMPro;

public class InitManager : MonoBehaviour 
{
	public static InitManager Instance { get; private set; }

	public PostProcessingBehaviour postProcess;
	public TextMeshProUGUI LoadingMissionText;

	void Awake ()
	{
		Instance = this;
		// DontDestroyOnLoad (gameObject);
	}

	void Start ()
	{
		LoadingMissionText.text = "MAIN MENU";

		SaveAndLoadScript.Instance.LoadSettingsData ();
		SaveAndLoadScript.Instance.SaveSettingsData ();
		SaveAndLoadScript.Instance.LoadPlayerData ();

		CheckPostProcessQuality ();
	}

	void CheckPostProcessQuality ()
	{
		int qualLevel = QualitySettings.GetQualityLevel ();

		if (qualLevel == 0) 
		{
			postProcess.enabled = false;
		}

		if (qualLevel == 1) 
		{
			postProcess.enabled = true;
		}
	}


	void OnApplicationQuit ()
	{
		Debug.Log ("Application ended after " + Time.unscaledTime + " seconds.");
	}
}