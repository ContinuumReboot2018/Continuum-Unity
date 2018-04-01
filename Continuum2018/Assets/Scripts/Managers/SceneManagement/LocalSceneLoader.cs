using System.Collections;
using UnityEngine;

public class LocalSceneLoader : MonoBehaviour 
{
	[Tooltip ("Scene loader from Init.")]
	public SceneLoader sceneLoaderScript;
	[Tooltip ("Scene is going to laod another scene.")]
	public bool SceneLoadCommit;

	void Start ()
	{
		sceneLoaderScript = GameObject.Find ("SceneLoader").GetComponent<SceneLoader> ();
	}
		
	public void LoadScene (string sceneName)
	{
		SceneLoadCommit = true;
		sceneLoaderScript.SceneName = sceneName;
		StartCoroutine (SceneLoadSequence ());
	}

	IEnumerator SceneLoadSequence ()
	{
		yield return new WaitForSecondsRealtime (0.1f);
		sceneLoaderScript.StartLoadSequence ();
	}
}