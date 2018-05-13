using UnityEngine;

public class GameObjectActivator : MonoBehaviour
{
	public GameObject[] ObjectsToActivate;

	public void ActivateObjects ()
	{
		foreach (GameObject objectToActivate in ObjectsToActivate)
		{
			objectToActivate.SetActive (true);
		}
	}
}