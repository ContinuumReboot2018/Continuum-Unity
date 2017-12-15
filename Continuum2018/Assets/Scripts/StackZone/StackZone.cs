using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackZone : MonoBehaviour 
{
	public bool isOccupied;
	public StackZone StackZoneBelow;
	public StackZone StackZoneAbove;
	public GameObject CapturedBlock;
	public AudioSource stackSound;

	void Start ()
	{
		stackSound = GameObject.Find ("StackSound").GetComponent<AudioSource> ();
		InvokeRepeating ("CheckStackZoneStateBelow", 0, 0.1f);
	}

	void CheckStackZoneStateBelow ()
	{
		// If there is a stack spot below.
		if (StackZoneBelow != null) 
		{
			if (isOccupied == true && StackZoneBelow.isOccupied == false)
			{
				VacateBlock ();
				isOccupied = false;
			}

			if (CapturedBlock != null && StackZoneBelow.CapturedBlock == null) 
			{
				if (Vector3.Distance (CapturedBlock.transform.position, transform.position) > 0.1f)
				{
					VacateBlock ();
					isOccupied = false;
				}
			}
		}
	}

	void Update ()
	{
		CheckStackZoneState ();

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

	void CaptureBlock ()
	{
		stackSound.Play ();
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
		stackSound.Play ();
		CapturedBlock.GetComponent<Block> ().OverwriteVelocity = true;
		CapturedBlock.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		CapturedBlock.GetComponent<SimpleFollow> ().enabled = true;
		CapturedBlock.GetComponent<SimpleFollow> ().FollowPosX = StackZoneAbove.gameObject.transform;
		CapturedBlock.GetComponent<SimpleFollow> ().FollowPosY = StackZoneAbove.gameObject.transform;
		CapturedBlock.GetComponent<SimpleFollow> ().FollowPosZ = StackZoneAbove.gameObject.transform;
		StackZoneAbove.isOccupied = true;
	}

	void MoveToSpaceBelow ()
	{
		if (StackZoneBelow.isOccupied == false) 
		{
			isOccupied = false;
			StackZoneBelow.CapturedBlock = CapturedBlock;
			CapturedBlock = null;
			StackZoneBelow.isOccupied = true;

			if (StackZoneBelow.CapturedBlock != null) 
			{
				StackZoneBelow.CapturedBlock.GetComponent<SimpleFollow> ().enabled = true;
				StackZoneBelow.CapturedBlock.GetComponent<SimpleFollow> ().FollowPosX = StackZoneBelow.gameObject.transform;
				StackZoneBelow.CapturedBlock.GetComponent<SimpleFollow> ().FollowPosY = StackZoneBelow.gameObject.transform;
				StackZoneBelow.CapturedBlock.GetComponent<SimpleFollow> ().FollowPosZ = StackZoneBelow.gameObject.transform;
			}
		}

		if (StackZoneBelow.CapturedBlock == null)
		{
			isOccupied = false;
			StackZoneBelow.CapturedBlock = CapturedBlock;
			CapturedBlock = null;
			StackZoneBelow.isOccupied = true;
		}

		if (CapturedBlock != null && StackZoneBelow.isOccupied == false) 
		{
			CapturedBlock = null;
			isOccupied = false;
			StackZoneBelow.CapturedBlock.GetComponent<SimpleFollow> ().enabled = true;
			StackZoneBelow.CapturedBlock.GetComponent<SimpleFollow> ().FollowPosX = StackZoneBelow.gameObject.transform;
			StackZoneBelow.CapturedBlock.GetComponent<SimpleFollow> ().FollowPosY = StackZoneBelow.gameObject.transform;
			StackZoneBelow.CapturedBlock.GetComponent<SimpleFollow> ().FollowPosZ = StackZoneBelow.gameObject.transform;
		}
	}

	public void VacateBlock ()
	{
		isOccupied = false;
	}

	void CheckStackZoneState ()
	{
		// If the current zone is empty.
		if (isOccupied == false)
		{
			// Any row between top and bottom
			if (StackZoneAbove != null && StackZoneBelow != null)
			{
				if (StackZoneAbove.isOccupied == true) 
				{
					CapturedBlock = StackZoneAbove.CapturedBlock;
					CaptureBlock ();
					StackZoneAbove.VacateBlock ();
				}
			}

			// For bottom row zones.
			if (StackZoneBelow == null && StackZoneAbove != null) 
			{
				if (StackZoneAbove.isOccupied == true)
				{
					CapturedBlock = StackZoneAbove.CapturedBlock;
					CaptureBlock ();
					StackZoneAbove.VacateBlock ();
				}
			}
		}

		if (isOccupied == true) 
		{
			if (CapturedBlock == null) 
			{
				VacateBlock ();
			}
				
			if (StackZoneAbove == null && StackZoneBelow != null) 
			{
				if (StackZoneBelow.isOccupied == false) 
				{
					VacateBlock ();
					MoveToSpaceBelow ();
				}
			}

			// Any row between top and bottom
			if (StackZoneAbove != null && StackZoneBelow != null)
			{
				if (StackZoneBelow.isOccupied == false) 
				{
					//VacateBlock ();
					MoveToSpaceBelow ();
				}
			}
		}
	}
}
