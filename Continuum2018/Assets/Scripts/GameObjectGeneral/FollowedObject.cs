using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowedObject : MonoBehaviour {

	// Use this for initialization
	void Start()
	{

	}

	public class Frame
	{
		public Vector3 position;
		public Quaternion rotation;
	}

	public List<Frame> formerPositions = new List<Frame>();
	public int frameDelay = 30;
	public float minimumDistance = 1;
	Vector3 formerPosition = Vector3.zero;

	// Update is called once per frame
	void LateUpdate()
	{
		if (formerPosition != transform.position)
		{
			formerPositions.Insert(0, new Frame { position=  transform.position, rotation =  transform.rotation});
			if (formerPositions.Count > frameDelay + 2)
				formerPositions.RemoveAt(formerPositions.Count - 1);
		}
		formerPosition = transform.position;
	}

	public void MoveFollower(Transform target, float maxSpeed = 1f, float maxAngleSpeed = 180f)
	{
		if (formerPositions.Count > frameDelay )
		{
			var frame = formerPositions[frameDelay];
			//You could project for the minimum distance here, I will just check
			if((target.position - frame.position).magnitude < minimumDistance)
				return;
			target.position = Vector3.MoveTowards(target.position, frame.position, maxSpeed * Time.deltaTime);
			//target.rotation = Quaternion.RotateTowards(target.rotation, frame.rotation, maxAngleSpeed * Time.deltaTime);
		}
	}



}

