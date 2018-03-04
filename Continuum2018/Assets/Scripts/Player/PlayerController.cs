using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl; 					// Accessing InControl's cross platform controller input.

#if !PLATFORM_STANDALONE_OSX
using XInputDotNetPure; 			// Accessing controller vibration system and raw inputs.
#endif

using UnityEngine.UI; 				// Accessing Unity's UI system.
using TMPro; 						// Accessing Text Mesh Pro components.
using UnityEngine.Audio; 			// Accessing Audio mixer settings.
using UnityStandardAssets.Utility;  // Accessing some standard assets and scripts.

// One instance per player.
public class PlayerController : MonoBehaviour 
{
	// Reference scripts.
	public GameController 		gameControllerScript;
	public TimescaleController  timescaleControllerScript;
	public AudioController 		audioControllerScript;
	public CursorManager 		cursorManagerScript;
	public CameraShake 			camShakeScript;
	public DeveloperMode 		developerModeScript;
	public TutorialManager 		tutorialManagerScript;

	[Header ("Player Movement")]
	[Range (-1, 1)]
	public float MovementX; // How much horizontal input is read.
	[Range (-1, 1)]
	public float MovementY; // How much vertical input is read.

	public bool UsePlayerFollow = true; // Sets whether the PLayer visual object follows a specific point defined by input.
	public Transform PlayerFollow; 	// The object the input is manipulating.
	public Rigidbody PlayerFollowRb; // The object that follows the player follow GameObject.
	public float PlayerFollowMoveSpeed; // How fast the player follow moves.
	public Vector2 XBounds, YBounds; // Defines bounds for the player position.
	public Rigidbody PlayerRb; // The player Rgidbody which is the same GameObject as the mesh collider for it.
	private float SmoothFollowVelX, SmoothFollowVelY; // Moving with smooth damp provate velocity variables.
	public float SmoothFollowTime = 1; // How much smoothing is applied to the player follow movement.

	public float YRotationAmount = 45; // How many degrees the player objects rotates based on horizontal velocity.
	public float YRotationMultiplier = 10; // Multiplier for the rotation.
	private float RotVelY; // Smooth ref variable for player rotation.
	public float PlayerVibrationDuration; // How long the controller vibrates for.
	public float PlayerVibrationTimeRemaining; // How much time the controller still needs to be vibrating.


	[Header ("Player stats")]
	public int PlayerId = 1; // Player's unique ID.
	public TextMeshProUGUI PlayerText; // Player label.
	public bool isJoined; // Is true if the player is active in the scene.

	// Player's main engine particle effect settings.
	public ParticleSystem MainEngineParticles;
	public float MainEngineParticleEmissionAmount = 1; 
	public float MainEngineParticleEmissionLerpSpeed = 4;

	public Transform ReferencePoint; // To be used to calculate distance and therefore time scale.
	public GameObject PlayerGuides; // Visuals for the player guides.

	// To be shown only in tutorial.
	public RectTransform MiddlePoint; 
	public RectTransform ForegroundPoint;
	public RawImage InputForegroundImage;
	public RawImage InputBackgroundImage;
	public RectTransform InputUIPoint;
	public LineRenderer InputLine;
	public float inputSensitivity = 3;

	[Header ("Shooting")]
	public GameObject CurrentShotObject; 	 // Base GameObject to instantiate when firing.
	public bool canShoot = true; 			 // Allows the player to instantiate the bullet or not.
	public float CurrentFireRate = 0.1f; 	 // Time between bullet spawns.
	public float FireRateTimeMultiplier = 2; // How fast to spawn bullets based on Time.timeScale.
	public float NextFire; 					 // Time.time must be >= for this to allow another shot to be spawned.

	// Shot types.
	public shotType ShotType; 
	public enum shotType
	{
		Standard = 0,
		Double = 1,
		Triple = 2,
		Ripple = 3
	}

	public GameObject StandardShot; 			// Default bullet to be used when there are no powerups.
	public GameObject StandardShotEnhanced; 	// Bullet to be used when ricochet and/or homing are enabled.
	public GameObject StandardShotOverdrive; 	// Bullet to be used when overdrive is enabled. Overrides enhanced shots and properties.
	public shotIteration StandardShotIteration; // Enumerates what shot type to be using for standard shot.
	public Transform StandardShotSpawn; 		// Where we spawn the standard shot.
	public float StandardFireRate = 0.1f; 		// How fast the standard shot fire rate should fire.

	[Header ("Impact")]
	public GameObject playerMesh; // The GameObject which holds the player's mesh and material.
	public Vector3 ImpactPoint;   // Where the impact point was.

	// Player has two colliders, one is in trigger mode.
	public Collider playerCol;
	public Collider playerTrigger;

	public bool isInCooldownMode; 		// Allows cooldown to happen.
	public float cooldownDuration; 		// How long the cooldown duration happens (scaled by time).
	public float cooldownTimeRemaining; // Timer for cooldown.

	// Stuff to do when hit.
	public ParticleSystem PlayerExplosionParticles;
	public AudioSource PlayerExplosionAudio;
	public Animator GlitchEffect; // Allows cool image effects tp play that simulates VHS glitch effects and animates them. 
	public MeshCollider InvincibleCollider;
	public MeshRenderer InvincibleMesh;
	public Animator InvincibleMeshAnim;

	// Stuff to look at when the player runs out of lives on the impact.
	public ParticleSystem GameOverExplosionParticles;
	public AudioSource GameOverExplosionAudio;

	[Header ("Ability")]
	public abilityState CurrentAbilityState;
	public enum abilityState
	{
		Charging, // Ability cannot be used, it is charging in this state.
		Ready, 	  // Ability can be used.
		Active 	  // Ability is being used.
	}

	// Ability stats.
	public float CurrentAbilityDuration; 	  // Maximum ability time.
	public float CurrentAbilityTimeRemaining; // Timer for the ability.

	[Range (0.0f, 1.0f)]
	public float AbilityTimeAmountProportion; 		  // time remining / duration.
	public float AbilityChargeSpeedMultiplier = 0.5f; // How fast the ability bar charges.
	public float AbilityUseSpeedMultiplier = 4;		  // How fast the ability bar diminishes.

	public string AbilityName; // Unique name of the ability.
	public ability Ability;    // Ability list.
	public enum ability
	{
		Shield, // Creates a shield, invincibility for a short time, cool warp screen effect.
		Emp, // Creates a quick exhaust blast of particles which interact with blocks and destroying them.
		VerticalBeam, // Fires particles vertically up, destroying particles in the way.
		HorizontalBeam, // Fires to streams of particles (left and right), destroying falling or stacked blocks that collide with it.
		Rewind // Rewinds time for a certain amount of seconds.
	}

	// Ability UI.
	public GameObject AbilityUI; 		// Ability bar UI.
	public RawImage AbilityImage; 		// Current RawImage of the ability.
	public Texture2D[] AbilityTextures; // Array of ability icons.
	public Image AbilityFillImage; 		// Image fill outline.
	// Colors for each ability state.
	public Color AbilityUseColor, AbilityChargingColor, AbilityChargingFullColor;

	[Header ("Powerups")]
	// General.
	public int powerupsInUse;	// Tracks how many simultaneous powerups are active.
	public bool isHoming; 		// Homing mode is on or off.
	public bool isRicochet; 	// Ricochet mode is on or off.
	public bool isInRapidFire;  // Rapidfire mode is on or off.
	public bool isInOverdrive;  // Overdrive mode is on or off.

	public GameObject RicochetGlowObject; 		   // Static glow bars on the top and bottom of the screen when ricochet mode is on.
	public RawImage[] RicochetGlowMeshes; 		   // Components to access the rendering of the glow bars.
	public ParticleSystem[] RicochetGlowParticles; // Particles that emit off the glow objects.

	// Double shot.
	public GameObject DoubleShotL; 				// Left bullet normal.
	public GameObject DoubleShotLEnhanced; 		// Left bullet enhanced.
	public GameObject DoubleShotLOverdrive; 	// Left bullet overdrive.
	public GameObject DoubleShotR;				// Right bullet normal.
	public GameObject DoubleShotREnhanced;  	// Right bullet enhanced.
	public GameObject DoubleShotROverdrive; 	// Right bullet overdrive.
	public Transform DoubleShotSpawnL;			// Spawn point for left bullet.
	public Transform DoubleShotSpawnR;			// Spawn point for right bullet.
	public float[] DoubleShotFireRates;			// [0] = normal fire rate, [1] = rapid fire rate.
	public shotIteration DoubleShotIteration;	// Enumerates what shot type to be using for double shot.
	public float DoubleShotNextFire;			// Time.time must be >= for this to allow another shot to be spawned.

