using UnityEngine;

public class Spotlights : MonoBehaviour 
{
	public Light[] SpotLights;
	public SimpleLookAt[] SpotlightLookAt;

	public Color NormalSpotlightColor;
	public Color BossSpotlightColor;
	public Color BigBossSpotlightColor;
	public Color ImpactSpotlightColor;
	public Color SuccessSpotlightColor;

	private Color TargetColor;

	public float NormalSpotlightIntensity;
	public float BossSpotlightIntensity;
	public float BigBossSpotlightIntensity;
	public float ImpactSpotlightIntensity;
	public float SuccessSpotlightIntensity;

	private float TargetIntensity;

	public float NormalSpotlightAngle;
	public float BossSpotlightAngle;
	public float BigBossSpotlightAngle;
	public float ImpactSpotlightAngle;
	public float SuccessSpotlightAngle;

	private float TargetAngle;

	public float smoothing;

	public Transform NewTarget;

	void Start () 
	{
		NormalSpotlightSettings ();
	}

	void Update ()
	{
		foreach (Light spotlight in SpotLights) 
		{
			spotlight.color = Color.Lerp (spotlight.color, TargetColor, smoothing * Time.deltaTime);
			spotlight.intensity = Mathf.Lerp (spotlight.intensity, TargetIntensity, smoothing * Time.deltaTime);
			spotlight.spotAngle = Mathf.Lerp (spotlight.spotAngle, TargetAngle, smoothing * Time.deltaTime);
		}
	}

	public void NormalSpotlightSettings ()
	{
		foreach (Light spotlight in SpotLights) 
		{
			TargetColor = NormalSpotlightColor;
			TargetIntensity = NormalSpotlightIntensity;
			TargetAngle = NormalSpotlightAngle;
		}
	}

	public void BossSpotlightSettings ()
	{
		foreach (Light spotlight in SpotLights) 
		{
			TargetColor = BossSpotlightColor;
			TargetIntensity = BossSpotlightIntensity;
			TargetAngle = BossSpotlightAngle;
		}
	}

	public void BigBossSpotlightSettings ()
	{
		foreach (Light spotlight in SpotLights) 
		{
			TargetColor = BigBossSpotlightColor;
			TargetIntensity = BigBossSpotlightIntensity;
			TargetAngle = BigBossSpotlightAngle;
		}
	}

	public void ImpactSpotlightSettings ()
	{
		foreach (Light spotlight in SpotLights) 
		{
			TargetColor = ImpactSpotlightColor;
			TargetIntensity = ImpactSpotlightIntensity;
			TargetAngle = ImpactSpotlightAngle;
		}
	}

	public void SuccessSpotlightSettings ()
	{
		foreach (Light spotlight in SpotLights) 
		{
			TargetColor = SuccessSpotlightColor;
			TargetIntensity = SuccessSpotlightIntensity;
			TargetAngle = SuccessSpotlightAngle;
		}
	}

	public void OverrideSpotlightLookObject ()
	{
		// Loop through all spotlights, set new look at Transforms.
		foreach (SimpleLookAt lookatscript in SpotlightLookAt) 
		{
			lookatscript.LookAtPos = NewTarget;
		}
	}
}
