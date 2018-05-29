using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class PostProcessBuild
{
	public static int buildNumber;

	[PostProcessBuildAttribute(1)]
	public static void OnPostprocessBuild (BuildTarget target, string pathToBuiltProject) 
	{
		Debug.Log (pathToBuiltProject);
		IncrementBuildNumber ();
	}

	public static void IncrementBuildNumber ()
	{
		buildNumber++;
		Debug.Log ("Build number: " + buildNumber);
	}
}