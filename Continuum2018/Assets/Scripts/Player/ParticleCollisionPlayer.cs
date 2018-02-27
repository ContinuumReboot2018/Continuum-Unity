using UnityEngine;

public class ParticleCollisionPlayer : MonoBehaviour 
{
	public PlayerController playerControllerScript_P1;
	public GameController gameControllerScript;
	public TimescaleController timeScaleControllerScript;
	public AudioController audioControllerScript; 
	public CameraShake camShakeScript;

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

			if (gameControllerScript.Lives > 1) 
			{
				playerControllerScript_P1.ImpactPoint = gameObject.transform.position;
				playerControllerScript_P1.StartCoroutine (playerControllerScript_P1.UseEmp ());
				playerControllerScript_P1.ResetPowerups ();
				playerControllerScript_P1.playerCol.enabled = false;
				playerControllerScript_P1.playerTrigger.enabled = false;
				playerControllerScript_P1.playerCol.gameObject.SetActive (false);
				playerControllerScript_P1.playerTrigger.gameObject.SetActive (false);
				playerControllerScript_P1.PlayerGuides.transform.position = Vector3.zero;
				playerControllerScript_P1.PlayerGuides.SetActive (false);
				playerControllerScript_P1.AbilityUI.transform.position = Vector3.zero;
				playerControllerScript_P1.AbilityUI.SetActive (false);
				playerControllerScript_P1.PlayerRb.velocity = Vector3.zero;
				playerControllerScript_P1.PlayerFollowRb.velocity = Vector3.zero;
				playerControllerScript_P1.MovementX = 0;
				playerControllerScript_P1.MovementY = 0;
				playerControllerScript_P1.canShoot = false;
				playerControllerScript_P1.StartCooldown ();
				playerControllerScript_P1.PlayerExplosionParticles.transform.position = gameObject.transform.position;
				playerControllerScript_P1.PlayerExplosionParticles.Play ();
				playerControllerScript_P1.PlayerExplosionAudio.Play ();

				gameControllerScript.combo = 1;
				timeScaleControllerScript.OverrideTimeScaleTimeRemaining = 2;
				timeScaleControllerScript.OverridingTimeScale = 0.25f;

				Instantiate (playerExplosion, transform.position, Quaternion.identity);
				DoCamShake ();
				SetTargetLowPassFreq (LowPassTargetFreq);
				SetTargetResonance (ResonanceTargetFreq);
			}

			// Make game over when lives run out.
			if (gameControllerScript.Lives == 1) 
			{
				playerControllerScript_P1.GameOver ();
			}
		}
	}

	void DoCamShake ()
	{
		camShakeScript.ShakeCam (newCamShakeAmount * 10, newCamShakeDuration * 2, 9);
		#if !PLATFORM_STANDALONE_OSX
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