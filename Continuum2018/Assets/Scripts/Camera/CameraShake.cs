using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
	// Transform of the camera to shake. Grabs the gameObject's transform
	// if null.
	public Transform camTransform;
	
	// How long the object should shake for.
	public float shakeDuration = 0f;
	public float shakeTimeRemaining;
	
	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.7f;
	public float decreaseFactor = 1.0f;
	
	Vector3 originalPos;

	public int Priority;

	public bool SyncWithShaker;
	public CameraShake SyncShaker;
	public float SyncMultiplier = 1;
	public Vector3 Offset;
	
	void Awake()
	{
		if (camTransform == null)
		{
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}
	}
	
	void OnEnable()
	{
		originalPos = camTransform.localPosition;
	}

	void Update()
	{
		//if (SyncWithShaker == false) 
		//{
			if (shakeTimeRemaining > 0)
			{
				camTransform.localPosition = originalPos + (Random.insideUnitSphere * shakeAmount) + Offset;
			
				shakeTimeRemaining -= Time.deltaTime * decreaseFactor;
			} 

			else 
			
			{
				Priority = 0;
				shakeTimeRemaining = 0f;
				camTransform.localPosition = originalPos + Offset;
			}
		//}

		if (SyncWithShaker == true) 
		{
			this.shakeDuration = SyncShaker.shakeDuration;
			this.shakeTimeRemaining = SyncShaker.shakeTimeRemaining;
			this.shakeAmount = SyncShaker.shakeAmount * SyncMultiplier;
		}
	}

	public void Shake ()
	{
		shakeTimeRemaining = shakeDuration;
	}

	public void ShakeCam (float strength, float time, int priority)
	{
		if (priority >= Priority)
		{
			shakeAmount = strength;
			shakeDuration = time;
			shakeTimeRemaining = time;
			Priority = priority;
		}
	}

	public void OverwriteCamShakeDuration (float duration)
	{
		shakeDuration = duration;
	}
}