	// Triple shot.
	public GameObject TripleShotL;				// Left bullet normal.
	public GameObject TripleShotLEnhanced;		// Left bullet enhanced.
	public GameObject TripleShotLOverdrive;		// Left bullet overdrive.
	public GameObject TripleShotM;				// Middle bullet normal.
	public GameObject TripleShotMEnhanced;		// Middle bullet enhanced.
	public GameObject TripleShotMOverdrive;		// Middle bullet overdrive.
	public GameObject TripleShotR;				// Right bullet normal.
	public GameObject TripleShotREnhanced;		// Right bullet enhanced.
	public GameObject TripleShotROverdrive;		// Right bullet overdrive.
	public Transform TripleShotSpawnL;			// Spawn point for left bullet.
	public Transform TripleShotSpawnM;			// Spawn point for middle bullet.
	public Transform TripleShotSpawnR;			// Spawn point for right bullet.
	public float[] TripleShotFireRates;			// [0] = normal fire rate, [1] = rapid fire rate.
	public shotIteration TripleShotIteration;	// Enumerates what shot type to be using for triple shot.
	public float TripleShotNextFire;			// Time.time must be >= for this to allow another shot to be spawned.

	// Ripple shot.
	public GameObject RippleShot;				// Standard ripple shot.
	public GameObject RippleShotEnhanced;		// Enhanced ripple shot.
	public GameObject RippleShotOverdrive;		// Overdrive ripple shot.
	public Transform RippleShotSpawn;			// Where the ripple shot will spawn.
	public float[] RippleShotFireRates;			// [0] = normal fire rate, [1] = rapid fire rate.
	public shotIteration RippleShotIteration;	// Enumerates what shot type to be using for ripple shot.
	public float RippleShotNextFire;			// Time.time must be >= for this to allow another shot to be spawned.

	// Shot iterations on bullet types.
	public enum shotIteration
	{
		Standard = 0, // Default.
		Enhanced = 1, // Ricochet or Homing.
		Rapid = 2,
		Overdrive = 3
	}

	[Header ("Shield")]
	public bool isShieldOn;					// Allows shield visuals and timer to activate.
	public GameObject Shield;				// Shield main object.
	public Lens lensScript;					// Camera effect to simulate gravitational lensing.
	public float TargetShieldScale;			// How large the lensing is.
	public float ShieldScaleSmoothTime = 1; // How slow the transition is for the shield lights to grow/shrink.
	public float LensOnRadius = 0.7f;		// Target lens size when shield is active.
	public float TargetLensRadius;			// Current target lens size.
	public float LensRadiusSmoothTime = 1;	// How slow the transition is for the lensing to grow/shrink.

	[Header ("VerticalBeam")]
	public GameObject VerticalBeam; 			   // The GameObject to set active or not depending whether the ability is being active.
	public ParticleSystem[] VerticalBeamParticles; // Array of particles to emit when enabled.

	[Header ("HorizontalBeam")]
	public GameObject HorizontalBeam; 				 // The GameObject to set active or not depending whether the ability is being active.
	public ParticleSystem[] HorizontalBeamParticles; // Array of particles to emit when enabled.

	[Header ("Emp")]
	public GameObject Emp; 				  // The GameObject to set active or not depending whether the ability is being active.
	public ParticleSystem[] EmpParticles; // Array of particles to emit when enabled.

	[Header ("Turret Player")]
	[Range (0, 4)]
	public int nextTurretSpawn; 					// Which turrent GameObject index to spawn next.
	public GameObject[] Turrets; 					// Turrets in scene.
	public AutoMoveAndRotate TurretRotatorScript; 	// Parented auto rotation script.
	public float TurretSpinSpeed; 					// How fast the turrents spin around the player.
	public float TurretSpinSpeedNormal = -220; 		// Setting while powerup time remaining is >= 3.
	public float TurretSpinSpeedFaster = -660;		// Setting while powerup time remaining is < 3.

	[Header ("Helix")]
	public GameObject Helix; 				// The GameObject to set active or not depending whether the powerup is activated.
	public ParticleSystem[] HelixParticles; // Array of particles to emit when enabled.
	public Collider[] HelixCol; 			// The helix has two colliders, stored here.

	[Header ("Visuals")]
	public Animator FlipScreenAnim; // Animator which controls animations for the screen orientation.

	[Header ("UI")]
	// Ability UI.
	public bool isHidingAbilityUI;
	public Animator AbilityUIHexes; 	// Ability Hexes situated underneath the score.
	public Vector3 AbilityCheckPlayerPos; // Where to check for the player position range.
	[Space (10)]
	// Score UI.
	public bool isHidingScoreUI; 		// Enabled when player position is close to the top middle.
	public Animator ScoreAnim; 			// Score fading in/out animator.
	public Vector3 ScoreCheckPlayerPos; // Where to check for the player position range.
	[Space (10)]
	// Shooting UI.
	public Animator ShootingUIHexes; 	// Shooting Hexes on the top right 
	[Space (10)]
	// Lives UI.
	public bool isHidingLivesUI; 		 // Checks top left of the screen if player is too close to the edge/corner.
	public Animator LivesAnim; 			 // Lives fade in/out animator.
	public Vector2 LivesCheckPlayerPosX; // Range to check horizontal player position.
	public Vector2 LivesCheckPlayerPosY; // Range to check vertical player position.
	[Space (10)]
	// Wave UI.
	public bool isHidingWaveUI; 	   // Checks for wave UI.
	public Animator WaveAnim;		   // Wave fade in/out animator.
	public Vector3 WaveCheckPlayerPos; // point to check for player proximity.
	[Space (10)]
	// PowerupUI
	public bool isHidingPowerupUI;
	public Vector3 PowerupUICheckPos;

	// InControl Player Actions.
	public PlayerActions playerActions; // Created for InControl and assigned at runtime.

	void Start () 
	{
		CreatePlayerActions ();
		AssignActionControls ();
		GetStartPlayerModifiers ();
		CheckPowerupImageUI ();

		InvokeRepeating ("CheckJoinState", 0, 0.5f);
		InvokeRepeating ("TurretRotatorCheck", 0, 0.5f);
		InvincibleMeshAnim.Play ("InvincibleMeshOffInstant");

		RefreshAbilityName ();
		RefreshAbilityImage ();

		TurretSpinSpeed = TurretSpinSpeedNormal;
		LivesAnim.gameObject.SetActive (false);
		CurrentShotObject = StandardShot;

		gameControllerScript.PowerupImage_P1 [0].GetComponent<Animator> ().Play ("PowerupListItemPopIn");

		foreach (RawImage powerupimage in gameControllerScript.PowerupImage_P1) 
		{
			if (powerupimage != gameControllerScript.PowerupImage_P1 [0]) 
			{
				powerupimage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemFadeOutInstant");
			}
		}
	}

	public void StartCoroutines ()
	{
	}

	void GetStartPlayerModifiers ()
	{
		isInRapidFire = gameControllerScript.gameModifier.AlwaysRapidfire;
		isHoming 	  = gameControllerScript.gameModifier.AlwaysHoming;
		isRicochet 	  = gameControllerScript.gameModifier.AlwaysRicochet;
		isInOverdrive = gameControllerScript.gameModifier.AlwaysOverdrive;

		if (gameControllerScript.gameModifier.AlwaysRicochet == true) 
		{
			StandardShotIteration = shotIteration.Enhanced;
			DoubleShotIteration = shotIteration.Enhanced;
			TripleShotIteration = shotIteration.Enhanced;
			RippleShotIteration = shotIteration.Enhanced;
			EnableRicochetObject ();
		}

		if (gameControllerScript.gameModifier.AlwaysOverdrive == true) 
		{
			StandardShotIteration = shotIteration.Overdrive;
			DoubleShotIteration = shotIteration.Overdrive;
			TripleShotIteration = shotIteration.Overdrive;
			RippleShotIteration = shotIteration.Overdrive;
		}
	}

	void Update ()
	{
		MovePlayer ();
		CheckShoot ();
		CheckPlayerVibration ();
		CheckUIVisibility ();
		CheckCooldownTime ();
		CheckAbilityTime ();
		DrawReferencePointLine ();
		UpdateInputUI ();
		CheckPause ();
		CheckCheatConsoleInput ();
	}

