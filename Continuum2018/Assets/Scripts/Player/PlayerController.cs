using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl; 					// Accessing InControl's cross platform controller input.
using UnityEngine.PostProcessing;   // Accessing Unity's Post Processing Stack.
using UnityEngine.UI; 				// Accessing Unity's UI system.
using TMPro; 						// Accessing Text Mesh Pro components.
using UnityEngine.Audio; 			// Accessing Audio mixer settings.
using UnityStandardAssets.Utility;  // Accessing some standard assets and scripts.

#if !PLATFORM_STANDALONE_OSX && !PLATFORM_ANDROID
using XInputDotNetPure; 			// Accessing controller vibration system and raw inputs.
#endif
// One instance per player.
public class PlayerController : MonoBehaviour 
{
	// Reference scripts.
	public GameController 		 gameControllerScript;
	public TimescaleController   timescaleControllerScript;
	public AudioController 		 audioControllerScript;
	public CursorManager 		 cursorManagerScript;
	public CameraShake 			 camShakeScript;
	public DeveloperMode 		 developerModeScript;
	public TutorialManager 		 tutorialManagerScript;
	public PostProcessingProfile PostProcessProfile;
	public MenuManager 			 pauseManagerScript;

	[Tooltip("Device ID for InControl.")]
	public int deviceID = 0;

	[Header ("Player Movement")]
	[Range (-1, 1)]
	[Tooltip("Current horizontal input.")]
	public float MovementX;
	[Range (-1, 1)]
	[Tooltip("Current vertical input.")]
	public float MovementY;
	[Tooltip("Sets whether the Player visual object follows a specific point defined by input.")]
	public bool UsePlayerFollow = true;
	[Tooltip("The object the input is manipulating.")]
	public Transform PlayerFollow;
	[Tooltip("The object that follows the player follow GameObject.")]
	public Rigidbody PlayerFollowRb;
	[Tooltip("Tweak this to change how fast the player moves.")]
	public float PlayerFollowMoveSpeed;
	[Tooltip("Defines bounds for player position.")]
	public Vector2 XBounds, YBounds;
	[Tooltip("The player Rigidbody which is the same GameObject as the mesh collider for it.")]
	public Rigidbody PlayerRb;
	// Moving with smooth damp provate velocity variables.
	private float SmoothFollowVelX, SmoothFollowVelY; 
	[Tooltip("How much smoothing is applied to the player follow movement.")]
	public float SmoothFollowTime = 1;
	[Tooltip("How many degrees the player objects rotates based on horizontal velocity.")]
	public float YRotationAmount = 45;
	[Tooltip("Multiplier for the rotation.")]
	public float YRotationMultiplier = 10;
	// Smooth ref variable for player rotation.
	private float RotVelY; 
	[Tooltip("How long the controller vibrates for.")]
	public float PlayerVibrationDuration;
	[Tooltip("How much time the controller still needs to be vibrating.")]
	public float PlayerVibrationTimeRemaining;

	[Header ("Player stats")]
	[Tooltip("Player's unique ID.")]
	public int PlayerId = 1;
	[Tooltip("Name for the player.")]
	public TextMeshProUGUI PlayerText;
	[Tooltip("For multiplayer: Shows if joined game.")]
	public bool isJoined;

	// Player's main engine particle effect settings.
	[Tooltip("The middle particle effect.")]
	public ParticleSystem MainEngineParticles;
	[Tooltip("Emission amount for the main engine.")]
	public float MainEngineParticleEmissionAmount = 1; 
	[Tooltip("Smooth change rate.")]
	public float MainEngineParticleEmissionLerpSpeed = 4;

	[Tooltip("To be used to calculate distance and therefore time scale.")]
	public Transform ReferencePoint;
	[Tooltip("Visuals for the player guides.")]
	public GameObject PlayerGuides;

	public AudioSource SpaceshipAmbience;

	[Header ("Player input UI")]
	// To be shown only in tutorial.
	public RectTransform MiddlePoint; 
	public RectTransform ForegroundPoint;
	public RawImage InputForegroundImage;
	public RawImage InputBackgroundImage;
	public RectTransform InputUIPoint;
	public LineRenderer InputLine;
	public float inputSensitivity = 3;
	public Image ShootingInputImage;
	public Image AbilityInputImage;

