using UnityEngine;
using InControl;

public class InControlPlatformChecker : MonoBehaviour 
{
	public TouchManager inControlTouchManager;

	void Start () 
	{
		if (inControlTouchManager == null) 
		{
			inControlTouchManager = GetComponent<TouchManager> ();
		}

		CheckPlatform ();
	}

	void CheckPlatform ()
	{
		if (Application.isMobilePlatform == true)
		{
			inControlTouchManager.enabled = true;
			Debug.Log ("InControl Touch Manager enabled.");
		} else {
			Debug.Log ("This is not a mobile or touchscreen platform. InControl Touch Manager component not enabled.");
		}

		#if UNITY_ANDROID
			inControlTouchManager.enabled = true;
			Debug.Log ("InControl Touch Manager enabled.");
		#endif
	}
}