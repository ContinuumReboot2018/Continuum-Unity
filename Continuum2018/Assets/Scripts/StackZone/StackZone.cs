using UnityEngine;

public class StackZone : MonoBehaviour 
{
	public bool isOccupied; // Is the current state of this stackzone occupied?
	public bool canOccupy; // Can a block occupy here?
	public StackZone StackZoneBelow; // The stack zone below.
	public StackZone StackZoneAbove; // The stack zone above.
	public GameObject CapturedBlock; // The currently captured block.
	public AudioSource stackSound; //The sound to play when a block stacks.

	void Awake ()
	{
		isOccupied = false; // Set to be not occupied.
	}

	void Start ()
	{
		stackSound = GameObject.Find ("StackSound").GetComponent<AudioSource> (); // Find stack sound.
		InvokeRepeating ("CheckState", 0, 0.25f);
	}
		
	void OnTriggerEnter (Collider other)
	{
		// Trigger was by a block.
		if (other.GetComponent<Collider>().tag == "Block" && 
			other.GetComponent<Block> ().Stackable == true)
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
						// Pass through normally.
						// Destroy (other.gameObject);
						// return.
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

	// Block in trigger.
	void OnTriggerStay (Collider other)
	{
		if (other.GetComponent<Collider> ().tag == "Block" && 
			other.GetComponent<Block>().Stackable == true) 
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

	// Set to current captured block in trigger.
	public void CaptureBlock ()
	{
		if (CapturedBlock.GetComponent<Block> ().isBossPart == false)
		{
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

	// Release the captured block.
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