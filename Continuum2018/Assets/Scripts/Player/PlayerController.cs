using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl; 					// Accessing InControl's cross platform controller input.
using UnityEngine.PostProcessing;   // Accessing Unity's Post Processing Stack.
using UnityEngine.UI; 				// Accessing Unity's UI system.
using TMPro; 						// Accessing Text Mesh Pro components.
using UnityEngine.Audio; 			// Accessing Audio mixer settings.
using UnityStandardAssets.Utility;  // Accessing some standard assets and scripts.

#if !PLATFORM_STANDALONE_OSX && !PLATFORM_ANDROID && !PLATFORM_WEBGL
using XInputDotNetPure; 			// Accessing controller vibration system and raw inputs.
#endif

// One instance per player.
public class PlayerController : MonoBehaviour 
{
	public static PlayerController PlayerOneInstance { get; private set; }
	public static PlayerController PlayerTwoInstance { get; private set; }

	// Reference scripts.
	public CameraShake 			 camShakeScript;
	public PostProcessingProfile PostProcessProfile;
	public MenuManager 			 pauseManagerScript;
	public Spotlights 			 spotlightsScript;

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
	public float RelativeDistance;
	public Transform ReferencePoint;
	[Tooltip("Visuals for the player guides.")]
	public GameObject PlayerGuides;

	public AudioSource SpaceshipAmbience;

	public float RiskDistanceTime;
	public float RiskDistance = 14;

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

	public float NonShootingTime;

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
	public Transform ImpactTransform;

