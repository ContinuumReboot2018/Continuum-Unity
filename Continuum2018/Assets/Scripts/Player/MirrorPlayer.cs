using UnityEngine;

public class MirrorPlayer : MonoBehaviour
{
	public PlayerController playerControllerScript;
	public Transform PlayerPos;
	public Transform MirrorPlayerPos;

	//public GameObject Shot;
	//public Transform ShotSpawn;

	public Animator PlayerRecoil;

	public Transform StandardShotSpawn;
	public Transform DoubleShotSpawnL;
	public Transform DoubleShotSpawnR;

	public Transform TripleShotSpawnL;
	public Transform TripleShotSpawnM;
	public Transform TripleShotSpawnR;

	public Transform RippleShotSpawn;

	void OnEnable ()
	{
		UpdateMirrorPlayerPos ();
	}

	void Update ()
	{
		UpdateMirrorPlayerPos ();
	}

	void UpdateMirrorPlayerPos ()
	{
		MirrorPlayerPos.position = new Vector3 (
			-PlayerPos.position.x,
			PlayerPos.position.y, 
			PlayerPos.position.z
		);
	}

	public void Shoot ()
	{
		/*
		GameObject newShot = Instantiate (Shot, ShotSpawn.position, ShotSpawn.rotation);
		newShot.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
		newShot.GetComponent<Bullet> ().playerPos = MirrorPlayerPos.transform;
		newShot.name = "Standard Shot_P" + playerControllerScript.PlayerId + " (Standard)";*/

		switch (playerControllerScript.ShotType) 
		{
		case PlayerController.shotType.Standard:
			switch (playerControllerScript.StandardShotIteration)
			{
			case PlayerController.shotIteration.Standard:
				GameObject shot = Instantiate (playerControllerScript.StandardShot, StandardShotSpawn.position, StandardShotSpawn.rotation);
				shot.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				shot.GetComponent<Bullet> ().playerPos = MirrorPlayerPos.transform;
				shot.name = "Standard Shot_P" + playerControllerScript.PlayerId + " (Standard)";
				break;
			case PlayerController.shotIteration.Enhanced:
				GameObject shotEnhanced = Instantiate (playerControllerScript.StandardShotEnhanced, StandardShotSpawn.position, StandardShotSpawn.rotation);
				shotEnhanced.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				shotEnhanced.GetComponent<Bullet> ().playerPos = MirrorPlayerPos.transform;
				shotEnhanced.name = "Standard Shot_P" + playerControllerScript.PlayerId + " (Enhanced)";
				break;
			case PlayerController.shotIteration.Rapid:
				GameObject shotRapid = Instantiate (playerControllerScript.StandardShotEnhanced, StandardShotSpawn.position, StandardShotSpawn.rotation);
				shotRapid.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				shotRapid.GetComponent<Bullet> ().playerPos = MirrorPlayerPos.transform;
				shotRapid.name = "Standard Shot_P" + playerControllerScript.PlayerId + " (Enhanced)";
				break;
			case PlayerController.shotIteration.Overdrive:
				GameObject shotOverdrive = Instantiate (playerControllerScript.StandardShotOverdrive, StandardShotSpawn.position, StandardShotSpawn.rotation);
				shotOverdrive.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				shotOverdrive.GetComponent<Bullet> ().playerPos = MirrorPlayerPos.transform;
				shotOverdrive.name = "Standard Shot_P" + playerControllerScript.PlayerId + " (Overdrive)";
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
				doubleshotL.GetComponent<Bullet> ().playerPos = MirrorPlayerPos.transform;
				doubleshotR.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				doubleshotR.GetComponent<Bullet> ().playerPos = MirrorPlayerPos.transform;
				doubleshotL.name = "Double ShotL_P" + playerControllerScript.PlayerId + " (Standard)";
				doubleshotR.name = "Double ShotR_P" + playerControllerScript.PlayerId + " (Standard)";
				break;
			case PlayerController.shotIteration.Enhanced:
				GameObject doubleshotLEnhanced = Instantiate (playerControllerScript.DoubleShotLEnhanced, DoubleShotSpawnL.position, DoubleShotSpawnL.rotation);
				GameObject doubleshotREnhanced = Instantiate (playerControllerScript.DoubleShotREnhanced, DoubleShotSpawnR.position, DoubleShotSpawnR.rotation);
				doubleshotLEnhanced.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				doubleshotLEnhanced.GetComponent<Bullet> ().playerPos = MirrorPlayerPos.transform;
				doubleshotREnhanced.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				doubleshotREnhanced.GetComponent<Bullet> ().playerPos = MirrorPlayerPos.transform;
				doubleshotLEnhanced.name = "Double ShotL_P" + playerControllerScript.PlayerId + " (Enhanced)";
				doubleshotREnhanced.name = "Double ShotR_P" + playerControllerScript.PlayerId + " (Enhanced)";
				break;
			case PlayerController.shotIteration.Rapid:
				GameObject doubleshotLRapid = Instantiate (playerControllerScript.DoubleShotLEnhanced, DoubleShotSpawnL.position, DoubleShotSpawnL.rotation);
				GameObject doubleshotRRapid = Instantiate (playerControllerScript.DoubleShotREnhanced, DoubleShotSpawnR.position, DoubleShotSpawnR.rotation);
				doubleshotLRapid.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				doubleshotLRapid.GetComponent<Bullet> ().playerPos = MirrorPlayerPos.transform;
				doubleshotRRapid.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				doubleshotRRapid.GetComponent<Bullet> ().playerPos = MirrorPlayerPos.transform;
				doubleshotLRapid.name = "Double ShotL_P" + playerControllerScript.PlayerId + " (Rapid)";
				doubleshotRRapid.name = "Double ShotR_P" + playerControllerScript.PlayerId + " (Rapid)";
				break;
			case PlayerController.shotIteration.Overdrive:
				GameObject doubleshotLOverdrive = Instantiate (playerControllerScript.DoubleShotLOverdrive, DoubleShotSpawnL.position, DoubleShotSpawnL.rotation);
				GameObject doubleshotROverdrive = Instantiate (playerControllerScript.DoubleShotROverdrive, DoubleShotSpawnR.position, DoubleShotSpawnR.rotation);
				doubleshotLOverdrive.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				doubleshotLOverdrive.GetComponent<Bullet> ().playerPos = MirrorPlayerPos.transform;
				doubleshotROverdrive.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				doubleshotROverdrive.GetComponent<Bullet> ().playerPos = MirrorPlayerPos.transform;
				doubleshotLOverdrive.name = "Double ShotL_P" + playerControllerScript.PlayerId + " (Overdrive)";
				doubleshotROverdrive.name = "Double ShotR_P" + playerControllerScript.PlayerId + " (Overdrive)";
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
				tripleShotLStandard.name = "Triple ShotL_P" + playerControllerScript.PlayerId + " (Standard)";
				tripleShotMStandard.name = "Triple ShotM_P" + playerControllerScript.PlayerId + " (Standard)";
				tripleShotRStandard.name = "Triple ShotR_P" + playerControllerScript.PlayerId + " (Standard)";
				break;
			case PlayerController.shotIteration.Enhanced:
				GameObject tripleShotLEnhanced = Instantiate (playerControllerScript.TripleShotLEnhanced, TripleShotSpawnL.position, TripleShotSpawnL.rotation);
				GameObject tripleShotMEnhanced = Instantiate (playerControllerScript.TripleShotMEnhanced, TripleShotSpawnM.position, TripleShotSpawnM.rotation);
				GameObject tripleShotREnhanced = Instantiate (playerControllerScript.TripleShotREnhanced, TripleShotSpawnR.position, TripleShotSpawnR.rotation);
				tripleShotLEnhanced.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				tripleShotMEnhanced.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				tripleShotREnhanced.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				tripleShotLEnhanced.name = "Triple ShotL_P" + playerControllerScript.PlayerId + " (Enhanced)";
				tripleShotMEnhanced.name = "Triple ShotM_P" + playerControllerScript.PlayerId + " (Enhanced)";
				tripleShotREnhanced.name = "Triple ShotR_P" + playerControllerScript.PlayerId + " (Enhanced)";
				break;
			case PlayerController.shotIteration.Rapid:
				GameObject tripleShotLRapid = Instantiate (playerControllerScript.TripleShotLEnhanced, TripleShotSpawnL.position, TripleShotSpawnL.rotation);
				GameObject tripleShotMRapid = Instantiate (playerControllerScript.TripleShotMEnhanced, TripleShotSpawnM.position, TripleShotSpawnM.rotation);
				GameObject tripleShotRRapid = Instantiate (playerControllerScript.TripleShotREnhanced, TripleShotSpawnR.position, TripleShotSpawnR.rotation);
				tripleShotLRapid.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				tripleShotMRapid.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				tripleShotRRapid.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				tripleShotLRapid.name = "Triple ShotL_P" + playerControllerScript.PlayerId + " (Rapid)";
				tripleShotMRapid.name = "Triple ShotM_P" + playerControllerScript.PlayerId + " (Rapid)";
				tripleShotRRapid.name = "Triple ShotR_P" + playerControllerScript.PlayerId + " (Rapid)";
				break;
			case PlayerController.shotIteration.Overdrive:
				GameObject tripleShotLOverdrive = Instantiate (playerControllerScript.TripleShotLOverdrive, TripleShotSpawnL.position, TripleShotSpawnL.rotation);
				GameObject tripleShotMOverdrive = Instantiate (playerControllerScript.TripleShotMOverdrive, TripleShotSpawnM.position, TripleShotSpawnM.rotation);
				GameObject tripleShotROverdrive = Instantiate (playerControllerScript.TripleShotROverdrive, TripleShotSpawnR.position, TripleShotSpawnR.rotation);
				tripleShotLOverdrive.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				tripleShotMOverdrive.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				tripleShotROverdrive.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				tripleShotLOverdrive.name = "Triple ShotL_P" + playerControllerScript.PlayerId + " (Overdrive)";
				tripleShotMOverdrive.name = "Triple ShotM_P" + playerControllerScript.PlayerId + " (Overdrive)";
				tripleShotROverdrive.name = "Triple ShotR_P" + playerControllerScript.PlayerId + " (Overdrive)";
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
				rippleShotStandard.name = "Ripple Shot_P" + playerControllerScript.PlayerId + " (Standard)";
				break;
			case PlayerController.shotIteration.Enhanced:
				GameObject rippleShotEnhanced = Instantiate (playerControllerScript.RippleShotEnhanced, RippleShotSpawn.position, Quaternion.Euler (0, 0, 0));
				rippleShotEnhanced.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				rippleShotEnhanced.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				rippleShotEnhanced.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				rippleShotEnhanced.name = "Ripple Shot_P" + playerControllerScript.PlayerId + " (Enhanced)";
				break;
			case PlayerController.shotIteration.Rapid:
				GameObject rippleShotRapid = Instantiate (playerControllerScript.RippleShotEnhanced, RippleShotSpawn.position, Quaternion.Euler (0, 0, 0));
				rippleShotRapid.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				rippleShotRapid.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				rippleShotRapid.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				rippleShotRapid.name = "Ripple Shot_P" + playerControllerScript.PlayerId + " (Rapid)";
				break;
			case PlayerController.shotIteration.Overdrive:
				GameObject rippleShotOverdrive = Instantiate (playerControllerScript.RippleShotOverdrive, RippleShotSpawn.position, Quaternion.Euler (0, 0, 0));
				rippleShotOverdrive.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				rippleShotOverdrive.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				rippleShotOverdrive.GetComponent<Bullet> ().playerControllerScript = playerControllerScript;
				rippleShotOverdrive.name = "Ripple Shot_P" + playerControllerScript.PlayerId + " (Overdrive)";
				break;
			}
			break;
		}

		PlayerRecoil.Play ("PlayerRecoil");
	}
}