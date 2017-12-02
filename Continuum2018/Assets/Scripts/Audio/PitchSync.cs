using UnityEngine;
using System.Collections;

public class PitchSync : MonoBehaviour 
{
	public AudioSource master;
	public AudioSource slave;

	void Update () 
	{
		if (slave.isPlaying == true || master.isPlaying == true)
		{
			slave.pitch = master.pitch;
		}
	}
}