	void FixedUpdate ()
	{
		MovePlayerPhysics ();
	}

	void MovePlayerPhysics ()
	{
		if (timescaleControllerScript.isEndSequence == false) 
		{
			// This moves the transform position which the player will follow.
			PlayerFollowRb.velocity = new Vector3 (
				MovementX * PlayerFollowMoveSpeed * Time.fixedUnscaledDeltaTime * (1 / (Time.timeScale + 0.1f)),
				MovementY * PlayerFollowMoveSpeed * Time.fixedUnscaledDeltaTime * (1 / (Time.timeScale + 0.1f)),
				0
			);
		}
	}

	void LateUpdate ()
	{
		if (UsePlayerFollow == true)
		{
			MovePlayerSmoothing ();
		}
	}

	void MovePlayerSmoothing ()
	{
		// Player follows [follow position] with smoothing.
		PlayerRb.position = new Vector3 (
			Mathf.SmoothDamp (
				PlayerRb.position.x, 
				PlayerFollow.position.x, 
				ref SmoothFollowVelX, 
				SmoothFollowTime * Time.fixedUnscaledDeltaTime
			),

			Mathf.SmoothDamp (
				PlayerRb.position.y, 
				PlayerFollow.position.y, 
				ref SmoothFollowVelY, 
				SmoothFollowTime * Time.fixedUnscaledDeltaTime
			),

			0
		);

		// Rotates on y-axis by x velocity.
		PlayerRb.rotation = Quaternion.Euler
			(
				0, 
				Mathf.Clamp (Mathf.SmoothDamp (PlayerRb.rotation.y, -MovementX, ref RotVelY, SmoothFollowTime * Time.unscaledDeltaTime) * YRotationMultiplier, 
					-YRotationAmount, 
					YRotationAmount
				), 
				0
			);

		// Clamps the bounds of the player follow transform.
		PlayerFollow.position = new Vector3 
			(
				Mathf.Clamp (PlayerFollow.position.x, XBounds.x, XBounds.y),
				Mathf.Clamp (PlayerFollow.position.y, YBounds.x, YBounds.y),
				0
			);
	}
		
	void CheckCooldownTime ()
	{
		if (isInCooldownMode == true)
		{
			if (cooldownTimeRemaining > 0) 
			{
				// Timer down for cooldown.
				cooldownTimeRemaining -= Time.unscaledDeltaTime;
			}

			// Cooldown time is finished, re enable the things.
			if (cooldownTimeRemaining <= 0 && gameControllerScript.Lives > 0) 
			{
				RejoinGame ();
				PlayerGuides.SetActive (true);
				AbilityUI.SetActive (true);
				playerCol.gameObject.SetActive (true);
				playerTrigger.gameObject.SetActive (true);
				gameControllerScript.Lives -= 1;
				Invoke ("EnableCollider", 5);
				isInCooldownMode = false;
			}
		}
	}

	void UpdateInputUI ()
	{
		// This maps a square input into a circle.
		InputUIPoint.anchoredPosition = new Vector3 (
			inputSensitivity * playerActions.Move.Value.x * Mathf.Sqrt (1 - playerActions.Move.Value.y * playerActions.Move.Value.y * 0.5f),
			inputSensitivity * playerActions.Move.Value.y * Mathf.Sqrt (1 - playerActions.Move.Value.x * playerActions.Move.Value.x * 0.5f),
			0
		);
			
		InputForegroundImage.rectTransform.sizeDelta = new Vector2 (
			Mathf.Lerp (
				InputForegroundImage.rectTransform.sizeDelta.x, 
				-(1 * (playerActions.Move.Value.normalized.magnitude)) + 2f, 
				8 * Time.unscaledDeltaTime
			), 

			Mathf.Lerp (
				InputForegroundImage.rectTransform.sizeDelta.y, 
				-(1 * (playerActions.Move.Value.normalized.magnitude)) + 2f, 
				8 * Time.unscaledDeltaTime
			)
		);

		// Creates a line between the mid point of the player and the input point.
		InputLine.SetPosition (0, MiddlePoint.position);
		InputLine.SetPosition (1, ForegroundPoint.position);

		// Sets input foreground image opacity color based on input.
		InputForegroundImage.color = new Color (
			0.375f, 
			0.738f, 
			1, 
			Mathf.Lerp (
				InputForegroundImage.color.a, 
				0.4f * playerActions.Move.Value.normalized.magnitude, 
				5 * Time.unscaledDeltaTime
			)
		);

		// Sets input background image opacity color based on input.
		InputBackgroundImage.color = new Color (
			0.375f, 
			0.738f, 
			1, 
			Mathf.Lerp (
				InputForegroundImage.color.a, 
				0.05f * playerActions.Move.Value.normalized.magnitude, 
				5 * Time.unscaledDeltaTime
			)
		);
	}


	// Called by other scrips to set cooldown times.
	public void SetCooldownTime (float cooldownTime)
	{
		cooldownDuration = cooldownTime;
		cooldownTimeRemaining = cooldownDuration;
	}

	// Cooldown sequence.
	public void StartCooldown ()
	{
		if (gameControllerScript.Lives > 0) 
		{
			gameControllerScript.TargetDepthDistance = 0.1f;
			isInCooldownMode = true;
			UsePlayerFollow = false;
			//PlayerFollow.transform.localPosition = new Vector3 (0, 0, 0);
			Invoke ("PlayerTransformPosCooldown", 0.25f);
		}
	}
		
	// Reset the player position ready to have it re enter.
	void PlayerTransformPosCooldown ()
	{
		//playerMesh.transform.localPosition = new Vector3 (0, -15, 0);
	}

	// When the player runs out of lives and unable to respawn.
	public void GameOver ()
	{
		gameControllerScript.isGameOver = true;
		timescaleControllerScript.isEndSequence = true;
		PlayerGuides.SetActive (false);
		canShoot = false;
		UsePlayerFollow = false;
		playerCol.enabled = false;
		playerTrigger.enabled = false;
		playerMesh.SetActive (false);
		PlayerFollowRb.velocity = Vector3.zero;
		GameOverExplosionParticles.Play ();
		GameOverExplosionAudio.Play ();
		audioControllerScript.StopAllSoundtracks ();
		StartCoroutine (timescaleControllerScript.EndSequenceTimeScale ());
	}

	// Impacts by any hazardous object.
	public void PlayerImpactGeneric ()
	{
		SetCooldownTime (5);
		GlitchEffect.Play ("CameraGlitchOn");
		PlayerExplosionParticles.Play ();
		PlayerRb.transform.position = new Vector3 (0, -15, 0);
		StartCoroutine (UseEmp ());
		ResetPowerups ();
		playerCol.enabled = false;
		playerTrigger.enabled = false;
		playerCol.gameObject.SetActive (false);
		playerTrigger.gameObject.SetActive (false);
		PlayerGuides.transform.position = Vector3.zero;
		AbilityUI.SetActive (false);
		PlayerRb.velocity = Vector3.zero;
		PlayerFollowRb.velocity = Vector3.zero;
		MovementX = 0;
		MovementY = 0;
		canShoot = false;
		StartCooldown ();
		PlayerExplosionAudio.Play ();
		gameControllerScript.combo = 1;
		timescaleControllerScript.OverrideTimeScaleTimeRemaining = 2;
		timescaleControllerScript.OverridingTimeScale = 0.25f;
	}

	// Special impacts by blocks.
	public void PlayerBlockImpact (Block ImpactBlock)
	{
		ImpactPoint = ImpactBlock.transform.position;
		PlayerExplosionParticles.transform.position = ImpactPoint;
		Instantiate (ImpactBlock.playerExplosion, ImpactPoint, Quaternion.identity);
		camShakeScript.ShakeCam (ImpactBlock.newCamShakeAmount * 10, ImpactBlock.newCamShakeAmount * 10, 99);
		audioControllerScript.SetTargetLowPassFreq (ImpactBlock.LowPassTargetFreq);
		audioControllerScript.SetTargetResonance (ImpactBlock.ResonanceTargetFreq);
	}

