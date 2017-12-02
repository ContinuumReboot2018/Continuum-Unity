using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PostProcessing;

public class GameController : MonoBehaviour 
{
	public PlayerController playerControllerScript_P1;
	public TimescaleController timescaleControllerScript;
	public CursorManager cursorManagerScript;
	public PostProcessingProfile ImageEffects;

	public bool showDebugMenu;
	public GameObject DebugMenu;

	[Header ("Scoring")]
	public bool CountScore;
	public float DisplayScore;
	public float CurrentScore;
	public float TargetScore;
	public float ScoreSmoothing;
	public TextMeshProUGUI ScoreText;

	public int ScoreMult;
	public float ScoreRate;

	[Header ("Camera")]
	public Camera MainCamera;
	public float OrthSize = 10;
	public float StartOrthSize = 10;
	private float OrthSizeVel;
	public float OrthSizeSmoothTime = 1;

	[Header ("Visuals")]
	public float StarFieldForegroundLifetimeMultipler = 0.1f;
	public float StarFieldForegroundSimulationSpeed = 1;
	public ParticleSystem StarFieldForeground;

	void Start () 
	{
		SetStartOrthSize ();
		cursorManagerScript.HideMouse ();
		cursorManagerScript.LockMouse ();
	}

	void Update () 
	{
		UpdateScore ();
		CheckOrthSize ();
		UpdateStarFieldparticleEffectTrail ();	
		CheckDebugMenu ();
		UpdateImageEffects ();
	}

	void UpdateStarFieldparticleEffectTrail ()
	{
		var StarFieldForegroundTrailModule = StarFieldForeground.trails;
		StarFieldForegroundTrailModule.lifetime = new ParticleSystem.MinMaxCurve (0, StarFieldForegroundLifetimeMultipler * Time.timeScale);

		var StarFieldForegroundMainModule = StarFieldForeground.main;
		StarFieldForegroundMainModule.simulationSpeed = StarFieldForegroundSimulationSpeed * Time.timeScale;
	}

	void UpdateScore ()
	{
		if (CountScore == true) 
		{
			TargetScore += ScoreRate * Time.deltaTime * ScoreMult * Time.timeScale;

			CurrentScore = Mathf.Lerp (CurrentScore, TargetScore, ScoreSmoothing * Time.unscaledDeltaTime);
			DisplayScore = Mathf.Round (CurrentScore);
			ScoreText.text = "" + DisplayScore;
		}
	}

	public void ResetScore ()
	{
		TargetScore = 0;
		CurrentScore = 0;
		DisplayScore = 0;
		ScoreText.text = "" + DisplayScore;
	}

	void UpdateImageEffects ()
	{
		var ImageEffectsMotionBlurModuleSettings = ImageEffects.motionBlur.settings;
		ImageEffectsMotionBlurModuleSettings.frameBlending = Mathf.Clamp(-0.12f * timescaleControllerScript.Distance + 1.18f, 0, 1); 
		ImageEffects.motionBlur.settings = ImageEffectsMotionBlurModuleSettings;
	}

	void SetStartOrthSize ()
	{
		//MainCamera.orthographicSize = StartOrthSize;
	}

	void CheckOrthSize ()
	{
		//OrthSize = -0.27f * (timeScaleControllerScript.Distance) + 10;
		//OrthSize = 4 * Mathf.Sin (0.17f * timeScaleControllerScript.Distance) + 8;

		//MainCamera.orthographicSize = Mathf.SmoothDamp (MainCamera.orthographicSize, OrthSize, ref OrthSizeVel, OrthSizeSmoothTime * Time.deltaTime);
	}

	void CheckDebugMenu ()
	{
		if (Input.GetKeyDown (KeyCode.Tab)) 
		{
			showDebugMenu = !showDebugMenu;

			if (showDebugMenu == true) 
			{
				DebugMenu.SetActive (true);
			}

			if (showDebugMenu == false) 
			{
				DebugMenu.SetActive (false);
			}
		}
	}
}
