using UnityEngine;

namespace FirefliesTwoO
{
    public static class ColorManager
    {
        private static readonly Color RedEmission = new (1.0f, 0.4f, 0.4f);
        private static readonly Color GreenEmission = new (0.4f, 1.0f, 0.4f);
        private static readonly Color YellowEmission = new (1.0f, 1.0f, 0.4f);
        private static readonly Color OrangeEmission = new (1.0f, 0.6f, 0.2f);
        private static readonly Color PurpleEmission = new (0.8f, 0.6f, 1.0f);
        private static readonly Color BlueEmission = new (0.6f, 0.8f, 1.0f);

        private static readonly ParticleSystem.MinMaxGradient CommonColors = new (GreenEmission, YellowEmission);
        private static readonly ParticleSystem.MinMaxGradient UncommonColors = new (BlueEmission, RedEmission);
        private static readonly ParticleSystem.MinMaxGradient RareColors = new (GreenEmission, OrangeEmission);
        private static readonly ParticleSystem.MinMaxGradient UniqueColors = new (BlueEmission, PurpleEmission);
        
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
        }
    }
}