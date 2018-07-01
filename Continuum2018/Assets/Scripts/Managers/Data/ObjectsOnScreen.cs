using UnityEngine;

public class ObjectsOnScreen : MonoBehaviour 
{
	public GameController gameControllerScript;

	public type ObjectType;
	public enum type
	{
		Block,
		Bullet
	}

	void Awake ()
	{
		gameControllerScript = GameObject.Find ("GameController").GetComponent<GameController> ();

		switch (ObjectType) 
		{
		case type.Block:
			GameController.Instance.numberOfBlocks++;
			break;
		case type.Bullet:
			GameController.Instance.numberOfBullets++;
			break;
		}
	}

	void OnDestroy ()
	{
		switch (ObjectType) 
		{
		case type.Block:
			GameController.Instance.numberOfBlocks--;
			break;
		case type.Bullet:
			GameController.Instance.numberOfBullets--;
			break;
		}
	}
}