using UnityEngine;

public class Turret : MonoBehaviour 
{
	public PlayerController playerControllerScript; // Reference to Player Controller.
	public GameController gameControllerScript; // Reference to Game Controller.
	[Space (10)]
	[Tooltip ("Simple: Shoots single bullets.\nComplex: Shoots whatever the player powerup is.")]
	public TurretShootingMethod turretShootingMethod;
	public enum TurretShootingMethod
	{
		Simple = 0,
		Complex = 1
	}

	[Space (10)]
	public Transform StandardShotSpawn;
	[Space (10)]
	public Transform DoubleShotSpawnL;
	public Transform DoubleShotSpawnR;
	[Space (10)]
	public Transform TripleShotSpawnL;
	public Transform TripleShotSpawnM;
	public Transform TripleShotSpawnR;
	[Space (10)]
	public Transform RippleShotSpawn;
	[Space (10)]
	[Tooltip ("Fire rate.")]
	public float FireRate = 0.1f;
	private float nextFire; // Time to next fire.

	void OnEnable () 
	{
		gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();

		if (playerControllerScript == null) 
		{
			playerControllerScript = GameObject.Find ("PlayerController_P1").GetComponent<PlayerController> ();
		}
	}

	void Update () 
	{
		// Check for powerup time remaining, disable if time ran out.
		if (GameController.Instance.PowerupTimeRemaining <= 0) 
		{
			gameObject.SetActive (false);
		}

		CheckForBlock (); // Finds a block in its line of sight.
	}

	// Finds a block in its line of sight.
	void CheckForBlock ()
	{
		Vector3 up = transform.TransformDirection (Vector3.up); // Find outwards direction.
		RaycastHit hit;

		// Look for Blocks pointing our from the player in the current direction.
		if (Physics.Raycast (transform.position, up, out hit, 40)) 
		{
			if (hit.collider.tag == ("Block"))
			{
				Shoot (); // Shoot at it if found.
			}
		}

		Debug.DrawRay (transform.position, transform.TransformDirection (Vector3.up) * 40, Color.magenta); // Show line in editor.
	}

