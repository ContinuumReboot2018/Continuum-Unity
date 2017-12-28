using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClonePlayer : MonoBehaviour 
{
	public PlayerController playerControllerScript;
	public Transform ShotSpawn;
	public GameObject Shot;
	public float FireRate = 0.25f;
	private float nextFire;

	void Start () 
	{
		if (playerControllerScript == null) 
		{
			playerControllerScript = GameObject.Find ("PlayerController_P1").GetComponent<PlayerController> ();
		}
	}

	void Update () 
	{
		CheckForBlock ();
	}

	void CheckForBlock ()
	{
		Vector3 up = transform.InverseTransformDirection (Vector3.up);
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
