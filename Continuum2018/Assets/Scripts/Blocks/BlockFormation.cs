using UnityEngine;

[ExecuteInEditMode]
public class BlockFormation : MonoBehaviour 
{
	private GameController gameControllerScript; // Reference to the GameController.

	[Header ("Stats")]
	[Tooltip ("The average speed of the total accumulated speed.")]
	public float speed;
	[Tooltip ("The aggregate speed of all the blocks in the formation.")]
	public float AccumulatedSpeed;
	[Tooltip ("Set amount of rows in the formation.")]
	public int Rows;
	[Tooltip ("Set amount of columns in the formation.")]
	public int Columns;
	[Tooltip ("All Blocks in the formation.")]
	public Block[] BlockElements;
	[Tooltip ("Range of missing blocks to have.")]
	public Vector2 MissingBlocksRange;
	[Tooltip ("How many missing blocks were calculated.")]
	public int missingBlocks;

	[Header ("Formation")]
	[Tooltip ("Drop the texture in this slot in the inspector to read from.")]
	public Texture2D map;
	[Header ("Color texture maps")]
	[Tooltip ("Set prefabs to spawn by color.")]
	public ColorToPrefab[] colorMappings;
	[Space (10)]
	[Header ("Transforms")]
	[Tooltip ("Set where the spawned prefab should parent to.")]
	public Transform ParentTransform;
	[Tooltip ("Spacing multiplier.")]
	public float Spacing = 1.5f;
	[Tooltip ("Scaling amount. (Default is 1.35).")]
	public Vector3 Scaling = new Vector3 (1.35f, 1.35f, 1.35f);
	[Header ("Centering")]
	[Tooltip ("Sets centre point on horizontal axis.")]
	public bool AutomaticallyCenterX;
	[Tooltip ("Sets centre point on vertical axis.")]
	public bool AutomaticallyCenterY;
	[Tooltip ("Offset to center the image. (Can be in decimals).")]
	public Vector2 Center;

	void Start () 
	{
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

		GetAccumulatedSpeed (); // Get aggregate speed additively.

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

	void GetBlockArray ()
	{
		BlockElements = GetComponentsInChildren<Block> ();

		foreach (Block block in BlockElements) 
		{
			if (block.gameObject.activeInHierarchy == false)
			{
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