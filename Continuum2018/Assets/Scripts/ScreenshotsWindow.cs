using UnityEngine;
using UnityEditor;

public class ScreenshotsWindow : EditorWindow
{
	[MenuItem ("Window/Screenshot Viewer")]
	public static void ShowWindow ()
	{
		//GetWindow<ScreenshotsWindow> ("Screenshots Viewer");
		string screenshotsfolder = Application.persistentDataPath + "/" + "Screenshots";
		Application.OpenURL (screenshotsfolder);
		Debug.Log ("Opened Screenshots folder at " + screenshotsfolder);
	}
}