// Syncronizes audio source with another audio source.

using UnityEngine;
using System.Collections;

public class AudioSync : MonoBehaviour 
{
	public AudioSource master;
	public AudioSource slave;

	void LateUpdate () 
	{
		if (slave.isPlaying == true || master.isPlaying == true) 
		{
			slave.timeSamples = master.timeSamples;
		}
	}
}
