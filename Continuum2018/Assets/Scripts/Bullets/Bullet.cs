using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour 
{
	// References.
	public PlayerController playerControllerScript; // Reference to the player controller.
	public FPSCounter fpsCounterScript;

	[Header ("Stats")]
	[Tooltip ("The bullet speed to set (Unscaled).")]
	public float BulletSpeedUnscaled;
	[Tooltip ("The bullet speed to set (Scaled).")]
	public float BulletSpeedScaled = 1300;
	[Tooltip ("Bullet speed based on scaled or unscaled time.")]
	public SpeedType BulletSpeedType;
	public enum SpeedType
	{
		Scaled,
		Unscaled
	}

	[Tooltip ("Minimum and maximum speed of the bullets.")]
	public Vector2 VelocityLimits;
	[Tooltip ("The bullet's Rigidbody.")]
	public Rigidbody BulletRb;
	[Tooltip ("The bullet's Collider.")]
	public Collider BulletCol;
	[Tooltip ("The name of the bullet.")]
	public string BulletTypeName;
	[Tooltip ("Should the bullet have a lifetime?")]
	public bool UseLifetime;
	[Tooltip ("How long the bullet lasts before destroying.")]
	public float Lifetime;
	[Tooltip ("Maximum time before the bullet destroys.")]
	public float MaxLifetime = 30;
	[Tooltip ("Point at which bullet is destroyed passing this height.")]
	public float DestroyMaxYPos = 30;
	[Tooltip ("Point at which bullet's collider is disabled passing this height.")]
	public float ColliderYMaxPos = 12;
	[Tooltip ("Checks if bullet has passed limit.")]
	public bool hasDisappeared = false;
	[Tooltip ("How long after collision the bullet can still be live before being destroyed.")]
	public float DestroyDelayTime = 1;
	[Tooltip ("The Transform component of the Player.")]
	public Transform playerPos;

	public bool hitABlock;

	[Header ("Overdrive")]
	[Tooltip ("Allows the bullet collider to deactivate on collision.")]
	public bool allowBulletColDeactivate = true; 

	[Header ("Ricochet")]
	[Tooltip ("Allows ricochet from walls.")]
	public bool isRicochet;
	[Tooltip ("Point to ricochet Y. (Need to have this dynamic based on screen ratio).")]
	public float RicochetYpos = 11.75f;
	[Tooltip ("Point to ricochet X.")]
	public float RicochetXpos = 21.2f;
	[Tooltip ("Sound to play when ricocheting.")]
	public AudioSource RicochetSound;

	[Header ("Homing")]
	[Tooltip ("Allows bullets to home in on objects.")]
	public bool isHoming;
	[Tooltip ("Referenced homing script.")]
	public Homing homingScript;

	[Header ("Visuals")]
	[Tooltip ("Outer particles of bullet.")]
	public ParticleSystem BulletOuterParticles;
	[Tooltip ("Inner laser like particles.")]
	public ParticleSystem BulletCoreParticles;
	[Tooltip ("Other particles coming from trail.")]
	public ParticleSystem[] TrailParticles;

	[Header ("Audio")]
	[Tooltip ("Sound to play when spawned.")]
	public AudioSource AwakeAudio;

	[Header ("Camera Shake")]
	[Tooltip ("Reference to camera shake.")]
	public CameraShake camShakeScript;
	[Tooltip ("How long to shake camera for.")]
	public float shakeDuration;
	[Tooltip ("Current time for camera shake.")]
	public float shakeTimeRemaining;
	[Tooltip ("How much the camera shakes.")]
	public float shakeAmount;

	[Header ("Player Vibration")]
	[Range (0, 1)]
	[Tooltip ("Vibrates left motor on controller.")]
	public float LeftMotorRumble = 0.2f;
	[Range (0, 1)]
	[Tooltip ("Vibrates right motor on controller.")]
	public float RightMotorRumble = 0.2f;
	[Tooltip ("How long to vibrate for.")]
	public float VibrationDuration = 0.25f; 

	void Awake ()
	{
		Lifetime = 0; // Resets lifetime.
		InvokeRepeating ("CheckForDestroy", 0, 1); // Check if the bullet can be destroyed every second.
	}

	void Start ()
	{
		fpsCounterScript = GameObject.Find ("FPSCounter").GetComponent<FPSCounter> ();
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

		// Only count bullets shot after tutorial is complete.
		if (TutorialManager.Instance.tutorialComplete == true) 
		{
			GameController.Instance.BulletsShot += 1;
		}

		// Bullet is set to homing but not a helix object.
		if (playerControllerScript.isHoming == true && 
			BulletTypeName != ("Helix"))
		{
			homingScript = GetComponent<Homing> (); // Get the homing script.
			isHoming = true; // Set homing to true.
			homingScript.enabled = true; // Enable the homing script.

			if (BulletSpeedType == SpeedType.Unscaled)
			{
				float unscaledVelocity = BulletSpeedUnscaled * 
					((Time.unscaledDeltaTime / Time.timeScale) * fpsCounterScript.FramesPerSec);
				
				// Clamp maximum speed by velocity limits.
				homingScript.speed = Mathf.Clamp (
					unscaledVelocity,
					VelocityLimits.x, 
					VelocityLimits.y
				);
			}
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

		/*
		// When the bullet is not a homing bullet, set the bullet velocity. 
		// Homing bullet's speed is managed by homing script.
		if (isHoming == false)
		{
			SetBulletVelocity ();
		}*/
	}

	void FixedUpdate ()
	{
		CheckForColliderDeactivate (); // Checks if bullet has reached vertical position.
	}

	void OnTriggerEnter (Collider other)
	{
		// Hits a block trigger.
		if (other.tag == "Block") 
		{
			// Vibrate the controller on impact.
			#if !PLATFORM_STANDALONE_OSX && !PLATFORM_ANDROID && !PLATFORM_WEBGL
			playerControllerScript.Vibrate (LeftMotorRumble, RightMotorRumble, VibrationDuration);
			#endif

			// Ricochet if on and is not a helix object.
			if (isRicochet == true && 
				BulletTypeName.Contains ("Helix") == false) 
			{
				Ricochet (true); // Does the ricochet.
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
			//BulletCol.enabled = false; // Turn off the collider.
			hasDisappeared = true; // Set to be in disappeared state.
		}

		/*
		// Stop homing if position greater than this. (Need to make this dynamic based on screen ratio).
		// Homing now deactivates when it ricochets instead.
		if (BulletRb.transform.position.y > 12) 
		{
			if (homingScript != null)
			{
				homingScript.enabled = false;
			}
		}*/

		// Bullet re enters intended space.
		if (BulletRb.transform.position.y <= ColliderYMaxPos && 
			hasDisappeared == true && 
			BulletTypeName != "Helix") 
		{
			hasDisappeared = false; // Set dissapear state to false.
		}
	}

	// Checks for ricochet, if conditions are met, ricochet.
	void CheckForRicochet ()
	{
		if (playerControllerScript.isRicochet == true)
		{
			// Moves to top of screen.
			if (transform.position.y > RicochetYpos) 
			{
				float newZrot = Random.Range (100, 260); // New rotation value.
				transform.rotation = Quaternion.Euler (0, 0, newZrot); // Set new rotation on Z.
				camShakeScript.ShakeCam (shakeAmount, shakeTimeRemaining, 2); // Make some camera shake.
				Ricochet (false);
				SetBulletVelocity ();
			}

			// Moves to bottom of screen.
			if (transform.position.y < -RicochetYpos) 
			{
				float newZrot = Random.Range (-80, 80); // New rotation value.
				transform.rotation = Quaternion.Euler (0, 0, newZrot); // Set new rotation on Z.
				camShakeScript.ShakeCam (shakeAmount, shakeTimeRemaining, 2);  // Make some camera shake.
				Ricochet (false);
				SetBulletVelocity ();
			}

			// Moves to a side of the screen.
			if (transform.position.x > RicochetXpos || transform.position.x < -RicochetXpos) 
			{
				Destroy (gameObject);
				return;
			}
		}
	}

	// Does the ricochet when it bounces off something other than walls.
	void Ricochet (bool newAngle)
	{
		bool playedRicochetSound = false;

		if (homingScript != null) 
		{
			homingScript.enabled = false;
			Invoke ("EnableHoming", 0.05f);
		}

		if (newAngle == true)
		{
			float newZrot = Random.Range (-180, 180); // Give entirely different angle.
			transform.rotation = Quaternion.Euler (0, 0, newZrot); // Set new angle to this.
		}

		if (playedRicochetSound == false && GameController.Instance.isPaused == false)
		{
			RicochetSound.Play (); // Play ricochet sound.
			playedRicochetSound = true;
		}

		camShakeScript.ShakeCam (shakeAmount, shakeTimeRemaining, 2);  // Make some camera shake.
	}

	void EnableHoming ()
	{
		if (homingScript != null && playerControllerScript.isHoming == true) 
		{
			homingScript.enabled = true;
		}
	}

	// Sets bullet to a set velocity
	void SetBulletVelocity ()
	{
		if (GameController.Instance.isPaused == false) 
		{
			// Scale by time scale.
			if (BulletSpeedType == SpeedType.Scaled) 
			{
				BulletRb.velocity = transform.TransformDirection (
					new Vector3 (
						0, 
						Mathf.Clamp (BulletSpeedScaled * Time.fixedUnscaledDeltaTime * (1.5f * Time.timeScale), VelocityLimits.x, VelocityLimits.y), 
						0
					)
				);
			}

			// Assumes time scale is always 1. Should compensate.
			if (BulletSpeedType == SpeedType.Unscaled)
			{
				float unscaledVelocity = BulletSpeedUnscaled * 
					((Time.unscaledDeltaTime / Time.timeScale) * fpsCounterScript.FramesPerSec);

				BulletRb.velocity = transform.TransformDirection (
					new Vector3 (
						0, 
							Mathf.Clamp (unscaledVelocity, VelocityLimits.x, VelocityLimits.y), 
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
		//BulletCol.enabled = false; // Turns of the collider though to not interact with anything else.
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
			//BulletCol.enabled = false;
			StartCoroutine (DestroyDelay ());
		}
	}

	// Destroys immediately.
	public void DestroyObject ()
	{
		if (transform.parent != null && transform.parent.name != "InstantiatedBullets") 
		{
			Destroy (transform.parent.gameObject);
		}

		Destroy (gameObject);
		return;
	}

	void OnDestroy ()
	{
		float TimeFire = Time.time + (playerControllerScript.CurrentFireRate / (playerControllerScript.FireRateTimeMultiplier));

		if (TimeFire >= playerControllerScript.NextFire && hitABlock == true) 
		{
			if (gameObject.name.Contains ("Turret") == false) 
			{
				playerControllerScript.NextFire *= 0.9f;
				//Debug.Log ("Next fire reset.");
			}
			return;
		}
	}
}