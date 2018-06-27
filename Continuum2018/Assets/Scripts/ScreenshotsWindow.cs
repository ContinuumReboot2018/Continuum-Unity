using UnityEngine;
using UnityEditor;

public class ScreenshotsWindow : EditorWindow
{
	[MenuItem ("Window/Screenshot Viewer")]
	public static void ShowWindow ()
	{
		//GetWindow<ScreenshotsWindow> ("Screenshots Viewer");
		Application.OpenURL (Application.persistentDataPath + "/" + "Screenshots");
	}
}