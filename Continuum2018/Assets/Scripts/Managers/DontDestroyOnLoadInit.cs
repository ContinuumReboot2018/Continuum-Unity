using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadInit : MonoBehaviour 
{
	public GameObject Managers;

	void Awake ()
	{
		DontDestroyOnLoad (Managers);
	}
}
