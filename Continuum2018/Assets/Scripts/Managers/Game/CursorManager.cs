using UnityEngine;

public class CursorManager : MonoBehaviour 
{
	[Tooltip ("How long the timer can last before hiding the mouse.")]
	public float VisibleTimer = 5.0f;
	public float VisibleTimerRemain;

	void Start ()
	{
		VisibleTimerRemain = 0;
		HideMouse ();
		LockMouse ();
	}

	void LateUpdate ()
	{
		if (Cursor.visible == false) 
		{
			// When the mouse has received input.
			if (Input.GetAxis ("Mouse X") != 0 || Input.GetAxis ("Mouse Y") != 0
				|| Input.GetMouseButtonDown (0)) 
			{
				UnlockMouse ();
				ShowMouse ();
				ResetMouseTimer ();
			}
		}

		if (Cursor.visible == true)
		{
			// When there is no mouse input.
			if (Input.GetAxis ("Mouse X") == 0 && Input.GetAxis ("Mouse Y") == 0) 
			{
				// Timer hasn't run out.
				if (VisibleTimerRemain > 0)
				{
					VisibleTimerRemain -= Time.unscaledDeltaTime;
				}

				// Timer has run out, hide and lock the mouse.
				if (VisibleTimerRemain <= 0)
				{
					HideMouse ();
					LockMouse ();
				}
			}
		}
	}

	public void HideMouse ()
	{
		Cursor.visible = false;
	}

	public void LockMouse ()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	public void ShowMouse ()
	{
		Cursor.visible = true;
	}

	public void UnlockMouse ()
	{
		Cursor.lockState = CursorLockMode.None;
	}

	// Resets current state of the mouse.
	void ResetMouseTimer ()
	{
		if (VisibleTimerRemain < VisibleTimer) 
		{
			VisibleTimerRemain = VisibleTimer;
		}
	}
}