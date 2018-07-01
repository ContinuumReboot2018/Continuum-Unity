using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerupPickup : MonoBehaviour 
{
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
		if (PlayerController.PlayerTwoInstance != null)
		{
			if (PlayerController.PlayerOneInstance.RelativeDistance > PlayerController.PlayerTwoInstance.RelativeDistance) 
			{
				Vector3 newPos = new Vector3 (
					                 PlayerController.PlayerOneInstance.playerMesh.transform.position.x, 
					                 PlayerController.PlayerOneInstance.playerMesh.transform.position.y, 
					                 -2.5f
				                 );

				transform.position = Vector3.MoveTowards (
					transform.position, 
					newPos, 
					MoveSpeed * Time.deltaTime
				);
			}

			if (PlayerController.PlayerOneInstance.RelativeDistance < PlayerController.PlayerTwoInstance.RelativeDistance)
			{
				Vector3 newPos = new Vector3 (
					                 PlayerController.PlayerTwoInstance.playerMesh.transform.position.x, 
					                 PlayerController.PlayerTwoInstance.playerMesh.transform.position.y, 
					                 -2.5f
				                 );

				transform.position = Vector3.MoveTowards (
					transform.position, 
					newPos, 
					MoveSpeed * Time.deltaTime
				);
			}
		} 

		else 
		{
			Vector3 newPos = new Vector3 (
				PlayerController.PlayerOneInstance.playerMesh.transform.position.x, 
				PlayerController.PlayerOneInstance.playerMesh.transform.position.y, 
				-2.5f
			);

			transform.position = Vector3.MoveTowards (
				transform.position, 
				newPos, 
				MoveSpeed * Time.deltaTime
			);
		}
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
			if (particle.name.Contains ("P1") || particle.name.Contains ("Shield_Col")
				|| particle.transform.parent.name.Contains ("P1")) 
			{
				CollisionWithObject (1);
			}

			if (particle.name.Contains ("P2") || particle.name.Contains ("Shield_Col")
				|| particle.transform.parent.name.Contains ("P2")) 
			{
				CollisionWithObject (2);
			}
		}

		if (particle.tag == "Player") 
		{
			if (particle.name.Contains ("P1")) 
			{
				CollisionWithObject (1);
			}

			if (particle.name.Contains ("P2")) 
			{
				CollisionWithObject (2);
			}
		}
	}

	// Trigger by bullets and player.
	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Bullet") 
		{
			if (other.name.Contains ("P1") || other.name.Contains ("Shield_Col"))
				//|| other.GetComponent<Bullet> ().playerControllerScript.PlayerId == 1) 
			{
				CollisionWithObject (1);
			}

			if (other.name.Contains ("P2") || other.name.Contains ("Shield_Col")) 
				//|| other.GetComponent<Bullet> ().playerControllerScript.PlayerId == 2) 
			{
				CollisionWithObject (2);
			}
		}

		if (other.tag == "Player") 
		{
			if (other.name.Contains ("P1")) 
			{
				CollisionWithObject (1);
			}

			if (other.name.Contains ("P2")) 
			{
				CollisionWithObject (2);
			}
		}

		if (other.tag == "Boundary") 
		{
			Destroy (gameObject);
			return;
		}
	}

	// Prepares the player to activate the assigned powerup and resets next fire.
	void CollisionWithObject (int PlayerId)
	{
		if (PlayerId == 1)
		{
			float nextfire = 
				Time.time + 
				(PlayerController.PlayerOneInstance.CurrentFireRate / (PlayerController.PlayerOneInstance.FireRateTimeMultiplier * Time.timeScale));

			PlayerController.PlayerOneInstance.CheckPowerupImageUI ();
			PlayerController.PlayerOneInstance.NextFire = nextfire; // Allow player to shoot.
			PlayerController.PlayerOneInstance.DoubleShotNextFire = nextfire; // Allow player to shoot.
			PlayerController.PlayerOneInstance.TripleShotNextFire = nextfire; // Allow player to shoot.
			PlayerController.PlayerOneInstance.RippleShotNextFire = nextfire; // Allow player to shoot.

			CheckActivatePowerup (1);
		}

		if (PlayerId == 2)
		{
			float nextfire = 
				Time.time + 
				(PlayerController.PlayerTwoInstance.CurrentFireRate / (PlayerController.PlayerTwoInstance.FireRateTimeMultiplier * Time.timeScale));

			PlayerController.PlayerTwoInstance.CheckPowerupImageUI ();
			PlayerController.PlayerTwoInstance.NextFire = nextfire; // Allow player to shoot.
			PlayerController.PlayerTwoInstance.DoubleShotNextFire = nextfire; // Allow player to shoot.
			PlayerController.PlayerTwoInstance.TripleShotNextFire = nextfire; // Allow player to shoot.
			PlayerController.PlayerTwoInstance.RippleShotNextFire = nextfire; // Allow player to shoot.

			CheckActivatePowerup (2);
		}
			
		if (PlayerController.PlayerOneInstance.timeIsSlowed == false ||
			PlayerController.PlayerTwoInstance.timeIsSlowed == false)
		{
			TimescaleController.Instance.OverrideTimeScaleTimeRemaining += 0.5f; // Set Timescale ovveride time.
			TimescaleController.Instance.OverridingTimeScale = 0.2f; // Set overriding time scale.
		}

		camShakeScript.ShakeCam (0.5f, 1, 1);
	}

	// Activates the powerup.
	void CheckActivatePowerup (int playerId)
	{
		CreatePowerupPickupUI (); // Creates UI at collection point.
		ActivatePowerup (playerId); // Searches through list and activates relevant powerup.
		#if !PLATFORM_STANDALONE_OSX && !PLATFORM_ANDROID && !PLATFORM_WEBGL
		if (playerId == 1)
		{
			PlayerController.PlayerOneInstance.Vibrate (0.6f, 0.6f, 0.3f); // Allows controller vibration.
		}

		if (playerId == 2)
		{
			PlayerController.PlayerTwoInstance.Vibrate (0.6f, 0.6f, 0.3f); // Allows controller vibration.
		}
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

	void IncrementPowerupsInUse (int PlayerId, bool isShootingPowerup)
	{
		if (isShootingPowerup == false) 
		{
			if (PlayerId == 1) 
			{
				PlayerController.PlayerOneInstance.powerupsInUse++;
			}

			if (PlayerId == 2) 
			{
				PlayerController.PlayerTwoInstance.powerupsInUse++;
			}
		} 

		else 
		
		{
			if (PlayerId == 1) 
			{
				if (PlayerController.PlayerOneInstance.ShotType == PlayerController.shotType.Standard) 
				{
					PlayerController.PlayerOneInstance.powerupsInUse++;
				}
			}

			if (PlayerId == 2) 
			{
				if (PlayerController.PlayerOneInstance.ShotType == PlayerController.shotType.Standard)
				{
					PlayerController.PlayerTwoInstance.powerupsInUse++;
				}
			}
		}
	}
		
	// Finds powerup from list and activates it.
	void ActivatePowerup (int PlayerId)
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

			if (PlayerController.PlayerOneInstance.timeIsSlowed == false || PlayerController.PlayerTwoInstance.timeIsSlowed == false) 
			{
				PlayerController.PlayerOneInstance.timeIsSlowed = true;

				if (PlayerController.PlayerTwoInstance != null) 
				{
					PlayerController.PlayerTwoInstance.timeIsSlowed = true;
				}

				SetPowerupTexture (GameController.Instance.NextPowerupSlot_P1);
				GameController.Instance.NextPowerupSlot_P1 += 1;

				if (PlayerId == 1) 
				{
					IncrementPowerupsInUse (1, false);
				}

				if (PlayerId == 2) 
				{
					IncrementPowerupsInUse (2, false);
				}
			}

		break;

		case powerups.DoubleShot: 

			if (PlayerId == 1)
			{
				if (PlayerController.PlayerOneInstance.ShotType == PlayerController.shotType.Standard)
				{
					PlayerController.PlayerOneInstance.AddParticleActiveEffects ();
				}

				IncrementPowerupsInUse (1, true);

				// Switches to double shot mode
				PlayerController.PlayerOneInstance.ShotType = PlayerController.shotType.Double;
				PlayerController.PlayerOneInstance.CurrentShootingHeatCost = PlayerController.PlayerOneInstance.DoubleShootingHeatCost;

				if (PlayerController.PlayerOneInstance.isInOverdrive == true)
				{
					PlayerController.PlayerOneInstance.DoubleShotIteration = PlayerController.shotIteration.Overdrive;
				}

				if (PlayerController.PlayerOneInstance.isInRapidFire == false)
				{
					PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.DoubleShotFireRates [0];
				}

				if (PlayerController.PlayerOneInstance.isInRapidFire == true)
				{
					PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.DoubleShotFireRates [1];
				}

				// Tweaks to conditions based on which iteration the player is on.
				switch (PlayerController.PlayerOneInstance.DoubleShotIteration)
				{
				case PlayerController.shotIteration.Standard:
				
					PlayerController.PlayerOneInstance.ShotType = PlayerController.shotType.Double;

					if (GameController.Instance.gameModifier.AlwaysRapidfire == false)
					{
						PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.DoubleShotFireRates [0];
					}
					break;
				case PlayerController.shotIteration.Enhanced:
					break;
				case PlayerController.shotIteration.Rapid:
					PlayerController.PlayerOneInstance.CurrentFireRate =PlayerController.PlayerOneInstance.DoubleShotFireRates [1];
					break;
				case PlayerController.shotIteration.Overdrive:
					break;
				}
			}

			if (PlayerId == 2)
			{
				if (PlayerController.PlayerTwoInstance.ShotType == PlayerController.shotType.Standard)
				{
					PlayerController.PlayerTwoInstance.AddParticleActiveEffects ();
				}

				IncrementPowerupsInUse (2, true);

				// Switches to double shot mode
				PlayerController.PlayerTwoInstance.ShotType = PlayerController.shotType.Double;
				PlayerController.PlayerTwoInstance.CurrentShootingHeatCost = PlayerController.PlayerTwoInstance.DoubleShootingHeatCost;

				if (PlayerController.PlayerTwoInstance.isInOverdrive == true)
				{
					PlayerController.PlayerTwoInstance.DoubleShotIteration = PlayerController.shotIteration.Overdrive;
				}

				if (PlayerController.PlayerTwoInstance.isInRapidFire == false)
				{
					PlayerController.PlayerTwoInstance.CurrentFireRate = PlayerController.PlayerTwoInstance.DoubleShotFireRates [0];
				}

				if (PlayerController.PlayerTwoInstance.isInRapidFire == true)
				{
					PlayerController.PlayerTwoInstance.CurrentFireRate = PlayerController.PlayerTwoInstance.DoubleShotFireRates [1];
				}

				// Tweaks to conditions based on which iteration the player is on.
				switch (PlayerController.PlayerTwoInstance.DoubleShotIteration)
				{
				case PlayerController.shotIteration.Standard:

					PlayerController.PlayerTwoInstance.ShotType = PlayerController.shotType.Double;

					if (GameController.Instance.gameModifier.AlwaysRapidfire == false)
					{
						PlayerController.PlayerTwoInstance.CurrentFireRate = PlayerController.PlayerTwoInstance.DoubleShotFireRates [0];
					}
					break;
				case PlayerController.shotIteration.Enhanced:
					break;
				case PlayerController.shotIteration.Rapid:
					PlayerController.PlayerTwoInstance.CurrentFireRate =PlayerController.PlayerTwoInstance.DoubleShotFireRates [1];
					break;
				case PlayerController.shotIteration.Overdrive:
					break;
				}
			}

			SetPowerupTexture (0);
			break;

		case powerups.TripleShot:

			if (PlayerId == 1) 
			{
				if (PlayerController.PlayerOneInstance.ShotType == PlayerController.shotType.Standard) 
				{
					PlayerController.PlayerOneInstance.AddParticleActiveEffects ();
				}

				IncrementPowerupsInUse (1, true);

				// Switches to triple shot mode
				PlayerController.PlayerOneInstance.ShotType = PlayerController.shotType.Triple;
				PlayerController.PlayerOneInstance.CurrentShootingHeatCost = PlayerController.PlayerOneInstance.TripleShootingHeatCost;

				if (PlayerController.PlayerOneInstance.isInOverdrive == true) 
				{
					PlayerController.PlayerOneInstance.TripleShotIteration = PlayerController.shotIteration.Overdrive;
				}

				if (PlayerController.PlayerOneInstance.isInRapidFire == false) 
				{
					PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.TripleShotFireRates [0];
				}

				if (PlayerController.PlayerOneInstance.isInRapidFire == true) 
				{
					PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.TripleShotFireRates [1];
				}
				
				// Tweaks to conditions based on which iteration the player is on.
				switch (PlayerController.PlayerOneInstance.TripleShotIteration) 
				{
				case PlayerController.shotIteration.Standard:
					PlayerController.PlayerOneInstance.ShotType = PlayerController.shotType.Triple;

					if (GameController.Instance.gameModifier.AlwaysRapidfire == false) 
					{
						PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.TripleShotFireRates [0];
					}
					break;

				case PlayerController.shotIteration.Enhanced:
					break;

				case PlayerController.shotIteration.Rapid:
					PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.TripleShotFireRates [1];
					break;

				case PlayerController.shotIteration.Overdrive:
					break;
				}
			}

			if (PlayerId == 2) 
			{
				if (PlayerController.PlayerTwoInstance.ShotType == PlayerController.shotType.Standard) 
				{
					PlayerController.PlayerTwoInstance.AddParticleActiveEffects ();
				}

				IncrementPowerupsInUse (2, true);

				// Switches to triple shot mode
				PlayerController.PlayerTwoInstance.ShotType = PlayerController.shotType.Triple;
				PlayerController.PlayerTwoInstance.CurrentShootingHeatCost = PlayerController.PlayerTwoInstance.TripleShootingHeatCost;

				if (PlayerController.PlayerTwoInstance.isInOverdrive == true) 
				{
					PlayerController.PlayerTwoInstance.TripleShotIteration = PlayerController.shotIteration.Overdrive;
				}

				if (PlayerController.PlayerTwoInstance.isInRapidFire == false) 
				{
					PlayerController.PlayerTwoInstance.CurrentFireRate = PlayerController.PlayerTwoInstance.TripleShotFireRates [0];
				}

				if (PlayerController.PlayerTwoInstance.isInRapidFire == true) 
				{
					PlayerController.PlayerTwoInstance.CurrentFireRate = PlayerController.PlayerTwoInstance.TripleShotFireRates [1];
				}

				// Tweaks to conditions based on which iteration the player is on.
				switch (PlayerController.PlayerTwoInstance.TripleShotIteration) 
				{
				case PlayerController.shotIteration.Standard:
					PlayerController.PlayerTwoInstance.ShotType = PlayerController.shotType.Triple;

					if (GameController.Instance.gameModifier.AlwaysRapidfire == false) 
					{
						PlayerController.PlayerTwoInstance.CurrentFireRate = PlayerController.PlayerTwoInstance.TripleShotFireRates [0];
					}
					break;

				case PlayerController.shotIteration.Enhanced:
					break;

				case PlayerController.shotIteration.Rapid:
					PlayerController.PlayerTwoInstance.CurrentFireRate = PlayerController.PlayerTwoInstance.TripleShotFireRates [1];
					break;

				case PlayerController.shotIteration.Overdrive:
					break;
				}
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
			break;

		case powerups.RippleShot: 

			if (PlayerId == 1) 
			{
				if (PlayerController.PlayerOneInstance.ShotType == PlayerController.shotType.Standard) 
				{
					PlayerController.PlayerOneInstance.AddParticleActiveEffects ();
				}

				IncrementPowerupsInUse (1, true);

				if (PlayerController.PlayerOneInstance.ShotType != PlayerController.shotType.Ripple) 
				{
					// Switches to ripple shot mode.
					PlayerController.PlayerOneInstance.ShotType = PlayerController.shotType.Ripple;
					PlayerController.PlayerOneInstance.CurrentShootingHeatCost = PlayerController.PlayerOneInstance.RippleShootingHeatCost;

					if (PlayerController.PlayerOneInstance.isInOverdrive == true || PlayerController.PlayerOneInstance.isHoming == true)
					{
						PlayerController.PlayerOneInstance.RippleShotIteration = PlayerController.shotIteration.Overdrive;
					}

					if (PlayerController.PlayerOneInstance.isInRapidFire == false) 
					{
						PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.RippleShotFireRates [0];
					}

					if (PlayerController.PlayerOneInstance.isInRapidFire == true) 
					{
						PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.RippleShotFireRates [1];
					}

					// Tweaks to conditions based on which iteration the player is on.
					switch (PlayerController.PlayerOneInstance.RippleShotIteration) 
					{
					case PlayerController.shotIteration.Standard:
						PlayerController.PlayerOneInstance.ShotType = PlayerController.shotType.Ripple;

						if (GameController.Instance.gameModifier.AlwaysRapidfire == false) 
						{
							PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.RippleShotFireRates [0];
						}
						break;

					case PlayerController.shotIteration.Enhanced:
						break;

					case PlayerController.shotIteration.Rapid:
						PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.RippleShotFireRates [1];
						break;

					case PlayerController.shotIteration.Overdrive:
						break;
					}
				}
			}

			if (PlayerId == 2) 
			{
				if (PlayerController.PlayerTwoInstance.ShotType == PlayerController.shotType.Standard) 
				{
					PlayerController.PlayerTwoInstance.AddParticleActiveEffects ();
				}

				IncrementPowerupsInUse (2, true);

				if (PlayerController.PlayerTwoInstance.ShotType != PlayerController.shotType.Ripple) 
				{
					// Switches to ripple shot mode.
					PlayerController.PlayerTwoInstance.ShotType = PlayerController.shotType.Ripple;
					PlayerController.PlayerTwoInstance.CurrentShootingHeatCost = PlayerController.PlayerTwoInstance.RippleShootingHeatCost;

					if (PlayerController.PlayerTwoInstance.isInOverdrive == true || PlayerController.PlayerTwoInstance.isHoming == true)
					{
						PlayerController.PlayerTwoInstance.RippleShotIteration = PlayerController.shotIteration.Overdrive;
					}

					if (PlayerController.PlayerTwoInstance.isInRapidFire == false) 
					{
						PlayerController.PlayerTwoInstance.CurrentFireRate = PlayerController.PlayerTwoInstance.RippleShotFireRates [0];
					}

					if (PlayerController.PlayerTwoInstance.isInRapidFire == true) 
					{
						PlayerController.PlayerTwoInstance.CurrentFireRate = PlayerController.PlayerTwoInstance.RippleShotFireRates [1];
					}

					// Tweaks to conditions based on which iteration the player is on.
					switch (PlayerController.PlayerTwoInstance.RippleShotIteration) 
					{
					case PlayerController.shotIteration.Standard:
						PlayerController.PlayerTwoInstance.ShotType = PlayerController.shotType.Ripple;

						if (GameController.Instance.gameModifier.AlwaysRapidfire == false) 
						{
							PlayerController.PlayerTwoInstance.CurrentFireRate = PlayerController.PlayerTwoInstance.RippleShotFireRates [0];
						}
						break;

					case PlayerController.shotIteration.Enhanced:
						break;

					case PlayerController.shotIteration.Rapid:
						PlayerController.PlayerTwoInstance.CurrentFireRate = PlayerController.PlayerTwoInstance.RippleShotFireRates [1];
						break;

					case PlayerController.shotIteration.Overdrive:
						break;
					}
				}
			}
			SetPowerupTexture (0); // Set the shooting powerup texture.
			break;

		case powerups.Helix:

			if (PlayerId == 1)
			{
				// Adds a helix object that follows the player.
				if (PlayerController.PlayerOneInstance.Helix.activeInHierarchy == false)
				{
					SetPowerupTexture (GameController.Instance.NextPowerupSlot_P1);
					GameController.Instance.NextPowerupSlot_P1 += 1;
					PlayerController.PlayerOneInstance.Helix.SetActive (true);
					PlayerController.PlayerOneInstance.AddParticleActiveEffects ();
					IncrementPowerupsInUse (1, false);
				}
			}

			if (PlayerId == 2)
			{
				// Adds a helix object that follows the player.
				if (PlayerController.PlayerTwoInstance.Helix.activeInHierarchy == false)
				{
					SetPowerupTexture (GameController.Instance.NextPowerupSlot_P1);
					GameController.Instance.NextPowerupSlot_P1 += 1;
					PlayerController.PlayerTwoInstance.Helix.SetActive (true);
					PlayerController.PlayerTwoInstance.AddParticleActiveEffects ();
					IncrementPowerupsInUse (2, false);
				}
			}

			break;

		case powerups.Turret:

			if (PlayerId == 1) 
			{
				// Adds turret to the player and rotates around it.
				if (PlayerController.PlayerOneInstance.nextTurretSpawn < 4) 
				{
					GameObject clone = PlayerController.PlayerOneInstance.Turrets [PlayerController.PlayerOneInstance.nextTurretSpawn];
					clone.SetActive (true);
					clone.GetComponent<Turret> ().playerControllerScript = PlayerController.PlayerOneInstance;

					SetPowerupTexture (GameController.Instance.NextPowerupSlot_P1);

					GameController.Instance.NextPowerupSlot_P1 += 1;
					PlayerController.PlayerOneInstance.nextTurretSpawn += 1;
					PlayerController.PlayerOneInstance.AddParticleActiveEffects ();
					IncrementPowerupsInUse (1, false);
				}
			}

			if (PlayerId == 2) 
			{
				// Adds turret to the player and rotates around it.
				if (PlayerController.PlayerTwoInstance.nextTurretSpawn < 4) 
				{
					GameObject clone = PlayerController.PlayerTwoInstance.Turrets [PlayerController.PlayerTwoInstance.nextTurretSpawn];
					clone.SetActive (true);
					clone.GetComponent<Turret> ().playerControllerScript = PlayerController.PlayerTwoInstance;

					SetPowerupTexture (GameController.Instance.NextPowerupSlot_P1);

					GameController.Instance.NextPowerupSlot_P1 += 1;
					PlayerController.PlayerTwoInstance.nextTurretSpawn += 1;
					PlayerController.PlayerTwoInstance.AddParticleActiveEffects ();
					IncrementPowerupsInUse (2, false);
				}
			}

			break;

		case powerups.Rapidfire:

			if (PlayerId == 1) 
			{
				// Player can shoot faster.
				if (PlayerController.PlayerOneInstance.isInRapidFire == false) 
				{
					switch (PlayerController.PlayerOneInstance.ShotType) 
					{
					case PlayerController.shotType.Standard:
						PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.DoubleShotFireRates [1];
						break;	
					case PlayerController.shotType.Double:
						PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.DoubleShotFireRates [1];
						break;
					case PlayerController.shotType.Triple:
						PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.TripleShotFireRates [1];
						break;
					case PlayerController.shotType.Ripple:
						PlayerController.PlayerOneInstance.CurrentFireRate = PlayerController.PlayerOneInstance.RippleShotFireRates [1];
						break;
					}

					GameController.Instance.RapidfireImage.enabled = true;
					GameController.Instance.RapidfireImage.transform.SetSiblingIndex (-GameController.Instance.NextPowerupShootingSlot_P1 + 3);

					GameController.Instance.NextPowerupShootingSlot_P1 += 1;
					GameController.Instance.RapidfireHex.enabled = true;
					PlayerController.PlayerOneInstance.isInRapidFire = true;
					GameController.Instance.RapidfireImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");
					PlayerController.PlayerOneInstance.AddParticleActiveEffects ();
					IncrementPowerupsInUse (1, false);
				}
			}

			if (PlayerId == 2) 
			{
				// Player can shoot faster.
				if (PlayerController.PlayerTwoInstance.isInRapidFire == false) 
				{
					switch (PlayerController.PlayerTwoInstance.ShotType) 
					{
					case PlayerController.shotType.Standard:
						PlayerController.PlayerTwoInstance.CurrentFireRate = PlayerController.PlayerTwoInstance.DoubleShotFireRates [1];
						break;	
					case PlayerController.shotType.Double:
						PlayerController.PlayerTwoInstance.CurrentFireRate = PlayerController.PlayerTwoInstance.DoubleShotFireRates [1];
						break;
					case PlayerController.shotType.Triple:
						PlayerController.PlayerTwoInstance.CurrentFireRate = PlayerController.PlayerTwoInstance.TripleShotFireRates [1];
						break;
					case PlayerController.shotType.Ripple:
						PlayerController.PlayerTwoInstance.CurrentFireRate = PlayerController.PlayerTwoInstance.RippleShotFireRates [1];
						break;
					}

					GameController.Instance.RapidfireImage.enabled = true;
					GameController.Instance.RapidfireImage.transform.SetSiblingIndex (-GameController.Instance.NextPowerupShootingSlot_P1 + 3);

					GameController.Instance.NextPowerupShootingSlot_P1 += 1;
					GameController.Instance.RapidfireHex.enabled = true;
					PlayerController.PlayerTwoInstance.isInRapidFire = true;
					GameController.Instance.RapidfireImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");
					PlayerController.PlayerTwoInstance.AddParticleActiveEffects ();
					IncrementPowerupsInUse (2, false);
				}
			}

			break;

		case powerups.Overdrive:

			if (PlayerId == 1)
			{
				// Player shoot special bullets which go through any hazard or block.
				if (PlayerController.PlayerOneInstance.isInOverdrive == false) 
				{
					PlayerController.PlayerOneInstance.StandardShotIteration = PlayerController.shotIteration.Overdrive;
					PlayerController.PlayerOneInstance.DoubleShotIteration = PlayerController.shotIteration.Overdrive;
					PlayerController.PlayerOneInstance.TripleShotIteration = PlayerController.shotIteration.Overdrive;
					PlayerController.PlayerOneInstance.RippleShotIteration = PlayerController.shotIteration.Overdrive;
					GameController.Instance.OverdriveImage.enabled = true;
					GameController.Instance.OverdriveImage.transform.SetSiblingIndex (-GameController.Instance.NextPowerupShootingSlot_P1 + 3);
					GameController.Instance.NextPowerupShootingSlot_P1 += 1;
					GameController.Instance.OverdriveHex.enabled = true;
					PlayerController.PlayerOneInstance.isInOverdrive = true;
					GameController.Instance.OverdriveImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");
					PlayerController.PlayerOneInstance.AddParticleActiveEffects ();
					IncrementPowerupsInUse (1, false);
				}
			}

			if (PlayerId == 2)
			{
				// Player shoot special bullets which go through any hazard or block.
				if (PlayerController.PlayerTwoInstance.isInOverdrive == false) 
				{
					PlayerController.PlayerTwoInstance.StandardShotIteration = PlayerController.shotIteration.Overdrive;
					PlayerController.PlayerTwoInstance.DoubleShotIteration = PlayerController.shotIteration.Overdrive;
					PlayerController.PlayerTwoInstance.TripleShotIteration = PlayerController.shotIteration.Overdrive;
					PlayerController.PlayerTwoInstance.RippleShotIteration = PlayerController.shotIteration.Overdrive;
					GameController.Instance.OverdriveImage.enabled = true;
					GameController.Instance.OverdriveImage.transform.SetSiblingIndex (-GameController.Instance.NextPowerupShootingSlot_P1 + 3);
					GameController.Instance.NextPowerupShootingSlot_P1 += 1;
					GameController.Instance.OverdriveHex.enabled = true;
					PlayerController.PlayerTwoInstance.isInOverdrive = true;
					GameController.Instance.OverdriveImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");
					PlayerController.PlayerTwoInstance.AddParticleActiveEffects ();
					IncrementPowerupsInUse (2, false);
				}
			}

			break;

		case powerups.Ricochet:

			if (PlayerId == 1) 
			{
				if (PlayerController.PlayerOneInstance.isRicochet == false) 
				{
					// Player shoots bullets which can bounce off the top and bottom of the screen.
					PlayerController.PlayerOneInstance.EnableRicochetObject ();

					if (PlayerController.PlayerOneInstance.DoubleShotIteration != PlayerController.shotIteration.Overdrive) 
					{
						PlayerController.PlayerOneInstance.DoubleShotIteration = PlayerController.shotIteration.Enhanced;
					}

					if (PlayerController.PlayerOneInstance.TripleShotIteration != PlayerController.shotIteration.Overdrive)
					{
						PlayerController.PlayerOneInstance.TripleShotIteration = PlayerController.shotIteration.Enhanced;
					}

					if (PlayerController.PlayerOneInstance.RippleShotIteration != PlayerController.shotIteration.Overdrive) 
					{
						PlayerController.PlayerOneInstance.RippleShotIteration = PlayerController.shotIteration.Enhanced;
					}

					if (PlayerController.PlayerOneInstance.StandardShotIteration != PlayerController.shotIteration.Overdrive) 
					{
						PlayerController.PlayerOneInstance.StandardShotIteration = PlayerController.shotIteration.Enhanced;
					}

					PlayerController.PlayerOneInstance.isRicochet = true;
					GameController.Instance.RicochetImage.transform.SetSiblingIndex (-GameController.Instance.NextPowerupShootingSlot_P1 + 3);
					GameController.Instance.NextPowerupShootingSlot_P1 += 1;
					GameController.Instance.RicochetImage.enabled = true;
					GameController.Instance.RicochetHex.enabled = true;
					GameController.Instance.RicochetImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");
					PlayerController.PlayerOneInstance.AddParticleActiveEffects ();
					IncrementPowerupsInUse (1, false);
				}
			}

			if (PlayerId == 2) 
			{
				if (PlayerController.PlayerTwoInstance.isRicochet == false) 
				{
					// Player shoots bullets which can bounce off the top and bottom of the screen.
					PlayerController.PlayerTwoInstance.EnableRicochetObject ();

					if (PlayerController.PlayerTwoInstance.DoubleShotIteration != PlayerController.shotIteration.Overdrive) 
					{
						PlayerController.PlayerTwoInstance.DoubleShotIteration = PlayerController.shotIteration.Enhanced;
					}

					if (PlayerController.PlayerTwoInstance.TripleShotIteration != PlayerController.shotIteration.Overdrive)
					{
						PlayerController.PlayerTwoInstance.TripleShotIteration = PlayerController.shotIteration.Enhanced;
					}

					if (PlayerController.PlayerTwoInstance.RippleShotIteration != PlayerController.shotIteration.Overdrive) 
					{
						PlayerController.PlayerTwoInstance.RippleShotIteration = PlayerController.shotIteration.Enhanced;
					}

					if (PlayerController.PlayerTwoInstance.StandardShotIteration != PlayerController.shotIteration.Overdrive) 
					{
						PlayerController.PlayerTwoInstance.StandardShotIteration = PlayerController.shotIteration.Enhanced;
					}

					PlayerController.PlayerTwoInstance.isRicochet = true;
					GameController.Instance.RicochetImage.transform.SetSiblingIndex (-GameController.Instance.NextPowerupShootingSlot_P1 + 3);
					GameController.Instance.NextPowerupShootingSlot_P1 += 1;
					GameController.Instance.RicochetImage.enabled = true;
					GameController.Instance.RicochetHex.enabled = true;
					GameController.Instance.RicochetImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");
					PlayerController.PlayerTwoInstance.AddParticleActiveEffects ();
					IncrementPowerupsInUse (2, false);
				}
			}

			break;
		
		case powerups.Homing:

			if (PlayerId == 1) 
			{
				// Player shoots bullets which home in on blocks.
				if (PlayerController.PlayerOneInstance.isHoming == false)
				{
					PlayerController.PlayerOneInstance.isHoming = true;
					GameController.Instance.HomingImage.transform.SetSiblingIndex (-GameController.Instance.NextPowerupShootingSlot_P1 + 3);
					GameController.Instance.NextPowerupShootingSlot_P1 += 1;
					GameController.Instance.HomingImage.enabled = true;
					GameController.Instance.HomingHex.enabled = true;
					GameController.Instance.HomingImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");
					PlayerController.PlayerOneInstance.AddParticleActiveEffects ();
					IncrementPowerupsInUse (1, false);
				}
			}

			if (PlayerId == 2) 
			{
				// Player shoots bullets which home in on blocks.
				if (PlayerController.PlayerTwoInstance.isHoming == false)
				{
					PlayerController.PlayerTwoInstance.isHoming = true;
					GameController.Instance.HomingImage.transform.SetSiblingIndex (-GameController.Instance.NextPowerupShootingSlot_P1 + 3);
					GameController.Instance.NextPowerupShootingSlot_P1 += 1;
					GameController.Instance.HomingImage.enabled = true;
					GameController.Instance.HomingHex.enabled = true;
					GameController.Instance.HomingImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");
					PlayerController.PlayerTwoInstance.AddParticleActiveEffects ();
					IncrementPowerupsInUse (2, false);
				}
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