	// Special impacts by hazards.
	public void PlayerHazardImpact (Hazard ImpactHazard)
	{
		ImpactPoint = ImpactHazard.transform.position;
		PlayerExplosionParticles.transform.position = ImpactPoint;
		Instantiate (ImpactHazard.playerExplosion, ImpactPoint, Quaternion.identity);
		camShakeScript.ShakeCam (ImpactHazard.newCamShakeAmount * 10, ImpactHazard.newCamShakeAmount * 10, 98);
		ImpactHazard.SetTargetLowPassFreq (ImpactHazard.LowPassTargetFreq);
		ImpactHazard.SetTargetResonance (ImpactHazard.ResonanceTargetFreq);
	}

	// When cooldown time is complete.
	void EnableCollider ()
	{
		UsePlayerFollow = true; // Allow player input to manipulate player position.

		// Checks for gode mode, allows god mode to stay on if needed.
		if (developerModeScript.isGod == false) 
		{
			playerCol.enabled = true;
			playerTrigger.enabled = true;
		}
	}

	// Allows player to re enter while temporarily invincible.
	void RejoinGame ()
	{
		gameControllerScript.UpdateLives ();
		PlayerFollow.transform.position = Vector3.zero;
		canShoot = true;
		UsePlayerFollow = true;
		playerMesh.SetActive (true);
		audioControllerScript.TargetCutoffFreq = 22000;
		audioControllerScript.TargetResonance = 1;
		InvincibleMeshAnim.Play ("InvincibleMeshFlash");
	}

	// Allows player input. Gets called by player parent script.
	public void EnablePlayerInput ()
	{
		UsePlayerFollow = true;
		canShoot = true;
		StartCoroutines ();
	}

	// Reads input from InControl Player Actions and sets values.
	void MovePlayer ()
	{
		if (gameControllerScript.isGameOver == false)
		{
			if (UsePlayerFollow == true && timescaleControllerScript.isEndSequence == false)
			{
				if (gameControllerScript.isPaused == false) 
				{
					// Reads movement input on two axis.
					if (FlipScreenAnim.transform.eulerAngles.z < 90) // When screen is right way up.
					{
						MovementX = playerActions.Move.Value.x;
						MovementY = playerActions.Move.Value.y;
					}

					if (FlipScreenAnim.transform.eulerAngles.z >= 90) // When screen is flipped.
					{
						MovementX = -playerActions.Move.Value.x;
						MovementY = -playerActions.Move.Value.y;
					}
				}
			}

			/*
			// Test hotkey to have screen flipping.
			if (Input.GetKeyDown (KeyCode.Return)) 
			{
				FlipScreenAnim.SetBool ("Flipped", true);
			}

			if (Input.GetKeyDown (KeyCode.Quote)) 
			{
				FlipScreenAnim.SetBool ("Flipped", false);
			}*/

			var MainEngineEmissionRate = MainEngineParticles.emission;
			float SmoothEmissionRate = 
				Mathf.Lerp (
					MainEngineEmissionRate.rateOverTime.constant, 
					MainEngineParticleEmissionAmount * playerActions.Move.Up.Value,
					MainEngineParticleEmissionLerpSpeed * Time.deltaTime
				);
			MainEngineEmissionRate.rateOverTime = SmoothEmissionRate;
		}
	}

	void CheckAbilityTime ()
	{
		// Updates the ability UI involved.
		AbilityTimeAmountProportion = CurrentAbilityTimeRemaining / CurrentAbilityDuration;
		AbilityFillImage.fillAmount = 1f * AbilityTimeAmountProportion;

		// Player presses ability button.
		if (playerActions.Ability.WasPressed && 
			gameControllerScript.isPaused == false && 
			playerCol.enabled == true && 
			isInCooldownMode == false) 
		{
			// Ability is charged.
			if (CurrentAbilityState == abilityState.Ready && 
				cooldownTimeRemaining <= 0 &&
				timescaleControllerScript.isInInitialSequence == false && 
				timescaleControllerScript.isInInitialCountdownSequence == false) 
			{
				ActivateAbility ();
				CurrentAbilityState = abilityState.Active;
			}
		}

		// Updates the ability timers.
		if (CurrentAbilityState == abilityState.Active) 
		{
			AbilityFillImage.color = AbilityUseColor;

			if (CurrentAbilityTimeRemaining > 0)
			{
				CurrentAbilityTimeRemaining -= 0.75f * AbilityUseSpeedMultiplier * Time.unscaledDeltaTime;
			}

			if (CurrentAbilityTimeRemaining <= 0) 
			{
				CurrentAbilityState = abilityState.Charging;
				DeactivateAbility ();
			}

			CurrentAbilityTimeRemaining = Mathf.Clamp (CurrentAbilityTimeRemaining, 0, CurrentAbilityDuration);
		}

		if (CurrentAbilityState == abilityState.Ready) 
		{
			AbilityFillImage.color = AbilityChargingFullColor;
		}

		if (CurrentAbilityState == abilityState.Charging) 
		{
			if (CurrentAbilityTimeRemaining < CurrentAbilityDuration && 
				cooldownTimeRemaining <= 0 &&
				timescaleControllerScript.isInInitialSequence == false && 
				timescaleControllerScript.isInInitialCountdownSequence == false && 
				gameControllerScript.isPaused == false &&
				tutorialManagerScript.tutorialComplete == true) 
			{
				CurrentAbilityTimeRemaining += AbilityChargeSpeedMultiplier * Time.unscaledDeltaTime; // Add slowdown.
			}

			if (CurrentAbilityTimeRemaining >= CurrentAbilityDuration) 
			{
				CurrentAbilityTimeRemaining = CurrentAbilityDuration;
				CurrentAbilityState = abilityState.Ready;
			}

			if (AbilityTimeAmountProportion < 1f)
			{
				AbilityFillImage.color = AbilityChargingColor;
			}
		}

		lensScript.radius = Mathf.Lerp (lensScript.radius, TargetLensRadius, LensRadiusSmoothTime * Time.unscaledDeltaTime);

		/*Shield.transform.localScale = new Vector3 (
			Mathf.Lerp (Shield.transform.localScale.x, TargetShieldScale, ShieldScaleSmoothTime * Time.unscaledDeltaTime), 
			Mathf.Lerp (Shield.transform.localScale.y, TargetShieldScale, ShieldScaleSmoothTime * Time.unscaledDeltaTime), 
			Mathf.Lerp (Shield.transform.localScale.z, TargetShieldScale, ShieldScaleSmoothTime * Time.unscaledDeltaTime)
		);*/

		Vector3 targetShieldScale = new Vector3 (TargetShieldScale, TargetShieldScale, TargetShieldScale);
		Shield.transform.localScale = Vector3.Lerp (Shield.transform.localScale, targetShieldScale, ShieldScaleSmoothTime * Time.unscaledDeltaTime);
	}

	// Activates ability based on current setting.
	public void ActivateAbility ()
	{
		switch (Ability) 
		{
		case ability.Shield:
			isShieldOn = true;
			playerCol.enabled = false;
			playerTrigger.enabled = false;
			Shield.SetActive (true);
			TargetLensRadius = LensOnRadius;
			TargetShieldScale = 1;
			break;
		case ability.Emp:
			Emp.transform.position = playerCol.transform.position;
			Emp.SetActive (true);
			break;
		case ability.VerticalBeam:
			VerticalBeam.SetActive (true);
			break;
		case ability.HorizontalBeam:
			HorizontalBeam.SetActive (true);
			break;
		case ability.Rewind:
			timescaleControllerScript.SetRewindTime (true, 8);
			break;
		}

		// Briefly slows time down for effect.
		timescaleControllerScript.OverrideTimeScaleTimeRemaining = 0.75f;
		timescaleControllerScript.OverridingTimeScale = 0.3f;

		camShakeScript.ShakeCam (0.4f, CurrentAbilityDuration, 6);
	}

	// Use Emp ability.
	public IEnumerator UseEmp ()
	{
		Emp.transform.position = ImpactPoint;
		Emp.gameObject.SetActive (true);
		EmpParticles [0].Play (true);
		EmpParticles [1].Play (true);
		yield return new WaitForSeconds (3);
		EmpParticles [0].Stop (true, ParticleSystemStopBehavior.StopEmitting);
		EmpParticles [1].Stop (true, ParticleSystemStopBehavior.StopEmitting);
		yield return new WaitForSeconds (3);
		Emp.gameObject.SetActive (false);
	}

