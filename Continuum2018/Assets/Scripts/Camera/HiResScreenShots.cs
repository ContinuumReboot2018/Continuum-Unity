using UnityEngine;

public class HiResScreenShots : MonoBehaviour 
{
	private Vector2 resolution = new Vector2 (1920, 1080); // Base resolution width.
	[Tooltip ("Multiplies by factor.")]
	public Vector2 ResolutionMultiplier = new Vector2 (1, 1);
	private bool takeHiResShot = false; // Is screenshot being taken now?
	private Camera cam; // Reference to camera.

	void Awake ()
	{
		cam = GetComponent<Camera> (); // Gets the Camera to take a screenshot with.

		float aspect = cam.aspect;

		// 16:9 ratio.
		if (aspect > 1.6f) 
		{
			resolution = new Vector2 (1920, 1080);
		}

		// 16:10 ratio.
		if (aspect <= 1.6f) 
		{
			resolution = new Vector2 (1920, 1200);
		}
	}

	// Creates file name.
	public static string ScreenShotName (int width, int height) 
	{
		return string.Format("{0}/Screenshots/screen_{1}x{2}_{3}.png", 
			Application.persistentDataPath, 
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
		// 16K (15360 x 8640)
		if (Input.GetKeyDown (KeyCode.F11))
		{
			resolution = new Vector2 (1920, 1080);
			ResolutionMultiplier = new Vector2 (8, 8);
			TakeHiResShot ();
		}

		// 8K (7680 x 4320)
		if (Input.GetKeyDown (KeyCode.F10))
		{
			resolution = new Vector2 (1920, 1080);
			ResolutionMultiplier = new Vector2 (4, 4);
			TakeHiResShot ();
		}

		// 4K (3840 x 2160)
		if (Input.GetKeyDown (KeyCode.F9))
		{
			resolution = new Vector2 (1920, 1080);
			ResolutionMultiplier = new Vector2 (2, 2);
			TakeHiResShot ();
		}

		// 1080p (1920 x 1080)
		if (Input.GetKeyDown (KeyCode.F8)) 
		{
			resolution = new Vector2 (1920, 1080);
			ResolutionMultiplier = new Vector2 (1, 1);
			TakeHiResShot ();
		}

		// 540p (960 x 540)
		if (Input.GetKeyDown (KeyCode.F7)) 
		{
			resolution = new Vector2 (1920, 1080);
			ResolutionMultiplier = new Vector2 (0.5f, 0.5f);
			TakeHiResShot ();
		}

		// 480p (854 x 480)
		if (Input.GetKeyDown (KeyCode.F5)) 
		{
			resolution = new Vector2 (854, 480);
			ResolutionMultiplier = new Vector2 (1, 1);
			TakeHiResShot ();
		}

		// 720p (1280 x 720)
		if (Input.GetKeyDown (KeyCode.F5)) 
		{
			resolution = new Vector2 (1280, 720);
			ResolutionMultiplier = new Vector2 (1, 1);
			TakeHiResShot ();
		}

		// 360p (640 x 360)
		if (Input.GetKeyDown (KeyCode.F4)) 
		{
			resolution = new Vector2 (1280, 720);
			ResolutionMultiplier = new Vector2 (0.5f, 0.5f);
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