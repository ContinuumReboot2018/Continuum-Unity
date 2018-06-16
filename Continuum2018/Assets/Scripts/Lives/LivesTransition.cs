using UnityEngine;

public class LivesTransition : MonoBehaviour 
{
	public GameController gameControllerScript;
	public Animator LastLifeText;

	void Start ()
	{
		InvokeRepeating ("CheckLastLifeText", 0, 0.5f);
	}

	void CheckLastLifeText ()
	{
		if (gameControllerScript.Lives != 1) 
		{
			LastLifeText.gameObject.SetActive (false);
		}
	}

	public void UpdateLives ()
	{
		gameControllerScript.UpdateLives ();

		if (gameControllerScript.LifeImages [gameControllerScript.Lives].gameObject.activeSelf == true) 
		{
			gameControllerScript.LifeImages [gameControllerScript.Lives].gameObject.GetComponent<Animator> ().SetTrigger ("LifeImageExit");
		}
	}

	public void CheckLastLife ()
	{
		if (gameControllerScript.Lives == 1) 
		{
			gameControllerScript.LifeImages [0].gameObject.SetActive (false);
			LastLifeText.gameObject.SetActive (true);
			LastLifeText.Play ("LastLifeText");
		}
	}
}