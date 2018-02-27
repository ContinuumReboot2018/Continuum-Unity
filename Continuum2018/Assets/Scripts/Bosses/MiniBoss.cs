﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : MonoBehaviour
{
	public GameController gameControllerScript;
	public TimescaleController timeScaleControllerScript;
	public GameObject Brain;
	public GameObject MiniBossParent;

	[Header ("Stats")]
	public float hitPoints = 5;
	public int StartingHitPoints;
	public float ParticleHitPointAmount = 0.01f;

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

	[Header ("Effects")]
	public Animator FlipScreen;

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

		if (GameObject.Find ("PlayerCollider").transform == null) 
		{
			BossPartsConvertToNoise ();
			hitPoints = 0;
			Instantiate (LargeExplosion, transform.position, transform.rotation);
			gameControllerScript.StartNewWave ();
			gameControllerScript.IsInWaveTransition = true;
			Destroy (MiniBossParent.gameObject);
		}
			
		if (gameControllerScript == null) 
		{
			gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		}

		if (timeScaleControllerScript == null) 
		{
			timeScaleControllerScript = GameObject.Find ("TimescaleController").GetComponent<TimescaleController> ();
		}

		Invoke ("GetBossParts", 0.5f);
		hitPoints = StartingHitPoints + gameControllerScript.Wave;

		StartCoroutine (DrawLineToPlayer ());

		FlipScreen = GameObject.Find ("Camera Rig").GetComponent<Animator>();
	}

	void GetBossParts ()
	{
		BossParts = BossPartsParent.GetComponentsInChildren<Block> (true);
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
				if (hitPoints <= 1) 
				{
					BossPartsConvertToNoise ();
					hitPoints = 0;
					Instantiate (LargeExplosion, transform.position, transform.rotation);
					gameControllerScript.StartNewWave ();
					gameControllerScript.IsInWaveTransition = true;
					timeScaleControllerScript.OverrideTimeScaleTimeRemaining = 1f;
					timeScaleControllerScript.OverridingTimeScale = 0.2f;

					if (FlipScreen.GetCurrentAnimatorStateInfo (0).IsName ("CameraRotateUpsideDown") == true) 
					{
						FlipScreen.SetBool ("Flipped", false);
					}

					Destroy (MiniBossParent.gameObject);
					return;
				}

				if (hitPoints > 1) 
				{
					hitPoints -= 1;
					Instantiate (SmallExplosion, transform.position, transform.rotation);
				}
			}
		}
	}

	void OnParticleCollision (GameObject col)
	{
		if (col.tag == "Bullet") 
		{
			if (hitPoints <= 0) 
			{
				BossPartsConvertToNoise ();
				hitPoints = 0;
				Instantiate (LargeExplosion, transform.position, transform.rotation);
				gameControllerScript.StartNewWave ();
				gameControllerScript.IsInWaveTransition = true;
				timeScaleControllerScript.OverrideTimeScaleTimeRemaining = 1f;
				timeScaleControllerScript.OverridingTimeScale = 0.2f;
				Destroy (MiniBossParent.gameObject);
				return;
			}

			if (hitPoints > 0) 
			{
				hitPoints -= 1 * ParticleHitPointAmount;

				if (IsInvoking ("InstanceExplosion") == false) 
				{
					Invoke ("InstanceExplosion", 0.25f);
				}
			}
		}
	}

	void InstanceExplosion ()
	{
		Instantiate (SmallExplosion, transform.position, transform.rotation);
	}

	void BossPartsConvertToNoise ()
	{
		/*for (int i = 0; i < BossParts.Length; i++) 
		{
			if (BossParts[i] != null)
			{
				BossParts[i].ConvertToNoiseBossPart ();
				BossParts[i].gameObject.GetComponent<ParentToTransform> ().ParentNow ();
			}
		}*/


		foreach (Block block in BossParts)
		{
			if (block != null)
			{
				block.ConvertToNoiseBossPart ();
				block.parentToTransformScript.ParentNow ();
			}
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