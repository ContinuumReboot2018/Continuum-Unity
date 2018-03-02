using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour 
{
	// References.
	public PlayerController playerControllerScript; // Reference to the player controller.
	public GameController gameControllerScript; // Reference to the game controller.

	[Header ("Stats")]
	public float BulletSpeed; // The bullet speed to set.
	public SpeedType BulletSpeedType; // Bullet speed based on scaled or unscaled time.
	public enum SpeedType
	{
		Scaled,
		Unscaled
	}
	public Vector2 VelocityLimits; // Minimum and maximum speed of the bullets.
	public Rigidbody BulletRb; // The bullet's Rigidbody.
	public Collider BulletCol; // The bullet's Collider.
	public string BulletTypeName; // The name of the bullet.
	public bool UseLifetime; // Should the bullet have a lifetime?
	public float Lifetime; // How long the bullet lasts before destroying.
	public float MaxLifetime = 30; // Maximum time before the bullet destroys.
	public float DestroyMaxYPos = 30; // Point at which bullet is destroyed passing this height.
	public float ColliderYMaxPos = 12; // Point at which bullet's collider is disabled passing this height.
	public bool hasDisappeared = false; // Checks if bullet has passed limit.

	public float DestroyDelayTime = 1; // How long after collision the bullet can still be live before being destroyed.
	public Transform playerPos; // The Transform component of the Player.

	[Header ("Overdrive")]
	public bool allowBulletColDeactivate = true; // Allows the bullet collider to deactivate on collision.

	[Header ("Ricochet")]
	public bool isRicochet; // Allows ricochet from walls.
	public float RicochetYpos = 11.75f; // Point to ricochet Y. (Need to have this dynamic based on screen ratio.)
	public float RicochetXpos = 21.2f;  // Point to ricochet X.
	public AudioSource RicochetSound; // Sound to play when ricocheting.

	[Header ("Homing")]
	public bool isHoming; // Allows bullets to home in on objects.
	public Homing homingScript; // Referenced homing script.

	[Header ("Visuals")]
	public ParticleSystem BulletOuterParticles; // Outer particles of bullet.
	public ParticleSystem BulletCoreParticles; // Inner laser like particles.
	public ParticleSystem[] TrailParticles; // Other particles coming from trail.

	[Header ("Audio")]
	public AudioSource AwakeAudio; // Sound to play when spawned.

	[Header ("Camera Shake")]
	public CameraShake camShakeScript; // Reference to camera shake.
	public float shakeDuration; // How long to shake camera for.
	public float shakeTimeRemaining; // Current time for camera shake.
	public float shakeAmount; // How much the camera shakes.

	[Header ("Player Vibration")]
	[Range (0, 1)]
	public float LeftMotorRumble = 0.2f; // Vibrates left motor on controller
	[Range (0, 1)]
	public float RightMotorRumble = 0.2f; // Vibrates right motor on controller.
	public float VibrationDuration = 0.25f; // How long to vibrate for.

	void Awake ()
	{
		Lifetime = 0; // Resets lifetime.
		InvokeRepeating ("CheckForDestroy", 0, 1); // Check if the bullet can be destroyed every second.
	}

	void Start ()
	{
		AwakeAudio = GetComponent<AudioSource> (); // Gets current attached audio source.
		AwakeAudio.panStereo = 0.04f * transform.position.x; // Pans audio based on x position.

		// Finds the player controller.
		if (playerControllerScript == null) 
		{
			playerControllerScript = GameObject.Find ("PlayerController_P1").GetComponent<PlayerController>();
		}
			
		// Sets ricochet variables.
		isRicochet = playerControllerScript.isRicochet;

		if (isRicochet) 
		{
			RicochetSound = GameObject.Find ("RicochetSound").GetComponent<AudioSource> (); // Finds ricochet sound.
		}

		// Find the game controller.
		if (gameControllerScript == null) 
		{
			gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		}

		// Only count bullets shot after tutorial is complete.
		if (playerControllerScript.tutorialManagerScript.tutorialComplete == true) 
		{
			gameControllerScript.BulletsShot += 1;
		}

		// Bullet is set to homing but not a helix object.
		if (playerControllerScript.isHoming == true && 
			BulletTypeName != ("Helix"))
		{
			homingScript = GetComponent<Homing> (); // Get the homing script.
			isHoming = true; // Set homing to true.
			homingScript.enabled = true; // Enable the homing script.

			// Clamp maximum speed by velocity limits.
			homingScript.speed = Mathf.Clamp (
				BulletSpeed * Time.fixedUnscaledDeltaTime * (4 * Time.timeScale), 
				VelocityLimits.x, 
				VelocityLimits.y
			);
		}

		camShakeScript = GameObject.Find ("CamShake").GetComponent<CameraShake> (); // Find the camera shake.
		StartCameraShake (); // Give initial camera shake.
		CheckBulletIteration (); // Checks the iteration of the bullet.
		SetBulletVelocity ();
	}

	void Update ()
	{
		// Increase lifetime if allowed.
		if (UseLifetime == true)
		{
			Lifetime += Time.unscaledDeltaTime;
		}

		// Checks for ricochet as long as it's not a helix object.
		if (isRicochet == true && BulletTypeName.Contains ("Helix") == false) 
		{
			CheckForRicochet ();
		}
	}

	void FixedUpdate ()
	{
		if (isHoming == false)
		{
			SetBulletVelocity ();
		}

		CheckForColliderDeactivate (); // Checks if bullet has reached vertical position.
	}

	void OnTriggerEnter (Collider other)
	{
		// Hits a block trigger.
		if (other.tag == "Block") 
		{
			// Vibrate the controller on impact.
			#if !PLATFORM_STANDALONE_OSX
			playerControllerScript.Vibrate (LeftMotorRumble, RightMotorRumble, VibrationDuration);
			#endif

			// Ricochet if on and is not a helix object.
			if (isRicochet == true && 
				BulletTypeName.Contains ("Helix") == false) 
			{
				Ricochet (); // Does the ricochet.
				camShakeScript.ShakeCam (shakeAmount, shakeTimeRemaining, 2); // Provides camera shake.
			}
		}
	}

	// Deactivates collider if conditions are not met.
	void CheckForColliderDeactivate ()
	{
		// Position is greater than intended position and hasn't disappeared yet.
		if (BulletRb.transform.position.y > ColliderYMaxPos && 
			hasDisappeared == false) 
		{
			BulletCol.enabled = false; // Turn off the collider.
			hasDisappeared = true; // Set to be in disappeared state.
		}

		// Stop homing if position greater than this. (Need to make this dynamic based on screen ratio).
		if (BulletRb.transform.position.y > 10) 
		{
			if (homingScript != null)
			{
				homingScript.enabled = false;
			}
		}

		// Bullet re enters intended space.
		if (BulletRb.transform.position.y <= ColliderYMaxPos && 
			hasDisappeared == true && 
			BulletTypeName != "Helix") 
		{
			BulletCol.enabled = true; // Enable the collider again.
			hasDisappeared = false; // Set dissapear state to false.
		}
	}

	// Checks for ricochet, if conditions are met, ricochet.
	void CheckForRicochet ()
	{
		if (playerControllerScript.isRicochet == true && playerControllerScript.isHoming == false) 
		{
			// Moves to top of screen.
			if (transform.position.y > RicochetYpos) 
			{
				float newZrot = Random.Range (100, 260); // New rotation value.
				transform.rotation = Quaternion.Euler (0, 0, newZrot); // Set new rotation on Z.
				RicochetSound.Play (); // Play ricochet sound.
				camShakeScript.ShakeCam (shakeAmount, shakeTimeRemaining, 2); // Make some camera shake.
			}

			// Moves to bottom of screen.
			if (transform.position.y < -RicochetYpos) 
			{
				float newZrot = Random.Range (-80, 80); // New rotation value.
				transform.rotation = Quaternion.Euler (0, 0, newZrot); // Set new rotation on Z.
				RicochetSound.Play (); // Play ricochet sound.
				camShakeScript.ShakeCam (shakeAmount, shakeTimeRemaining, 2);  // Make some camera shake.
			}

			// Moves to right of screen.
			if (transform.position.x > RicochetXpos) 
			{
				//float newZrot = Random.Range (10, 170);
				//transform.rotation = Quaternion.Euler (0, 0, newZrot);
				//RicochetSound.Play ();
				Destroy (gameObject);
				return;
			}

			// Moves to left of screen.
			if (transform.position.x < -RicochetXpos) 
			{
				//float newZrot = Random.Range (-170, -10);
				//transform.rotation = Quaternion.Euler (0, 0, newZrot);
				//RicochetSound.Play ();
				Destroy (gameObject);
				return;
			}
		}
	}

	// Does the ricochet when it bounces off something other than walls.
	void Ricochet ()
	{
		float newZrot = Random.Range (-180, 180); // Give entirely different angle.
		transform.rotation = Quaternion.Euler (0, 0, newZrot); // Set new angle to this.
		RicochetSound.Play (); // Play ricochet sound.
		camShakeScript.ShakeCam (shakeAmount, shakeTimeRemaining, 2);  // Make some camera shake.
	}

	// Sets bullet to a set velocity
	void SetBulletVelocity ()
	{
		if (gameControllerScript.isPaused == false) 
		{
			// Scale by time scale.
			if (BulletSpeedType == SpeedType.Scaled) 
			{
				BulletRb.velocity = transform.TransformDirection 
			//BulletRb.velocity = transform.InverseTransformDirection 
			(
					new Vector3 (
						0, 
						Mathf.Clamp (BulletSpeed * Time.fixedUnscaledDeltaTime * (4 * Time.timeScale), VelocityLimits.x, VelocityLimits.y), 
						0
					)
				);
			}

			// Assumes time scale is always 1. Should compensate.
			if (BulletSpeedType == SpeedType.Unscaled)
			{
				BulletRb.velocity = transform.TransformDirection 
				//BulletRb.velocity = transform.InverseTransformDirection 
				(
					new Vector3 (
						0, 
						Mathf.Clamp (BulletSpeed * Time.fixedUnscaledDeltaTime * 4, VelocityLimits.x, VelocityLimits.y), 
						0
					)
				);
			}
		}
	}

	// If any condition here is met, bullet get destroyed.
	void CheckForDestroy ()
	{
		// Exceeds lifetime.
		if (Lifetime > MaxLifetime) 
		{
			Destroy (gameObject);
			return;
		}

		// Y position too high.
		if (BulletRb.transform.position.y > DestroyMaxYPos) 
		{
			Destroy (gameObject);
			return;
		}

		// Destroy bullet if vertical position is below -30.
		if (transform.position.y < -30) 
		{
			Destroy (gameObject);
			return;
		}
	}

	// Checks what iteration the bullet is on and sets if the bullet's collider can be deactivated.
	void CheckBulletIteration ()
	{
		if (BulletTypeName.Contains ("DoubleShot") || 
			BulletTypeName.Contains ("TripleShot")) 
		{
			switch (playerControllerScript.DoubleShotIteration)
			{
			case PlayerController.shotIteration.Standard:
				allowBulletColDeactivate = true;
				break;
			case PlayerController.shotIteration.Enhanced:
				allowBulletColDeactivate = true;
				break;
			case PlayerController.shotIteration.Rapid:
				allowBulletColDeactivate = true;
				break;
			case PlayerController.shotIteration.Overdrive:
				allowBulletColDeactivate = false;
				break;
			}
		}
	}

	// Makes the camera shake based on time, strengthm and priority.
	void StartCameraShake ()
	{
		camShakeScript.ShakeCam (shakeAmount, shakeTimeRemaining, 2);
	}

	// Allows the bullet to be active some more time after a collision.
	public IEnumerator DestroyDelay ()
	{
		BulletCol.enabled = false; // Turns of the collider though to not interact with anything else.
		yield return new WaitForSecondsRealtime (DestroyDelayTime);
		Destroy (gameObject);
	}

	// When bullet has been collided, stop emitting trail particles but don't clear them.
	public void StopEmittingTrail ()
	{
		// Finds all particles in the array and stops them.
		foreach (ParticleSystem particletrail in TrailParticles) 
		{
			particletrail.Stop (true, ParticleSystemStopBehavior.StopEmitting);
		}
	}

	// Hits a block, stops emitting particles and set to destroy with delay.
	public void BlockHit ()
	{
		if (BulletTypeName.Contains ("Overdrive") == false && BulletTypeName.Contains ("Helix") == false) 
		{
			BulletOuterParticles.Stop (true, ParticleSystemStopBehavior.StopEmitting);
			BulletCoreParticles.Stop (true, ParticleSystemStopBehavior.StopEmitting);
			StopEmittingTrail ();
			BulletCol.enabled = false;
			StartCoroutine (DestroyDelay ());
		}
	}

	// Destroys immediately.
	public void DestroyObject ()
	{
		Destroy (gameObject);
		return;
	}
}