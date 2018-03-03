using UnityEngine;
using UnityEngine.UI;

public class FindCameraInCanvas : MonoBehaviour 
{
	public Canvas canvas; // Canvas to look at.
	public Camera cam; // Camera to look at.

	void Start () 
	{
		Canvas canvasFind = GetComponent<Canvas> ();

		// Assigns canvas if none is set already.
		if (canvas == null && canvasFind != null) 
		{
			canvas = GetComponent<Canvas> ();
		}

		// Assigns Main Camera if none is assigned.
		if (cam == null)
		{
			cam = Camera.main;
		}
			
		canvas.worldCamera = cam; // Set Camera to the canvas.
	}
}