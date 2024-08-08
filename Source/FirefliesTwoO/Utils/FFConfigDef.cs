using System.Collections.Generic;
using RimWorld;
using Verse;

namespace FirefliesTwoO
{
    public class FFConfigDef : Def
    {
        // MapComponent_FFNightlySpawning
        public List<BiomeDef> allowedBiomes;
        public int numFFSystems = 10;
        public int particlesMaxCount = 20;
        public float particleEmissionRate = 100f;
        public float particleLifetime = 0.1f;
        public float particleSizeFactor = 0.3f;
        public float particleVelocityFactor = 0.1f;
        public float startHour = 19f;
        public float endHour = 4f;
        public int spawnableAreaCellFactor = 13;
        public int minEdgeDistance = 20;

        // ParticleBuilder
        public FloatRange shapeRandomDirectionAmount = new (0f, 360f);
        public int noiseOctaveCount = 3;
        public float noiseFrequency = 1.5f;
        public float noisePositionAmount = 0.25f;
        public float noiseStrength = 10f;
    }
}