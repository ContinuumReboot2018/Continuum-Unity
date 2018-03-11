using System.Collections;
using UnityEngine;

public class CursorManager : MonoBehaviour 
{
	public float VisibleTimer = 5.0f;
	private float VisibleTimerRemain;

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

	void Update ()
	{
		if (Input.GetAxis ("Mouse X") != 0 || Input.GetAxis ("Mouse Y") != 0)
		{
			UnlockMouse ();
			ShowMouse ();
			ResetMouseTimer ();
		}

		if (Input.GetAxis ("Mouse X") == 0 && Input.GetAxis ("Mouse Y") == 0) 
		{
			if (VisibleTimerRemain > 0)
			{
				VisibleTimerRemain -= Time.unscaledDeltaTime;
			}

			if (VisibleTimerRemain <= 0) 
			{
				HideMouse ();
				LockMouse ();
			}
		}
	}

	void ResetMouseTimer ()
	{
		if (VisibleTimerRemain < VisibleTimer) 
		{
			VisibleTimerRemain = VisibleTimer;
		}
	}
}
