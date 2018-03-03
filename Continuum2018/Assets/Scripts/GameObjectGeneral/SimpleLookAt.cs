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

	void LateUpdate ()
	{
		// Look towards.
		if (LookMethod == lookType.LookTowards && LookAtPos != null) 
		{
			transform.LookAt (LookAtPos.position, Vector3.forward);
		}

		// Look away.
		if (LookMethod == lookType.LookAway) 
		{
			transform.LookAt (LookAtPos.position, -Vector3.forward);
		}
	}
}