	// Turn off ability when timer runs out. But allow things to gradually disappear by invoking with delay.
	public void DeactivateAbility ()
	{
		// Deactivates shield.
		isShieldOn = false;
		TargetLensRadius = 0;
		TargetShieldScale = 0;
		Invoke ("DeactivateShield", 1);

		// Deactivates vertcial beam.
		Invoke ("DeactivateVerticalBeam", 3);
		foreach (ParticleSystem vbParticles in VerticalBeamParticles) 
		{
			vbParticles.Stop ();
		}

		// Deactivates horizontal shield.
		Invoke ("DeactivateHorizontalBeam", 3);
		foreach (ParticleSystem hzParticles in HorizontalBeamParticles)
		{
			hzParticles.Stop ();
		}

		// Deactivates Emp.
		Invoke ("DeactivateEmp", 3);
		foreach (ParticleSystem empParticles in EmpParticles) 
		{
			empParticles.Stop ();
		}

		StopRewinding (); // Stops rewinding.

		// Reset the camera shake.
		camShakeScript.ShakeCam (0.0f, 0, 7);
	}

	void StopRewinding ()
	{
		// Stop rewinding.
		timescaleControllerScript.SetRewindTime (false, 0);
	}

	// Deactivate Shield.
	void DeactivateShield ()
	{
		if (developerModeScript.isGod == false) 
		{
			playerCol.enabled = true;
			playerTrigger.enabled = true;
		}
		Shield.SetActive (false);
	}

	// Deactivate vertical beam.
	void DeactivateVerticalBeam ()
	{
		VerticalBeam.SetActive (false);
	}

	// Deactivate horizontal beam.
	void DeactivateHorizontalBeam ()
	{
		HorizontalBeam.SetActive (false);
	}

	// Deactivate emp.
	void DeactivateEmp ()
	{
		Emp.SetActive (false);
	}

	// Sync ability name in the list value.
	public void RefreshAbilityName ()
	{
		AbilityName = Ability.ToString ();

		/*switch (Ability) 
		{
		case ability.Shield:
			AbilityName = "Shield";
			break;
		case ability.Emp:
			AbilityName = "Emp";
			break;
		case ability.VerticalBeam:
			AbilityName = "VerticalBeam";
			break;
		case ability.HorizontalBeam:
			AbilityName = "HorizontalBeam";
			break;
		}*/
	}

	// Sync ability image.
	public void RefreshAbilityImage ()
	{
		switch (Ability) 
		{
		case ability.Shield:
			AbilityImage.texture = AbilityTextures [0];
			break;
		case ability.Emp:
			AbilityImage.texture = AbilityTextures [1];
			break;
		case ability.VerticalBeam:
			AbilityImage.texture = AbilityTextures [2];
			break;
		case ability.HorizontalBeam:
			AbilityImage.texture = AbilityTextures [3];
			break;
		case ability.Rewind:
			AbilityImage.texture = AbilityTextures [4];
			break;
		}
	}
		
	void DrawReferencePointLine ()
	{
		Debug.DrawLine (playerCol.transform.position, ReferencePoint.transform.position, Color.red);
	}

	// Checks shooting state.
	void CheckShoot ()
	{
		if (canShoot == true) 
		{
			if (playerActions.Shoot.Value > 0.75f && Time.time >= NextFire && gameControllerScript.isPaused == false) 
			{
				// Every time the player shoots, decremement the combo.
				if (gameControllerScript.combo > 1)
				{
					gameControllerScript.combo -= 1;
				}

				Shoot ();

				NextFire = Time.time + (CurrentFireRate / (FireRateTimeMultiplier * Time.timeScale));
			}
		}
	}

