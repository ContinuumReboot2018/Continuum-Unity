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

	[Header ("Powerups")]

	[Header ("Shield")]
	public bool isShieldOn;
	public Lens lensScript;

	[Header ("UI")]
	public bool isHidingScoreUI;
	public Animator ScoreAnim;
	public Vector3 ScoreCheckPlayerPos;

	public PlayerActions playerActions;

	void Start () 
	{
		CreatePlayerActions ();
		AssignActionControls ();
	}

	public void StartCoroutines ()
	{
		StartCoroutine (MovePlayer ());
		StartCoroutine (CheckPause ());
		StartCoroutine (CheckBulletType ());
	}

	void Update ()
	{
		CheckShoot ();
		CheckPlayerVibration ();
		CheckUIVisibility ();
	}

	IEnumerator MovePlayer ()
	{
		UsePlayerFollow = true;

		while (UsePlayerFollow == true) 
		{
			if (gameControllerScript.isPaused == false) 
			{
				// Reads movement input on two axis.
				MovementX = playerActions.Move.Value.x;
				MovementY = playerActions.Move.Value.y;

				// This moves the transform position which the player will follow.
				PlayerFollowRb.velocity = new Vector3 (
					MovementX * PlayerFollowMoveSpeed * Time.unscaledDeltaTime * (1 / (Time.timeScale + 0.1f)),
					MovementY * PlayerFollowMoveSpeed * Time.unscaledDeltaTime * (1 / (Time.timeScale + 0.1f)),
					0
				);

				// Clamps the bounds of the player follow transform.
				PlayerFollow.position = new Vector3 (
					Mathf.Clamp (PlayerFollow.position.x, XBounds.x, XBounds.y),
					Mathf.Clamp (PlayerFollow.position.y, YBounds.x, YBounds.y),
					0
				);

				// Player follows [follow position] with smoothing.
				PlayerRb.position = new Vector3 (
					Mathf.SmoothDamp (PlayerRb.position.x, PlayerFollow.position.x, ref SmoothFollowVelX, SmoothFollowTime * Time.unscaledDeltaTime),
					Mathf.SmoothDamp (PlayerRb.position.y, PlayerFollow.position.y, ref SmoothFollowVelY, SmoothFollowTime * Time.unscaledDeltaTime),
					0
				);

				// Rotates on y-axis by x velocity.
				PlayerRb.rotation = Quaternion.Euler
				(
					0, 
					Mathf.Clamp (Mathf.SmoothDamp (PlayerRb.rotation.y, -MovementX, ref RotVelY, 10 * Time.deltaTime) * YRotationMultiplier, -YRotationAmount, YRotationAmount), 
					0
				);

				//Camera.main.transform.rotation = Quaternion.Euler (0, 0, PlayerFollow.position.x-PlayerRb.position.x);
			}

			yield return null;
		}
	}

	IEnumerator CheckBulletType ()
	{
		canShoot = true;
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
		gameControllerScript.canPause = true;
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
		// Vertical position.
		if (PlayerRb.position.y > ScoreCheckPlayerPos.y) 
		{
			// Horizontal position.
			if (PlayerRb.position.x > -ScoreCheckPlayerPos.x && PlayerRb.position.x < ScoreCheckPlayerPos.x) 
			{
				if (ScoreAnim.GetCurrentAnimatorStateInfo (0).IsName ("ScoreFadeOut") == false && isHidingScoreUI == false) 
				{
					ScoreAnim.Play ("ScoreFadeOut");
					isHidingScoreUI = true;
				}
			}

			// Horizontal position.
			if (PlayerRb.position.x <= -ScoreCheckPlayerPos.x || PlayerRb.position.x >= ScoreCheckPlayerPos.x) 
			{
				if (ScoreAnim.GetCurrentAnimatorStateInfo (0).IsName ("ScoreFadeIn") == false && isHidingScoreUI == true) 
				{
					ScoreAnim.Play ("ScoreFadeIn");
					isHidingScoreUI = false;
				}
			}
		}

		if (PlayerRb.position.y <= ScoreCheckPlayerPos.y) 
		{
			if (ScoreAnim.GetCurrentAnimatorStateInfo (0).IsName ("ScoreFadeIn") == false && isHidingScoreUI == true) 
			{
				ScoreAnim.Play ("ScoreFadeIn");
				isHidingScoreUI = false;
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

	public void Vibrate (float LeftMotor, float RightMotor, float duration)
	{
		PlayerVibrationDuration = duration;
		PlayerVibrationTimeRemaining = PlayerVibrationDuration;
		GamePad.SetVibration (PlayerIndex.One, LeftMotor, RightMotor);
	}

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

	void ResetPlayerVibration ()
	{
		GamePad.SetVibration (PlayerIndex.One, 0, 0);
	}
}