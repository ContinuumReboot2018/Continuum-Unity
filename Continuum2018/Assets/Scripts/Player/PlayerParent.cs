using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParent : MonoBehaviour
{
	public Animator anim; 
	public PlayerController playerControllerScript;

	public void EnablePlayerInput ()
	{
		playerControllerScript.EnablePlayerInput ();
		anim.enabled = false;
	}
}