	// Instantiates relevant bullet based on what bullet iteration it is on and what shooting powerup is active.
	void Shoot ()
	{
		switch (ShotType) 
		{
		case shotType.Standard:
			switch (StandardShotIteration)
			{
			case shotIteration.Standard:
				GameObject shot = Instantiate (StandardShot, StandardShotSpawn.position, StandardShotSpawn.rotation);
				shot.GetComponent<Bullet> ().playerControllerScript = this;
				shot.GetComponent<Bullet> ().playerPos = playerCol.transform;
				shot.name = "Standard Shot_P" + PlayerId + " (Standard)";
				CurrentShotObject = StandardShot;
				break;
			case shotIteration.Enhanced:
				GameObject shotEnhanced = Instantiate (StandardShotEnhanced, StandardShotSpawn.position, StandardShotSpawn.rotation);
				shotEnhanced.GetComponent<Bullet> ().playerControllerScript = this;
				shotEnhanced.GetComponent<Bullet> ().playerPos = playerCol.transform;
				shotEnhanced.name = "Standard Shot_P" + PlayerId + " (Enhanced)";
				CurrentShotObject = StandardShotEnhanced;
				break;
			case shotIteration.Rapid:
				GameObject shotRapid = Instantiate (StandardShotEnhanced, StandardShotSpawn.position, StandardShotSpawn.rotation);
				shotRapid.GetComponent<Bullet> ().playerControllerScript = this;
				shotRapid.GetComponent<Bullet> ().playerPos = playerCol.transform;
				shotRapid.name = "Standard Shot_P" + PlayerId + " (Enhanced)";
				CurrentShotObject = StandardShotEnhanced;
				break;
			case shotIteration.Overdrive:
				GameObject shotOverdrive = Instantiate (StandardShotOverdrive, StandardShotSpawn.position, StandardShotSpawn.rotation);
				shotOverdrive.GetComponent<Bullet> ().playerControllerScript = this;
				shotOverdrive.GetComponent<Bullet> ().playerPos = playerCol.transform;
				shotOverdrive.name = "Standard Shot_P" + PlayerId + " (Overdrive)";
				CurrentShotObject = StandardShotOverdrive;
				break;
			}
			break;

		case shotType.Double:
			switch (DoubleShotIteration)
			{
			case shotIteration.Standard:
				GameObject doubleshotL = Instantiate (DoubleShotL, DoubleShotSpawnL.position, DoubleShotSpawnL.rotation);
				GameObject doubleshotR = Instantiate (DoubleShotR, DoubleShotSpawnR.position, DoubleShotSpawnR.rotation);
				doubleshotL.GetComponent<Bullet> ().playerControllerScript = this;
				doubleshotL.GetComponent<Bullet> ().playerPos = playerCol.transform;
				doubleshotR.GetComponent<Bullet> ().playerControllerScript = this;
				doubleshotR.GetComponent<Bullet> ().playerPos = playerCol.transform;
				doubleshotL.name = "Double ShotL_P" + PlayerId + " (Standard)";
				doubleshotR.name = "Double ShotR_P" + PlayerId + " (Standard)";
				CurrentShotObject = DoubleShotL;
					break;
			case shotIteration.Enhanced:
				GameObject doubleshotLEnhanced = Instantiate (DoubleShotLEnhanced, DoubleShotSpawnL.position, DoubleShotSpawnL.rotation);
				GameObject doubleshotREnhanced = Instantiate (DoubleShotREnhanced, DoubleShotSpawnR.position, DoubleShotSpawnR.rotation);
				doubleshotLEnhanced.GetComponent<Bullet> ().playerControllerScript = this;
				doubleshotLEnhanced.GetComponent<Bullet> ().playerPos = playerCol.transform;
				doubleshotREnhanced.GetComponent<Bullet> ().playerControllerScript = this;
				doubleshotREnhanced.GetComponent<Bullet> ().playerPos = playerCol.transform;
				doubleshotLEnhanced.name = "Double ShotL_P" + PlayerId + " (Enhanced)";
				doubleshotREnhanced.name = "Double ShotR_P" + PlayerId + " (Enhanced)";
				CurrentShotObject = DoubleShotLEnhanced;
					break;
				case shotIteration.Rapid:
				GameObject doubleshotLRapid = Instantiate (DoubleShotLEnhanced, DoubleShotSpawnL.position, DoubleShotSpawnL.rotation);
				GameObject doubleshotRRapid = Instantiate (DoubleShotREnhanced, DoubleShotSpawnR.position, DoubleShotSpawnR.rotation);
				doubleshotLRapid.GetComponent<Bullet> ().playerControllerScript = this;
				doubleshotLRapid.GetComponent<Bullet> ().playerPos = playerCol.transform;
				doubleshotRRapid.GetComponent<Bullet> ().playerControllerScript = this;
				doubleshotRRapid.GetComponent<Bullet> ().playerPos = playerCol.transform;
				doubleshotLRapid.name = "Double ShotL_P" + PlayerId + " (Rapid)";
				doubleshotRRapid.name = "Double ShotR_P" + PlayerId + " (Rapid)";
				CurrentShotObject = DoubleShotLEnhanced;
					break;
				case shotIteration.Overdrive:
				GameObject doubleshotLOverdrive = Instantiate (DoubleShotLOverdrive, DoubleShotSpawnL.position, DoubleShotSpawnL.rotation);
				GameObject doubleshotROverdrive = Instantiate (DoubleShotROverdrive, DoubleShotSpawnR.position, DoubleShotSpawnR.rotation);
				doubleshotLOverdrive.GetComponent<Bullet> ().playerControllerScript = this;
				doubleshotLOverdrive.GetComponent<Bullet> ().playerPos = playerCol.transform;
				doubleshotROverdrive.GetComponent<Bullet> ().playerControllerScript = this;
				doubleshotROverdrive.GetComponent<Bullet> ().playerPos = playerCol.transform;
				doubleshotLOverdrive.name = "Double ShotL_P" + PlayerId + " (Overdrive)";
				doubleshotROverdrive.name = "Double ShotR_P" + PlayerId + " (Overdrive)";
				CurrentShotObject = DoubleShotLOverdrive;
					break;
			}

			break;

		case shotType.Triple:
			switch (TripleShotIteration) 
			{
			case shotIteration.Standard:
				GameObject tripleShotLStandard = Instantiate (TripleShotL, TripleShotSpawnL.position, TripleShotSpawnL.rotation);
				GameObject tripleShotMStandard = Instantiate (TripleShotM, TripleShotSpawnM.position, TripleShotSpawnM.rotation);
				GameObject tripleShotRStandard = Instantiate (TripleShotR, TripleShotSpawnR.position, TripleShotSpawnR.rotation);
				tripleShotLStandard.GetComponent<Bullet> ().playerControllerScript = this;
				tripleShotMStandard.GetComponent<Bullet> ().playerControllerScript = this;
				tripleShotRStandard.GetComponent<Bullet> ().playerControllerScript = this;
				tripleShotLStandard.name = "Triple ShotL_P" + PlayerId + " (Standard)";
				tripleShotMStandard.name = "Triple ShotM_P" + PlayerId + " (Standard)";
				tripleShotRStandard.name = "Triple ShotR_P" + PlayerId + " (Standard)";
				CurrentShotObject = TripleShotM;
					break;
			case shotIteration.Enhanced:
				GameObject tripleShotLEnhanced = Instantiate (TripleShotLEnhanced, TripleShotSpawnL.position, TripleShotSpawnL.rotation);
				GameObject tripleShotMEnhanced = Instantiate (TripleShotMEnhanced, TripleShotSpawnM.position, TripleShotSpawnM.rotation);
				GameObject tripleShotREnhanced = Instantiate (TripleShotREnhanced, TripleShotSpawnR.position, TripleShotSpawnR.rotation);
				tripleShotLEnhanced.GetComponent<Bullet> ().playerControllerScript = this;
				tripleShotMEnhanced.GetComponent<Bullet> ().playerControllerScript = this;
				tripleShotREnhanced.GetComponent<Bullet> ().playerControllerScript = this;
				tripleShotLEnhanced.name = "Triple ShotL_P" + PlayerId + " (Enhanced)";
				tripleShotMEnhanced.name = "Triple ShotM_P" + PlayerId + " (Enhanced)";
				tripleShotREnhanced.name = "Triple ShotR_P" + PlayerId + " (Enhanced)";
				CurrentShotObject = TripleShotMEnhanced;
					break;
				case shotIteration.Rapid:
				GameObject tripleShotLRapid = Instantiate (TripleShotLEnhanced, TripleShotSpawnL.position, TripleShotSpawnL.rotation);
				GameObject tripleShotMRapid = Instantiate (TripleShotMEnhanced, TripleShotSpawnM.position, TripleShotSpawnM.rotation);
				GameObject tripleShotRRapid = Instantiate (TripleShotREnhanced, TripleShotSpawnR.position, TripleShotSpawnR.rotation);
				tripleShotLRapid.GetComponent<Bullet> ().playerControllerScript = this;
				tripleShotMRapid.GetComponent<Bullet> ().playerControllerScript = this;
				tripleShotRRapid.GetComponent<Bullet> ().playerControllerScript = this;
				tripleShotLRapid.name = "Triple ShotL_P" + PlayerId + " (Rapid)";
				tripleShotMRapid.name = "Triple ShotM_P" + PlayerId + " (Rapid)";
				tripleShotRRapid.name = "Triple ShotR_P" + PlayerId + " (Rapid)";
				CurrentShotObject = TripleShotMEnhanced;
					break;
				case shotIteration.Overdrive:
				GameObject tripleShotLOverdrive = Instantiate (TripleShotLOverdrive, TripleShotSpawnL.position, TripleShotSpawnL.rotation);
				GameObject tripleShotMOverdrive = Instantiate (TripleShotMOverdrive, TripleShotSpawnM.position, TripleShotSpawnM.rotation);
				GameObject tripleShotROverdrive = Instantiate (TripleShotROverdrive, TripleShotSpawnR.position, TripleShotSpawnR.rotation);
				tripleShotLOverdrive.GetComponent<Bullet> ().playerControllerScript = this;
				tripleShotMOverdrive.GetComponent<Bullet> ().playerControllerScript = this;
				tripleShotROverdrive.GetComponent<Bullet> ().playerControllerScript = this;
				tripleShotLOverdrive.name = "Triple ShotL_P" + PlayerId + " (Overdrive)";
				tripleShotMOverdrive.name = "Triple ShotM_P" + PlayerId + " (Overdrive)";
				tripleShotROverdrive.name = "Triple ShotR_P" + PlayerId + " (Overdrive)";
				CurrentShotObject = TripleShotMOverdrive;
					break;
			}

			break;

		case shotType.Ripple:
			switch (RippleShotIteration)
			{
			case shotIteration.Standard:
				GameObject rippleShotStandard = Instantiate (RippleShot, RippleShotSpawn.position, Quaternion.Euler (0, 0, 0));
				rippleShotStandard.GetComponent<Bullet> ().playerControllerScript = this;
				rippleShotStandard.GetComponent<Bullet> ().playerControllerScript = this;
				rippleShotStandard.GetComponent<Bullet> ().playerControllerScript = this;
				rippleShotStandard.name = "Ripple Shot_P" + PlayerId + " (Standard)";
				CurrentShotObject = RippleShot;
				break;
			case shotIteration.Enhanced:
				GameObject rippleShotEnhanced = Instantiate (RippleShotEnhanced, RippleShotSpawn.position, Quaternion.Euler (0, 0, 0));
				rippleShotEnhanced.GetComponent<Bullet> ().playerControllerScript = this;
				rippleShotEnhanced.GetComponent<Bullet> ().playerControllerScript = this;
				rippleShotEnhanced.GetComponent<Bullet> ().playerControllerScript = this;
				rippleShotEnhanced.name = "Ripple Shot_P" + PlayerId + " (Enhanced)";
				CurrentShotObject = RippleShotEnhanced;
				break;
			case shotIteration.Rapid:
				GameObject rippleShotRapid = Instantiate (RippleShotEnhanced, RippleShotSpawn.position, Quaternion.Euler (0, 0, 0));
				rippleShotRapid.GetComponent<Bullet> ().playerControllerScript = this;
				rippleShotRapid.GetComponent<Bullet> ().playerControllerScript = this;
				rippleShotRapid.GetComponent<Bullet> ().playerControllerScript = this;
				rippleShotRapid.name = "Ripple Shot_P" + PlayerId + " (Rapid)";
				CurrentShotObject = RippleShotEnhanced;
				break;
			case shotIteration.Overdrive:
				GameObject rippleShotOverdrive = Instantiate (RippleShotOverdrive, RippleShotSpawn.position, Quaternion.Euler (0, 0, 0));
				rippleShotOverdrive.GetComponent<Bullet> ().playerControllerScript = this;
				rippleShotOverdrive.GetComponent<Bullet> ().playerControllerScript = this;
				rippleShotOverdrive.GetComponent<Bullet> ().playerControllerScript = this;
				rippleShotOverdrive.name = "Ripple Shot_P" + PlayerId + " (Overdrive)";
				CurrentShotObject = RippleShotOverdrive;
				break;
			}
			break;
		}
	}

