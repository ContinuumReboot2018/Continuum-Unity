using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillateByTime : MonoBehaviour 
{
	private float scaledTime;
	private float unscaledTime;

	public Transform OscillateObject;

	[Header ("Oscillate Position")]
	public bool OscillatePosX;
	public bool OscillatePosY;
	public bool OscillatePosZ;

	public Vector3 MoveAmount;
	public Vector3 MovePeriod;
	public Vector3 MoveOffset;

	public CircularMethod CircularPosFunctionX;
	public CircularMethod CircularPosFunctionY;
	public CircularMethod CircularPosFunctionZ;

	public bool UseUnscaledPosTimeX;
	public bool UseUnscaledPosTimeY;
	public bool UseUnscaledPosTimeZ;

	public bool useGlobalPosition;
	public bool UseWorldPositionX;
	public bool UseWorldPositionY;
	public bool UseWorldPositionZ;

	[Header ("Oscillate Rotation")]
	public bool OscillateRotX;
	public bool OscillateRotY;
	public bool OscillateRotZ;

	public Vector3 RotAmount;
	public Vector3 RotPeriod;
	public Vector3 RotOffset;

	public CircularMethod CircularRotFunctionX;
	public CircularMethod CircularRotFunctionY;
	public CircularMethod CircularRotFunctionZ;

	public bool UseUnscaledRotTimeX;
	public bool UseUnscaledRotTimeY;
	public bool UseUnscaledRotTimeZ;

	public bool useGlobalRotation;
	public bool UseWorldRotationX;
	public bool UseWorldRotationY;
	public bool UseWorldRotationZ;

	// Current Values.
	public float NewPosSinScaledX;
	public float NewPosSinUnscaledX;

	public float NewPosCosScaledX;
	public float NewPosCosUnscaledX;

	public float NewPosSinScaledY;
	public float NewPosSinUnscaledY;

	public float NewPosCosScaledY;
	public float NewPosCosUnscaledY;

	public float NewPosSinScaledZ;
	public float NewPosSinUnscaledZ;

	public float NewPosCosScaledZ;
	public float NewPosCosUnscaledZ;

	public float NewRotSinScaledX;
	public float NewRotSinUnscaledX;

	public float NewRotCosScaledX;
	public float NewRotCosUnscaledX;

	public float NewRotSinScaledY;
	public float NewRotSinUnscaledY;

	public float NewRotCosScaledY;
	public float NewRotCosUnscaledY;

	public float NewRotSinScaledZ;
	public float NewRotSinUnscaledZ;

	public float NewRotCosScaledZ;
	public float NewRotCosUnscaledZ;

	public enum CircularMethod
	{
		Sin,
		Cos
	}

	void Update ()
	{
		CountTime ();
		GetPosValues ();
		SetPosValues ();
		GetRotValues ();
		SetRotValues ();
	}

	void CountTime ()
	{
		scaledTime += Time.deltaTime;
		unscaledTime += Time.unscaledDeltaTime;
	}

	void GetPosValues ()
	{
		if (OscillatePosX) 
		{
			NewPosSinScaledX   = MoveAmount.x * Mathf.Sin (MovePeriod.x * scaledTime)   + MoveOffset.x;
			NewPosSinUnscaledX = MoveAmount.x * Mathf.Sin (MovePeriod.x * unscaledTime) + MoveOffset.x;

			NewPosCosScaledX   = MoveAmount.x * Mathf.Cos (MovePeriod.x * scaledTime)   + MoveOffset.x;
			NewPosCosUnscaledX = MoveAmount.x * Mathf.Cos (MovePeriod.x * unscaledTime) + MoveOffset.x;
		}

		if (!OscillatePosX) 
		{
			if (UseWorldPositionX) 
			{
				NewPosSinScaledX = transform.position.x;
				NewPosSinUnscaledX = transform.position.x;

				NewPosCosScaledX = transform.position.x;
				NewPosCosUnscaledX = transform.position.x;
			}

			if (!UseWorldPositionX) 
			{
				NewPosSinScaledX = transform.localPosition.x;
				NewPosSinUnscaledX = transform.localPosition.x;

				NewPosCosScaledX = transform.localPosition.x;
				NewPosCosUnscaledX = transform.localPosition.x;
			}
		}

		if (OscillatePosY) 
		{
			NewPosSinScaledY   = MoveAmount.y * Mathf.Sin (MovePeriod.y * scaledTime)   + MoveOffset.y;
			NewPosSinUnscaledY = MoveAmount.y * Mathf.Sin (MovePeriod.y * unscaledTime) + MoveOffset.y;

			NewPosCosScaledY   = MoveAmount.y * Mathf.Cos (MovePeriod.y * scaledTime)   + MoveOffset.y;
			NewPosCosUnscaledY = MoveAmount.y * Mathf.Cos (MovePeriod.y * unscaledTime) + MoveOffset.y;
		}

		if (!OscillatePosY) 
		{
			if (UseWorldPositionY) 
			{
				NewPosSinScaledY = transform.position.y;
				NewPosSinUnscaledY = transform.position.y;

				NewPosCosScaledY = transform.position.y;
				NewPosCosUnscaledY = transform.position.y;
			}

			if (!UseWorldPositionY) 
			{
				NewPosSinScaledY = transform.localPosition.y;
				NewPosSinUnscaledY = transform.localPosition.y;

				NewPosCosScaledY = transform.localPosition.y;
				NewPosCosUnscaledY = transform.localPosition.y;
			}
		}

		if (OscillatePosZ) 
		{
			NewPosSinScaledZ   = MoveAmount.z * Mathf.Sin (MovePeriod.z * scaledTime)   + MoveOffset.z;
			NewPosSinUnscaledZ = MoveAmount.z * Mathf.Sin (MovePeriod.z * unscaledTime) + MoveOffset.z;

			NewPosCosScaledZ   = MoveAmount.z * Mathf.Cos (MovePeriod.z * scaledTime)   + MoveOffset.z;
			NewPosCosUnscaledZ = MoveAmount.z * Mathf.Cos (MovePeriod.z * unscaledTime) + MoveOffset.z;
		}

		if (!OscillatePosZ) 
		{
			if (UseWorldPositionZ)
			{
				NewPosSinScaledZ = transform.position.z;
				NewPosSinUnscaledZ = transform.position.z;

				NewPosCosScaledZ = transform.position.z;
				NewPosCosUnscaledZ = transform.position.z;
			}
			
			if (!UseWorldPositionZ)
			{
				NewPosSinScaledZ = transform.localPosition.z;
				NewPosSinUnscaledZ = transform.localPosition.z;

				NewPosCosScaledZ = transform.localPosition.z;
				NewPosCosUnscaledZ = transform.localPosition.z;
			}
		}
	}

	void SetPosValues ()
	{
		if (OscillateObject != null)
		{
			if (useGlobalPosition == true) 
			{
				transform.position = new Vector3 (
					UseUnscaledPosTimeX ? NewPosSinScaledX : NewPosSinUnscaledX,
					UseUnscaledPosTimeY ? NewPosSinScaledY : NewPosSinUnscaledY,
					UseUnscaledPosTimeZ ? NewPosSinScaledZ : NewPosSinUnscaledZ
				);
			}

			if (useGlobalPosition == false) 
			{
				transform.localPosition = new Vector3 (
					UseUnscaledPosTimeX ? NewPosSinScaledX : NewPosSinUnscaledX,
					UseUnscaledPosTimeY ? NewPosSinScaledY : NewPosSinUnscaledY,
					UseUnscaledPosTimeZ ? NewPosSinScaledZ : NewPosSinUnscaledZ
				);
			}
		}
	}

	void GetRotValues ()
	{
		if (OscillateRotX) 
		{
			NewRotSinScaledX = RotAmount.x * Mathf.Sin (RotPeriod.x * scaledTime) + RotOffset.x;
			NewRotSinUnscaledX = RotAmount.x * Mathf.Sin (RotPeriod.x * unscaledTime) + RotOffset.x;

			NewRotCosScaledX = RotAmount.x * Mathf.Cos (RotPeriod.x * scaledTime) + RotOffset.x;
			NewRotCosUnscaledX = RotAmount.x * Mathf.Cos (RotPeriod.x * unscaledTime) + RotOffset.x;
		}

		if (!OscillateRotX) 
		{
			if (UseWorldRotationX) 
			{
				NewRotSinScaledX = transform.rotation.x;
				NewRotSinUnscaledX = transform.rotation.x;
			}

			if (!UseWorldRotationX) 
			{
				NewRotSinScaledX = transform.localRotation.x;
				NewRotSinUnscaledX = transform.localRotation.x;
			}
		}

		if (OscillateRotY) 
		{
			NewRotSinScaledY = RotAmount.y * Mathf.Sin (RotPeriod.y * scaledTime) + RotOffset.y;
			NewRotSinUnscaledY = RotAmount.y * Mathf.Sin (RotPeriod.y * unscaledTime) + RotOffset.y;

			NewRotCosScaledY = RotAmount.y * Mathf.Cos (RotPeriod.y * scaledTime) + RotOffset.y;
			NewRotCosUnscaledY = RotAmount.y * Mathf.Cos (RotPeriod.y * unscaledTime) + RotOffset.y;
		}

		if (!OscillateRotY) 
		{
			if (UseWorldRotationY) 
			{
				NewRotSinScaledY = transform.rotation.y;
				NewRotSinUnscaledY = transform.rotation.y;
			}

			if (!UseWorldRotationY) 
			{
				NewRotSinScaledY = transform.localRotation.y;
				NewRotSinUnscaledY = transform.localRotation.y;
			}
		}

		if (OscillateRotZ) 
		{
			NewRotSinScaledZ = RotAmount.z * Mathf.Sin (RotPeriod.z * scaledTime) + RotOffset.z;
			NewRotSinUnscaledZ = RotAmount.z * Mathf.Sin (RotPeriod.z * unscaledTime) + RotOffset.z;

			NewRotCosScaledZ = RotAmount.z * Mathf.Cos (RotPeriod.z * scaledTime) + RotOffset.z;
			NewRotCosUnscaledZ = RotAmount.z * Mathf.Cos (RotPeriod.z * unscaledTime) + RotOffset.z;
		}

		if (!OscillateRotZ) 
		{
			if (UseWorldRotationZ) 
			{
				NewRotSinScaledZ = transform.rotation.z;
				NewRotSinUnscaledZ = transform.rotation.z;
			}

			if (!UseWorldRotationZ) 
			{
				NewRotSinScaledZ = transform.localRotation.z;
				NewRotSinUnscaledZ = transform.localRotation.z;
			}
		}
	}

	void SetRotValues ()
	{
		if (OscillateObject != null)
		{
			if (useGlobalRotation == true) 
			{
				transform.rotation = Quaternion.Euler (
					UseUnscaledRotTimeX ? NewRotSinScaledX : NewRotSinUnscaledX,
					UseUnscaledRotTimeY ? NewRotSinScaledY : NewRotSinUnscaledY,
					UseUnscaledRotTimeZ ? NewRotSinScaledZ : NewRotSinUnscaledZ
				);
			}

			if (useGlobalRotation == false) {
				transform.localRotation = Quaternion.Euler (
					UseUnscaledRotTimeX ? NewRotSinScaledX : NewRotSinUnscaledX,
					UseUnscaledRotTimeY ? NewRotSinScaledY : NewRotSinUnscaledY,
					UseUnscaledRotTimeZ ? NewRotSinScaledZ : NewRotSinUnscaledZ
				);
			}
		}
	}
}
