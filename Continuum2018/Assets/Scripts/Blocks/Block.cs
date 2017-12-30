using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Block : MonoBehaviour
{
	public PlayerController playerControllerScript_P1;
	public GameController gameControllerScript;
	public TimescaleController timeScaleControllerScript;
	public AudioController audioControllerScript;
	public MeshRenderer rend;
	public ScrollTextureOverTime textureScrollScript;
	public bool isBossPart = false;
	public bool GotDetached;
	public bool Stackable;

	[Header ("Stats")]
	public float speed;
	public bool OverwriteVelocity;
	private Rigidbody rb;

	public float AquaSpeed;
	public float BlueSpeed;
	public float PurpleSpeed;
	public float PinkSpeed;

	public Material AquaMat;
	public Material BlueMat;
	public Material PurpleMat;
	public Material PinkMat;

	public float AquaBasePointValue   = 1;
	public float BlueBasePointValue   = 2;
	public float PurpleBasePointValue = 3;
	public float PinkBasePointValue   = 4;

	public GameObject AquaExplosion;
	public GameObject BlueExplosion;
	public GameObject PurpleExplosion;
	public GameObject PinkExplosion;

	public Color AquaTextColor;
	public Color BlueTextColor;
	public Color PurpleTextColor;
	public Color PinkTextColor;

	[Header ("Block Types")]
	public mainBlockType BlockType;
	public enum mainBlockType
	{
		Aqua   = 0,
		Blue   = 1,
		Purple = 2, 
		Pink   = 3
	}

	public float ChangeRate;
	public blockChangeType BlockChangeType;
	public enum blockChangeType
	{
		Static = 0,
		Sequential = 1,
		Random = 2,
	}

	public bool isSpecialBlockType = false;

	public specialBlockType SpecialBlockType;
	public enum specialBlockType
	{
		Noise = 0,
		Red = 1
	}

	public Material noiseMat;
	public GameObject NoiseExplosion;

	[Header ("Explosion Combo")]
	public GameObject Explosion;
	public float BasePointValue;
	public float totalPointValue;

	public Color TextColor;
	public float MinYPos = -15.0f;

	public GameObject playerExplosion;

	[Header ("Camera Shake")]
	public CameraShake camShakeScript;
	public float newCamShakeDuration = 0.1f;
	public float newCamShakeAmount = 0.1f;

	public float LowPassTargetFreq = 1500;
	public float ResonanceTargetFreq = 1;

	void Awake ()
	{
		rb = GetComponent<Rigidbody> ();
		rend = GetComponent<MeshRenderer> ();
		InvokeRepeating ("UpdateBlockType", 0, ChangeRate);
	}

	void Start () 
	{
		playerControllerScript_P1 = GameObject.Find ("PlayerController_P1").GetComponent<PlayerController> ();
		gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		timeScaleControllerScript = GameObject.Find ("TimescaleController").GetComponent<TimescaleController> ();
		audioControllerScript = GameObject.Find ("AudioController").GetComponent<AudioController> ();
		camShakeScript = GameObject.Find ("CamShake").GetComponent<CameraShake> ();
		if (textureScrollScript == null) 
		{
			textureScrollScript = GetComponent<ScrollTextureOverTime> ();
		}
	}

	void Update ()
	{
		if (isBossPart == true && transform.parent == null && GotDetached == false) 
		{
			textureScrollScript.enabled = true;
			isSpecialBlockType = true;
			SpecialBlockType = specialBlockType.Noise;
			speed = 0;
			rend.material = noiseMat;
			BasePointValue = 0;
			TextColor = new Color (0.5f, 0.5f, 0.5f, 1);
			Explosion = NoiseExplosion;
			transform.localScale = new Vector3 
				(
					0.75f * transform.localScale.x,
					0.75f * transform.localScale.y,
					0.75f * transform.localScale.z
				);
			GotDetached = true;
		}
	}

	void FixedUpdate () 
	{
		if (OverwriteVelocity == false && isBossPart == false) 
		{
			rb.velocity = new Vector3 (0, speed * Time.fixedUnscaledDeltaTime * Time.timeScale, 0);
		}

		if (transform.position.y < MinYPos) 
		{
			Destroy (gameObject);
		}

		if (isBossPart == true) 
		{
			rb.velocity = Vector3.zero;
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Bullet") 
		{
			GetTotalPointValue ();
			CreateExplosion ();

			if (other.GetComponent<Bullet> () != null)
			{
				// Stops the bullet that hit it from hanging around.
				if (other.GetComponent<Bullet> ().allowBulletColDeactivate == true)
				{
					other.GetComponent<Bullet> ().BulletOuterParticles.Stop (true, ParticleSystemStopBehavior.StopEmitting);
					other.GetComponent<Bullet> ().BulletCoreParticles.Stop (true, ParticleSystemStopBehavior.StopEmitting);
					other.GetComponent<Bullet> ().BulletCol.enabled = false;
					other.GetComponent<Bullet> ().StartCoroutine (other.GetComponent<Bullet> ().DestroyDelay ());
				}
			}

			DoCamShake ();

			//timeScaleControllerScript.OverrideTimeScaleTimeRemaining = OverwriteTimeDuration;
			//timeScaleControllerScript.OverridingTimeScale = OverwriteTimeScale;

			Destroy (gameObject);
		}

		if (other.tag == "Player") 
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

	public void GetTotalPointValue ()
	{
		// Calculates total point value based on current combo from game controller and time scale.
		totalPointValue = Mathf.Clamp (BasePointValue * gameControllerScript.combo * Time.timeScale, 1, 10000);
	}

	public void RefreshCombo ()
	{
		// Adds point value to target score in game controller and plays animation.
		gameControllerScript.TargetScore += totalPointValue;

		if (gameControllerScript.ScoreAnim.GetCurrentAnimatorStateInfo (0).IsName ("ScoreFadeOut") == false) 
		{
			gameControllerScript.ScoreAnim.Play ("ScorePoints");
		}

		// Adds to next combo.
		if (gameControllerScript.combo > 1)
		{
			gameControllerScript.combo += 2;
		}

		if (gameControllerScript.combo <= 1) 
		{
			gameControllerScript.combo += 1;
		}

		// Resets combo time.
		gameControllerScript.comboTimeRemaining = gameControllerScript.comboDuration;
	}

	void SetTargetLowPassFreq (float lowPassFreq)
	{
		audioControllerScript.TargetCutoffFreq = lowPassFreq;
	}

	void SetTargetResonance (float resAmt)
	{
		audioControllerScript.TargetResonance = resAmt;
	}

	void DoCamShake ()
	{
		camShakeScript.shakeDuration = newCamShakeDuration;
		camShakeScript.shakeAmount = newCamShakeAmount;
		camShakeScript.Shake ();
		playerControllerScript_P1.Vibrate (0.7f, 0.7f, 0.2f);
	}

	void CreateExplosion ()
	{
		// Have to create an instance of the Explosion prefab.
		GameObject _Explosion = Instantiate (Explosion, transform.position, Quaternion.identity);
		_Explosion.GetComponent<Explosion> ().blockScript = this;
		_Explosion.GetComponent<Explosion> ().TextColor = TextColor;
		_Explosion.GetComponent<Explosion> ().Anim ();
	}

	void UpdateBlockType ()
	{
		if (isSpecialBlockType == true) 
		{
			switch (SpecialBlockType) 
			{
			case specialBlockType.Noise:
				speed = 0;
				rend.material = noiseMat;
				BasePointValue = 0;
				TextColor = new Color (0.5f, 0.5f, 0.5f, 1);
				Explosion = NoiseExplosion;
				break;
			}
		}

		if (isSpecialBlockType == false) 
		{
			// Update speed stat.
			// Update material stat.
			// Update base point value stat.
			// Update change explosion text color reference stat.
			switch (BlockType) 
			{
			case mainBlockType.Aqua:
				speed = AquaSpeed;
				rend.material = AquaMat;
				BasePointValue = AquaBasePointValue;
				TextColor = AquaTextColor;
				Explosion = AquaExplosion;
				break;
			case mainBlockType.Blue:
				speed = BlueSpeed;
				rend.material = BlueMat;
				BasePointValue = BlueBasePointValue;
				TextColor = BlueTextColor;
				Explosion = BlueExplosion;
				break;
			case mainBlockType.Purple:
				speed = PurpleSpeed;
				rend.material = PurpleMat;
				BasePointValue = PurpleBasePointValue;
				TextColor = PurpleTextColor;
				Explosion = PurpleExplosion;
				break;
			case mainBlockType.Pink:
				speed = PinkSpeed;
				rend.material = PinkMat;
				BasePointValue = PinkBasePointValue;
				TextColor = PinkTextColor;
				Explosion = PinkExplosion;
				break;
			}
		}
	}
}
