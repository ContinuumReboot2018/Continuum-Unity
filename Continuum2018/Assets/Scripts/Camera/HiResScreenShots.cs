using UnityEngine;
using System.Collections;

public class HiResScreenShots : MonoBehaviour 
{
	public int resWidth = 3840; 
	public int resHeight = 2160;

	private bool takeHiResShot = false;
	private bool takeHiDoubleResShot = false;

	private Camera cam;

	void Awake ()
	{
		cam = GetComponent<Camera> ();
	}

	public static string ScreenShotName(int width, int height) 
	{
		return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png", 
			Application.dataPath, 
			width, height, 
			System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
	}

	public void TakeHiResShot() 
	{
		takeHiResShot = true;
	}

	public void TakeHiDoubleResShot ()
	{
		takeHiDoubleResShot = true;
	}

	void LateUpdate() 
	{
		if (Input.GetKeyDown (KeyCode.Alpha9))
		{
			TakeHiResShot ();
		}

		if (Input.GetKeyDown (KeyCode.Alpha8)) 
		{
			TakeHiDoubleResShot ();
		}

		if (takeHiResShot == true) 
		{
			RenderTexture rt = new RenderTexture (resWidth, resHeight, 24);
			cam.targetTexture = rt;
			Texture2D screenShot = new Texture2D (resWidth, resHeight, TextureFormat.RGB24, false);
			cam.Render();
			RenderTexture.active = rt;
			screenShot.ReadPixels (new Rect (0, 0, resWidth, resHeight), 0, 0);
			cam.targetTexture = null;
			RenderTexture.active = null; // JC: added to avoid errors
			Destroy(rt);
			byte[] bytes = screenShot.EncodeToPNG();
			string filename = ScreenShotName (resWidth, resHeight);
			System.IO.File.WriteAllBytes (filename, bytes);
			Debug.Log (string.Format("Took screenshot to: {0}", filename));
			takeHiResShot = false;
		}

		if (takeHiDoubleResShot == true) 
		{
			RenderTexture rt = new RenderTexture (resWidth * 2, resHeight * 2, 24);
			cam.targetTexture = rt;
			Texture2D screenShot = new Texture2D (resWidth* 2, resHeight* 2, TextureFormat.RGB24, false);
			cam.Render();
			RenderTexture.active = rt;
			screenShot.ReadPixels (new Rect (0, 0, resWidth * 2, resHeight * 2), 0, 0);
			cam.targetTexture = null;
			RenderTexture.active = null; // JC: added to avoid errors
			Destroy(rt);
			byte[] bytes = screenShot.EncodeToPNG();
			string filename = ScreenShotName (resWidth * 2, resHeight * 2);
			System.IO.File.WriteAllBytes (filename, bytes);
			Debug.Log (string.Format("Took screenshot to: {0}", filename));
			takeDoubleHiResShot = false;
		}
	}
}