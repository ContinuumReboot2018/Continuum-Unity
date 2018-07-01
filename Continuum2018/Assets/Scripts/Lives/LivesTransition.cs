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
		if (GameController.Instance.Lives != 1) 
		{
			LastLifeText.gameObject.SetActive (false);
		}
	}

	public void UpdateLives ()
	{
		GameController.Instance.UpdateLives ();

		if (GameController.Instance.Lives < GameController.Instance.LifeImages.Length)
		{
			if (GameController.Instance.LifeImages [GameController.Instance.Lives].gameObject.activeSelf == true) 
			{
				GameController.Instance.LifeImages [GameController.Instance.Lives].gameObject.GetComponent<Animator> ().SetTrigger ("LifeImageExit");
			}
		}
	}

	public void CheckLastLife ()
	{
		if (GameController.Instance.Lives == 1) 
		{
			GameController.Instance.LifeImages [0].gameObject.SetActive (false);
			LastLifeText.gameObject.SetActive (true);
			LastLifeText.Play ("LastLifeText");
		}
	}
}