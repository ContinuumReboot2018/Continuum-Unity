using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentToTransform : MonoBehaviour 
{
	public string ParentTransformName = "Instantiated";

	void Start () 
	{
		ParentNow ();
	}

	public void ParentNow ()
	{
		GameObject ParentTransform = GameObject.Find (ParentTransformName);
		transform.SetParent (ParentTransform.transform);
	}
}
