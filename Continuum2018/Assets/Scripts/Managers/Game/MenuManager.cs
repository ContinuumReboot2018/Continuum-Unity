using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
	public PlayerActions menuActions;

	void Start ()
	{
		// Get input data.
		menuActions = new PlayerActions ();
		pointerEventData = new PointerEventData (EventSystem.current);
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
			// Player presses up on the left stick or D-Pad up.
			if ((menuActions.ActiveDevice.LeftStickUp.Value > 0.75f ||
			    menuActions.ActiveDevice.DPadUp.IsPressed) ||
			    Input.GetKeyDown (KeyCode.UpArrow)) 
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

						MainPauseMenuOnEnter (HighlightUpVal);
						MainPauseMenuOnExit (UnHighlightUpVal);
					}

					// Reset scroll speed.
					nextScroll = Time.unscaledTime + scrollSpeed;
				}
			}

			// Player presses down on the left stick or D-Pad down.
			if ((menuActions.ActiveDevice.LeftStickDown.Value > 0.75f ||
			    menuActions.ActiveDevice.DPadDown.IsPressed) ||
			    Input.GetKeyDown (KeyCode.DownArrow))
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

						MainPauseMenuOnEnter (HighlightDownVal);
						MainPauseMenuOnExit (UnHighlightDownVal);
					}

					// Reset scroll speed.
					nextScroll = Time.unscaledTime + scrollSpeed;
				}
			}

			// Player presses the A button.
			if (menuActions.ActiveDevice.Action1.IsPressed ||
			    Input.GetKeyDown (KeyCode.Return)) 
			{
				if (Time.unscaledTime > aButtonNextCooldown) 
				{
					MainPauseMenuOnClick ();

					// Reset A button cooldown.
					aButtonNextCooldown = Time.unscaledTime + aButtonCoolDown;
				}
			}

			// Player presses the B button.
			if (menuActions.ActiveDevice.Action2.IsPressed)
			{
				if (Time.unscaledTime > bButtonNextCooldown) 
				{
					// Set button to first option and execute the command.
					menuButtons.buttonIndex = 0;
					//MainPauseMenuOnClick ();

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

	// Invoke an OnClick event.
	public void MainPauseMenuOnClick ()
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
	public void MainPauseMenuOnEnter (int index)
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

	public void MainPauseMenuOnExit (int index)
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

	[Header ("Menu Buttons")]
	public MenuButtons menuButtons;
	[System.Serializable]
	public class MenuButtons
	{
		[Tooltip ("Current index for the menu button focus.")]
		public int buttonIndex = 0;
		[Tooltip ("The size of the amount of buttons or event triggers.")]
		public int maxButtons = 3;

		[Tooltip ("All the Buttons in order.")]
		public Button[] menuButtons;
		[Tooltip ("All the EventTriggers in order.")]
		public EventTrigger[] menuEvents;

		[Tooltip ("The button that gets pressed when B is pressed.")]
		public Button BackButton;
	}
}