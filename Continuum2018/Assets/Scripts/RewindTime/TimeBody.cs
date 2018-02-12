﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBody : MonoBehaviour 
{
	public TimescaleController timeScaleControllerScript;

	List<PointInTime> pointsInTime; // This is where the data is captured of position and rotation;

	public float MaxRecordingTime = 5.0f; // Recording time multiplier.
	public Rigidbody rb; // Use Rigidbody component to turn on isKinematic property.

	public bool isBlock;
	private Block blockScript;

	void Start ()
	{
		timeScaleControllerScript = GameObject.Find ("TimescaleController").GetComponent<TimescaleController> ();

		if (isBlock == true) 
		{
			blockScript = GetComponent<Block> ();
		}

		pointsInTime = new List<PointInTime> ();

		if (GetComponent<Rigidbody> () != null) 
		{
			rb = GetComponent<Rigidbody> ();
		}
	}

	void Update ()
	{
		/*if (Input.GetKeyDown (KeyCode.Return)) 
		{
			StartRewind ();
		}

		if (Input.GetKeyUp (KeyCode.Return)) 
		{
			StopRewind ();
		}*/
	}

	void FixedUpdate ()
	{
		if (timeScaleControllerScript.isRewinding == true) 
		{
			Rewind ();

			if (isBlock == true) 
			{
				if (blockScript.isStacked == true)
				{
					blockScript.stack.VacateBlock ();
					blockScript.isStacked = false;
				}
			}

		} else {
			Record ();
		}

		/*if (isRewinding) 
		{
			Rewind ();
		} 
		else {
			Record ();
		}*/
	}

	void Rewind ()
	{
		if (pointsInTime.Count > 0) {
			PointInTime pointInTime = pointsInTime [0];
			transform.position = pointInTime.position;
			transform.rotation = pointInTime.rotation;
			pointsInTime.RemoveAt (0);
		} else {
			StopRewind ();
		}
	}

	void Record ()
	{
		if (pointsInTime.Count > Mathf.Round (MaxRecordingTime / Time.fixedDeltaTime)) 
		{
			pointsInTime.RemoveAt (pointsInTime.Count - 1);
		}

		pointsInTime.Insert (0, new PointInTime (transform.position, transform.rotation));
	}

	public void StartRewind ()
	{
		if (rb != null) 
		{
			rb.isKinematic = true;
		}
	}

	public void StopRewind ()
	{
		if (rb != null) 
		{
			rb.isKinematic = false;
		}
	}
}