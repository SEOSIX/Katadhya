using UnityEngine;

public class Align2DParticlesToVelocity : MonoBehaviour
{
    ParticleSystem ps;
    ParticleSystem.Particle[] particles;
    [SerializeField] int offset;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
    }

    void LateUpdate()
    {
        int count = ps.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            Vector3 vel = particles[i].velocity;

            if (vel.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
                particles[i].rotation3D = new Vector3(0, 0, angle + offset);
            }
        }

        ps.SetParticles(particles, count);
    }
}
