using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PowerupPickup : MonoBehaviour 
{
	public GameController gameControllerScript; // Reference to Game Controller.
	public PlayerController playerControllerScript_P1; // Reference to Player Controller.
	public TimescaleController timescaleControllerScript; // Reference to Timescale Controller.
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
		gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		playerControllerScript_P1 = GameObject.Find ("PlayerController_P1").GetComponent<PlayerController> ();
		timescaleControllerScript = GameObject.Find ("TimescaleController").GetComponent<TimescaleController> ();
		PowerupTimeRunningOutAudio = GameObject.Find ("PowerupRunningOutSound").GetComponent<AudioSource> ();
		camShakeScript = GameObject.Find ("CamShake").GetComponent<CameraShake> ();
		destroyByTimeScript = GetComponent<DestroyByTime> (); // Timer for lifetime.
		anim = GetComponent<Animator> (); // The animator which scales in size.

		StartCoroutine (ShowPowerup ()); // Start sequence to animate the powerup pickup to be visible.
		StartCoroutine (DestroyAnimation ()); // Start sequence to animate the powerup to disappear.
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
			timescaleControllerScript.OverrideTimeScaleTimeRemaining += 0.5f; // Set Timescale ovveride time.
			timescaleControllerScript.OverridingTimeScale = 0.2f; // Set overriding time scale.
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
		GameObject powerupPickupUI = Instantiate (gameControllerScript.PowerupPickupUI, transform.position, Quaternion.identity);
		powerupPickupUI.GetComponentInChildren<RawImage> ().texture = PowerupTexture;
		powerupPickupUI.GetComponentInChildren<RawImage> ().color = PowerupPickupUIColor;
	}
		
	// Finds powerup from list and activates it.
	void ActivatePowerup_P1 ()
	{
		Instantiate (CollectExplosion, transform.position, Quaternion.identity); // Creates powerup explosion particles.
		gameControllerScript.SetPowerupTime (PowerupTime);

		switch (ThisPowerup) 
		{

		case powerups.FreezeTime:
			timescaleControllerScript.OverridingTimeScale = 0.3f;
			timescaleControllerScript.OverrideTimeScaleTimeRemaining += PowerupTime;
			timescaleControllerScript.isOverridingTimeScale = true;

			if (playerControllerScript_P1.timeIsSlowed == false) 
			{
				playerControllerScript_P1.timeIsSlowed = true;
				SetPowerupTexture (gameControllerScript.NextPowerupSlot_P1);
				gameControllerScript.NextPowerupSlot_P1 += 1;
			}
			break;

		case powerups.DoubleShot: 

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

				if (gameControllerScript.gameModifier.AlwaysRapidfire == false)
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

				if (gameControllerScript.gameModifier.AlwaysRapidfire == false)
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
			gameControllerScript.Lives += 1;

			if (gameControllerScript.Lives < gameControllerScript.MaxLives)
			{
				gameControllerScript.MaxLivesText.text = "";
			}

			if (gameControllerScript.Lives >= gameControllerScript.MaxLives)
			{
				gameControllerScript.MaxLivesText.text = "MAX";
				Debug.Log ("Reached maximum lives.");
			}
				
			// Caps maximum lives.
			gameControllerScript.Lives = Mathf.Clamp (gameControllerScript.Lives, 0, gameControllerScript.MaxLives);

			gameControllerScript.UpdateLives (); // Updates lives UI.
			break;

		case powerups.RippleShot: 
			
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

				if (gameControllerScript.gameModifier.AlwaysRapidfire == false)
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
			break;

		case powerups.Helix:

			// Adds a helix object that follows the player.
			if (playerControllerScript_P1.Helix.activeInHierarchy == false) 
			{
				SetPowerupTexture (gameControllerScript.NextPowerupSlot_P1);
				gameControllerScript.NextPowerupSlot_P1 += 1;
				playerControllerScript_P1.Helix.SetActive (true);
			}

			break;

		case powerups.Turret:

			// Adds turret to the player and rotates around it.
			if (playerControllerScript_P1.nextTurretSpawn < 4) 
			{
				GameObject clone = playerControllerScript_P1.Turrets [playerControllerScript_P1.nextTurretSpawn];
				clone.SetActive (true);
				clone.GetComponent<Turret> ().playerControllerScript = playerControllerScript_P1;

				SetPowerupTexture (gameControllerScript.NextPowerupSlot_P1);

				gameControllerScript.NextPowerupSlot_P1 += 1;
				playerControllerScript_P1.nextTurretSpawn += 1;
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

				gameControllerScript.RapidfireImage.enabled = true;
				gameControllerScript.RapidfireImage.transform.SetSiblingIndex (-gameControllerScript.NextPowerupShootingSlot_P1 + 3);

				gameControllerScript.NextPowerupShootingSlot_P1 += 1;
				gameControllerScript.RapidfireHex.enabled = true;
				playerControllerScript_P1.isInRapidFire = true;
				gameControllerScript.RapidfireImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");
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
				gameControllerScript.OverdriveImage.enabled = true;
				gameControllerScript.OverdriveImage.transform.SetSiblingIndex (-gameControllerScript.NextPowerupShootingSlot_P1 + 3);
				gameControllerScript.NextPowerupShootingSlot_P1 += 1;
				gameControllerScript.OverdriveHex.enabled = true;
				playerControllerScript_P1.isInOverdrive = true;
				gameControllerScript.OverdriveImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");
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
				gameControllerScript.RicochetImage.transform.SetSiblingIndex (-gameControllerScript.NextPowerupShootingSlot_P1 + 3);
				gameControllerScript.NextPowerupShootingSlot_P1 += 1;
				gameControllerScript.RicochetImage.enabled = true;
				gameControllerScript.RicochetHex.enabled = true;
				gameControllerScript.RicochetImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");
			}
			break;
		
		case powerups.Homing:
			
			// Player shoots bullets which home in on blocks.
			if (playerControllerScript_P1.isHoming == false) 
			{
				playerControllerScript_P1.isHoming = true;
				gameControllerScript.HomingImage.transform.SetSiblingIndex (-gameControllerScript.NextPowerupShootingSlot_P1 + 3);
				gameControllerScript.NextPowerupShootingSlot_P1 += 1;
				gameControllerScript.HomingImage.enabled = true;
				gameControllerScript.HomingHex.enabled = true;
				gameControllerScript.HomingImage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");
			}
			break;
		}

		Destroy (gameObject);
		return;
	}

	void SetPowerupTexture (int index)
	{
		gameControllerScript.PowerupImage_P1 [index].gameObject.SetActive (true);
		gameControllerScript.PowerupImage_P1 [index].texture = PowerupTexture;
		gameControllerScript.PowerupImage_P1 [index].color = new Color (1, 1, 1, 1);
		gameControllerScript.PowerupImage_P1 [index].gameObject.GetComponent<Animator> ().Play ("PowerupListItemPopIn");
	}
}