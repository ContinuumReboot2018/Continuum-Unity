using UnityEngine;

public class DontDestroyOnLoadInit : MonoBehaviour 
{
	public static DontDestroyOnLoadInit Instance { get; private set; }

	[Tooltip ("Managers Prefab.")]
	public GameObject ManagersPrefab;

	void Awake ()
	{
		Instance = this;
		// DontDestroyOnLoad (gameObject);
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