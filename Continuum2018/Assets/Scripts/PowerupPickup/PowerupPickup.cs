using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerupPickup : MonoBehaviour 
{
	public PlayerController playerControllerScript_P1; // Reference to Player Controller.
	public float ThresholdPowerupMoveDistance = 12;
	public float MoveSpeed = 1;
	public CameraShake camShakeScript; // Reference to camera shake.

	[Header ("On Awake")]
	[Tooltip ("The audio effect to play when spawned.")]
	public AudioSource AwakeAudio;
	[Tooltip ("The mesh renderer which has the powerup pickup texture.")]
	public MeshRenderer meshrend;
	[Tooltip ("The collider.")]
	public SphereCollider col;
	[Tooltip ("The particle effect to play when spawned.")]
	public ParticleSystem AwakeParticles;
	[Tooltip ("The delay to show the powerup after spawning.")]
	public float AwakeDelay = 1;

	[Header ("Powerup Stats")]
	[Tooltip ("The current powerup type.")]
	public powerups ThisPowerup;
	public enum powerups
	{
		DoubleShot = 0, // Player shoots two bullets each shot.
		TripleShot = 1, // Player shoots one bullet up and two at 45 degree angles.
		ExtraLife = 2, 	// Gives the player a life.
		RippleShot = 3, // Player shoots a ring which expands over time.
		Turret = 4, 	// A turret will be enabled and rotate around the player if collected.
		Helix = 5, 		// A double helix will be enabled which rotates and moves around the player/ 
		Rapidfire = 6, 	// Increases the fire rate for the player.
		Overdrive = 7, 	// Player shoots bullets which go through anything.
		Ricochet = 8,	// Bullets bounce off the top and bottom of the screen.
		Homing = 9, 	// Bullets find the closest block and move towards it.
		FreezeTime = 10 // Freezes time temporarily.
	}

	[Header ("On Pickup")]
	[Tooltip ("The powerup texture which will feed to the powerup list and explosion UI.")]
	public Texture2D PowerupTexture;
	[Tooltip ("Checks whether this is a shooting powerup.")]
	public bool isShootingPowerup = true;
	[Tooltip ("How much time the powerup adds to the powerup time remaining.")]
	public float PowerupTime = 20;
	[Tooltip ("The explosion when the player or bullet collides with it.")]
	public GameObject CollectExplosion;
	[Tooltip ("The sound that plays when the powerup time is running out.")]
	public AudioSource PowerupTimeRunningOutAudio;
	[Tooltip ("The color of the powerup explosion UI texture.")]
	public Color PowerupPickupUIColor = Color.white;

	[Header ("On End Life")]
	[Tooltip ("Destroy by time script.")]
	public DestroyByTime destroyByTimeScript;
	[Tooltip ("The animator that the powerup pickup uses.")]
	public Animator anim;

	void Start ()
	{
		playerControllerScript_P1 = GameObject.Find ("PlayerController_P1").GetComponent<PlayerController> ();
		PowerupTimeRunningOutAudio = GameObject.Find ("PowerupRunningOutSound").GetComponent<AudioSource> ();
		camShakeScript = GameObject.Find ("CamShake").GetComponent<CameraShake> ();
		destroyByTimeScript = GetComponent<DestroyByTime> (); // Timer for lifetime.
		anim = GetComponent<Animator> (); // The animator which scales in size.

		StartCoroutine (ShowPowerup ()); // Start sequence to animate the powerup pickup to be visible.
		StartCoroutine (DestroyAnimation ()); // Start sequence to animate the powerup to disappear.
	}

	void Update ()
	{
		CheckPlayerDistance ();
	}

	void CheckPlayerDistance ()
	{
		if (TimescaleController.Instance.Distance >= ThresholdPowerupMoveDistance) 
		{
			MovePowerupToPlayer ();
		}
	}

	void MovePowerupToPlayer ()
	{
		Vector3 newPos = new Vector3 (
			playerControllerScript_P1.playerMesh.transform.position.x, 
			playerControllerScript_P1.playerMesh.transform.position.y, 
			-2.5f
		);

		transform.position = Vector3.MoveTowards (
			transform.position, 
			newPos, 
			MoveSpeed * Time.deltaTime
		);
	}

	// Makes powerup visible.
	IEnumerator ShowPowerup ()
	{
		yield return new WaitForSeconds (AwakeDelay);
		meshrend.enabled = true; // Turn on the mesh renderer.
		AwakeParticles.Play (); // Play particles.
	}

	// Timer for destroying.
	IEnumerator DestroyAnimation ()
	{
		yield return new WaitForSecondsRealtime (destroyByTimeScript.delay - 1);
		anim.Play ("PowerupPickupDestroy"); // Play the animation to destroy.
	}
		
	// Collisions via particles.
	void OnParticleCollision (GameObject particle)
	{
		if (particle.tag == "Bullet") 
		{
			CollisionWithObject ();
		}

		if (particle.tag == "Player") 
		{
			CollisionWithObject ();
		}
	}

	// Trigger by bullets and player.
	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Bullet") 
		{
			if (other.name.Contains ("P1") || other.name.Contains ("Shield_Col") || 
				other.GetComponent<Bullet> ().playerControllerScript.PlayerId == 1) 
			{
				CollisionWithObject ();
			}
		}

		if (other.tag == "Player") 
		{
			if (other.name.Contains ("P1")) 
			{
				CollisionWithObject ();
			}
		}

		if (other.tag == "Boundary") 
		{
			Destroy (gameObject);
			return;
		}
	}

	// Prepares the player to activate the assigned powerup and resets next fire.
	void CollisionWithObject ()
	{
		// Gives new next fire amount so the player can keep on firing.
		float nextfire = 
			Time.time + 
			(playerControllerScript_P1.CurrentFireRate / (playerControllerScript_P1.FireRateTimeMultiplier * Time.timeScale));
		
		playerControllerScript_P1.CheckPowerupImageUI (); // Add to powerup list UI.
	
		playerControllerScript_P1.NextFire = nextfire; // Allow player to shoot.
		playerControllerScript_P1.DoubleShotNextFire = nextfire; // Allow player to shoot.
		playerControllerScript_P1.TripleShotNextFire = nextfire; // Allow player to shoot.
		playerControllerScript_P1.RippleShotNextFire = nextfire; // Allow player to shoot.
		CheckActivatePowerup ();

		if (playerControllerScript_P1.timeIsSlowed == false)
		{
			TimescaleController.Instance.OverrideTimeScaleTimeRemaining += 0.5f; // Set Timescale ovveride time.
			TimescaleController.Instance.OverridingTimeScale = 0.2f; // Set overriding time scale.
		}

		camShakeScript.ShakeCam (0.5f, 1, 1);
	}

	// Activates the powerup.
	void CheckActivatePowerup ()
	{
		CreatePowerupPickupUI (); // Creates UI at collection point.
		ActivatePowerup_P1 (); // Searches through list and activates relevant powerup.
		#if !PLATFORM_STANDALONE_OSX && !PLATFORM_ANDROID && !PLATFORM_WEBGL
		playerControllerScript_P1.Vibrate (0.6f, 0.6f, 0.3f); // Allows controller vibration.
		#endif
		PowerupTimeRunningOutAudio.Stop (); // If powerup running out audio is playing, stop it.
		Destroy (gameObject); // Destroy the pickup.
		return;
	}

	// Creates a particle effect at the impact point and sets powerup textures.
	public void CreatePowerupPickupUI ()
	{
		GameObject powerupPickupUI = Instantiate (GameController.Instance.PowerupPickupUI, transform.position, Quaternion.identity);
		powerupPickupUI.GetComponentInChildren<RawImage> ().texture = PowerupTexture;
		powerupPickupUI.GetComponentInChildren<RawImage> ().color = PowerupPickupUIColor;
	}

	void IncrementPowerupsInUse (bool isShootingPowerup)
	{
		if (isShootingPowerup == false) 
		{
			playerControllerScript_P1.powerupsInUse++;
		} 

		else 
		
		{
			if (playerControllerScript_P1.ShotType == PlayerController.shotType.Standard) 
			{
				playerControllerScript_P1.powerupsInUse++;
			}
		}
	}
		
	// Finds powerup from list and activates it.
	void ActivatePowerup_P1 ()
	{
		Instantiate (CollectExplosion, transform.position, Quaternion.identity); // Creates powerup explosion particles.
		GameController.Instance.SetPowerupTime (PowerupTime);
		GameController.Instance.totalPowerupsCollected++;

		switch (ThisPowerup) 
		{

		case powerups.FreezeTime:

			if (GameController.Instance.VhsAnim.GetCurrentAnimatorStateInfo (0).IsName ("Slow") == false) 
			{
				GameController.Instance.VhsAnim.SetTrigger ("Slow");
			}
				
			TimescaleController.Instance.OverridingTimeScale = 0.3f;
			TimescaleController.Instance.OverrideTimeScaleTimeRemaining += PowerupTime;
			TimescaleController.Instance.isOverridingTimeScale = true;

			if (playerControllerScript_P1.timeIsSlowed == false) 
			{
				playerControllerScript_P1.timeIsSlowed = true;
				SetPowerupTexture (GameController.Instance.NextPowerupSlot_P1);
				GameController.Instance.NextPowerupSlot_P1 += 1;
				IncrementPowerupsInUse (false);
			}
			break;

		case powerups.DoubleShot: 

			if (playerControllerScript_P1.ShotType == PlayerController.shotType.Standard) 
			{
				playerControllerScript_P1.AddParticleActiveEffects ();
			}

			IncrementPowerupsInUse (true);

			// Switches to double shot mode
			playerControllerScript_P1.ShotType = PlayerController.shotType.Double;
			playerControllerScript_P1.CurrentShootingHeatCost = playerControllerScript_P1.DoubleShootingHeatCost;

			if (playerControllerScript_P1.isInOverdrive == true) 
			{
				playerControllerScript_P1.DoubleShotIteration = PlayerController.shotIteration.Overdrive;
			}

			if (playerControllerScript_P1.isInRapidFire == false) 
			{
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.DoubleShotFireRates [0];
			}

			if (playerControllerScript_P1.isInRapidFire == true) 
			{
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.DoubleShotFireRates [1];
			}

			// Tweaks to conditions based on which iteration the player is on.
			switch (playerControllerScript_P1.DoubleShotIteration) 
			{
			case PlayerController.shotIteration.Standard:
				
				playerControllerScript_P1.ShotType = PlayerController.shotType.Double;

				if (GameController.Instance.gameModifier.AlwaysRapidfire == false)
				{
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.DoubleShotFireRates [0];
				}
				break;
			case PlayerController.shotIteration.Enhanced:
				break;
			case PlayerController.shotIteration.Rapid:
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.DoubleShotFireRates [1];
				break;
			case PlayerController.shotIteration.Overdrive:
				break;
			}

			SetPowerupTexture (0);
				break;

		case powerups.TripleShot:

			if (playerControllerScript_P1.ShotType == PlayerController.shotType.Standard) 
			{
				playerControllerScript_P1.AddParticleActiveEffects ();
			}

			IncrementPowerupsInUse (true);

			// Switches to triple shot mode
			playerControllerScript_P1.ShotType = PlayerController.shotType.Triple;
			playerControllerScript_P1.CurrentShootingHeatCost = playerControllerScript_P1.TripleShootingHeatCost;

			if (playerControllerScript_P1.isInOverdrive == true) 
			{
				playerControllerScript_P1.TripleShotIteration = PlayerController.shotIteration.Overdrive;
			}

			if (playerControllerScript_P1.isInRapidFire == false) 
			{
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.TripleShotFireRates [0];
			}

			if (playerControllerScript_P1.isInRapidFire == true) 
			{
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.TripleShotFireRates [1];
			}
				
			// Tweaks to conditions based on which iteration the player is on.
			switch (playerControllerScript_P1.TripleShotIteration) 
			{
			case PlayerController.shotIteration.Standard:
				playerControllerScript_P1.ShotType = PlayerController.shotType.Triple;

				if (GameController.Instance.gameModifier.AlwaysRapidfire == false)
				{
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.TripleShotFireRates [0];
				}
				break;

			case PlayerController.shotIteration.Enhanced:
				break;

			case PlayerController.shotIteration.Rapid:
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.TripleShotFireRates [1];
				break;

			case PlayerController.shotIteration.Overdrive:
				break;
			}

			SetPowerupTexture (0);
			break;

		case powerups.ExtraLife: 
			
			// Increases life count.
			GameController.Instance.Lives += 1;

			if (GameController.Instance.Lives < GameController.Instance.MaxLives) 
			{
				GameController.Instance.MaxLivesText.text = "";
			}

			if (GameController.Instance.Lives >= GameController.Instance.MaxLives) 
			{
				GameController.Instance.MaxLivesText.text = "MAX";
				Debug.Log ("Reached maximum lives.");
			}
				
			// Updates lives.
			GameController.Instance.Lives = Mathf.Clamp (GameController.Instance.Lives, 0, GameController.Instance.MaxLives);
			GameController.Instance.LifeImages [GameController.Instance.Lives - 2].gameObject.SetActive (true);
			GameController.Instance.LifeImages [GameController.Instance.Lives - 2].enabled = true;
			GameController.Instance.LifeImages [GameController.Instance.Lives - 2].color = Color.white;
			//GameController.Instance.UpdateLives (); // Updates lives UI.
			break;

		case powerups.RippleShot: 

			if (playerControllerScript_P1.ShotType == PlayerController.shotType.Standard) 
			{
				playerControllerScript_P1.AddParticleActiveEffects ();
			}

			IncrementPowerupsInUse (true);

			if (playerControllerScript_P1.ShotType != PlayerController.shotType.Ripple)
			{
				// Switches to ripple shot mode.
				playerControllerScript_P1.ShotType = PlayerController.shotType.Ripple;
				playerControllerScript_P1.CurrentShootingHeatCost = playerControllerScript_P1.RippleShootingHeatCost;

				if (playerControllerScript_P1.isInOverdrive == true || playerControllerScript_P1.isHoming == true) 
				{
					playerControllerScript_P1.RippleShotIteration = PlayerController.shotIteration.Overdrive;
				}

				if (playerControllerScript_P1.isInRapidFire == false) 
				{
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.RippleShotFireRates [0];
				}

				if (playerControllerScript_P1.isInRapidFire == true) 
				{
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.RippleShotFireRates [1];
				}

				// Tweaks to conditions based on which iteration the player is on.
				switch (playerControllerScript_P1.RippleShotIteration) 
				{
				case PlayerController.shotIteration.Standard:
					playerControllerScript_P1.ShotType = PlayerController.shotType.Ripple;

					if (GameController.Instance.gameModifier.AlwaysRapidfire == false)
					{
						playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.RippleShotFireRates [0];
					}
					break;

				case PlayerController.shotIteration.Enhanced:
					break;

				case PlayerController.shotIteration.Rapid:
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.RippleShotFireRates [1];
					break;

				case PlayerController.shotIteration.Overdrive:
					break;
				}

				SetPowerupTexture (0); // Set the shooting powerup texture.
			}
			break;

		case powerups.Helix:

			// Adds a helix object that follows the player.
			if (playerControllerScript_P1.Helix.activeInHierarchy == false) 
			{
				SetPowerupTexture (GameController.Instance.NextPowerupSlot_P1);
				GameController.Instance.NextPowerupSlot_P1 += 1;
				playerControllerScript_P1.Helix.SetActive (true);
				playerControllerScript_P1.AddParticleActiveEffects ();
				IncrementPowerupsInUse (false);
			}

			break;

		case powerups.Turret:

			// Adds turret to the player and rotates around it.
			if (playerControllerScript_P1.nextTurretSpawn < 4) 
			{
				GameObject clone = playerControllerScript_P1.Turrets [playerControllerScript_P1.nextTurretSpawn];
				clone.SetActive (true);
				clone.GetComponent<Turret> ().playerControllerScript = playerControllerScript_P1;

				SetPowerupTexture (GameController.Instance.NextPowerupSlot_P1);

				GameController.Instance.NextPowerupSlot_P1 += 1;
				playerControllerScript_P1.nextTurretSpawn += 1;
				playerControllerScript_P1.AddParticleActiveEffects ();
				IncrementPowerupsInUse (false);
			}
			break;

		case powerups.Rapidfire:
			
			// Player can shoot faster.
			if (playerControllerScript_P1.isInRapidFire == false)
			{
				switch (playerControllerScript_P1.ShotType) 
				{
				case PlayerController.shotType.Standard:
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.DoubleShotFireRates [1];
					break;	
				case PlayerController.shotType.Double:
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.DoubleShotFireRates [1];
					break;
				case PlayerController.shotType.Triple:
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.TripleShotFireRates [1];
					break;
				case PlayerController.shotType.Ripple:
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.RippleShotFireRates [1];
					break;
				}

				GameController.Instance.RapidfireImage.enabled = true;
				GameController.Instance.RapidfireImage.transform.SetSiblingIndex (-GameController.Instance.NextPowerupShootingSlot_P1 + 3);

				GameController.Instance.NextPowerupShootingSlot_P1 += 1;
				GameController.Instance.RapidfireHex.enabled = true;
				playerControllerScript_P1.isInRapidFire = true;
				GameController.Instance.RapidfireImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");
				playerControllerScript_P1.AddParticleActiveEffects ();
				IncrementPowerupsInUse (false);
			}

			break;

		case powerups.Overdrive:

			// Player shoot special bullets which go through any hazard or block.
			if (playerControllerScript_P1.isInOverdrive == false) 
			{
				playerControllerScript_P1.StandardShotIteration = PlayerController.shotIteration.Overdrive;
				playerControllerScript_P1.DoubleShotIteration = PlayerController.shotIteration.Overdrive;
				playerControllerScript_P1.TripleShotIteration = PlayerController.shotIteration.Overdrive;
				playerControllerScript_P1.RippleShotIteration = PlayerController.shotIteration.Overdrive;
				GameController.Instance.OverdriveImage.enabled = true;
				GameController.Instance.OverdriveImage.transform.SetSiblingIndex (-GameController.Instance.NextPowerupShootingSlot_P1 + 3);
				GameController.Instance.NextPowerupShootingSlot_P1 += 1;
				GameController.Instance.OverdriveHex.enabled = true;
				playerControllerScript_P1.isInOverdrive = true;
				GameController.Instance.OverdriveImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");
				playerControllerScript_P1.AddParticleActiveEffects ();
				IncrementPowerupsInUse (false);
			}
			break;

		case powerups.Ricochet:

			if (playerControllerScript_P1.isRicochet == false) 
			{
				// Player shoots bullets which can bounce off the top and bottom of the screen.
				playerControllerScript_P1.EnableRicochetObject ();

				if (playerControllerScript_P1.DoubleShotIteration != PlayerController.shotIteration.Overdrive)
				{
					playerControllerScript_P1.DoubleShotIteration = PlayerController.shotIteration.Enhanced;
				}

				if (playerControllerScript_P1.TripleShotIteration != PlayerController.shotIteration.Overdrive) 
				{
					playerControllerScript_P1.TripleShotIteration = PlayerController.shotIteration.Enhanced;
				}

				if (playerControllerScript_P1.RippleShotIteration != PlayerController.shotIteration.Overdrive)
				{
					playerControllerScript_P1.RippleShotIteration = PlayerController.shotIteration.Enhanced;
				}

				if (playerControllerScript_P1.StandardShotIteration != PlayerController.shotIteration.Overdrive) 
				{
					playerControllerScript_P1.StandardShotIteration = PlayerController.shotIteration.Enhanced;
				}

				playerControllerScript_P1.isRicochet = true;
				GameController.Instance.RicochetImage.transform.SetSiblingIndex (-GameController.Instance.NextPowerupShootingSlot_P1 + 3);
				GameController.Instance.NextPowerupShootingSlot_P1 += 1;
				GameController.Instance.RicochetImage.enabled = true;
				GameController.Instance.RicochetHex.enabled = true;
				GameController.Instance.RicochetImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");
				playerControllerScript_P1.AddParticleActiveEffects ();
				IncrementPowerupsInUse (false);
			}
			break;
		
		case powerups.Homing:
			
			// Player shoots bullets which home in on blocks.
			if (playerControllerScript_P1.isHoming == false) 
			{
				playerControllerScript_P1.isHoming = true;
				GameController.Instance.HomingImage.transform.SetSiblingIndex (-GameController.Instance.NextPowerupShootingSlot_P1 + 3);
				GameController.Instance.NextPowerupShootingSlot_P1 += 1;
				GameController.Instance.HomingImage.enabled = true;
				GameController.Instance.HomingHex.enabled = true;
				GameController.Instance.HomingImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");
				playerControllerScript_P1.AddParticleActiveEffects ();
				IncrementPowerupsInUse (false);
			}
			break;
		}

		Destroy (gameObject);
		return;
	}

	void SetPowerupTexture (int index)
	{
		GameController.Instance.PowerupImage_P1 [index].gameObject.SetActive (true);
		GameController.Instance.PowerupImage_P1 [index].texture = PowerupTexture;
		GameController.Instance.PowerupImage_P1 [index].color = new Color (1, 1, 1, 1);
		GameController.Instance.PowerupImage_P1 [index].gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");
	}
}