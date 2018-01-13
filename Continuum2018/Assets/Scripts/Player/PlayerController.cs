using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using XInputDotNetPure;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class PlayerController : MonoBehaviour 
{
	public GameController gameControllerScript;
	public TimescaleController timescaleControllerScript;
	public AudioController audioControllerScript;
	public CursorManager cursorManagerScript;
	public CameraShake camShakeScript;
	public DeveloperMode developerModeScript;
	public int PlayerId = 1;
	public TextMeshProUGUI PlayerText;
	public bool isJoined;

	[Header ("Player Movement")]
	[Range (-1, 1)]
	public float MovementX;
	[Range (-1, 1)]
	public float MovementY;

	public bool UsePlayerFollow = true;
	public Transform PlayerFollow;
	public Rigidbody PlayerFollowRb;
	public float PlayerFollowMoveSpeed;
	public Vector2 XBounds, YBounds;
	public Rigidbody PlayerRb;
	private float SmoothFollowVelX, SmoothFollowVelY;
	public float SmoothFollowTime = 1;

	public float YRotationAmount = 45;
	public float YRotationMultiplier = 10;
	private float RotVelY;

	public float PlayerVibrationDuration;
	public float PlayerVibrationTimeRemaining;

	public ParticleSystem MainEngineParticles;
	public float MainEngineParticleEmissionAmount = 1;
	public float MainEngineParticleEmissionLerpSpeed = 4;

	[Header ("Shooting")]
	public bool canShoot = true;
	public float CurrentFireRate = 0.1f;
	public float FireRateTimeMultiplier = 2;
	private float NextFire;

	public shotType ShotType; 
	public enum shotType
	{
		Standard,
		Double,
		Triple,
	}

	public GameObject StandardShot;
	public Transform StandardShotSpawn;
	public float StandardFireRate = 0.1f;

	[Header ("Impact")]
	public GameObject playerMesh;
	public Collider playerCol;
	public bool isInCooldownMode;
	public float cooldownDuration;
	public float cooldownTimeRemaining;
	public ParticleSystem PlayerExplosionParticles;
	public AudioSource PlayerExplosionAudio;
	public ParticleSystem GameOverExplosionParticles;
	public AudioSource GameOverExplosionAudio;
	public MeshRenderer InvincibleMesh;

	[Header ("Ability")]
	public abilityState CurrentAbilityState;
	public enum abilityState
	{
		Charging,
		Ready,
		Active
	}

	public float CurrentAbilityTimeRemaining;
	public float CurrentAbilityDuration;
	public float AbilityTimeAmountProportion;
	public float AbilityChargeSpeedMultiplier = 0.5f;
	public float AbilityUseSpeedMultiplier = 4;

	public string AbilityName;
	public ability Ability;
	public enum ability
	{
		Shield,
		Emp
	}
	public Image AbilityFillImageL;
	public Image AbilityFillImageR;
	public TextMeshProUGUI AbilityReadyText;
	public RawImage AbilityReadyBackground;
	public Color AbilityUseColor, AbilityChargingColor, AbilityChargingFullColor;

	[Header ("Powerups")]
	public int powerupsInUse;

	// Double shot.
	public GameObject DoubleShotL;
	public GameObject DoubleShotLEnhanced;
	public GameObject DoubleShotLOverdrive;
	public GameObject DoubleShotR;
	public GameObject DoubleShotREnhanced;
	public GameObject DoubleShotROverdrive;
	public Transform DoubleShotSpawnL;
	public Transform DoubleShotSpawnR;
	public float[] DoubleShotFireRates;
	public shotIteration DoubleShotIteration;
	public int NextDoubleShotIteration;
	private float DoubleShotNextFire;

	// Triple shot.
	public GameObject TripleShotL;
	public GameObject TripleShotLEnhanced;
	public GameObject TripleShotLOverdrive;
	public GameObject TripleShotM;
	public GameObject TripleShotMEnhanced;
	public GameObject TripleShotMOverdrive;
	public GameObject TripleShotR;
	public GameObject TripleShotREnhanced;
	public GameObject TripleShotROverdrive;
	public Transform TripleShotSpawnL;
	public Transform TripleShotSpawnM;
	public Transform TripleShotSpawnR;
	public float[] TripleShotFireRates;
	public shotIteration TripleShotIteration;
	public int NextTripleShotIteration;
	private float TripleShotNextFire;

	public enum shotIteration
	{
		Standard = 0,
		Enhanced = 1,
		Rapid = 2,
		Overdrive = 3
	}

	[Header ("Shield")]
	public bool isShieldOn;
	public GameObject Shield;
	public Lens lensScript;
	public float TargetShieldScale;
	public float ShieldScaleSmoothTime = 1;
	public float LensOnRadius = 0.7f;
	public float TargetLensRadius;
	public float LensRadiusSmoothTime = 1;

	[Header ("Clone Player")]
	public GameObject Clone;

	[Header ("UI")]
	public bool isHidingScoreUI;
	public Animator ScoreAnim;
	public Vector3 ScoreCheckPlayerPos;

	public bool isHidingLivesUI;
	public Animator LivesAnim;
	public Vector2 LivesCheckPlayerPosX;
	public Vector2 LivesCheckPlayerPosY;

	public bool isHidingWaveUI;
	public Animator WaveAnim;
	public Vector3 WaveCheckPlayerPos;

	public PlayerActions playerActions;

	void Awake ()
	{
		AbilityReadyText.text = "";
		PlayerText.text = " ";
		RefreshAbilityName ();
	}

	void Start () 
	{
		CreatePlayerActions ();
		AssignActionControls ();
		InvokeRepeating ("CheckJoinState", 0, 0.5f);

		// This is for WSA platforms.
		//lensScript.ratio = 1;
	}

	public void StartCoroutines ()
	{
		StartCoroutine (CheckPause ());
		StartCoroutine (CheckBulletType ());
		PlayerText.text = "Player " + PlayerId;
	}

	void Update ()
	{
		MovePlayer ();
		CheckShoot ();
		CheckPlayerVibration ();
		CheckUIVisibility ();
		CheckCooldownTime ();
		CheckAbilityTime ();
		//CheckCheatConsoleInput ();
	}

	void FixedUpdate ()
	{
		MovePlayerPhysics ();
	}

	void MovePlayerPhysics ()
	{
		if (timescaleControllerScript.isEndSequence == false) {
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
		MovePlayerSmoothing ();
		//Camera.main.transform.LookAt (playerCol.transform);
	}

	void MovePlayerSmoothing ()
	{
		// Player follows [follow position] with smoothing.
		PlayerRb.position = new Vector3 (
			Mathf.SmoothDamp (PlayerRb.position.x, PlayerFollow.position.x, ref SmoothFollowVelX, SmoothFollowTime * Time.fixedDeltaTime),
			Mathf.SmoothDamp (PlayerRb.position.y, PlayerFollow.position.y, ref SmoothFollowVelY, SmoothFollowTime * Time.fixedDeltaTime),
			0
		);

		// Rotates on y-axis by x velocity.
		PlayerRb.rotation = Quaternion.Euler
			(
				0, 
				Mathf.Clamp (Mathf.SmoothDamp (PlayerRb.rotation.y, -MovementX, ref RotVelY, SmoothFollowTime * Time.unscaledDeltaTime) * YRotationMultiplier, -YRotationAmount, YRotationAmount), 
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
				//UsePlayerFollow = false;
				cooldownTimeRemaining -= Time.unscaledDeltaTime;
			}

			if (cooldownTimeRemaining <= 0 && gameControllerScript.Lives > 0) 
			{
				RejoinGame ();
				playerCol.gameObject.SetActive (true);
				gameControllerScript.Lives -= 1;
				Invoke ("EnableCollider", 3);
				isInCooldownMode = false;
			}
		}
	}

	public void SetCooldownTime (float cooldownTime)
	{
		cooldownDuration = cooldownTime;
		cooldownTimeRemaining = cooldownDuration;
	}

	public void StartCooldown ()
	{
		if (gameControllerScript.Lives > 0) 
		{
			gameControllerScript.TargetDepthDistance = 0.1f;
			isInCooldownMode = true;
			UsePlayerFollow = false;
			playerCol.transform.localPosition = Vector3.zero;
			PlayerFollow.transform.localPosition = Vector3.zero;
			playerMesh.transform.localPosition = Vector3.zero;
		}
	}

	public void GameOver ()
	{
		gameControllerScript.isGameOver = true;
		timescaleControllerScript.isEndSequence = true;
		canShoot = false;
		UsePlayerFollow = false;
		playerCol.enabled = false;
		playerMesh.SetActive (false);
		PlayerFollowRb.velocity = Vector3.zero;
		GameOverExplosionParticles.Play ();
		GameOverExplosionAudio.Play ();
		audioControllerScript.StopAllSoundtracks ();
		StartCoroutine (timescaleControllerScript.EndSequenceTimeScale ());
	}

	void EnableCollider ()
	{
		UsePlayerFollow = true;

		if (developerModeScript.isGod == false) 
		{
			playerCol.enabled = true;
		}
		InvincibleMesh.enabled = false;
	}

	void RejoinGame ()
	{
		gameControllerScript.TargetDepthDistance = 100;
		canShoot = true;
		UsePlayerFollow = true;
		playerMesh.SetActive (true);
		audioControllerScript.TargetCutoffFreq = 22000;
		audioControllerScript.TargetResonance = 1;

		if (InvincibleMesh.enabled == false) 
		{
			InvincibleMesh.enabled = true;
		}
	}

	public void EnablePlayerInput ()
	{
		UsePlayerFollow = true;
		canShoot = true;
		StartCoroutines ();
	}

	void MovePlayer ()
	{
		if (UsePlayerFollow == true && timescaleControllerScript.isEndSequence == false) 
		{
			if (gameControllerScript.isPaused == false)
			{
				// Reads movement input on two axis.
				MovementX = playerActions.Move.Value.x;
				MovementY = playerActions.Move.Value.y;
			}
		}

		var MainEngineEmissionRate = MainEngineParticles.emission;
		float SmoothEmissionRate = 
			Mathf.Lerp (
				MainEngineEmissionRate.rateOverTime.constant, 
				MainEngineParticleEmissionAmount * playerActions.Move.Up.Value,
				MainEngineParticleEmissionLerpSpeed * Time.deltaTime
			);
		MainEngineEmissionRate.rateOverTime = SmoothEmissionRate;
	}

	void CheckAbilityTime ()
	{
		// Updates the ability UI involved.
		AbilityTimeAmountProportion = CurrentAbilityTimeRemaining / CurrentAbilityDuration;
		AbilityFillImageL.fillAmount = 0.16f * AbilityTimeAmountProportion; // Fills to a sixth.
		AbilityFillImageR.fillAmount = 0.16f * AbilityTimeAmountProportion; // Fills to a sixth.

		// Player presses ability button.
		if (playerActions.Ability.WasPressed && gameControllerScript.isPaused == false) 
		{
			// Ability is charged.
			if (CurrentAbilityState == abilityState.Ready && cooldownTimeRemaining <= 0 &&
				timescaleControllerScript.isInInitialSequence == false && timescaleControllerScript.isInInitialCountdownSequence == false) 
			{
				ActivateAbility ();
				AbilityReadyText.text = "";
				AbilityReadyBackground.enabled = false;
				CurrentAbilityState = abilityState.Active;
			}
		}

		// Updates the ability timers.
		if (CurrentAbilityState == abilityState.Active) 
		{
			AbilityFillImageL.color = AbilityUseColor;
			AbilityFillImageR.color = AbilityUseColor;

			if (CurrentAbilityTimeRemaining > 0)
			{
				CurrentAbilityTimeRemaining -= 0.75f * AbilityUseSpeedMultiplier * Time.unscaledDeltaTime;
			}

			if (CurrentAbilityTimeRemaining <= 0) 
			{
				CurrentAbilityState = abilityState.Charging;
				DeactivateAbility ();
			}
		}

		if (CurrentAbilityState == abilityState.Charging) 
		{
			if (CurrentAbilityTimeRemaining < CurrentAbilityDuration && cooldownTimeRemaining <= 0 &&
				timescaleControllerScript.isInInitialSequence == false && timescaleControllerScript.isInInitialCountdownSequence == false && 
				gameControllerScript.isPaused == false) 
			{
				AbilityReadyText.text = "";
				AbilityReadyBackground.enabled = false;
				CurrentAbilityTimeRemaining += AbilityChargeSpeedMultiplier * Time.unscaledDeltaTime; // Add slowdown.
			}

			if (CurrentAbilityTimeRemaining >= CurrentAbilityDuration) 
			{
				CurrentAbilityTimeRemaining = CurrentAbilityDuration;
				AbilityReadyText.text = "READY";
				AbilityReadyBackground.enabled = true;
				CurrentAbilityState = abilityState.Ready;
			}

			AbilityChargingColor = new Color (1 - (0.765f * AbilityTimeAmountProportion), 1, 1 - 0.376f * AbilityTimeAmountProportion, 1);

			if (AbilityTimeAmountProportion < 1f)
			{
				AbilityFillImageL.color = AbilityChargingColor;
				AbilityFillImageR.color = AbilityChargingColor;
			}

			if (AbilityTimeAmountProportion == 1)
			{
				AbilityFillImageL.color = AbilityChargingFullColor;
				AbilityFillImageR.color = AbilityChargingFullColor;
			}
		}

		lensScript.radius = Mathf.Lerp (lensScript.radius, TargetLensRadius, LensRadiusSmoothTime * Time.unscaledDeltaTime);
		Shield.transform.localScale = new Vector3 (
			Mathf.Lerp (Shield.transform.localScale.x, TargetShieldScale, ShieldScaleSmoothTime * Time.unscaledDeltaTime), 
			Mathf.Lerp (Shield.transform.localScale.y, TargetShieldScale, ShieldScaleSmoothTime * Time.unscaledDeltaTime), 
			Mathf.Lerp (Shield.transform.localScale.z, TargetShieldScale, ShieldScaleSmoothTime * Time.unscaledDeltaTime)
		);
	}

	void ActivateAbility ()
	{
		switch (Ability) 
		{
		case ability.Shield:
			isShieldOn = true;
			playerCol.enabled = false;
			Shield.SetActive (true);
			TargetLensRadius = LensOnRadius;
			TargetShieldScale = 1;
			break;
		case ability.Emp:
			break;
		}
	}

	void DeactivateAbility ()
	{
		switch (Ability) 
		{
		case ability.Shield:
			isShieldOn = false;
			TargetLensRadius = 0;
			TargetShieldScale = 0;
			Invoke ("DeactivateShield", 1);
			break;
		case ability.Emp:
			break;
		}
	}

	void DeactivateShield ()
	{
		if (developerModeScript.isGod == false) 
		{
			playerCol.enabled = true;
		}
		Shield.SetActive (false);
	}

	public void RefreshAbilityName ()
	{
		switch (Ability) 
		{
		case ability.Shield:
			AbilityName = "Shield";
			break;
		case ability.Emp:
			AbilityName = "Emp";
			break;
		}
	}

	IEnumerator CheckBulletType ()
	{
		while (canShoot == true) 
		{
			switch (ShotType) 
			{
			case shotType.Standard:
				CurrentFireRate = StandardFireRate;
				break;
			case shotType.Double:
				
				break;
			}

			yield return null;
		}
	}

	// Checks shooting state.
	void CheckShoot ()
	{
		if (canShoot == true) 
		{
			if (playerActions.Shoot.Value > 0.75f && Time.time > NextFire && gameControllerScript.isPaused == false) 
			{
				// Every time the player shoots, decremement the combo.
				if (gameControllerScript.combo > 1)
				{
					gameControllerScript.combo -= 1;
				}

				Shoot ();

				NextFire = Time.time + CurrentFireRate / (FireRateTimeMultiplier * Time.timeScale);
			}
		}
	}

	void Shoot ()
	{
		switch (ShotType) 
		{
		case shotType.Standard:
			GameObject shot = Instantiate (StandardShot, StandardShotSpawn.position, StandardShotSpawn.rotation);
			shot.GetComponent<Bullet> ().playerControllerScript = this;
			shot.GetComponent<Bullet> ().playerPos = playerCol.transform;
			shot.name = "Standard Shot_P" + PlayerId + "";
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
					break;
			}

			break;
		}
	}

	IEnumerator CheckPause ()
	{
		while (gameControllerScript.canPause == true) 
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

			yield return null;
		}
	}

	void CheckUIVisibility ()
	{
		// SCORE TEXT
		// When the player is close to the score text.
		// Vertical position.
		if (PlayerRb.position.y > ScoreCheckPlayerPos.y) 
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
			if (PlayerRb.position.x <= -ScoreCheckPlayerPos.x || PlayerRb.position.x >= ScoreCheckPlayerPos.x) 
			{
				if (ScoreAnim.GetCurrentAnimatorStateInfo (0).IsName ("ScoreFadeIn") == false && isHidingScoreUI == true) 
				{
					ScoreAnim.Play ("ScoreFadeIn");
					isHidingScoreUI = false;
				}
			}
		}

		// Vertical position too far from score text.
		if (PlayerRb.position.y <= ScoreCheckPlayerPos.y) 
		{
			if (ScoreAnim.GetCurrentAnimatorStateInfo (0).IsName ("ScoreFadeIn") == false && isHidingScoreUI == true) 
			{
				ScoreAnim.Play ("ScoreFadeIn");
				isHidingScoreUI = false;
			}
		}

		// LIVES TEXT
		// When the player is close to the lives text.
		// Vertical position.
		if (PlayerRb.position.y > LivesCheckPlayerPosY.y) 
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
		if (PlayerRb.position.y <= LivesCheckPlayerPosY.y) 
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
	}

	public void ResetPowerups ()
	{
		ShotType = shotType.Standard;
		CurrentFireRate = StandardFireRate;
		DoubleShotIteration = shotIteration.Standard;
		TripleShotIteration = shotIteration.Standard;
		NextDoubleShotIteration = 0;
		NextTripleShotIteration = 0;

		gameControllerScript.PowerupShootingImage_P1.texture = null; // Replace with standard shot texture.
		gameControllerScript.PowerupShootingImage_P1.color = new Color (0, 0, 0, 0); // Replace with full color once standard shot texture is in.
		gameControllerScript.PowerupShootingText_P1.text = " ";

		gameControllerScript.PowerupOneImage_P1.texture = null;
		gameControllerScript.PowerupOneImage_P1.color = new Color (0, 0, 0, 0);
		gameControllerScript.PowerupOneText_P1.text = " ";

		gameControllerScript.PowerupTwoImage_P1.texture = null;
		gameControllerScript.PowerupTwoImage_P1.color = new Color (0, 0, 0, 0);
		gameControllerScript.PowerupTwoText_P1.text = " ";

		gameControllerScript.PowerupThreeImage_P1.texture = null;
		gameControllerScript.PowerupThreeImage_P1.color = new Color (0, 0, 0, 0);
		gameControllerScript.PowerupThreeText_P1.text = " ";
	}

	// This is for InControl for initialization.
	void CreatePlayerActions ()
	{
		playerActions = new PlayerActions ();
	}

	// This is for InControl to be able to read input.
	void AssignActionControls ()
	{
		playerActions.MoveLeft.AddDefaultBinding (Key.A);
		playerActions.MoveLeft.AddDefaultBinding (Key.LeftArrow);
		playerActions.MoveLeft.AddDefaultBinding (InputControlType.LeftStickLeft);
		playerActions.MoveLeft.AddDefaultBinding (InputControlType.DPadLeft);

		playerActions.MoveRight.AddDefaultBinding (Key.D);
		playerActions.MoveRight.AddDefaultBinding (Key.RightArrow);
		playerActions.MoveRight.AddDefaultBinding (InputControlType.LeftStickRight);
		playerActions.MoveRight.AddDefaultBinding (InputControlType.DPadRight);

		playerActions.MoveUp.AddDefaultBinding (Key.W);
		playerActions.MoveUp.AddDefaultBinding (Key.UpArrow);
		playerActions.MoveUp.AddDefaultBinding (InputControlType.LeftStickUp);
		playerActions.MoveUp.AddDefaultBinding (InputControlType.DPadUp);

		playerActions.MoveDown.AddDefaultBinding (Key.S);
		playerActions.MoveDown.AddDefaultBinding (Key.DownArrow);
		playerActions.MoveDown.AddDefaultBinding (InputControlType.LeftStickDown);
		playerActions.MoveDown.AddDefaultBinding (InputControlType.DPadDown);

		playerActions.Shoot.AddDefaultBinding (Key.Space);
		playerActions.Shoot.AddDefaultBinding (Key.LeftControl);
		playerActions.Shoot.AddDefaultBinding (Mouse.LeftButton);
		playerActions.Shoot.AddDefaultBinding (InputControlType.RightTrigger);
		playerActions.Shoot.AddDefaultBinding (InputControlType.Action1);

		//playerActions.Ability.AddDefaultBinding (Key.LeftShift);
		playerActions.Ability.AddDefaultBinding (Key.LeftAlt);
		playerActions.Ability.AddDefaultBinding (Mouse.RightButton);
		playerActions.Ability.AddDefaultBinding (InputControlType.LeftTrigger);

		playerActions.Pause.AddDefaultBinding (Key.Escape);
		playerActions.Pause.AddDefaultBinding (InputControlType.Command);

		playerActions.DebugMenu.AddDefaultBinding (Key.Tab);
		playerActions.DebugMenu.AddDefaultBinding (InputControlType.LeftBumper);

		playerActions.CheatConsole.AddDefaultBinding (Key.Backquote);
	}

	// Creates vibration.
	public void Vibrate (float LeftMotor, float RightMotor, float duration)
	{
		PlayerVibrationDuration = duration;
		PlayerVibrationTimeRemaining = PlayerVibrationDuration;
		GamePad.SetVibration (PlayerIndex.One, LeftMotor, RightMotor);
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
		GamePad.SetVibration (PlayerIndex.One, 0, 0);
	}

	void CheckJoinState ()
	{
		if (this.enabled == true) 
		{
			isJoined = true;
		}
	}

	void CheckCheatConsoleInput ()
	{
		/*
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
		}*/
	}

	void OnDisable ()
	{
		if (isJoined == true) 
		{
			isJoined = false;
			PlayerText.text = " ";
		}
	}
}