using UnityEngine;

public class ParticleEffectEmissionModifier : MonoBehaviour 
{
	public ParticleSystem particleEffect;
	public float BaseRateDistance = 24;
	public float BaseRateTime = 24;

	void Awake ()
	{
		FindComponents ();
	}

	void FindComponents ()
	{
		// If no save and load script found, bail out.
		if (SaveAndLoadScript.Instance == null) 
		{
			return;
		}

		// Find the particle effect component if null.
		if (particleEffect == null) 
		{
			particleEffect = GetComponent<ParticleSystem> ();
		}	

		// Found save and load script.
		if (SaveAndLoadScript.Instance != null) 
		{
			RefreshParticleRates ();
			RefreshBursts ();
			RefreshMaxParticles ();
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
		float newRateOverDist = BaseRateDistance * SaveAndLoadScript.Instance.ParticleEmissionMultiplier;
		ParticleEmission.rateOverDistance = new ParticleSystem.MinMaxCurve (newRateOverDist);

		// Modify rate over time.
		float newRateOverTime = BaseRateTime * SaveAndLoadScript.Instance.ParticleEmissionMultiplier;
		ParticleEmission.rateOverTime = new ParticleSystem.MinMaxCurve (newRateOverTime);

		// Max particles modifier.
		//var ParticleMax = particleEffect.main;
		//ParticleMax.maxParticles = Mathf.RoundToInt (ParticleMax.maxParticles * SaveAndLoadScript.Instance.ParticleEmissionMultiplier);
	}

	void RefreshBursts ()
	{
		int ParticleBurstCount = particleEffect.emission.burstCount; // Get burst count in particle effect.

		var burstEmission = particleEffect.emission; // Reference emission module.

		// Loop through bursts and set new values.
		for (int i = 0; i < ParticleBurstCount; i++)
		{
			burstEmission.SetBurst (
				i, 
				new ParticleSystem.Burst (
					burstEmission.GetBurst (i).time,
					burstEmission.GetBurst (i).count.constant * SaveAndLoadScript.Instance.ParticleEmissionMultiplier
				)
			);
		}
	}

	void RefreshMaxParticles ()
	{
		int MaxParticles = particleEffect.main.maxParticles;

		var ParticleMainModule = particleEffect.main;

		ParticleMainModule.maxParticles *= Mathf.RoundToInt (SaveAndLoadScript.Instance.ParticleEmissionMultiplier * 3);
	}
}