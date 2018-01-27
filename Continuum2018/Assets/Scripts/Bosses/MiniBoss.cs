using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : MonoBehaviour
{
	public GameController gameControllerScript;
	public GameObject Brain;
	public GameObject MiniBossParent;

	[Header ("Stats")]
	public int hitPoints = 5;
	public int StartingHitPoints;

	public GameObject SmallExplosion;
	public GameObject LargeExplosion;

	public Transform FollowPlayerPos;
	public Transform PlayerPos;
	public SimpleFollow simpleFollowScript;
	public SimpleLookAt BrainLookScript;

	[Header ("Boss Parts")]
	public GameObject BossPartsParent;
	public Block[] BossParts;

	[Header ("Shooting")]
	public bool AllowShoot;
	public float FireRate;
	private float NextFire;
	public Vector2 FireRateRange;
	public GameObject Missile;

	public LineRenderer Line;
	public float LineTimerOffDuration = 4;
	public float LineTimerOnDuration = 3;

	void Awake ()
	{
		if (GameObject.Find ("PlayerFollow").transform != null)
		{
			FollowPlayerPos = GameObject.Find ("PlayerFollow").transform;
		}

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
		if (GameObject.Find ("PlayerCollider").transform != null) 
		{
			PlayerPos = GameObject.Find ("PlayerCollider").transform;
		}
			
		if (gameControllerScript == null) 
		{
			gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		}

		BossParts = GetComponentsInChildren<Block> (true);
		hitPoints = StartingHitPoints + gameControllerScript.Wave;

		StartCoroutine (DrawLineToPlayer ());
	}

	void Update ()
	{
		if (Line.enabled == true) 
		{
			Line.SetPosition (0, transform.position + new Vector3 (0, 0, 0.36f));
			Line.SetPosition (1, FollowPlayerPos.position);
			Shoot ();
		}

		Debug.DrawLine (transform.position, FollowPlayerPos.position, Color.green);
	}

	void Shoot ()
	{
		if (Time.time > NextFire && AllowShoot == true) 
		{
			GameObject missile = Instantiate (Missile, transform.position, Quaternion.identity);
			missile.transform.LookAt (PlayerPos);
			FireRate = Random.Range (FireRateRange.x, FireRateRange.y);
			NextFire = Time.time + FireRate;
		}
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

	void OnParticleCollision (GameObject col)
	{
		if (col.tag == "Bullet") 
		{
			if (hitPoints > 1) 
			{
				// Refreshes array of block components in children.
				//BossParts = GetComponentsInChildren<Block> ();
				hitPoints -= 1;
				//Instantiate (SmallExplosion, transform.position, transform.rotation);
			}

			if (hitPoints <= 1) 
			{
				hitPoints = 0;
				Instantiate (LargeExplosion, transform.position, transform.rotation);
				BossPartsParent.transform.DetachChildren ();
				BossPartsConvertToNoise ();
				gameControllerScript.StartNewWave ();
				gameControllerScript.IsInWaveTransition = true;
				Destroy (MiniBossParent.gameObject);
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

	IEnumerator DrawLineToPlayer ()
	{
		AllowShoot = false;
		Line.enabled = false;
		yield return new WaitForSeconds (LineTimerOffDuration);
		Line.SetPosition (0, transform.position + new Vector3 (0, 0, 0.36f));
		Line.SetPosition (1, FollowPlayerPos.position);
		Line.enabled = true;
		Line.positionCount = 2;
		yield return new WaitForSeconds (1);
		AllowShoot = true;
		yield return new WaitForSeconds (LineTimerOnDuration);
		StartCoroutine (DrawLineToPlayer ());
	}
}
