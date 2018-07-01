using UnityEngine;
using System.Collections.Generic;

public class BlockChecker : MonoBehaviour 
{
	public static BlockChecker Instance;

	public List<GameObject> BlocksInstanced;

	void Awake ()
	{
		Instance = this;
	}
}