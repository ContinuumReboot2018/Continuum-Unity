using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentToTransform : MonoBehaviour 
{
	public string ParentTransformName = "Instantiated";
	public bool OnStart = true;

	void Start () 
	{
		if (OnStart)
		{
			ParentNow ();
		}
	}

	public void ParentNow ()
	{
		GameObject ParentTransform = GameObject.Find (ParentTransformName);
		transform.SetParent (ParentTransform.transform);
	}
}
