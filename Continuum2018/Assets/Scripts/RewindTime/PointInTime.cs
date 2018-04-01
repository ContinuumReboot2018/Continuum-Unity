using UnityEngine;

// Returns in list of Points In Time.
public class PointInTime
{
	[Tooltip ("Get position.")]
	public Vector3 position;
	[Tooltip ("Get rotation.")]
	public Quaternion rotation;

	// Returns position and rotation neatly.
	public PointInTime (Vector3 _position, Quaternion _rotation)
	{
		position = _position;
		rotation = _rotation;
	}
}