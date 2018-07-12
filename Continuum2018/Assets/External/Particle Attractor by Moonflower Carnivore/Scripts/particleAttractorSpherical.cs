using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
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

	public bool AutoFindPlayer;

	void Start () 
	{
		ps = GetComponent<ParticleSystem>();

		if (!GetComponent<Transform>())
		{
			GetComponent<Transform>();
		}

		if (AutoFindPlayer == true) 
		{
			FindAttractor (PlayerController.PlayerOneInstance.playerCol.transform, speed, initialDelay);
		}
	}

	void Update () 
	{
		if (lifetime < initialDelay) 
		{
			lifetime += Time.deltaTime;
			return;
		}

		CheckParticlesToAttract ();
	}

	void CheckParticlesToAttract ()
	{
		m_Particles = new ParticleSystem.Particle[ps.main.maxParticles];
		numParticlesAlive = ps.GetParticles(m_Particles);

		float step = speed * Time.deltaTime;

		if (target != null)
		{
			for (int i = 0; i < numParticlesAlive; i++)
			{
				m_Particles[i].position = Vector3.LerpUnclamped (m_Particles[i].position, target.position, step);
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