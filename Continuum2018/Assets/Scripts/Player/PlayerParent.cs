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

	void Start ()
	{
		if (SceneManager.GetActiveScene ().name == "Menu") 
		{
			anim.enabled = true;
			anim.Play ("PlayerEntry");
			return;
		}

		playerControllerScript = GetComponentInChildren <PlayerController> ();
		//anim.enabled = true;
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