using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.PostProcessing;

using System.Collections;
using TMPro;

public class SceneLoader : MonoBehaviour 
{
	public static SceneLoader Instance { get; private set; }

	public bool isLoading;
	public float delay; // How long before the actual loading of the next scene starts.
	public string SceneName; // The name of the scene that other scripts can modify. The next scene should loaded by this name.
	public float ProgressBarSmoothTime = 1;
	public PostProcessingBehaviour PostPorcessSceneLoader;
	private float SmoothProgress;

	[Header ("UI Elements")]
	public Canvas LevelLoadUICanvas;
	public TextMeshProUGUI LoadProgressText;
	public Animator SceneLoaderUI;
	public ParticleSystem[] LoadingParticles;

	private AsyncOperation async = null; // The async operation variable. 

	void Awake ()
	{
		Instance = this;
		// DontDestroyOnLoad (gameObject);
	}

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
		LoadProgressText.text = "0%";

		foreach (ParticleSystem loadParticle in LoadingParticles) 
		{
			loadParticle.Play ();
		}
	}

	IEnumerator LoadProgress ()
	{
		SmoothProgress = 0;
		LoadProgressText.text = "";
		SceneLoaderUI.gameObject.SetActive (true);
		SceneLoaderUI.Play ("SceneLoaderUIAppear");

		yield return new WaitForSecondsRealtime (delay);

		PostPorcessSceneLoader.enabled = true;
		async = SceneManager.LoadSceneAsync (SceneName, LoadSceneMode.Single);
		async.allowSceneActivation = false; // Prevents the loading scene from activating.

		while (!async.isDone) 
		{
			float asyncprogress = Mathf.Round (async.progress * 100 / 0.9f);
			AudioListener.volume -= 0.2f * Time.unscaledDeltaTime;

			// UI checks load progress and displays for the player.
			SmoothProgress = Mathf.Lerp (SmoothProgress, async.progress, ProgressBarSmoothTime * Time.unscaledDeltaTime);

			foreach (ParticleSystem loadParticle in LoadingParticles) 
			{
				var ParticleStartLifetimeMain = loadParticle.main;
				ParticleStartLifetimeMain.startLifetime = async.progress + 0.5f;
			}
				
			if (asyncprogress < 100) 
			{
				Debug.Log ("Scene load async progress: " + Mathf.Round ((async.progress * 100) / 0.9f) + "%");
			}

			// Somehow async operations load up to 90% before loading the next scene,
			// we have to compensate by adding 10% to the progress text.
			LoadProgressText.text = Mathf.Round ((SmoothProgress * 100) / 0.9f) + "%";

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