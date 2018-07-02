using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class PlayerParent : MonoBehaviour
{
	private PlayerController playerControllerScript;
	[Tooltip ("Animator for player parent to move up the screen.")]
	public Animator anim;
	[Tooltip ("Checks whether player input should be used.")]
	public bool DontUsePlayerInput;

	void Awake ()
	{
		if (SceneManager.GetActiveScene ().name == "Menu") 
		{
			return;
		}

		playerControllerScript = GetComponentInChildren <PlayerController> ();

		if (TimescaleController.Instance.useTwoPlayers == true) 
		{
			if (playerControllerScript.PlayerId == 1)
			{
				transform.position = new Vector3 (
					-2.9f, 
					transform.position.y, 
					transform.position.z
				);
			}

			if (playerControllerScript.PlayerId == 2) 
			{
				transform.position = new Vector3 (
					2.9f, 
					transform.position.y, 
					transform.position.z
				);
			}
		} 

		if (TimescaleController.Instance.useTwoPlayers == false) 
		{
			if (playerControllerScript.PlayerId == 1)
			{
				transform.position = new Vector3 (
					0, 
					transform.position.y, 
					transform.position.z
				);
			}
		}

		anim.enabled = true;
	}

	// Enables player input by animation events.
	public void EnablePlayerInput ()
	{
		if (DontUsePlayerInput == false) 
		{
			playerControllerScript.EnablePlayerInput ();
		}

		anim.enabled = false;
	}
}