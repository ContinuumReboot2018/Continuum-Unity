using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : MonoBehaviour
{
	public GameController gameControllerScript; // Reference to Game Controller.
	public TimescaleController timeScaleControllerScript; // Reference to Timescale Controller.
	public CameraShake camShakeScript; // Reference to camera shake.
	public GameObject Brain; // The brain/ (heart).
	public GameObject MiniBossParent; // The base GameObject of the mini boss.
	public MiniBossFormationGenerator miniBossFormation;

	[Header ("Stats")]
	public float hitPoints = 5; // Current hit points.
	public float StartingHitPoints = 1; // Starting hit points.
	public float ParticleHitPointAmount = 0.01f; // How much damage particles with collision deal to it.

	public GameObject SmallExplosion; // Small explosion to instantiate.
	public GameObject LargeExplosion; // Large explosion to instantiate upon defeat.

	public Transform FollowPlayerPos; // Player follow point with offset.
	public Transform PlayerPos; // The actual position of the player.
	public SimpleFollow simpleFollowScript; // Simple follow script.
	public SimpleLookAt BrainLookScript; // Looking at script for brain.

	[Header ("Boss Parts")]
	public GameObject BossPartsParent; // The parent object where all the boss parts will spawn in.
	public Block[] BossParts; // All the boss parts spawned here as blocks.
	public List<Block> BossPartsList;

	[Header ("Shooting")]
	public bool AllowShoot; // Mini boss can shoot at the player?
	public float FireRate; // How fast to shoot.
	private float NextFire; // When allowed to shoot next.
	public Vector2 FireRateRange; // Range of fire rate.
	public GameObject Missile; // Object to shoot.

	public LineRenderer Line; // Line to aim at player position.
	public float LineTimerOffDuration = 4; // How long the non aiming lasts for.
	public float LineTimerOnDuration = 3; // How long the aiming lasts for.

	[Header ("Effects")]
	public Animator FlipScreen; // Ability for bosses to flip the screen if needed. (Will probably move this functionality to big bosses).

	void Start () 
	{
		gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		timeScaleControllerScript = GameObject.Find ("TimescaleController").GetComponent<TimescaleController> ();
		camShakeScript = GameObject.Find ("CamShake").GetComponent<CameraShake> ();

		// If no brain has been referenced, reference the brain from here.
		if (Brain == null) 
		{
			Brain = this.gameObject;
		}

		// Found a player, set PlayerPos.
		if (GameObject.Find ("PlayerCollider").transform != null) 
		{
			PlayerPos = GameObject.Find ("PlayerCollider").transform;
			FollowPlayerPos = PlayerPos;
			Invoke ("GetBossParts", 0.5f);
			hitPoints = StartingHitPoints;
			StartCoroutine (DrawLineToPlayer ());
			FlipScreen = GameObject.Find ("Camera Rig").GetComponent<Animator>();

			// Set looking at script and following script accordingly.
			BrainLookScript.LookAtPos = FollowPlayerPos.transform;
			simpleFollowScript.OverrideTransform = FollowPlayerPos.transform;
		}
			
		// Couldn't find the player. Bail out and go to next wave.
		if (GameObject.Find ("PlayerCollider").transform == null) 
		{
			BossPartsConvertToNoise ();
			hitPoints = 0;
			Instantiate (LargeExplosion, transform.position, transform.rotation);
			gameControllerScript.StartNewWave ();
			gameControllerScript.IsInWaveTransition = true;
			Debug.LogWarning ("No player found, bailing out.");
			Destroy (MiniBossParent.gameObject);
			return;
		}
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
	}

	void Update ()
	{
		// Creates a line from the LineRenderer from itself to the player position and shoots.
		if (Line.enabled == true) 
		{
			Line.SetPosition (0, transform.position + new Vector3 (0, 0, 0.36f));
			//Line.SetPosition (1, FollowPlayerPos.position);
			Line.SetPosition (1, simpleFollowScript.OverrideTransform.position);
			Shoot ();
		}

		// Draws a line in scene view.
		#if UNITY_EDITOR || UNITY_EDITOR_64
		//Debug.DrawLine (transform.position, FollowPlayerPos.position, Color.green);
		Debug.DrawLine (transform.position, simpleFollowScript.OverrideTransform.position, Color.green);
		#endif

		// Mini boss gets defeated.
		if (hitPoints <= 0) 
		{
			KillMiniBoss ();
			Destroy (MiniBossParent.gameObject); // Destroy the boss parent.
			return;
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
			if ((other.name.Contains ("P1") || other.name.Contains ("Shield_Col")) &&
				other.GetComponent<Bullet> () != null)
			{
				if (other.GetComponent<Bullet> ().playerControllerScript.PlayerId == 1) 
				{
					// Mini boss gets defeated.
					if (hitPoints <= 0) {
						KillMiniBoss ();
						Destroy (MiniBossParent.gameObject); // Destroy the boss parent.
						return;
					}

					// Mini boss takes damage.
					if (hitPoints > 0) {
						hitPoints -= 1; // Reduce a full hit point.
						DoCamShake ();
						Instantiate (SmallExplosion, transform.position, transform.rotation); // Spawn a small explosion.
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

	// Defeat this mini boss and move on to the next wave. 
	void KillMiniBoss ()
	{
		BossPartsConvertToNoise (); // Convert all blocks in array to noise and detach.
		hitPoints = 0; // Reset all hit points.
		Instantiate (LargeExplosion, transform.position, transform.rotation); // Spawn a large explosion.
		gameControllerScript.StartNewWave (); // Go to next wave.
		gameControllerScript.IsInWaveTransition = true; // Set to be in wave transition.
		timeScaleControllerScript.OverrideTimeScaleTimeRemaining = 1f; // Temporarily override time scale. 
		timeScaleControllerScript.OverridingTimeScale = 0.2f; // Set overriding time scale.

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

		yield return new WaitForSeconds (LineTimerOffDuration); // Wait...

		Line.SetPosition (0, transform.position + new Vector3 (0, 0, 0.36f)); // First line point at the boss with some offset.
		//Line.SetPosition (1, FollowPlayerPos.position); // Second line point at the player.
		Line.SetPosition (1, simpleFollowScript.OverrideTransform.position); // Second line point at the player.
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