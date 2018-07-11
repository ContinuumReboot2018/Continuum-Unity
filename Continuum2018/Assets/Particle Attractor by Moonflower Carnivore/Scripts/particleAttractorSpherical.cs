using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class particleAttractorSpherical : MonoBehaviour 
{
	ParticleSystem ps;
	ParticleSystem.Particle[] m_Particles;

	public Transform target;
	public float speed = 5f;
	public float initialDelay;
	private float lifetime;
	int numParticlesAlive;

	void Start () 
	{
		ps = GetComponent<ParticleSystem>();

		if (!GetComponent<Transform>())
		{
			GetComponent<Transform>();
		}
	}

	void Update () 
	{
		if (lifetime < initialDelay) 
		{
			lifetime += Time.deltaTime;
			return;
		}

		m_Particles = new ParticleSystem.Particle[ps.main.maxParticles];
		numParticlesAlive = ps.GetParticles(m_Particles);

		float step = speed * Time.deltaTime;

		if (target != null)
		{
			for (int i = 0; i < numParticlesAlive; i++)
			{
				m_Particles[i].position = Vector3.SlerpUnclamped (m_Particles[i].position, target.position, step);
			}
		}

		ps.SetParticles (m_Particles, numParticlesAlive);
	}

	public void FindAttractor (Transform attractor, float particleSpeed, float delay)
	{
		target = attractor;
		speed = particleSpeed;
		initialDelay = delay;
	}
}