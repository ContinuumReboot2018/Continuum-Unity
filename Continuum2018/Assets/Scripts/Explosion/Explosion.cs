using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Explosion : MonoBehaviour 
{
	public GameController gameControllerScript;
	public Animator ComboAnim;
	public TextMeshProUGUI ComboPointsText;
	public Block blockScript;
	public Color TextColor;
	public float MaxScale = 2;
	public float totalPointVal;

	void Start () 
	{
		gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();

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
