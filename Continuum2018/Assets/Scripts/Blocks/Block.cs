﻿using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Block : MonoBehaviour
{
	public AudioProcessor processor;
	private TimeBody timeBodyScript;
	public MeshRenderer rend; // This Mesh Renderer.
	private Rigidbody rb; // The current Rigidbody.
	private Collider BoxCol; // The collider/trigger.
	public ScrollTextureOverTime textureScrollScript; // Reference to Texture Scroll Script.
	public ParentToTransform parentToTransformScript; // Parent to transform script reference.

	[Header ("Current Stats")]
	[Tooltip ("How fast the block falls from the top.")]
	public float speed;
	[Tooltip ("If on, velocity can be overridden.")]
	public bool OverwriteVelocity;
	[Tooltip ("If destroyed this is checked to see if Blocks Destroyed is incremented.")]
	public bool DontIncrementBlocksDestroyed;
	[Tooltip ("Is the block able to be stacked?")]
	public bool Stackable;
	[Tooltip ("Is the block currently stacked?")]
	public bool isStacked;
	[Tooltip ("The current stack zone of the stacked block.")]
	public StackZone stack;
	[Tooltip ("How much ability time is added when destroyed.")]
	public float AddAbilityTime = 0.01f;
	[Tooltip ("List of homing objects that have this object locked on to it.")]
	public List<Homing> homedObjects;

	[Header ("BlockTypes")]
	public BlockManager AquaBlock;
	public BlockManager BlueBlock;
	public BlockManager PurpleBlock;
	public BlockManager PinkBlock;

	[Header ("Boss Part")]
	[Tooltip ("Is the block part of a boss.")]
	public bool isBossPart;
	[Tooltip ("Has the block been detached from the boss.")]
	public bool GotDetached;
	[Tooltip ("Connection to Mini Boss.")]
	public MiniBoss miniBoss;
	[Tooltip ("Index in boss barts array.")]
	public int bossPartIndex;
	[Tooltip ("How much damage this boss part does to the mini boss when a collision happens with a bullet or particle.")]
	public float BossDamage = 1.0f;
	[Tooltip ("How many hit points this boss part can witrhstand before destroying.")]
	public int HitPoints = 5;
	[Tooltip ("Allow particle collisions?")]
	public bool allowParticleCollisionBoss;

	[Header ("Boundary")]
	[Tooltip ("Horizontal bounds.")]
	public Vector2 BoundaryX;
	[Tooltip ("Vertical bounds.")]
	public Vector2 BoundaryY;

	[Header ("Normal Block Types")]
	[Tooltip ("The current block type for normal types.")]
	public mainBlockType BlockType;
	public enum mainBlockType
	{
		Aqua   = 0,
		Blue   = 1,
		Purple = 2, 
		Pink   = 3
	}
	private int normalBlockTypeListLength;
			
	[Header ("Block changes")]
	[Tooltip ("How the block type changes.")]
	public blockChangeType BlockChangeType;
	public enum blockChangeType
	{
		Static = 0, // Always stays the same type throughout lifetime.
		Sequential = 1, // Scroll in order of type then loop around throughout lifetime.
		RandomOnce = 2, // Randomly select a type when awake.
		RandomScroll = 3 // Randomly select types every change rate time.
	}

	[Tooltip ("How frequent the type changes over time.")]
	public float ChangeRate;

	[Header ("Special Block Types")]
	[Tooltip ("Be a special block.")]
	public bool isSpecialBlockType = false;
	[Tooltip ("Type of special block.")]
	public specialBlockType SpecialBlockType;
	public enum specialBlockType
	{
		Noise = 0, // Frozen and scrolling noise texture to achieve glitch effect.
		Red = 1, // Explosive and its explosion can harm the player.
		Tutorial = 2 // Like a normal block except doesnt contribute to the game.
	}

	public bool isBonusBlock;

	public static List<Block> allBlocks;

	[Tooltip ("Material for noise type.")]
	public Material noiseMat;
	[Tooltip ("Explosion for noise type.")]
	public GameObject NoiseExplosion;

	[Header ("Explosion Combo")]
	[Tooltip ("The currently selected explosion.")]
	public GameObject Explosion;
	[Tooltip ("The current base point value.")]
	public float BasePointValue;
	[Tooltip ("The total point value as a calculation.")]
	public float totalPointValue;
	[Tooltip ("The current text color for the current explosion text.")]
	public Color TextColor; // 
	[Tooltip ("The minimum y pos before being destroyed automatically.")]
	public float MinYPos = -15.0f;
	[Tooltip ("The explosion to create on the player if collided with.")]
	public GameObject playerExplosion;

	[Header ("Block Formation")]
	[Tooltip ("If the block is part of a formation, reference this.")]
	public BlockFormation blockFormationScript;
	[Tooltip ("Checks if the block is connected to the referenced block formation.")]
	public bool isBlockFormationConnected;

	[Header ("Camera Shake")]
	[Tooltip ("Camera shaker.")]
	public CameraShake camShakeScript;
	[Tooltip ("How long to shake the camera for.")]
	public float newCamShakeDuration = 0.1f;
	[Tooltip ("How much the camera shakes.")]
	public float newCamShakeAmount = 0.1f;

	[Header ("Audio")]
	[Tooltip ("Frequency for the low pass filter.")]
	public float LowPassTargetFreq = 1500;
	[Tooltip ("Resonance for the low pass filter.")]
	public float ResonanceTargetFreq = 1;

	[Header ("Tutorial")]
	[Tooltip ("Checks if it is a tutorial block to bypass settings.")]
	public bool isTutorialBlock;
	[Tooltip ("Checks the block index if part of the tutorial.")]
	public int tutorialBlockIndex;

	void OnEnable ()
	{
		if (allBlocks == null) 
		{
			allBlocks = new List<Block> ();
		}

		allBlocks.Add (this);
		SetNewBlockType ();
		RefreshBlockStats ();
		UpdateBlockType ();
	}

	void SetNewBlockType ()
	{
		if (isBossPart == false && isBonusBlock == false) 
		{
			if (GameController.Instance.Wave < 5) 
			{
				int BlockId = Random.Range (0, GameController.Instance.Wave);
				BlockType = (mainBlockType)BlockId;
			} 

			else 
			
			{
				int BlockId = Random.Range (0, 4);
				BlockType = (mainBlockType)BlockId;
			}
		}
	}

	void OnDisable ()
	{
		if (isStacked == true) 
		{
			stack.VacateBlock ();
			isStacked = false;
		}

		allBlocks.Remove (this);

		if (processor != null)
		{
			processor.onBeat.RemoveListener (onOnbeatDetected);
		}

		//SaveAndLoadScript.Instance.blocksDestroyed++;
	}

	void OnDestroy ()
	{
		if (isStacked == true) 
		{
			stack.VacateBlock ();
			isStacked = false;
		}

		BlockChecker.Instance.BlocksInstanced.Remove (gameObject);
		allBlocks.Remove (this);

		if (processor != null)
		{
			processor.onBeat.RemoveListener (onOnbeatDetected);
		}

		//SaveAndLoadScript.Instance.blocksDestroyed++;
	}
		
	void Awake ()
	{
		// Initialize self.
		BoxCol = GetComponent<Collider> ();
		rb = GetComponent<Rigidbody> ();
		rend = GetComponentInChildren<MeshRenderer> (true);
		InvokeRepeating ("CheckBounds", 0, 0.5f);
		normalBlockTypeListLength = System.Enum.GetValues (typeof(mainBlockType)).Length;

		if (isBossPart == false && isBonusBlock == false) 
		{
			BlockChecker.Instance.BlocksInstanced.Add (gameObject);
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
		UpdateBlockType ();
	}

	public void RefreshBlockStats ()
	{
		if (isStacked == true)
		{
			stack.VacateBlock ();
		}
	}

	public void Start () 
	{
		// Find external scripts.
		camShakeScript = GameObject.Find ("CamShake").GetComponent<CameraShake> ();
		parentToTransformScript = GetComponent<ParentToTransform> ();
		timeBodyScript = GetComponent<TimeBody> ();

		processor = AudioController.Instance.BeatDetectionTracks [(int)BlockType].GetComponent<AudioProcessor> ();
		processor.onBeat.AddListener (onOnbeatDetected);

		// Finds texture scroll script.
		if (textureScrollScript == null) 
		{
			textureScrollScript = GetComponentInChildren<ScrollTextureOverTime> (true);
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
	}

	void onOnbeatDetected ()
	{
		// Animates in beat based on block type.
		if (AudioController.Instance.BeatInBar == (int)BlockType + 1) 
		{
			GetComponentInChildren<Animator> ().Play ("BlockBeat");
		}

		// Changes block type if random.
		if (BlockChangeType == blockChangeType.RandomScroll) 
		{
			RandomScroll ();
			UpdateBlockType ();
			return;
		}

		// Sequential: Change orderly, update self.
		if (BlockChangeType == blockChangeType.Sequential) 
		{
			SequentialScroll ();
			UpdateBlockType ();
			return;
		}
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
		if (TimescaleController.Instance.isRewinding == true) 
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
		if (TimescaleController.Instance.isRewinding == false && 
			isBlockFormationConnected == false && 
			isBossPart == false && isStacked == false) 
		{
			OverwriteVelocity = false;
		}
	}

	// Collisions via particles.
	void OnParticleCollision (GameObject particle)
	{
		if (particle.tag == "Bullet" ||
		    particle.tag == "Hazard") 
		{
			if (transform.position.y > 15) 
			{
				return;
			}

			if (transform.position.y <= 15)
			{
				if (isBossPart == false) 
				{
					BoxCol.enabled = false; // Turn off box collider to prevent multiple collisions.

					// If tutorial script is referenced.
					if (TutorialManager.Instance != null && TutorialManager.Instance.tutorialComplete == false) 
					{
						// Reset block index in info section.
						if (TutorialManager.Instance.TutorialPhase != TutorialManager.tutorialPhase.Info
							&& TutorialManager.Instance.gameObject.activeSelf == true) 
						{
							TutorialManager.Instance.Blocks [tutorialBlockIndex] = null;
						}

						// Turn off the tutorial in info section.
						if (TutorialManager.Instance.TutorialPhase == TutorialManager.tutorialPhase.Info
							&& TutorialManager.Instance.gameObject.activeSelf == true) 
						{
							Debug.Log ("Attempted to turn off tutorial.");
							TutorialManager.Instance.TurnOffTutorial (false);
							TutorialManager.Instance.gameObject.SetActive (false);
						}
					}

					// Other object has a bullet component.
					if (particle.GetComponentInParent<Bullet> () != null) 
					{
						if (isBonusBlock == true) 
						{
							IncrementBlocksDestroyed ();
						}

						particle.GetComponentInParent<Bullet> ().hitABlock = true;

						// Stops the bullet that hit it from hanging around.
						if (particle.GetComponentInParent<Bullet> ().allowBulletColDeactivate == true) 
						{
							particle.GetComponentInParent<Bullet> ().DestroyObject ();
						}
					}

					GetTotalPointValue (); // Get total point calculation.
					CreateExplosion (); // Create the explosion.
					IncrementBlocksDestroyed (); // Increment blocks destroyed.
					DoCamShake (); // Shake camera.
					DoVibrate ();

					//Destroy (gameObject);
					gameObject.SetActive (false);
					return;
				}

				if (isBossPart == true) 
				{
					if (GotDetached == true)
					{
						HitPoints = 0;
					}
						
					if (HitPoints <= 0) 
					{
						GetTotalPointValue (); // Get total point calculation.
						IncrementBlocksDestroyed ();
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
					
						//Destroy (gameObject); // Destroy this object.
						gameObject.SetActive (false);
						return; // Prevent any further code execution.
					}

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

						GetTotalPointValue (); // Get total point calculation.
						CreateExplosion (); // Create the explosion.
						return;
					}
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
					//Destroy (gameObject); // Destroy this object.
					gameObject.SetActive (false);
					return; // Prevent any further code execution.
				}
			}
		}

		// Other object's tag is Bullet.
		if (other.tag == "Bullet") 
		{
			if (isBonusBlock == true) 
			{
				IncrementBlocksDestroyed ();
			}

			if (transform.position.y > 15) 
			{
				return;
			}

			if (transform.position.y <= 15) 
			{
				if (isBossPart == false)
				{
					BoxCol.enabled = false; // Turn off box collider to prevent multiple collisions.

					// If tutorial script is referenced.
					if (TutorialManager.Instance != null && TutorialManager.Instance.tutorialComplete == false)
					{
						// Reset block index in info section.
						if (TutorialManager.Instance.TutorialPhase != TutorialManager.tutorialPhase.Info)
						{
							TutorialManager.Instance.Blocks [tutorialBlockIndex] = null;
						}

						// Turn off the tutorial in info section.
						if (TutorialManager.Instance.TutorialPhase == TutorialManager.tutorialPhase.Info) 
						{
							Debug.Log ("Attempted to turn off tutorial.");
							TutorialManager.Instance.TurnOffTutorial (false);
							TutorialManager.Instance.gameObject.SetActive (false);
						}
					}

					GetTotalPointValue (); // Get total point calculation.
					CreateExplosion (); // Create the explosion.
					IncrementBlocksDestroyed (); // Increment blocks destroyed.

					// Other object has a bullet component.
					if (other.GetComponent<Bullet> () != null) 
					{
						other.GetComponentInParent<Bullet> ().hitABlock = true;

						// Stops the bullet that hit it from hanging around.
						if (other.GetComponent<Bullet> ().allowBulletColDeactivate == true)
						{
							other.GetComponentInParent<Bullet> ().DestroyObject ();
						}
					}
						
					DoCamShake (); // Destroy this object.
					DoVibrate ();
					//Destroy (gameObject); // Destroy this object.
					gameObject.SetActive (false);
					return; // Prevent any further code execution.
				}
					
				if (isBossPart == true)
				{
					if (HitPoints > 0) 
					{
						HitPoints--;

						GetTotalPointValue (); // Get total point calculation.
						CreateExplosion (); // Create the explosion.

						if (other.GetComponent<Bullet> () != null)
						{
							if (other.GetComponent<Bullet> ().allowBulletColDeactivate == true)
							{
								if (other.GetComponentInParent<Bullet> ().BulletTypeName.Contains ("Helix") == false) 
								{
									Destroy (other.gameObject);
									//Destroy (gameObject); // Destroy this object.
									gameObject.SetActive (false);
								}
							}
						}
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
								if (other.GetComponentInParent<Bullet> ().BulletTypeName.Contains ("Helix") == false) 
								{
									Destroy (other.gameObject);
									//Destroy (gameObject); // Destroy this object.
									gameObject.SetActive (false);
								}
							}
						}

						//Destroy (gameObject); // Destroy this object.
						gameObject.SetActive (false);
						return; // Prevent any further code execution.
					}
				}
			}
		}

		// Other object has Player tag and this bloc isn't a tutorial block.
		if (other.tag == "Player" && isTutorialBlock == false) 
		{
			// Impact the player normally when lives > 1.
			if (GameController.Instance.Lives > 1) 
			{
				if (other.name.Contains ("P1")) 
				{
					PlayerController.PlayerOneInstance.PlayerBlockImpact (this);
					PlayerController.PlayerOneInstance.PlayerImpactGeneric ();
				}

				if (other.name.Contains ("P2")) 
				{
					PlayerController.PlayerTwoInstance.PlayerBlockImpact (this);
					PlayerController.PlayerTwoInstance.PlayerImpactGeneric ();
				}

				DoCamShake ();
				DoVibrate ();
				//Destroy (gameObject);
				gameObject.SetActive (false);
				return;
			}

			// On last life, set game over.
			if (GameController.Instance.Lives == 1) 
			{
				PlayerController.PlayerOneInstance.GameOver ();
			}
		}
	}

	// MiniBoss script calls this on all its children blocks.
	public void ConvertToNoiseBossPart ()
	{
		//parentToTransformScript.ParentNow ();

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
		GetComponentInChildren<Animator> (true).enabled = false;

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

		CheckForNoiseBoundary ();
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
			//Destroy (gameObject);
			gameObject.SetActive (false);
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
					//Destroy (gameObject);
					gameObject.SetActive (false);
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
			if (GameController.Instance != null)
			{
				GameController.Instance.BlocksDestroyed++;
			}
			return;
		}
	}

	// Called when collided with bullet and calculates point value.
	public void GetTotalPointValue ()
	{
		if (GameController.Instance != null)
		{
			// Calculates total point value based on current combo from game controller and time scale.
			totalPointValue = Mathf.Clamp (
				(BasePointValue + GameController.Instance.Wave) * GameController.Instance.combo * Time.timeScale, 
			// Keep between...
				1, GameController.Instance.MaximumBlockPoints
			);

			GameController.Instance.pointsThisCombo += (int)totalPointValue;
			GameController.Instance.PointsThisComboAnim.SetTrigger ("PointsThisCombo");
			GameController.Instance.PointsThisComboText.text = "+ " + GameController.Instance.pointsThisCombo;
		}

		// If the game controller is not found yet and block gets destroyed.
		if (GameController.Instance == null) 
		{
			totalPointValue = BasePointValue;
		}

		if (PlayerController.PlayerOneInstance != null) 
		{
			// If player is not recovering.
			if (PlayerController.PlayerOneInstance.isInCooldownMode == false)
			{
				// Ability time remaining must be less than the required duration.
				if (PlayerController.PlayerOneInstance.CurrentAbilityTimeRemaining < PlayerController.PlayerOneInstance.CurrentAbilityDuration)
				{
					if (isBossPart == false) // Not a boss part.
					{
						PlayerController.PlayerOneInstance.CurrentAbilityTimeRemaining += AddAbilityTime * PlayerController.PlayerOneInstance.AbilityDampening; // Increase ability time.
					}

					if (isBossPart == true) // Is boss part.
					{
						PlayerController.PlayerOneInstance.CurrentAbilityTimeRemaining += AddAbilityTime * PlayerController.PlayerOneInstance.AbilityDampening; // Increase ability time.
					}
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
				//Debug.Log ("Hit boss part.");
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
		if (GameController.Instance != null)
		{
			// Adds point value to target score in game controller.
			GameController.Instance.TargetScore += totalPointValue;

			// Plays animation if not faded out score text.
			if (PlayerController.PlayerOneInstance.ScoreAnim.GetCurrentAnimatorStateInfo (0).IsName ("ScoreFadeOut") == false) 
			{
				PlayerController.PlayerOneInstance.ScoreAnim.Play ("ScorePoints");
			}

			// Adds to next combo.
			if (GameController.Instance.combo > 1)
			{
				GameController.Instance.combo += 2;
			}

			if (GameController.Instance.combo <= 1)
			{
				GameController.Instance.combo += 1;
			}

			// Resets combo time.
			GameController.Instance.comboTimeRemaining = GameController.Instance.comboDuration;
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
		#if !PLATFORM_STANDALONE_OSX && !PLATFORM_ANDROID && !PLATFORM_WEBGL
		PlayerController.PlayerOneInstance.Vibrate (0.7f, 0.7f, 0.2f);

		if (PlayerController.PlayerTwoInstance != null)
		{
			PlayerController.PlayerTwoInstance.Vibrate (0.7f, 0.7f, 0.2f);
		}

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
	public void CheckForNoiseBoundary ()
	{
		if (transform.position.x > BoundaryX.y || 
			transform.position.x < BoundaryX.x || 
			transform.position.y > BoundaryY.y || 
			transform.position.y < BoundaryY.x) 
		{
			//Destroy (gameObject);
			gameObject.SetActive (false);
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
				//CheckForNoiseBoundary ();
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
				transform.name = "Aqua Block";
				break;
			case mainBlockType.Blue:
				speed = BlueBlock.Speed;
				rend.material = BlueBlock.Material;
				BasePointValue = BlueBlock.BasePointValue;
				TextColor = BlueBlock.TextColor;
				Explosion = BlueBlock.Explosion;
				transform.name = "Blue Block";
				break;
			case mainBlockType.Purple:
				speed = PurpleBlock.Speed;
				rend.material = PurpleBlock.Material;
				BasePointValue = PurpleBlock.BasePointValue;
				TextColor = PurpleBlock.TextColor;
				Explosion = PurpleBlock.Explosion;
				transform.name = "Purple Block";
				break;
			case mainBlockType.Pink:
				speed = PinkBlock.Speed;
				rend.material = PinkBlock.Material;
				BasePointValue = PinkBlock.BasePointValue;
				TextColor = PinkBlock.TextColor;
				Explosion = PinkBlock.Explosion;
				transform.name = "Pink Block";
				break;
			}
		}
	}
}