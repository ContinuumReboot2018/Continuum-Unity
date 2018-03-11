using UnityEngine;

public class SimplePlaySound : MonoBehaviour 
{
	public bool PlayOnEnable;

	public AudioSource Sound;

	void OnEnable ()
	{
		if (PlayOnEnable == true) 
		{
			PlaySound ();
		}
	}

	public void PlaySound ()
	{
		Sound.Play ();
	}
}