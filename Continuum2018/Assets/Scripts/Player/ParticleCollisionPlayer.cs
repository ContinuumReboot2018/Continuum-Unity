using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollisionPlayer : MonoBehaviour {

	public PlayerController playerControllerScript_P1;
	public GameController gameControllerScript;
	public TimescaleController timeScaleControllerScript;
	public CameraShake camShakeScript;

	public GameObject playerExplosion;

	public float newCamShakeAmount;
	public float newCamShakeDuration;

	void OnParticleCollision (GameObject particle)
	{
		if (particle.name.Contains ("Red") == true) 
		{
			playerControllerScript_P1.SetCooldownTime (5);

			if (gameControllerScript.Lives > 1) 
			{
				//SetTargetLowPassFreq (LowPassTargetFreq);
				//SetTargetResonance (ResonanceTargetFreq);
				gameControllerScript.combo = 1;
				timeScaleControllerScript.OverrideTimeScaleTimeRemaining = 2;
				timeScaleControllerScript.OverridingTimeScale = 0.25f;

				Instantiate (playerExplosion, transform.position, Quaternion.identity);

				playerControllerScript_P1.ResetPowerups ();
				playerControllerScript_P1.playerCol.enabled = false;
				playerControllerScript_P1.playerCol.gameObject.SetActive (false);
				playerControllerScript_P1.PlayerGuides.transform.position = Vector3.zero;
				playerControllerScript_P1.PlayerGuides.SetActive (false);
				playerControllerScript_P1.AbilityUI.transform.position = Vector3.zero;
				playerControllerScript_P1.AbilityUI.SetActive (false);
				playerControllerScript_P1.PlayerRb.velocity = Vector3.zero;
				playerControllerScript_P1.PlayerFollowRb.velocity = Vector3.zero;
				playerControllerScript_P1.MovementX = 0;
				playerControllerScript_P1.MovementY = 0;
				playerControllerScript_P1.canShoot = false;

				newCamShakeAmount = 0.5f;
				newCamShakeDuration = 1.5f;
				DoCamShake ();
				playerControllerScript_P1.StartCooldown ();
				playerControllerScript_P1.PlayerExplosionParticles.transform.position = gameObject.transform.position;
				playerControllerScript_P1.PlayerExplosionParticles.Play ();
				playerControllerScript_P1.PlayerExplosionAudio.Play ();

				Invoke ("DestroyAllBlocks", 0.5f);
			}

			if (gameControllerScript.Lives == 1) 
			{
				playerControllerScript_P1.GameOver ();
			}
		}
	}

	void DoCamShake ()
	{
		camShakeScript.ShakeCam (newCamShakeAmount, newCamShakeDuration, 1);
		//camShakeScript.shakeDuration = newCamShakeDuration;
		//camShakeScript.shakeAmount = newCamShakeAmount;
		//camShakeScript.Shake ();
		playerControllerScript_P1.Vibrate (0.7f, 0.7f, 0.2f);
	}
}
