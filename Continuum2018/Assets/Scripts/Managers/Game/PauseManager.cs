using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour 
{
	public GameController gameControllerScript;
	public TimescaleController timeScaleControllerScript;
	public PlayerController playerControllerScript_P1;
	public GameObject PauseUI;

	[Header ("Scroll attributes")]
	public float scrollSpeed = 0.5f;
	private float nextScroll;
	public float aButtonCoolDown = 0.5f;
	public float aButtonNextCooldown;
	public float bButtonCoolDown = 0.5f;
	public float bButtonNextCooldown;
	public float startButtonCooldown;
	public float startButtonNextCooldown;

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
		if (gameControllerScript.isPaused == true && PauseUI.activeInHierarchy == true) 
		{
			// Player presses up on the left stick or D-Pad up.
			if ((menuActions.ActiveDevice.LeftStickUp.Value > 0.75f ||
			    menuActions.ActiveDevice.DPadUp.IsPressed) ||
			    Input.GetKeyDown (KeyCode.UpArrow)) 
			{
				if (Time.unscaledTime > nextScroll) 
				{
					// Scrolling down, before reaching the end.
					if (mainPauseMenu.buttonIndex >= 1)
					{
						mainPauseMenu.buttonIndex -= 1; // Decrement button index.
						mainPauseMenu.buttonIndex = Mathf.Clamp (mainPauseMenu.buttonIndex, 0, mainPauseMenu.maxButtons);
						int HighlightUpVal = mainPauseMenu.buttonIndex;
						int UnHighlightUpVal = Mathf.Clamp (mainPauseMenu.buttonIndex + 1, 0, mainPauseMenu.maxButtons);
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
					if (mainPauseMenu.buttonIndex < mainPauseMenu.maxButtons) 
					{
						mainPauseMenu.buttonIndex += 1; // Increment button index.
						mainPauseMenu.buttonIndex = Mathf.Clamp (mainPauseMenu.buttonIndex, 0, mainPauseMenu.maxButtons);
						int HighlightDownVal = mainPauseMenu.buttonIndex;
						int UnHighlightDownVal = Mathf.Clamp (mainPauseMenu.buttonIndex - 1, 0, mainPauseMenu.maxButtons);
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
					mainPauseMenu.buttonIndex = 0;
					MainPauseMenuOnClick ();

					// Reset B button cooldown.
					bButtonNextCooldown = Time.unscaledTime + bButtonCoolDown;
				}
			}
		} 

		else 
		
		{
		}
	}

	public void MainPauseMenuOnClick ()
	{
		ExecuteEvents.Execute (
			mainPauseMenu.PauseMenuButtons [mainPauseMenu.buttonIndex].gameObject, 
			pointerEventData, 
			ExecuteEvents.pointerClickHandler
		);
	}

	public void MainPauseMenuOnEnter (int index)
	{
		ExecuteEvents.Execute (
			mainPauseMenu.PauseMenuEvents [index].gameObject, 
			pointerEventData, 
			ExecuteEvents.pointerEnterHandler
		);
	}

	public void MainPauseMenuOnExit (int index)
	{
		ExecuteEvents.Execute (
			mainPauseMenu.PauseMenuEvents [index].gameObject, 
			pointerEventData, 
			ExecuteEvents.pointerExitHandler
		);
	}

	[Header ("Main Pause Menu")]
	public MainPauseMenu mainPauseMenu;
	[System.Serializable]
	public class MainPauseMenu
	{
		public int buttonIndex = 0;
		public int maxButtons = 3;

		public Button[] PauseMenuButtons;
		public EventTrigger[] PauseMenuEvents;
	}
}