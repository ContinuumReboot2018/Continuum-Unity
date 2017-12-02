using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentToTransform : MonoBehaviour 
{
	public string ParentTransformName = "Instantiated";

	void Start () 
	{
		GameObject ParentTransform = GameObject.Find (ParentTransformName);
		transform.SetParent (ParentTransform.transform);
	}
}
