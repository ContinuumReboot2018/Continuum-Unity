using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour 
{
	public PlayerController playerControllerScript_P1;
	public GameController gameControllerScript;
	public TimescaleController timeScaleControllerScript;
	public AudioController audioControllerScript;
	public hazardType HazardType;
	public enum hazardType
	{
		Missile
	}

	public GameObject Explosion;
	public GameObject playerExplosion;

	[Header ("Camera Shake")]
	public CameraShake camShakeScript;
	public float newCamShakeDuration = 0.1f;
	public float newCamShakeAmount = 0.1f;

	public float LowPassTargetFreq = 1500;
	public float ResonanceTargetFreq = 1;

	void Start () 
	{
		playerControllerScript_P1 = GameObject.Find ("PlayerController_P1").GetComponent<PlayerController> ();
		gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		timeScaleControllerScript = GameObject.Find ("TimescaleController").GetComponent<TimescaleController> ();
		audioControllerScript = GameObject.Find ("AudioController").GetComponent<AudioController> ();
		camShakeScript = GameObject.Find ("CamShake").GetComponent<CameraShake> ();
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Bullet") 
		{
			CreateExplosion ();
			DoCamShake ();
			Destroy (gameObject);
		}

		if (other.tag == "Player") 
		{
			if (HazardType == hazardType.Missile) 
			{
				playerControllerScript_P1.SetCooldownTime (5);

				if (gameControllerScript.Lives > 1) 
				{
					SetTargetLowPassFreq (LowPassTargetFreq);
					SetTargetResonance (ResonanceTargetFreq);

					gameControllerScript.combo = 1;

					timeScaleControllerScript.OverrideTimeScaleTimeRemaining = 2;
					timeScaleControllerScript.OverridingTimeScale = 0.25f;

					Instantiate (playerExplosion, transform.position, Quaternion.identity);

					playerControllerScript_P1.ResetPowerups ();
					playerControllerScript_P1.playerCol.enabled = false;
					playerControllerScript_P1.playerCol.gameObject.SetActive (false);
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

					Destroy (gameObject, 0.52f);
				}

				if (gameControllerScript.Lives == 1) 
				{
					playerControllerScript_P1.GameOver ();
				}
			}
		}
	}

	void CreateExplosion ()
	{
		Instantiate (Explosion, transform.position, Quaternion.identity);
	}

	void DestroyAllBlocks ()
	{
		GameObject[] Blocks = GameObject.FindGameObjectsWithTag ("Block");
		GameObject[] PowerupPickups = GameObject.FindGameObjectsWithTag ("PowerupPickup");

		foreach (GameObject block in Blocks) 
		{
			if (block.GetComponent<Block> () != null)
			{
				if (block.GetComponent<Block> ().isBossPart == false)
				{
					Destroy (block);
				}
			}
		}

		foreach (GameObject powerupPickup in PowerupPickups) 
		{
			Destroy (powerupPickup);
		}
	}

	void DoCamShake ()
	{
		camShakeScript.shakeDuration = newCamShakeDuration;
		camShakeScript.shakeAmount = newCamShakeAmount;
		camShakeScript.Shake ();
		playerControllerScript_P1.Vibrate (0.7f, 0.7f, 0.2f);
	}

	void SetTargetLowPassFreq (float lowPassFreq)
	{
		audioControllerScript.TargetCutoffFreq = lowPassFreq;
	}

	void SetTargetResonance (float resAmt)
	{
		audioControllerScript.TargetResonance = resAmt;
	}
}
