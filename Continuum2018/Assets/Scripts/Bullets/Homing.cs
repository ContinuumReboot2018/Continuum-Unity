using UnityEngine;

[RequireComponent(typeof (Rigidbody))]
public class Homing : MonoBehaviour
{
	[Tooltip ("Object to follow.")]
	public Transform target;
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

	//[SerializeField]
	private Vector2 direction;
	//[SerializeField]
	private Vector3 rotateAmount;

	public GameObject[] gos;

	void Start () 
	{
		rb = GetComponent<Rigidbody> ();
		Invoke ("GetObjectToHome", 0);
	}

	void GetObjectToHome ()
	{
		GameObject CheckTagObjectBrain = FindClosestEnemyTag ("Brain"); // Find the closest brain to home in on.

		// Found an object to home in on.
		if (CheckTagObjectBrain != null) 
		{
			target = CheckTagObjectBrain.transform; // Assign GameObject to target.
			speed = initialSpeed;
		} 

		else 

		{
			// Find the closest block to home in on.
			GameObject CheckTagObjectBlockUnstacked = FindClosestEnemyTag ("Block"); 

			// Found a GameObject with block tag.
			if (CheckTagObjectBlockUnstacked != null) 
			{
				// Has a block component.
				if (CheckTagObjectBlockUnstacked.GetComponent<Block> () != null)
				{
					// Block is not stacked.
					if (CheckTagObjectBlockUnstacked.GetComponent<Block> ().isStacked == false)
					{
						target = CheckTagObjectBlockUnstacked.transform;
						speed = initialSpeed;
					} 

					else // Is stacked
					
					{
						// Find the closest block to home in on.
						GameObject CheckTagObjectBlockStacked = FindClosestEnemyTag ("Block"); 

						// Has a block component.
						if (CheckTagObjectBlockUnstacked.GetComponent<Block> () != null) 
						{
							// Block is not stacked.
							if (CheckTagObjectBlockUnstacked.GetComponent<Block> ().isStacked == true) 
							{
								target = CheckTagObjectBlockStacked.transform;
								speed = initialSpeed;
							} 
						} 

						else 
						
						{
							ReleaseHoming (); // Release and bail out, revert to normal movement.
							Invoke ("GetObjectToHome", 3);
						}
					}
				}
			}
		}
	}

	void Update ()
	{
		if (target != null) 
		{
			Debug.DrawLine (transform.position, target.position, new Color (0.47f, 1.0f, 0.05f));

			if (target.gameObject.activeInHierarchy == false) 
			{
				ReleaseHoming ();
				target = null;
				//Invoke ("GetObjectToHome", 0);
			}
		}
	}

	void FixedUpdate () 
	{
		if (target == null)
		{
			rb.angularVelocity = Vector3.zero;
			speed = initialSpeed;
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
			speed = Mathf.Clamp (speed, 10, initialSpeed);

			if (target.gameObject.activeSelf == false) 
			{
				ReleaseHoming ();
				target = null;
				//Invoke ("GetObjectToHome", 1);
			}
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
				Mathf.Clamp (initialSpeed * Time.fixedUnscaledDeltaTime * (1.5f * Time.timeScale), VelocityLimits.x, VelocityLimits.y), 
				0
			)
		);

		speed = initialSpeed;
	}

	void OnDisable ()
	{
		ReleaseHoming ();
	}

	// Finds closest GameObject with requirements.
	public GameObject FindClosestEnemyTag (string tag)
	{
		// Starts array of GameObjects. 
		//GameObject[] gos; 
		//Candidates = gos;

		//BlockChecker.Instance.BlocksInstanced.ToArray ();

		// Finds GameObjects by tag in whole scene.
		//gos = GameObject.FindGameObjectsWithTag(tag); 

		if (tag == "Block")
		{
			gos = BlockChecker.Instance.BlocksInstanced.ToArray ();
		}

		else
		
		{
			gos = GameObject.FindGameObjectsWithTag (tag); 
		}

		// Reset closest GameObject to null (none);
		GameObject closest = null; 

		// Check for distances to compare starting off at infinity.
		float distance = Mathf.Infinity; 

		// Get current position.
		Vector3 position = transform.position;

		// Loop through found GameObjects.
		foreach (GameObject go in gos)
		{
			if (go.activeSelf == true) 
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
		}

		return closest; // Return best and closest distance GameObject.
	}
}