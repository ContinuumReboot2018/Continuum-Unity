using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovement : MonoBehaviour 
{
	[Header ("Movement")]
	public float speed = 1;
	public InterpolationMode TranslateInterpolation;
	[Space (10)]
	public float rotationSpeed = 50;
	public InterpolationMode RotationInterpolation;
	public Vector2 xBounds;
	public Vector2 yBounds;

	[Header ("Segments")]
	public GameObject bodyPrefab;
	public int startSize;
	public float minDistance = 0.25f;
	public float TThreshold = 0.5f;

	[Space (10)]
	public List<Transform> BodyParts = new List<Transform> ();

	private float distance;
	private Transform curBodyPart;
	private Transform PrevBodyPart;
	private float CurrentVelocity;
	private Vector3 PreviousVelocity;

	public enum InterpolationMode
	{
		Lerp,
		Slerp
	}

	void Start () 
	{
		for (int i = 0; i < startSize - 1; i++) 
		{
			AddBodyPart ();
		}
	}

	void Update () 
	{
		CalculateVelocity ();

		Move ();

		if (Input.GetKeyDown (KeyCode.Q)) 
		{
			AddBodyPart ();
		}
	}

	void CalculateVelocity ()
	{
		CurrentVelocity = ((BodyParts[0].transform.position - PreviousVelocity).magnitude) / Time.deltaTime;
		PreviousVelocity = BodyParts[0].transform.position;
	}

	public void Move ()
	{
		float curspeed = speed;
	
		BodyParts [0].Translate (BodyParts[0].forward * curspeed * Time.smoothDeltaTime , Space.World);

		if (Input.GetAxis ("Horizontal") != 0)
		{
			BodyParts [0].Rotate (Vector3.up * rotationSpeed * Time.deltaTime * Input.GetAxis ("Horizontal"));
		}

		for (int i = 1; i < BodyParts.Count; i++) 
		{
			curBodyPart = BodyParts [i];
			PrevBodyPart = BodyParts [i - 1];

			distance = Vector3.Distance (PrevBodyPart.position, curBodyPart.position);

			Vector3 newPos = PrevBodyPart.position;

			float T = Time.deltaTime * distance / minDistance * CurrentVelocity * curspeed;

			if (T > TThreshold) 
			{
				T = TThreshold;
			}
				
			if (TranslateInterpolation == InterpolationMode.Slerp) 
			{
				curBodyPart.position = Vector3.Slerp (curBodyPart.position, newPos, T);
			}

			if (RotationInterpolation == InterpolationMode.Slerp)
			{
				curBodyPart.rotation = Quaternion.Slerp (curBodyPart.rotation, PrevBodyPart.rotation, T);
			}

			if (TranslateInterpolation == InterpolationMode.Lerp) 
			{
				curBodyPart.position = Vector3.Lerp (curBodyPart.position, newPos, T);
			}

			if (RotationInterpolation == InterpolationMode.Lerp)
			{
				curBodyPart.rotation = Quaternion.Lerp (curBodyPart.rotation, PrevBodyPart.rotation, T);
			}
		}

		CheckBoundary ();
	}

	void CheckBoundary ()
	{
		BodyParts [0].transform.position = new Vector3 (
			Mathf.Clamp (BodyParts [0].transform.position.x, xBounds.x, xBounds.y),
			Mathf.Clamp (BodyParts [0].transform.position.y, yBounds.x, yBounds.y),
			0
		);
	}

	public void AddBodyPart ()
	{
		Transform newpart = (Instantiate (
			bodyPrefab, 
			BodyParts[BodyParts.Count - 1].position, 
			BodyParts[BodyParts.Count - 1].rotation) as GameObject
		).transform;

		newpart.SetParent (transform);
		BodyParts.Add (newpart);
	}
}