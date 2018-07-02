using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MiniBoss : MonoBehaviour
{
	public CameraShake camShakeScript; // Reference to camera shake.
	public EvasiveManeuver evasiveManeuverScript;
	public MiniBossFormationGenerator miniBossFormation;

	public bool isBigBoss;

	[Tooltip ("The brain, (heart).")]
	public GameObject Brain;
	[Tooltip ("The base GameObject of the mini boss.")]
	public GameObject MiniBossParent;
	[Tooltip ("The UI to spawn when the boss spawns.")]
	public GameObject MiniBossUI;
	[Tooltip ("The name of this instance of the boss.")]
	public string MiniBossName;
	//[Tooltip ("The barrier that comes with this mini boss.")]
	//public GameObject Barrier;
	public float ColliderTime = 3;

	[Header ("Stats")]
	[Tooltip ("Current hit points.")]
	public float hitPoints = 5;
	[Tooltip ("Starting hit points.")]
	public float StartingHitPoints = 1;
	[Tooltip ("How much damage particles with collision deal to it.")]
	public float ParticleHitPointAmount = 0.01f; 
	[Tooltip ("Small explosion to instantiate.")]
	public GameObject SmallExplosion;
	[Tooltip ("Large explosion to instantiate upon defeat.")]
	public GameObject LargeExplosion; 
	[Tooltip ("Player follow point with offset.")]
	public Transform FollowPlayerPos;
	[Tooltip ("The actual position of the player.")]
	public Transform PlayerPos;
	[Tooltip ("Simple follow script.")]
	public SimpleFollow simpleFollowScript;
	[Tooltip ("Looking at script for brain.")]
	public SimpleLookAt BrainLookScript;
	[Tooltip ("The collider for the mini boss.")]
	public Collider col;

	[Header ("Boss Parts")]
	[Tooltip ("The parent object where all the boss parts will spawn in.")]
	public GameObject BossPartsParent;
	[Tooltip ("All the boss parts spawned here as blocks.")]
	public Block[] BossParts;
	[Tooltip ("Same as the array in list form.")]
	public List<Block> BossPartsList;

	[Header ("Shooting")]
	[Tooltip ("Mini boss can shoot at the player?")]
	public bool AllowShoot;
	[Tooltip ("How fast to shoot.")]
	public float FireRate;
	private float NextFire; // When allowed to shoot next.
	[Tooltip ("Range of fire rate.")]
	public Vector2 FireRateRange;
	[Tooltip ("Object to shoot.")]
	public GameObject Missile;
	[Tooltip ("Line to aim at player position.")]
	public LineRenderer Line;
	[Tooltip ("How long the non aiming lasts for.")]
	public float LineTimerOffDuration = 4;
	[Tooltip ("How long the aiming lasts for.")]
	public float LineTimerOnDuration = 3;

	[Header ("Effects")]
	[Tooltip ("Ability for bosses to flip the screen if needed. (Will probably move this functionality to big bosses).")]
	public Animator FlipScreen;

	void Awake ()
	{
		hitPoints = StartingHitPoints;
	}

	void OnDestroy ()
	{
		if (AudioController.Instance != null) 
		{
			if (AudioController.Instance.BigBossSoundtrack.isPlaying == true)
			{
				AudioController.Instance.BigBossSoundtrack.Stop ();
			}
		}
	}

	void SpotlightOverrideTransform ()
	{
		PlayerController.PlayerOneInstance.spotlightsScript.NewTarget = this.transform;
	}

	void Start () 
	{
		camShakeScript = GameObject.Find ("CamShake").GetComponent<CameraShake> ();

		evasiveManeuverScript.enabled = false;
		InvokeRepeating ("GetBossParts", 0.1f, 1f);
		Invoke ("TurnOnEvasiveManeuverScript", 4);

		GameObject MiniBossUIObject = GameObject.Find("MiniBossUI");
		MiniBossUIObject.GetComponentInChildren<TextMeshProUGUI> ().text = MiniBossName;
		MiniBossUIObject.GetComponentInChildren<Animator> ().Play ("MiniBossUI");

		InvokeRepeating ("SpotlightOverrideTransform", 0, 1);
		PlayerController.PlayerOneInstance.spotlightsScript.InvokeRepeating ("OverrideSpotlightLookObject", 0, 1);

		if (isBigBoss == false) 
		{
			PlayerController.PlayerOneInstance.spotlightsScript.InvokeRepeating ("BossSpotlightSettings", 0, 1);
		}

		if (isBigBoss == true) 
		{
			PlayerController.PlayerOneInstance.spotlightsScript.InvokeRepeating ("BigBossSpotlightSettings", 0, 1);
		}

		// If no brain has been referenced, reference the brain from here.
		if (Brain == null) 
		{
			Brain = this.gameObject;
		}

		// Found a player, set PlayerPos.
		if (PlayerController.PlayerOneInstance != null) 
		{
			PlayerPos = PlayerController.PlayerOneInstance.transform;
			FollowPlayerPos = PlayerPos;
			Invoke ("GetBossParts", 0.5f);
			StartCoroutine (DrawLineToPlayer ());
			FlipScreen = GameObject.Find ("Camera Rig").GetComponent<Animator>();

			// Set looking at script and following script accordingly.
			BrainLookScript.LookAtPos = FollowPlayerPos.transform;
			simpleFollowScript.OverrideTransform = FollowPlayerPos.transform;
		}


		// Couldn't find the player. Bail out and go to next wave.
		if (PlayerController.PlayerOneInstance == null) 
		{
			BossPartsConvertToNoise ();
			hitPoints = 0;
			Instantiate (LargeExplosion, transform.position, Quaternion.identity);
			GameController.Instance.StartNewWave ();
			GameController.Instance.IsInWaveTransition = true;
			Debug.LogWarning ("No player found, bailing out.");
			Destroy (MiniBossParent.gameObject);
			return;
		}

		if (col == null) 
		{
			col = GetComponent<Collider> ();
		}

		if (col != null) 
		{
			//col.enabled = false;
		}

		Invoke ("EnableCol", 5);
	}

	void TurnOnEvasiveManeuverScript ()
	{
		evasiveManeuverScript.enabled = true;
	}

	void EnableCol ()
	{
		col.enabled = true;
	}

	// Creates an array of boss parts with Block components that were spawned in the BossParts parent GameObject.
	void GetBossParts ()
	{
		BossParts = BossPartsParent.GetComponentsInChildren<Block> (true);
		BossPartsList = new List<Block> ();

		for (int i = 0; i < BossParts.Length; i++)
		{
			BossPartsList.Add (BossParts[i]);
			BossParts [i].bossPartIndex = i;
		}

		if (transform.position.x > 30 || transform.position.x < -30) 
		{
			KillMiniBoss ();
			Destroy (MiniBossParent.gameObject); // Destroy the boss parent.
			return;
		}
	}

	void Update ()
	{
		// Creates a line from the LineRenderer from itself to the player position and shoots.
		if (Line.enabled == true && simpleFollowScript.OverrideTransform != null) 
		{
			Line.SetPosition (
				0, 
				transform.position + new Vector3 (0, 0, 0.36f)
			);

			Line.SetPosition (
				1, 
				Vector3.Lerp (
					Line.GetPosition (1), 
					simpleFollowScript.OverrideTransform.position, 
					10 * Time.deltaTime
				)
			);

			Shoot ();

			// Draws a line in scene view.
			#if UNITY_EDITOR || UNITY_EDITOR_64
			Debug.DrawLine (
				transform.position, 
				Vector3.Lerp (
					Line.GetPosition (1), 
					simpleFollowScript.OverrideTransform.position, 
					10 * Time.deltaTime), 
				Color.green
			);
			#endif
		}
			
		// Mini boss gets defeated.
		if (hitPoints <= 0) 
		{
			KillMiniBoss ();
			Destroy (MiniBossParent.gameObject); // Destroy the boss parent.
			return;
		}

		if (transform.position.y > 12) 
		{
			//col.enabled = false;
		}

		if (transform.position.y <= 12)
		{
			col.enabled = true;
		}
	}

	// Shoots missles to the player.
	void Shoot ()
	{
		// When the mini boss can shoot and enough time has passed to fire.
		if (Time.time > NextFire && AllowShoot == true) 
		{
			GameObject missile = Instantiate (Missile, transform.position, Quaternion.identity); // Spawn the missle.
			missile.transform.LookAt (PlayerPos, Vector3.forward); // Get missile to look at the player.
			FireRate = Random.Range (FireRateRange.x, FireRateRange.y); // Set next fire range.
			NextFire = Time.time + FireRate; // Increment new value of next fire (scaled by Time.timeScale).
		}
	}

	// Collisions by other triggers.
	void OnTriggerEnter (Collider other)
	{
		// Bullet tag.
		if (other.tag == "Bullet") 
		{
			if (transform.position.y > 13) 
			{
				return;
			}

			if (transform.position.y <= 13)
			{
				if ((other.name.Contains ("P1") || other.name.Contains ("Shield_Col")) &&
				   other.GetComponent<Bullet> () != null) 
				{
					if (other.GetComponent<Bullet> ().playerControllerScript.PlayerId == 1) 
					{
						// Mini boss gets defeated.
						if (hitPoints <= 0) 
						{
							KillMiniBoss ();
							Destroy (MiniBossParent.gameObject); // Destroy the boss parent.
							return;
						}

						// Mini boss takes damage.
						if (hitPoints > 0) 
						{
							hitPoints -= 1; // Reduce a full hit point.
							DoCamShake ();
							Instantiate (SmallExplosion, transform.position, transform.rotation); // Spawn a small explosion.
						}
					}
				}
			}
		}
	}

	// Collisions with particles.
	void OnParticleCollision (GameObject col)
	{
		if (col.tag == "Bullet") 
		{
			if (transform.position.y > 13) 
			{
				return;
			}

			if (transform.position.y <= 13)
			{
				// Mini boss gets defeated.
				if (hitPoints <= 0) 
				{
					KillMiniBoss ();
					Destroy (MiniBossParent.gameObject);
					return;
				}

				// Mini boss takes damage.
				if (hitPoints > 0)
				{
					hitPoints -= 1 * ParticleHitPointAmount;
					DoCamShake ();

					if (IsInvoking ("InstanceExplosion") == false) 
					{
						Invoke ("InstanceExplosion", 0.1f);
					}
				}
			}
		}
	}

	// Defeat this mini boss and move on to the next wave. 
	void KillMiniBoss ()
	{
		BossPartsConvertToNoise (); // Convert all blocks in array to noise and detach.

		hitPoints = 0; // Reset all hit points.
		Instantiate (LargeExplosion, transform.position, Quaternion.identity); // Spawn a large explosion.

		GameController.Instance.StartNewWave (); // Go to next wave.
		GameController.Instance.IsInWaveTransition = true; // Set to be in wave transition.

		if (PlayerController.PlayerOneInstance.timeIsSlowed == false)
		{
			TimescaleController.Instance.OverrideTimeScaleTimeRemaining += 1f; // Temporarily override time scale. 
			TimescaleController.Instance.OverridingTimeScale = 0.3f; // Set overriding time scale.
		}

		PlayerController.PlayerOneInstance.spotlightsScript.CancelInvoke ("SpotlightOverrideTransform");
		PlayerController.PlayerOneInstance.spotlightsScript.CancelInvoke ("OverrideSpotlightLookObject");
		PlayerController.PlayerOneInstance.spotlightsScript.CancelInvoke ("BossSpotlightSettings");
		PlayerController.PlayerOneInstance.spotlightsScript.CancelInvoke ("BigBossSpotlightSettings");

		PlayerController.PlayerOneInstance.spotlightsScript.SuccessSpotlightSettings ();
		PlayerController.PlayerOneInstance.spotlightsScript.NewTarget = PlayerPos;
		PlayerController.PlayerOneInstance.spotlightsScript.OverrideSpotlightLookObject ();

		// Reset camera rotation animator parameter.
		if (FlipScreen.GetCurrentAnimatorStateInfo (0).IsName ("CameraRotateUpsideDown") == true) 
		{
			FlipScreen.SetBool ("Flipped", false);
		}
	}

	// Creates small explosion.
	void InstanceExplosion ()
	{
		Instantiate (SmallExplosion, transform.position, transform.rotation);
	}

	// Converts all existing boss parts in array into the static noise type.
	void BossPartsConvertToNoise ()
	{
		foreach (Block block in BossParts)
		{
			if (block != null)
			{
				block.ConvertToNoiseBossPart ();
				//block.CheckForNoiseBoundary ();
				block.parentToTransformScript.ParentNow ();
			}
		}

		DoStrongCamShake ();
	}

	// Sequence for showing a line to 
	IEnumerator DrawLineToPlayer ()
	{
		AllowShoot = false; // Prevent shooting.
		Line.enabled = false; // Hide line.

		Line.SetPosition (
			0, 
			transform.position + new Vector3 (0, 0, 0.36f)
		);

		Line.SetPosition (
			1, 
			transform.position + new Vector3 (0, 0, 0.36f)
		);

		yield return new WaitForSeconds (LineTimerOffDuration); // Wait...

		Line.SetPosition (
			0, 
			transform.position + new Vector3 (0, 0, 0.36f)
		);

		Line.SetPosition (
			1, 
			transform.position + new Vector3 (0, 0, 0.36f)
		);

		Line.enabled = true; // Show line.
		Line.positionCount = 2; // Set position count to 2.

		yield return new WaitForSeconds (1); // Wait again..

		AllowShoot = true; // Allow shooting.

		yield return new WaitForSeconds (LineTimerOnDuration); // Wait.

		StartCoroutine (DrawLineToPlayer ()); // Stop this coroutine.
	}

	void DoCamShake ()
	{
		camShakeScript.ShakeCam (0.7f, 0.3f, 20);
	}

	void DoStrongCamShake ()
	{
		camShakeScript.ShakeCam (1.25f, 0.75f, 30);
	}
}