using UnityEngine;

public class Attractor : MonoBehaviour 
{
	public Transform MasterAttractor;
	public Block blockScript;
	public SimpleFollow simpleFollowScript;
	public bool OverrideMovement;
	public Vector3 OverrideSmoothing = new Vector3 (1, 1, 0);

	void Start () 
	{
		MasterAttractor = GameObject.Find ("MasterAttractor").transform;
		blockScript = GetComponent<Block> ();
		simpleFollowScript = GetComponent<SimpleFollow> ();
	}

	void FixedUpdate () 
	{
		if (OverrideMovement == true) 
		{
			if (blockScript != null && simpleFollowScript != null) 
			{
				bool overrideblock = false;

				if (overrideblock == false) 
				{
					simpleFollowScript.FollowPosSmoothTime = OverrideSmoothing;
					simpleFollowScript.enabled = true;
					blockScript.speed = 0;
					blockScript.OverwriteVelocity = true;
					blockScript.GetComponent<Rigidbody> ().isKinematic = true;
					overrideblock = true;
				}

				if (blockScript.isStacked == true && blockScript.stack != null) 
				{
					blockScript.stack.VacateBlock ();
				}

				if (simpleFollowScript.OverrideTransform != MasterAttractor)
				{
					simpleFollowScript.OverrideTransform = MasterAttractor;
				}
			}
		}
	}
}