using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class MyBuildPostprocessor 
{
	[PostProcessBuildAttribute(1)]
	public static void OnPostprocessBuild (BuildTarget target, string pathToBuiltProject) 
	{
		Debug.Log (pathToBuiltProject);
	}
}