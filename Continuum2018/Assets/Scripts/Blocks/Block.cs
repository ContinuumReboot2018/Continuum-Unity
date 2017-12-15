using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Block : MonoBehaviour
{
	public PlayerController playerControllerScript;
	public GameController gameControllerScript;
	public TimescaleController timeScaleControllerScript;
	//public float OverwriteTimeDuration = 0.5f;
	//public float OverwriteTimeScale = 0.2f;
	public MeshRenderer rend;

	[Header ("Stats")]
	public float speed;
	public bool OverwriteVelocity;
	private Rigidbody rb;

	public float OrangeSpeed;
	public float YellowSpeed;
	public float GreenSpeed;
	public float BlueSpeed;
	public float PurpleSpeed;

	public Material OrangeMat;
	public Material YellowMat;
	public Material GreenMat;
	public Material BlueMat;
	public Material PurpleMat;

	public float OrangeBasePointValue = 1;
	public float YellowBasePointValue = 2;
	public float GreenBasePointValue = 3;
	public float BlueBasePointValue = 4;
	public float PurpleBasePointValue = 5;

	public GameObject OrangeExplosion;
	public GameObject YellowExplosion;
	public GameObject GreenExplosion;
	public GameObject BlueExplosion;
	public GameObject PurpleExplosion;

	public Color OrangeTextColor;
	public Color YellowTextColor;
	public Color GreenTextColor;
	public Color BlueTextColor;
	public Color PurpleTextColor;

	[Header ("Block Types")]
	public mainBlockType BlockType;
	public enum mainBlockType
	{
		Orange = 0,
		Yellow = 1,
		Green = 2, 
		Blue = 3, 
		Purple = 4
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

	void Start () 
	{
		playerControllerScript = GameObject.Find ("PlayerController").GetComponent<PlayerController> ();
		gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		timeScaleControllerScript = GameObject.Find ("TimescaleController").GetComponent<TimescaleController> ();
		camShakeScript = GameObject.Find ("CamShake").GetComponent<CameraShake> ();
		rb = GetComponent<Rigidbody> ();
		rend = GetComponent<MeshRenderer> ();
		InvokeRepeating ("UpdateBlockType", 0, ChangeRate);
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

			// Stops the bullet that hit it from hanging around.
			other.GetComponent<Bullet> ().BulletOuterParticles.Stop (true, ParticleSystemStopBehavior.StopEmitting);
			other.GetComponent<Bullet> ().BulletCoreParticles.Stop (true, ParticleSystemStopBehavior.StopEmitting);
			other.GetComponent<Bullet> ().StartCoroutine (other.GetComponent<Bullet> ().DestroyDelay ());

			DoCamShake ();

			//timeScaleControllerScript.OverrideTimeScaleTimeRemaining = OverwriteTimeDuration;
			//timeScaleControllerScript.OverridingTimeScale = OverwriteTimeScale;

			Destroy (gameObject);
		}

		if (other.tag == "Player") 
		{
			playerControllerScript.SetCooldownTime (5);

			if (gameControllerScript.Lives >= 1) 
			{
				timeScaleControllerScript.OverrideTimeScaleTimeRemaining = 2;
				timeScaleControllerScript.OverridingTimeScale = 0.25f;

				Instantiate (playerExplosion, transform.position, Quaternion.identity);

				playerControllerScript.playerCol.enabled = false;
				playerControllerScript.playerMesh.SetActive (false);
				playerControllerScript.PlayerRb.velocity = Vector3.zero;
				playerControllerScript.PlayerFollowRb.velocity = Vector3.zero;
				playerControllerScript.MovementX = 0;
				playerControllerScript.MovementY = 0;
				playerControllerScript.canShoot = false;

				newCamShakeAmount = 0.5f;
				newCamShakeDuration = 1.5f;
				DoCamShake ();
				playerControllerScript.StartCooldown ();

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
			case mainBlockType.Orange:
				speed = OrangeSpeed;
				rend.material = OrangeMat;
				BasePointValue = OrangeBasePointValue;
				TextColor = OrangeTextColor;
				Explosion = OrangeExplosion;
				break;
			case mainBlockType.Yellow:
				speed = YellowSpeed;
				rend.material = YellowMat;
				BasePointValue = YellowBasePointValue;
				TextColor = YellowTextColor;
				Explosion = YellowExplosion;
				break;
			case mainBlockType.Green:
				speed = GreenSpeed;
				rend.material = GreenMat;
				BasePointValue = GreenBasePointValue;
				TextColor = GreenTextColor;
				Explosion = GreenExplosion;
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
			}
		}
	}
}
