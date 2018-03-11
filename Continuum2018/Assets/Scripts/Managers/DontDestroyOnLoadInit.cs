using UnityEngine;

public class DontDestroyOnLoadInit : MonoBehaviour 
{
	public GameObject Managers;
	public GameObject ManagersPrefab;

	void Awake ()
	{
		Time.timeScale = 1;
		// If there is no MANAGERS GameObject present,
		// Create one and make it not destory on load.
		if (GameObject.Find ("MANAGERS") == null && Managers == null) 
		{
			GameObject managers = Instantiate (ManagersPrefab);
			managers.name = "MANAGERS";
			DontDestroyOnLoad (managers.gameObject); 
		}
			
		if (Managers != null) 
		{
			DontDestroyOnLoad (Managers);
		}
	}
}