	[Header ("Shooting Overview")]
	[Tooltip("Base GameObject to instantiate when firing.")]
	public GameObject CurrentShotObject;
	[Tooltip("Allows the player to instantiate the bullet or not.")]
	public bool canShoot = true;
	[Tooltip("The starting fire rate and current fire rate updated by other values.")]
	public float CurrentFireRate = 0.1f;
	[Tooltip("How much the fire rate changes when the time scale changes.")]
	public float FireRateTimeMultiplier = 2;
	[Tooltip("Time.time must be greater than or equal to this to allow another shot to be spawned.")]
	public float NextFire; 		

	public enum shotIteration
	{
		Standard = 0, // Default.
		Enhanced = 1, // Ricochet or Homing.
		Rapid = 2,
		Overdrive = 3
	}

	public Animator PlayerRecoil;

	[Header ("Overheating")]
	[Range (0, 1)]
	[Tooltip("Linear representation of current heat level.")]
	public float CurrentShootingCooldown;
	[Range (0, 1)]
	[Tooltip("Other representation of current heat level.")]
	public float CurrentShootingHeat;
	[Tooltip("How fast the cooldown cools down before it reaches the overheat state.")]
	public float ShootingCooldownDecreaseRate = 1;
	[Tooltip("How much value to add to the current heat.")]
	public float CurrentShootingHeatCost;
	[Tooltip("Game will not use overheat mechanic if off.")]
	public bool useOverheat;
	[Tooltip("Is the player in an overheat state?")]
	public bool Overheated;
	[Tooltip("How fast to cooldown when in overheated.")]
	public float OverheatCooldownDecreaseRate = 2;
	[Tooltip("UI for overheat controlling fill amount.")]
	public Image OverheatImageL;
	[Tooltip("Smoothing amount for overheat fill.")]
	public float OverheatFillSmoothing = 0.25f;
	[Tooltip("Color for overheating.")]
	[ColorUsageAttribute (true, true, 0, 99, 0, 0)]
	public Color HotColor  = Color.red;
	[Tooltip("Brightness multiplier for overheating.")]
	public float HeatUIBrightness = 20;
	[Tooltip("Sound to play when it reaches overheating.")]
	public AudioSource OverheatSound;
			 
	// Shot types.
	[Tooltip("Current shot type the player is on.")]
	public shotType ShotType; 
	public enum shotType
	{
		Standard = 0,
		Double = 1,
		Triple = 2,
		Ripple = 3
	}

	[Tooltip("Player shoots this when there are no shooting powerups active.")]
	public GameObject StandardShot;
	[Tooltip("Player shoots this when ricochet and/or homing is active but has not collected another shooting powerup.")]
	public GameObject StandardShotEnhanced;
	[Tooltip("Player shoots this when overdrive is active but has not collected another shooting powerup.")]
	public GameObject StandardShotOverdrive;
	[Tooltip("Shows what iteration the standard shot is currently on. Enumerates what shot type to be using for standard shot.")]
	public shotIteration StandardShotIteration;
	[Tooltip("Where do we spawn a standard bullet?")]
	public Transform StandardShotSpawn;
	[Tooltip("How fast the standard fire rate is by default.")]
	public float StandardFireRate = 0.1f;
	[Tooltip("The heat cost of shooting a standard shot.")]
	public float StandardShootingHeatCost = 0.0125f;

	[Header ("Impact")]
	[Tooltip("The GameObject which holds the player's mesh and material.")]
	public GameObject playerMesh;
	[Tooltip("Where the last impact point was.")]
	public Vector3 ImpactPoint;

	// Player has two colliders, one is in trigger mode.
	[Tooltip("Player collider: Trigger is false.")]
	public Collider playerCol;
	[Tooltip("Player collider: Trigger is true.")]
	public Collider playerTrigger;
	[Tooltip("Is the player in cooldown mode after an impact? Allows cooldown to happen.")]
	public bool isInCooldownMode;
	[Tooltip("How long the cooldown duration happens (scaled by time scale).")]
	public float cooldownDuration; 
	[Tooltip("Timer for cooldown.")]
	public float cooldownTimeRemaining;

	// Stuff to do when hit.
	public ParticleSystem PlayerExplosionParticles;
	public AudioSource PlayerExplosionAudio;
	[Tooltip("Allows cool image effects tp play that simulates VHS glitch effects and animates them.")]
	public Animator GlitchEffect;
	[Tooltip("Collider for invincibility.")]
	public MeshCollider InvincibleCollider;
	[Tooltip("MeshRenderer for invincibility.")]
	public MeshRenderer InvincibleMesh;
	[Tooltip("Animator for invincibility.")]
	public Animator InvincibleMeshAnim;

