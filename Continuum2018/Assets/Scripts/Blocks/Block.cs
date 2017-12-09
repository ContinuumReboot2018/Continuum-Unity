using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Block : MonoBehaviour
{
	public GameController gameControllerScript;
	public TimescaleController timeScaleControllerScript;
	//public float OverwriteTimeDuration = 0.5f;
	//public float OverwriteTimeScale = 0.2f;
	public float speed;
	public bool OverwriteVelocity;
	private Rigidbody rb;

	public float BasePointValue;
	public float totalPointValue;
	public GameObject Explosion;
	public Color TextColor;
	public float MinYPos = -15.0f;

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

		if (transform.position.y < MinYPos) 
		{
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Bullet") 
		{
			//GetTotalPointValue ();
			CreateExplosion ();

			// Stops the bullet that hit it from hanging around.
			other.GetComponent<Bullet> ().BulletOuterParticles.Stop (true, ParticleSystemStopBehavior.StopEmitting);
			other.GetComponent<Bullet> ().BulletCoreParticles.Stop (true, ParticleSystemStopBehavior.StopEmitting);
			other.GetComponent<Bullet> ().StartCoroutine (other.GetComponent<Bullet> ().DestroyDelay ());

			DoCamShake ();

			//timeScaleControllerScript.OverrideTimeScaleTimeRemaining = OverwriteTimeDuration;
			//timeScaleControllerScript.OverridingTimeScale = OverwriteTimeScale;

			Destroy (gameObject);
		}
	}

	public void GetTotalPointValue ()
	{
		// Calculates total point value based on current combo from game controller and time scale.
		totalPointValue = Mathf.Clamp (BasePointValue * gameControllerScript.combo * Time.timeScale, 1, 10000);
	}

	public void RefreshCombo ()
	{
		// Adds point value to target score in game controller and plays animation.
		gameControllerScript.TargetScore += totalPointValue;
		gameControllerScript.ScoreAnim.Play ("ScorePoints");

		// Adds to next combo.
		gameControllerScript.combo += 2;

		// Resets combo time.
		gameControllerScript.comboTimeRemaining = gameControllerScript.comboDuration;
	}

	void DoCamShake ()
	{
		camShakeScript.shakeDuration = newCamShakeDuration;
		camShakeScript.shakeAmount = newCamShakeAmount;
		camShakeScript.Shake ();
	}

	void CreateExplosion ()
	{
		Instantiate (Explosion, transform.position, Quaternion.identity);
		Explosion.GetComponent<Explosion> ().blockScript = this;
		Explosion.GetComponent<Explosion> ().TextColor = TextColor;
		Explosion.GetComponent<Explosion> ().Anim ();
	}
}
