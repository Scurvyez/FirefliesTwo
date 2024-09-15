using UnityEngine;

namespace FirefliesTwoO
{
    public static class ColorManager
    {
        public static readonly Color RedEmission = new (1.0f, 0.4f, 0.4f);
        public static readonly Color GreenEmission = new (0.4f, 1.0f, 0.4f);
        public static readonly Color YellowEmission = new (1.0f, 1.0f, 0.4f);
        public static readonly Color OrangeEmission = new (1.0f, 0.6f, 0.2f);
        public static readonly Color PurpleEmission = new (0.8f, 0.6f, 1.0f);
        public static readonly Color BlueEmission = new (0.6f, 0.8f, 1.0f);

        private static readonly ParticleSystem.MinMaxGradient CommonColors = new (GreenEmission, YellowEmission);
        private static readonly ParticleSystem.MinMaxGradient UncommonColors = new (BlueEmission, RedEmission);
        private static readonly ParticleSystem.MinMaxGradient RareColors = new (GreenEmission, OrangeEmission);
        private static readonly ParticleSystem.MinMaxGradient UniqueColors = new (BlueEmission, PurpleEmission);
        
        private static string _lastSelectedGradientName;

        public static string LastSelectedGradientName => _lastSelectedGradientName;
        
        public static void RecalculateBaseColorGradient(ParticleSystem particleSys)
        {
            ParticleSystem.MainModule mainModule = particleSys.main;
            ParticleSystem.MinMaxGradient mainModuleStartColor = mainModule.startColor;
            mainModuleStartColor.mode = ParticleSystemGradientMode.RandomColor;
            float rand = Random.value;
            
            mainModule.startColor = rand switch
            {
                > 0.3f => CommonColors,
                > 0.1f => UncommonColors,
                > 0.05f => RareColors,
                _ => UniqueColors
            };
            
            _lastSelectedGradientName = rand switch
            {
                > 0.3f => "CommonColors",
                > 0.1f => "UncommonColors",
                > 0.05f => "RareColors",
                _ => "UniqueColors"
            };
        }

        public static void AmplifyParticleShaderAlpha(ParticleSystem particleSys, float alphaFactor)
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