using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Block : MonoBehaviour
{
	public PlayerController playerControllerScript_P1;
	public GameController gameControllerScript;
	public TimescaleController timeScaleControllerScript;
	public MeshRenderer rend;

	[Header ("Stats")]
	public float speed;
	public bool OverwriteVelocity;
	private Rigidbody rb;

	public float AquaSpeed;
	public float BlueSpeed;
	public float GreenSpeed;
	public float PurpleSpeed;
	public float PinkSpeed;

	public Material AquaMat;
	public Material BlueMat;
	public Material GreenMat;
	public Material PurpleMat;
	public Material PinkMat;

	public float AquaBasePointValue = 1;
	public float BlueBasePointValue = 2;
	public float GreenBasePointValue = 3;
	public float PurpleBasePointValue = 4;
	public float PinkBasePointValue = 5;

	public GameObject AquaExplosion;
	public GameObject BlueExplosion;
	public GameObject GreenExplosion;
	public GameObject PurpleExplosion;
	public GameObject PinkExplosion;

	public Color AquaTextColor;
	public Color BlueTextColor;
	public Color GreenTextColor;
	public Color PurpleTextColor;
	public Color PinkTextColor;

	[Header ("Block Types")]
	public mainBlockType BlockType;
	public enum mainBlockType
	{
		Aqua = 0,
		Blue = 1,
		Green = 2, 
		Purple = 3, 
		Pink = 4
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
		camShakeScript = GameObject.Find ("CamShake").GetComponent<CameraShake> ();
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
			GetTotalPointValue ();
			CreateExplosion ();

			if (other.GetComponent<Bullet> () != null)
			{
				// Stops the bullet that hit it from hanging around.
				if (other.GetComponent<Bullet> ().allowBulletColDeactivate == true)
				{
					other.GetComponent<Bullet> ().BulletOuterParticles.Stop (true, ParticleSystemStopBehavior.StopEmitting);
					other.GetComponent<Bullet> ().BulletCoreParticles.Stop (true, ParticleSystemStopBehavior.StopEmitting);
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

			if (gameControllerScript.Lives >= 1) 
			{
				gameControllerScript.combo = 1;
				timeScaleControllerScript.OverrideTimeScaleTimeRemaining = 2;
				timeScaleControllerScript.OverridingTimeScale = 0.25f;

				Instantiate (playerExplosion, transform.position, Quaternion.identity);

				playerControllerScript_P1.ResetPowerups ();
				playerControllerScript_P1.playerCol.enabled = false;
				playerControllerScript_P1.playerCol.gameObject.SetActive (false);
				//playerControllerScript_P1.playerMesh.SetActive (false);
				playerControllerScript_P1.PlayerRb.velocity = Vector3.zero;
				playerControllerScript_P1.PlayerFollowRb.velocity = Vector3.zero;
				playerControllerScript_P1.MovementX = 0;
				playerControllerScript_P1.MovementY = 0;
				playerControllerScript_P1.canShoot = false;

				newCamShakeAmount = 0.5f;
				newCamShakeDuration = 1.5f;
				DoCamShake ();
				playerControllerScript_P1.StartCooldown ();

				GameObject[] Blocks = GameObject.FindGameObjectsWithTag ("Block");

				foreach (GameObject block in Blocks) 
				{
					Destroy (block);
				}
			}
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
		// Have to create an instance of the Explosion prefab.
		GameObject _Explosion = Instantiate (Explosion, transform.position, Quaternion.identity);
		_Explosion.GetComponent<Explosion> ().blockScript = this;
		_Explosion.GetComponent<Explosion> ().TextColor = TextColor;
		_Explosion.GetComponent<Explosion> ().Anim ();
	}

	void UpdateBlockType ()
	{
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
			case mainBlockType.Green:
				speed = GreenSpeed;
				rend.material = GreenMat;
				BasePointValue = GreenBasePointValue;
				TextColor = GreenTextColor;
				Explosion = GreenExplosion;
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
