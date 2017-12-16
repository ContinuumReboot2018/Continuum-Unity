using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackZone : MonoBehaviour 
{
	public bool isOccupied;
	public bool canOccupy;
	public StackZone StackZoneBelow;
	public StackZone StackZoneAbove;
	public GameObject CapturedBlock;
	public AudioSource stackSound;

	void Start ()
	{
		isOccupied = false;
		stackSound = GameObject.Find ("StackSound").GetComponent<AudioSource> ();
	}

	void Update ()
	{
		CheckState ();
	}

	void OnTriggerEnter (Collider other)
	{
		// Trigger was by a block.
		if (other.GetComponent<Collider>().tag == "Block")
		{
			if (isOccupied == true) 
			{
				// If a top zone.
				if (StackZoneAbove == null) 
				{
					// If zone below is occupied.
					if (StackZoneBelow.isOccupied == true) 
					{
						Destroy (other.gameObject);
					}
				}
			}

			// If not occupied at the moment.
			if (isOccupied == false) 
			{
				// For any zones above bottom row.
				if (StackZoneBelow != null) 
				{
					if (StackZoneBelow.isOccupied == true)
					{
						CapturedBlock = other.gameObject;
						CaptureBlock ();
					}
				}

				// For bottom row zones.
				if (StackZoneBelow == null) 
				{
					CapturedBlock = other.gameObject;
					CaptureBlock ();
				}
			}
		}
	}

	void CaptureBlock ()
	{
		CapturedBlock.GetComponent<Block> ().OverwriteVelocity = true;
		CapturedBlock.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		CapturedBlock.GetComponent<SimpleFollow> ().enabled = true;
		CapturedBlock.GetComponent<SimpleFollow> ().FollowPosX = transform;
		CapturedBlock.GetComponent<SimpleFollow> ().FollowPosY = transform;
		CapturedBlock.GetComponent<SimpleFollow> ().FollowPosZ = transform;
		isOccupied = true;
		stackSound.Play ();
	}

	void VacateBlock ()
	{
		CapturedBlock.GetComponent<Block> ().OverwriteVelocity = false;
		CapturedBlock.GetComponent<SimpleFollow> ().enabled = false;
		CapturedBlock = null;
		isOccupied = false;
	}

	void CheckState ()
	{
		// If already occupied.
		if (isOccupied == true) 
		{
			// If there is a zone below.
			if (StackZoneBelow != null) 
			{
				// if zone below is not occupied.
				if (StackZoneBelow.isOccupied == false) 
				{
					if (CapturedBlock != null)
					{
						VacateBlock ();
					}
				}
			}

			// If captured block is missing.
			if (CapturedBlock == null) 
			{
				isOccupied = false;
			}

			if (CapturedBlock != null) 
			{
				// If there is a zone below.
				if (StackZoneBelow != null) 
				{
					// if zone below is not occupied.
					if (StackZoneBelow.isOccupied == false) 
					{
						VacateBlock ();
					}
				}
			}
		}
	}
}
