using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Rigidbody))]
public class Homing : MonoBehaviour
{
	public Transform target;
	public string findObject = "Block";
	public float speed = 5.0f;
	public float rotateSpeed = 200.0f;
	public float homingTime = 5;

	private Rigidbody rb;

	void Start () 
	{
		Invoke ("ReleaseHoming", homingTime);

		GameObject CheckTagObject = FindClosestEnemyTag ();

		if (CheckTagObject != null) 
		{
			target = FindClosestEnemyTag ().transform;
		}

		rb = GetComponent<Rigidbody> ();
	}

	void FixedUpdate () 
	{
		if (target)
		{
			try
			{
				if (target != null)
				{
					Vector2 direction = (Vector2)target.position - (Vector2)rb.position;
					direction.Normalize ();
					Vector3 rotateAmount = Vector3.Cross (direction, transform.up);
					rb.angularVelocity = -rotateAmount * rotateSpeed;
					rb.velocity = transform.up * speed;
				} 

				else 
				
				{
					rb.velocity = transform.up * speed;
				}
			}

			catch (MissingReferenceException)
			{
				Debug.Log ("Found a missing reference, setting target to null.");
				target = null;
				return;
			}
		}
	}

	void ReleaseHoming ()
	{
		target = null;
		rb.angularVelocity = Vector3.zero;
		rb.velocity = transform.up * speed;
	}

	public GameObject FindClosestEnemyTag()
	{
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag(findObject);
		GameObject closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = transform.position;
		foreach (GameObject go in gos)
		{
			Vector3 diff = go.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance)
			{
				closest = go;
				distance = curDistance;
			}
		}
		return closest;
	}
}
