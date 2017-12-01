using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollow : MonoBehaviour 
{
	public Transform Follow;
	public bool FollowPosition;
	public followPosMethod FollowPosMethod;
	public enum followPosMethod
	{
		Lerp,
		SmoothDamp
	}
	private float FollowPosVelX, FollowPosVelY, FollowPosVelZ;
	public float FollowPosSmoothTime;

	public bool FollowRotation;

	void Update () 
	{
		if (FollowPosition == true) 
		{
			FollowObjectPosition ();
		}
	}

	void FollowObjectPosition ()
	{
		if (FollowPosMethod == followPosMethod.Lerp) 
		{
			transform.position = new Vector3 
				(
					Mathf.Lerp (transform.position.x, Follow.position.x, FollowPosSmoothTime * Time.deltaTime),
					Mathf.Lerp (transform.position.y, Follow.position.y, FollowPosSmoothTime * Time.deltaTime),
					Mathf.Lerp (transform.position.z, Follow.position.z, FollowPosSmoothTime * Time.deltaTime)
				);
		}

		if (FollowPosMethod == followPosMethod.SmoothDamp) 
		{
			transform.position = new Vector3 
				(
					Mathf.SmoothDamp (transform.position.x, Follow.position.x, ref FollowPosVelX, FollowPosSmoothTime * Time.deltaTime),
					Mathf.SmoothDamp (transform.position.y, Follow.position.y, ref FollowPosVelY, FollowPosSmoothTime * Time.deltaTime),
					Mathf.SmoothDamp (transform.position.z, Follow.position.z, ref FollowPosVelZ, FollowPosSmoothTime * Time.deltaTime)
				);
		}
	}
}
