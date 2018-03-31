using UnityEngine;
using InControl;

public class SelectionMenuHand : MonoBehaviour
{
	public SelectionMenu selectionMenuScript;
	public Transform FollowSelection;
	public PlayerActions playerActions;

	public AudioSource HoverSound;
	public AudioSource SelectionSound;

	void Start ()
	{
		CreatePlayerActions ();
	}

	void OnTriggerEnter (Collider other)
	{
		other.GetComponent<Animator> ().SetTrigger ("Hover");
		HoverSound.Play ();
	}

	void OnTriggerExit (Collider other)
	{
		other.GetComponent<Animator> ().SetTrigger ("Normal");
	}

	void OnTriggerStay (Collider other)
	{
		if (playerActions.Shoot.WasPressed && selectionMenuScript.handFocussed == true) 
		{
			other.GetComponent<Animator> ().SetTrigger ("Pressed");
			selectionMenuScript.SetFocus (false);
			SnapToSelection (other.transform);
			SelectionSound.Play ();
		} 

		else 
		
		{
			//other.GetComponent<Animator> ().SetTrigger ("Hover");
		}
	}

	void SnapToSelection (Transform snap)
	{
		FollowSelection.position = snap.position;
	}

	void CreatePlayerActions ()
	{
		playerActions = new PlayerActions ();

		playerActions.MoveLeft.AddDefaultBinding (Key.LeftArrow);
		playerActions.MoveRight.AddDefaultBinding (Key.RightArrow);
		playerActions.MoveUp.AddDefaultBinding (Key.UpArrow);
		playerActions.MoveDown.AddDefaultBinding (Key.DownArrow);

		playerActions.Shoot.AddDefaultBinding (Key.Return);
		playerActions.Back.AddDefaultBinding (Key.Escape);
	}
}