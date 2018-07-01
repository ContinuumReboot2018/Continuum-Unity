using UnityEngine;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour 
{

	[System.Serializable]
	public class Pool
	{
		public string tag;
		public GameObject prefab;
		public int size;
	}

	public static ObjectPooler instance;

	private void Awake ()
	{
		instance = this;
	}

	public List<Pool> pools;
	public Dictionary<string, Queue<GameObject>> poolDictionary;

	void Start ()
	{
		poolDictionary = new Dictionary <string, Queue<GameObject>> ();

		foreach (Pool pool in pools)
		{
			Queue<GameObject> objectPool = new Queue<GameObject> ();

			for (int i = 0; i < pool.size; i++)
			{
				GameObject obj = Instantiate (pool.prefab);
				obj.SetActive (false);
				objectPool.Enqueue (obj);
			}

			poolDictionary.Add (pool.tag, objectPool);
		}
	}

	public GameObject SpawnFromPool (string tag, Vector3 position, Quaternion rotation)
	{
		if (!poolDictionary.ContainsKey (tag)) 
		{
			Debug.LogWarning ("Pool with tag " + tag + " doesnt exist.");
			return null;
		}

		GameObject objectToSpawn = poolDictionary [tag].Dequeue ();

		objectToSpawn.SetActive (true);
		objectToSpawn.transform.position = position;
		objectToSpawn.transform.rotation = rotation;

		IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject> ();

		if (pooledObj != null) 
		{
			pooledObj.OnObjectSpawn ();
		}

		poolDictionary [tag].Enqueue (objectToSpawn);

		return objectToSpawn;
	}
}