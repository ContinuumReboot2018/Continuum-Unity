using UnityEngine;

public class PlayerChecker : MonoBehaviour 
{
	public GameObject PlayerOne;
	public GameObject PlayerTwo;

	void Start () 
	{
		if (GameController.Instance.missionModiferSettings [SaveAndLoadScript.Instance.MissionId].Multiplayer == true) 
		{
			PlayerTwo.SetActive (true);
			TimescaleController.Instance.Start ();
		}

		if (GameController.Instance.missionModiferSettings [SaveAndLoadScript.Instance.MissionId].Multiplayer == false) 
		{
			PlayerTwo.SetActive (false);
			TimescaleController.Instance.Start ();
		}
	}
}
