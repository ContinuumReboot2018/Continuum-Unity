using UnityEngine;

public class PlayerParent : MonoBehaviour
{
	[Tooltip ("Reference to Player Controller.")]
	public PlayerController playerControllerScript;
	[Tooltip ("Animator for player parent to move up the screen.")]
	public Animator anim;
	[Tooltip ("Checks whether player input should be used.")]
	public bool DontUsePlayerInput;

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