	// Gets pause state.
	void CheckPause ()
	{
		if (playerActions.Pause.WasPressed) 
		{
			if (Time.unscaledTime > gameControllerScript.NextPauseCooldown) 
			{
				gameControllerScript.CheckPause ();
			}
		}

		if (gameControllerScript.isPaused == true) 
		{
			if (isShieldOn == true) 
			{
				if (lensScript.enabled == true) 
				{
					lensScript.enabled = false;
				}
			}
		}

		if (gameControllerScript.isPaused == false) 
		{
			if (isShieldOn == true) 
			{
				if (lensScript.enabled == false) 
				{
					lensScript.enabled = true;
				}
			}
		}
	}

	// Autohiding UI.
	void CheckUIVisibility ()
	{
		// ABILITY UI
		// When the player is close to the ability UI.
		// Vertical position.
		if (PlayerRb.position.y > AbilityCheckPlayerPos.y && isInCooldownMode == false) 
		{
			// Horizontal position too far.
			if (PlayerRb.position.x < AbilityCheckPlayerPos.x) 
			{
				if (AbilityUIHexes.GetCurrentAnimatorStateInfo (0).IsName ("HexesFadeOut") == false && isHidingAbilityUI == false) 
				{
					AbilityUIHexes.Play ("HexesFadeOut");
					isHidingAbilityUI = true;
				}
			}

			// Horizontal position in range.
			if (PlayerRb.position.x >= AbilityCheckPlayerPos.x) 
			{
				if (AbilityUIHexes.GetCurrentAnimatorStateInfo (0).IsName ("HexesFadeIn") == false && isHidingAbilityUI == true) 
				{
					AbilityUIHexes.Play ("HexesFadeIn");
					isHidingAbilityUI = false;
				}
			}
		}

		// Vertical position too far from score text.
		if (PlayerRb.position.y <= AbilityCheckPlayerPos.y && isInCooldownMode == false) 
		{
			if (AbilityUIHexes.GetCurrentAnimatorStateInfo (0).IsName ("HexesFadeIn") == false && isHidingAbilityUI == true) 
			{
				AbilityUIHexes.Play ("HexesFadeIn");
				isHidingAbilityUI = false;
			}
		}

		// SCORE UI
		// When the player is close to the score text.
		// Vertical position.
		if (PlayerRb.position.y > ScoreCheckPlayerPos.y && isInCooldownMode == false) 
		{
			// Horizontal position too far.
			if (PlayerRb.position.x > -ScoreCheckPlayerPos.x && PlayerRb.position.x < ScoreCheckPlayerPos.x) 
			{
				if (ScoreAnim.GetCurrentAnimatorStateInfo (0).IsName ("ScoreFadeOut") == false && isHidingScoreUI == false) 
				{
					ScoreAnim.Play ("ScoreFadeOut");
					isHidingScoreUI = true;
				}
			}

			// Horizontal position in range.
			if (PlayerRb.position.x <= -ScoreCheckPlayerPos.x || PlayerRb.position.x >= ScoreCheckPlayerPos.x && isInCooldownMode == false)  
			{
				if (ScoreAnim.GetCurrentAnimatorStateInfo (0).IsName ("ScoreFadeIn") == false && isHidingScoreUI == true) 
				{
					ScoreAnim.Play ("ScoreFadeIn");
					isHidingScoreUI = false;
				}
			}
		}

		// Vertical position too far from score text.
		if (PlayerRb.position.y <= ScoreCheckPlayerPos.y && isInCooldownMode == false) 
		{
			if (ScoreAnim.GetCurrentAnimatorStateInfo (0).IsName ("ScoreFadeIn") == false && isHidingScoreUI == true) 
			{
				ScoreAnim.Play ("ScoreFadeIn");
				isHidingScoreUI = false;
			}
		}

		// LIVES UI
		// When the player is close to the lives text.
		// Vertical position.
		if (PlayerRb.position.y > LivesCheckPlayerPosY.y && LivesAnim.gameObject.activeInHierarchy == true && isInCooldownMode == false) 
		{
			// Horizontal position too far.
			if (PlayerRb.position.x < LivesCheckPlayerPosX.x) 
			{
				if (LivesAnim.GetCurrentAnimatorStateInfo (0).IsName ("LivesFadeOut") == false && isHidingLivesUI == false) 
				{
					LivesAnim.Play ("LivesFadeOut");
					isHidingLivesUI = true;
				}
			}

			// Horizontal position in range.
			if (PlayerRb.position.x >= LivesCheckPlayerPosX.x) 
			{
				if (LivesAnim.GetCurrentAnimatorStateInfo (0).IsName ("LivesFadeIn") == false && isHidingLivesUI == true) 
				{
					LivesAnim.Play ("LivesFadeIn");
					isHidingLivesUI = false;
				}
			}
		}

		// Vertical position too far from lives text.
		if (PlayerRb.position.y <= LivesCheckPlayerPosY.y && LivesAnim.gameObject.activeInHierarchy == true && isInCooldownMode == false) 
		{
			if (LivesAnim.GetCurrentAnimatorStateInfo (0).IsName ("LivesFadeIn") == false && isHidingLivesUI == true) 
			{
				LivesAnim.Play ("LivesFadeIn");
				isHidingLivesUI = false;
			}
		}

		/*
		// WAVE TEXT
		// When the player is close to the wave text.
		// Vertical position.
		if (PlayerRb.position.y > WaveCheckPlayerPos.y) 
		{
			// Horizontal position too far.
			if (PlayerRb.position.x > -WaveCheckPlayerPos.x && PlayerRb.position.x < WaveCheckPlayerPos.x) 
			{
				if (WaveAnim.GetCurrentAnimatorStateInfo (0).IsName ("WaveUIExit") == false && isHidingWaveUI == false) 
				{
					WaveAnim.Play ("WaveUIExit");
					isHidingWaveUI = true;
				}
			}

			// Horizontal position in range.
			if (PlayerRb.position.x <= -WaveCheckPlayerPos.x || PlayerRb.position.x >= WaveCheckPlayerPos.x) 
			{
				if (WaveAnim.GetCurrentAnimatorStateInfo (0).IsName ("WaveUIEnter") == false && isHidingWaveUI == true) 
				{
					WaveAnim.Play ("WaveUIEnter");
					isHidingWaveUI = false;
				}
			}
		}

		// Vertical position too far from lives text.
		if (PlayerRb.position.y <= WaveCheckPlayerPos.y) 
		{
			if (WaveAnim.GetCurrentAnimatorStateInfo (0).IsName ("WaveUIEnter") == false && isHidingWaveUI == true) 
			{
				WaveAnim.Play ("WaveUIEnter");
				isHidingWaveUI = false;
			}
		}*/

		CheckPowerupImageUI ();

		// Defaults powerup texture with standard shot image.
		if (gameControllerScript.PowerupImage_P1 [0].texture == null) 
		{
			gameControllerScript.PowerupImage_P1 [0].color = Color.white;
			gameControllerScript.PowerupImage_P1 [0].texture = gameControllerScript.StandardShotTexture;
		}
	}

	// Tracks powerup images and available slots to populate.
	// Also has autohiding.
	public void CheckPowerupImageUI ()
	{
		/*
		if (playerCol.transform.position.y > 7 && playerCol.transform.position.x > 12) 
		{
			if (ShootingUIHexes.GetCurrentAnimatorStateInfo (0).IsName ("HexesFadeOut") == false)
			{
				ShootingUIHexes.Play ("HexesFadeOut");
			}
		}

		if (playerCol.transform.position.y <= 7 || playerCol.transform.position.x <= 12) 
		{
			if (ShootingUIHexes.GetCurrentAnimatorStateInfo (0).IsName ("HexesFadeOut") == true)
			{
				ShootingUIHexes.Play ("HexesFadeIn");
			}
		}*/

		foreach (RawImage powerupimage in gameControllerScript.PowerupImage_P1)
		{
			if (powerupimage == gameControllerScript.PowerupImage_P1 [0])
			{
				if (playerCol.transform.position.y > PowerupUICheckPos.y &&
					playerCol.transform.position.x > PowerupUICheckPos.x
				) 
				{
					if (powerupimage.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("PowerupListItemFadeOut") == false
						&& powerupimage.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("PowerupListItemPopIn") == false
						&& isHidingPowerupUI == false) 
					{
						powerupimage.GetComponent<Animator> ().Play ("PowerupListItemFadeOut");
						isHidingPowerupUI = true;
					}
				}

				if (playerCol.transform.position.y <= PowerupUICheckPos.y || 
					playerCol.transform.position.x <= PowerupUICheckPos.x)
				{
					if (powerupimage.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("PowerupListItemFadeOut") == true
						&& powerupimage.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("PowerupListItemPopIn") == false
						&& isHidingPowerupUI == true) 
					{
						powerupimage.GetComponent<Animator> ().Play ("PowerupListItemFadeIn");
						isHidingPowerupUI = false;
					}
				}
			}
		}
	}

