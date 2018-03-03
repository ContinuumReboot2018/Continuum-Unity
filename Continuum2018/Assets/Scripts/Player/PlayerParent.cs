using UnityEngine;

public class PlayerParent : MonoBehaviour
{
	public PlayerController playerControllerScript; // Reference to Player Controller.
	public Animator anim; // Animator for player parent to move up the screen.
	public bool DontUsePlayerInput; // Checks whether player input should be used.

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