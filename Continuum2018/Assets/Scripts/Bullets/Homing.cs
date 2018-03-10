using UnityEngine;

[RequireComponent(typeof (Rigidbody))]
public class Homing : MonoBehaviour
{
	public Transform target; 					// Object to follow.
	public string findObject = "Block"; 		// Object to find string.
	[Space(10)]
	public float speed = 5.0f; 					// Speed to follow position.
	public float rotateSpeed = 200.0f; 			// Speed to follow by rotation.
	public float homingTime = 5; 				// How long to be homing before abandoning.
	public float cutoffHeight = 12;				// Maximum y position to stop homing.
	public float maxRange = 5; 					// Range to look for homing objects.
	public float RotateSpeedIncreaseRate = 1; 	// Rate of increase of homing rotation speed, so it doesn't keep going in circles.
	public Vector2 VelocityLimits = 
		new Vector2 (180, 220);
	private Rigidbody rb; 						// Reference to current RigidBody.

	[SerializeField]
	private Vector2 direction;
	[SerializeField]
	private float rotateAmount;

	void Start () 
	{
		rb = GetComponent<Rigidbody> ();
		Invoke ("ReleaseHoming", homingTime); // Prepeare homing to release by homing time.
		InvokeRepeating ("GetObjectToHome", 0, 1);
	}

	void GetObjectToHome ()
	{
		GameObject CheckTagObject = 
			FindClosestEnemyTag (); // Find the closest object to home in on.

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
		// Target has been assigned.
		if (target != null)
		{
			rotateSpeed += RotateSpeedIncreaseRate * Time.deltaTime;
		
			// Find direction homing needs to face.
			direction = (Vector2)target.position - (Vector2)rb.position;
			direction.Normalize (); // Normalise vector.
			rotateAmount = Vector3.Cross (direction, transform.up).z; // Calculate rotation axis.

			//rb.position = Vector3.MoveTowards (rb.position, target.position, 50 * Time.deltaTime);
			//rb.transform.LookAt (target.position);

			rb.angularVelocity = new Vector3 (
				0,
				-rotateAmount * rotateSpeed,
				0
			);

			//rb.velocity =
				
				//transform.up * speed * Time.fixedUnscaledDeltaTime * (1.5f * Time.timeScale)

				//; // Set movement.
					

			rb.velocity = transform.TransformDirection (
				new Vector3 (
					0, 
					Mathf.Clamp (
						speed * Time.fixedUnscaledDeltaTime * (1.5f * Time.timeScale), 
						VelocityLimits.x, 
						VelocityLimits.y
					), 
					0
				)
			);

			Debug.DrawLine (transform.position, target.position, Color.green);
		}

		// No target, revert to normal movement.
		if (target == null)
		{
			rotateSpeed = 0;

			rb.velocity = transform.TransformDirection (
				new Vector3 (
					0, 
					Mathf.Clamp (
						speed * Time.fixedUnscaledDeltaTime * (1.5f * Time.timeScale), 
						VelocityLimits.x, 
						VelocityLimits.y
					), 
					0
				)
			);
		}
	}

	// Bail out homing.
	void ReleaseHoming ()
	{
		target = null; // Reset target.
		rb.angularVelocity = Vector3.zero; // Reset angular velocity.
		//rb.velocity = transform.up * speed; // Resume movmement.
		rb.velocity = transform.TransformDirection (
			new Vector3 (
				0, 
				Mathf.Clamp (speed * Time.fixedUnscaledDeltaTime * (1.5f * Time.timeScale), VelocityLimits.x, VelocityLimits.y), 
				0
			)
		);
	}

	// Finds closest GameObject with requirements.
	public GameObject FindClosestEnemyTag ()
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