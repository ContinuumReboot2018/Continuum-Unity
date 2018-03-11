using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour 
{
	public GameController gameControllerScript;
	public TimescaleController timeScaleControllerScript;
	public PlayerController playerControllerScript_P1;
	public GameObject UI;

	[Header ("Scroll attributes")]
	public float scrollSpeed = 0.5f;
	private float nextScroll;
	public float aButtonCoolDown = 0.5f;
	private float aButtonNextCooldown;
	public float bButtonCoolDown = 0.5f;
	private float bButtonNextCooldown;
	public float startButtonCooldown;
	private float startButtonNextCooldown;

	public PointerEventData pointerEventData;
	public PlayerActions menuActions;

	void Start ()
	{
		menuActions = new PlayerActions ();
		pointerEventData = new PointerEventData (EventSystem.current);
	}

	void Update () 
	{
		CheckForInput ();
	}

	void CheckForInput ()
	{
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
					MainPauseMenuOnClick ();

					// Reset B button cooldown.
					bButtonNextCooldown = Time.unscaledTime + bButtonCoolDown;
				}
			}
		} 
	}

	public void MainPauseMenuOnClick ()
	{
		ExecuteEvents.Execute (
			menuButtons.menuButtons [menuButtons.buttonIndex].gameObject, 
			pointerEventData, 
			ExecuteEvents.pointerClickHandler
		);
	}

	public void MainPauseMenuOnEnter (int index)
	{
		ExecuteEvents.Execute (
			menuButtons.menuEvents [index].gameObject, 
			pointerEventData, 
			ExecuteEvents.pointerEnterHandler
		);
	}

	public void MainPauseMenuOnExit (int index)
	{
		ExecuteEvents.Execute (
			menuButtons.menuEvents [index].gameObject, 
			pointerEventData, 
			ExecuteEvents.pointerExitHandler
		);
	}

	[Header ("Menu Buttons")]
	public MenuButtons menuButtons;
	[System.Serializable]
	public class MenuButtons
	{
		public int buttonIndex = 0;
		public int maxButtons = 3;

		public Button[] menuButtons;
		public EventTrigger[] menuEvents;
	}

	void OnDestroy ()
	{
		menuActions.Destroy ();
	}
}