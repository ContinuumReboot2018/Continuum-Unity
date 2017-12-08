using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
	public GameController gameControllerScript;
	public TimescaleController timeScaleControllerScript;
	public float OverwriteTimeDuration = 0.5f;
	public float OverwriteTimeScale = 0.2f;
	public float speed;
	public bool OverwriteVelocity;
	private Rigidbody rb;

	public float points;

	[Header ("Camera Shake")]
	public CameraShake camShakeScript;
	public float newCamShakeDuration = 0.1f;
	public float newCamShakeAmount = 0.1f;

	void Start () 
	{
		gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		timeScaleControllerScript = GameObject.Find ("TimescaleController").GetComponent<TimescaleController> ();
		camShakeScript = GameObject.Find ("CamShake").GetComponent<CameraShake> ();
		rb = GetComponent<Rigidbody> ();
	}

	void Update () 
	{
		if (OverwriteVelocity == false) 
		{
			rb.velocity = new Vector3 (0, speed * Time.deltaTime * Time.timeScale, 0);
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Bullet") 
		{
			other.GetComponent<Bullet> ().BulletOuterParticles.Stop (true, ParticleSystemStopBehavior.StopEmitting);
			other.GetComponent<Bullet> ().StartCoroutine (other.GetComponent<Bullet> ().DestroyDelay ());
			other.GetComponent<Bullet> ().BulletCoreParticles.Stop (true, ParticleSystemStopBehavior.StopEmitting);
			camShakeScript.shakeDuration = newCamShakeDuration;
			camShakeScript.shakeAmount = newCamShakeAmount;
			camShakeScript.Shake ();
			gameControllerScript.TargetScore += points;
			timeScaleControllerScript.OverrideTimeScaleTimeRemaining = OverwriteTimeDuration;
			timeScaleControllerScript.OverridingTimeScale = OverwriteTimeScale;

			Destroy (gameObject);
		}
	}
}
