using UnityEngine;
using System.Collections.Generic;

public class GravitationalAttractor : MonoBehaviour 
{
	public float G = 6.674f;
	private Rigidbody rb;
	public static List<GravitationalAttractor> Attractors;

	void OnEnable ()
	{
		rb = GetComponent<Rigidbody> ();

		if (Attractors == null) 
			Attractors = new List<GravitationalAttractor> ();

		Attractors.Add (this);
	}

	void FixedUpdate ()
	{
		foreach (GravitationalAttractor attractor in Attractors)
		{
			Attract (attractor);
		}
	}

	void Attract (GravitationalAttractor objToAttract)
	{
		// F = ((m1 * m2)/d^2) * G
		Rigidbody rbToAttract = objToAttract.rb;
		Vector3 direction = rb.position - rbToAttract.position;
		float distance = direction.magnitude;

		if (distance <= 0) 
		{
			return;
		}

		float forceMagnitude = G * (rb.mass * rbToAttract.mass) / Mathf.Pow (distance, 2);
		Vector3 force = direction.normalized * forceMagnitude;

		rbToAttract.AddForce (force);
	}

	void OnDisable ()
	{
		Attractors.Remove (this);
	}
}