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
	[Tooltip ("Follow position?")]
	public bool FollowPosition;
	[Tooltip ("Automatically find Player?")]
	public bool AutomaticallyFindPlayerPosObject;
	[Tooltip ("GameObject to look for.")]
	public string LookForPosName = "Player";
	[Tooltip ("Override all positions.")]
	public Transform OverrideTransform;
	[Tooltip ("Follow on X Axis.")]
	public Transform FollowPosX;
	[Tooltip ("Follow on Y Axis.")]
	public Transform FollowPosY;
	[Tooltip ("Follow on Z Axis.")]
	public Transform FollowPosZ;
	[Tooltip ("How to follow.")]
	public followPosMethod FollowPosMethod;
	public enum followPosMethod
	{
		Lerp,
		SmoothDamp,
		NoSmoothing
	}

	private float FollowPosVelX, FollowPosVelY, FollowPosVelZ;
	[Tooltip ("Offset of each axis.")]
	public Vector3 FollowPosOffset;
	[Tooltip ("Smoothing amounts on each axis.")]
	public Vector3 FollowPosSmoothTime;

	[Tooltip ("X boundaries.")]
	public Vector2 PosBoundsX;
	[Tooltip ("Y boundaries.")]
	public Vector2 PosBoundsY;
	[Tooltip ("Z boundaries.")]
	public Vector2 PosBoundsZ;

	[Tooltip ("Follow rotation?")]
	public bool FollowRotation;
	[Tooltip ("Automatically find Player?")]
	public bool AutomaticallyFindPlayerRotObject;
	[Tooltip ("GameObject to look for.")]
	public string LookForRotName = "Player";
	[Tooltip ("Follow rotation on X Axis.")]
	public Transform FollowRotX;
	[Tooltip ("Follow rotation on Y Axis.")]
	public Transform FollowRotY;
	[Tooltip ("Follow rotation on Z Axis.")]
	public Transform FollowRotZ;
	[Tooltip ("Rotation offsets.")]
	public Vector3 FollowRotOffset;
	[Tooltip ("Roation smoothin on axes.")]
	public Vector3 FollowRotSmoothTime;

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
							FollowPosSmoothTime.x * Time.unscaledDeltaTime), 

						PosBoundsX.x, 
						PosBoundsX.y
					),

					// Y position.
					Mathf.Clamp (
						Mathf.Lerp (
							transform.position.y, 
							FollowPosY.position.y + FollowPosOffset.y, 
							FollowPosSmoothTime.y * Time.unscaledDeltaTime), 

						PosBoundsY.x, 
						PosBoundsY.y
					),

					// Z position.
					Mathf.Clamp (
						Mathf.Lerp (
							transform.position.z, 
							FollowPosZ.position.z + FollowPosOffset.z, 
							FollowPosSmoothTime.z * Time.unscaledDeltaTime), 

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
							FollowPosSmoothTime.x * Time.unscaledDeltaTime), 

						PosBoundsX.x, 
						PosBoundsX.y
					),

					// Y position.
					Mathf.Clamp (
						Mathf.SmoothDamp (
							transform.position.y, 
							FollowPosY.position.y + FollowPosOffset.y, 
							ref FollowPosVelY, 
							FollowPosSmoothTime.y * Time.unscaledDeltaTime), 

						PosBoundsY.x, 
						PosBoundsY.y
					),

					// Z position.
					Mathf.Clamp (
						Mathf.SmoothDamp (
							transform.position.z, 
							FollowPosZ.position.z + FollowPosOffset.z, 
							ref FollowPosVelZ, 
							FollowPosSmoothTime.z * Time.unscaledDeltaTime), 

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
					FollowRotSmoothTime.x * Time.unscaledDeltaTime),
				
				Mathf.LerpAngle (
					transform.eulerAngles.y, 
					FollowRotY.eulerAngles.y + FollowRotOffset.y, 
					FollowRotSmoothTime.y * Time.unscaledDeltaTime),
				
				Mathf.LerpAngle (
					transform.eulerAngles.z, 
					FollowRotZ.eulerAngles.z + FollowRotOffset.z, 
					FollowRotSmoothTime.z * Time.unscaledDeltaTime)
			);

		transform.rotation = Quaternion.Euler(RotationAngle);
	}

	public void OverridePos (Transform pos)
	{
		OverrideTransform = pos;
	}
}