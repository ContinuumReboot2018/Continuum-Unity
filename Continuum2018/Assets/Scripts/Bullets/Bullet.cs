using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour 
{
	public float BulletSpeed;
	public Rigidbody BulletRb;

	public Transform DestroyPos;
	public float Lifetime;
	public float MaxLifetime = 30;

	public float ColliderYMaxPos = 12;
	public Collider BulletCol;
	public float DestroyDelayTime = 1;

	[Header ("Visuals")]
	public ParticleSystem BulletOuterParticles;
	public ParticleSystem BulletCoreParticles;

	[Header ("Camera Shake")]
	public CameraShake camShakeScript;
	public float shakeDuration;
	public float shakeTimeRemaining;
	public float shakeAmount;

	void Start ()
	{
		
		camShakeScript = GameObject.Find ("CamShake").GetComponent<CameraShake> ();
		StartCameraShake ();
		Lifetime = 0;
		InvokeRepeating ("CheckForDestroy", 0, 1);
	}

	void Update ()
	{
		BulletRb.velocity = transform.InverseTransformDirection (
				new Vector3 (
					0, 
					BulletSpeed * Time.deltaTime * Time.timeScale, 
					0
				));
		
		Lifetime += Time.unscaledDeltaTime;
		CheckForDestroy ();

		CheckForColliderDeactivate ();
	}

	void OnTriggerEnter (Collider other)
	{
		
	}

	void CheckForColliderDeactivate ()
	{
		if (BulletRb.transform.position.y > ColliderYMaxPos) 
		{
			BulletCol.enabled = false;
		}
	}

	void CheckForDestroy ()
	{
		if (Lifetime > MaxLifetime) 
		{
			Destroy (gameObject);
		}

		if (BulletRb.transform.position.y > DestroyPos.position.y) 
		{
			Destroy (gameObject);
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
