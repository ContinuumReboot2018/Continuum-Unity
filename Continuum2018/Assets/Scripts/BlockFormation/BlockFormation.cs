using UnityEngine;

[ExecuteInEditMode]
public class BlockFormation : MonoBehaviour 
{
	private Rigidbody rb; // Reference to the RigidBody.
	private GameController gameControllerScript; // Reference to the GameController.
	private TimescaleController timeScaleControllerScript;

	[Header ("Stats")]
	public float speed; // The average speed of the total accumulated speed.
	public float AccumulatedSpeed; // The aggregate speed of all the blocks in the formation.
	public int Rows; // Set amount of rows in the formation.
	public int Columns; // Set amount of columns in the formation.
	public Block[] BlockElements; // All Blocks in the formation.
	public Vector2 MissingBlocksRange; // Range of missing blocks to have.
	public int missingBlocks; // How many missing blocks were calculated.

	[Header ("Formation")]
	public Texture2D map; // Drop the texture in this slot in the inspector to read from.
	[Header ("Color texture maps")]
	public ColorToPrefab[] colorMappings; // Set prefabs to spawn by color.
	[Space (10)]
	[Header ("Transforms")]
	public Transform ParentTransform; // Set where the spawned prefab should parent to.
	public float Spacing = 1.5f; // Spacing multiplier.
	public Vector3 Scaling = new Vector3 (1.35f, 1.35f, 1.35f); // Scaling amount. (Default is 1.35).
	[Header ("Centering")]
	public bool AutomaticallyCenterX; // Sets centre point on horizontal axis.
	public bool AutomaticallyCenterY; // Sets centre point on vertical axis.
	public Vector2 Center; // Offset to center the image. (Can be in decimals).

	void Start () 
	{
		//GetBlockArray ();
		// Get range of missing blocks.
		missingBlocks = Random.Range (
			Mathf.RoundToInt(MissingBlocksRange.x), 
			Mathf.RoundToInt(MissingBlocksRange.y)
		);
			
		AutoCenterImage (); // Checks if the image should auto center.
		GenerateMiniBossFormation (); // Does the creation of the formation.

		CheckMissingBlocks (); // Disables blocks that are deemed missing.
		GetBlockArray ();

		gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		SetSpawnPosition (); // Restricts spawn position.

		timeScaleControllerScript = GameObject.Find ("TimescaleController").GetComponent<TimescaleController> ();

		GetAccumulatedSpeed (); // Get aggregate speed additively.
		rb = GetComponent<Rigidbody> (); // Get the current RigidBody component.
		/*
		// Set current velocity.
		rb.velocity = new Vector3 (
			0, 
			speed * Time.fixedUnscaledDeltaTime * Time.timeScale, 
			0
		);*/

		InvokeRepeating ("CheckForChildObjects", 0, 1); // Destroys parent object if there are no child objects.
		InvokeRepeating ("GetBlockArray", 3, 1);
	}

	// Read the image then generate the formation.
	void GenerateMiniBossFormation ()
	{
		// Loop through all pixels on each row of pixels.
		for (int x = 0; x < map.width; x++) 
		{
			// Loop through all pizels on each column.
			for (int y = 0; y < map.height; y++) 
			{
				GenerateTile (x, y);
			}
		}
	}

	// Spawns a prefab by color to coordinate.
	void GenerateTile (int x, int y)
	{
		Color pixelColor = map.GetPixel (x, y); // Reads the pixel data.

		if (pixelColor.a == 0) 
		{
			// The pixel is transparent. Let's ignore it.
			return;
		}

		// Found a matching color, spawn the relevant prefab to it at the correct position.
		foreach (ColorToPrefab colorMapping in colorMappings) 
		{
			// We found a matching color in the map.
			if (colorMapping.color.Equals (pixelColor)) 
			{
				// Converts pixel position to unity transform position units.
				Vector2 position = new Vector2 (
					(x * Spacing) - (Center.x * Spacing), 
					(y * Spacing) - (Center.y * Spacing)
				);

				// Creates the relevant prefab at the set position.
				GameObject ColorMapObject = Instantiate (colorMapping.prefab, position, Quaternion.identity, ParentTransform);
				ColorMapObject.GetComponent<Block> ().blockFormationScript = this;
				ColorMapObject.transform.localPosition = new Vector3 (position.x, position.y, 0);
				ColorMapObject.transform.localScale = new Vector3 (Scaling.x, Scaling.y, Scaling.z);

				//ColorMapObject.GetComponent<Block> ().miniBoss = MiniBossBrain;
			}
		}
	}

	// Allow optional centering.
	void AutoCenterImage ()
	{
		// Half of map height doesn't divide equally in two. There is a remainder, 
		// therefore the image height is odd number of pixels.
		if (map.height % 2 != 0) 
		{
			// Automatically center width: Half of map width minus a half.
			// Automatically center height: Half of map height minus a third of the spacing, then round to nearest integer. (It works).
			// Manually center: Use value from Vector2.
			Center = new Vector2 (
				AutomaticallyCenterX ? 0.5f * map.width - 0.5f : Center.x, 
				AutomaticallyCenterY ? Mathf.Round (0.5f * map.height - (0.333334f * Spacing)) : Center.y
			);
		} 

		else
			// The map height is even, divides equally in 2.
		{
			// Automatically center width: Half of map width minus a half.
			// Automatically center height: Half of map height minus a third of the spacing, then round to 2 decimal places. (It works).
			// Manually center: Use value from Vector2.
			Center = new Vector2 (
				AutomaticallyCenterX ? 0.5f * map.width - 0.5f : Center.x, 
				AutomaticallyCenterY ? (float)System.Math.Round (0.5f * map.height - (0.33333f * Spacing), 2) : Center.y
			);
		}
	}