	[Tooltip("Stuff to look at when the player runs out of lives on the impact.")]
	public ParticleSystem GameOverExplosionParticles;
	[Tooltip("Game over sound for the explosion.")]
	public AudioSource GameOverExplosionAudio;

	[Header ("Ability")]
	[Tooltip("Current state of the ability charge.")]
	public abilityState CurrentAbilityState;
	public enum abilityState
	{
		Charging, // Ability cannot be used, it is charging in this state.
		Ready, 	  // Ability can be used.
		Active 	  // Ability is being used.
	}

	// Ability stats.
	[Tooltip("Maximum ability time.")]
	public float CurrentAbilityDuration;
	[Tooltip("Timer for the ability.")]
	public float CurrentAbilityTimeRemaining;
	[Range (0.0f, 1.0f)]
	[Tooltip("Time remining / duration.")]
	public float AbilityTimeAmountProportion;
	[Tooltip("How fast the ability bar charges.")]
	public float AbilityChargeSpeedMultiplier = 0.5f;
	[Tooltip("How fast the ability bar diminishes.")]
	public float AbilityUseSpeedMultiplier = 4;
	[Tooltip("Unique name of the ability.")]
	public string AbilityName;
	[Tooltip("Ability list.")]
	public ability Ability;
	public enum ability
	{
		Shield, // Creates a shield, invincibility for a short time, cool warp screen effect.
		Emp, // Creates a quick exhaust blast of particles which interact with blocks and destroying them.
		VerticalBeam, // Fires particles vertically up, destroying particles in the way.
		HorizontalBeam, // Fires to streams of particles (left and right), destroying falling or stacked blocks that collide with it.
		Rewind // Rewinds time for a certain amount of seconds.
	}

	// Ability UI.
	[Tooltip("Ability UI.")]
	public GameObject AbilityUI;
	[Tooltip("Animator for Ability UI.")]
	public Animator AbilityAnim;
	[Tooltip("Current RawImage of the ability.")]
	public RawImage AbilityImage;
	[Tooltip("Array of ability icons.")]
	public Texture2D[] AbilityTextures;
	[Tooltip("Image fill outline.")]
	public Image AbilityFillImage;
	// Colors for each ability state.
	[Tooltip("Color of ability being used.")]
	[ColorUsageAttribute (true, true, 0, 99, 0, 0)]
	public Color AbilityUseColor;
	[Tooltip("Color of ability charging.")]
	[ColorUsageAttribute (true, true, 0, 99, 0, 0)]
	public Color AbilityChargingColor;
	[Tooltip("Color of ability being ready.")]
	[ColorUsageAttribute (true, true, 0, 99, 0, 0)]
	public Color AbilityChargingFullColor;
	[Tooltip("Color of ability brightness multiplier.")]
	public float AbilityBrightness = 8;
	[Tooltip("Animator for when the ability is ready.")]
	public Animator AbilityCompletion;
	[Tooltip("Texture to use when the ability is ready.")]
	public RawImage AbilityCompletionTexture;
	[Tooltip("Text for the ability is ready UI.")]
	public TextMeshProUGUI AbilityCompletionText;

	[Header ("Powerups")]
	[Tooltip("Tracks how many powerups are active at the time.")]
	public int powerupsInUse;
	[Tooltip("Homing mode is on or off.")]
	public bool isHoming;
	[Tooltip("Ricochet mode is on or off.")]
	public bool isRicochet;
	[Tooltip("Rapidfire mode is on or off.")]
	public bool isInRapidFire;
	[Tooltip("Overdrive mode is on or off.")]
	public bool isInOverdrive; 
	[Tooltip("Static glow bars on the top and bottom of the screen when ricochet mode is on.")]
	public GameObject RicochetGlowObject;
	[Tooltip("Components to access the rendering of the glow bars.")]
	public RawImage[] RicochetGlowMeshes;
	[Tooltip("Particles that emit off the glow objects.")]
	public ParticleSystem[] RicochetGlowParticles;

