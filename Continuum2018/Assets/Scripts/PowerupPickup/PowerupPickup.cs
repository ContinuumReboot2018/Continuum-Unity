using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupPickup : MonoBehaviour 
{
	public GameController gameControllerScript;
	public PlayerController playerControllerScript_P1;
	public Texture2D PowerupTexture;
	public bool isShootingPowerup = true;

	[Header ("On Awake")]
	public AudioSource AwakeAudio;
	public MeshRenderer meshrend;
	public SphereCollider col;
	public ParticleSystem AwakeParticles;
	public float AwakeDelay = 1;

	[Header ("Powerup Stats")]
	public powerups ThisPowerup;
	public enum powerups
	{
		DoubleShot,
		TripleShot,
		ExtraLife,
		RippleShot,
		Turret,
		Helix,
		Rapidfire,
		Overdrive,
		Ricochet
	}

	[Header ("On Pickup")]
	public float PowerupTime = 20;
	public GameObject CollectExplosion;
	public GameObject PowerupDeathExplosion;
	public Animator PowerupUI;
	public AudioSource PowerupTimeRunningOutAudio;

	[Header ("On End Life")]
	public Animator anim;
	public DestroyByTime destroyByTimeScript;

	public GameObject Turret;

	void Awake ()
	{
		meshrend.enabled = false;
		col.enabled = false;
		StartCoroutine (ShowPowerup ());
		destroyByTimeScript = GetComponent<DestroyByTime> ();
		StartCoroutine (DestroyAnimation ());
		anim = GetComponent<Animator> ();
	}

	IEnumerator ShowPowerup ()
	{
		yield return new WaitForSeconds (AwakeDelay);
		meshrend.enabled = true;
		col.enabled = true;
		AwakeParticles.Play ();
	}

	IEnumerator DestroyAnimation ()
	{
		yield return new WaitForSecondsRealtime (destroyByTimeScript.delay - 1);
		anim.Play ("PowerupPickupDestroy");
	}

	void Start ()
	{
		PowerupUI = GameObject.Find ("PowerupUI").GetComponent<Animator> ();
		gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		playerControllerScript_P1 = GameObject.Find ("PlayerController_P1").GetComponent<PlayerController> ();
		PowerupTimeRunningOutAudio = GameObject.Find ("PowerupRunningOutSound").GetComponent<AudioSource> ();
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Bullet") 
		{
			if (other.name.Contains ("P1") || other.name.Contains ("Shield_Col") || 
				other.GetComponent<Bullet> ().playerControllerScript.PlayerId == 1) 
			{
				if (IsInvoking ("CheckActivatePowerup") == false) 
				{
					Invoke("CheckActivatePowerup", 0.01667f);
				}
			}
		}

		if (other.tag == "Player") 
		{
			if (other.name.Contains ("P1")) 
			{
				if (IsInvoking ("CheckActivatePowerup") == false) 
				{
					Invoke("CheckActivatePowerup", 0.01667f);
				}
			}
		}

		if (other.tag == "Boundary") 
		{
			Destroy (gameObject);
		}
	}

	void CheckActivatePowerup ()
	{
		ActivatePowerup_P1 ();
		playerControllerScript_P1.Vibrate (0.6f, 0.6f, 0.3f);
		Instantiate (PowerupDeathExplosion, transform.position, Quaternion.identity);
		PowerupTimeRunningOutAudio.Stop ();
		Destroy (gameObject);
	}

	void ActivatePowerup_P1 ()
	{
		Instantiate (CollectExplosion, transform.position, Quaternion.identity);

		switch (ThisPowerup) 
		{
		case powerups.DoubleShot: 

			// Switches to triple shot mode
			playerControllerScript_P1.ShotType = PlayerController.shotType.Double;

			if (playerControllerScript_P1.isInOverdrive == true) 
			{
				playerControllerScript_P1.DoubleShotIteration = PlayerController.shotIteration.Overdrive;
			}

			if (playerControllerScript_P1.isInRapidFire == false) 
			{
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.DoubleShotFireRates [0];
			}

			if (playerControllerScript_P1.isInRapidFire == true) 
			{
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.DoubleShotFireRates [1];
			}

			// Tweaks to conditions based on which iteration the player is on.
			switch (playerControllerScript_P1.DoubleShotIteration) 
			{
			case PlayerController.shotIteration.Standard:
				gameControllerScript.SetPowerupTime (PowerupTime);
				playerControllerScript_P1.ShotType = PlayerController.shotType.Double;
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.DoubleShotFireRates [0];
				break;
			case PlayerController.shotIteration.Enhanced:
				gameControllerScript.SetPowerupTime (PowerupTime);
				break;
			case PlayerController.shotIteration.Rapid:
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.DoubleShotFireRates [1];
				gameControllerScript.SetPowerupTime (PowerupTime);
				break;
			case PlayerController.shotIteration.Overdrive:
				break;
			}

			SetPowerupTexture (0);
			gameControllerScript.PowerupText_P1[0].text = "";
				break;

		case powerups.TripleShot:
			
			// Switches to triple shot mode
			playerControllerScript_P1.ShotType = PlayerController.shotType.Triple;

			if (playerControllerScript_P1.isInOverdrive == true) 
			{
				playerControllerScript_P1.TripleShotIteration = PlayerController.shotIteration.Overdrive;
			}

			if (playerControllerScript_P1.isInRapidFire == false) 
			{
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.TripleShotFireRates [0];
			}

			if (playerControllerScript_P1.isInRapidFire == true) 
			{
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.TripleShotFireRates [1];
			}
				
			// Tweaks to conditions based on which iteration the player is on.
			switch (playerControllerScript_P1.TripleShotIteration) 
			{
			case PlayerController.shotIteration.Standard:
				gameControllerScript.SetPowerupTime (PowerupTime);
				playerControllerScript_P1.ShotType = PlayerController.shotType.Triple;
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.TripleShotFireRates [0];
				break;

			case PlayerController.shotIteration.Enhanced:
				gameControllerScript.SetPowerupTime (PowerupTime);
				break;

			case PlayerController.shotIteration.Rapid:
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.TripleShotFireRates [1];
				gameControllerScript.SetPowerupTime (PowerupTime);
				break;

			case PlayerController.shotIteration.Overdrive:
				break;
			}

			SetPowerupTexture (0);
			gameControllerScript.PowerupText_P1[0].text = "";
			break;

		case powerups.ExtraLife: 
			gameControllerScript.Lives += 1;
			break;

		case powerups.RippleShot: 

			playerControllerScript_P1.ShotType = PlayerController.shotType.Ripple;

			if (playerControllerScript_P1.isInOverdrive == true) 
			{
				playerControllerScript_P1.RippleShotIteration = PlayerController.shotIteration.Overdrive;
			}

			if (playerControllerScript_P1.isInRapidFire == false) 
			{
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.RippleShotFireRates [0];
			}

			if (playerControllerScript_P1.isInRapidFire == true) 
			{
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.RippleShotFireRates [1];
			}

			// Tweaks to conditions based on which iteration the player is on.
			switch (playerControllerScript_P1.RippleShotIteration) 
			{
			case PlayerController.shotIteration.Standard:
				gameControllerScript.SetPowerupTime (PowerupTime);
				playerControllerScript_P1.ShotType = PlayerController.shotType.Ripple;
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.RippleShotFireRates [0];
				break;

			case PlayerController.shotIteration.Enhanced:
				gameControllerScript.SetPowerupTime (PowerupTime);
				break;

			case PlayerController.shotIteration.Rapid:
				playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.RippleShotFireRates [1];
				gameControllerScript.SetPowerupTime (PowerupTime);
				break;

			case PlayerController.shotIteration.Overdrive:
				break;
			}

			SetPowerupTexture (0);
			gameControllerScript.PowerupText_P1[0].text = "";
			break;

		case powerups.Helix:
			
			gameControllerScript.SetPowerupTime (PowerupTime);

			if (playerControllerScript_P1.Helix.activeInHierarchy == false) 
			{
				SetPowerupTexture (gameControllerScript.NextPowerupSlot_P1);
				gameControllerScript.PowerupText_P1 [gameControllerScript.NextPowerupSlot_P1].text = "";
				gameControllerScript.NextPowerupSlot_P1 += 1;
				playerControllerScript_P1.Helix.SetActive (true);
			}
			break;

		case powerups.Turret:

			gameControllerScript.SetPowerupTime (PowerupTime);

			if (playerControllerScript_P1.nextTurretSpawn < 4) 
			{
				GameObject clone = playerControllerScript_P1.Turrets [playerControllerScript_P1.nextTurretSpawn];
				clone.SetActive (true);
				clone.GetComponent<Turret> ().playerControllerScript = playerControllerScript_P1;

				gameControllerScript.PowerupText_P1 [gameControllerScript.NextPowerupSlot_P1].text = "";
				gameControllerScript.PowerupImage_P1 [gameControllerScript.NextPowerupSlot_P1].texture = PowerupTexture;
				gameControllerScript.PowerupImage_P1 [gameControllerScript.NextPowerupSlot_P1].color = Color.white;

				gameControllerScript.NextPowerupSlot_P1 += 1;
				playerControllerScript_P1.nextTurretSpawn += 1;
			}
			break;

		case powerups.Rapidfire:
			
			gameControllerScript.SetPowerupTime (PowerupTime);

			if (playerControllerScript_P1.isInRapidFire == false)
			{
				switch (playerControllerScript_P1.ShotType) 
				{
				case PlayerController.shotType.Double:
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.DoubleShotFireRates [1];
					break;
				case PlayerController.shotType.Triple:
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.TripleShotFireRates [1];
					break;
				case PlayerController.shotType.Ripple:
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.RippleShotFireRates [1];
					break;
				case PlayerController.shotType.Standard:
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.DoubleShotFireRates [1];
					break;				
				}
				gameControllerScript.RapidfireImage.enabled = true;
				playerControllerScript_P1.isInRapidFire = true;
			}

			break;

		case powerups.Overdrive:

			gameControllerScript.SetPowerupTime (PowerupTime);

			if (playerControllerScript_P1.isInOverdrive == false) 
			{
				playerControllerScript_P1.StandardShotIteration = PlayerController.shotIteration.Overdrive;
				playerControllerScript_P1.DoubleShotIteration = PlayerController.shotIteration.Overdrive;
				playerControllerScript_P1.TripleShotIteration = PlayerController.shotIteration.Overdrive;
				playerControllerScript_P1.RippleShotIteration = PlayerController.shotIteration.Overdrive;
				gameControllerScript.OverdriveImage.enabled = true;
				playerControllerScript_P1.isInOverdrive = true;
			}
			break;

		case powerups.Ricochet:
			gameControllerScript.SetPowerupTime (PowerupTime);

			if (playerControllerScript_P1.DoubleShotIteration != PlayerController.shotIteration.Overdrive) {
				playerControllerScript_P1.DoubleShotIteration = PlayerController.shotIteration.Enhanced;
			}

			if (playerControllerScript_P1.TripleShotIteration != PlayerController.shotIteration.Overdrive) {
				playerControllerScript_P1.TripleShotIteration = PlayerController.shotIteration.Enhanced;
			}

			if (playerControllerScript_P1.RippleShotIteration != PlayerController.shotIteration.Overdrive) {
				playerControllerScript_P1.RippleShotIteration = PlayerController.shotIteration.Enhanced;
			}

			if (playerControllerScript_P1.StandardShotIteration != PlayerController.shotIteration.Overdrive) {
				playerControllerScript_P1.StandardShotIteration = PlayerController.shotIteration.Enhanced;
			}
			playerControllerScript_P1.isRicochet = true;
			gameControllerScript.RicochetImage.enabled = true;
			break;
		}

		Destroy (gameObject);
	}

	void SetPowerupTexture (int index)
	{
		gameControllerScript.PowerupImage_P1[index].texture = PowerupTexture;
		gameControllerScript.PowerupImage_P1[index].color = new Color (1, 1, 1, 1);
	}
}
