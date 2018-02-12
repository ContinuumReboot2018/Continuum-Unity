using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupPickup : MonoBehaviour 
{
	public GameController gameControllerScript;
	public PlayerController playerControllerScript_P1;
	public TimescaleController timescaleControllerScript;
	public DestroyByTime destroyByTimeScript;

	[Header ("On Awake")]
	public AudioSource AwakeAudio; // The audio effect to play when spawned.
	public MeshRenderer meshrend; // The mesh renderer which has the powerup pickup texture.
	public SphereCollider col; // The collider.
	public ParticleSystem AwakeParticles; // The particle effect to play when spawned.
	public float AwakeDelay = 1; // The delay to show the powerup after spawning.

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
		Ricochet,
		RewindTime,
		Homing
	}

	[Header ("On Pickup")]
	public Texture2D PowerupTexture; // The powerup texture which will feed to the powerup list and explosion UI.
	public bool isShootingPowerup = true; // Checks whether this is a shooting powerup.
	public float PowerupTime = 20; // How much time the powerup adds to the powerup time remaining.
	public GameObject CollectExplosion; // The explosion when the player or bullet collides with it.
	public AudioSource PowerupTimeRunningOutAudio; // The sound that plays when the powerup time is running out.
	public Color PowerupPickupUIColor = Color.white; // The color of the powerup explosion UI texture.

	[Header ("On End Life")]
	public Animator anim; // The animator that the powerup pickup uses.

	[Header ("Turret")]
	public GameObject Turret;

	void Awake ()
	{
		//col.enabled = false;
		StartCoroutine (ShowPowerup ());
		destroyByTimeScript = GetComponent<DestroyByTime> ();
		StartCoroutine (DestroyAnimation ());
		anim = GetComponent<Animator> ();
	}

	IEnumerator ShowPowerup ()
	{
		yield return new WaitForSeconds (AwakeDelay);
		meshrend.enabled = true;
		//col.enabled = true;
		AwakeParticles.Play ();
	}

	IEnumerator DestroyAnimation ()
	{
		yield return new WaitForSecondsRealtime (destroyByTimeScript.delay - 1);
		anim.Play ("PowerupPickupDestroy");
	}

	void Start ()
	{
		gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		playerControllerScript_P1 = GameObject.Find ("PlayerController_P1").GetComponent<PlayerController> ();
		timescaleControllerScript = GameObject.Find ("TimescaleController").GetComponent<TimescaleController> ();
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
					playerControllerScript_P1.CheckPowerupImageUI ();
					timescaleControllerScript.OverrideTimeScaleTimeRemaining = 0.5f;
					timescaleControllerScript.OverridingTimeScale = 0.2f;
					playerControllerScript_P1.NextFire = 0;
					playerControllerScript_P1.DoubleShotNextFire = 0;
					playerControllerScript_P1.TripleShotNextFire = 0;
					playerControllerScript_P1.RippleShotNextFire = 0;
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
					playerControllerScript_P1.CheckPowerupImageUI ();
					timescaleControllerScript.OverrideTimeScaleTimeRemaining = 0.5f;
					timescaleControllerScript.OverridingTimeScale = 0.2f;
					playerControllerScript_P1.NextFire = 0;
					playerControllerScript_P1.DoubleShotNextFire = 0;
					playerControllerScript_P1.TripleShotNextFire = 0;
					playerControllerScript_P1.RippleShotNextFire = 0;
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
		CreatePowerupPickupUI ();
		ActivatePowerup_P1 ();
		playerControllerScript_P1.Vibrate (0.6f, 0.6f, 0.3f);
		PowerupTimeRunningOutAudio.Stop ();
		Destroy (gameObject);
	}

	public void CreatePowerupPickupUI ()
	{
		GameObject powerupPickupUI = Instantiate (gameControllerScript.PowerupPickupUI, transform.position, Quaternion.identity);
		powerupPickupUI.GetComponentInChildren<RawImage> ().texture = PowerupTexture;
		powerupPickupUI.GetComponentInChildren<RawImage> ().color = PowerupPickupUIColor;
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

			if (gameControllerScript.Lives < gameControllerScript.MaxLives)
			{
				gameControllerScript.MaxLivesText.text = "";
			}

			if (gameControllerScript.Lives >= gameControllerScript.MaxLives)
			{
				gameControllerScript.MaxLivesText.text = "MAX";
				Debug.Log ("Reached maximum lives.");
			}
				
			// Caps maximum lives.
			gameControllerScript.Lives = Mathf.Clamp (gameControllerScript.Lives, 0, gameControllerScript.MaxLives);

			gameControllerScript.UpdateLives ();
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

				foreach (Collider helixcol in playerControllerScript_P1.HelixCol) 
				{
					helixcol.enabled = true;
				}
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
				case PlayerController.shotType.Standard:
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.DoubleShotFireRates [1];
					break;	
				case PlayerController.shotType.Double:
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.DoubleShotFireRates [1];
					break;
				case PlayerController.shotType.Triple:
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.TripleShotFireRates [1];
					break;
				case PlayerController.shotType.Ripple:
					playerControllerScript_P1.CurrentFireRate = playerControllerScript_P1.RippleShotFireRates [1];
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

			if (playerControllerScript_P1.DoubleShotIteration != PlayerController.shotIteration.Overdrive) 
			{
				playerControllerScript_P1.DoubleShotIteration = PlayerController.shotIteration.Enhanced;
			}

			if (playerControllerScript_P1.TripleShotIteration != PlayerController.shotIteration.Overdrive) 
			{
				playerControllerScript_P1.TripleShotIteration = PlayerController.shotIteration.Enhanced;
			}

			if (playerControllerScript_P1.RippleShotIteration != PlayerController.shotIteration.Overdrive) 
			{
				playerControllerScript_P1.RippleShotIteration = PlayerController.shotIteration.Enhanced;
			}

			if (playerControllerScript_P1.StandardShotIteration != PlayerController.shotIteration.Overdrive) 
			{
				playerControllerScript_P1.StandardShotIteration = PlayerController.shotIteration.Enhanced;
			}

			playerControllerScript_P1.isRicochet = true;
			gameControllerScript.RicochetImage.enabled = true;
			break;

		case powerups.RewindTime:
			timescaleControllerScript.SetRewindTime (true, 3);
			break;

		case powerups.Homing:
			gameControllerScript.SetPowerupTime (PowerupTime);
			playerControllerScript_P1.isHoming = true;
			gameControllerScript.HomingImage.enabled = true;
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
