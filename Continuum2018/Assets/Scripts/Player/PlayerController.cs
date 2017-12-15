using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using XInputDotNetPure;

public class PlayerController : MonoBehaviour 
{
	public GameController gameControllerScript;
	public TimescaleController timescaleControllerScript;
	public AudioController audioControllerScript;
	public CursorManager cursorManagerScript;
	public CameraShake camShakeScript;

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
	public GameObject CurrentShot;
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

	[Header ("Powerups")]

	[Header ("Shield")]
	public bool isShieldOn;
	public Lens lensScript;

	[Header ("UI")]
	public bool isHidingScoreUI;
	public Animator ScoreAnim;
	public Vector3 ScoreCheckPlayerPos;

	public bool isHidingLivesUI;
	public Animator LivesAnim;
	public Vector3 LivesCheckPlayerPos;

	public PlayerActions playerActions;

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

	IEnumerator CheckBulletType ()
	{
		while (canShoot == true) 
		{
			switch (ShotType) 
			{
			case shotType.Standard:
				CurrentShot = StandardShot;
				CurrentFireRate = StandardFireRate;
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
			if (CurrentShot == null) 
			{
				CurrentShot = StandardShot;
			}

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
		if (CurrentShot == StandardShot)
		{
			Instantiate (StandardShot, StandardShotSpawn.position, StandardShotSpawn.rotation);
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

		playerActions.Ability.AddDefaultBinding (Key.LeftShift);
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