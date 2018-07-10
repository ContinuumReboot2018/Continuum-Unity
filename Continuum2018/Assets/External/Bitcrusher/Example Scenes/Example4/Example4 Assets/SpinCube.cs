using UnityEngine;
using System.Collections;

public class SpinCube : MonoBehaviour {

	void Update ()
	{
		transform.Rotate (0,2,0*Time.deltaTime);
	}

}