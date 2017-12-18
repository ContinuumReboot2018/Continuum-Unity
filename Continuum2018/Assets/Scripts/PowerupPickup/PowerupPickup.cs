using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupPickup : MonoBehaviour 
{
	public GameController gameControllerScript;
	public PlayerController playerControllerScript_P1;

	[Header ("Powerup Stats")]
	public powerups ThisPowerup;
	public enum powerups
	{
		DoubleShot,
		TripleShot,
		ExtraLife,
		BouncingShot,
		RippleShot,
		SlowEnemies,
		Clone
	}

	[Header ("On Pickup")]
	public float PowerupTime = 20;
	public GameObject CollectExplosion;
	public GameObject PowerupDeathExplosion;

	void Start ()
	{
		gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		playerControllerScript_P1 = GameObject.Find ("PlayerController_P1").GetComponent<PlayerController> ();
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Bullet") 
		{
			if (other.name.Contains ("P1") || other.GetComponent<Bullet> ().playerControllerScript.PlayerId == 1) 
			{
				if (gameControllerScript.powerupsInUse < gameControllerScript.MaxSimultaneousPowerups) 
				{
					ActivatePowerup_P1 ();
				}

				if (gameControllerScript.powerupsInUse >= gameControllerScript.MaxSimultaneousPowerups) 
				{
					Instantiate (PowerupDeathExplosion, transform.position, Quaternion.identity);
				}
			}
		}

		if (other.tag == "Player") 
		{
			if (other.name.Contains ("P1")) 
			{
				if (gameControllerScript.powerupsInUse < gameControllerScript.MaxSimultaneousPowerups) 
				{
					ActivatePowerup_P1 ();
				}

				if (gameControllerScript.powerupsInUse >= gameControllerScript.MaxSimultaneousPowerups) 
				{
					gameControllerScript.SetPowerupTime (PowerupTime);
					Instantiate (PowerupDeathExplosion, transform.position, Quaternion.identity);
					Destroy (gameObject);
				}
			}
		}

		if (other.tag == "Boundary") 
		{
			Destroy (gameObject);
		}
	}

	void ActivatePowerup_P1 ()
	{
		gameControllerScript.powerupsInUse += 1;
		Instantiate (CollectExplosion, transform.position, Quaternion.identity);
		gameControllerScript.SetPowerupTime (PowerupTime);

		switch (ThisPowerup) 
		{
		case powerups.DoubleShot: 
			playerControllerScript_P1.ShotType = PlayerController.shotType.Double;
			break;
		case powerups.TripleShot:
			break;
		case powerups.ExtraLife: 
			break;
		case powerups.BouncingShot:
			break;
		case powerups.RippleShot: 
			break;
		case powerups.SlowEnemies:
			break;
		case powerups.Clone: 
			break;
		}

		Destroy (gameObject);
	}
}
