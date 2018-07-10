using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using InControl;

public class MenuManager : MonoBehaviour 
{
	[Tooltip ("UI to check for activation.")]
	public GameObject UI;

	[Header ("Scroll attributes")]
	[Tooltip ("How much time needs to pass before the player can press up or down.")]
	public float scrollSpeed = 0.5f;
	private float nextScroll;

	[Tooltip ("How much time needs to pass before the player can press A.")]
	public float aButtonCoolDown = 0.5f;
	private float aButtonNextCooldown;

	[Tooltip ("How much time needs to pass before the player can press B.")]
	public float bButtonCoolDown = 0.5f;
	private float bButtonNextCooldown;

	[Tooltip ("How much time needs to pass before the player can press Start.")]
	public float startButtonCooldown = 0.5f;
	private float startButtonNextCooldown;

	// Input data.
	public PointerEventData pointerEventData;
	//public static PlayerActions menuActions;

	[Header ("Menu Buttons")]
	public MenuButtons menuButtons;

	void Awake ()
	{
		nextScroll = Time.time + scrollSpeed;

		/*
		// Get input data.
		if (menuActions == null) 
		{
			menuActions = new PlayerActions ();
			AssignActionControls ();
		}
		*/

		pointerEventData = new PointerEventData (EventSystem.current);

		if (menuButtons.BackButton != null) 
		{
			menuButtons.BackButton.enabled = false;
		}
	}

	void OnEnable ()
	{
		if (menuButtons.BackButton != null) 
		{
			StartCoroutine (EnableBackButton ());
		}
	}

	IEnumerator EnableBackButton ()
	{
		yield return new WaitForSecondsRealtime (0.25f);
		menuButtons.BackButton.enabled = true;
	}

	void OnDisable ()
	{
		if (menuButtons.BackButton != null) 
		{
			menuButtons.BackButton.enabled = false;
		}
	}

	void Update () 
	{
		CheckForInput ();
	}

