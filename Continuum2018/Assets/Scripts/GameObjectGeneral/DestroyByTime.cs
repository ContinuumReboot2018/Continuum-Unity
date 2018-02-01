using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByTime : MonoBehaviour
{
	public float delay = 1;
	public bool useUnscaledTime;

	void Awake () 
	{
		if (useUnscaledTime == true) 
		{
			StartCoroutine (DestroyObjectUnscaled ());
		}

		if (useUnscaledTime == false) 
		{
			StartCoroutine (DestroyObjectScaled ());
		}
	}
		
	IEnumerator DestroyObjectUnscaled ()
	{
		yield return new WaitForSecondsRealtime (delay);
		Destroy (gameObject);
	}

	IEnumerator DestroyObjectScaled ()
	{
		yield return new WaitForSeconds(delay);
		Destroy (gameObject);
	}

	public void CancelDestroy ()
	{
		StopCoroutine (DestroyObjectScaled ());
		StopCoroutine (DestroyObjectUnscaled ());
	}
}
