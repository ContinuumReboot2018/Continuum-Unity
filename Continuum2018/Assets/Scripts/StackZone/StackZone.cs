using UnityEngine;

public class StackZone : MonoBehaviour 
{
	[Tooltip ("Is the current state of this stackzone occupied?")]
	public bool isOccupied;
	[Tooltip ("Can a block occupy here?")]
	public bool canOccupy;
	[Tooltip ("The stack zone below.")]
	public StackZone StackZoneBelow;
	[Tooltip ("The stack zone above.")]
	public StackZone StackZoneAbove;
	[Tooltip ("The currently captured block.")]
	public GameObject CapturedBlock;
	[Tooltip ("The sound to play when a block stacks.")]
	public AudioSource stackSound;

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
		Block CapturedBlockScript = CapturedBlock.GetComponent<Block> ();
		SimpleFollow CapturedBlockSimpleFollow = CapturedBlock.GetComponent<SimpleFollow> ();
		Rigidbody CapturedBlockRigidbody = CapturedBlock.GetComponent<Rigidbody> ();
		ParentToTransform CapturedBlockParentToTransform = CapturedBlock.GetComponent<ParentToTransform> ();

		BlockFormation CapturedBlockParentBlockFormation = CapturedBlock.GetComponentInParent<BlockFormation> ();
		Rigidbody CapturedBlockParentRigidbody = CapturedBlock.GetComponentInParent<Rigidbody> ();

		if (CapturedBlockScript.isBossPart == false)
		{
			CapturedBlockRigidbody.isKinematic = false;
			CapturedBlockScript.OverwriteVelocity = true;
			CapturedBlockScript.isStacked = true;
			CapturedBlockScript.stack = this;
			CapturedBlockRigidbody.velocity = Vector3.zero;
			CapturedBlockSimpleFollow.enabled = true;
			CapturedBlockSimpleFollow.FollowPosX = transform;
			CapturedBlockSimpleFollow.FollowPosY = transform;
			CapturedBlockSimpleFollow.FollowPosZ = transform;
			isOccupied = true;

			if (stackSound.isPlaying == false)
			{
				stackSound.Play ();
			}

			if (CapturedBlockParentBlockFormation != null &&
				CapturedBlockParentRigidbody != null) 
			{
				CapturedBlockParentRigidbody.velocity = Vector3.zero;
				//CapturedBlockParentBlockFormation.enabled = false;
				CapturedBlockParentToTransform.ParentNow ();
			}
		}
	}

	// Release the captured block.
	public void VacateBlock ()
	{
		Block CapturedBlock_BlockScript = CapturedBlock.GetComponent<Block> ();
		Rigidbody CapturedBlock_Rigidbody = CapturedBlock.GetComponent<Rigidbody> ();
		SimpleFollow CapturedBlock_SimpleFollow = CapturedBlock.GetComponent<SimpleFollow> ();

		CapturedBlock_BlockScript.stack = null;
		CapturedBlock_BlockScript.isStacked = false;
		CapturedBlock_Rigidbody.isKinematic = false;
		CapturedBlock_BlockScript.OverwriteVelocity = false;
		CapturedBlock_SimpleFollow.enabled = false;
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
				if (StackZoneBelow != null && 
					StackZoneAbove != null) 
				{
					// Zone above is not occupied but also has a captured block for some reason.
					if (StackZoneAbove.isOccupied == false && 
						StackZoneAbove.CapturedBlock != null) 
					{
						StackZoneAbove.VacateBlock ();
					}

					// Zone above is occupied or has a captured block for some reason.
					if (StackZoneAbove.isOccupied == true || 
						StackZoneAbove.CapturedBlock != null) 
					{
						//StackZoneAbove.VacateBlock ();
					}

					// Zone above is occuped and somehow there is no captured block.
					if (StackZoneAbove.isOccupied == true && 
						StackZoneAbove.CapturedBlock == null) 
					{
						StackZoneAbove.isOccupied = false;
						canOccupy = true;
					}
				}
			}
		}
	}
}