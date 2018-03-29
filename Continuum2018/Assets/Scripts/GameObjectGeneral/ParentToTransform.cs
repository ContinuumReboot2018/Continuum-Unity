using UnityEngine;

public class ParentToTransform : MonoBehaviour 
{
	public string ParentTransformName = "Instantiated"; // Finds GameObject by this string to parent to.
	public bool OnStart = true; // Parent to GameObject on Start?
	private Transform ParentTransform; // The parent Trasnform.

	void Start () 
	{
		if (OnStart == true)
		{
			ParentNow ();
		}
	}

	// Finds GameObject at this time and parents to it.
	public void ParentNow ()
	{
		ParentTransform = GameObject.Find (ParentTransformName).transform;
		transform.SetParent (ParentTransform);
		transform.SetAsLastSibling ();
	}
}