	// Double shot.
	[Tooltip("Left bullet normal.")]
	public GameObject DoubleShotL;
	[Tooltip("Left bullet enhanced.")]
	public GameObject DoubleShotLEnhanced;
	[Tooltip("Left bullet overdrive.")]
	public GameObject DoubleShotLOverdrive; 
	[Tooltip("Right bullet normal.")]
	public GameObject DoubleShotR;
	[Tooltip("Right bullet enhanced.")]
	public GameObject DoubleShotREnhanced; 
	[Tooltip("Right bullet overdrive.")]
	public GameObject DoubleShotROverdrive; 
	[Tooltip("Spawn point for left bullet.")]
	public Transform DoubleShotSpawnL;
	[Tooltip("Spawn point for right bullet.")]
	public Transform DoubleShotSpawnR;
	[Tooltip("[0] = normal fire rate, [1] = rapid fire rate.")]
	public float[] DoubleShotFireRates;	
	[Tooltip("Enumerates what shot type to be using for double shot.")]
	public shotIteration DoubleShotIteration;
	[Tooltip("Time.time must be >= for this to allow another shot to be spawned.")]
	public float DoubleShotNextFire;
	[Tooltip("Cost for double shot.")]
	public float DoubleShootingHeatCost = 0.0125f;

	// Triple shot.
	[Tooltip("Left bullet normal.")]
	public GameObject TripleShotL;
	[Tooltip("Left bullet enhanced.")]
	public GameObject TripleShotLEnhanced;
	[Tooltip("Left bullet overdrive.")]
	public GameObject TripleShotLOverdrive;	
	[Tooltip("Middle bullet normal.")]
	public GameObject TripleShotM;		
	[Tooltip("Middle bullet enhanced.")]
	public GameObject TripleShotMEnhanced;		
	[Tooltip("Middle bullet overdrive..")]
	public GameObject TripleShotMOverdrive;		
	[Tooltip("Right bullet normal.")]
	public GameObject TripleShotR;	
	[Tooltip("Right bullet enhanced.")]
	public GameObject TripleShotREnhanced;		
	[Tooltip("Right bullet overdrive.")]
	public GameObject TripleShotROverdrive;		
	[Tooltip("Spawn point for left bullet.")]
	public Transform TripleShotSpawnL;			
	[Tooltip("Spawn point for middle bullet.")]
	public Transform TripleShotSpawnM;			
	[Tooltip("Spawn point for right bullet.")]
	public Transform TripleShotSpawnR;		
	[Tooltip("[0] = normal fire rate, [1] = rapid fire rate.")]
	public float[] TripleShotFireRates;	
	[Tooltip("Enumerates what shot type to be using for triple shot.")]
	public shotIteration TripleShotIteration;
	[Tooltip("Time.time must be >= for this to allow another shot to be spawned.")]
	public float TripleShotNextFire;
	[Tooltip("Heat cost for a triple shot.")]
	public float TripleShootingHeatCost = 0.0125f;

	// Ripple shot.
	[Tooltip("Standard ripple shot.")]
	public GameObject RippleShot;
	[Tooltip("Enhanced ripple shot.")]
	public GameObject RippleShotEnhanced;
	[Tooltip("Overdrive ripple shot.")]
	public GameObject RippleShotOverdrive;
	[Tooltip("Where the ripple shot will spawn.")]
	public Transform RippleShotSpawn;
	[Tooltip("[0] = normal fire rate, [1] = rapid fire rate.")]
	public float[] RippleShotFireRates;	
	[Tooltip("Enumerates what shot type to be using for ripple shot.")]
	public shotIteration RippleShotIteration;
	[Tooltip("Time.time must be >= for this to allow another shot to be spawned.")]
	public float RippleShotNextFire;
	[Tooltip("Cost of firing a ripple shot.")]
	public float RippleShootingHeatCost = 0.0125f;


	[Header ("Shield")]
	[Tooltip("Allows shield visuals and timer to activate.")]
	public bool isShieldOn;
	[Tooltip("Shield main object.")]
	public GameObject Shield;
	[Tooltip("Camera effect to simulate gravitational lensing.")]
	public Lens lensScript;
	[Tooltip("How large the lensing is.")]
	public float TargetShieldScale;	
	[Tooltip("How slow the transition is for the shield lights to grow/shrink.")]
	public float ShieldScaleSmoothTime = 1;
	[Tooltip("Target lens size when shield is active.")]
	public float LensOnRadius = 0.7f;
	[Tooltip("Current target lens size.")]
	public float TargetLensRadius;
	[Tooltip("How slow the transition is for the lensing to grow/shrink.")]
	public float LensRadiusSmoothTime = 1;

	[Header ("VerticalBeam")]
	[Tooltip("The GameObject to set active or not depending whether the ability is being active.")]
	public GameObject VerticalBeam;
	[Tooltip("Array of particles to emit when enabled.")]
	public ParticleSystem[] VerticalBeamParticles;

	[Header ("HorizontalBeam")]
	[Tooltip("The GameObject to set active or not depending whether the ability is being active.")]
	public GameObject HorizontalBeam; 
	[Tooltip("Array of particles to emit when enabled.")]
	public ParticleSystem[] HorizontalBeamParticles;

