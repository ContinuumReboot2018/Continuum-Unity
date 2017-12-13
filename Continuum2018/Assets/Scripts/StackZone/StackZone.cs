using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackZone : MonoBehaviour 
{
	public bool isOccupied;
	public StackZone StackZoneBelow;
	public StackZone StackZoneAbove;
	public GameObject CapturedBlock;

	void Start () 
	{
		InvokeRepeating ("CheckStackZoneState", 0, 1);
	}

	void Update ()
	{
		if (CapturedBlock == null) 
		{
			isOccupied = false;
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (isOccupied == false) 
		{
			if (other.GetComponent<Collider>().tag == "Block")
			{
				// Any row above the bottom row.
				if (StackZoneBelow != null) 
				{
					if (StackZoneBelow.isOccupied == true) 
					{
						CapturedBlock = other.gameObject;
						CaptureBlock ();
					}
				}

				// Must be a zone on the bottom.
				if (StackZoneBelow == null)
				{
					// Only if the stack zone above is not occupied.
					if (StackZoneAbove.isOccupied == false) 
					{
						CapturedBlock = other.gameObject;
						CaptureBlock ();
					}
				}
			}
		}
	}


	void OnTriggerExit (Collider other)
	{
		if (isOccupied == false) 
		{
			if (other.GetComponent<Collider>().tag == "Block")
			{
				// Any row above the bottom row.
				if (StackZoneBelow != null) 
				{
					if (StackZoneBelow.isOccupied == true) 
					{
						CapturedBlock = other.gameObject;
						CaptureBlock ();
					}
				}

				// Must be a zone on the bottom.
				if (StackZoneBelow == null)
				{
					// Only if the stack zone above is not occupied.
					if (StackZoneAbove.isOccupied == false) 
					{
						CapturedBlock = other.gameObject;
						CaptureBlock ();
					}
				}
			}
		}
	}

	void CaptureBlock ()
	{
		CapturedBlock.GetComponent<Block> ().OverwriteVelocity = true;
		CapturedBlock.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		CapturedBlock.GetComponent<SimpleFollow> ().enabled = true;
		CapturedBlock.GetComponent<SimpleFollow> ().FollowPosX = gameObject.transform;
		CapturedBlock.GetComponent<SimpleFollow> ().FollowPosY = gameObject.transform;
		CapturedBlock.GetComponent<SimpleFollow> ().FollowPosZ = gameObject.transform;
		isOccupied = true;
	}

	void CaptureAboveBlock ()
	{
		CapturedBlock.GetComponent<Block> ().OverwriteVelocity = true;
		CapturedBlock.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		CapturedBlock.GetComponent<SimpleFollow> ().enabled = true;
		CapturedBlock.GetComponent<SimpleFollow> ().FollowPosX = StackZoneAbove.gameObject.transform;
		CapturedBlock.GetComponent<SimpleFollow> ().FollowPosY = StackZoneAbove.gameObject.transform;
		CapturedBlock.GetComponent<SimpleFollow> ().FollowPosZ = StackZoneAbove.gameObject.transform;
		StackZoneAbove.isOccupied = true;
	}

	void OnCollisionExit (Collision col)
	{
		if (col.collider.gameObject == CapturedBlock) 
		{
			VacateBlock ();
		}
	}

	public void VacateBlock ()
	{
		// Allows release of the block if it is still there and can be triggered elsewhere.
		if (CapturedBlock != null) 
		{
			CapturedBlock.GetComponent<Block> ().OverwriteVelocity = false;
			CapturedBlock.GetComponent<SimpleFollow> ().enabled = false;
		}

		isOccupied = false;
	}

	void CheckStackZoneState ()
	{
		if (CapturedBlock == null) 
		{
			VacateBlock ();
		}
	}
}