	void CheckForInput ()
	{
		// Only perform tasks if this UI is active.
		if (UI.activeInHierarchy == true) 
		{
			if (menuButtons.useStartButton == true) 
			{
				if (InputManager.Devices.Count > 0) 
				{
					if (InputManager.Devices [0].Command.WasPressed) 
					{
						if (Time.unscaledTime > startButtonNextCooldown) 
						{
							startButtonNextCooldown = Time.unscaledTime + startButtonCooldown;
							menuButtons.buttonIndex = 0;

							if (menuButtons.ignoreAutoClick == false) 
							{
								MenuOnClick ();
							}
						}
					}
				} 

				if (InputManager.Devices.Count > 0 == false) 
				{
					if (Input.GetKeyDown (KeyCode.Escape)) 
					{
						if (Time.unscaledTime > startButtonNextCooldown) 
						{
							startButtonNextCooldown = Time.unscaledTime + startButtonCooldown;
							menuButtons.buttonIndex = 0;

							if (menuButtons.ignoreAutoClick == false) 
							{
								MenuOnClick ();
							}
						}
					}
				}
			}

			if (InputManager.Devices.Count > 0)
			{
				// Player presses up on the left stick or D-Pad up.
				if (InputManager.Devices [0].LeftStickUp.Value > 0.75f || InputManager.Devices [0].DPadUp.WasPressed) 
				{
					if (Time.unscaledTime > nextScroll) 
					{
						// Scrolling down, before reaching the end.
						if (menuButtons.buttonIndex >= 1) 
						{
							menuButtons.buttonIndex -= 1; // Decrement button index.
							menuButtons.buttonIndex = Mathf.Clamp (
								menuButtons.buttonIndex, 
								0, 
								menuButtons.maxButtons
							);

							int HighlightUpVal = menuButtons.buttonIndex;
							int UnHighlightUpVal = Mathf.Clamp (
								                       menuButtons.buttonIndex + 1, 
								                       0, 
								                       menuButtons.maxButtons
							                       );

							MenuOnEnter (HighlightUpVal);
							MenuOnExit (UnHighlightUpVal);
						}

						// Reset scroll speed.
						nextScroll = Time.unscaledTime + scrollSpeed;
					}
				} 
			}

			if (InputManager.Devices.Count > 0 == false)
			{
				if (Input.GetKeyDown (KeyCode.UpArrow)) 
				{
					if (Time.unscaledTime > nextScroll) 
					{
						// Scrolling down, before reaching the end.
						if (menuButtons.buttonIndex >= 1) 
						{
							menuButtons.buttonIndex -= 1; // Decrement button index.
							menuButtons.buttonIndex = Mathf.Clamp (
								menuButtons.buttonIndex, 
								0, 
								menuButtons.maxButtons
							);

							int HighlightUpVal = menuButtons.buttonIndex;
							int UnHighlightUpVal = Mathf.Clamp (
								menuButtons.buttonIndex + 1, 
								0, 
								menuButtons.maxButtons
							);

							MenuOnEnter (HighlightUpVal);
							MenuOnExit (UnHighlightUpVal);
						}

						// Reset scroll speed.
						nextScroll = Time.unscaledTime + scrollSpeed;
					}
				}
			}

			if (InputManager.Devices.Count > 0) 
			{
				// Player presses down on the left stick or D-Pad down.
				if (InputManager.Devices [0].LeftStickDown.Value > 0.75f || InputManager.Devices [0].DPadDown.WasPressed) 
				{
					if (Time.unscaledTime > nextScroll) 
					{
						// Scrolling down, before reaching the end.
						if (menuButtons.buttonIndex < menuButtons.maxButtons) 
						{
							menuButtons.buttonIndex += 1; // Increment button index.
							menuButtons.buttonIndex = Mathf.Clamp (
								menuButtons.buttonIndex, 
								0, 
								menuButtons.maxButtons
							);

							int HighlightDownVal = menuButtons.buttonIndex;
							int UnHighlightDownVal = Mathf.Clamp (
								                        menuButtons.buttonIndex - 1, 
								                        0, 
								                        menuButtons.maxButtons
							                        );

							MenuOnEnter (HighlightDownVal);
							MenuOnExit (UnHighlightDownVal);
						}

						// Reset scroll speed.
						nextScroll = Time.unscaledTime + scrollSpeed;
					}
				}
			}

			if (InputManager.Devices.Count > 0 == false) 
			{
				// Player presses down on the left stick or D-Pad down.
				if (Input.GetKeyDown (KeyCode.DownArrow)) 
				{
					if (Time.unscaledTime > nextScroll) 
					{
						// Scrolling down, before reaching the end.
						if (menuButtons.buttonIndex < menuButtons.maxButtons) 
						{
							menuButtons.buttonIndex += 1; // Increment button index.
							menuButtons.buttonIndex = Mathf.Clamp (
								menuButtons.buttonIndex, 
								0, 
								menuButtons.maxButtons
							);

							int HighlightDownVal = menuButtons.buttonIndex;
							int UnHighlightDownVal = Mathf.Clamp (
								menuButtons.buttonIndex - 1, 
								0, 
								menuButtons.maxButtons
							);

							MenuOnEnter (HighlightDownVal);
							MenuOnExit (UnHighlightDownVal);
						}

						// Reset scroll speed.
						nextScroll = Time.unscaledTime + scrollSpeed;
					}
				}
			}

			if (InputManager.Devices.Count > 0) 
			{
				// Player presses the A button.
				if (InputManager.Devices [0].Action1.WasPressed) 
				{
					if (Time.unscaledTime > aButtonNextCooldown) 
					{
						MenuOnClick ();

						// Reset A button cooldown.
						aButtonNextCooldown = Time.unscaledTime + aButtonCoolDown;
					}
				}
			}

			if (InputManager.Devices.Count > 0 == false) 
			{
				// Player presses the A button.
				if (Input.GetKeyDown (KeyCode.Return)) 
				{
					if (Time.unscaledTime > aButtonNextCooldown) 
					{
						MenuOnClick ();

						// Reset A button cooldown.
						aButtonNextCooldown = Time.unscaledTime + aButtonCoolDown;
					}
				}
			}

			if (InputManager.Devices.Count > 0) 
			{
				// Player presses the B button.
				if (InputManager.Devices [0].Action2.WasPressed) 
				{
					if (Time.unscaledTime > bButtonNextCooldown && menuButtons.ignoreAutoClick == false)
					{
						// Set button to first option and execute the command.
						menuButtons.buttonIndex = 0;

						// Override button event for navigating back.
						if (menuButtons.BackButton != null) 
						{
							Button OnClickEvent = menuButtons.BackButton.GetComponent<Button> ();

							if (OnClickEvent != null) 
							{
								ExecuteEvents.Execute (
									menuButtons.BackButton.gameObject, 
									pointerEventData, 
									ExecuteEvents.pointerClickHandler
								);
							}
						}

						// Reset B button cooldown.
						bButtonNextCooldown = Time.unscaledTime + bButtonCoolDown;
					}
				}
			}

			if (InputManager.Devices.Count > 0 == false) 
			{
				// Player presses the B button.
				if (Input.GetKeyDown (KeyCode.Escape)) 
				{
					if (Time.unscaledTime > bButtonNextCooldown && menuButtons.ignoreAutoClick == false)
					{
						// Set button to first option and execute the command.
						menuButtons.buttonIndex = 0;

						// Override button event for navigating back.
						if (menuButtons.BackButton != null) 
						{
							Button OnClickEvent = menuButtons.BackButton.GetComponent<Button> ();

							if (OnClickEvent != null) 
							{
								ExecuteEvents.Execute (
									menuButtons.BackButton.gameObject, 
									pointerEventData, 
									ExecuteEvents.pointerClickHandler
								);
							}
						}

						// Reset B button cooldown.
						bButtonNextCooldown = Time.unscaledTime + bButtonCoolDown;
					}
				}
			}
		} 
	}

