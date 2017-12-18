using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using XInputDotNetPure;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour 
{
	public GameController gameControllerScript;
	public TimescaleController timescaleControllerScript;
	public AudioController audioControllerScript;
	public CursorManager cursorManagerScript;
	public CameraShake camShakeScript;
	public int PlayerId = 1;

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

	[Header ("Shooting")]
	public bool canShoot = true;
	public float CurrentFireRate = 0.1f;
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
	public GameObject DoubleShotL;
	public GameObject DoubleShotR;
	public Transform DoubleShotSpawnL;
	public Transform DoubleShotSpawnR;
	public float DoubleShotFireRate = 0.25f;
	private float DoubleShotNextFire;

	[Header ("Shield")]
	public bool isShieldOn;
	public GameObject Shield;
	public Lens lensScript;
	public float TargetShieldScale;
	public float ShieldScaleSmoothTime = 1;
	public float LensOnRadius = 0.7f;
	public float TargetLensRadius;
	public float LensRadiusSmoothTime = 1;

	[Header ("UI")]
	public bool isHidingScoreUI;
	public Animator ScoreAnim;
	public Vector3 ScoreCheckPlayerPos;

	public bool isHidingLivesUI;
	public Animator LivesAnim;
	public Vector3 LivesCheckPlayerPos;

	public bool isHidingWaveUI;
	public Animator WaveAnim;
	public Vector3 WaveCheckPlayerPos;

	public PlayerActions playerActions;

	void Awake ()
	{
		Application.targetFrameRate = 60;
		AbilityReadyText.text = "";
		RefreshAbilityName ();
	}

	void Start () 
	{
		CreatePlayerActions ();
		AssignActionControls ();
	}

	public void StartCoroutines ()
	{
		StartCoroutine (CheckPause ());
		StartCoroutine (CheckBulletType ());
	}

	void Update ()
	{
		MovePlayer ();
		CheckShoot ();
		CheckPlayerVibration ();
		CheckUIVisibility ();
		CheckCooldownTime ();
		CheckAbilityTime ();
	}

	void CheckCooldownTime ()
	{
		if (isInCooldownMode == true)
		{
			if (cooldownTimeRemaining > 0) 
			{
				UsePlayerFollow = false;
				cooldownTimeRemaining -= Time.unscaledDeltaTime;
			}

			if (cooldownTimeRemaining <= 0) 
			{
				RejoinGame ();
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
		isInCooldownMode = true;
	}

	void EnableCollider ()
	{
		playerCol.enabled = true;
	}

	void RejoinGame ()
	{
		canShoot = true;
		UsePlayerFollow = true;
		playerMesh.SetActive (true);
	}

	public void EnablePlayerInput ()
	{
		UsePlayerFollow = true;
		canShoot = true;
		StartCoroutines ();
	}

	void MovePlayer ()
	{
		if (UsePlayerFollow == true) 
		{
			if (gameControllerScript.isPaused == false && cooldownTimeRemaining <= 0) 
			{
				// Reads movement input on two axis.
				MovementX = playerActions.Move.Value.x;
				MovementY = playerActions.Move.Value.y;

				// This moves the transform position which the player will follow.
				PlayerFollowRb.velocity = new Vector3 
				(
					MovementX * PlayerFollowMoveSpeed * Time.unscaledDeltaTime * (1 / (Time.timeScale + 0.1f)),
					MovementY * PlayerFollowMoveSpeed * Time.unscaledDeltaTime * (1 / (Time.timeScale + 0.1f)),
					0
				);
						
				// Player follows [follow position] with smoothing.
				PlayerRb.position = new Vector3 (
					Mathf.SmoothDamp (PlayerRb.position.x, PlayerFollow.position.x, ref SmoothFollowVelX, SmoothFollowTime * Time.unscaledDeltaTime),
					Mathf.SmoothDamp (PlayerRb.position.y, PlayerFollow.position.y, ref SmoothFollowVelY, SmoothFollowTime * Time.unscaledDeltaTime),
					0
				);
			}
		}

		// Rotates on y-axis by x velocity.
		PlayerRb.rotation = Quaternion.Euler
			(
				0, 
				Mathf.Clamp (Mathf.SmoothDamp (PlayerRb.rotation.y, -MovementX, ref RotVelY, 10 * Time.deltaTime) * YRotationMultiplier, -YRotationAmount, YRotationAmount), 
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

	void CheckAbilityTime ()
	{
		// Updates the ability UI involved.
		AbilityTimeAmountProportion = CurrentAbilityTimeRemaining / CurrentAbilityDuration;
		AbilityFillImageL.fillAmount = 0.16f * AbilityTimeAmountProportion; // Fills to a sixth.
		AbilityFillImageR.fillAmount = 0.16f * AbilityTimeAmountProportion; // Fills to a sixth.

		// Player presses ability button.
		if (playerActions.Ability.WasPressed) 
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
				CurrentAbilityTimeRemaining -= AbilityUseSpeedMultiplier * Time.unscaledDeltaTime;
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
				timescaleControllerScript.isInInitialSequence == false && timescaleControllerScript.isInInitialCountdownSequence == false) 
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

		lensScript.radius = Mathf.Lerp (lensScript.radius, TargetLensRadius, LensRadiusSmoothTime * Time.deltaTime);
		Shield.transform.localScale = new Vector3 (
			Mathf.Lerp(Shield.transform.localScale.x, TargetShieldScale, ShieldScaleSmoothTime * Time.deltaTime), 
			Mathf.Lerp(Shield.transform.localScale.y, TargetShieldScale, ShieldScaleSmoothTime * Time.deltaTime), 
			Mathf.Lerp(Shield.transform.localScale.z, TargetShieldScale, ShieldScaleSmoothTime * Time.deltaTime)
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
			Invoke ("DeactivateShield", 3);
			break;
		case ability.Emp:
			break;
		}
	}

	void DeactivateShield ()
	{
		playerCol.enabled = true;
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
				CurrentFireRate = DoubleShotFireRate;
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
			if (playerActions.Shoot.Value > 0.75f && Time.unscaledTime > NextFire && gameControllerScript.isPaused == false) 
			{
				// Every time the player shoots, decremement the combo.
				if (gameControllerScript.combo > 1)
				{
					gameControllerScript.combo -= 1;
				}

				Shoot ();

				NextFire = Time.unscaledTime + CurrentFireRate / (2 * Time.timeScale);
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
			GameObject doubleshotL = Instantiate (DoubleShotL, DoubleShotSpawnL.position, DoubleShotSpawnL.rotation);
			GameObject doubleshotR = Instantiate (DoubleShotR, DoubleShotSpawnR.position, DoubleShotSpawnR.rotation);
			doubleshotL.GetComponent<Bullet> ().playerControllerScript = this;
			doubleshotL.GetComponent<Bullet> ().playerPos = playerCol.transform;
			doubleshotR.GetComponent<Bullet> ().playerControllerScript = this;
			doubleshotR.GetComponent<Bullet> ().playerPos = playerCol.transform;
			doubleshotL.name = "Double ShotL_P" + PlayerId + "";
			doubleshotR.name = "Double ShotR_P" + PlayerId + "";
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
		if (PlayerRb.position.y > LivesCheckPlayerPos.y) 
		{
			// Horizontal position too far.
			if (PlayerRb.position.x < LivesCheckPlayerPos.x) 
			{
				if (LivesAnim.GetCurrentAnimatorStateInfo (0).IsName ("LivesFadeOut") == false && isHidingLivesUI == false) 
				{
					LivesAnim.Play ("LivesFadeOut");
					isHidingLivesUI = true;
				}
			}

			// Horizontal position in range.
			if (PlayerRb.position.x >= LivesCheckPlayerPos.x) 
			{
				if (LivesAnim.GetCurrentAnimatorStateInfo (0).IsName ("LivesFadeIn") == false && isHidingLivesUI == true) 
				{
					LivesAnim.Play ("LivesFadeIn");
					isHidingLivesUI = false;
				}
			}
		}

		// Vertical position too far from lives text.
		if (PlayerRb.position.y <= LivesCheckPlayerPos.y) 
		{
			if (LivesAnim.GetCurrentAnimatorStateInfo (0).IsName ("LivesFadeIn") == false && isHidingLivesUI == true) 
			{
				LivesAnim.Play ("LivesFadeIn");
				isHidingLivesUI = false;
			}
		}

		// WAVE TEXT
		// When the player is close to the wave text.
		// Vertical position.
		if (PlayerRb.position.y > WaveCheckPlayerPos.y) 
		{
			// Horizontal position too far.
			if (PlayerRb.position.x > WaveCheckPlayerPos.x) 
			{
				if (WaveAnim.GetCurrentAnimatorStateInfo (0).IsName ("WaveUIExit") == false && isHidingWaveUI == false) 
				{
					WaveAnim.Play ("WaveUIExit");
					isHidingWaveUI = true;
				}
			}

			// Horizontal position in range.
			if (PlayerRb.position.x <= WaveCheckPlayerPos.x) 
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
		}
	}

	public void ResetPowerups ()
	{
		ShotType = shotType.Standard;
		CurrentFireRate = StandardFireRate;
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

		playerActions.MoveRight.AddDefaultBinding (Key.D);
		playerActions.MoveRight.AddDefaultBinding (Key.RightArrow);
		playerActions.MoveRight.AddDefaultBinding (InputControlType.LeftStickRight);

		playerActions.MoveUp.AddDefaultBinding (Key.W);
		playerActions.MoveUp.AddDefaultBinding (Key.UpArrow);
		playerActions.MoveUp.AddDefaultBinding (InputControlType.LeftStickUp);

		playerActions.MoveDown.AddDefaultBinding (Key.S);
		playerActions.MoveDown.AddDefaultBinding (Key.DownArrow);
		playerActions.MoveDown.AddDefaultBinding (InputControlType.LeftStickDown);

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
		playerActions.DebugMenu.AddDefaultBinding (InputControlType.DPadUp);
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
}