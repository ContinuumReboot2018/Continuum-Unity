using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Block : MonoBehaviour
{
	public PlayerController playerControllerScript_P1; // Reference to Player Controller.
	public GameController gameControllerScript; // Reference to Game Controller
	public TimescaleController timeScaleControllerScript; // Reference to Timescale Controller.
	public AudioController audioControllerScript; // Reference to Audio Controller.
	public AudioProcessor processor;
	private TimeBody timeBodyScript;
	public MeshRenderer rend; // This Mesh Renderer.
	private Rigidbody rb; // The current Rigidbody.
	private Collider BoxCol; // The collider/trigger.
	public ScrollTextureOverTime textureScrollScript; // Reference to Texture Scroll Script.
	public ParentToTransform parentToTransformScript; // Parent to transform script reference.
	public bool allowParticleCollisionBoss;

	[Header ("Current Stats")]
	public float speed; // How fast the block falls from the top.
	public bool OverwriteVelocity; // If on, velocity can be overridden.
	public bool DontIncrementBlocksDestroyed; // If destroyed this is checked to see if Blocks Destroyed is incremented.
	public bool Stackable; // Is the block able to be stacked?
	public bool isStacked; // Is the bloc currently stacked?
	public StackZone stack; // The current stack zone of the stacked block.
	public float AddAbilityTime = 0.01f;
	public List<Homing> homedObjects;

	[Header ("BlockTypes")]
	public BlockManager AquaBlock;
	public BlockManager BlueBlock;
	public BlockManager PurpleBlock;
	public BlockManager PinkBlock;

	[Header ("Boss Part")]
	public bool isBossPart; // Is the block part of a boss.
	public bool GotDetached; // Has the block been detached from the boss.
	public MiniBoss miniBoss; // Connection to Mini Boss.
	public int bossPartIndex;
	public float BossDamage = 1.0f;
	public int HitPoints = 5;

	[Header ("Boundary")]
	public Vector2 BoundaryX; // Horizontal bounds.
	public Vector2 BoundaryY; // Vertical bounds.

	[Header ("Normal Block Types")]
	public mainBlockType BlockType; // The current block type for normal types.
	public enum mainBlockType
	{
		Aqua   = 0,
		Blue   = 1,
		Purple = 2, 
		Pink   = 3
	}
	private int normalBlockTypeListLength;
			
	[Header ("Block changes")]
	public blockChangeType BlockChangeType; // How the block type changes.
	public enum blockChangeType
	{
		Static = 0, // Always stays the same type throughout lifetime.
		Sequential = 1, // Scroll in order of type then loop around throughout lifetime.
		RandomOnce = 2, // Randomly select a type when awake.
		RandomScroll = 3 // Randomly select types every change rate time.
	}

	public float ChangeRate; // How frequent the type changes over time.

	[Header ("Special Block Types")]
	public bool isSpecialBlockType = false; // Be a special block.
	public specialBlockType SpecialBlockType;
	public enum specialBlockType
	{
		Noise = 0, // Frozen and scrolling noise texture to achieve glitch effect.
		Red = 1, // Explosive and its explosion can harm the player.
		Tutorial = 2 // Like a normal block except doesnt contribute to the game.
	}

	public Material noiseMat; // Material for noise type.
	public GameObject NoiseExplosion; // Explosion for noise type.

	[Header ("Explosion Combo")]
	public GameObject Explosion; // The currently selected explosion.
	public float BasePointValue; // The current base point value.
	public float totalPointValue; // The total point value as a calculation.

	public Color TextColor; // The current text color for the current explosion text/
	public float MinYPos = -15.0f; // The minimum y pos before being destroyed automatically.

	public GameObject playerExplosion; // The explosion to create on the player if collided with.

	[Header ("Block Formation")]
	public BlockFormation blockFormationScript; // If the block is part of a formation, reference this.
	public bool isBlockFormationConnected; // Checks if the block is connected to the referenced block formation.

	[Header ("Camera Shake")]
	public CameraShake camShakeScript; // Camera shaker.
	public float newCamShakeDuration = 0.1f; // How long to shake the camera for.
	public float newCamShakeAmount = 0.1f; // How much the camera shakes.

	[Header ("Audio")]
	public float LowPassTargetFreq = 1500; // Frequency for the low pass filter.
	public float ResonanceTargetFreq = 1; // Resonance for the low pass filter.

	[Header ("Tutorial")]
	public bool isTutorialBlock; // Checks if it is a tutorial block to bypass settings.
	public TutorialManager tutorialManagerScript; // Reference to tutorial manager.
	public int tutorialBlockIndex; // Checks the block index if part of the tutorial.

	void Awake ()
	{
		// Initialize self.
		BoxCol = GetComponent<Collider> ();
		rb = GetComponent<Rigidbody> ();
		rend = GetComponentInChildren<MeshRenderer> ();
		InvokeRepeating ("CheckBounds", 0, 0.5f);
		normalBlockTypeListLength = System.Enum.GetValues (typeof(mainBlockType)).Length;

		// Random scroll: Change randomly, update self.
		if (BlockChangeType == blockChangeType.RandomScroll) 
		{
			InvokeRepeating ("RandomScroll", 0, ChangeRate);
			InvokeRepeating ("UpdateBlockType", 0, ChangeRate);
			return;
		}

		// Sequential: Change orderly, update self.
		if (BlockChangeType == blockChangeType.Sequential) 
		{
			InvokeRepeating ("SequentialScroll", 0, ChangeRate);
			InvokeRepeating ("UpdateBlockType", 0, ChangeRate);
			return;
		}

		// Static: Don't change type, update once.
		if (BlockChangeType == blockChangeType.Static) 
		{
			UpdateBlockType ();
			return;
		}

		// Random once: Change type once randomly, update self.
		if (BlockChangeType == blockChangeType.RandomOnce) 
		{
			BlockType = (mainBlockType)(Random.Range (0, 4));
			UpdateBlockType ();
			return;
		}

		Invoke ("AllowParticleCollisionBossNow", 2);
	}

	void Start () 
	{
		// Find external scripts.
		playerControllerScript_P1 = GameObject.Find ("PlayerController_P1").GetComponent<PlayerController> ();
		gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		timeScaleControllerScript = GameObject.Find ("TimescaleController").GetComponent<TimescaleController> ();
		audioControllerScript = GameObject.Find ("AudioController").GetComponent<AudioController> ();
		camShakeScript = GameObject.Find ("CamShake").GetComponent<CameraShake> ();
		parentToTransformScript = GetComponent<ParentToTransform> ();
		timeBodyScript = GetComponent<TimeBody> ();
		processor = GameObject.Find ("BeatDetectionTrack").GetComponent<AudioProcessor> ();
		processor.onBeat.AddListener (onOnbeatDetected);

		// Finds texture scroll script.
		if (textureScrollScript == null) 
		{
			textureScrollScript = GetComponent<ScrollTextureOverTime> ();
		}
			
		// If not connected to a block formation, set property to not connected to block formation.
		if (blockFormationScript == null) 
		{
			isBlockFormationConnected = false;
			return;
		}

		// If connected to a block formation, set property to connected.
		if (blockFormationScript != null) 
		{
			isBlockFormationConnected = true;
		}

		// If block formation, overwrite velocity and follow parent object position.
		if (isBlockFormationConnected == true) 
		{
			OverwriteVelocity = true;
			rb.velocity = Vector3.zero;
		}
	}

	void onOnbeatDetected ()
	{
		GetComponentInChildren<Animator> ().Play ("BlockBeat");
	}

	void FixedUpdate () 
	{
		// Check velocity as a normal block.
		if (OverwriteVelocity == false && 
			isBossPart == false) 
		{
			//rb.velocity = new Vector3 (0, speed * Time.fixedUnscaledDeltaTime * Time.timeScale, 0);
			rb.velocity = new Vector3 (0, speed * Time.fixedDeltaTime * Time.timeScale, 0);
		}

		// Check if rewinding.
		if (timeScaleControllerScript.isRewinding == true) 
		{
			if (timeBodyScript != null) 
			{
				// No rewinding data.
				if (timeBodyScript.pointsInTime.Count == 0)
				{
					// Overwrite velocity.
					OverwriteVelocity = true;
					rb.velocity = Vector3.zero;
				}
			}

			if (isBlockFormationConnected == true) 
			{
				if (blockFormationScript != null)
				{
					blockFormationScript.enabled = true;
				}
			}
		}

		// For a block which is not part of a formation and is not a boss part and is not stacked yet.
		if (timeScaleControllerScript.isRewinding == false && 
			isBlockFormationConnected == false && 
			isBossPart == false && isStacked == false) 
		{
			OverwriteVelocity = false;
		}
	}

	// Collisions via particles.
	void OnParticleCollision (GameObject particle)
	{
		if (particle.tag == "Bullet" || particle.tag == "Hazard") 
		{
			if (isBossPart == false)
			{
				BoxCol.enabled = false; // Turn off box collider to prevent multiple collisions.

				// If tutorial script is referenced.
				if (tutorialManagerScript != null)
				{
					// Reset block index in info section.
					if (tutorialManagerScript.TutorialPhase != TutorialManager.tutorialPhase.Info)
					{
						tutorialManagerScript.Blocks [tutorialBlockIndex] = null;
					}

					// Turn off the tutorial in info section.
					if (tutorialManagerScript.TutorialPhase == TutorialManager.tutorialPhase.Info)
					{
						Debug.Log ("Attempted to turn off tutorial.");
						tutorialManagerScript.TurnOffTutorial ();
					}
				}

				// Other object has a bullet component.
				if (particle.GetComponentInParent<Bullet> () != null) 
				{
					// Stops the bullet that hit it from hanging around.
					if (particle.GetComponentInParent<Bullet> ().allowBulletColDeactivate == true) 
					{
						//particle.GetComponentInParent<Bullet> ().BlockHit ();
						particle.GetComponentInParent<Bullet> ().DestroyObject ();
					}
				}

				GetTotalPointValue (); // Get total point calculation.
				CreateExplosion (); // Create the explosion.
				IncrementBlocksDestroyed (); // Increment blocks destroyed.
				DoCamShake (); // Shake camera.
				DoVibrate ();
				Destroy (gameObject);
				return;
			}

			if (isBossPart == true) 
			{
				if (HitPoints > 0) 
				{
					HitPoints--;

					if (particle.GetComponentInParent<Bullet> () != null) 
					{
						if (particle.GetComponentInParent<Bullet> ().allowBulletColDeactivate == true &&
							particle.GetComponentInParent<Bullet> ().BulletTypeName != "Helix") 
						{
							Destroy (particle.gameObject);
						}
					}

					//if (particle.GetComponentInParent<Bullet> () == null) 
					//{
					//	Destroy (particle.gameObject);
					//}

					GetTotalPointValue (); // Get total point calculation.
					CreateExplosion (); // Create the explosion.
				}

				if (HitPoints <= 0) 
				{
					GetTotalPointValue (); // Get total point calculation.
					CreateExplosion (); // Create the explosion.
					DoCamShake (); // Destroy this object.
					DoVibrate ();

					if (particle.GetComponentInParent<Bullet> () != null) 
					{
						if (particle.GetComponentInParent<Bullet> ().allowBulletColDeactivate == true &&
							particle.GetComponentInParent<Bullet> ().BulletTypeName != "Helix") 
						{
							Destroy (particle.gameObject);
						}
					}

					//if (particle.GetComponentInParent<Bullet> () == null) 
					//{
					//	Destroy (particle.gameObject);
					//}

					Destroy (gameObject); // Destroy this object.
					return; // Prevent any further code execution.
				}
			}


		}
	}

	// Trigger via other objects.
	void OnTriggerEnter (Collider other)
	{
		// Other object has missile in its name.
		if (other.gameObject.name.Contains ("Missile")) 
		{
			BoxCol.enabled = false; // Turn off box collider to prevent multiple collisions.

			if (isBossPart == false) 
			{
				GetTotalPointValue (); // Get total point calculation.
				CreateExplosion (); // Create the explosion.
				IncrementBlocksDestroyed (); // Increment blocks destroyed.
				DoCamShake (); // Shake camera.
				DoVibrate ();

				// If the tag is not a bullet.
				if (other.tag != "Bullet") 
				{
		
					//Destroy (other.gameObject); // Destroy other object.
					Destroy (gameObject); // Destroy this object.
					return; // Prevent any further code execution.
				}
			}
		}

		// Other object's tag is Bullet.
		if (other.tag == "Bullet") 
		{
			if (isBossPart == false) 
			{
				BoxCol.enabled = false; // Turn off box collider to prevent multiple collisions.

				// If tutorial script is referenced.
				if (tutorialManagerScript != null) 
				{
					// Reset block index in info section.
					if (tutorialManagerScript.TutorialPhase != TutorialManager.tutorialPhase.Info) 
					{
						tutorialManagerScript.Blocks [tutorialBlockIndex] = null;
					}

					// Turn off the tutorial in info section.
					if (tutorialManagerScript.TutorialPhase == TutorialManager.tutorialPhase.Info) 
					{
						Debug.Log ("Attempted to turn off tutorial.");
						tutorialManagerScript.TurnOffTutorial ();
					}
				}

				GetTotalPointValue (); // Get total point calculation.
				CreateExplosion (); // Create the explosion.
				IncrementBlocksDestroyed (); // Increment blocks destroyed.

				// Other object has a bullet component.
				if (other.GetComponent<Bullet> () != null)
				{
					// Stops the bullet that hit it from hanging around.
					if (other.GetComponent<Bullet> ().allowBulletColDeactivate == true) 
					{
						//other.GetComponent<Bullet> ().BlockHit ();
						other.GetComponentInParent<Bullet> ().DestroyObject ();
					} else 
					{
						//Destroy (other.gameObject);
					}
				}

				DoCamShake (); // Destroy this object.
				DoVibrate ();
				Destroy (gameObject); // Destroy this object.
				return; // Prevent any further code execution.
			}

			if (isBossPart == true) 
			{
				if (HitPoints > 0) 
				{
					HitPoints--;

					if (other.GetComponent<Bullet> () != null)
					{
						if (other.GetComponent<Bullet> ().allowBulletColDeactivate == true) 
						{
							if (other.GetComponentInParent<Bullet> ().BulletTypeName.Contains ("Helix") == false)
							{
								Destroy (other.gameObject);
							}
						}
					}

					GetTotalPointValue (); // Get total point calculation.
					CreateExplosion (); // Create the explosion.
				}

				if (HitPoints <= 0) 
				{
					GetTotalPointValue (); // Get total point calculation.
					CreateExplosion (); // Create the explosion.
					DoCamShake (); // Destroy this object.
					DoVibrate ();

					if (other.GetComponent<Bullet> () != null)
					{
						if (other.GetComponent<Bullet> ().allowBulletColDeactivate == true)
						{
							if (other.GetComponentInParent<Bullet> ().BulletTypeName != "Helix")
							{
								Destroy (other.gameObject);
							}
						}
					}

					Destroy (gameObject); // Destroy this object.
					return; // Prevent any further code execution.
				}
			}
		}

		// Other object has Player tag and this bloc isn't a tutorial block.
		if (other.tag == "Player" && isTutorialBlock == false) 
		{
			// Impact the player normally when lives > 1.
			if (gameControllerScript.Lives > 1) 
			{
				playerControllerScript_P1.PlayerBlockImpact (this);
				playerControllerScript_P1.PlayerImpactGeneric ();
				DoCamShake ();
				DoVibrate ();
				Destroy (gameObject);
				return;
			}

			// On last life, set game over.
			if (gameControllerScript.Lives == 1) 
			{
				playerControllerScript_P1.GameOver ();
			}
		}
	}

	// MiniBoss script calls this on all its children blocks.
	public void ConvertToNoiseBossPart ()
	{
		// Checks if this is a boss part, it doesnt have a parent, and did not get attached yet.
		HitPoints = 1;
		textureScrollScript.enabled = true; // Turn on texture scroll script.
		isSpecialBlockType = true; // Set to special block type.
		SpecialBlockType = specialBlockType.Noise; // Set to block type list.
		speed = 0; // Freeze speed.
		rend.material = noiseMat; // Set material to noise.
		BasePointValue = 0; // Reset base point value.
		TextColor = new Color (0.5f, 0.5f, 0.5f, 1); // set gray text color.
		Explosion = NoiseExplosion; // Set noise explosion.

		// Set scale for noise.
		transform.localScale = new Vector3 
			(
				0.9f * transform.localScale.x,
				0.9f * transform.localScale.y,
				0.9f * transform.localScale.z
			);

		transform.rotation = Quaternion.identity; // Reset rotation.
		GotDetached = true; // Set detached.
		parentToTransformScript.enabled = true; // Parent to transform.

		if (GetComponentInChildren<ParticleSystem> () != null) 
		{
			GetComponentInChildren<ParticleSystem> ().Stop ();
		}
	}

	// Checks block position within boundaries or not.
	void CheckBounds ()
	{
		// Position higher than 60 units, don't collide.
		if (transform.position.y > 14) 
		{
			BoxCol.enabled = false;
		}

		// Position lower than 60 units. Allow collisions.
		if (transform.position.y < 14) 
		{
			BoxCol.enabled = true;
		}

		// Block falls below MinYPos, destroy it.
		if (transform.position.y < MinYPos) 
		{
			Destroy (gameObject);
			return;
		}

		// Boss part check for out of bounds and destroy if so.
		if (isBossPart == true) 
		{
			if (GotDetached == true) 
			{
				if (miniBoss != null) 
				{
					miniBoss = null;
				}

				if (transform.position.y > BoundaryY.y) 
				{
					Destroy (gameObject);
					return;
				}
			}
		}
	}

	// Increment blocks destroyed property.
	void IncrementBlocksDestroyed ()
	{
		if (DontIncrementBlocksDestroyed == false) 
		{
			if (gameControllerScript != null)
			{
				gameControllerScript.BlocksDestroyed += 1;
			}
			return;
		}
	}

	// Called when collided with bullet and calculates point value.
	public void GetTotalPointValue ()
	{
		if (gameControllerScript != null)
		{
			// Calculates total point value based on current combo from game controller and time scale.
			totalPointValue = Mathf.Clamp (
				(BasePointValue + gameControllerScript.Wave) * gameControllerScript.combo * Time.timeScale, 
			// Keep between...
				1, gameControllerScript.MaximumBlockPoints
			);
		}

		// If the game controller is not found yet and block gets destroyed.
		if (gameControllerScript == null) 
		{
			totalPointValue = BasePointValue;
		}

		if (playerControllerScript_P1 != null) 
		{
			if (playerControllerScript_P1.CurrentAbilityTimeRemaining < playerControllerScript_P1.CurrentAbilityDuration) 
			{
				if (isBossPart == false) 
				{
					playerControllerScript_P1.CurrentAbilityTimeRemaining += AddAbilityTime * gameControllerScript.combo; // Increase ability time.
				}

				if (isBossPart == true) 
				{
					playerControllerScript_P1.CurrentAbilityTimeRemaining += AddAbilityTime * gameControllerScript.combo * 0.1f; // Increase ability time.
				}
			}
		}

		// While boss part is still attached to main boss.
		if (GotDetached == false && 
			isBossPart == true)
		{
			if ((miniBoss.StartingHitPoints * 0.5f) < (miniBoss.hitPoints) && 
				allowParticleCollisionBoss == true && 
				(miniBoss.hitPoints > 0.5f * miniBoss.StartingHitPoints))
			{
				Debug.Log ("Hit boss part.");
				float blockHitPoints = 1 / (0.5f * miniBoss.BossParts.Length);
				miniBoss.hitPoints -= blockHitPoints; // Help with 50% of hit points.
				return;
			}
		}
	}

	void AllowParticleCollisionBossNow ()
	{
		allowParticleCollisionBoss = true;
	}

	// Changes combo when collided with.
	public void RefreshCombo ()
	{
		if (gameControllerScript != null)
		{
			// Adds point value to target score in game controller.
			gameControllerScript.TargetScore += totalPointValue;

			// Plays animation if not faded out score text.
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
	}

	// Shakes the camera and vibrates controller.

	void DoCamShake ()
	{
		if (camShakeScript != null) 
		{
			camShakeScript.ShakeCam (0.4f, 0.2f, 10);
		}
	}

	void DoVibrate ()
	{
		#if !PLATFORM_STANDALONE_OSX
		playerControllerScript_P1.Vibrate (0.7f, 0.7f, 0.2f);
		#endif
	}

	// Create explosion when hit with bullet.
	void CreateExplosion ()
	{
		// Have to create an instance of the Explosion prefab.
		GameObject _Explosion = Instantiate (Explosion, transform.position, Quaternion.identity);
		_Explosion.GetComponent<Explosion> ().blockScript = this;
		_Explosion.GetComponent<Explosion> ().TextColor = TextColor;
		_Explosion.GetComponent<Explosion> ().Anim ();
	}

	// Checks boundary for noise blocks.
	void CheckForNoiseBoundary ()
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

	// Scroll block type sequentially.
	void SequentialScroll ()
	{
		// Increment block type index if still going through the list.
		if (BlockType < (mainBlockType)normalBlockTypeListLength) 
		{
			BlockType += 1;
		}

		// Reset block type index when end of list is reached.
		if (BlockType >= (mainBlockType)normalBlockTypeListLength) 
		{
			BlockType = 0;
		}
	}

	// Change block type in list randomly.
	void RandomScroll ()
	{
		BlockType = (mainBlockType)Random.Range (0, normalBlockTypeListLength);
	}

	// Updates properties of block type periodically.
	void UpdateBlockType ()
	{
		// For special block types.
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
				CheckForNoiseBoundary ();
				break;
			}

			return; // Don't bother checking for normal block type changes if this was executed.
		}

		// Check for normal block type changes.
		if (isSpecialBlockType == false) 
		{
			// Update speed stat.
			// Update material stat.
			// Update base point value stat.
			// Update change explosion text color reference stat.
			switch (BlockType) 
			{
			case mainBlockType.Aqua:
				speed = AquaBlock.Speed;
				rend.material = AquaBlock.Material;
				BasePointValue = AquaBlock.BasePointValue;
				TextColor = AquaBlock.TextColor;
				Explosion = AquaBlock.Explosion;
				break;
			case mainBlockType.Blue:
				speed = BlueBlock.Speed;
				rend.material = BlueBlock.Material;
				BasePointValue = BlueBlock.BasePointValue;
				TextColor = BlueBlock.TextColor;
				Explosion = BlueBlock.Explosion;
				break;
			case mainBlockType.Purple:
				speed = PurpleBlock.Speed;
				rend.material = PurpleBlock.Material;
				BasePointValue = PurpleBlock.BasePointValue;
				TextColor = PurpleBlock.TextColor;
				Explosion = PurpleBlock.Explosion;
				break;
			case mainBlockType.Pink:
				speed = PinkBlock.Speed;
				rend.material = PinkBlock.Material;
				BasePointValue = PinkBlock.BasePointValue;
				TextColor = PinkBlock.TextColor;
				Explosion = PinkBlock.Explosion;
				break;
			}
		}
	}
}