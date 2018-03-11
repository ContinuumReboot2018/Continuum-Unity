using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PostProcessing;

public class SceneLoader : MonoBehaviour 
{
	public bool isLoading;
	public float delay; // How long before the actual loading of the next scene starts.
	public string SceneName; // The name of the scene that other scripts can modify. The next scene should loaded by this name.
	public float ProgressBarSmoothTime = 1;
	public PostProcessingBehaviour PostPorcessSceneLoader;

	[Header ("UI Elements")]
	public Canvas LevelLoadUICanvas;
	public TextMeshProUGUI LoadProgressText;
	public Slider ProgressBarL;
	public Slider ProgressBarR;
	public Animator SceneLoaderUI;
	public ParticleSystem[] LoadingParticles;

	private AsyncOperation async = null; // The async operation variable. 

	void Start () 
	{
		// Checks if the currently loaded scene is the init scene.
		if (SceneManager.GetActiveScene ().name == "Init") 
		{
			StartLoadSequence ();
		}
	}

	public void StartLoadSequence ()
	{
		isLoading = true;
		StartCoroutine (LoadProgress ());

		// Resets all UI fill and text values.
		ProgressBarL.value = 0;
		ProgressBarR.value = 0;
		LoadProgressText.text = "0%";

		foreach (ParticleSystem loadParticle in LoadingParticles) 
		{
			loadParticle.Play ();
		}
	}

	IEnumerator LoadProgress ()
	{
		SceneLoaderUI.gameObject.SetActive (true);
		SceneLoaderUI.Play ("SceneLoaderUIAppear");
		yield return new WaitForSecondsRealtime (delay);
		PostPorcessSceneLoader.enabled = true;
		async = SceneManager.LoadSceneAsync (SceneName, LoadSceneMode.Single);
		async.allowSceneActivation = false; // Prevents the loading scene from activating.

		while (!async.isDone) 
		{
			// UI checks load progress and displays for the player.
			ProgressBarL.value = Mathf.Lerp (ProgressBarL.value, async.progress, ProgressBarSmoothTime * Time.unscaledDeltaTime);
			ProgressBarR.value = Mathf.Lerp (ProgressBarR.value, async.progress, ProgressBarSmoothTime * Time.unscaledDeltaTime);

			//Debug.Log ("Scene load async progress: " + Mathf.Round((ProgressBarL.value * 100) / 0.9f) + "%");

			// Somehow async operations load up to 90% before loading the next scene,
			// we have to compensate by adding 10% to the progress text.
			LoadProgressText.text = Mathf.Round((ProgressBarL.value * 100) / 0.9f) + "%";

			// Checks if the scene has been completely loaded into memory. 
			if (LoadProgressText.text == "100%") 
			{
				StartCoroutine (LoadThisScene ());
				SceneLoaderUI.Play ("SceneLoaderUIDisappear");
			}

			yield return null;
		}
	}

	IEnumerator LoadThisScene ()
	{
		LoadProgressText.text = "";

		foreach (ParticleSystem loadParticle in LoadingParticles) 
		{
			loadParticle.Stop (true, ParticleSystemStopBehavior.StopEmitting);
		}

		// The short delay is to leave the progress text visible at 100% for longer.
		yield return new WaitForSecondsRealtime (1);
		isLoading = false;
		PostPorcessSceneLoader.enabled = false;
		// Finally, we can activate the newly loaded scene.
		async.allowSceneActivation = true;
	}
}