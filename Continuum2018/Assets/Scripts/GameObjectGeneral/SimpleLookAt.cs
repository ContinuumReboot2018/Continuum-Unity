using UnityEngine;

public class SimpleLookAt : MonoBehaviour 
{
	public Transform LookAtPos; // Position to look at.
	public lookType LookMethod; // How to look at the Transform.
	public enum lookType
	{
		LookTowards,
		LookAway
	}

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