	void Update ()
	{
		/*if (timeScaleControllerScript.isRewinding == false) 
		{
			speed = AccumulatedSpeed / BlockElements.Length;

			// Set current velocity.
			rb.velocity = new Vector3 (
				0, 
				speed * Time.fixedUnscaledDeltaTime * Time.timeScale, 
				0
			);

			return;
		}

		if (timeScaleControllerScript.isRewinding == true) 
		{
			speed = 0;

			// Set current velocity.
			rb.velocity = new Vector3 (
				0, 
				speed * Time.fixedUnscaledDeltaTime * Time.timeScale, 
				0
			);
		} */
	}

	void GetBlockArray ()
	{
		BlockElements = GetComponentsInChildren<Block> ();

		foreach (Block block in BlockElements) 
		{
			if (block.gameObject.activeInHierarchy == false)
			{
				//Destroy (block.transform.parent.gameObject);
				Destroy (block.gameObject);
			}
		}
	}

	// Adds up all the speeds in the child blocks and calculates average speed.
	void GetAccumulatedSpeed ()
	{
		// Get the accumulated speed.
		foreach (Block block in BlockElements) 
		{
			if (block.gameObject.activeSelf == true) 
			{
				AccumulatedSpeed += block.speed;
			}
		}

		// Set speed to reference variable.
		speed = AccumulatedSpeed / BlockElements.Length;

		// Assign new speed to all blocks again.
		foreach (Block block in BlockElements) 
		{
			if (block.gameObject.activeSelf == true) 
			{
				block.speed = speed;
			}
		}
	}
		
	// Sets spawn position based on how many columns there are in the formation.
	void SetSpawnPosition ()
	{
		switch (Columns) 
		{
		case 1:
			int blockPosIdOneColumn = Random.Range (0, gameControllerScript.BlockSpawnXPositions.Length);
			transform.position = new Vector3 (
				gameControllerScript.BlockSpawnXPositions [blockPosIdOneColumn], 
				gameControllerScript.BlockSpawnYPosition, 
				0);
			break;
		case 2:
			int blockPosIdTwoColumns = Random.Range (1, gameControllerScript.BlockSpawnXPositions.Length - 1);
			transform.position = new Vector3 (
				gameControllerScript.BlockSpawnXPositions [blockPosIdTwoColumns], 
				gameControllerScript.BlockSpawnYPosition, 
				0);
			break;
		case 3:
			int blockPosIdThreeColumns = Random.Range (2, gameControllerScript.BlockSpawnXPositions.Length - 2);
			transform.position = new Vector3 (
				gameControllerScript.BlockSpawnXPositions [blockPosIdThreeColumns], 
				gameControllerScript.BlockSpawnYPosition, 
				0);
			break;
		case 4:
			int blockPosIdFourColumns = Random.Range (3, gameControllerScript.BlockSpawnXPositions.Length - 3);
			transform.position = new Vector3 (
				gameControllerScript.BlockSpawnXPositions [blockPosIdFourColumns], 
				gameControllerScript.BlockSpawnYPosition, 
				0);
			break;
		}
	}

	// Removes missing blocks randomly.
	void CheckMissingBlocks ()
	{
		switch (missingBlocks)
		{
		case 0:
			break;
		case 1:
			GetBlockArray ();
			BlockElements [Random.Range (0, BlockElements.Length)].gameObject.SetActive (false);
			GetBlockArray ();
			break;
		case 2:
			GetBlockArray ();
			BlockElements [Random.Range (0, BlockElements.Length)].gameObject.SetActive (false);
			GetBlockArray ();
			BlockElements [Random.Range (0, BlockElements.Length)].gameObject.SetActive (false);
			GetBlockArray ();
			break;
		case 3:
			GetBlockArray ();
			BlockElements [Random.Range (0, BlockElements.Length)].gameObject.SetActive (false);
			GetBlockArray ();
			BlockElements [Random.Range (0, BlockElements.Length)].gameObject.SetActive (false);
			GetBlockArray ();
			BlockElements [Random.Range (0, BlockElements.Length)].gameObject.SetActive (false);
			GetBlockArray ();
			break;
		case 4:
			GetBlockArray ();
			BlockElements [Random.Range (0, BlockElements.Length)].gameObject.SetActive (false);
			GetBlockArray ();
			BlockElements [Random.Range (0, BlockElements.Length)].gameObject.SetActive (false);
			GetBlockArray ();
			BlockElements [Random.Range (0, BlockElements.Length)].gameObject.SetActive (false);
			GetBlockArray ();
			BlockElements [Random.Range (0, BlockElements.Length)].gameObject.SetActive (false);
			GetBlockArray ();
			break;
		}
			
		// Destroys the objects that were set inactive.
		foreach (Block blockElement in BlockElements) 
		{
			if (blockElement.gameObject.activeSelf == false) 
			{
				Destroy (gameObject);
				return;
			}
		}
	}

	// Destroys parent object if there no child objects.
	void CheckForChildObjects ()
	{
		if (transform.childCount == 0) 
		{
			Destroy (gameObject);
			return;
		}
	}
}