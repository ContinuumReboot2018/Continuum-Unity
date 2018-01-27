using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour 
{
	public PlayerController playerControllerScript;
	public float BulletSpeed;
	public Vector2 VelocityLimits;
	public Rigidbody BulletRb;
	public Collider BulletCol;

	[Header ("Stats")]
	public string BulletTypeName;
	public bool UseLifetime;
	public float Lifetime;
	public float MaxLifetime = 30;
	public float DestroyMaxYPos = 30;
	public float ColliderYMaxPos = 12;

	public float DestroyDelayTime = 1;
	public Transform playerPos;
	public bool movedEnough;

	[Header ("Overdrive")]
	public bool allowBulletColDeactivate = true;

	[Header ("Ricochet")]
	public bool isRicochet;
	public float RicochetYpos = 11.75f;
	public float RicochetXpos = 21.2f; 
	public AudioSource RicochetSound;

	[Header ("Homing")]
	public bool isHoming;
	public Transform HomingPoint;

	[Header ("Visuals")]
	public ParticleSystem BulletOuterParticles;
	public ParticleSystem BulletCoreParticles;
	public ParticleSystem[] TrailParticles;

	[Header ("Audio")]
	public AudioSource AwakeAudio;

	[Header ("Camera Shake")]
	public CameraShake camShakeScript;
	public float shakeDuration;
	public float shakeTimeRemaining;
	public float shakeAmount;

	[Header ("Player Vibration")]
	public float LeftMotorRumble = 0.2f;
	public float RightMotorRumble = 0.2f;
	public float VibrationDuration = 0.25f;

	void Awake ()
	{
		AwakeAudio = GetComponent<AudioSource> ();
		AwakeAudio.panStereo = 0.04f * transform.position.x;
		Lifetime = 0;
		InvokeRepeating ("CheckForDestroy", 0, 1);
	}

	void Start ()
	{
		if (playerControllerScript == null) 
		{
			playerControllerScript = GameObject.Find ("PlayerController_P1").GetComponent<PlayerController>();
		}

		if (BulletTypeName != "Ripple")
		{
			isRicochet = playerControllerScript.isRicochet;
		}

		if (isRicochet) 
		{
			RicochetSound = GameObject.Find ("RicochetSound").GetComponent<AudioSource> ();
		}

		camShakeScript = GameObject.Find ("CamShake").GetComponent<CameraShake> ();
		StartCameraShake ();
		CheckBulletIteration ();
	}

	void Update ()
	{
		if (isRicochet == true && BulletTypeName.Contains ("Ripple") == false && BulletTypeName.Contains ("Helix") == false) 
		{
			CheckForRicochet ();
		}

		if (UseLifetime == true)
		{
			Lifetime += Time.unscaledDeltaTime;
		}

		if (isHoming == true) 
		{
			MoveTowardsHomingObject ();
		}
	}

	void FixedUpdate ()
	{
		if (isHoming == false)
		{
			SetBulletVelocity ();
		}

		CheckForDestroy ();
		CheckForColliderDeactivate ();
		CheckColActivate ();
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Block") 
		{
			playerControllerScript.Vibrate (LeftMotorRumble, RightMotorRumble, VibrationDuration);

			if (isRicochet == true && BulletTypeName.Contains ("Ripple") == false && BulletTypeName.Contains ("Helix") == false) 
			{
				Ricochet ();
				camShakeScript.ShakeCam (shakeAmount, shakeTimeRemaining, 2);
			}
		}
	}

	void CheckForColliderDeactivate ()
	{
		if (BulletRb.transform.position.y > ColliderYMaxPos) 
		{
			BulletCol.enabled = false;
		}
	}

	void CheckForRicochet ()
	{
		// Moves to top of screen.
		if (transform.position.y > RicochetYpos) 
		{
			float newZrot = Random.Range (100, 260);
			transform.rotation = Quaternion.Euler (0, 0, newZrot);
			RicochetSound.Play ();
			camShakeScript.ShakeCam (shakeAmount, shakeTimeRemaining, 2);
		}

		// Moves to bottom of screen.
		if (transform.position.y < -RicochetYpos) 
		{
			float newZrot = Random.Range (-80, 80);
			transform.rotation = Quaternion.Euler (0, 0, newZrot);
			RicochetSound.Play ();
			camShakeScript.ShakeCam (shakeAmount, shakeTimeRemaining, 2);
		}

		// Moves to right of screen.
		if (transform.position.x > RicochetXpos) 
		{
			//float newZrot = Random.Range (10, 170);
			//transform.rotation = Quaternion.Euler (0, 0, newZrot);
			//RicochetSound.Play ();
			Destroy (gameObject);
		}

		// Moves to left of screen.
		if (transform.position.x < -RicochetXpos) 
		{
			//float newZrot = Random.Range (-170, -10);
			//transform.rotation = Quaternion.Euler (0, 0, newZrot);
			//RicochetSound.Play ();
			Destroy (gameObject);
		}
	}

	void Ricochet ()
	{
		float newZrot = Random.Range (-180, 180);
		transform.rotation = Quaternion.Euler (0, 0, newZrot);
		RicochetSound.Play ();
		camShakeScript.ShakeCam (shakeAmount, shakeTimeRemaining, 2);
	}

	void MoveTowardsHomingObject ()
	{
		if (HomingPoint != null) 
		{
			transform.position = Vector3.MoveTowards (transform.position, HomingPoint.position, BulletSpeed * Time.deltaTime);
		} else 
		{
			SetBulletVelocity ();
		}
	}

	void SetBulletVelocity ()
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

	void CheckColActivate ()
	{
		/*
		if (movedEnough == false) 
		{
			//Debug.Log (Vector3.Distance(transform.position, playerPos.position));
			if (Vector3.Distance(transform.position, playerPos.position) < 0.75f)
			{
				BulletCol.enabled = false;
			}

			if (Vector3.Distance(transform.position, playerPos.position) >= 0.75f)
			{
				BulletCol.enabled = true;
				movedEnough = true;
			}
		}*/
	}

	void CheckForDestroy ()
	{
		if (Lifetime > MaxLifetime) 
		{
			Destroy (gameObject);
		}

		if (BulletRb.transform.position.y > DestroyMaxYPos) 
		{
			Destroy (gameObject);
		}
	}

	void CheckBulletIteration ()
	{
		if (BulletTypeName.Contains ("DoubleShot")) 
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

		if (BulletTypeName.Contains ("TripleShot")) 
		{
			switch (playerControllerScript.TripleShotIteration)
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

	void StartCameraShake ()
	{
		camShakeScript.ShakeCam (shakeAmount, shakeTimeRemaining, 2);

		/*// Current shake strength is less than this shake strength. 
		if (camShakeScript.shakeAmount < shakeAmount) 
		{
			// Current shake time remaining is less than this shake time remain.
			if (camShakeScript.shakeTimeRemaining <= shakeTimeRemaining) 
			{
				// Give it the new shake amounts.
				camShakeScript.shakeAmount = shakeAmount; 
				camShakeScript.shakeDuration = shakeDuration;
				camShakeScript.shakeTimeRemaining = shakeTimeRemaining;
			}
		}

		if (camShakeScript.shakeAmount > shakeAmount) 
		{
			if (camShakeScript.shakeTimeRemaining <= shakeTimeRemaining) 
			{
				//camShakeScript.shakeAmount = shakeAmount;
				//camShakeScript.shakeDuration = shakeDuration;
				//camShakeScript.shakeTimeRemaining = shakeTimeRemaining;
			}
		}*/
	}

	public IEnumerator DestroyDelay ()
	{
		BulletCol.enabled = false;
		yield return new WaitForSecondsRealtime (DestroyDelayTime);
		Destroy (gameObject);
	}

	public void StopEmittingTrail ()
	{
		foreach (ParticleSystem particletrail in TrailParticles) 
		{
			particletrail.Stop (true, ParticleSystemStopBehavior.StopEmitting);
		}
	}

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

	public void DestroyObject ()
	{
		Destroy (gameObject);
	}
}
