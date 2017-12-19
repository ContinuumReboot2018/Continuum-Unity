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
	public Animator PowerupUI;

	void Start ()
	{
		PowerupUI = GameObject.Find ("PowerupUI").GetComponent<Animator> ();
		gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		playerControllerScript_P1 = GameObject.Find ("PlayerController_P1").GetComponent<PlayerController> ();
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Bullet") 
		{
			if (other.name.Contains ("P1") || other.GetComponent<Bullet> ().playerControllerScript.PlayerId == 1) 
			{
				if (IsInvoking ("CheckActivatePowerup") == false) 
				{
					Invoke("CheckActivatePowerup", 0.1f);
				}
			}
		}

		if (other.tag == "Player") 
		{
			if (other.name.Contains ("P1")) 
			{
				if (IsInvoking ("CheckActivatePowerup") == false) 
				{
					Invoke("CheckActivatePowerup", 0.1f);
				}
			}
		}

		if (other.tag == "Boundary") 
		{
			Destroy (gameObject);
		}
	}

	void CheckActivatePowerup ()
	{
		if (playerControllerScript_P1.powerupsInUse < gameControllerScript.MaxSimultaneousPowerups) 
		{
			ActivatePowerup_P1 ();
		}

		if (playerControllerScript_P1.powerupsInUse >= gameControllerScript.MaxSimultaneousPowerups) 
		{
			Instantiate (PowerupDeathExplosion, transform.position, Quaternion.identity);
			Destroy (gameObject);
		}
	}

	void ActivatePowerup_P1 ()
	{
		Instantiate (CollectExplosion, transform.position, Quaternion.identity);
		gameControllerScript.SetPowerupTime (PowerupTime);

		switch (ThisPowerup) 
		{
		case powerups.DoubleShot: 

			// Sets double shot iteration (enum) as the next double shot iteration (int).
			if (playerControllerScript_P1.NextDoubleShotIteration < 4) 
			{
				playerControllerScript_P1.DoubleShotIteration = 
					(PlayerController.doubleShotIteration)playerControllerScript_P1.NextDoubleShotIteration;
			}

			// Increases iteration count.
			if (playerControllerScript_P1.NextDoubleShotIteration < 4) 
			{
				playerControllerScript_P1.NextDoubleShotIteration += 1;
			}


			// Tweaks to conditions based on which iteration the player is on.
			switch (playerControllerScript_P1.DoubleShotIteration) 
			{
			case PlayerController.doubleShotIteration.Standard:
				playerControllerScript_P1.powerupsInUse += 1; // Increases powerups in use on first iteration.
				playerControllerScript_P1.ShotType = PlayerController.shotType.Double;
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.DoubleShotFireRates [0];
				break;
			case PlayerController.doubleShotIteration.Enhanced:
				break;
			case PlayerController.doubleShotIteration.Faster:
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.DoubleShotFireRates [1];
				break;
			case PlayerController.doubleShotIteration.Overdrive:
				break;
			}

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
