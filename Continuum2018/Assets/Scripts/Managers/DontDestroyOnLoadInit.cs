using UnityEngine;

public class DontDestroyOnLoadInit : MonoBehaviour 
{
	[Tooltip ("Managers Prefab.")]
	public GameObject ManagersPrefab;

	void Awake ()
	{
		Time.timeScale = 1;
		DetectManagers ();
	}

	public void DetectManagers ()
	{
		// If there is no MANAGERS GameObject present,
		// Create one and make it not destory on load.
		if (GameObject.Find ("MANAGERS") == null) 
		{
			GameObject managers = Instantiate (ManagersPrefab);
			managers.name = "MANAGERS";
			DontDestroyOnLoad (managers.gameObject); 
		}
	}
}