	[Header ("Emp")]
	[Tooltip("The GameObject to set active or not depending whether the ability is being active.")]
	public GameObject Emp; 				  // The GameObject to set active or not depending whether the ability is being active.
	[Tooltip("Array of particles to emit when enabled.")]
	public ParticleSystem[] EmpParticles; // Array of particles to emit when enabled.

	[Header ("Turret Player")]
	[Range (0, 4)]
	[Tooltip("Which turrent GameObject index to spawn next.")]
	public int nextTurretSpawn;
	[Tooltip("Turrets in scene.")]
	public GameObject[] Turrets;
	[Tooltip("Parented auto rotation script.")]
	public AutoMoveAndRotate TurretRotatorScript;
	[Tooltip("How fast the turrents spin around the player.")]
	public float TurretSpinSpeed;
	[Tooltip("Setting while powerup time remaining is >= 3.")]
	public float TurretSpinSpeedNormal = -220;
	[Tooltip("Setting while powerup time remaining is < 3.")]
	public float TurretSpinSpeedFaster = -660;

	[Header ("Helix")]
	[Tooltip("The GameObject to set active or not depending whether the powerup is activated.")]
	public GameObject Helix;
	[Tooltip("Array of particles to emit when enabled.")]
	public ParticleSystem[] HelixParticles;
	[Tooltip("The helix has two colliders, stored here.")]
	public Collider[] HelixCol;

	[Header ("Visuals")]
	[Tooltip("Animator which controls animations for the screen orientation.")]
	public Animator FlipScreenAnim;

	[Header ("UI")]
	// Ability UI.
	[Tooltip("Is the ability UI hidden now?")]
	public bool isHidingAbilityUI;
	[Tooltip("Ability Hexes situated underneath the score.")]
	public Animator AbilityUIHexes;
	[Tooltip("Where to check for the player position range.")]
	public Vector3 AbilityCheckPlayerPos;
	[Space (10)]
	// Score UI.
	[Tooltip("Enabled when player position is close to the top middle.")]
	public bool isHidingScoreUI;
	[Tooltip("Score fading in/out animator.")]
	public Animator ScoreAnim;
	[Tooltip("Where to check for the player position range.")]
	public Vector3 ScoreCheckPlayerPos;
	[Space (10)]
	// Shooting UI.
	[Tooltip("Shooting Hexes on the top right.")]
	public Animator ShootingUIHexes; 
	[Space (10)]
	// Lives UI.
	[Tooltip("Checks top left of the screen if player is too close to the edge/corner.")]
	public bool isHidingLivesUI;
	[Tooltip("Lives fade in/out animator.")]
	public Animator LivesAnim;
	[Tooltip("Range to check horizontal player position.")]
	public Vector2 LivesCheckPlayerPosX;
	[Tooltip("Range to check vertical player position.")]
	public Vector2 LivesCheckPlayerPosY;

	public Animator LivesLeftUI;
	public TextMeshProUGUI LivesLeftText;
	//public string LivesLeftString = "3 LIVES LEFT";

	[Space (10)]
	// Wave UI.
	[Tooltip("Checks for wave UI.")]
	public bool isHidingWaveUI;
	[Tooltip("Wave fade in/out animator.")]
	public Animator WaveAnim;
	[Tooltip("Point to check for player proximity.")]
	public Vector3 WaveCheckPlayerPos;
	[Space (10)]
	// PowerupUI
	[Tooltip("Is the UI for powerups hidden?.")]
	public bool isHidingPowerupUI;
	[Tooltip("Position to check for powerup UI autohiding.")]
	public Vector3 PowerupUICheckPos;

	// InControl Player Actions.
	public PlayerActions playerActions; // Created for InControl and assigned at runtime.

	InputDevice mInputDevice
	{
		get
		{
			return GameController.playerDevices[deviceID];
		}
	}

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

