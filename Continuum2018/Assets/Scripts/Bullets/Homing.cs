using UnityEngine;

[RequireComponent(typeof (Rigidbody))]
public class Homing : MonoBehaviour
{
	[Tooltip ("Object to follow.")]
	public Transform target;
	[Tooltip ("Object to find string.")]
	public string findObject = "Block";
	[Space(10)]
	[Tooltip ("Speed to follow position.")]
	public float speed = 5.0f;
	[Tooltip ("Initial speed when spawned.")]
	public float initialSpeed = 60;
	[Tooltip ("Speed to follow by rotation.")]
	public float rotateSpeed = 200.0f;
	[Tooltip ("How long to be homing before abandoning.")]
	public float homingTime = 5;
	[Tooltip ("Maximum y position to stop homing.")]
	public float cutoffHeight = 12;
	[Tooltip ("Range to look for homing objects.")]
	public float maxRange = 5;
	[Tooltip ("Rate of increase of homing rotation speed, so it doesn't keep going in circles.")]
	public float RotateSpeedIncreaseRate = 1;
	[Tooltip ("")]
	public Vector2 VelocityLimits = new Vector2 (180, 220);
	private Rigidbody rb; // Reference to current RigidBody.

	[SerializeField]
	private Vector2 direction;
	[SerializeField]
	private Vector3 rotateAmount;

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
			speed = initialSpeed;
		} 

		else 

		{
			ReleaseHoming (); // Release and bail out, revert to normal movement.
		}
	}

	void Update ()
	{
		if (target != null) 
		{
			Debug.DrawLine (transform.position, target.position, new Color (0.47f, 1.0f, 0.05f));
		}
	}

	void FixedUpdate () 
	{
		if (target == null)
		{
			rb.angularVelocity = Vector3.zero;
			return;
		}

		if (target != null) 
		{
			rotateSpeed += RotateSpeedIncreaseRate * Time.deltaTime;
			Vector2 direction = (Vector2)target.position - (Vector2)rb.position;
			direction.Normalize ();
			Vector3 rotateAmount = Vector3.Cross (direction, transform.up);
			rb.angularVelocity = -rotateAmount * rotateSpeed;
			rb.velocity = transform.up * speed;
			speed -= 25 * Time.deltaTime;
			speed = Mathf.Clamp (speed, 10, 1000);
		}
	}
		
	// Bail out homing.
	void ReleaseHoming ()
	{
		target = null; // Reset target.
		rb.angularVelocity = Vector3.zero; // Reset angular velocity.
		rb.velocity = transform.TransformDirection (
			new Vector3 (
				0, 
				Mathf.Clamp (speed * Time.fixedUnscaledDeltaTime * (1.5f * Time.timeScale), VelocityLimits.x, VelocityLimits.y), 
				0
			)
		);
	}

	void OnDisable ()
	{
		ReleaseHoming ();
	}

	// Finds closest GameObject with requirements.
	public GameObject FindClosestEnemyTag ()
	{
		// Starts array of GameObjects. 
		GameObject[] gos; 

		// Finds GameObjects by tag in whole scene.
		gos = GameObject.FindGameObjectsWithTag(findObject); 

		// Reset closest GameObject to null (none);
		GameObject closest = null; 

		// Check for distances to compare starting off at infinity.
		float distance = Mathf.Infinity; 

		// Get current position.
		Vector3 position = transform.position;

		// Loop through found GameObjects.
		foreach (GameObject go in gos)
		{
			// Get difference in distances.
			Vector3 diff = go.transform.position - position; 

			// Get square magnitudes.
			float curDistance = diff.sqrMagnitude;

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