using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneLoader : MonoBehaviour 
{
	public float delay;
	public LoadType sceneLoadType;
	public enum LoadType
	{
		Simple,
		Async
	}

	public string SceneName;
	public TextMeshProUGUI LoadProgressText;
	public Slider ProgressBarL;
	public Slider ProgressBarR;
	public float ProgressBarSmoothTime = 1;
	private AsyncOperation async = null;

	void Start () 
	{
		if (SceneManager.GetActiveScene ().name == "Init") 
		{
			LoadProgressText.text = "0%";
			Invoke ("StartLoadProgressCoroutine", 1);
		}
	}

	void StartLoadProgressCoroutine ()
	{
		StartCoroutine (LoadProgress ());
	}

	IEnumerator LoadProgress ()
	{
		float ListVol = 1;
		async = SceneManager.LoadSceneAsync (SceneName, LoadSceneMode.Single);
		async.allowSceneActivation = false;

		while (!async.isDone) 
		{
			ProgressBarL.value = Mathf.Lerp (ProgressBarL.value, async.progress, ProgressBarSmoothTime * Time.deltaTime);
			ProgressBarR.value = Mathf.Lerp (ProgressBarR.value, async.progress, ProgressBarSmoothTime * Time.deltaTime);
			LoadProgressText.text = Mathf.Round((ProgressBarL.value * 100) + 10) + "%";

			if (LoadProgressText.text == "100%") 
			{
				StartCoroutine (LoadThisScene ());
			}

			yield return null;
		}
	}

	IEnumerator LoadThisScene ()
	{
		yield return new WaitForSecondsRealtime (1);
		async.allowSceneActivation = true;
	}
}