		useOverheat = gameControllerScript.gameModifier.useOverheat;
		OverheatImageL.fillAmount = 0;
		OverheatImageL.material.EnableKeyword ("_EMISSION");
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
		CheckShootingCooldown ();
		CheckPlayerVibration ();
		CheckUIVisibility ();
		CheckCooldownTime ();
		CheckAbilityTime ();
		DrawReferencePointLine ();
		UpdateInputUI ();
		CheckPause ();
		CheckCheatConsoleInput ();
		UpdateImageEffects ();
		UpdateAudio ();
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
				//AbilityUI.SetActive (true);
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
		ShootingInputImage.fillAmount = playerActions.Shoot.Value;
		AbilityInputImage.fillAmount = playerActions.Ability.Value;

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
		gameControllerScript.NextPowerupSlot_P1 = 1;
		gameControllerScript.NextPowerupShootingSlot_P1 = 0;
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
		camShakeScript.ShakeCam (2, 3, 99);
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
		//AbilityUI.SetActive (false);
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
		CurrentShootingHeat = 0;
		CurrentShootingCooldown = 0;
		PlayerFollow.transform.position = Vector3.zero;
		canShoot = true;
		UsePlayerFollow = true;
		playerMesh.SetActive (true);
		audioControllerScript.TargetCutoffFreq = 22000;
		audioControllerScript.TargetResonance = 1;
		InvincibleMeshAnim.Play ("InvincibleMeshFlash");

