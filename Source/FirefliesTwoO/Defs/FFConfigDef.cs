using System.Collections.Generic;
using RimWorld;
using Verse;

namespace FirefliesTwoO
{
    public class FFConfigDef : Def
    {
        // MapComponent_FFNightlySpawning
        public List<BiomeDef> allowedBiomes;
        
        public SimpleCurve particlesMaxCountCurve;
        public SimpleCurve particleEmissionRateCurve;
        
        public float particleLifetime = 1f;
        public float particleSizeFactor = 1f;
        public float startHour = 19f;
        public float endHour = 4f;

        // ParticleBuilder
        public FloatRange shapeRandomDirectionAmount = new (0f, 360f);
        public int noiseOctaveCount = 3;
        public float noiseFrequency = 1.5f;
        public float noisePositionAmount = 0.25f;
        public float noiseStrength = 10f;
    }
}