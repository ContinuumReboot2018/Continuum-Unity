using UnityEngine;

public class SimpleFollow : MonoBehaviour 
{
	public UpdateMethod updateMethod;
	public enum UpdateMethod
	{
		Update = 0,
		FixedUpdate = 1,
		LateUpdate = 2
	}
	public bool FollowPosition;
	public bool AutomaticallyFindPlayerPosObject;
	public string LookForPosName = "Player";
	public Transform OverrideTransform;
	public Transform FollowPosX;
	public Transform FollowPosY;
	public Transform FollowPosZ;
	public followPosMethod FollowPosMethod;
	public enum followPosMethod
	{
		Lerp,
		SmoothDamp,
		NoSmoothing
	}

	private float FollowPosVelX, FollowPosVelY, FollowPosVelZ;
	public Vector3 FollowPosOffset;
	public Vector3 FollowPosSmoothTime;
	public bool XPosTimeUnscaled = true;
	public bool YPosTimeUnscaled = true;
	public bool ZPosTimeUnscaled = true;
	public Vector2 PosBoundsX;
	public Vector2 PosBoundsY;
	public Vector2 PosBoundsZ;

	public bool FollowRotation;
	public bool AutomaticallyFindPlayerRotObject;
	public string LookForRotName = "Player";
	public Transform FollowRotX;
	public Transform FollowRotY;
	public Transform FollowRotZ;
	public Vector3 FollowRotOffset;
	public Vector3 FollowRotSmoothTime;
	public bool XRotTimeUnscaled = true;
	public bool YRotTimeUnscaled = true;
	public bool ZRotTimeUnscaled = true;

	public enum delta
	{
		Scaled,
		Unscaled,
		Fixed
	}

	public void Start ()
	{
		if (AutomaticallyFindPlayerPosObject == true)
		{
			Transform PlayerPos = GameObject.Find (LookForPosName).transform;

			FollowPosX = PlayerPos;
			FollowPosY = PlayerPos;
			FollowPosZ = PlayerPos;
		}

		if (AutomaticallyFindPlayerRotObject == true)
		{
			Transform PlayerRot = GameObject.Find (LookForRotName).transform;

			FollowRotX = PlayerRot;
			FollowRotY = PlayerRot;
			FollowRotZ = PlayerRot;
		}
	}

	void Update () 
	{
		if (updateMethod == UpdateMethod.Update) 
		{
			if (OverrideTransform != null) 
			{
				FollowPosX = OverrideTransform.transform;
				FollowPosY = OverrideTransform.transform;
				FollowPosZ = OverrideTransform.transform;
				FollowRotX = OverrideTransform.transform;
				FollowRotY = OverrideTransform.transform;
				FollowRotZ = OverrideTransform.transform;
			}

			if (FollowPosition == true && FollowPosMethod != followPosMethod.NoSmoothing) 
			{
				FollowObjectPosition ();
			}

			CheckFollowPosTransforms ();

			if (FollowRotation == true) 
			{
				FollowObjectRotation ();
			}

			if (FollowPosMethod == followPosMethod.NoSmoothing) 
			{
				transform.position = new Vector3 (FollowPosX.position.x, FollowPosY.position.y, FollowPosZ.position.z);
			}
		}
	}

	void FixedUpdate () 
	{
		if (updateMethod == UpdateMethod.FixedUpdate) 
		{
			if (OverrideTransform != null) 
			{
				FollowPosX = OverrideTransform.transform;
				FollowPosY = OverrideTransform.transform;
				FollowPosZ = OverrideTransform.transform;
				FollowRotX = OverrideTransform.transform;
				FollowRotY = OverrideTransform.transform;
				FollowRotZ = OverrideTransform.transform;
			}

			if (FollowPosition == true && FollowPosMethod != followPosMethod.NoSmoothing) 
			{
				FollowObjectPosition ();
			}

			CheckFollowPosTransforms ();

			if (FollowRotation == true) 
			{
				FollowObjectRotation ();
			}

			if (FollowPosMethod == followPosMethod.NoSmoothing) 
			{
				transform.position = new Vector3 (FollowPosX.position.x, FollowPosY.position.y, FollowPosZ.position.z);
			}
		}
	}

