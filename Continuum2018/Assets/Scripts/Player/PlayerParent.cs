using UnityEngine;

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
		playerControllerScript = GetComponentInChildren <PlayerController> ();
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