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

	[Header ("Current Stats")]
	public float speed;
	public bool OverwriteVelocity;
	public bool DontIncrementBlocksDestroyed;
	public bool Stackable;
	public bool isStacked;
	public StackZone stack;
	public bool wasCollided;
	public bool isBossPart = false;
	public bool GotDetached;

	private Rigidbody rb;
	private Collider BoxCol;

	[Header ("Boundary")]
	public Vector2 BoundaryX;
	public Vector2 BoundaryY;

	[Header ("All block stats")]
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
		RandomOnce = 2,
		RandomScroll = 3
	}

	public bool isSpecialBlockType = false;

	public specialBlockType SpecialBlockType;
	public enum specialBlockType
	{
		Noise = 0,
		Red = 1,
		Tutorial = 2
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

	[Header ("Block Formation")]
	public BlockFormation blockFormationScript;
	public bool isBlockFormationConnected;

	[Header ("Camera Shake")]
	public CameraShake camShakeScript;
	public float newCamShakeDuration = 0.1f;
	public float newCamShakeAmount = 0.1f;

	public float LowPassTargetFreq = 1500;
	public float ResonanceTargetFreq = 1;

	[Header ("Tutorial")]
	public bool isTutorialBlock;
	public TutorialManager tutorialManagerScript;
	public int tutorialBlockIndex;

	void Awake ()
	{
		BoxCol = GetComponent<Collider> ();
		rb = GetComponent<Rigidbody> ();
		rend = GetComponent<MeshRenderer> ();


		if (blockFormationScript == null) 
		{
			isBlockFormationConnected = false;
		}

		if (blockFormationScript != null) 
		{
			isBlockFormationConnected = true;
		}
			
		if (isBlockFormationConnected == true) 
		{
			OverwriteVelocity = true;
			rb.velocity = Vector3.zero;
		}

		if (BlockChangeType == blockChangeType.RandomScroll) 
		{
			InvokeRepeating ("RandomScroll", 0, ChangeRate);
			InvokeRepeating ("UpdateBlockType", 0, ChangeRate);
		}

		if (BlockChangeType == blockChangeType.Sequential) 
		{
			InvokeRepeating ("SequentialScroll", 0, ChangeRate);
			InvokeRepeating ("UpdateBlockType", 0, ChangeRate);
		}

		if (BlockChangeType == blockChangeType.Static) 
		{
			UpdateBlockType ();
		}

		if (BlockChangeType == blockChangeType.RandomOnce) 
		{
			BlockType = (mainBlockType)(Random.Range (0, 4));
			UpdateBlockType ();
		}
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

			transform.rotation = Quaternion.identity;
			GotDetached = true;
		}

		if (transform.position.y > 60) 
		{
			
		}

		if (transform.position.y <= 60) 
		{
			BoxCol.enabled = true;
		}
	}

	void FixedUpdate () 
	{
		if (OverwriteVelocity == false && isBossPart == false) 
		{
			//rb.velocity = new Vector3 (0, speed * Time.fixedUnscaledDeltaTime * Time.timeScale, 0);
			rb.velocity = new Vector3 (0, speed * Time.fixedDeltaTime * Time.timeScale, 0);
		}

		if (transform.position.y < MinYPos) 
		{
			Destroy (gameObject);
			return;
		}

		if (isBossPart == true) 
		{
			rb.velocity = Vector3.zero;

			if (transform.position.y > 11.5f && GotDetached == true) 
			{
				Destroy (gameObject);
				return;
			}
		}
	}

	void OnParticleCollision (GameObject particle)
	{
		BoxCol.enabled = false;
		GetTotalPointValue ();
		CreateExplosion ();
		DoCamShake ();
		IncrementBlocksDestroyed ();
		Destroy (gameObject);
		return;
	}

	void OnTriggerEnter(Collider other)
	{
		BoxCol.enabled = false;

		if (other.gameObject.name.Contains ("Missile")) 
		{
			if (isBossPart == false) 
			{
				GetTotalPointValue ();
				CreateExplosion ();
				IncrementBlocksDestroyed ();
				DoCamShake ();

				if (other.tag != "Bullet") 
				{
					Destroy (other.gameObject);
					Destroy (gameObject);
					return;
				}
			}
		}

		if (other.tag == "Bullet") 
		{
			if (tutorialManagerScript != null)
			{
				if (tutorialManagerScript.TutorialPhase != TutorialManager.tutorialPhase.Info)
				{
					tutorialManagerScript.Blocks [tutorialBlockIndex] = null;
				}

				if (tutorialManagerScript.TutorialPhase == TutorialManager.tutorialPhase.Info) 
				{
					Debug.Log ("Attempted to turn off tutorial.");
					tutorialManagerScript.TurnOffTutorial ();
				}
			}

			GetTotalPointValue ();
			CreateExplosion ();
			IncrementBlocksDestroyed ();

			if (other.GetComponent<Bullet> () != null)
			{
				// Stops the bullet that hit it from hanging around.
				if (other.GetComponent<Bullet> ().allowBulletColDeactivate == true)
				{
					other.GetComponent<Bullet> ().BlockHit ();
				}
			}

			DoCamShake ();
			Destroy (gameObject);
			return;
		}

		if (other.tag == "Player" && isTutorialBlock == false) 
		{
			if (gameControllerScript.Lives > 1) 
			{
				playerControllerScript_P1.PlayerBlockImpact (this);
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

	void IncrementBlocksDestroyed ()
	{
		if (DontIncrementBlocksDestroyed == false) 
		{
			gameControllerScript.BlocksDestroyed += 1;
		}
	}

	public void GetTotalPointValue ()
	{
		// Calculates total point value based on current combo from game controller and time scale.
		if (isTutorialBlock == false)
		{
			totalPointValue = Mathf.Clamp ((BasePointValue + gameControllerScript.Wave) * gameControllerScript.combo * Time.timeScale, 1, 10000);
		}

		if (isTutorialBlock == true) 
		{
			totalPointValue = BasePointValue;
		}
	}

	public void RefreshCombo ()
	{
		// Adds point value to target score in game controller and plays animation.
		gameControllerScript.TargetScore += totalPointValue;
		playerControllerScript_P1.ScoreAnim.Play ("ScorePoints");

		if (playerControllerScript_P1.ScoreAnim.GetCurrentAnimatorStateInfo (0).IsName ("ScoreFadeOut") == false) 
		{
			playerControllerScript_P1.ScoreAnim.Play ("ScorePoints");
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
		camShakeScript.ShakeCam (newCamShakeAmount, newCamShakeDuration, 2);
		#if !PLATFORM_STANDALONE_OSX
		playerControllerScript_P1.Vibrate (0.7f, 0.7f, 0.2f);
		#endif
	}

	void CreateExplosion ()
	{
		// Have to create an instance of the Explosion prefab.
		GameObject _Explosion = Instantiate (Explosion, transform.position, Quaternion.identity);
		_Explosion.GetComponent<Explosion> ().blockScript = this;
		_Explosion.GetComponent<Explosion> ().TextColor = TextColor;
		_Explosion.GetComponent<Explosion> ().Anim ();
	}

	void CheckForBoundary ()
	{
		if (transform.position.x > BoundaryX.y || 
			transform.position.x < BoundaryX.x || 
			transform.position.y > BoundaryY.y || 
			transform.position.y < BoundaryY.x) 
		{
			Destroy (gameObject);
			return;
		}
	}

	void SequentialScroll ()
	{
		int blockTypeLength = System.Enum.GetValues (typeof(mainBlockType)).Length;

		if (BlockType < (mainBlockType)blockTypeLength) 
		{
			BlockType += 1;
		}

		if (BlockType >= (mainBlockType)blockTypeLength) 
		{
			BlockType = 0;
		}
	}

	void RandomScroll ()
	{
		int blockTypeLength = System.Enum.GetValues (typeof(mainBlockType)).Length;
		BlockType = (mainBlockType)Random.Range (0, blockTypeLength);
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
				CheckForBoundary ();
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
