using UnityEngine;

namespace FirefliesTwoO
{
    public class RotationHandler : MonoBehaviour
    {
        private ParticleSystem _particleSystem;
        private ParticleSystem.Particle[] _particles;

        private void Start()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            _particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
        }

        private void LateUpdate()
        {
            int particleCount = _particleSystem.GetParticles(_particles);

            for (int i = 0; i < particleCount; i++)
            {
                // Get the current particle velocity
                Vector3 velocity = _particles[i].velocity;

                // Calculate the angle in radians for 3D rotation around the Y-axis
                float angle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;

                // Set the particle's rotation around the Y-axis
                _particles[i].rotation3D = new Vector3(0f, angle, 0f); // Rotate only on Y-axis
            }

            // Apply the updated particle rotation
            _particleSystem.SetParticles(_particles, particleCount);
        }
    }
}