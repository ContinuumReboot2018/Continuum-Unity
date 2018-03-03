using UnityEngine;
using System.Collections;

public class EvasiveManeuver : MonoBehaviour 
{
	public Vector2 InitialPosition = new Vector2 (16, 12);
	public Vector2 smoothingRange = new Vector2 (10, 60);
	public Vector2 startWait;
	public Vector2 maneuverTime;

	public Vector2 boundaryX = new Vector2 (-10, 10);
	public Vector2 boundaryY = new Vector2 (0, 6);

	private Vector3 MovePoint;

	void Start ()
	{
		MovePoint = new Vector3 (
			Random.Range (-InitialPosition.x, InitialPosition.x),
			InitialPosition.y,
			0
		);

		StartCoroutine (Evade ());
	}

	IEnumerator Evade ()
	{
		yield return new WaitForSeconds (Random.Range (startWait.x, startWait.y));

		while (true)
		{
			// Pick a point to move towards.
			MovePoint = new Vector3 (
				Random.Range (boundaryX.x, boundaryX.y),
				Random.Range (boundaryY.x, boundaryY.y),
				0
			);

			yield return new WaitForSeconds (Random.Range (maneuverTime.x, maneuverTime.y));
		}
	}

	void Update ()
	{
		transform.position = Vector3.Lerp (
			transform.position, 
			MovePoint, 
			Time.deltaTime * Random.Range (smoothingRange.x, smoothingRange.y)
		);
	}
}