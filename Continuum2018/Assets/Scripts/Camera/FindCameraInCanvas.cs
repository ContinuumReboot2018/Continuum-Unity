using UnityEngine;
using UnityEngine.UI;

public class FindCameraInCanvas : MonoBehaviour 
{
	public Canvas canvas; // Canvas to look at.
	public Camera cam; // Camera to look at.
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