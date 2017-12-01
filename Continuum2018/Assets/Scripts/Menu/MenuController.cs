using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour 
{
	public SceneLoader sceneLoaderScript;

	void Start ()
	{
		sceneLoaderScript = GameObject.Find ("SceneLoader").GetComponent<SceneLoader> ();
	}
		
	public void LoadScene (string sceneName)
	{
		sceneLoaderScript.SceneName = sceneName;
		sceneLoaderScript.StartLoadSequence ();
	}
}
