﻿using UnityEngine;

public class Hazard : MonoBehaviour 
{
	public PlayerController playerControllerScript_P1; // Reference to Player Controller.

	[Header ("Hazard Type")]
	[Tooltip ("Current hazard type.")]
	public hazardType HazardType;
	public enum hazardType
	{
		Missile
	}

	[Tooltip ("Explosion to play when a bullet or particle collides with it.")]
	public GameObject Explosion;
	[Tooltip ("Explosion to play when the player hits the hazard.")]
	public GameObject playerExplosion;

	[Header ("Camera Shake")]
	[Tooltip ("Reference to Camera Shake script.")]
	public CameraShake camShakeScript;
	[Tooltip ("How long to shake for.")]
	public float newCamShakeDuration = 0.1f;
	[Tooltip ("Strength of the shake.")]
	public float newCamShakeAmount = 0.1f;

	[Header ("Audio")]
	[Tooltip ("Audio frequency for low pass filter.")]
	public float LowPassTargetFreq = 1500;
	[Tooltip ("Resonance amount for low pass filter.")]
	public float ResonanceTargetFreq = 1;

	void Start () 
	{
		FindReferences (); // Find references.
	}

	void FindReferences ()
	{
		playerControllerScript_P1 = GameObject.Find ("PlayerController_P1").GetComponent<PlayerController> ();
		camShakeScript = GameObject.Find ("CamShake").GetComponent<CameraShake> ();
	}

	// Collisions with particles.
	void OnParticleCollision (GameObject particle)
	{
		if (particle.tag == "Bullet") 
		{
			CreateExplosion ();
			DoCamShake ();

			if (particle.GetComponentInParent<Bullet> () != null)
			{
				if (particle.GetComponentInParent<Bullet> ().BulletTypeName.Contains ("Ripple") ||
				    particle.GetComponentInParent<Bullet> ().BulletTypeName.Contains ("Helix")) 
				{
					Destroy (gameObject);
					return;
				}

				else
					
				{
					Destroy (particle.gameObject);
				}
			}

			Destroy (gameObject);
			return;
		}
	}

	// Trigger collisions with objects.
	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Bullet") 
		{
			CreateExplosion ();
			DoCamShake ();
			Destroy (gameObject);
			return;
		}

		if (other.tag == "Player") 
		{
			if (HazardType == hazardType.Missile) 
			{
				playerControllerScript_P1.SetCooldownTime (5);

				if (GameController.Instance.Lives > 1) 
				{
					playerControllerScript_P1.PlayerHazardImpact (this);
					playerControllerScript_P1.PlayerImpactGeneric ();
					DoCamShake ();
					Destroy (gameObject);
					return;
				}

				if (GameController.Instance.Lives == 1) 
				{
					playerControllerScript_P1.GameOver ();
				}
			}
		}
	}

	// Explosion when destroyed.
	void CreateExplosion ()
	{
		Instantiate (Explosion, transform.position, Quaternion.identity);
	}

	void DoCamShake ()
	{
		#if !PLATFORM_STANDALONE_OSX && !PLATFORM_ANDROID && !PLATFORM_WEBGL
		playerControllerScript_P1.Vibrate (0.7f, 0.7f, 0.2f);
		#endif
	}

	// Audio effects.
	public void SetTargetLowPassFreq (float lowPassFreq)
	{
		AudioController.Instance.TargetCutoffFreq = lowPassFreq;
	}

	public void SetTargetResonance (float resAmt)
	{
		AudioController.Instance.TargetResonance = resAmt;
	}
}
