using UnityEngine;

[RequireComponent(typeof (Rigidbody))]
public class Homing : MonoBehaviour
{
	public Transform target; // Object to follow.
	public string findObject = "Block"; // Object to find string.
	[Space(10)]
	public float speed = 5.0f; // Speed to follow position.
	public float rotateSpeed = 200.0f; // Speed to follow by rotation.
	public float homingTime = 5; // How long to be homing before abandoning.
	public float cutoffHeight = 12; // Maximum y position to stop homing.
	public float maxRange = 5; // Range to look for homing objects.
	public float RotateSpeedIncreaseRate = 1; // Rate of increase of homing rotation speed, so it doesn't keep going in circles.

	private Rigidbody rb; // Reference to current RigidBody.

	void Start () 
	{
		rb = GetComponent<Rigidbody> ();
		Invoke ("ReleaseHoming", homingTime); // Prepeare homing to release by homing time.

		GameObject CheckTagObject = FindClosestEnemyTag (); // Find the closest object to home in on.

		// Found an object to home in on.
		if (CheckTagObject != null) 
		{
			target = FindClosestEnemyTag ().transform; // Assign GameObject to target.
		} 

		else 
		
		{
			ReleaseHoming (); // Release and bail out, revert to normal movement.
		}
	}

	void FixedUpdate () 
	{
		rotateSpeed += RotateSpeedIncreaseRate * Time.deltaTime;

		// Target has been assigned.
		if (target)
		{
			try
			{
				// Find direction homing needs to face.
				Vector2 direction = (Vector2)target.position - (Vector2)rb.position;
				direction.Normalize (); // Normalise vector.
				Vector3 rotateAmount = Vector3.Cross (direction, transform.up); // Calculate rotation axis.
				rb.angularVelocity = -rotateAmount * rotateSpeed; // Set angular velocity.
				rb.velocity = transform.up * speed; // Set movement.
			}

			catch (MissingReferenceException)
			{
				Debug.Log ("Found a missing reference, setting target to null.");
				target = null;
				return;
			}
		}
			
		else 

		// No target, revert to normal movement.
		{
			rb.velocity = transform.up * speed;
		}
	}

	// Bail out homing.
	void ReleaseHoming ()
	{
		target = null; // Reset target.
		rb.angularVelocity = Vector3.zero; // Reset angular velocity.
		rb.velocity = transform.up * speed; // Resume movmement.
	}

	// Finds closest GameObject with requirements.
	public GameObject FindClosestEnemyTag()
	{
		GameObject[] gos; // Starts array of GameObjects. 
		gos = GameObject.FindGameObjectsWithTag(findObject); // Finds GameObjects by tag in whole scene.
		GameObject closest = null; // Reset closest GameObject to null (none);
		float distance = Mathf.Infinity; // Check for distances to compare starting off at infinity.
		Vector3 position = transform.position; // Get current position.

		// Loop through found GameObjects.
		foreach (GameObject go in gos)
		{
			Vector3 diff = go.transform.position - position; // Get difference in distances.
			float curDistance = diff.sqrMagnitude; //Get square magnitudes.

			// curDistance must be less than best distance to advance.
			// Obect's position myst be under the cutoff height.
			// Diff magnitude squared must be less than max range squared.
			if (curDistance < distance && 
				go.transform.position.y < cutoffHeight &&
				diff.sqrMagnitude < (maxRange * maxRange))
			{
				closest = go; // Assign to closest object found.
				distance = curDistance; // Set best distance.
			}
		}

		return closest; // Return best and closest distance GameObject.
	}
}