	// Turning off Powerups.

	// Turn off Helix.
	void TurnOffHelix ()
	{
		Helix.SetActive (false);
	}

	// Tracks rotation amount degrees per second based on powerup time remaining.
	void TurretRotatorCheck ()
	{
		TurretRotatorScript.rotateDegreesPerSecond.value = new Vector3 (0, 0, TurretSpinSpeed);

		if (gameControllerScript.PowerupTimeRemaining > 3) 
		{
			TurretSpinSpeed = TurretSpinSpeedNormal;
		}
	}

	// Shows ricochet visuals and particle effects.
	public void EnableRicochetObject ()
	{
		foreach (ParticleSystem glowParticles in RicochetGlowParticles)
		{
			glowParticles.Play ();
		}

		foreach (RawImage meshrendglow in RicochetGlowMeshes) 
		{
			meshrendglow.enabled = true;
		}
	}


	// Resets all active powerups back to standard shot. Does not modify modifiers if enabled.
	public void ResetPowerups ()
	{
		// Sets all shot types to standard iteration.
		ShotType = shotType.Standard;

		nextTurretSpawn = 0; // Resets turrent spawn index.

		// Resets homing mode if not modified by game modifier object.
		isHoming = gameControllerScript.gameModifier.AlwaysHoming;
		isRicochet = gameControllerScript.gameModifier.AlwaysRicochet;
		isInRapidFire = gameControllerScript.gameModifier.AlwaysRapidfire;
		isInOverdrive = gameControllerScript.gameModifier.AlwaysOverdrive;

		// Resets ricochet mode if not modified by game modifier object.
		if (gameControllerScript.gameModifier.AlwaysRicochet == false)
		{
			foreach (ParticleSystem glowParticles in RicochetGlowParticles)
			{
				glowParticles.Stop (true, ParticleSystemStopBehavior.StopEmitting);
			}

			foreach (RawImage meshrendglow in RicochetGlowMeshes) 
			{
				meshrendglow.enabled = false;
			}
		}

		// Resets rapidfire mode if not modified by game modifier object.
		if (gameControllerScript.gameModifier.AlwaysRapidfire == false) 
		{
			CurrentFireRate = StandardFireRate;
		}
			
		// Resets overdrive mode if not modified by game modifier object.
		if (gameControllerScript.gameModifier.AlwaysOverdrive == false) 
		{
			StandardShotIteration = shotIteration.Standard;
			DoubleShotIteration = shotIteration.Standard;
			TripleShotIteration = shotIteration.Standard;
			RippleShotIteration = shotIteration.Standard;
		}
			
		// Turns off the helix with delay.
		Invoke ("TurnOffHelix", 1);

		foreach (ParticleSystem helixparticle in HelixParticles)
		{
			helixparticle.Stop ();
		}

		foreach (Collider helixcol in HelixCol) 
		{
			helixcol.enabled = false;
		}

		// Turns off turrets all turrets.
		foreach (GameObject clone in Turrets) 
		{
			clone.SetActive (false);
		}

		// Resets powerup time remaining.
		gameControllerScript.PowerupTimeRemaining = 0;

		// Clears powerup UI.
		CheckPowerupImageUI ();
		gameControllerScript.ClearPowerupUI ();
	}

	// This is for InControl for initialization.
	void CreatePlayerActions ()
	{
		playerActions = new PlayerActions ();
	}

	// This is for InControl to be able to read input.
	void AssignActionControls ()
	{
		// LEFT
		playerActions.MoveLeft.AddDefaultBinding (Key.A);
		playerActions.MoveLeft.AddDefaultBinding (Key.LeftArrow);
		playerActions.MoveLeft.AddDefaultBinding (InputControlType.LeftStickLeft);
		playerActions.MoveLeft.AddDefaultBinding (InputControlType.DPadLeft);

		// RIGHT
		playerActions.MoveRight.AddDefaultBinding (Key.D);
		playerActions.MoveRight.AddDefaultBinding (Key.RightArrow);
		playerActions.MoveRight.AddDefaultBinding (InputControlType.LeftStickRight);
		playerActions.MoveRight.AddDefaultBinding (InputControlType.DPadRight);

		// UP
		playerActions.MoveUp.AddDefaultBinding (Key.W);
		playerActions.MoveUp.AddDefaultBinding (Key.UpArrow);
		playerActions.MoveUp.AddDefaultBinding (InputControlType.LeftStickUp);
		playerActions.MoveUp.AddDefaultBinding (InputControlType.DPadUp);

		// DOWN
		playerActions.MoveDown.AddDefaultBinding (Key.S);
		playerActions.MoveDown.AddDefaultBinding (Key.DownArrow);
		playerActions.MoveDown.AddDefaultBinding (InputControlType.LeftStickDown);
		playerActions.MoveDown.AddDefaultBinding (InputControlType.DPadDown);

		// SHOOT
		playerActions.Shoot.AddDefaultBinding (Key.Space);
		playerActions.Shoot.AddDefaultBinding (Key.LeftControl);
		playerActions.Shoot.AddDefaultBinding (Mouse.LeftButton);
		playerActions.Shoot.AddDefaultBinding (InputControlType.RightTrigger);
		playerActions.Shoot.AddDefaultBinding (InputControlType.Action1);

		// ABILITY
		playerActions.Ability.AddDefaultBinding (Key.LeftAlt);
		playerActions.Ability.AddDefaultBinding (Mouse.RightButton);
		playerActions.Ability.AddDefaultBinding (InputControlType.LeftTrigger);

		// PAUSE / UNPAUSE
		playerActions.Pause.AddDefaultBinding (Key.Escape);
		playerActions.Pause.AddDefaultBinding (InputControlType.Command);

		// DEBUG / CHEATS
		playerActions.DebugMenu.AddDefaultBinding (Key.Tab);
		playerActions.DebugMenu.AddDefaultBinding (InputControlType.LeftBumper);
		playerActions.CheatConsole.AddDefaultBinding (Key.Backquote);
	}

	#if !PLATFORM_STANDALONE_OSX
	// Creates vibration.
	public void Vibrate (float LeftMotor, float RightMotor, float duration)
	{
		PlayerVibrationDuration = duration;
		PlayerVibrationTimeRemaining = PlayerVibrationDuration;
		GamePad.SetVibration (PlayerIndex.One, LeftMotor, RightMotor);
	}
	#endif

	// Vibration time remaining is timed and turns off vibration when the timer runs out.
	void CheckPlayerVibration ()
	{
		if (PlayerVibrationTimeRemaining > 0) 
		{
			PlayerVibrationTimeRemaining -= Time.deltaTime;
		}

		if (PlayerVibrationTimeRemaining < 0) 
		{
			ResetPlayerVibration ();
			PlayerVibrationTimeRemaining = 0;
		}
	}

	// Resets player vibration.
	void ResetPlayerVibration ()
	{
		#if !PLATFORM_STANDALONE_OSX
		GamePad.SetVibration (PlayerIndex.One, 0, 0);
		#endif
	}

	// Check joined state.
	void CheckJoinState ()
	{
		if (this.enabled == true) 
		{
			isJoined = true;
		}
	}

	void CheckCheatConsoleInput ()
	{
		if (developerModeScript.useCheats == true) 
		{
			if (playerActions.CheatConsole.WasPressed) 
			{
				developerModeScript.CheatConsole.SetActive (!developerModeScript.CheatConsole.activeSelf);
				developerModeScript.ClearCheatString ();

				if (developerModeScript.CheatConsole.activeSelf) 
				{
					Debug.Log ("Cheat console opened.");
				}

				if (!developerModeScript.CheatConsole.activeSelf) 
				{
					Debug.Log ("Cheat console closed.");
				}
			}

			if (playerActions.CheatConsole.WasReleased) {
				developerModeScript.ClearCheatString ();
				developerModeScript.CheatInputText.text = ">_ ";
			}

			developerModeScript.CheatInputText.text = developerModeScript.CheatString;
		}
	}

	// Resets joined on disable.
	void OnDisable ()
	{
		if (isJoined == true) 
		{
			isJoined = false;
			PlayerText.text = " ";
		}
	}
}