	// Player has two colliders, one is set as trigger.
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
	public ParticleSystem PlayerExplosionEmp;
	[Tooltip("Allows cool image effects tp play that simulates VHS glitch effects and animates them.")]
	public Animator GlitchEffect;
	[Tooltip("Collider for invincibility.")]
	public Collider InvincibleCollider;
	[Tooltip("MeshRenderer for invincibility.")]
	public MeshRenderer InvincibleMesh;
	[Tooltip("Animator for invincibility.")]
	public Animator InvincibleMeshAnim;
	public ParticleSystem InvincibleParticles;

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
		Shield = 0, 		 // Creates a shield, invincibility for a short time, cool warp screen effect.
		Emp = 1,			 // Creates a quick exhaust blast of particles which interact with blocks and destroying them.
		VerticalBeam = 2, 	 // Fires particles vertically up, destroying particles in the way.
		HorizontalBeam = 3,  // Fires to streams of particles (left and right), destroying falling or stacked blocks that collide with it.
		Rewind = 4,   		 // Rewinds time for a certain amount of seconds.
		Mirror = 5 			 // Adds a mirror clone to the player.
	}

	public ParticleSystem AbilityActiveParticles;
	public ParticleSystem AbilityActiveBurstParticles;
	public int AbilityActivations;

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

	public float AbilityDampening = 1;
	public float AbilityDampeningMultiplier = 0.9f;

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
	public ParticleSystem PowerupActiveParticles;
	public float newMinSize_PowerupActiveParticles;
	public float newMaxSize_PowerupActiveParticles;
	public ParticleSystem PowerupActiveParticlesBurst;
	public float newMinSize_PowerupActiveParticlesBurst;
	public float newMaxSize_PowerupActiveParticlesBurst;

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
	public GameObject Emp; 
	[Tooltip("Array of particles to emit when enabled.")]
	public ParticleSystem[] EmpParticles;

	[Header ("Mirror Player")]
	[Tooltip ("Reference to mirror player object.")]
	public GameObject MirrorPlayer;
	[Tooltip ("Reference to mirror player script.")]
	public MirrorPlayer mirrorPlayerScript;

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

	[Header ("Slow time")]
	public bool timeIsSlowed;

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
	public Light PlayerLight;

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
	public Animator PointsThisComboAnim;
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
	public GameObject PowerupUI;

	// InControl Player Actions.
	public PlayerActions playerActions_P1; // Created for InControl and assigned at runtime.
	public PlayerActions playerActions_P2; // Created for InControl and assigned at runtime.

	InputDevice mInputDevice
	{
		get
		{
			return GameController.playerDevices[deviceID];
		}
	}

	void Awake ()
	{
		if (PlayerId == 1) 
		{
			PlayerOneInstance = this;
			Debug.Log ("Player one instance set.");
		}

		if (PlayerId == 2) 
		{
			PlayerTwoInstance = this;
			Debug.Log ("Player two instance set.");
		}

		playerCol.transform.localPosition = Vector3.zero;
	}

	void Start () 
	{
		// Get the save and load script.
		if (SaveAndLoadScript.Instance == null)
		{
			SaveAndLoadScript.Instance.playerControllerScript_P1 = this;
		}

		CreatePlayerActions ();

		if (PlayerId == 1) 
		{
			AssignActionControls_P1 ();
		}

		if (PlayerId == 2) 
		{
			AssignActionControls_P2 ();
		}

		GetStartPlayerModifiers ();
		CheckPowerupImageUI ();

		Invoke ("PlaySpaceshipAmbience", 0);
		InvokeRepeating ("CheckJoinState", 0, 0.5f);
		InvokeRepeating ("TurretRotatorCheck", 0, 0.5f);
		InvincibleParticles.Stop (true, ParticleSystemStopBehavior.StopEmittingAndClear);

		Ability = (ability)SaveAndLoadScript.Instance.SelectedAbility;
		RefreshAbilityImage ();
		RefreshAbilityName ();

		AbilityUI.SetActive (false);
		PowerupUI.SetActive (false);

		TurretSpinSpeed = TurretSpinSpeedNormal;
		LivesAnim.gameObject.SetActive (false);
		CurrentShotObject = StandardShot;

		if (PowerupUI.activeInHierarchy == true) 
		{
			GameController.Instance.PowerupImage_P1 [0].GetComponent<Animator> ().Play ("PowerupListItemPopIn");

			foreach (RawImage powerupimage in GameController.Instance.PowerupImage_P1) 
			{
				if (powerupimage != GameController.Instance.PowerupImage_P1 [0])
				{
					powerupimage.gameObject.GetComponent<Animator> ().Play ("PowerupListItemFadeOutInstant");
				}
			}
		}

		useOverheat = GameController.Instance.gameModifier.useOverheat;
		OverheatImageL.fillAmount = 0;
		OverheatImageL.material.EnableKeyword ("_EMISSION");
	}

	public void StartCoroutines ()
	{
	}

	void GetStartPlayerModifiers ()
	{
		isInRapidFire = GameController.Instance.gameModifier.AlwaysRapidfire;
		isHoming 	  = GameController.Instance.gameModifier.AlwaysHoming;
		isRicochet 	  = GameController.Instance.gameModifier.AlwaysRicochet;
		isInOverdrive = GameController.Instance.gameModifier.AlwaysOverdrive;

		if (GameController.Instance.gameModifier.AlwaysRicochet == true) 
		{
			StandardShotIteration = shotIteration.Enhanced;
			DoubleShotIteration = shotIteration.Enhanced;
			TripleShotIteration = shotIteration.Enhanced;
			RippleShotIteration = shotIteration.Enhanced;
			EnableRicochetObject ();
		}

		if (GameController.Instance.gameModifier.AlwaysOverdrive == true) 
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
		UpdateParticleEffects ();
		UpdateRiskDistanceTime ();
		//CheckDistanceToOtherPlayer ();
	}

	void CheckDistanceToOtherPlayer ()
	{
		if (PlayerOneInstance == this) 
		{
			if (Vector3.Distance (PlayerOneInstance.transform.position, PlayerTwoInstance.transform.position) < 2)
			{
				// Other player is on the right.
				if (PlayerTwoInstance.transform.position.x > transform.position.x) 
				{
					
				}

				// Other player is on the left.
				if (PlayerTwoInstance.transform.position.x < transform.position.x) 
				{

				}

				// Other player is above.
				if (PlayerTwoInstance.transform.position.y > transform.position.y) 
				{

				}

				// Other player is below.
				if (PlayerTwoInstance.transform.position.y < transform.position.y) 
				{

				}
			}
		}

		if (PlayerTwoInstance == this) 
		{
			if (Vector3.Distance (PlayerOneInstance.transform.position, PlayerTwoInstance.transform.position) < 2)
			{

			}
		}
	}

	void UpdateRiskDistanceTime ()
	{
		if (playerCol.transform.position.y >= RiskDistance) 
		{
			if (GameController.Instance.isPaused == false && GameController.Instance.isGameOver == false)
			{
				RiskDistanceTime += Time.unscaledDeltaTime;
			}
		} 
	}

	void FixedUpdate ()
	{
		MovePlayerPhysics ();
	}

	void MovePlayerPhysics ()
	{
		if (TimescaleController.Instance.isEndSequence == false) 
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
		// Player follows "follow position" with smoothing.
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
				Mathf.Clamp (
					Mathf.SmoothDamp (
						PlayerRb.rotation.y, -MovementX, ref RotVelY, SmoothFollowTime * Time.unscaledDeltaTime) * YRotationMultiplier,
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
				PlayerLight.intensity = 0;

				TimescaleController.Instance.OverridingTimeScale = 0.3f;
				TimescaleController.Instance.OverrideTimeScaleTimeRemaining = cooldownTimeRemaining;
			}

			// Cooldown time is finished, re enable the things.
			if (cooldownTimeRemaining <= 0 && GameController.Instance.Lives > 0) 
			{
				RejoinGame ();
				PlayerGuides.SetActive (true);
				playerCol.gameObject.SetActive (true);
				playerTrigger.gameObject.SetActive (true);
				//GameController.Instance.Lives -= 1;
				Invoke ("EnableCollider", 5);
				isInCooldownMode = false;
			}
		}
	}

	void UpdateParticleEffects ()
	{
		var AbilityParticlesForceModule = AbilityActiveParticles.forceOverLifetime;

		if (PlayerId == 1) 
		{
			AbilityParticlesForceModule.x = -playerActions_P1.Move.Value.x * 0.5f;
			AbilityParticlesForceModule.y = -playerActions_P1.Move.Value.y * 0.1f;
		}

		if (PlayerId == 2) 
		{
			AbilityParticlesForceModule.x = -playerActions_P2.Move.Value.x * 0.5f;
			AbilityParticlesForceModule.y = -playerActions_P2.Move.Value.y * 0.1f;
		}
	}

	public void AddParticleActiveEffects ()
	{
		var PowerupActiveParticlesSustainMainModule = PowerupActiveParticles.main;
		newMinSize_PowerupActiveParticles += 0.1f;
		newMaxSize_PowerupActiveParticles += 0.1f;
		PowerupActiveParticlesSustainMainModule.startSize = new ParticleSystem.MinMaxCurve (
			newMinSize_PowerupActiveParticles,
			newMaxSize_PowerupActiveParticles
		);

		PowerupActiveParticles.Play ();
			
		var PowerupActiveParticlesBurstMainModule = PowerupActiveParticlesBurst.main;
		newMinSize_PowerupActiveParticlesBurst += 0.5f;
		newMaxSize_PowerupActiveParticlesBurst += 0.1f;
		PowerupActiveParticlesBurstMainModule.startSize = new ParticleSystem.MinMaxCurve (
			newMinSize_PowerupActiveParticlesBurst,
			newMaxSize_PowerupActiveParticlesBurst
		);

		PowerupActiveParticlesBurst.Play ();
		// Play firey whoosh sound.
	}

	void ResetParticleActiveEffects ()
	{
		PowerupActiveParticles.Stop (true, ParticleSystemStopBehavior.StopEmitting);

		var PowerupActiveParticlesSustainMainModule = PowerupActiveParticles.main;
		newMinSize_PowerupActiveParticles = 0;
		newMaxSize_PowerupActiveParticles = 0.5f;
		PowerupActiveParticlesSustainMainModule.startSize = new ParticleSystem.MinMaxCurve (
			newMinSize_PowerupActiveParticles,
			newMaxSize_PowerupActiveParticles
		);

		var PowerupActiveParticlesBurstMainModule = PowerupActiveParticlesBurst.main;
		newMinSize_PowerupActiveParticlesBurst = 0;
		newMaxSize_PowerupActiveParticlesBurst = 3;
		PowerupActiveParticlesBurstMainModule.startSize = new ParticleSystem.MinMaxCurve (
			newMinSize_PowerupActiveParticlesBurst,
			newMaxSize_PowerupActiveParticlesBurst
		);
	}

	void UpdateInputUI ()
	{
		ShootingInputImage.fillAmount = playerActions_P1.Shoot.Value;
		AbilityInputImage.fillAmount = playerActions_P1.Ability.Value;

		// This maps a square input into a circle.
		InputUIPoint.anchoredPosition = new Vector3 (
			inputSensitivity * playerActions_P1.Move.Value.x * Mathf.Sqrt (1 - playerActions_P1.Move.Value.y * playerActions_P1.Move.Value.y * 0.5f),
			inputSensitivity * playerActions_P1.Move.Value.y * Mathf.Sqrt (1 - playerActions_P1.Move.Value.x * playerActions_P1.Move.Value.x * 0.5f),
			0
		);
			
		InputForegroundImage.rectTransform.sizeDelta = new Vector2 (
			Mathf.Lerp (
				InputForegroundImage.rectTransform.sizeDelta.x, 
				-(1 * (playerActions_P1.Move.Value.normalized.magnitude)) + 2f, 
				8 * Time.unscaledDeltaTime
			), 

			Mathf.Lerp (
				InputForegroundImage.rectTransform.sizeDelta.y, 
				-(1 * (playerActions_P1.Move.Value.normalized.magnitude)) + 2f, 
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
				0.4f * playerActions_P1.Move.Value.normalized.magnitude, 
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
				0.05f * playerActions_P1.Move.Value.normalized.magnitude, 
				5 * Time.unscaledDeltaTime
			)
		);
	}
		
	// Called by other scrips to set cooldown times.
	public void SetCooldownTime (float cooldownTime)
	{
		cooldownDuration = cooldownTime;
		cooldownTimeRemaining = cooldownDuration;
		GameController.Instance.NextPowerupSlot_P1 = 1;
		GameController.Instance.NextPowerupShootingSlot_P1 = 0;
	}

	// Cooldown sequence.
	public void StartCooldown ()
	{
		if (GameController.Instance.Lives > 0) 
		{
			GameController.Instance.TargetDepthDistance = 0.1f;
			isInCooldownMode = true;
			UsePlayerFollow = false;
		}
	}

	// When the player runs out of lives and unable to respawn.
	public void GameOver ()
	{
		StopCoroutine (GameOverDelay (GameController.Instance.gameModifier.TrialTime));
		GameController.Instance.isGameOver = true;
		TimescaleController.Instance.isEndSequence = true;
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
		AudioController.Instance.StopAllSoundtracks ();
		StartCoroutine (TimescaleController.Instance.EndSequenceTimeScale ());
		spotlightsScript.gameObject.SetActive (false);
	}

	public IEnumerator GameOverDelay (float delay)
	{
		yield return new WaitForSecondsRealtime (delay);
		GameOver ();
	}

	// Impacts by any hazardous object.
	public void PlayerImpactGeneric ()
	{
		#if UNITY_EDITOR
		//UnityEditor.EditorApplication.isPaused = true;
		#endif

		GameController.Instance.isUpdatingImageEffects = true;
		timeIsSlowed = false;
		SetCooldownTime (5);
		GlitchEffect.Play ("CameraGlitchOn");
		PlayerExplosionParticles.Play ();
		PlayerRb.transform.position = new Vector3 (0, -15, 0);
		StartCoroutine (UsePlayerExplosion ());
		ResetPowerups ();
		playerCol.enabled = false;
		playerTrigger.enabled = false;
		playerCol.gameObject.SetActive (false);
		playerTrigger.gameObject.SetActive (false);
		PlayerGuides.transform.position = Vector3.zero;
		PlayerRb.velocity = Vector3.zero;
		PlayerFollowRb.velocity = Vector3.zero;
		MovementX = 0;
		MovementY = 0;
		canShoot = false;
		StartCooldown ();
		PlayerExplosionAudio.Play ();
		GameController.Instance.combo = 1;

		LivesAnim.enabled = false;
		GameController.Instance.LifeImages [9].gameObject.SetActive (false); // Turn off element 9 life image.
		GameController.Instance.Lives -= 1;
		GameController.Instance.Lives = Mathf.Clamp (GameController.Instance.Lives, 0, GameController.Instance.MaxLives);

		if (GameController.Instance.Lives <= 10) 
		{
			for (int i = 0; i < 10; i++) 
			{
				if (i > 0) 
				{
					GameController.Instance.LifeImages [Mathf.Clamp (i, 0, GameController.Instance.Lives - 1)].gameObject.SetActive (true);
				}

				RawImage NewLife = GameController.Instance.LifeImages [i];
				NewLife.enabled = true;
				NewLife.color = Color.white;
				NewLife.texture = NewLife.GetComponent<TextureSwapper> ().Textures [0];

				// Update animator states.
				if (NewLife.gameObject.activeInHierarchy == true) 
				{
					//NewLife.GetComponent<Animator> ().Play ("LifeImageEnter");
					//NewLife.GetComponent<Animator> ().SetTrigger ("LifeImageEnter");
					NewLife.GetComponent<Animator> ().SetBool ("Hidden", false);
				}

				// Max lives state.
				GameController.Instance.LivesSpacing.SetActive (false);
				GameController.Instance.LivesText.gameObject.SetActive (false);
				GameController.Instance.LivesText.text = "";
				GameController.Instance.MaxLivesText.text = "";
			}
		}

		GameController.Instance.LivesAnim.SetTrigger ("UpdateLives");

		if (GameController.Instance.LifeImages [GameController.Instance.Lives - 1].gameObject.activeSelf == true) 
		{
			GameController.Instance.LifeImages [GameController.Instance.Lives - 1].gameObject.GetComponent<Animator> ().SetTrigger ("LifeImageExit");
		}

		if (timeIsSlowed == false) 
		{
			TimescaleController.Instance.OverrideTimeScaleTimeRemaining += 2;
			TimescaleController.Instance.OverridingTimeScale = 0.25f;
		}

		ImpactTransform.position = ImpactPoint;
		spotlightsScript.NewTarget = ImpactTransform;
		spotlightsScript.OverrideSpotlightLookObject ();
		spotlightsScript.ImpactSpotlightSettings ();

		OverrideBitcrushParameters ();

		if (GameController.Instance.isInBonusRound == true) 
		{
			Debug.Log ("Bonus round cancelled, loading up next boss.");
		}

		return;
	}

	// Special impacts by blocks.
	public void PlayerBlockImpact (Block ImpactBlock)
	{
		ImpactPoint = ImpactBlock.transform.position;
		PlayerExplosionParticles.transform.position = ImpactPoint;
		Instantiate (ImpactBlock.playerExplosion, ImpactPoint, Quaternion.identity);
		camShakeScript.ShakeCam (ImpactBlock.newCamShakeAmount * 10, ImpactBlock.newCamShakeAmount * 10, 99);
		AudioController.Instance.SetTargetLowPassFreq (ImpactBlock.LowPassTargetFreq);
		AudioController.Instance.SetTargetResonance (ImpactBlock.ResonanceTargetFreq);
		return;
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
		return;
	}

	// When cooldown time is complete.
	void EnableCollider ()
	{
		UsePlayerFollow = true; // Allow player input to manipulate player position.
		spotlightsScript.OverrideSpotlightLookObject ();
		spotlightsScript.NormalSpotlightSettings ();
		PlayerLight.intensity = 12;

		// Checks for gode mode, allows god mode to stay on if needed.
		if (DeveloperMode.Instance.isGod == false) 
		{
			playerCol.enabled = true;
			playerTrigger.enabled = true;
		}
	}

	// Allows player to re enter while temporarily invincible.
	void RejoinGame ()
	{
		CurrentShootingHeat = 0;
		CurrentShootingCooldown = 0;
		PlayerFollow.transform.position = Vector3.zero;
		canShoot = true;
		UsePlayerFollow = true;
		playerMesh.SetActive (true);
		AudioController.Instance.TargetCutoffFreq = 22000;
		AudioController.Instance.TargetResonance = 1;
		InvincibleParticles.Play ();
		spotlightsScript.NewTarget = playerMesh.transform;
		spotlightsScript.OverrideSpotlightLookObject ();
		spotlightsScript.SuccessSpotlightSettings ();
		GameController.Instance.VhsAnim.SetTrigger ("Play");
		Invoke ("ResetBitcrushParameters", 5);
	}

	void OverrideBitcrushParameters ()
	{
		BitcrusherGroup.Instance.UpdateBitcrusherParameters (BitcrusherGroup.Instance.OverrideBitcrushParameters);
		BitcrusherGroup.Instance.UpdateBitcrushers ();
	}

	void ResetBitcrushParameters ()
	{
		BitcrusherGroup.Instance.UpdateBitcrusherParameters (BitcrusherGroup.Instance.NormalBitcrushParameters);
		BitcrusherGroup.Instance.UpdateBitcrushers ();
	}

	void PlaySpaceshipAmbience ()
	{
		SpaceshipAmbience.Play ();
	}

	void UpdateAudio ()
	{
		SpaceshipAmbience.panStereo = 0.04f * transform.position.x; // Pans audio based on x-position.

		if (PlayerId == 1)
		{
			SpaceshipAmbience.pitch = Mathf.Lerp (
				SpaceshipAmbience.pitch, 
				Time.timeScale * playerActions_P1.Move.Value.magnitude + 0.2f, 
				Time.deltaTime * 10
			);

			SpaceshipAmbience.volume = Mathf.Lerp (
				SpaceshipAmbience.volume, 
				0.1f * playerActions_P1.Move.Value.magnitude + 0.1f, 
				Time.deltaTime * 10
			);
		}

		if (PlayerId == 2)
		{
			SpaceshipAmbience.pitch = Mathf.Lerp (
				SpaceshipAmbience.pitch, 
				Time.timeScale * playerActions_P2.Move.Value.magnitude + 0.2f, 
				Time.deltaTime * 10
			);

			SpaceshipAmbience.volume = Mathf.Lerp (
				SpaceshipAmbience.volume, 
				0.1f * playerActions_P2.Move.Value.magnitude + 0.1f, 
				Time.deltaTime * 10
			);
		}
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
		if (GameController.Instance.isGameOver == false)
		{
			if (UsePlayerFollow == true && TimescaleController.Instance.isEndSequence == false)
			{
				if (GameController.Instance.isPaused == false) 
				{
					// Reads movement input on two axis.
					if (FlipScreenAnim.transform.eulerAngles.z < 90) // When screen is right way up.
					{
						if (PlayerId == 1) 
						{
							if (InputManager.Devices.Count > 0) 
							{
								if (InputManager.Devices [0].IsAttached) 
								{
									MovementX = InputManager.Devices [0].LeftStick.Value.x;
									MovementY = InputManager.Devices [0].LeftStick.Value.y;
								}
							} 

							else 
							
							{
								MovementX = playerActions_P1.Move.Value.x;
								MovementY = playerActions_P1.Move.Value.y;
							}
						}

						if (PlayerId == 2) 
						{
							if (InputManager.Devices.Count > 1) 
							{
								if (InputManager.Devices [1].IsAttached)
								{
									MovementX = InputManager.Devices [1].LeftStick.Value.x;
									MovementY = InputManager.Devices [1].LeftStick.Value.y;
								}
							}
						}
					}

					// Mirror the controls if the screen is flipped.
					if (FlipScreenAnim.transform.eulerAngles.z >= 90) // When screen is flipped.
					{
						if (PlayerId == 1) 
						{
							if (InputManager.Devices [0].IsAttached) 
							{
								MovementX = -InputManager.Devices [0].LeftStick.Value.x;
								MovementY = InputManager.Devices [0].LeftStick.Value.y;
							}
						}

						if (PlayerId == 2) 
						{
							if (InputManager.Devices.Count > 1)
							{
								if (InputManager.Devices [1].IsAttached) 
								{
									MovementX = -InputManager.Devices [1].LeftStick.Value.x;
									MovementY = InputManager.Devices [1].LeftStick.Value.y;
								}
							}
						}
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

			if (PlayerId == 1)
			{
				float SmoothEmissionRate = 
					Mathf.Lerp (
						MainEngineEmissionRate.rateOverTime.constant, 
						MainEngineParticleEmissionAmount * playerActions_P1.Move.Up.Value,
						MainEngineParticleEmissionLerpSpeed * Time.deltaTime
					);
			
				MainEngineEmissionRate.rateOverTime = SmoothEmissionRate;
			}

			if (PlayerId == 2) 
			{
				float SmoothEmissionRate = 
					Mathf.Lerp (
						MainEngineEmissionRate.rateOverTime.constant, 
						MainEngineParticleEmissionAmount * playerActions_P2.Move.Up.Value,
						MainEngineParticleEmissionLerpSpeed * Time.deltaTime
					);

				MainEngineEmissionRate.rateOverTime = SmoothEmissionRate;
			}
		}
	}

	void CheckAbilityTime ()
	{
		// Updates the ability UI involved.
		GameController.Instance.AbilityTimeAmountProportion = GameController.Instance.CurrentAbilityTimeRemaining / GameController.Instance.CurrentAbilityDuration;
		AbilityFillImage.fillAmount = 1f * GameController.Instance.AbilityTimeAmountProportion;

		if (PlayerId == 1) 
		{
			if (InputManager.Devices.Count > 0) 
			{
				if (InputManager.Devices [0].IsAttached) 
				{
					// Player presses ability button.
					if ((InputManager.Devices [0].LeftTrigger.Value > 0.75f || InputManager.Devices [0].Action3.IsPressed)
					    && GameController.Instance.isPaused == false &&
					    isInCooldownMode == false)
					{
						// Ability is charged.
						if (CurrentAbilityState == abilityState.Ready &&
						    cooldownTimeRemaining <= 0 &&
						    TimescaleController.Instance.isInInitialSequence == false &&
						    TimescaleController.Instance.isInInitialCountdownSequence == false)
						{
							ActivateAbility ();
							CurrentAbilityState = abilityState.Active;
						}
					}
				}
			} 

			else 
			
			{
				if (Input.GetKeyDown (KeyCode.LeftAlt) && 
					GameController.Instance.isPaused == false &&
					isInCooldownMode == false) 
				{
					// Ability is charged.
					if (CurrentAbilityState == abilityState.Ready &&
						cooldownTimeRemaining <= 0 &&
						TimescaleController.Instance.isInInitialSequence == false &&
						TimescaleController.Instance.isInInitialCountdownSequence == false)
					{
						ActivateAbility ();
						CurrentAbilityState = abilityState.Active;
					}
				}
			}
		}

		if (PlayerId == 2) 
		{
			if (InputManager.Devices.Count > 1) 
			{
				if (InputManager.Devices [1].IsAttached) 
				{
					// Player presses ability button.
					if ((InputManager.Devices [1].LeftTrigger.Value > 0.75f || InputManager.Devices [1].Action3.IsPressed)
					   && GameController.Instance.isPaused == false &&
					   isInCooldownMode == false)
					{
						// Ability is charged.
						if (CurrentAbilityState == abilityState.Ready &&
						   cooldownTimeRemaining <= 0 &&
						   TimescaleController.Instance.isInInitialSequence == false &&
						   TimescaleController.Instance.isInInitialCountdownSequence == false) 
						{
							ActivateAbility ();
							CurrentAbilityState = abilityState.Active;
						}
					}
				}
			}
		}

		// Updates the ability timers.
		if (CurrentAbilityState == abilityState.Active) 
		{
			AbilityFillImage.material.SetColor ("_EmissionColor",
				AbilityUseColor * AbilityBrightness
			);
				
			if (GameController.Instance.CurrentAbilityTimeRemaining > 0)
			{
				GameController.Instance.CurrentAbilityTimeRemaining -= 0.75f * AbilityUseSpeedMultiplier * Time.unscaledDeltaTime;
			}

			if (GameController.Instance.CurrentAbilityTimeRemaining <= 0) 
			{
				CurrentAbilityState = abilityState.Charging;
				DeactivateAbility ();
				AbilityDampening *= AbilityDampeningMultiplier;
			}

			if (AbilityActiveParticles.isPlaying == false)
			{
				AbilityActiveParticles.Play ();
			}	

			GameController.Instance.CurrentAbilityTimeRemaining = Mathf.Clamp (GameController.Instance.CurrentAbilityTimeRemaining, 0, GameController.Instance.CurrentAbilityDuration);
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
			if (GameController.Instance.CurrentAbilityTimeRemaining < GameController.Instance.CurrentAbilityDuration && 
				cooldownTimeRemaining <= 0 &&
				TimescaleController.Instance.isInInitialSequence == false && 
				TimescaleController.Instance.isInInitialCountdownSequence == false && 
				GameController.Instance.isPaused == false &&
				TutorialManager.Instance.tutorialComplete == true) 
			{
				//GameController.Instance.CurrentAbilityTimeRemaining += AbilityChargeSpeedMultiplier * Time.unscaledDeltaTime; // Add slowdown.
			}

			if (GameController.Instance.CurrentAbilityTimeRemaining >= GameController.Instance.CurrentAbilityDuration) 
			{
				GameController.Instance.CurrentAbilityTimeRemaining = GameController.Instance.CurrentAbilityDuration;
				AbilityCompletion.Play ("AbilityComplete");
				AbilityCompletionTexture.texture = AbilityImage.texture;
				AbilityCompletionText.text = ParseByCase(Ability.ToString ());

				if (timeIsSlowed == false)
				{
					TimescaleController.Instance.OverrideTimeScaleTimeRemaining += 1f;
					TimescaleController.Instance.OverridingTimeScale = 0.1f;
				}

				CurrentAbilityState = abilityState.Ready;

				AbilityAnim.Play ("AbilityBounce");
			}

			if (GameController.Instance.AbilityTimeAmountProportion < 1f)
			{
				AbilityFillImage.material.SetColor ("_EmissionColor",
					AbilityChargingColor * AbilityBrightness
				);
			}
		}

		// Update lens length.
		if (lensScript.BlackHoles.Count > 0) 
		{
			lensScript.BlackHoles [PlayerId - 1].radius = Mathf.Lerp (
				lensScript.BlackHoles [PlayerId - 1].radius, 
				TargetLensRadius, 
				LensRadiusSmoothTime * Time.unscaledDeltaTime
			);
		}

		Vector3 targetShieldScale = new Vector3 (
			TargetShieldScale, 
			TargetShieldScale, 
			TargetShieldScale
		);

		Shield.transform.localScale = Vector3.Lerp (
			Shield.transform.localScale, 
			targetShieldScale, 
			ShieldScaleSmoothTime * Time.unscaledDeltaTime
		);
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

			//lensScript.ratio = rati

			if (PlayerId == 1) 
			{
				lensScript.BlackHoles [0].enabled = true;
			}

			if (PlayerId == 2) 
			{
				lensScript.BlackHoles [1].enabled = true;
			}

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
			TimescaleController.Instance.SetRewindTime (true, 8);
			GameController.Instance.VhsAnim.SetTrigger ("Rewind");
			break;
		case ability.Mirror:
			MirrorPlayer.SetActive (true);
			break;
		}

		AbilityActivations++;

		// Briefly slows time down for effect.
		TimescaleController.Instance.OverrideTimeScaleTimeRemaining += 0.75f;
		TimescaleController.Instance.OverridingTimeScale = 0.3f;

		camShakeScript.ShakeCam (0.4f, GameController.Instance.CurrentAbilityDuration, 6);

		AbilityActiveParticles.Play ();
		AbilityActiveBurstParticles.Play ();
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

	// Use Emp ability.
	public IEnumerator UsePlayerExplosion ()
	{
		PlayerExplosionEmp.transform.position = ImpactPoint;
		PlayerExplosionEmp.Play ();

		yield return new WaitForSeconds (3);

		PlayerExplosionEmp.Stop (true, ParticleSystemStopBehavior.StopEmitting);

		yield return new WaitForSeconds (3);
	}

	// Turn off ability when timer runs out. But allow things to gradually disappear by invoking with delay.
	public void DeactivateAbility ()
	{
		AbilityActiveParticles.Stop (true, ParticleSystemStopBehavior.StopEmitting);

		// Deactivates shield.
		isShieldOn = false;
		TargetLensRadius = 0;
		TargetShieldScale = 0;
		Invoke ("DeactivateShield", 1);

		if (AbilityActiveParticles.isPlaying == true)
		{
			AbilityActiveParticles.Stop (true, ParticleSystemStopBehavior.StopEmitting);
		}	

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

		if (Ability == ability.Rewind)
		{
			StopRewinding (); // Stops rewinding.
			GameController.Instance.VhsAnim.SetTrigger ("Play");
		}

		// Reset the camera shake.
		camShakeScript.ShakeCam (0.0f, 0, 7);

		MirrorPlayer.SetActive (false);
	}

	void StopRewinding ()
	{
		// Stop rewinding.
		TimescaleController.Instance.SetRewindTime (false, 0);
	}

	// Deactivate Shield.
	void DeactivateShield ()
	{
		if (DeveloperMode.Instance.isGod == false) 
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
	public static string ParseByCase (string strInput)
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
			}

			// Put a space before each upper case character if the previous character is lower case
			if (char.IsUpper(chrCurrentInputChar) == true && char.IsLower(chrPreviousInputChar) == true)
			{   
				// Add a space to the output string
				strOutput += " ";
			}

			// Add the character from the input string to the output string
			strOutput += chrCurrentInputChar;
		}

		// Return the altered string
		return strOutput;
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
		case ability.Mirror:
			AbilityImage.texture = AbilityTextures [5];
			break;
		}
	}
		
	void DrawReferencePointLine ()
	{
		RelativeDistance = Vector3.Distance (playerCol.transform.position, ReferencePoint.transform.position);

		if (PlayerController.PlayerOneInstance == this)
		{
			Debug.DrawLine (playerCol.transform.position, ReferencePoint.transform.position, Color.red);
		}

		if (PlayerController.PlayerTwoInstance == this) 
		{
			Debug.DrawLine (playerCol.transform.position, PlayerController.PlayerOneInstance.playerCol.transform.position, new Color (1, 0.5f, 0));
		}
	}

	void CheckShootingCooldown ()
	{
		if (useOverheat == true)
		{
			// Maps heat to squared of shooting cooldown.
			CurrentShootingHeat = Mathf.Pow (CurrentShootingCooldown, 1);

			// Clamps to 0 and 1.
			CurrentShootingHeat = Mathf.Clamp (CurrentShootingHeat, 0, 1);
			CurrentShootingCooldown = Mathf.Clamp (CurrentShootingCooldown, 0, 1);

			OverheatImageL.fillAmount = Mathf.Lerp (
				OverheatImageL.fillAmount, 
				CurrentShootingHeat, 
				OverheatFillSmoothing * Time.unscaledDeltaTime
			);

			if (CurrentShootingHeat <= 0.01f) 
			{
				Overheated = false;
			}

			if (Overheated == false) 
			{
				OverheatImageL.material.SetColor ("_EmissionColor", new Color (
					OverheatImageL.fillAmount,
					-Mathf.Cos ((Mathf.PI * OverheatImageL.fillAmount) + (0.5f * Mathf.PI)),
					1 - OverheatImageL.fillAmount
				) * HeatUIBrightness);

				if (OverheatImageL.fillAmount > 0.99f) 
				{
					if (OverheatSound.isPlaying == false) 
					{
						OverheatSound.Play ();
					}

					Overheated = true;
				}
			}

			if (Overheated == true) 
			{
				CurrentShootingCooldown -= Time.unscaledDeltaTime * OverheatCooldownDecreaseRate;
				OverheatImageL.color = HotColor * HeatUIBrightness;
			}
		}
	}

	// Checks shooting state.
	void CheckShoot ()
	{
		if (PlayerId == 1) 
		{
			if (canShoot == true) 
			{
				if (InputManager.Devices.Count > 0) 
				{
					if (InputManager.Devices [0].IsAttached)
					{
						if ((InputManager.Devices [0].RightTrigger.Value > 0.75f || InputManager.Devices [0].Action1.IsPressed)
						    && GameController.Instance.isPaused == false) 
						{
							if (Time.time >= NextFire) 
							{
								// Every time the player shoots, decremement the combo.
								if (GameController.Instance.combo > 1) 
								{
									GameController.Instance.combo -= 1;
								}

								if (Overheated == false && AbilityFillImage.color != HotColor) 
								{
									Shoot ();

									if (MirrorPlayer.activeInHierarchy == true) 
									{
										mirrorPlayerScript.Shoot ();
									}
							
									NextFire = Time.time + (CurrentFireRate / (FireRateTimeMultiplier));
									NonShootingTime = 0;
								}
							}

							if (Overheated == false) {
								CurrentShootingCooldown += (CurrentShootingHeatCost / FireRateTimeMultiplier) * Time.deltaTime; // Increase by cost.
							}
						}

						if ((InputManager.Devices [0].RightTrigger.Value < 0.75f || InputManager.Devices [0].Action1.WasReleased)
						    && Time.time >= NextFire && GameController.Instance.isPaused == false) {
							if (GameController.Instance.isPaused == false && GameController.Instance.isGameOver == false) 
							{
								NonShootingTime += Time.unscaledDeltaTime;
							}

							if (Overheated == false) 
							{
								// Keeps decreasing heat over time.
								CurrentShootingCooldown -= Time.deltaTime * ShootingCooldownDecreaseRate;
							}
						}
					}
				}

				else 
				
				{
					if ((Input.GetKey (KeyCode.Space) || Input.GetKey (KeyCode.LeftControl))
						&& GameController.Instance.isPaused == false) 
					{
						if (Time.time >= NextFire) 
						{
							// Every time the player shoots, decremement the combo.
							if (GameController.Instance.combo > 1) 
							{
								GameController.Instance.combo -= 1;
							}

							if (Overheated == false && AbilityFillImage.color != HotColor) 
							{
								Shoot ();

								if (MirrorPlayer.activeInHierarchy == true) 
								{
									mirrorPlayerScript.Shoot ();
								}

								NextFire = Time.time + (CurrentFireRate / (FireRateTimeMultiplier));
								NonShootingTime = 0;
							}
						}

						if (Overheated == false) {
							CurrentShootingCooldown += (CurrentShootingHeatCost / FireRateTimeMultiplier) * Time.deltaTime; // Increase by cost.
						}
					}

					if ((Input.GetKeyUp (KeyCode.Space) || Input.GetKeyUp (KeyCode.LeftControl))
						&& Time.time >= NextFire && GameController.Instance.isPaused == false) {
						if (GameController.Instance.isPaused == false && GameController.Instance.isGameOver == false) 
						{
							NonShootingTime += Time.unscaledDeltaTime;
						}

						if (Overheated == false) 
						{
							// Keeps decreasing heat over time.
							CurrentShootingCooldown -= Time.deltaTime * ShootingCooldownDecreaseRate;
						}
					}
				}
			}
		}

		if (PlayerId == 2) 
		{
			if (canShoot == true) 
			{
				if (InputManager.Devices.Count > 1) 
				{
					if (InputManager.Devices [1].IsAttached) 
					{
						if ((InputManager.Devices [1].RightTrigger.Value > 0.75f || InputManager.Devices [1].Action1.IsPressed)
						   && GameController.Instance.isPaused == false) 
						{
							if (Time.time >= NextFire)
							{
								// Every time the player shoots, decremement the combo.
								if (GameController.Instance.combo > 1) 
								{
									GameController.Instance.combo -= 1;
								}

								if (Overheated == false && AbilityFillImage.color != HotColor) 
								{
									Shoot ();

									if (MirrorPlayer.activeInHierarchy == true) 
									{
										mirrorPlayerScript.Shoot ();
									}

									NextFire = Time.time + (CurrentFireRate / (FireRateTimeMultiplier));
									NonShootingTime = 0;
								}
							}

							if (Overheated == false) 
							{
								CurrentShootingCooldown += (CurrentShootingHeatCost / FireRateTimeMultiplier) * Time.deltaTime; // Increase by cost.
							}
						}

						if ((InputManager.Devices [1].RightTrigger.Value < 0.75f || InputManager.Devices [1].Action1.WasReleased)
						   && Time.time >= NextFire && GameController.Instance.isPaused == false) 
						{
							if (GameController.Instance.isPaused == false && GameController.Instance.isGameOver == false) 
							{
								NonShootingTime += Time.unscaledDeltaTime;
							}

							if (Overheated == false) 
							{
								// Keeps decreasing heat over time.
								CurrentShootingCooldown -= Time.deltaTime * ShootingCooldownDecreaseRate;
							}
						}
					}
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

		//PlayerRecoil.Play ("PlayerRecoil");
		PlayerRecoil.SetTrigger ("Recoil");
	}

	// Gets pause state.
	// Only allow player 1 to pause.
	void CheckPause ()
	{
		if (playerActions_P1.Pause.WasPressed) 
		{
			if (Time.unscaledTime > GameController.Instance.NextPauseCooldown) 
			{
				GameController.Instance.CheckPause ();
			}
		}

		if (GameController.Instance.isPaused == true) 
		{
			if (isShieldOn == true) 
			{
				if (lensScript.enabled == true) 
				{
					lensScript.enabled = false;
				}
			}
		}

		if (GameController.Instance.isPaused == false) 
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
			if (AbilityUI.activeInHierarchy == true) 
			{
				// Horizontal position too far.
				if (PlayerRb.position.x < AbilityCheckPlayerPos.x)
				{
					if (AbilityUIHexes.GetCurrentAnimatorStateInfo (0).IsName ("HexesFadeOut") == false && isHidingAbilityUI == false) {
						AbilityUIHexes.Play ("HexesFadeOut");
						isHidingAbilityUI = true;
					}
				}

				// Horizontal position in range.
				if (PlayerRb.position.x >= AbilityCheckPlayerPos.x) 
				{
					if (AbilityUIHexes.GetCurrentAnimatorStateInfo (0).IsName ("HexesFadeIn") == false && isHidingAbilityUI == true) {
						AbilityUIHexes.Play ("HexesFadeIn");
						isHidingAbilityUI = false;
					}
				}
			}
		}

		if (AbilityUI.activeInHierarchy == true)
		{
			// Vertical position too far from score text.
			if (PlayerRb.position.y <= AbilityCheckPlayerPos.y && isInCooldownMode == false) 
			{
				if (AbilityUIHexes.GetCurrentAnimatorStateInfo (0).IsName ("HexesFadeIn") == false && isHidingAbilityUI == true)
				{
					AbilityUIHexes.Play ("HexesFadeIn");
					isHidingAbilityUI = false;
				}
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
					PointsThisComboAnim.SetBool ("Hidden", true);
				}
			}

			// Horizontal position in range.
			if (PlayerRb.position.x <= -ScoreCheckPlayerPos.x || PlayerRb.position.x >= ScoreCheckPlayerPos.x && isInCooldownMode == false)  
			{
				if (ScoreAnim.GetCurrentAnimatorStateInfo (0).IsName ("ScoreFadeIn") == false && isHidingScoreUI == true) 
				{
					ScoreAnim.Play ("ScoreFadeIn");
					isHidingScoreUI = false;
					PointsThisComboAnim.SetBool ("Hidden", false);
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
				PointsThisComboAnim.SetBool ("Hidden", false);
			}
		}

		// LIVES UI
		// When the player is close to the lives text.
		// Vertical position in range.
		if (PlayerRb.position.y > LivesCheckPlayerPosY.y && LivesAnim.gameObject.activeInHierarchy == true && isInCooldownMode == false) 
		{
			// Horizontal position in range.
			if (PlayerRb.position.x < LivesCheckPlayerPosX.x) 
			{
				if (LivesAnim.GetCurrentAnimatorStateInfo (0).IsName ("LivesFadeOut") == false && isHidingLivesUI == false
					&& isInCooldownMode == false) 
				{
					if (GameController.Instance.LifeImages [GameController.Instance.Lives - 1].gameObject.activeInHierarchy == true) 
					{
						GameController.Instance.LifeImages [GameController.Instance.Lives - 1].gameObject.GetComponent<Animator> ().enabled = true;
					}

					LivesAnim.Play ("LivesFadeOut");
					isHidingLivesUI = true;

					for (int i = 0; i < GameController.Instance.LifeImages.Length; i++) 
					{
						if (GameController.Instance.LifeImages [i].gameObject.activeInHierarchy == true) 
						{
							if (GameController.Instance.LifeImages [i].GetComponent<Animator> ().GetBool ("Hidden") == false) 
							{
								GameController.Instance.LifeImages [i].GetComponent<Animator> ().SetBool ("Hidden", true);
							}
						}
					}
				}
			}

			// Horizontal position outside range.
			if (PlayerRb.position.x >= LivesCheckPlayerPosX.x) 
			{
				if (LivesAnim.GetCurrentAnimatorStateInfo (0).IsName ("LivesFadeIn") == false && isHidingLivesUI == true
					&& isInCooldownMode == false) 
				{
					LivesAnim.enabled = true;
					GameController.Instance.LifeImages [GameController.Instance.Lives - 1].gameObject.GetComponent<Animator> ().enabled = false;
					LivesAnim.Play ("LivesFadeIn");
					isHidingLivesUI = false;

					for (int i = 0; i < GameController.Instance.LifeImages.Length; i++) 
					{
						if (GameController.Instance.LifeImages [i].gameObject.activeInHierarchy == true) 
						{
							if (GameController.Instance.LifeImages [i].GetComponent<Animator> ().GetBool ("Hidden") == true) 
							{
								GameController.Instance.LifeImages [i].GetComponent<Animator> ().SetBool ("Hidden", false);
							}
						}
					}
				}
			}
		}

		// Vertical position too far from lives text.
		if (PlayerRb.position.y <= LivesCheckPlayerPosY.y && LivesAnim.gameObject.activeInHierarchy == true && isInCooldownMode == false) 
		{
			if (LivesAnim.GetCurrentAnimatorStateInfo (0).IsName ("LivesFadeIn") == false && isHidingLivesUI == true
				&& isInCooldownMode == false) 
			{
				LivesAnim.enabled = true;
				LivesAnim.Play ("LivesFadeIn");
				isHidingLivesUI = false;

				for (int i = 0; i < GameController.Instance.LifeImages.Length; i++) 
				{
					if (GameController.Instance.LifeImages [i].gameObject.activeInHierarchy == true) 
					{
						if (GameController.Instance.LifeImages [i].GetComponent<Animator> ().GetBool ("Hidden") == true) 
						{
							GameController.Instance.LifeImages [i].GetComponent<Animator> ().SetBool ("Hidden", false);
						}
					}
				}
			}
		}

		CheckPowerupImageUI ();

		// Defaults powerup texture with standard shot image.
		if (GameController.Instance.PowerupImage_P1 [0].texture == null) 
		{
			GameController.Instance.PowerupImage_P1 [0].color = Color.white;
			GameController.Instance.PowerupImage_P1 [0].texture = GameController.Instance.StandardShotTexture;
		}
	}

	// Tracks powerup images and available slots to populate.
	// Also has autohiding.
	public void CheckPowerupImageUI ()
	{
		if (PowerupUI.activeInHierarchy == true && isInCooldownMode == false) 
		{
			foreach (RawImage powerupimage in GameController.Instance.PowerupImage_P1) 
			{
				if (powerupimage.gameObject.activeInHierarchy == true) 
				{
					if (playerCol.transform.position.y > PowerupUICheckPos.y &&
					   playerCol.transform.position.x > PowerupUICheckPos.x) 
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
			   playerCol.transform.position.x > PowerupUICheckPos.x)
			{
				GameController.Instance.HomingImage.GetComponent<Animator> ().SetBool ("Visible", false);
				GameController.Instance.RicochetImage.GetComponent<Animator> ().SetBool ("Visible", false);
				GameController.Instance.RapidfireImage.GetComponent<Animator> ().SetBool ("Visible", false);
				GameController.Instance.OverdriveImage.GetComponent<Animator> ().SetBool ("Visible", false);
			}

			if (playerCol.transform.position.y <= PowerupUICheckPos.y ||
			   playerCol.transform.position.x <= PowerupUICheckPos.x) 
			{
				GameController.Instance.HomingImage.GetComponent<Animator> ().SetBool ("Visible", true);
				GameController.Instance.RicochetImage.GetComponent<Animator> ().SetBool ("Visible", true);
				GameController.Instance.RapidfireImage.GetComponent<Animator> ().SetBool ("Visible", true);
				GameController.Instance.OverdriveImage.GetComponent<Animator> ().SetBool ("Visible", true);
			}
		}
	}

	// Turn off Helix.
	void TurnOffHelix ()
	{
		Helix.SetActive (false);
	}

	// Tracks rotation amount degrees per second based on powerup time remaining.
	void TurretRotatorCheck ()
	{
		TurretRotatorScript.rotateDegreesPerSecond.value = new Vector3 (0, 0, TurretSpinSpeed);

		if (GameController.Instance.PowerupTimeRemaining > 3) 
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
		GameController.Instance.isUpdatingImageEffects = true;

		// Sets all shot types to standard iteration.
		ShotType = shotType.Standard;
		CurrentShootingHeatCost = StandardShootingHeatCost;
		nextTurretSpawn = 0; // Resets turrent spawn index.

		if (timeIsSlowed == true) 
		{
			timeIsSlowed = false;
			GameController.Instance.VhsAnim.SetTrigger ("Play");
		}

		// Resets homing mode if not modified by game modifier object.
		isHoming = GameController.Instance.gameModifier.AlwaysHoming;
		isRicochet = GameController.Instance.gameModifier.AlwaysRicochet;
		isInRapidFire = GameController.Instance.gameModifier.AlwaysRapidfire;
		isInOverdrive = GameController.Instance.gameModifier.AlwaysOverdrive;

		// Resets ricochet mode if not modified by game modifier object.
		if (GameController.Instance.gameModifier.AlwaysRicochet == false)
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
		if (GameController.Instance.gameModifier.AlwaysRapidfire == false) 
		{
			CurrentFireRate = StandardFireRate;
		}
			
		// Resets overdrive mode if not modified by game modifier object.
		if (GameController.Instance.gameModifier.AlwaysOverdrive == false) 
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
		GameController.Instance.PowerupTimeRemaining = 0;
		powerupsInUse = 0;

		// Clears powerup UI.
		CheckPowerupImageUI ();
		GameController.Instance.ClearPowerupUI ();

		TimescaleController.Instance.OverrideTimeScaleTimeRemaining = 0;
		TimescaleController.Instance.isOverridingTimeScale = false;

		ResetParticleActiveEffects ();
	}

	void UpdateImageEffects ()
	{
		if (GameController.Instance.isUpdatingImageEffects == true) 
		{
			float localDistance = TimescaleController.Instance.Distance;

			var PostProcessBloomSettings = PostProcessProfile.bloom.settings;
			PostProcessBloomSettings.bloom.intensity = 0.001f * localDistance + 0.005f;
			PostProcessProfile.bloom.settings = PostProcessBloomSettings;

			var PostProcessColorGradingSettings = PostProcessProfile.colorGrading.settings;
			PostProcessColorGradingSettings.basic.saturation = Mathf.Lerp (
				PostProcessColorGradingSettings.basic.saturation,
				0.003f * localDistance + 0.85f,
				Time.unscaledDeltaTime
			);
			PostProcessProfile.colorGrading.settings = PostProcessColorGradingSettings;
		}
	}

	// This is for InControl for initialization.
	void CreatePlayerActions ()
	{
		playerActions_P1 = new PlayerActions ();
		playerActions_P2 = new PlayerActions ();
	}

	// This is for InControl to be able to read input.
	void AssignActionControls_P1 ()
	{
		// LEFT
		playerActions_P1.MoveLeft.AddDefaultBinding (Key.A);
		playerActions_P1.MoveLeft.AddDefaultBinding (Key.LeftArrow);
		playerActions_P1.MoveLeft.AddDefaultBinding (InputControlType.LeftStickLeft);
		playerActions_P1.MoveLeft.AddDefaultBinding (InputControlType.DPadLeft);

		// RIGHT
		playerActions_P1.MoveRight.AddDefaultBinding (Key.D);
		playerActions_P1.MoveRight.AddDefaultBinding (Key.RightArrow);
		playerActions_P1.MoveRight.AddDefaultBinding (InputControlType.LeftStickRight);
		playerActions_P1.MoveRight.AddDefaultBinding (InputControlType.DPadRight);

		// UP
		playerActions_P1.MoveUp.AddDefaultBinding (Key.W);
		playerActions_P1.MoveUp.AddDefaultBinding (Key.UpArrow);
		playerActions_P1.MoveUp.AddDefaultBinding (InputControlType.LeftStickUp);
		playerActions_P1.MoveUp.AddDefaultBinding (InputControlType.DPadUp);

		// DOWN
		playerActions_P1.MoveDown.AddDefaultBinding (Key.S);
		playerActions_P1.MoveDown.AddDefaultBinding (Key.DownArrow);
		playerActions_P1.MoveDown.AddDefaultBinding (InputControlType.LeftStickDown);
		playerActions_P1.MoveDown.AddDefaultBinding (InputControlType.DPadDown);

		// SHOOT
		playerActions_P1.Shoot.AddDefaultBinding (Key.Space);
		playerActions_P1.Shoot.AddDefaultBinding (Key.LeftControl);
		//playerActions_P1.Shoot.AddDefaultBinding (Mouse.LeftButton); // Commented out for touch controls to work properly.
		playerActions_P1.Shoot.AddDefaultBinding (InputControlType.RightTrigger);
		playerActions_P1.Shoot.AddDefaultBinding (InputControlType.Action1);

		// ABILITY
		playerActions_P1.Ability.AddDefaultBinding (Key.LeftAlt);
		playerActions_P1.Ability.AddDefaultBinding (Mouse.RightButton);
		playerActions_P1.Ability.AddDefaultBinding (InputControlType.LeftTrigger);

		// PAUSE / UNPAUSE
		playerActions_P1.Pause.AddDefaultBinding (Key.Escape);
		playerActions_P1.Pause.AddDefaultBinding (InputControlType.Command);

		// DEBUG / CHEATS
		playerActions_P1.DebugMenu.AddDefaultBinding (Key.Tab);
		playerActions_P1.DebugMenu.AddDefaultBinding (InputControlType.LeftBumper);
		playerActions_P1.CheatConsole.AddDefaultBinding (Key.Backquote);

		playerActions_P1.Back.AddDefaultBinding (Key.Backspace);
		playerActions_P1.Back.AddDefaultBinding (InputControlType.Action2);
	}

	void AssignActionControls_P2 ()
	{
		// LEFT
		//playerActions_P2.MoveLeft.AddDefaultBinding (Key.A);
		playerActions_P2.MoveLeft.AddDefaultBinding (Key.LeftArrow);
		playerActions_P2.MoveLeft.AddDefaultBinding (InputControlType.LeftStickLeft);
		playerActions_P2.MoveLeft.AddDefaultBinding (InputControlType.DPadLeft);
		//playerActions_P2.MoveLeft.AddDefaultBinding (GameController.playerDevices[deviceID].DPadLeft);
		//playerActions_P2.MoveLeft.AddDefaultBinding (GameController.playerDevices[deviceID].LeftStickLeft);

		// RIGHT
		//playerActions_P2.MoveRight.AddDefaultBinding (Key.D);
		playerActions_P2.MoveRight.AddDefaultBinding (Key.RightArrow);
		playerActions_P2.MoveRight.AddDefaultBinding (InputControlType.LeftStickRight);
		playerActions_P2.MoveRight.AddDefaultBinding (InputControlType.DPadRight);

		// UP
		//playerActions_P2.MoveUp.AddDefaultBinding (Key.W);
		playerActions_P2.MoveUp.AddDefaultBinding (Key.UpArrow);
		playerActions_P2.MoveUp.AddDefaultBinding (InputControlType.LeftStickUp);
		playerActions_P2.MoveUp.AddDefaultBinding (InputControlType.DPadUp);

		// DOWN
		//playerActions_P2.MoveDown.AddDefaultBinding (Key.S);
		playerActions_P2.MoveDown.AddDefaultBinding (Key.DownArrow);
		playerActions_P2.MoveDown.AddDefaultBinding (InputControlType.LeftStickDown);
		playerActions_P2.MoveDown.AddDefaultBinding (InputControlType.DPadDown);

		// SHOOT
		//playerActions_P2.Shoot.AddDefaultBinding (Key.Space);
		playerActions_P2.Shoot.AddDefaultBinding (Key.RightControl);
		//playerActions_P2.Shoot.AddDefaultBinding (Mouse.LeftButton); // Commented out for touch controls to work properly.
		playerActions_P2.Shoot.AddDefaultBinding (InputControlType.RightTrigger);
		playerActions_P2.Shoot.AddDefaultBinding (InputControlType.Action1);

		// ABILITY
		playerActions_P2.Ability.AddDefaultBinding (Key.RightAlt);
		//playerActions_P2.Ability.AddDefaultBinding (Mouse.RightButton);
		playerActions_P2.Ability.AddDefaultBinding (InputControlType.LeftTrigger);

		// PAUSE / UNPAUSE
		//playerActions_P2.Pause.AddDefaultBinding (Key.Escape);
		//playerActions_P2.Pause.AddDefaultBinding (InputControlType.Command);

		// DEBUG / CHEATS
		//playerActions_P2.DebugMenu.AddDefaultBinding (Key.Tab);
		//playerActions_P2.DebugMenu.AddDefaultBinding (InputControlType.LeftBumper);
		//playerActions_P2.CheatConsole.AddDefaultBinding (Key.Backquote);

		//playerActions_P2.Back.AddDefaultBinding (Key.Backspace);
		//playerActions_P2.Back.AddDefaultBinding (InputControlType.Action2);
	}

	// Creates vibration.
	public void Vibrate (float LeftMotor, float RightMotor, float duration)
	{
		#if !PLATFORM_STANDALONE_OSX && !PLATFORM_ANDROID && !PLATFORM_WEBGL
		PlayerVibrationDuration = duration;
		PlayerVibrationTimeRemaining = PlayerVibrationDuration;

		if (PlayerId == 1)
		{
			GamePad.SetVibration (PlayerIndex.One, LeftMotor, RightMotor);
		}

		if (PlayerId == 2)
		{
			GamePad.SetVibration (PlayerIndex.Two, LeftMotor, RightMotor);
		}
		#endif
	}

	public void ResetVibration ()
	{
		#if !PLATFORM_STANDALONE_OSX && !PLATFORM_ANDROID && !PLATFORM_WEBGL
		PlayerVibrationDuration = 0;
		PlayerVibrationTimeRemaining = 0;
		GamePad.SetVibration (PlayerIndex.One, 0, 0);
		GamePad.SetVibration (PlayerIndex.Two, 0, 0);
		#endif
	}

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
		#if !PLATFORM_STANDALONE_OSX && !PLATFORM_ANDROID && !PLATFORM_WEBGL
		GamePad.SetVibration (PlayerIndex.One, 0, 0);
		GamePad.SetVibration (PlayerIndex.Two, 0, 0);
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
		if (DeveloperMode.Instance.useCheats == true) 
		{
			if (playerActions_P1.CheatConsole.WasPressed) 
			{
				DeveloperMode.Instance.CheatConsole.SetActive (!DeveloperMode.Instance.CheatConsole.activeSelf);
				DeveloperMode.Instance.ClearCheatString ();

				if (DeveloperMode.Instance.CheatConsole.activeSelf) 
				{
					Debug.Log ("Cheat console opened.");
				}

				if (!DeveloperMode.Instance.CheatConsole.activeSelf) 
				{
					Debug.Log ("Cheat console closed.");
				}
			}

			if (playerActions_P1.CheatConsole.WasReleased)
			{
				DeveloperMode.Instance.ClearCheatString ();
				DeveloperMode.Instance.CheatInputText.text = ">_ ";
			}

			DeveloperMode.Instance.CheatInputText.text = DeveloperMode.Instance.CheatString;
		}
	}

	// Resets joined on disable.
	void OnDisable ()
	{
		if (isJoined == true) 
		{
			isJoined = false;
		}
	}
}