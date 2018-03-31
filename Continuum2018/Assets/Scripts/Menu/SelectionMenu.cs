using UnityEngine;
using InControl;

public class SelectionMenu : MonoBehaviour 
{
	public Transform Hand;
	public float Speed = 100.0f;
	public float xBoundary, yBoundary;
	public bool handFocussed;

	public PlayerActions playerActions;

	void OnEnable ()
	{
		handFocussed = true;
	}

	void OnDisable ()
	{
		handFocussed = false;
	}

	void Start ()
	{
		CreatePlayerActions ();
	}

	void Update ()
	{
		GetMovementInput ();
	}

	void GetMovementInput ()
	{
		if (handFocussed == true)
		{
			Hand.transform.Translate (
				new Vector3 (
					playerActions.Move.Value.x * Speed * Time.unscaledDeltaTime, 
					playerActions.Move.Value.y * Speed * Time.unscaledDeltaTime, 
					0
				), Space.Self
			);

			Hand.transform.localPosition = new Vector3 (
				Mathf.Clamp (Hand.localPosition.x, -xBoundary, xBoundary),
				Mathf.Clamp (Hand.localPosition.y, -yBoundary, yBoundary),
				Hand.localPosition.z
			);

			//Debug.Log (playerActions.Move.Value);

			// If using mouse.
			//Vector3 mouselocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			//Hand.localPosition = mouselocation;
		}
	}

	public void SetFocus (bool focus)
	{
		handFocussed = focus;
	}

	void CreatePlayerActions ()
	{
		playerActions = new PlayerActions ();

		playerActions.MoveLeft.AddDefaultBinding (Key.LeftArrow);
		playerActions.MoveRight.AddDefaultBinding (Key.RightArrow);
		playerActions.MoveUp.AddDefaultBinding (Key.UpArrow);
		playerActions.MoveDown.AddDefaultBinding (Key.DownArrow);

		playerActions.Shoot.AddDefaultBinding (Key.Return);
		playerActions.Back.AddDefaultBinding (Key.Escape);
	}
}