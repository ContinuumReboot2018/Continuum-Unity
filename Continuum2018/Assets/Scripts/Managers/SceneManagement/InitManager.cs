using UnityEngine;
using UnityEngine.PostProcessing;

public class InitManager : MonoBehaviour 
{
	public SaveAndLoadScript saveAndLoadScript;

	public PostProcessingBehaviour postProcess;

	void Start ()
	{
		if (saveAndLoadScript.QualitySettingsIndex == 0) 
		{
			postProcess.enabled = false;
		}

		if (saveAndLoadScript.QualitySettingsIndex == 1) 
		{
			postProcess.enabled = true;
		}
	}
}
