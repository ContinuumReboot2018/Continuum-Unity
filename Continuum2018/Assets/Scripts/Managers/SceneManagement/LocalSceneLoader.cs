using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalSceneLoader : MonoBehaviour 
{
	public SceneLoader sceneLoaderScript;
	public bool SceneLoadCommit;

	void Start ()
	{
		sceneLoaderScript = GameObject.Find ("SceneLoader").GetComponent<SceneLoader> ();
	}
		
	public void LoadScene (string sceneName)
	{
		SceneLoadCommit = true;
		sceneLoaderScript.SceneName = sceneName;
		//sceneLoaderScript.Invoke ("StartLoadSequence", 1);
		StartCoroutine (SceneLoadSequence ());
	}

	IEnumerator SceneLoadSequence ()
	{
		yield return new WaitForSecondsRealtime (0.1f);
		sceneLoaderScript.StartLoadSequence ();
	}
}
