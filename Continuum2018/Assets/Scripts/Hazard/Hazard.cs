using UnityEngine;

public class Hazard : MonoBehaviour 
{
	public PlayerController playerControllerScript_P1; // Reference to Player Controller.
	public GameController gameControllerScript; // Reference to Game Controller.
	public TimescaleController timeScaleControllerScript; // Reference to Timescale Controller.
	public AudioController audioControllerScript; // Reference to Audio Controller.

	[Header ("Hazard Type")]
	public hazardType HazardType; // Current hazard type.
	public enum hazardType
	{
		Missile
	}

	public GameObject Explosion; // Explosion to play when a bullet or particle collides with it.
	public GameObject playerExplosion; // Explosion to play when the player hits the hazard.

	[Header ("Camera Shake")]
	public CameraShake camShakeScript; // Reference to Camera Shake script.
	public float newCamShakeDuration = 0.1f; // How long to shake for.
	public float newCamShakeAmount = 0.1f; // Strength of the shake.

	public float LowPassTargetFreq = 1500; // Audio frequency for low pass filter.
	public float ResonanceTargetFreq = 1; // Resonance amount for low pass filter.

	void Start () 
	{
		FindReferences (); // Find references.
	}

	void FindReferences ()
	{
		playerControllerScript_P1 = GameObject.Find ("PlayerController_P1").GetComponent<PlayerController> ();
		gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		timeScaleControllerScript = GameObject.Find ("TimescaleController").GetComponent<TimescaleController> ();
		audioControllerScript = GameObject.Find ("AudioController").GetComponent<AudioController> ();
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
				if (particle.GetComponentInParent<Bullet> ().BulletTypeName.Contains ("Ripple") == false) 
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

				if (gameControllerScript.Lives > 1) 
				{
					playerControllerScript_P1.PlayerHazardImpact (this);
					playerControllerScript_P1.PlayerImpactGeneric ();
					DoCamShake ();
					Destroy (gameObject);
					return;
				}

				if (gameControllerScript.Lives == 1) 
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
		#if !PLATFORM_STANDALONE_OSX
		playerControllerScript_P1.Vibrate (0.7f, 0.7f, 0.2f);
		#endif
	}

	// Audio effects.
	public void SetTargetLowPassFreq (float lowPassFreq)
	{
		audioControllerScript.TargetCutoffFreq = lowPassFreq;
	}

	public void SetTargetResonance (float resAmt)
	{
		audioControllerScript.TargetResonance = resAmt;
	}
}
