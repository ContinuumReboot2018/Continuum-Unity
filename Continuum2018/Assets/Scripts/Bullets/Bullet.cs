using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour 
{
	public PlayerController playerControllerScript;
	public float BulletSpeed;
	public Rigidbody BulletRb;

	[Header ("Stats")]
	public string BulletTypeName;
	public float Lifetime;
	public float MaxLifetime = 30;
	public float DestroyMaxYPos = 30;
	public float ColliderYMaxPos = 12;
	public Collider BulletCol;
	public float DestroyDelayTime = 1;
	public Transform playerPos;
	public bool movedEnough;
	public Vector2 VelocityLimits;
	public bool allowBulletColDeactivate = true;

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
		//BulletCol.enabled = false;
		//movedEnough = false;
		Lifetime = 0;
		InvokeRepeating ("CheckForDestroy", 0, 1);
	}

	void Start ()
	{
		if (playerControllerScript == null) 
		{
			playerControllerScript = GameObject.Find ("PlayerController_P1").GetComponent<PlayerController>();
		}

		camShakeScript = GameObject.Find ("CamShake").GetComponent<CameraShake> ();
		StartCameraShake ();
		CheckBulletIteration ();
	}

	void FixedUpdate ()
	{
		BulletRb.velocity = transform.InverseTransformDirection 
		(
				new Vector3 
				(
					0, 
					Mathf.Clamp(BulletSpeed * Time.fixedUnscaledDeltaTime * (4 * Time.timeScale), VelocityLimits.x, VelocityLimits.y), 
					0
				)
		);
		
		Lifetime += Time.unscaledDeltaTime;
		CheckForDestroy ();
		CheckForColliderDeactivate ();
		CheckColActivate ();
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Block") 
		{
			playerControllerScript.Vibrate (LeftMotorRumble, RightMotorRumble, VibrationDuration);
		}
	}

	void CheckForColliderDeactivate ()
	{
		if (BulletRb.transform.position.y > ColliderYMaxPos) 
		{
			BulletCol.enabled = false;
		}
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
		// Current shake strength is less than this shake strength. 
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
		}
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
		if (BulletTypeName.Contains ("Overdrive") == false) 
		{
			BulletOuterParticles.Stop (true, ParticleSystemStopBehavior.StopEmitting);
			BulletCoreParticles.Stop (true, ParticleSystemStopBehavior.StopEmitting);
			StopEmittingTrail ();
			BulletCol.enabled = false;
			StartCoroutine (DestroyDelay ());
		}
	}
}
