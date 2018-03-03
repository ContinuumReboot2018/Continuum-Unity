using UnityEngine;
using TMPro;

public class Explosion : MonoBehaviour 
{
	public GameController gameControllerScript; // Reference to Game Controller.
	public Block blockScript; // Reference to Block script.
	public float Lifetime = 2; // Lifetime of explosion before it destroys.
	public AudioSource ExplosionSound; // Sound to pay when exploding.

	[Header ("Read Combo")]
	private float totalPointVal; // Number to shot on text in explosion UI.

	[Header ("UI")]
	public TextMeshProUGUI ComboPointsText; // Text to display total points.
	public Animator ComboAnim; // Animator for the combo.
	public Color TextColor; // Color for the points text.
	public float MaxScale = 2; // Maximum scale for the text.

	public Vector2 TextPos; // Position offset for the text, so it's not always centered.

	void Start () 
	{
		Destroy (gameObject, Lifetime); // Set a destroy command.
		ExplosionSound = GameObject.Find("ComboSound").GetComponent<AudioSource> ();
		gameControllerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
		ExplosionSound.pitch = Mathf.Clamp (0.01f * gameControllerScript.combo + 0.5f, 0, 2.5f); // Clamp explosion sound pitch.
		ExplosionSound.Play (); // Play explosion sound.

		// Scales points text based on combo.
		ComboAnim.gameObject.transform.localScale = new Vector3 (
			Mathf.Clamp (0.02f * gameControllerScript.combo + 1.2f, 0, MaxScale), 
			Mathf.Clamp (0.02f * gameControllerScript.combo + 1.2f, 0, MaxScale), 
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