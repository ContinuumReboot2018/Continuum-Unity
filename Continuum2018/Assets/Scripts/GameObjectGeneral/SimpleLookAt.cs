using UnityEngine;

[ExecuteInEditMode]
public class SimpleLookAt : MonoBehaviour 
{
	[Tooltip ("Position to look at.")]
	public Transform LookAtPos;
	[Tooltip ("How to look at the Transform.")]
	public lookType LookMethod;
	public enum lookType
	{
		LookTowards,
		LookAway
	}

	[Tooltip ("Find up direction instead of forward direction.")]
	public bool useUpDirection;

	public bool useSmoothing;
	public float SmoothingAmount;
	public Vector3 Offset;

	void LateUpdate ()
	{
		if (useSmoothing == true) 
		{
			Quaternion lookPos = Quaternion.LookRotation (LookAtPos.position - transform.position - Offset);

			transform.rotation = Quaternion.Slerp (transform.rotation, lookPos, SmoothingAmount * Time.deltaTime);

			return;
		}

		if (useSmoothing == false) 
		{
			// Look towards.
			if (LookMethod == lookType.LookTowards && LookAtPos != null) 
			{
				if (useUpDirection == false) 
				{
					transform.LookAt (LookAtPos.position, Vector3.forward);
				}

				if (useUpDirection == true) 
				{
					transform.LookAt (LookAtPos.position, Vector3.up);
				}
			}

			// Look away.
			if (LookMethod == lookType.LookAway) 
			{
				if (useUpDirection == false) 
				{
					transform.LookAt (LookAtPos.position, -Vector3.forward);
				}

				if (useUpDirection == true) 
				{
					transform.LookAt (LookAtPos.position, -Vector3.up);
				}
			}
		}
	}
}