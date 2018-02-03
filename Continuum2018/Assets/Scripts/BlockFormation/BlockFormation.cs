using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockFormation : MonoBehaviour 
{
	private Rigidbody rb;
	public GameController gameControllerScript;

	[Header ("Stats")]
	public float speed;
	public float AccumulatedSpeed;
	public int Rows;
	public int Columns;
	public Block[] BlockElements;
	public Vector2 MissingBlocksRange;
	public int missingBlocks;

	void Start () 
	{
		missingBlocks = Random.Range (Mathf.RoundToInt(MissingBlocksRange.x), Mathf.RoundToInt(MissingBlocksRange.y));
		CheckMissingBlocks ();

		gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		SetSpawnPosition ();

		GetAccumulatedSpeed ();
		rb = GetComponent<Rigidbody> ();
		rb.velocity = new Vector3 (0, speed * Time.fixedUnscaledDeltaTime * Time.timeScale, 0);

		InvokeRepeating ("CheckForChildObjects", 0, 1);
	}

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

	void CheckMissingBlocks ()
	{
		switch (missingBlocks)
		{
		case 0:
			break;
		case 1:
			BlockElements [Random.Range (0, BlockElements.Length)].gameObject.SetActive (false);
			break;
		case 2:
			BlockElements [Random.Range (0, BlockElements.Length)].gameObject.SetActive (false);
			BlockElements [Random.Range (0, BlockElements.Length)].gameObject.SetActive (false);
			break;
		case 3:
			BlockElements [Random.Range (0, BlockElements.Length)].gameObject.SetActive (false);
			BlockElements [Random.Range (0, BlockElements.Length)].gameObject.SetActive (false);
			BlockElements [Random.Range (0, BlockElements.Length)].gameObject.SetActive (false);
			break;
		case 4:
			BlockElements [Random.Range (0, BlockElements.Length)].gameObject.SetActive (false);
			BlockElements [Random.Range (0, BlockElements.Length)].gameObject.SetActive (false);
			BlockElements [Random.Range (0, BlockElements.Length)].gameObject.SetActive (false);
			BlockElements [Random.Range (0, BlockElements.Length)].gameObject.SetActive (false);
			break;
		}
	}

	void CheckForChildObjects ()
	{
		if (transform.childCount == 0) 
		{
			Destroy (gameObject);
		}
	}
}