	void Shoot ()
	{
		if (Time.unscaledTime > nextFire) 
		{
			if (turretShootingMethod == TurretShootingMethod.Simple) 
			{
				switch (playerControllerScript.StandardShotIteration)
				{
				case PlayerController.shotIteration.Standard:
					GameObject shot = Instantiate (playerControllerScript.StandardShot, StandardShotSpawn.position, StandardShotSpawn.rotation);
					shot.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
					shot.GetComponent<Bullet> ().playerPos = transform;
					shot.name = "Standard Shot_P" + playerControllerScript.PlayerId + " (Standard)_Turret";
					break;
				case PlayerController.shotIteration.Enhanced:
					GameObject shotEnhanced = Instantiate (playerControllerScript.StandardShotEnhanced, StandardShotSpawn.position, StandardShotSpawn.rotation);
					shotEnhanced.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
					shotEnhanced.GetComponent<Bullet> ().playerPos = transform;
					shotEnhanced.name = "Standard Shot_P" + playerControllerScript.PlayerId + " (Enhanced)_Turret";
					break;
				case PlayerController.shotIteration.Rapid:
					GameObject shotRapid = Instantiate (playerControllerScript.StandardShotEnhanced, StandardShotSpawn.position, StandardShotSpawn.rotation);
					shotRapid.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
					shotRapid.GetComponent<Bullet> ().playerPos = transform;
					shotRapid.name = "Standard Shot_P" + playerControllerScript.PlayerId + " (Enhanced)_Turret";
					break;
				case PlayerController.shotIteration.Overdrive:
					GameObject shotOverdrive = Instantiate (playerControllerScript.StandardShotOverdrive, StandardShotSpawn.position, StandardShotSpawn.rotation);
					shotOverdrive.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
					shotOverdrive.GetComponent<Bullet> ().playerPos = transform;
					shotOverdrive.name = "Standard Shot_P" + playerControllerScript.PlayerId + " (Overdrive)_Turret";
					break;
				}
			}

			if (turretShootingMethod == TurretShootingMethod.Complex) 
			{
				switch (playerControllerScript.ShotType) 
				{
				case PlayerController.shotType.Standard:
					switch (playerControllerScript.StandardShotIteration) 
					{
					case PlayerController.shotIteration.Standard:
						GameObject shot = Instantiate (playerControllerScript.StandardShot, StandardShotSpawn.position, StandardShotSpawn.rotation);
						shot.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						shot.GetComponent<Bullet> ().playerPos = transform;
						shot.name = "Standard Shot_P" + playerControllerScript.PlayerId + " (Standard)_Turret";
						break;
					case PlayerController.shotIteration.Enhanced:
						GameObject shotEnhanced = Instantiate (playerControllerScript.StandardShotEnhanced, StandardShotSpawn.position, StandardShotSpawn.rotation);
						shotEnhanced.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						shotEnhanced.GetComponent<Bullet> ().playerPos = transform;
						shotEnhanced.name = "Standard Shot_P" + playerControllerScript.PlayerId + " (Enhanced)_Turret";
						break;
					case PlayerController.shotIteration.Rapid:
						GameObject shotRapid = Instantiate (playerControllerScript.StandardShotEnhanced, StandardShotSpawn.position, StandardShotSpawn.rotation);
						shotRapid.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						shotRapid.GetComponent<Bullet> ().playerPos = transform;
						shotRapid.name = "Standard Shot_P" + playerControllerScript.PlayerId + " (Enhanced)_Turret";
						break;
					case PlayerController.shotIteration.Overdrive:
						GameObject shotOverdrive = Instantiate (playerControllerScript.StandardShotOverdrive, StandardShotSpawn.position, StandardShotSpawn.rotation);
						shotOverdrive.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						shotOverdrive.GetComponent<Bullet> ().playerPos = transform;
						shotOverdrive.name = "Standard Shot_P" + playerControllerScript.PlayerId + " (Overdrive)_Turret";
						break;
					}
					break;

				case PlayerController.shotType.Double:
					switch (playerControllerScript.DoubleShotIteration)
					{
					case PlayerController.shotIteration.Standard:
						GameObject doubleshotL = Instantiate (playerControllerScript.DoubleShotL, DoubleShotSpawnL.position, DoubleShotSpawnL.rotation);
						GameObject doubleshotR = Instantiate (playerControllerScript.DoubleShotR, DoubleShotSpawnR.position, DoubleShotSpawnR.rotation);
						doubleshotL.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						doubleshotL.GetComponent<Bullet> ().playerPos = transform;
						doubleshotR.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						doubleshotR.GetComponent<Bullet> ().playerPos = transform;
						doubleshotL.name = "Double ShotL_P" + playerControllerScript.PlayerId + " (Standard)_Turret";
						doubleshotR.name = "Double ShotR_P" + playerControllerScript.PlayerId + " (Standard)_Turret";
						break;
					case PlayerController.shotIteration.Enhanced:
						GameObject doubleshotLEnhanced = Instantiate (playerControllerScript.DoubleShotLEnhanced, DoubleShotSpawnL.position, DoubleShotSpawnL.rotation);
						GameObject doubleshotREnhanced = Instantiate (playerControllerScript.DoubleShotREnhanced, DoubleShotSpawnR.position, DoubleShotSpawnR.rotation);
						doubleshotLEnhanced.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						doubleshotLEnhanced.GetComponent<Bullet> ().playerPos = transform;
						doubleshotREnhanced.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						doubleshotREnhanced.GetComponent<Bullet> ().playerPos = transform;
						doubleshotLEnhanced.name = "Double ShotL_P" + playerControllerScript.PlayerId + " (Enhanced)_Turret";
						doubleshotREnhanced.name = "Double ShotR_P" + playerControllerScript.PlayerId + " (Enhanced)_Turret";
						break;
					case PlayerController.shotIteration.Rapid:
						GameObject doubleshotLRapid = Instantiate (playerControllerScript.DoubleShotLEnhanced, DoubleShotSpawnL.position, DoubleShotSpawnL.rotation);
						GameObject doubleshotRRapid = Instantiate (playerControllerScript.DoubleShotREnhanced, DoubleShotSpawnR.position, DoubleShotSpawnR.rotation);
						doubleshotLRapid.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						doubleshotLRapid.GetComponent<Bullet> ().playerPos = transform;
						doubleshotRRapid.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						doubleshotRRapid.GetComponent<Bullet> ().playerPos = transform;
						doubleshotLRapid.name = "Double ShotL_P" + playerControllerScript.PlayerId + " (Rapid)_Turret";
						doubleshotRRapid.name = "Double ShotR_P" + playerControllerScript.PlayerId + " (Rapid)_Turret";
						break;
					case PlayerController.shotIteration.Overdrive:
						GameObject doubleshotLOverdrive = Instantiate (playerControllerScript.DoubleShotLOverdrive, DoubleShotSpawnL.position, DoubleShotSpawnL.rotation);
						GameObject doubleshotROverdrive = Instantiate (playerControllerScript.DoubleShotROverdrive, DoubleShotSpawnR.position, DoubleShotSpawnR.rotation);
						doubleshotLOverdrive.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						doubleshotLOverdrive.GetComponent<Bullet> ().playerPos = transform;
						doubleshotROverdrive.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						doubleshotROverdrive.GetComponent<Bullet> ().playerPos = transform;
						doubleshotLOverdrive.name = "Double ShotL_P" + playerControllerScript.PlayerId + " (Overdrive)_Turret";
						doubleshotROverdrive.name = "Double ShotR_P" + playerControllerScript.PlayerId + " (Overdrive)_Turret";
						break;
					}

					break;

				case PlayerController.shotType.Triple:
					switch (playerControllerScript.TripleShotIteration) 
					{
					case PlayerController.shotIteration.Standard:
						GameObject tripleShotLStandard = Instantiate (playerControllerScript.TripleShotL, TripleShotSpawnL.position, TripleShotSpawnL.rotation);
						GameObject tripleShotMStandard = Instantiate (playerControllerScript.TripleShotM, TripleShotSpawnM.position, TripleShotSpawnM.rotation);
						GameObject tripleShotRStandard = Instantiate (playerControllerScript.TripleShotR, TripleShotSpawnR.position, TripleShotSpawnR.rotation);
						tripleShotLStandard.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						tripleShotMStandard.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						tripleShotRStandard.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						tripleShotLStandard.name = "Triple ShotL_P" + playerControllerScript.PlayerId + " (Standard)_Turret";
						tripleShotMStandard.name = "Triple ShotM_P" + playerControllerScript.PlayerId + " (Standard)_Turret";
						tripleShotRStandard.name = "Triple ShotR_P" + playerControllerScript.PlayerId + " (Standard)_Turret";
						break;
					case PlayerController.shotIteration.Enhanced:
						GameObject tripleShotLEnhanced = Instantiate (playerControllerScript.TripleShotLEnhanced, TripleShotSpawnL.position, TripleShotSpawnL.rotation);
						GameObject tripleShotMEnhanced = Instantiate (playerControllerScript.TripleShotMEnhanced, TripleShotSpawnM.position, TripleShotSpawnM.rotation);
						GameObject tripleShotREnhanced = Instantiate (playerControllerScript.TripleShotREnhanced, TripleShotSpawnR.position, TripleShotSpawnR.rotation);
						tripleShotLEnhanced.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						tripleShotMEnhanced.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						tripleShotREnhanced.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						tripleShotLEnhanced.name = "Triple ShotL_P" + playerControllerScript.PlayerId + " (Enhanced)_Turret";
						tripleShotMEnhanced.name = "Triple ShotM_P" + playerControllerScript.PlayerId + " (Enhanced)_Turret";
						tripleShotREnhanced.name = "Triple ShotR_P" + playerControllerScript.PlayerId + " (Enhanced)_Turret";
						break;
					case PlayerController.shotIteration.Rapid:
						GameObject tripleShotLRapid = Instantiate (playerControllerScript.TripleShotLEnhanced, TripleShotSpawnL.position, TripleShotSpawnL.rotation);
						GameObject tripleShotMRapid = Instantiate (playerControllerScript.TripleShotMEnhanced, TripleShotSpawnM.position, TripleShotSpawnM.rotation);
						GameObject tripleShotRRapid = Instantiate (playerControllerScript.TripleShotREnhanced, TripleShotSpawnR.position, TripleShotSpawnR.rotation);
						tripleShotLRapid.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						tripleShotMRapid.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						tripleShotRRapid.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						tripleShotLRapid.name = "Triple ShotL_P" + playerControllerScript.PlayerId + " (Rapid)_Turret";
						tripleShotMRapid.name = "Triple ShotM_P" + playerControllerScript.PlayerId + " (Rapid)_Turret";
						tripleShotRRapid.name = "Triple ShotR_P" + playerControllerScript.PlayerId + " (Rapid)_Turret";
						break;
					case PlayerController.shotIteration.Overdrive:
						GameObject tripleShotLOverdrive = Instantiate (playerControllerScript.TripleShotLOverdrive, TripleShotSpawnL.position, TripleShotSpawnL.rotation);
						GameObject tripleShotMOverdrive = Instantiate (playerControllerScript.TripleShotMOverdrive, TripleShotSpawnM.position, TripleShotSpawnM.rotation);
						GameObject tripleShotROverdrive = Instantiate (playerControllerScript.TripleShotROverdrive, TripleShotSpawnR.position, TripleShotSpawnR.rotation);
						tripleShotLOverdrive.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						tripleShotMOverdrive.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						tripleShotROverdrive.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						tripleShotLOverdrive.name = "Triple ShotL_P" + playerControllerScript.PlayerId + " (Overdrive)_Turret";
						tripleShotMOverdrive.name = "Triple ShotM_P" + playerControllerScript.PlayerId + " (Overdrive)_Turret";
						tripleShotROverdrive.name = "Triple ShotR_P" + playerControllerScript.PlayerId + " (Overdrive)_Turret";
						break;
					}

					break;

				case PlayerController.shotType.Ripple:
					switch (playerControllerScript.RippleShotIteration) 
					{
					case PlayerController.shotIteration.Standard:
						GameObject rippleShotStandard = Instantiate (playerControllerScript.RippleShot, RippleShotSpawn.position, Quaternion.Euler (0, 0, 0));
						rippleShotStandard.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						rippleShotStandard.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						rippleShotStandard.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						rippleShotStandard.name = "Ripple Shot_P" + playerControllerScript.PlayerId + " (Standard)_Turret";
						break;
					case PlayerController.shotIteration.Enhanced:
						GameObject rippleShotEnhanced = Instantiate (playerControllerScript.RippleShotEnhanced, RippleShotSpawn.position, Quaternion.Euler (0, 0, 0));
						rippleShotEnhanced.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						rippleShotEnhanced.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						rippleShotEnhanced.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						rippleShotEnhanced.name = "Ripple Shot_P" + playerControllerScript.PlayerId + " (Enhanced)_Turret";
						break;
					case PlayerController.shotIteration.Rapid:
						GameObject rippleShotRapid = Instantiate (playerControllerScript.RippleShotEnhanced, RippleShotSpawn.position, Quaternion.Euler (0, 0, 0));
						rippleShotRapid.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						rippleShotRapid.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						rippleShotRapid.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						rippleShotRapid.name = "Ripple Shot_P" + playerControllerScript.PlayerId + " (Rapid)_Turret";
						break;
					case PlayerController.shotIteration.Overdrive:
						GameObject rippleShotOverdrive = Instantiate (playerControllerScript.RippleShotOverdrive, RippleShotSpawn.position, Quaternion.Euler (0, 0, 0));
						rippleShotOverdrive.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						rippleShotOverdrive.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						rippleShotOverdrive.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
						rippleShotOverdrive.name = "Ripple Shot_P" + playerControllerScript.PlayerId + " (Overdrive)_Turret";
						break;
					}
					break;
				}
			}

			nextFire = Time.unscaledTime + FireRate;
		}
	}
}