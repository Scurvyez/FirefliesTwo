using UnityEngine;

namespace FirefliesTwoO
{
    public static class ColorUtility
    {
        public static readonly Color RedEmission = new (1.0f, 0.4f, 0.4f);
        public static readonly Color GreenEmission = new (0.4f, 1.0f, 0.4f);
        public static readonly Color YellowEmission = new (1.0f, 1.0f, 0.4f);
        public static readonly Color PurpleEmission = new (0.8f, 0.6f, 1.0f);
        public static readonly Color BlueEmission = new (0.6f, 0.8f, 1.0f);

        public static readonly ParticleSystem.MinMaxGradient CommonColors = new (GreenEmission, YellowEmission);
        public static readonly ParticleSystem.MinMaxGradient UncommonColors = new (BlueEmission, RedEmission);
        public static readonly ParticleSystem.MinMaxGradient RareColors = new (BlueEmission, PurpleEmission);
    }
}