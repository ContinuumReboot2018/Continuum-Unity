using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingObject : MonoBehaviour
{
	public FollowedObject target;
	public float maxSpeed = 2;
	public float maxAngleSpeed = 180f;

	void LateUpdate()
	{
		if (target != null)
		{
			target.MoveFollower(transform, maxSpeed, maxAngleSpeed);
		}
	}
}
