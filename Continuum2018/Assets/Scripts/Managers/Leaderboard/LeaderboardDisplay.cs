using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using InControl;

public class LeaderboardDisplay : MonoBehaviour 
{
	private SaveAndLoadScript saveAndLoadScript;
	public GameObject UI;
	private int menuLeaderboardId;
	public TextMeshProUGUI MissionText;

	public TextMeshProUGUI[] Names;
	public TextMeshProUGUI[] Scores;
	public TextMeshProUGUI[] Waves;

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

	[Space (10)]
	public bool useDisplayMode;
	[Space (10)]

	public LeaderboardModeButtons leaderboardModeButtons;
	public LeaderboardDisplayButtons leaderboardDisplayButtons;
	public Button LeaderboardBackButton;

	public PointerEventData eventData;
	public PlayerActions LeaderboardActions;

	void Start ()
	{
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();
		menuLeaderboardId = saveAndLoadScript.MissionId;
		UpdateLeaderboardMissionText_ ();
		UpdateLeaderboard ();
		eventData = new PointerEventData (EventSystem.current);
		LeaderboardActions = new PlayerActions ();
		AssignActionControls ();
	}

	IEnumerator RefreshLeaderboard ()
	{
		yield return new WaitForSecondsRealtime (1);
		UpdateLeaderboard ();
	}

	void Update ()
	{
		CheckForInput ();
	}

	public void NextLeaderboardId ()
	{
		if (menuLeaderboardId < 7) 
		{
			menuLeaderboardId += 1;

			// leaderboard ID skip.
			if (menuLeaderboardId == 1) 
			{
				menuLeaderboardId += 1;
			}
		} 

		else 

		{
			menuLeaderboardId = 0;
		}
	}

	public void PreviousLeaderboardId ()
	{
		if (menuLeaderboardId > 0) 
		{
			menuLeaderboardId -= 1;

			// leaderboard ID skip.
			if (menuLeaderboardId == 1) 
			{
				menuLeaderboardId -= 1;
			}
		} 

		else 

		{
			menuLeaderboardId = 7;
		}
	}

