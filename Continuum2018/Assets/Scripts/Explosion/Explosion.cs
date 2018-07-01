using UnityEngine;
using TMPro;

public class Explosion : MonoBehaviour 
{
	public Block blockScript; // Reference to Block script.
	public float Lifetime = 2; // Lifetime of explosion before it destroys.
	public AudioSource ExplosionSound; // Sound to pay when exploding.

	[Header ("Read Combo")]
	[Tooltip ("Number to shot on text in explosion UI.")]
	private float totalPointVal;

	[Header ("UI")]
	[Tooltip ("Text to display total points.")]
	public TextMeshProUGUI ComboPointsText;
	[Tooltip ("Animator for the combo.")]
	public Animator ComboAnim;
	[Tooltip ("Color for the points text.")]
	public Color TextColor;
	[Tooltip ("Maximum scale for the text.")]
	public float MaxScale = 2;
	[Tooltip ("Position offset for the text, so it's not always centered.")]
	public Vector2 TextPos;

	void Start () 
	{
		Destroy (gameObject, Lifetime); // Set a destroy command.
		ExplosionSound = GameObject.Find("ComboSound").GetComponent<AudioSource> ();
		ExplosionSound.pitch = Mathf.Clamp (0.01f * GameController.Instance.combo + 0.5f, 0, 2.5f); // Clamp explosion sound pitch.
		ExplosionSound.Play (); // Play explosion sound.

		// Scales points text based on combo.
		ComboAnim.gameObject.transform.localScale = new Vector3 (
			Mathf.Clamp (0.02f * GameController.Instance.combo + 1.2f, 0, MaxScale), 
			Mathf.Clamp (0.02f * GameController.Instance.combo + 1.2f, 0, MaxScale), 
			1
		);

		// Offsets local position of text.
		ComboPointsText.gameObject.transform.localPosition = new Vector3 (
			Random.Range (-TextPos.x, TextPos.x),
			Random.Range (-TextPos.y, TextPos.y),
			0
		);
	}

	// Animation attributes for explosion text points.
	public void Anim ()
	{
		if (blockScript != null) 
		{
			totalPointVal = Mathf.Round (blockScript.totalPointValue); // Round points to nearest integer.
			ComboPointsText.text = "" + totalPointVal + ""; // Display points on the text.
			ComboAnim.Play ("ComboPoints"); // Play the animation.
			ComboPointsText.color = TextColor; // Set the text color.
			blockScript.RefreshCombo (); // Refresh block combo.
		}
	}
}