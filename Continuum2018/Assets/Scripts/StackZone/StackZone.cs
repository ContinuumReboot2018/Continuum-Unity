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

	void Awake ()
	{
		isOccupied = false;
	}

	void Start ()
	{
		stackSound = GameObject.Find ("StackSound").GetComponent<AudioSource> ();
	}

	void Update ()
	{
		CheckState ();
	}

	void OnTriggerEnter (Collider other)
	{
		// Trigger was by a block.
		if (other.GetComponent<Collider>().tag == "Block" && other.GetComponent<Block> ().Stackable == true)
		{
			// If already occupied.
			if (isOccupied == true) 
			{
				// If a top zone.
				if (StackZoneAbove == null)
				{
					// If zone below is occupied.
					if (StackZoneBelow.isOccupied == true) 
					{
						//Destroy (other.gameObject);
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

	void OnTriggerStay (Collider other)
	{
		if (other.GetComponent<Collider> ().tag == "Block" && other.GetComponent<Block>().Stackable == true) 
		{
			if (StackZoneBelow != null) 
			{
				if (StackZoneBelow.isOccupied == true) 
				{
					if (CapturedBlock == null)
					{
						CapturedBlock = other.gameObject;
						CaptureBlock ();
					}
				}
			}

			if (StackZoneBelow == null) 
			{
				if (CapturedBlock == null) 
				{
					CapturedBlock = other.gameObject;
					CaptureBlock ();
				}
			}
		}
	}

	public void CaptureBlock ()
	{
		if (CapturedBlock.GetComponent<Block> ().isBossPart == false)
		{
			//CapturedBlock.transform.parent = null;
			CapturedBlock.GetComponent<Rigidbody> ().isKinematic = false;
			CapturedBlock.GetComponent<Block> ().OverwriteVelocity = true;
			CapturedBlock.GetComponent<Block> ().isStacked = true;
			CapturedBlock.GetComponent<Block> ().stack = this;
			CapturedBlock.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			CapturedBlock.GetComponent<SimpleFollow> ().enabled = true;
			CapturedBlock.GetComponent<SimpleFollow> ().FollowPosX = transform;
			CapturedBlock.GetComponent<SimpleFollow> ().FollowPosY = transform;
			CapturedBlock.GetComponent<SimpleFollow> ().FollowPosZ = transform;
			isOccupied = true;

			if (stackSound.isPlaying == false)
			{
				stackSound.Play ();
			}

			if (CapturedBlock.GetComponentInParent<BlockFormation> () != null &&
				CapturedBlock.GetComponentInParent<Rigidbody>() != null) 
			{
				CapturedBlock.GetComponentInParent<Rigidbody> ().velocity = Vector3.zero;
				CapturedBlock.GetComponentInParent<BlockFormation> ().enabled = false;
				CapturedBlock.GetComponent<ParentToTransform> ().ParentNow ();
			}
		}
	}

	public void VacateBlock ()
	{
		CapturedBlock.GetComponent<Block> ().stack = null;
		CapturedBlock.GetComponent<Block> ().isStacked = false;
		CapturedBlock.GetComponent<Rigidbody> ().isKinematic = false;
		CapturedBlock.GetComponent<Block> ().OverwriteVelocity = false;
		CapturedBlock.GetComponent<SimpleFollow> ().enabled = false;
		CapturedBlock = null;
		isOccupied = false;
		canOccupy = true;
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

		// If zone is not occupied.
		if (isOccupied == false) 
		{
			// If doesn't have a captured block.
			if (CapturedBlock == null) 
			{
				// For bottom row block.
				if (StackZoneBelow == null) 
				{
					// Allow to occupy.
					canOccupy = true;
				}

				// For all zones above the bottom or below the top.
				if (StackZoneBelow != null && StackZoneAbove != null) 
				{
					// Zone above is not occupied but also has a captured block for some reason.
					if (StackZoneAbove.isOccupied == false && StackZoneAbove.CapturedBlock != null) 
					{
						StackZoneAbove.VacateBlock ();
					}

					// Zone above is occupied or has a captured block for some reason.
					if (StackZoneAbove.isOccupied == true || StackZoneAbove.CapturedBlock != null) 
					{
						//StackZoneAbove.VacateBlock ();
					}

					// Zone above is occuped and somehow there is no captured block.
					if (StackZoneAbove.isOccupied == true && StackZoneAbove.CapturedBlock == null) 
					{
						StackZoneAbove.isOccupied = false;
						canOccupy = true;
					}
				}
			}
		}
	}
}