	void LateUpdate () 
	{
		if (updateMethod == UpdateMethod.LateUpdate) 
		{
			if (OverrideTransform != null) 
			{
				FollowPosX = OverrideTransform.transform;
				FollowPosY = OverrideTransform.transform;
				FollowPosZ = OverrideTransform.transform;
				FollowRotX = OverrideTransform.transform;
				FollowRotY = OverrideTransform.transform;
				FollowRotZ = OverrideTransform.transform;
			}

			if (FollowPosition == true && FollowPosMethod != followPosMethod.NoSmoothing) 
			{
				FollowObjectPosition ();
			}

			CheckFollowPosTransforms ();

			if (FollowRotation == true) 
			{
				FollowObjectRotation ();
			}

			if (FollowPosMethod == followPosMethod.NoSmoothing) 
			{
				transform.position = new Vector3 (FollowPosX.position.x, FollowPosY.position.y, FollowPosZ.position.z);
			}
		}
	}

	void FollowObjectPosition ()
	{
		if (FollowPosMethod == followPosMethod.Lerp) 
		{
			transform.position = new Vector3 
				(
					// X position.
					Mathf.Clamp (
						Mathf.Lerp (
							transform.position.x, 
							FollowPosX.position.x + FollowPosOffset.x, 
							FollowPosSmoothTime.x * (XPosTimeUnscaled ? Time.unscaledDeltaTime : Time.deltaTime)), 

						PosBoundsX.x, 
						PosBoundsX.y
					),


					// Y position.
					Mathf.Clamp (
						Mathf.Lerp (
							transform.position.y, 
							FollowPosY.position.y + FollowPosOffset.y, 
							FollowPosSmoothTime.y * (YPosTimeUnscaled ? Time.unscaledDeltaTime : Time.deltaTime)), 

						PosBoundsY.x, 
						PosBoundsY.y
					),


					// Z position.
					Mathf.Clamp (
						Mathf.Lerp (
							transform.position.z, 
							FollowPosZ.position.z + FollowPosOffset.z, 
							FollowPosSmoothTime.z * (ZPosTimeUnscaled ? Time.unscaledDeltaTime : Time.deltaTime)), 

						PosBoundsZ.x, 
						PosBoundsZ.y
					)
				);
		}

		if (FollowPosMethod == followPosMethod.SmoothDamp) 
		{
			transform.position = new Vector3 
				(
					// X position.
					Mathf.Clamp (
						Mathf.SmoothDamp (
							transform.position.x, 
							FollowPosX.position.x + FollowPosOffset.x, 
							ref FollowPosVelX, 
							FollowPosSmoothTime.x * (XPosTimeUnscaled ? Time.unscaledDeltaTime : Time.deltaTime)), 

						PosBoundsX.x, 
						PosBoundsX.y
					),

					// Y position.
					Mathf.Clamp (
						Mathf.SmoothDamp (
							transform.position.y, 
							FollowPosY.position.y + FollowPosOffset.y, 
							ref FollowPosVelY, 
							FollowPosSmoothTime.y * (YPosTimeUnscaled ? Time.unscaledDeltaTime : Time.deltaTime)), 

						PosBoundsY.x, 
						PosBoundsY.y
					),

					// Z position.
					Mathf.Clamp (
						Mathf.SmoothDamp (
							transform.position.z, 
							FollowPosZ.position.z + FollowPosOffset.z, 
							ref FollowPosVelZ, 
							FollowPosSmoothTime.z * (ZPosTimeUnscaled ? Time.unscaledDeltaTime : Time.deltaTime)), 

						PosBoundsZ.x, 
						PosBoundsZ.y
					)
				);
		}
	}

	void CheckFollowPosTransforms ()
	{
		if (FollowPosX.transform == null) 
		{
			FollowPosX = this.transform;
		}

		if (FollowPosY.transform == null) 
		{
			FollowPosY = this.transform;
		}

		if (FollowPosZ.transform == null) 
		{
			FollowPosZ = this.transform;
		}
	}

	void FollowObjectRotation ()
	{
		Vector3 RotationAngle = new Vector3 
			(
				Mathf.LerpAngle (
					transform.eulerAngles.x, 
					FollowRotX.eulerAngles.x + FollowRotOffset.x, 
					FollowRotSmoothTime.x * (XRotTimeUnscaled ? Time.unscaledDeltaTime : Time.deltaTime)),
				
				Mathf.LerpAngle (
					transform.eulerAngles.y, 
					FollowRotY.eulerAngles.y + FollowRotOffset.y, 
					FollowRotSmoothTime.y * (YRotTimeUnscaled ? Time.unscaledDeltaTime : Time.deltaTime)),
				
				Mathf.LerpAngle (
					transform.eulerAngles.z, 
					FollowRotZ.eulerAngles.z + FollowRotOffset.z, 
					FollowRotSmoothTime.z * (ZRotTimeUnscaled ? Time.unscaledDeltaTime : Time.deltaTime))
			);

		transform.rotation = Quaternion.Euler(RotationAngle);
	}

	public void OverridePos (Transform pos)
	{
		OverrideTransform = pos;
	}
}