		// Animate lives left text.
		Invoke ("UpdateLivesLeftA", 0.6f);
		Invoke ("UpdateLivesLeftB", 1.2f);
		Invoke ("UpdateLivesLeftC", 1.8f);
	}

	void UpdateLivesLeftA ()
	{
		if (gameControllerScript.Lives > 1)
		{
			LivesLeftText.text = (gameControllerScript.Lives).ToString ();
		}

		if (gameControllerScript.Lives <= 1) 
		{
			LivesLeftText.text = "LAST";
		}

		LivesLeftUI.Play ("LivesLeft");
	}

	void UpdateLivesLeftB ()
	{
		if (gameControllerScript.Lives <= 1) 
		{
			LivesLeftText.text = "LIFE";
		}

		if (gameControllerScript.Lives > 1) 
		{
			LivesLeftText.text = "LIVES";
		}

		LivesLeftUI.Play ("LivesLeft");
	}

	void UpdateLivesLeftC ()
	{
		if (gameControllerScript.Lives > 1)
		{
			LivesLeftText.text = "LEFT";
			LivesLeftUI.Play ("LivesLeft");
		}
	}

	void UpdateAudio ()
	{
		SpaceshipAmbience.panStereo = 0.04f * transform.position.x; // Pans audio based on x position.
		SpaceshipAmbience.pitch = Mathf.Lerp (SpaceshipAmbience.pitch, Time.timeScale * playerActions.Move.Value.magnitude + 0.2f, Time.deltaTime * 10);
		SpaceshipAmbience.volume = Mathf.Lerp (SpaceshipAmbience.volume, 0.1f * playerActions.Move.Value.magnitude + 0.1f, Time.deltaTime * 10);
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
				//AbilityAnim.Play ("HexesFadeIn");
			}
		}

		// Updates the ability timers.
		if (CurrentAbilityState == abilityState.Active) 
		{
			//AbilityFillImage.color = AbilityUseColor * 25;
			AbilityFillImage.material.SetColor ("_EmissionColor",
				AbilityUseColor * AbilityBrightness
			);
				
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
			//AbilityAnim.Play ("AbilityBounce");
			AbilityAnim.Play ("HexesFadeIn");
		}

		if (CurrentAbilityState == abilityState.Ready) 
		{
			AbilityFillImage.material.SetColor ("_EmissionColor",
				AbilityChargingFullColor * AbilityBrightness
			);

			if (isHidingAbilityUI == false) 
			{
				AbilityAnim.Play ("AbilityBounce");
			}
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
				//CurrentAbilityTimeRemaining += AbilityChargeSpeedMultiplier * Time.unscaledDeltaTime; // Add slowdown.
			}

			if (CurrentAbilityTimeRemaining >= CurrentAbilityDuration) 
			{
				CurrentAbilityTimeRemaining = CurrentAbilityDuration;
				AbilityCompletion.Play ("AbilityComplete");
				AbilityCompletionTexture.texture = AbilityImage.texture;
				AbilityCompletionText.text = ParseByCase(Ability.ToString ());
				timescaleControllerScript.OverrideTimeScaleTimeRemaining = 1f;
				timescaleControllerScript.OverridingTimeScale = 0.1f;
				CurrentAbilityState = abilityState.Ready;

				AbilityAnim.Play ("AbilityBounce");
			}

			if (AbilityTimeAmountProportion < 1f)
			{
				//AbilityFillImage.color = AbilityChargingColor * 25;
				AbilityFillImage.material.SetColor ("_EmissionColor",
					AbilityChargingColor * AbilityBrightness
				);
			}
		}

		lensScript.radius = Mathf.Lerp (lensScript.radius, TargetLensRadius, LensRadiusSmoothTime * Time.unscaledDeltaTime);
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
		string SentenceCaseAbility = ParseByCase (Ability.ToString ());

		AbilityName = SentenceCaseAbility;
	}

	/// <summary>
	/// Parse the input string by placing a space between character case changes in the string
	/// </summary>
	/// <param name="strInput">The string to parse</param>
	/// <returns>The altered string</returns>
	public static string ParseByCase(string strInput)
	{
		// The altered string (with spaces between the case changes)
		string strOutput = "";

		// The index of the current character in the input string
		int intCurrentCharPos = 0;

		// The index of the last character in the input string
		int intLastCharPos = strInput.Length - 1;

		// for every character in the input string
		for (intCurrentCharPos = 0; intCurrentCharPos <= intLastCharPos; intCurrentCharPos++)
		{
			// Get the current character from the input string
			char chrCurrentInputChar = strInput[intCurrentCharPos];

			// At first, set previous character to the current character in the input string
			char chrPreviousInputChar = chrCurrentInputChar;

			// If this is not the first character in the input string
			if (intCurrentCharPos > 0)
			{
				// Get the previous character from the input string
				chrPreviousInputChar = strInput[intCurrentCharPos - 1];

			} // end if

			// Put a space before each upper case character if the previous character is lower case
			if (char.IsUpper(chrCurrentInputChar) == true && char.IsLower(chrPreviousInputChar) == true)
			{   
				// Add a space to the output string
				strOutput += " ";

			} // end if

			// Add the character from the input string to the output string
			strOutput += chrCurrentInputChar;

		} // next

		// Return the altered string
		return strOutput;

	} // end method

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

	void CheckShootingCooldown ()
	{
		if (useOverheat == true) {
			// Maps heat to squared of shooting cooldown.
			//CurrentShootingHeat = CurrentShootingCooldown * CurrentShootingCooldown;
			CurrentShootingHeat = Mathf.Pow (CurrentShootingCooldown, 1);

			// Clamps to 0 and 1.
			CurrentShootingHeat = Mathf.Clamp (CurrentShootingHeat, 0, 1);
			CurrentShootingCooldown = Mathf.Clamp (CurrentShootingCooldown, 0, 1);

			OverheatImageL.fillAmount = Mathf.Lerp (
				OverheatImageL.fillAmount, 
				CurrentShootingHeat, 
				OverheatFillSmoothing * Time.unscaledDeltaTime
			);

			if (CurrentShootingHeat <= 0.01f) {
				Overheated = false;
			}

			if (Overheated == false) {
				OverheatImageL.material.SetColor ("_EmissionColor", new Color (
					OverheatImageL.fillAmount,
					-Mathf.Cos ((Mathf.PI * OverheatImageL.fillAmount) + (0.5f * Mathf.PI)),
					1 - OverheatImageL.fillAmount
				) * HeatUIBrightness);

				if (OverheatImageL.fillAmount > 0.99f) {
					if (OverheatSound.isPlaying == false) {
						OverheatSound.Play ();
					}

					Overheated = true;
				}
			}

			if (Overheated == true) {
				CurrentShootingCooldown -= Time.unscaledDeltaTime * OverheatCooldownDecreaseRate;
				OverheatImageL.color = HotColor * HeatUIBrightness;
			}
		}
	}

	// Checks shooting state.
	void CheckShoot ()
	{
		if (canShoot == true) 
		{
			if (playerActions.Shoot.Value > 0.75f && gameControllerScript.isPaused == false) 
			{
				if (Time.time >= NextFire)
				{
					// Every time the player shoots, decremement the combo.
					if (gameControllerScript.combo > 1)
					{
						gameControllerScript.combo -= 1;
					}

					if (Overheated == false && AbilityFillImage.color != HotColor)
					{
						Shoot ();
						//NextFire = Time.time + (CurrentFireRate / (FireRateTimeMultiplier * Time.timeScale));
						NextFire = Time.time + (CurrentFireRate / (FireRateTimeMultiplier));
					}
				}

				if (Overheated == false)
				{
					CurrentShootingCooldown += (CurrentShootingHeatCost / FireRateTimeMultiplier) * Time.deltaTime; // Increase by cost.
					//CurrentShootingCooldown -= Time.deltaTime * (0.5f * ShootingCooldownDecreaseRate);
				}
			}

			if (playerActions.Shoot.Value < 0.75f && Time.time >= NextFire && 
				gameControllerScript.isPaused == false) 
			{
				if (Overheated == false) 
				{
					// Keeps decreasing heat over time.
					CurrentShootingCooldown -= Time.deltaTime * ShootingCooldownDecreaseRate;
				}
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

		PlayerRecoil.Play ("PlayerRecoil");
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
		if (PlayerRb.position.y > AbilityCheckPlayerPos.y) 
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
		foreach (RawImage powerupimage in gameControllerScript.PowerupImage_P1)
		{
			//powerupimage == gameControllerScript.PowerupImage_P1 [0]
			if (powerupimage.gameObject.activeInHierarchy == true)
			{
				if (playerCol.transform.position.y > PowerupUICheckPos.y &&
					playerCol.transform.position.x > PowerupUICheckPos.x
				) 
				{
					powerupimage.GetComponent<Animator> ().SetBool ("Visible", false);
				}

				if (playerCol.transform.position.y <= PowerupUICheckPos.y || 
					playerCol.transform.position.x <= PowerupUICheckPos.x)
				{
					powerupimage.GetComponent<Animator> ().SetBool ("Visible", true);
				}
			}
		}
			
		if (playerCol.transform.position.y > PowerupUICheckPos.y &&
			playerCol.transform.position.x > PowerupUICheckPos.x
		) 
		{
			gameControllerScript.HomingImage.GetComponent<Animator> ().SetBool ("Visible", false);
			gameControllerScript.RicochetImage.GetComponent<Animator> ().SetBool ("Visible", false);
			gameControllerScript.RapidfireImage.GetComponent<Animator> ().SetBool ("Visible", false);
			gameControllerScript.OverdriveImage.GetComponent<Animator> ().SetBool ("Visible", false);
		}

		if (playerCol.transform.position.y <= PowerupUICheckPos.y || 
			playerCol.transform.position.x <= PowerupUICheckPos.x)
		{
			gameControllerScript.HomingImage.GetComponent<Animator> ().SetBool ("Visible", true);
			gameControllerScript.RicochetImage.GetComponent<Animator> ().SetBool ("Visible", true);
			gameControllerScript.RapidfireImage.GetComponent<Animator> ().SetBool ("Visible", true);
			gameControllerScript.OverdriveImage.GetComponent<Animator> ().SetBool ("Visible", true);
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
		CurrentShootingHeatCost = StandardShootingHeatCost;
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

	void UpdateImageEffects ()
	{
		float localDistance = timescaleControllerScript.Distance;

		var PostProcessBloomSettings = PostProcessProfile.bloom.settings;
		PostProcessBloomSettings.bloom.intensity = 0.001f * localDistance + 0.005f;
		PostProcessProfile.bloom.settings = PostProcessBloomSettings;

		var PostProcessColorGradingSettings = PostProcessProfile.colorGrading.settings;
		PostProcessColorGradingSettings.basic.saturation = 0.003f * localDistance + 0.85f;
		PostProcessProfile.colorGrading.settings = PostProcessColorGradingSettings;
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
		//playerActions.Shoot.AddDefaultBinding (Mouse.LeftButton); // Commented out for touch controls to work properly.
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

		//var player1 = InputManager.Devices [0];

		//InputManager.OnDeviceAttached += inputDevice => Debug.Log( "Attached: " + inputDevice.Name );
		//InputManager.OnDeviceDetached += inputDevice => Debug.Log( "Detached: " + inputDevice.Name );
		//InputManager.OnActiveDeviceChanged += inputDevice => Debug.Log( "Switched: " + inputDevice.Name );
	}

	void OnDeviceAttached (InputDevice device)
	{
		Debug.Log ("Attached: " + device.Name);

		for (int i=0; i< GameController.playerDevices.Count; i++)
		{
			InputDevice InD = GameController.playerDevices[i];

			if (!InD.active)
			{
				if (InD.Name == device.Name && InD.Meta == device.Meta)
				{
					GameController.playerDevices[i] = device;
					break;
				}
			}
		}
	}

	void OnDeviceDetached (InputDevice device)
	{
		Debug.Log ("Detached: " + device.Name);

		foreach (InputDevice InD in GameController.playerDevices)
		{
			if (InD == device)
			{
				InD.active = false;
				break;
			}
		}
	}

	#if !PLATFORM_STANDALONE_OSX && !PLATFORM_ANDROID
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
		#if !PLATFORM_STANDALONE_OSX && !PLATFORM_ANDROID
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

			if (playerActions.CheatConsole.WasReleased)
			{
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
			//PlayerText.text = " ";
		}
	}

	/*void OnDestroy ()
	{
		playerActions.Destroy ();
	}*/
}