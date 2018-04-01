using UnityEngine;
using UnityEngine.UI;

public class FindCameraInCanvas : MonoBehaviour 
{
	[Tooltip ("Canvas to look at.")]
	public Canvas canvas;
	[Tooltip ("Camera to look at.")]
	public Camera cam;
	[Tooltip ("Use the camera space mode.")]
	public bool CameraSpace;

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
			
		if (CameraSpace == true) 
		{
			canvasFind.renderMode = RenderMode.ScreenSpaceCamera;
		}

		canvas.worldCamera = cam; // Set Camera to the canvas.
	}
}