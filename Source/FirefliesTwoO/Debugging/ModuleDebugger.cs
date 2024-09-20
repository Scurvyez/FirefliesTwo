using UnityEngine;

namespace FirefliesTwoO
{
    public static class ModuleDebugger
    {
        public static void LogParticleLifetimes(ParticleSystem particleSys)
        {
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSys.main.maxParticles];
            int numParticlesAlive = particleSys.GetParticles(particles);
            
            for (int i = 0; i < numParticlesAlive; i++)
            {
                ParticleSystem.Particle particle = particles[i];
                float startLifetime = Mathf.Round(particle.startLifetime * 10f) / 10f;
                FFLog.Message($"Particle {i}: StartLifetime = {startLifetime:F2} seconds");
            }
        }
    }
}