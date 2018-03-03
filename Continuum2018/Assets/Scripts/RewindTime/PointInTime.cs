using UnityEngine;

// Returns in list of Points In Time.
public class PointInTime
{
	public Vector3 position; // Get position.
	public Quaternion rotation; // Get rotation.

	// Returns position and rotation neatly.
	public PointInTime (Vector3 _position, Quaternion _rotation)
	{
		position = _position;
		rotation = _rotation;
	}
}