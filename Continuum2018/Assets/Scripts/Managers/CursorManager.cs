using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour 
{
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
}
