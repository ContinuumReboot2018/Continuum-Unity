﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollow : MonoBehaviour 
{
	public bool FollowPosition;
	public Transform FollowPosX;
	public Transform FollowPosY;
	public Transform FollowPosZ;

	public followPosMethod FollowPosMethod;
	public enum followPosMethod
	{
		Lerp,
		SmoothDamp
	}

	private float FollowPosVelX, FollowPosVelY, FollowPosVelZ;
	public Vector3 FollowPosSmoothTime;

	public bool FollowRotation;
	public Transform FollowRotX;
	public Transform FollowRotY;
	public Transform FollowRotZ;

	public Vector3 FollowRotSmoothTime;

	void Update () 
	{
		if (FollowPosition == true) 
		{
			FollowObjectPosition ();
		}

		CheckFollowPosTransforms ();

		if (FollowRotation == true) 
		{
			FollowObjectRotation ();
		}
	}

	void FollowObjectPosition ()
	{
		if (FollowPosMethod == followPosMethod.Lerp) 
		{
			transform.position = new Vector3 
				(
					Mathf.Lerp (transform.position.x, FollowPosX.position.x, FollowPosSmoothTime.x * Time.deltaTime),
					Mathf.Lerp (transform.position.y, FollowPosY.position.y, FollowPosSmoothTime.y * Time.deltaTime),
					Mathf.Lerp (transform.position.z, FollowPosZ.position.z, FollowPosSmoothTime.z * Time.deltaTime)
				);
		}

		if (FollowPosMethod == followPosMethod.SmoothDamp) 
		{
			transform.position = new Vector3 
				(
					Mathf.SmoothDamp (transform.position.x, FollowPosX.position.x, ref FollowPosVelX, FollowPosSmoothTime.x * Time.deltaTime),
					Mathf.SmoothDamp (transform.position.y, FollowPosY.position.y, ref FollowPosVelY, FollowPosSmoothTime.y * Time.deltaTime),
					Mathf.SmoothDamp (transform.position.z, FollowPosZ.position.z, ref FollowPosVelZ, FollowPosSmoothTime.z * Time.deltaTime)
				);
		}
	}

	void CheckFollowPosTransforms ()
	{
		if (FollowPosX == null) 
		{
			FollowPosX = gameObject.transform;
		}

		if (FollowPosY == null) 
		{
			FollowPosY = gameObject.transform;
		}

		if (FollowPosZ == null) 
		{
			FollowPosZ = gameObject.transform;
		}
	}

	void FollowObjectRotation ()
	{
		Vector3 RotationAngle = new Vector3 
			(
				Mathf.LerpAngle (transform.eulerAngles.x, FollowRotX.eulerAngles.x, FollowRotSmoothTime.x * Time.deltaTime),
				Mathf.LerpAngle (transform.eulerAngles.y, FollowRotY.eulerAngles.y, FollowRotSmoothTime.y * Time.deltaTime),
				Mathf.LerpAngle (transform.eulerAngles.z, FollowRotZ.eulerAngles.z, FollowRotSmoothTime.z * Time.deltaTime)
			);

		transform.rotation = Quaternion.Euler(RotationAngle);
	}
}
