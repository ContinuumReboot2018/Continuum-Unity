using UnityEngine;

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

	void LateUpdate ()
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