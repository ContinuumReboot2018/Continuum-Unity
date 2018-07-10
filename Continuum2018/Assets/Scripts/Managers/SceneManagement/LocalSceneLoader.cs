using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LocalSceneLoader : MonoBehaviour 
{
	public static LocalSceneLoader Instance { get; private set; }

	[Tooltip ("Scene is going to laod another scene.")]
	public bool SceneLoadCommit;
		
	void Awake ()
	{
		Instance = this;
	}

	public void LoadScene (string sceneName)
	{
		if (SceneLoadCommit == false)
		{
			SceneLoader.Instance.SceneName = sceneName;
			SceneLoadSequence ();
			//StartCoroutine (SceneLoadSequence ());

			if (sceneName == "menu") 
			{
				InitManager.Instance.LoadingMissionText.text = "MAIN MENU";
			}

			SceneLoadCommit = true;
		}

		return;
	}

	void SceneLoadSequence ()
	{
		SceneLoader.Instance.StartLoadSequence ();
	}

	/*
	IEnumerator SceneLoadSequence ()
	{
		SceneLoader.Instance.StartLoadSequence ();
		yield return null;
	}
	*/
}