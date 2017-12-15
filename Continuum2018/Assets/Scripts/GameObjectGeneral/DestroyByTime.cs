using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByTime : MonoBehaviour
{
	public float delay = 1;

	void Awake () 
	{
		StartCoroutine (DestroyObject ());
	}
		
	IEnumerator DestroyObject ()
	{
		yield return new WaitForSecondsRealtime (delay);
		Destroy (gameObject);
	}
}
