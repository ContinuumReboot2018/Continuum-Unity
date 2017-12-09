using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindCameraInCanvas : MonoBehaviour 
{
	public Canvas canvas;
	public Camera cam;

	void Start () 
	{
		if (canvas == null) 
		{
			canvas = GetComponent<Canvas> ();
		}

		if (cam == null)
		{
			cam = Camera.main;
		}

		canvas.worldCamera = cam;
	}
}
