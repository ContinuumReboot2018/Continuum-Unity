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
		BulletCol.enabled = false;
		movedEnough = false;
		Lifetime = 0;
		InvokeRepeating ("CheckForDestroy", 0, 1);
	}

	void Start ()
	{
		camShakeScript = GameObject.Find ("CamShake").GetComponent<CameraShake> ();
		StartCameraShake ();
		CheckBulletIteration ();
	}

	void FixedUpdate ()
	{
		BulletRb.velocity = transform.InverseTransformDirection (
				new Vector3 (
					0, 
				Mathf.Clamp(BulletSpeed * Time.fixedDeltaTime * (6 * Time.timeScale), VelocityLimits.x, VelocityLimits.y), 
					0
				));
		
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
		}
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
		switch (BulletTypeName) 
		{
		case "DoubleShot":
			switch (playerControllerScript.DoubleShotIteration)
			{
			case PlayerController.doubleShotIteration.Standard:
				allowBulletColDeactivate = true;
				break;
			case PlayerController.doubleShotIteration.Enhanced:
				allowBulletColDeactivate = true;
				break;
			case PlayerController.doubleShotIteration.Faster:
				allowBulletColDeactivate = true;
				break;
			case PlayerController.doubleShotIteration.Overdrive:
				allowBulletColDeactivate = false;
				break;
			}
			break;
		}
	}

	void StartCameraShake ()
	{
		if (camShakeScript.shakeDuration < shakeDuration)
		{
			camShakeScript.shakeDuration = shakeDuration;
		}

		if (camShakeScript.shakeTimeRemaining < shakeTimeRemaining)
		{
			camShakeScript.shakeTimeRemaining = shakeTimeRemaining;
		}
			
		camShakeScript.shakeAmount = shakeAmount;
	}

	public IEnumerator DestroyDelay ()
	{
		BulletCol.enabled = false;
		yield return new WaitForSecondsRealtime (DestroyDelayTime);
		Destroy (gameObject);
	}
}
