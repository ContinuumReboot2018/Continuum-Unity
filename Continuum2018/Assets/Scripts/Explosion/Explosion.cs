using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Explosion : MonoBehaviour 
{
	public GameController gameControllerScript;
	public Block blockScript;
	public float Lifetime = 2;
	public AudioSource ExplosionSound;

	[Header ("Read Combo")]
	private float totalPointVal;
	public Animator ComboAnim;
	public TextMeshProUGUI ComboPointsText;

	public Color TextColor;
	public float MaxScale = 2;

	void Awake ()
	{
		ExplosionSound = GetComponent<AudioSource> ();
	}

	void Start () 
	{
		Destroy (gameObject, Lifetime);
		gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
		ExplosionSound.pitch = 0.005f * gameControllerScript.combo + 0.5f;

		// Scales points text based on combo.
		ComboAnim.gameObject.transform.localScale = new Vector3 (
			Mathf.Clamp (0.015f * gameControllerScript.combo + 0.485f, 0, MaxScale), 
			Mathf.Clamp (0.015f * gameControllerScript.combo + 0.485f, 0, MaxScale), 
			1);
	}

	public void Anim ()
	{
		if (blockScript != null) 
		{
			totalPointVal = Mathf.Round (blockScript.totalPointValue);
			ComboPointsText.text = "" + totalPointVal + "";
			ComboAnim.Play ("ComboPoints");
			ComboPointsText.color = TextColor;
			blockScript.RefreshCombo ();
		}
	}
}
