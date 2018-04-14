using UnityEngine;

public class ParticleEffectEmissionModifier : MonoBehaviour 
{
	public ParticleSystem particleEffect;
	private SaveAndLoadScript saveAndLoadScript;
	public float BaseRateDistance = 24;
	public float BaseRateTime = 24;

	void Start ()
	{
		//saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();
		FindComponents ();
	}

	void FindComponents ()
	{
		// Try to find save and load script.
		if (GameObject.Find ("SaveAndLoad") != null) 
		{
			saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();
		}

		// If no save and load script found, bail out.
		if (saveAndLoadScript == null) 
		{
			return;
		}

		// Find the particle effect component if null.
		if (particleEffect == null) 
		{
			particleEffect = GetComponent<ParticleSystem> ();
		}	

		// Found save and load script.
		if (saveAndLoadScript != null) 
		{
			InvokeRepeating ("RefreshParticleRates", 0.1f, 2);
		}
	}

	void RefreshParticleRates ()
	{
		// Emission rate modifier.
		var ParticleEmission = particleEffect.emission;

		// Set to use constant values.
		var ParticleEmissionRateoverTimeMode = ParticleEmission.rateOverTime.mode;
		var ParticleEmissionRateoverDistanceMode = ParticleEmission.rateOverTime.mode;

		ParticleEmissionRateoverTimeMode = ParticleSystemCurveMode.Constant;
		ParticleEmissionRateoverDistanceMode = ParticleSystemCurveMode.Constant;

		// Modify rate over distance.
		float newRateOverDist = BaseRateDistance * saveAndLoadScript.ParticleEmissionMultiplier;
		ParticleEmission.rateOverDistance = new ParticleSystem.MinMaxCurve (newRateOverDist);

		// Modify rate over time.
		float newRateOverTime = BaseRateTime * saveAndLoadScript.ParticleEmissionMultiplier;
		ParticleEmission.rateOverTime = new ParticleSystem.MinMaxCurve (newRateOverTime);

		// Max particles modifier.
		//var ParticleMax = particleEffect.main;
		//ParticleMax.maxParticles = Mathf.RoundToInt (ParticleMax.maxParticles * saveAndLoadScript.ParticleEmissionMultiplier);
	}
}