using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SimpleTimedSceneLoader : MonoBehaviour 
{
	public string SceneToLoad;
	public float Delay = 5;

	void Start ()
	{
		StartCoroutine (TimedSceneLoad ());
	}

	IEnumerator TimedSceneLoad ()
	{
		yield return new WaitForSecondsRealtime (Delay);
		SceneManager.LoadScene (SceneToLoad);
	}
}