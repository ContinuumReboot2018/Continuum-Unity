using UnityEngine;

public class ParticleCollisionPlayer : MonoBehaviour 
{
	public PlayerController playerControllerScript_P1; // Reference to Player Controller.
	public GameController gameControllerScript; // Reference to Game Controller.
	public TimescaleController timeScaleControllerScript; // Reference to Timescale Controller.
	public AudioController audioControllerScript;  // Reference to Audio Controller.
	public CameraShake camShakeScript; // Reference to camera shake.
	[Space (10)]
	[Tooltip ("Player explosion.")]
	public GameObject playerExplosion;

	[Header ("Camera shake")]
	public float newCamShakeAmount;
	public float newCamShakeDuration;

	[Header ("Audio")]
	public float LowPassTargetFreq = 1500;
	public float ResonanceTargetFreq = 1;

	// Player collides with particles.
	void OnParticleCollision (GameObject particle)
	{
		// Red particles.
		if (particle.name.Contains ("Red") == true) 
		{
			playerControllerScript_P1.SetCooldownTime (5);

			if (GameController.Instance.Lives > 1) 
			{
				playerControllerScript_P1.PlayerImpactGeneric ();
				Instantiate (playerExplosion, transform.position, Quaternion.identity);
				DoCamShake ();
				SetTargetLowPassFreq (LowPassTargetFreq);
				SetTargetResonance (ResonanceTargetFreq);
			}

			// Make game over when lives run out.
			if (GameController.Instance.Lives == 1) 
			{
				playerControllerScript_P1.GameOver ();
			}
		}
	}

	void DoCamShake ()
	{
		camShakeScript.ShakeCam (newCamShakeAmount, newCamShakeDuration, 9);
		#if !PLATFORM_STANDALONE_OSX && !PLATFORM_ANDROID && !PLATFORM_WEBGL
		playerControllerScript_P1.Vibrate (0.7f, 0.7f, 0.2f);
		#endif
	}

	// Set the low pass filter cutoff frequency.
	void SetTargetLowPassFreq (float lowPassFreq)
	{
		audioControllerScript.TargetCutoffFreq = lowPassFreq;
	}

	// Set the low pass filter resonance.
	void SetTargetResonance (float resAmt)
	{
		audioControllerScript.TargetResonance = resAmt;
	}
}