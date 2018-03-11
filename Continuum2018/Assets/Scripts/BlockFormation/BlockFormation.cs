using UnityEngine;

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

	void Start () 
	{
		GetBlockArray ();
		// Get range of missing blocks.
		missingBlocks = Random.Range (
			Mathf.RoundToInt(MissingBlocksRange.x), 
			Mathf.RoundToInt(MissingBlocksRange.y)
		);

		CheckMissingBlocks (); // Disables blocks that are deemed missing.

		gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		SetSpawnPosition (); // Restricts spawn position.

		timeScaleControllerScript = GameObject.Find ("TimescaleController").GetComponent<TimescaleController> ();

		GetAccumulatedSpeed (); // Get aggregate speed additively.
		rb = GetComponent<Rigidbody> (); // Get the current RigidBody component.

		// Set current velocity.
		rb.velocity = new Vector3 (
			0, 
			speed * Time.fixedUnscaledDeltaTime * Time.timeScale, 
			0
		);

		InvokeRepeating ("CheckForChildObjects", 0, 1); // Destroys parent object if there are no child objects.
	}

	void GetBlockArray ()
	{
		BlockElements = GetComponentsInChildren<Block> ();
	}

	// Adds up all the speeds in the child blocks and calculates average speed.
	void GetAccumulatedSpeed ()
	{
		foreach (Block block in BlockElements) 
		{
			if (block.gameObject.activeSelf == true) 
			{
				AccumulatedSpeed += block.speed;
			}
		}

		speed = AccumulatedSpeed / BlockElements.Length;
	}

	void Update ()
	{
		if (timeScaleControllerScript.isRewinding == false) 
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