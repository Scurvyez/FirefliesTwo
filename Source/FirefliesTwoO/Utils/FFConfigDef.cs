using System.Collections.Generic;
using RimWorld;
using Verse;

namespace FirefliesTwoO
{
    public class FFConfigDef : Def
    {
        // MapComponent_FFNightlySpawning
        public List<BiomeDef> allowedBiomes; // TODO: CHANGE FOR RELEASE
        public int numFFSystems = 10; // TODO: CHANGE FOR RELEASE
        public int particlesMaxCount = 20; // TODO: CHANGE FOR RELEASE
        public float particleEmissionRate = 100f; // TODO: CHANGE FOR RELEASE
        public float particleLifetime = 0.1f; // TODO: CHANGE FOR RELEASE
        public float particleSizeFactor = 0.3f; // TODO: CHANGE FOR RELEASE
        public float particleVelocityFactor = 0.1f; // TODO: CHANGE FOR RELEASE?
        public float startHour = 19f; // TODO: CHANGE FOR RELEASE, make a setting?
        public float endHour = 4f; // TODO: CHANGE FOR RELEASE, make a setting?
        public int spawnableAreaCellFactor = 13; // TODO: CHANGE FOR RELEASE
        public int minEdgeDistance = 20; // TODO: CHANGE FOR RELEASE

        // ParticleBuilder
        public FloatRange shapeRandomDirectionAmount = new (0f, 360f);
        public int noiseOctaveCount = 3;
        public float noiseFrequency = 1.5f;
        public float noisePositionAmount = 0.25f;
        public float noiseStrength = 10f;
    }
}