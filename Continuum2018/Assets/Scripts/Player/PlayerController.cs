using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class PlayerController : MonoBehaviour 
{
	public PlayerActions playerActions;

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

	void Start () 
	{
		CreatePlayerActions ();
		AssignActionControls ();
	}

	void Update () 
	{
		MovePlayer ();
	}

	void MovePlayer ()
	{
		// Reads movement input on two axis.
		MovementX = playerActions.Move.Value.x;
		MovementY = playerActions.Move.Value.y;

		if (UsePlayerFollow == true) 
		{
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

		playerActions.MoveRight.AddDefaultBinding (Key.D);
		playerActions.MoveRight.AddDefaultBinding (Key.RightArrow);

		playerActions.MoveUp.AddDefaultBinding (Key.W);
		playerActions.MoveUp.AddDefaultBinding (Key.UpArrow);

		playerActions.MoveDown.AddDefaultBinding (Key.S);
		playerActions.MoveDown.AddDefaultBinding (Key.DownArrow);

		playerActions.Shoot.AddDefaultBinding (Key.Space);
		playerActions.Shoot.AddDefaultBinding (Mouse.LeftButton);
		playerActions.Shoot.AddDefaultBinding (InputControlType.RightTrigger);

		playerActions.Ability.AddDefaultBinding (Key.LeftShift);
		playerActions.Ability.AddDefaultBinding (Mouse.RightButton);
		playerActions.Ability.AddDefaultBinding (InputControlType.LeftTrigger);

		playerActions.Pause.AddDefaultBinding (Key.Escape);
		playerActions.Pause.AddDefaultBinding (InputControlType.Command);
	}
}