	// Invoke an OnClick event.
	public void MenuOnClick ()
	{
		// Check if there is an Button component present.
		Button OnClickEvent = menuButtons.menuButtons [menuButtons.buttonIndex].GetComponent<Button> ();

		if (OnClickEvent != null) 
		{
			ExecuteEvents.Execute (
				menuButtons.menuButtons [menuButtons.buttonIndex].gameObject, 
				pointerEventData, 
				ExecuteEvents.pointerClickHandler
			);
		}
	}

	// Invoke an OnPointerEnter event.
	public void MenuOnEnter (int index)
	{
		// Check if there is an EventTrigger component present.
		EventTrigger OnEnterEvent = menuButtons.menuEvents [index].GetComponent<EventTrigger> ();

		if (OnEnterEvent != null) 
		{
			ExecuteEvents.Execute (
				menuButtons.menuEvents [index].gameObject, 
				pointerEventData, 
				ExecuteEvents.pointerEnterHandler
			);
		}
	}

	public void NextMenuDefault ()
	{
		MenuOnEnter (0);
		SetButtonIndex (0);
	}

	public void MenuOnExit (int index)
	{
		// Check if there is an EventTrigger component present.
		EventTrigger OnExitEvent = menuButtons.menuEvents [index].GetComponent<EventTrigger> ();

		if (OnExitEvent != null)
		{
			ExecuteEvents.Execute (
				menuButtons.menuEvents [index].gameObject, 
				pointerEventData, 
				ExecuteEvents.pointerExitHandler
			);
		}
	}

	public void SetButtonIndex (int buttonIndex)
	{
		menuButtons.buttonIndex = buttonIndex;
		MenuOnEnter (buttonIndex);
	}

	public void RefreshButtonIndex ()
	{
		MenuOnEnter (menuButtons.buttonIndex);
	}

	/*
	void AssignActionControls ()
	{
		// LEFT
		menuActions.MoveLeft.AddDefaultBinding (Key.A);
		menuActions.MoveLeft.AddDefaultBinding (Key.LeftArrow);
		menuActions.MoveLeft.AddDefaultBinding (InputControlType.LeftStickLeft);
		menuActions.MoveLeft.AddDefaultBinding (InputControlType.DPadLeft);

		// RIGHT
		menuActions.MoveRight.AddDefaultBinding (Key.D);
		menuActions.MoveRight.AddDefaultBinding (Key.RightArrow);
		menuActions.MoveRight.AddDefaultBinding (InputControlType.LeftStickRight);
		menuActions.MoveRight.AddDefaultBinding (InputControlType.DPadRight);

		// UP
		menuActions.MoveUp.AddDefaultBinding (Key.W);
		menuActions.MoveUp.AddDefaultBinding (Key.UpArrow);
		menuActions.MoveUp.AddDefaultBinding (InputControlType.LeftStickUp);
		menuActions.MoveUp.AddDefaultBinding (InputControlType.DPadUp);

		// DOWN
		menuActions.MoveDown.AddDefaultBinding (Key.S);
		menuActions.MoveDown.AddDefaultBinding (Key.DownArrow);
		menuActions.MoveDown.AddDefaultBinding (InputControlType.LeftStickDown);
		menuActions.MoveDown.AddDefaultBinding (InputControlType.DPadDown);

		// A
		menuActions.Shoot.AddDefaultBinding (Key.Space);
		menuActions.Shoot.AddDefaultBinding (Key.Return);
		menuActions.Shoot.AddDefaultBinding (InputControlType.Action1);

		// B
		menuActions.Ability.AddDefaultBinding (Key.Escape);
		menuActions.Ability.AddDefaultBinding (InputControlType.Action2);

		// Start
		menuActions.Pause.AddDefaultBinding (Key.Return);
		menuActions.Pause.AddDefaultBinding (InputControlType.Command);
	}
	*/
		
	[System.Serializable]
	public class MenuButtons
	{
		[Tooltip ("Current index for the menu button focus.")]
		public int buttonIndex = 0;
		[Tooltip ("1 less the size of the amount of buttons or event triggers.")]
		public int maxButtons = 3;

		[Tooltip ("All the Buttons in order.")]
		public Button[] menuButtons;
		[Tooltip ("All the EventTriggers in order.")]
		public EventTrigger[] menuEvents;

		[Tooltip ("The button OnClick () event that gets pressed when B is pressed.")]
		public Button BackButton;

		public bool useStartButton;
		public bool ignoreAutoClick = true;
	}
}