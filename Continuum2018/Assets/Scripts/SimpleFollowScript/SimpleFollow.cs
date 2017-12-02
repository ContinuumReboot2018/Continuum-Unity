using System.Collections;
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

	public Vector2 PosBoundsX, PosBoundsY, PosBoundsZ;

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
					Mathf.Clamp (Mathf.Lerp (transform.position.x, FollowPosX.position.x, FollowPosSmoothTime.x * Time.deltaTime), PosBoundsX.x, PosBoundsX.y),
					Mathf.Clamp (Mathf.Lerp (transform.position.y, FollowPosY.position.y, FollowPosSmoothTime.y * Time.deltaTime), PosBoundsY.x, PosBoundsY.y),
					Mathf.Clamp (Mathf.Lerp (transform.position.z, FollowPosZ.position.z, FollowPosSmoothTime.z * Time.deltaTime), PosBoundsZ.x, PosBoundsZ.y)
				);
		}

		if (FollowPosMethod == followPosMethod.SmoothDamp) 
		{
			transform.position = new Vector3 
				(
					Mathf.Clamp(Mathf.SmoothDamp (transform.position.x, FollowPosX.position.x, ref FollowPosVelX, FollowPosSmoothTime.x * Time.deltaTime), PosBoundsX.x, PosBoundsX.y),
					Mathf.Clamp(Mathf.SmoothDamp (transform.position.y, FollowPosY.position.y, ref FollowPosVelY, FollowPosSmoothTime.y * Time.deltaTime), PosBoundsY.x, PosBoundsY.y),
					Mathf.Clamp(Mathf.SmoothDamp (transform.position.z, FollowPosZ.position.z, ref FollowPosVelZ, FollowPosSmoothTime.z * Time.deltaTime), PosBoundsZ.x, PosBoundsZ.y)
				);
		}
	}

	void CheckFollowPosTransforms ()
	{
		if (FollowPosX.transform == null) 
		{
			FollowPosX = this.transform;
		}

		if (FollowPosY.transform == null) 
		{
			FollowPosY = this.transform;
		}

		if (FollowPosZ.transform == null) 
		{
			FollowPosZ = this.transform;
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
