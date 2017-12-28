using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : MonoBehaviour
{
	public GameController gameControllerScript;
	public GameObject Brain;
	public int StartingHitPoints;
	public int hitPoints = 5;
	public GameObject SmallExplosion;
	public GameObject LargeExplosion;
	public SimpleFollow simpleFollowScript;
	public SimpleLookAt BrainLookScript;
	public GameObject BossPartsParent;
	public Block[] BossParts;
	public Transform FollowPlayerPos;
	public GameObject MiniBossParent;

	void Awake ()
	{
		hitPoints = StartingHitPoints;
		FollowPlayerPos = GameObject.Find ("PlayerFollow").transform;
		BrainLookScript.LookAtPos = FollowPlayerPos.transform;
		simpleFollowScript.FollowPosX = FollowPlayerPos;
		simpleFollowScript.FollowPosY = FollowPlayerPos;
		simpleFollowScript.FollowPosZ = FollowPlayerPos;

		if (Brain == null) 
		{
			Brain = this.gameObject;
		}
	}

	void Start () 
	{
		if (gameControllerScript == null) 
		{
			gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		}

		BossParts = GetComponentsInChildren<Block> (true);
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Bullet") 
		{
			if (other.name.Contains ("P1") || other.name.Contains ("Shield_Col") ||
			    other.GetComponent<Bullet> ().playerControllerScript.PlayerId == 1) 
			{
				if (hitPoints > 1) 
				{
					// Refreshes array of block components in children.
					BossParts = GetComponentsInChildren<Block> ();
					hitPoints -= 1;
					Instantiate (SmallExplosion, transform.position, transform.rotation);
				}

				if (hitPoints <= 1) 
				{
					hitPoints = 0;
					Instantiate (LargeExplosion, transform.position, transform.rotation);
					BossPartsParent.transform.DetachChildren ();
					BossPartsConvertToNoise ();
					gameControllerScript.StartNewWave ();
					Destroy (MiniBossParent.gameObject);
				}
			}
		}
	}

	void BossPartsConvertToNoise ()
	{
		foreach (Block block in BossParts)
		{
			block.isSpecialBlockType = true;
			block.SpecialBlockType = Block.specialBlockType.Noise;
			block.textureScrollScript.enabled = true;
			block.gameObject.GetComponent<ParentToTransform> ().enabled = true;
		}
	}
}
