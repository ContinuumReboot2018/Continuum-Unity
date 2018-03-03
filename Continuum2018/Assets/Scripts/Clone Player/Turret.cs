using UnityEngine;

public class Turret : MonoBehaviour 
{
	public PlayerController playerControllerScript; // Reference to Player Controller.
	public GameController gameControllerScript; // Reference to Game Controller.
	public Transform ShotSpawn; // Shot spawn transform.
	public GameObject Shot; // Shot to shoot.
	public float FireRate = 0.1f; // Fire rate.
	private float nextFire; // Time to next fire.

	void OnEnable () 
	{
		gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();

		if (playerControllerScript == null) 
		{
			playerControllerScript = GameObject.Find ("PlayerController_P1").GetComponent<PlayerController> ();
		}
	}

	void Update () 
	{
		// Check for powerup time remaining, disable if time ran out.
		if (gameControllerScript.PowerupTimeRemaining <= 0) 
		{
			gameObject.SetActive (false);
		}

		if (playerControllerScript.CurrentShotObject == null) 
		{
			Shot = playerControllerScript.StandardShot;
		}

		// Match shot with player shot.
		if (Shot != playerControllerScript.CurrentShotObject && playerControllerScript.CurrentShotObject != null)
		{
			Shot = playerControllerScript.CurrentShotObject;
		}

		CheckForBlock (); // Finds a block in its line of sight.

		Debug.DrawRay (transform.position, transform.TransformDirection (Vector3.up) * 40, Color.magenta); // Show line in editor.
	}

	// Finds a block in its line of sight.
	void CheckForBlock ()
	{
		Vector3 up = transform.TransformDirection (Vector3.up); // Find outwards direction.
		RaycastHit hit;

		// Look for Blocks pointing our from the player in the current direction.
		if (Physics.Raycast (transform.position, up, out hit, 40)) 
		{
			if (hit.collider.tag == ("Block"))
			{
				Shoot (); // Shoot at it if found.
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