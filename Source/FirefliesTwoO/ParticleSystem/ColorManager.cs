using UnityEngine;

namespace FirefliesTwoO
{
    public static class ColorManager
    {
        public static readonly Color GreenEmission = new (0.4f, 1.0f, 0.4f);
        public static readonly Color YellowEmission = new (1.0f, 1.0f, 0.4f);
        public static readonly Color OrangeEmission = new (1.0f, 0.6f, 0.2f);
        public static readonly Color RedEmission = new (1.0f, 0.4f, 0.4f);
        public static readonly Color BlueEmission = new (0.6f, 0.8f, 1.0f);
        public static readonly Color PurpleEmission = new (0.8f, 0.6f, 1.0f);

        private static readonly Gradient colorGradient = new ()
        {
            colorKeys =
            [
                new GradientColorKey(GreenEmission, 0.5f),
                new GradientColorKey(YellowEmission, 0.8f),
                new GradientColorKey(OrangeEmission, 0.9f),
                new GradientColorKey(RedEmission, 0.95f),
                new GradientColorKey(BlueEmission, 0.975f),
                new GradientColorKey(PurpleEmission, 1f)
            ]
        };
        
        public static void GetBaseColorGradient(ParticleSystem particleSys)
        {
            ParticleSystem.MainModule mainModule = particleSys.main;
            ParticleSystem.MinMaxGradient gradientColor = new (colorGradient);
            mainModule.startColor = gradientColor;
        }

        public static void SetParticleAlpha(ParticleSystem particleSys, float alphaFactor)
        {
            Renderer particleRenderer = particleSys.GetComponent<Renderer>();
            if (particleRenderer != null && particleRenderer.material.HasProperty("_Color"))
            {
                Color currentColor = particleRenderer.material.GetColor(Shader.PropertyToID("_Color"));
                currentColor.a *= alphaFactor;
                particleRenderer.material.SetColor(Shader.PropertyToID("_Color"), currentColor);
            }
            else
            {
                FFLog.Message("No _Color property found on the particle material!");
            }
        }
    }
}