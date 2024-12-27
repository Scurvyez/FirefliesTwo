using Verse;

namespace FirefliesTwoO
{
    public class FFConfigDef : Def
    {
        // MapComponent_FFNightlySpawning
        public float particleSizeFactor = 1f;
        public float sunGlowThreshold = 1f;

        // ParticleBuilder
        public FloatRange shapeRandomDirectionAmount = new (0f, 360f);
        public int noiseOctaveCount = 3;
        public float noiseFrequency = 1.5f;
        public float noisePositionAmount = 0.25f;
        public float noiseStrength = 10f;
        
        // Jobs
        public float chaseFirefliesJobFactor = 1f;
        public float caughtAFireflyChance = 1f;
    }
}