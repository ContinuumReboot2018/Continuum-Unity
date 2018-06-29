using UnityEngine;
using UnityEngine.PostProcessing;
using TMPro;

public class InitManager : MonoBehaviour 
{
	public SaveAndLoadScript saveAndLoadScript;
	public PostProcessingBehaviour postProcess;
	public TextMeshProUGUI LoadingMissionText;

	void Start ()
	{
		LoadingMissionText.text = "MAIN MENU";

		saveAndLoadScript.LoadSettingsData ();
		saveAndLoadScript.SaveSettingsData ();
		saveAndLoadScript.LoadPlayerData ();

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
}