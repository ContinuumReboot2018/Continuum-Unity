using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour 
{
	public PlayerController playerControllerScript;
	public GameController gameControllerScript;
	public Transform ShotSpawn;
	public GameObject Shot;
	public float FireRate = 0.25f;
	private float nextFire;

	void Start () 
	{
		gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();

		if (playerControllerScript == null) 
		{
			playerControllerScript = GameObject.Find ("PlayerController_P1").GetComponent<PlayerController> ();
		}
	}

	void Update () 
	{
		if (gameControllerScript.PowerupTimeRemaining <= 0) 
		{
			gameObject.SetActive (false);
		}

		CheckForBlock ();

		Debug.DrawRay (transform.position, transform.TransformDirection (Vector3.up), Color.magenta);
	}

	void CheckForBlock ()
	{
		Vector3 up = transform.TransformDirection (Vector3.up);
		RaycastHit hit;

		if (Physics.Raycast (transform.position, up, out hit, 25)) 
		{
			if (hit.collider.tag == ("Block"))
			{
				Shoot ();
			}
		}
	}

	void Shoot ()
	{
		if (Time.unscaledTime > nextFire) 
		{
			Instantiate (Shot, ShotSpawn.position, ShotSpawn.rotation);
			nextFire = Time.unscaledTime + FireRate;
		}
	}
}
