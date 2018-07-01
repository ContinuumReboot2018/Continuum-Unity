using UnityEngine;

public class ObjectsOnScreen : MonoBehaviour 
{
	public type ObjectType;
	public enum type
	{
		Block,
		Bullet
	}

	void Awake ()
	{
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