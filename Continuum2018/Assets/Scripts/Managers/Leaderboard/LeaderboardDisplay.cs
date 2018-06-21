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
		StartCoroutine (RefreshLeaderboard ());
		eventData = new PointerEventData (EventSystem.current);
		LeaderboardActions = new PlayerActions ();
		AssignActionControls ();
	}

	IEnumerator RefreshLeaderboard ()
	{
		yield return new WaitForSecondsRealtime (1);
		UpdateLeaderboard ();
		StartCoroutine (RefreshLeaderboard ());
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

						UpdateLeaderboardMissionText_ ();
						//UpdateLeaderboardDelayed ();
						UpdateLeaderboard ();
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

						UpdateLeaderboardMissionText_ ();
						//UpdateLeaderboardDelayed ();
						UpdateLeaderboard ();
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
	public void LeaderboardModeOnClick ()
	{
		// Check if there is an Button component present.
		Button OnClickEvent = leaderboardModeButtons.Buttons [leaderboardModeButtons.LeaderboardModeIndex].GetComponent<Button> ();

		if (OnClickEvent != null) 
		{
			ExecuteEvents.Execute (
				leaderboardModeButtons.Buttons [leaderboardModeButtons.LeaderboardModeIndex].gameObject, 
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

	public void UpdateLeaderboardDelayed ()
	{
		switch (menuLeaderboardId)
		{
		case 0:
			for (int a = 0; a < saveAndLoadScript.Leaderboard_ArcadeMode.Count; a++) 
			{
				Names [a].text = saveAndLoadScript.Leaderboard_ArcadeMode [a].name.ToString ();
				Scores [a].text = saveAndLoadScript.Leaderboard_ArcadeMode [a].score.ToString ("N0");
				Waves [a].text = saveAndLoadScript.Leaderboard_ArcadeMode [a].wave.ToString ();
			}

			Debug.Log ("Opened arcade leaderboard display.");
			break;
		case 1:
			/*
			for (int b = 0; b < saveAndLoadScript.Leaderboard_ModsMode.Count; b++)
			{
				Names  [b].text = saveAndLoadScript.Leaderboard_ModsMode [b].score.ToString ("N0");
				Waves  [b].text = saveAndLoadScript.Leaderboard_ModsMode [b].wave.ToString ();
			}
			*/
			break;
		case 2:
			for (int c = 0; c < saveAndLoadScript.Leaderboard_BossRushMode.Count; c++)
			{
				Names  [c].text = saveAndLoadScript.Leaderboard_BossRushMode [c].name.ToString ();
				Scores [c].text = saveAndLoadScript.Leaderboard_BossRushMode [c].score.ToString ("N0");
				Waves  [c].text = saveAndLoadScript.Leaderboard_BossRushMode [c].wave.ToString ();
			}
			Debug.Log ("Opened boss mode leaderboard display.");
			break;
		case 3:
			for (int d = 0; d < saveAndLoadScript.Leaderboard_LuckyMode.Count; d++)
			{
				Names  [d].text = saveAndLoadScript.Leaderboard_LuckyMode [d].name.ToString ();
				Scores [d].text = saveAndLoadScript.Leaderboard_LuckyMode [d].score.ToString ("N0");
				Waves  [d].text = saveAndLoadScript.Leaderboard_LuckyMode [d].wave.ToString ();
			}
			Debug.Log ("Opened lucky mode leaderboard display.");
			break;
		case 4:
			for (int e = 0; e < saveAndLoadScript.Leaderboard_FullyLoadedMode.Count; e++)
			{
				Names  [e].text = saveAndLoadScript.Leaderboard_FullyLoadedMode [e].name.ToString ();
				Scores [e].text = saveAndLoadScript.Leaderboard_FullyLoadedMode [e].score.ToString ("N0");
				Waves  [e].text = saveAndLoadScript.Leaderboard_FullyLoadedMode [e].wave.ToString ();
			}
			Debug.Log ("Opened fully loaded leaderboard display.");
			break;
		case 5:
			for (int f = 0; f < saveAndLoadScript.Leaderboard_ScavengerMode.Count; f++)
			{
				Names  [f].text = saveAndLoadScript.Leaderboard_ScavengerMode [f].name.ToString ();
				Scores [f].text = saveAndLoadScript.Leaderboard_ScavengerMode [f].score.ToString ("N0");
				Waves  [f].text = saveAndLoadScript.Leaderboard_ScavengerMode [f].wave.ToString ();
			}
			Debug.Log ("Opened scavenger leaderboard display.");
			break;
		case 6:
			for (int g = 0; g < saveAndLoadScript.Leaderboard_HellMode.Count; g++)
			{
				Names  [g].text = saveAndLoadScript.Leaderboard_HellMode [g].name.ToString ();
				Scores [g].text = saveAndLoadScript.Leaderboard_HellMode [g].score.ToString ("N0");
				Waves  [g].text = saveAndLoadScript.Leaderboard_HellMode [g].wave.ToString ();
			}
			Debug.Log ("Opened hell mode leaderboard display.");
			break;
		case 7:
			for (int h = 0; h < saveAndLoadScript.Leaderboard_FastTrackMode.Count; h++)
			{
				Names  [h].text = saveAndLoadScript.Leaderboard_FastTrackMode [h].name.ToString ();
				Scores [h].text = saveAndLoadScript.Leaderboard_FastTrackMode [h].score.ToString ("N0");
				Waves  [h].text = saveAndLoadScript.Leaderboard_FastTrackMode [h].wave.ToString ();
			}
			Debug.Log ("Opened fast track mode leaderboard display.");
			break;
		}

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
		//case 1:
			//MissionText.text = "MODS";
			//break;
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

		switch (saveAndLoadScript.MissionId) 
		{
		case 0:
			for (int i = 0; i < saveAndLoadScript.Leaderboard_ArcadeMode.Count; i++)
			{
				Names  [i].text = saveAndLoadScript.Leaderboard_ArcadeMode [i].name.ToString ();
				Scores [i].text = saveAndLoadScript.Leaderboard_ArcadeMode [i].score.ToString ("N0");
				Waves  [i].text = saveAndLoadScript.Leaderboard_ArcadeMode [i].wave.ToString ();
			}
			break;
		case 1:
			/*
			for (int j = 0; j < saveAndLoadScript.Leaderboard_ModsMode.Count; j++)
			{
				Names  [j].text = saveAndLoadScript.Leaderboard_ModsMode [j].score.ToString ("N0");
				Waves  [j].text = saveAndLoadScript.Leaderboard_ModsMode [j].wave.ToString ();
			}
			*/
			break;
		case 2:
			for (int k = 0; k < saveAndLoadScript.Leaderboard_BossRushMode.Count; k++)
			{
				Names  [k].text = saveAndLoadScript.Leaderboard_BossRushMode [k].name.ToString ();
				Scores [k].text = saveAndLoadScript.Leaderboard_BossRushMode [k].score.ToString ("N0");
				Waves  [k].text = saveAndLoadScript.Leaderboard_BossRushMode [k].wave.ToString ();
			}
			break;
		case 3:
			for (int l = 0; l < saveAndLoadScript.Leaderboard_LuckyMode.Count; l++)
			{
				Names  [l].text = saveAndLoadScript.Leaderboard_LuckyMode [l].name.ToString ();
				Scores [l].text = saveAndLoadScript.Leaderboard_LuckyMode [l].score.ToString ("N0");
				Waves  [l].text = saveAndLoadScript.Leaderboard_LuckyMode [l].wave.ToString ();
			}
			break;
		case 4:
			for (int m = 0; m < saveAndLoadScript.Leaderboard_FullyLoadedMode.Count; m++)
			{
				Names  [m].text = saveAndLoadScript.Leaderboard_FullyLoadedMode [m].name.ToString ();
				Scores [m].text = saveAndLoadScript.Leaderboard_FullyLoadedMode [m].score.ToString ("N0");
				Waves  [m].text = saveAndLoadScript.Leaderboard_FullyLoadedMode [m].wave.ToString ();
			}
			break;
		case 5:
			for (int n = 0; n < saveAndLoadScript.Leaderboard_ScavengerMode.Count; n++)
			{
				Names  [n].text = saveAndLoadScript.Leaderboard_ScavengerMode [n].name.ToString ();
				Scores [n].text = saveAndLoadScript.Leaderboard_ScavengerMode [n].score.ToString ("N0");
				Waves  [n].text = saveAndLoadScript.Leaderboard_ScavengerMode [n].wave.ToString ();
			}
			break;
		case 6:
			for (int o = 0; o < saveAndLoadScript.Leaderboard_HellMode.Count; o++)
			{
				Names  [o].text = saveAndLoadScript.Leaderboard_HellMode [o].name.ToString ();
				Scores [o].text = saveAndLoadScript.Leaderboard_HellMode [o].score.ToString ("N0");
				Waves  [o].text = saveAndLoadScript.Leaderboard_HellMode [o].wave.ToString ();
			}
			break;
		case 7:
			for (int p = 0; p < saveAndLoadScript.Leaderboard_FastTrackMode.Count; p++)
			{
				Names  [p].text = saveAndLoadScript.Leaderboard_FastTrackMode [p].name.ToString ();
				Scores [p].text = saveAndLoadScript.Leaderboard_FastTrackMode [p].score.ToString ("N0");
				Waves  [p].text = saveAndLoadScript.Leaderboard_FastTrackMode [p].wave.ToString ();
			}
			break;
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