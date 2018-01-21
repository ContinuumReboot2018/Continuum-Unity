using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLookAt : MonoBehaviour 
{
	public Transform LookAtPos;
	public lookType LookMethod;
	public enum lookType
	{
		LookTowards,
		LookAway
	}

	public float RotDegPerSec = 10;

	void LateUpdate ()
	{
		if (LookMethod == lookType.LookTowards && LookAtPos != null) 
		{
			transform.LookAt (LookAtPos.position, Vector3.forward);
		}

		if (LookMethod == lookType.LookAway) 
		{
			//Quaternion rot = Quaternion.LookRotation (LookAtPos.position, Vector3.up);
			//transform.LookAt (2*transform.position, LookAtPos.position);
		}
	}
}
