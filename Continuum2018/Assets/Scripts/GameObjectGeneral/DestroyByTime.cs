using System.Collections;
using UnityEngine;

public class DestroyByTime : MonoBehaviour
{
	public float delay = 1; // Total lifetime before the gameObject is 
	public GameObject DestroyObj; // Object to destroy, sets to gameObject if null.
	public bool useUnscaledTime; // Whether object should be destroyed with or without influence of Time.timeScale.

	void Start ()
	{
		// If there is no DestroyObj assigned. Set reference to self.
		if (DestroyObj == null) 
		{
			DestroyObj = gameObject;
		}

		// Call this if intending to use unscaled time.
		if (useUnscaledTime == true) 
		{
			StartCoroutine (DestroyObjectUnscaled ());
		}

		// Call this if intending to use scaled time.
		if (useUnscaledTime == false) 
		{
			StartCoroutine (DestroyObjectScaled ());
		}
	}
		
	// Count down to destroy using unscaled time.
	IEnumerator DestroyObjectUnscaled ()
	{
		yield return new WaitForSecondsRealtime (delay);
		Destroy (DestroyObj);
	}

	// Count down to destroy using scaled time.
	IEnumerator DestroyObjectScaled ()
	{
		yield return new WaitForSeconds(delay);
		Destroy (DestroyObj);
	}

	// Optional to cancel a destroy function.
	public void CancelDestroy ()
	{
		StopCoroutine (DestroyObjectScaled ());
		StopCoroutine (DestroyObjectUnscaled ());
	}
}