	void CheckForInput ()
	{
		// Only perform tasks if this UI is active.
		if (UI.activeInHierarchy == true) 
		{
			// Player presses up on the left stick or D-Pad up.
			if (LeaderboardActions.MoveUp.Value > 0.75f) 
			{
				if (Time.unscaledTime > nextScroll) 
				{
					useDisplayMode = false;

					// Reset scroll speed.
					nextScroll = Time.unscaledTime + scrollSpeed;
				}
			}

			// Player presses down on the left stick or D-Pad down.
			if (LeaderboardActions.MoveDown.Value > 0.75f)
			{
				if (Time.unscaledTime > nextScroll) 
				{
					useDisplayMode = true;

					// Reset scroll speed.
					nextScroll = Time.unscaledTime + scrollSpeed;
				}
			}

			// Player presses left on the left stick or D-Pad left.
			if (LeaderboardActions.MoveLeft.Value > 0.75f)
			{
				if (Time.unscaledTime > nextScroll) 
				{
					if (useDisplayMode == true) 
					{
						
					}

					if (useDisplayMode == false) 
					{
						LeaderboardModeOnClick (0);
					}

					// Reset scroll speed.
					nextScroll = Time.unscaledTime + scrollSpeed;
				}
			}

			// Player presses right on the left stick or D-Pad right.
			if (LeaderboardActions.MoveRight.Value > 0.75f)
			{
				if (Time.unscaledTime > nextScroll) 
				{
					if (useDisplayMode == true) 
					{

					}

					if (useDisplayMode == false) 
					{
						LeaderboardModeOnClick (1);
					}

					// Reset scroll speed.
					nextScroll = Time.unscaledTime + scrollSpeed;
				}
			}

			// Player presses the B button.
			if (LeaderboardActions.Ability.WasPressed)
			{
				if (Time.unscaledTime > bButtonNextCooldown) 
				{
					// Override button event for navigating back.
					if (LeaderboardBackButton != null) 
					{
						Button OnClickEvent = LeaderboardBackButton.GetComponent<Button> ();

						if (OnClickEvent != null) 
						{
							ExecuteEvents.Execute (
								LeaderboardBackButton.gameObject, 
								eventData, 
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
	public void LeaderboardModeOnClick (int mode)
	{
		// Check if there is an Button component present.
		Button OnClickEvent = leaderboardModeButtons.Buttons [mode].GetComponent<Button> ();

		if (OnClickEvent != null) 
		{
			ExecuteEvents.Execute (
				leaderboardModeButtons.Buttons [mode].gameObject, 
				eventData, 
				ExecuteEvents.pointerClickHandler
			);
		}
	}

	// Invoke an OnClick event.
	public void LeaderboardDisplayOnClick ()
	{
		// Check if there is an Button component present.
		Button OnClickEvent = leaderboardDisplayButtons.Buttons [leaderboardDisplayButtons.LeaderboardDisplayIndex].GetComponent<Button> ();

		if (OnClickEvent != null) 
		{
			ExecuteEvents.Execute (
				leaderboardDisplayButtons.Buttons [leaderboardDisplayButtons.LeaderboardDisplayIndex].gameObject, 
				eventData, 
				ExecuteEvents.pointerClickHandler
			);
		}
	}

	// Invoke an OnPointerEnter event.
	public void MenuOnEnter_LeaderboardMode (int index)
	{
		// Check if there is an EventTrigger component present.
		EventTrigger OnEnterEvent = leaderboardModeButtons.Events [index].GetComponent<EventTrigger> ();

		if (OnEnterEvent != null) 
		{
			ExecuteEvents.Execute (
				leaderboardModeButtons.Events [index].gameObject, 
				eventData, 
				ExecuteEvents.pointerEnterHandler
			);
		}
	}

	// Invoke an OnPointerEnter event.
	public void MenuOnEnter_LeaderboardDisplay (int index)
	{
		// Check if there is an EventTrigger component present.
		EventTrigger OnEnterEvent = leaderboardDisplayButtons.Events [index].GetComponent<EventTrigger> ();

		if (OnEnterEvent != null) 
		{
			ExecuteEvents.Execute (
				leaderboardDisplayButtons.Events [index].gameObject, 
				eventData, 
				ExecuteEvents.pointerEnterHandler
			);
		}
	}

	public void MenuOnExit_LeaderboardMode (int index)
	{
		// Check if there is an EventTrigger component present.
		EventTrigger OnExitEvent = leaderboardModeButtons.Events [index].GetComponent<EventTrigger> ();

		if (OnExitEvent != null)
		{
			ExecuteEvents.Execute (
				leaderboardModeButtons.Events [index].gameObject, 
				eventData, 
				ExecuteEvents.pointerExitHandler
			);
		}
	}

	public void MenuOnExit_LeaderboardDisplay (int index)
	{
		// Check if there is an EventTrigger component present.
		EventTrigger OnExitEvent = leaderboardDisplayButtons.Events [index].GetComponent<EventTrigger> ();

		if (OnExitEvent != null)
		{
			ExecuteEvents.Execute (
				leaderboardDisplayButtons.Events [index].gameObject, 
				eventData, 
				ExecuteEvents.pointerExitHandler
			);
		}
	}

	public void SetButtonIndex_LeaderboardMode (int buttonIndex)
	{
		leaderboardModeButtons.LeaderboardModeIndex = buttonIndex;
		MenuOnEnter_LeaderboardMode (buttonIndex);
	}

	public void SetButtonIndex_LeaderboardDisplay (int buttonIndex)
	{
		leaderboardDisplayButtons.LeaderboardDisplayIndex = buttonIndex;
		MenuOnEnter_LeaderboardDisplay (buttonIndex);
	}

	public void RefreshButtonIndex_LeaderboardMode ()
	{
		MenuOnEnter_LeaderboardMode (leaderboardModeButtons.LeaderboardModeIndex);
	}

	public void RefreshButtonIndex_LeaderboardDisplay ()
	{
		MenuOnEnter_LeaderboardDisplay (leaderboardDisplayButtons.LeaderboardDisplayIndex);
	}

	// Use this only for menus.
	public void UpdateLeaderboardDelayed ()
	{
		for (int i = 0; i < saveAndLoadScript.Leaderboards[menuLeaderboardId].leaderboard.Count; i++) 
		{
			Names  [i].text = saveAndLoadScript.Leaderboards[menuLeaderboardId].leaderboard [i].name.ToString ();
			Scores [i].text = saveAndLoadScript.Leaderboards[menuLeaderboardId].leaderboard [i].score.ToString ("N0");
			Waves  [i].text = saveAndLoadScript.Leaderboards[menuLeaderboardId].leaderboard [i].wave.ToString ();
		}
			
		//UpdateLeaderboard ();
		return;
	}

	public void UpdateLeaderboardMenu (int leaderboardMenu)
	{
		menuLeaderboardId = leaderboardMenu;

		Invoke ("UpdateLeaderboardDelayed", 0.01667f);
	}

	public void UpdateLeaderboardMissionText (string missionText)
	{
		MissionText.text = missionText;
	}

	public void UpdateLeaderboardMissionText_ ()
	{
		switch (menuLeaderboardId)
		{
		case 0:
			MissionText.text = "ARCADE";
			break;
		case 2:
			MissionText.text = "BOSS RUSH";
			break;
		case 3:
			MissionText.text = "LUCKY";
			break;
		case 4:
			MissionText.text = "FULLY LOADED";
			break;
		case 5:
			MissionText.text = "SCAVENGER";
			break;
		case 6:
			MissionText.text = "HELL";
			break;
		case 7:
			MissionText.text = "FAST TRACK";
			break;
		}
	}

	public void UpdateLeaderboard ()
	{
		Debug.Log ("Updating leaderboard.");

		for (int i = 0; i < saveAndLoadScript.Leaderboards[menuLeaderboardId].leaderboard.Count; i++)
		{
			Names  [i].text = saveAndLoadScript.Leaderboards[menuLeaderboardId].leaderboard [i].name.ToString ();
			Scores [i].text = saveAndLoadScript.Leaderboards[menuLeaderboardId].leaderboard [i].score.ToString ("N0");
			Waves  [i].text = saveAndLoadScript.Leaderboards[menuLeaderboardId].leaderboard [i].wave.ToString ();
		}
	}

	void AssignActionControls ()
	{
		// LEFT
		LeaderboardActions.MoveLeft.AddDefaultBinding (Key.A);
		LeaderboardActions.MoveLeft.AddDefaultBinding (Key.LeftArrow);
		LeaderboardActions.MoveLeft.AddDefaultBinding (InputControlType.LeftStickLeft);
		LeaderboardActions.MoveLeft.AddDefaultBinding (InputControlType.DPadLeft);

		// RIGHT
		LeaderboardActions.MoveRight.AddDefaultBinding (Key.D);
		LeaderboardActions.MoveRight.AddDefaultBinding (Key.RightArrow);
		LeaderboardActions.MoveRight.AddDefaultBinding (InputControlType.LeftStickRight);
		LeaderboardActions.MoveRight.AddDefaultBinding (InputControlType.DPadRight);

		// UP
		LeaderboardActions.MoveUp.AddDefaultBinding (Key.W);
		LeaderboardActions.MoveUp.AddDefaultBinding (Key.UpArrow);
		LeaderboardActions.MoveUp.AddDefaultBinding (InputControlType.LeftStickUp);
		LeaderboardActions.MoveUp.AddDefaultBinding (InputControlType.DPadUp);

		// DOWN
		LeaderboardActions.MoveDown.AddDefaultBinding (Key.S);
		LeaderboardActions.MoveDown.AddDefaultBinding (Key.DownArrow);
		LeaderboardActions.MoveDown.AddDefaultBinding (InputControlType.LeftStickDown);
		LeaderboardActions.MoveDown.AddDefaultBinding (InputControlType.DPadDown);

		// A
		LeaderboardActions.Shoot.AddDefaultBinding (Key.Space);
		LeaderboardActions.Shoot.AddDefaultBinding (Key.Return);
		LeaderboardActions.Shoot.AddDefaultBinding (InputControlType.Action1);

		// B
		LeaderboardActions.Ability.AddDefaultBinding (Key.Escape);
		LeaderboardActions.Ability.AddDefaultBinding (InputControlType.Action2);

		// Start
		LeaderboardActions.Pause.AddDefaultBinding (Key.Return);
		LeaderboardActions.Pause.AddDefaultBinding (InputControlType.Command);
	}

	[System.Serializable]
	public class LeaderboardModeButtons
	{
		public int LeaderboardModeIndex;
		public Button[] Buttons;
		public EventTrigger[] Events;
	}

	[System.Serializable]
	public class LeaderboardDisplayButtons
	{
		public int LeaderboardDisplayIndex;
		public Button[] Buttons;
		public EventTrigger[] Events;
	}
}