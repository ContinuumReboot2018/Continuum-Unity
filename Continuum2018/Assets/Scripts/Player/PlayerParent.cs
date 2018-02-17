using UnityEngine;

public class PlayerParent : MonoBehaviour
{
	public Animator anim; 
	public PlayerController playerControllerScript;
	public bool DontUsePlayerInput;

	public void EnablePlayerInput ()
	{
		if (DontUsePlayerInput == false) 
		{
			playerControllerScript.EnablePlayerInput ();
		}

		anim.enabled = false;
	}
}