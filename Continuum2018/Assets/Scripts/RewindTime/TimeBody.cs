using System.Collections.Generic;
using UnityEngine;

public class TimeBody : MonoBehaviour 
{
	[Tooltip ("Recording time multiplier.")]
	public float MaxRecordingTime = 5.0f;
	[Tooltip ("Use Rigidbody component to turn on isKinematic property.")]
	public Rigidbody rb;
	[Tooltip ("Set to true if the gameObject is a block.")]
	public bool isBlock;
	private Block blockScript; // The block script.

	[Header ("Points In Time")]
	public List<PointInTime> pointsInTime; // This is where the data is captured of position and rotation;

	void Awake ()
	{
		pointsInTime = new List<PointInTime> (); // Create list for points in time to be recorded in.
	}

	void Start ()
	{
		// Check if block.
		if (isBlock == true) 
		{
			blockScript = GetComponent<Block> ();
		}
			
		// Check for Rigidbody.
		if (GetComponent<Rigidbody> () != null) 
		{
			rb = GetComponent<Rigidbody> ();
		}
	}

	void FixedUpdate ()
	{
		// If the timescale controller's state is on rewinding.
		if (TimescaleController.Instance.isRewinding == true) 
		{
			Rewind (); // Call rewind method.

			// If this is a block.
			if (isBlock == true) 
			{
				// If it is already stacked.
				if (blockScript.isStacked == true)
				{
					blockScript.stack.VacateBlock (); // Vacate itself.
					blockScript.isStacked = false; // Set to not stacked.
				}

				if (transform.position.y > 14) 
				{
					Destroy (gameObject);
				}
			}
		} 

		else 
		
		{
			if (blockScript != null)
			{
				Record (); // Record last point in time.

				/*
				// If the block is not stacked.
				if (blockScript.isStacked == false) 
				{
					Record (); // Record last point in time.
				}*/
			}

			if (isBlock == false) 
			{
				Record (); // Record last point in time.
			}
		}
	}

	// Backtracks position and rotation.
	void Rewind ()
	{
		// If there are points in time recorded.
		if (pointsInTime.Count > 0) 
		{
			PointInTime pointInTime = pointsInTime [0]; // Look the point in time at index 0.
			transform.position = pointInTime.position; // Set position as recorded position.
			transform.rotation = pointInTime.rotation; // Set rotation as recorded rotation.
			pointsInTime.RemoveAt (0); // Remove the recorded point in time.
		} 

		else 
		
		{
			if (TimescaleController.Instance.RewindTimeRemaining <= 0)
			{
				StopRewind (); // Stop backtracking.
			}
		}
	}

	// Records position and rotation in a list of points in time.
	void Record ()
	{
		// If the points in time is bigger than max record time / change in time. 
		// E.g (5.0f / 0.02f) = 250 points in time.
		// Remove first recorded time once the points in time count is exceeded.
		if (pointsInTime.Count > Mathf.Round (MaxRecordingTime / Time.fixedDeltaTime)) 
		{
			pointsInTime.RemoveAt (pointsInTime.Count - 1);
		}

		// Insert a new point in time at index 0.
		pointsInTime.Insert (0, new PointInTime (transform.position, transform.rotation));
	}

	// Turn on rewinding and check if we have a Rigidbody component.
	// If so, set Rigidbody to be Kinematic so physics is bypassed in the rewinding process.
	public void StartRewind ()
	{
		if (rb != null) 
		{
			rb.isKinematic = true;
		}
	}

	// Turn on rewinding and check if we have a Rigidbody component.
	// If so, set Rigidbody to be not Kinematic so Physics is resumed as the rewinding process finishes.
	public void StopRewind ()
	{
		if (rb != null) 
		{
			rb.isKinematic = false;
		}
	}
}