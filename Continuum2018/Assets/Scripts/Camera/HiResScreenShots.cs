using UnityEngine;

public class HiResScreenShots : MonoBehaviour 
{
	public Vector2 resolution = new Vector2 (1920, 1080); // Base resolution width.
	public Vector2 ResolutionMultiplier = new Vector2 (1, 1); // Multiplies by factor, Quadruples total resolution. +1 = x2.
	private bool takeHiResShot = false; // Is screenshot being taken now?

	private Camera cam; // Reference to camera.

	void Awake ()
	{
		cam = GetComponent<Camera> (); // Gets the Camera to take a screenshot with.
	}

	// Creates file name.
	public static string ScreenShotName (int width, int height) 
	{
		return string.Format("{0}/Screenshots/screen_{1}x{2}_{3}.png", 
			Application.dataPath, 
			width, height, 
			System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
	}

	public void TakeHiResShot() 
	{
		takeHiResShot = true;
	}

	// Frame has finished rendering.
	// Only want to take screenshots once the frame is fully rendered.
	void LateUpdate() 
	{
		if (Input.GetKeyDown (KeyCode.F9))
		{
			ResolutionMultiplier = new Vector2 (2, 2);
			TakeHiResShot ();
		}

		if (Input.GetKeyDown (KeyCode.F8)) 
		{
			ResolutionMultiplier = new Vector2 (1, 1);
			TakeHiResShot ();
		}

		if (takeHiResShot == true) 
		{
			// Create file name and directory if it doesn't exist.
			if (System.IO.Directory.Exists (Application.persistentDataPath + "/" + "Screenshots") == false)
			{
				System.IO.Directory.CreateDirectory (Application.persistentDataPath + "/" + "Screenshots");
				Debug.Log ("Screenshot folder was missing, created new one.");
			}
				
			// Scxreenshot directory exists? Take a screenshot.
			if (System.IO.Directory.Exists (Application.persistentDataPath + "/" + "Screenshots") == true)
			{
				// Get new resolution.
				Vector2 newResolution = new Vector2 (
					Mathf.RoundToInt ((int)resolution.x * ResolutionMultiplier.x), 
					Mathf.RoundToInt ((int)resolution.y * ResolutionMultiplier.y)
				);
			
				// Create a new render texture and store it.
				RenderTexture rt = new RenderTexture (
					(int)newResolution.x, 
					(int)newResolution.y,
					24
				);

				// Set camera target texture to render texture temporaily.
				cam.targetTexture = rt;

				// Create a Texture 2D with new resolution and bit depth. Don't allow mipmaps.
				Texture2D screenShot = new Texture2D (
					(int)newResolution.x, 
					(int)newResolution.y, 
					TextureFormat.RGB24, 
					false
				);

				// Manually render the camera.
				cam.Render ();

				// Assign currently active render texture to temporary texture.
				RenderTexture.active = rt;

				// Read all the pixels in the new image with no offset and specify width and height.
				screenShot.ReadPixels (
					new Rect (
						0, 
						0, 
						(int)newResolution.x, 
						(int)newResolution.y), 
					0, 
					0
				);

				// Unnassign target texture.
				cam.targetTexture = null;

				// Unnassign active render texture to avoid errors.
				RenderTexture.active = null;

				// Destroy the temporary render texture.
				Destroy(rt);

				// Encode the pixels to a .png format.
				byte[] bytes = screenShot.EncodeToPNG();

				// Give screenshot name and specify the dimensions.
				string filename = ScreenShotName (
					(int)newResolution.x, 
					(int)newResolution.y
				);

				// Write the file and specify bytes to store.
				System.IO.File.WriteAllBytes (filename, bytes);

				// Confirm that the screenshot took place.
				Debug.Log (string.Format("Took screenshot to: {0}", filename));
			}
				
			takeHiResShot = false; // Stop taking a screenshot.
		}
	}
}