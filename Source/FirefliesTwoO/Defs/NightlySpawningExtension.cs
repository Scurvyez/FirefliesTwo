using Verse;

namespace FirefliesTwoO
{
    public class NightlySpawningExtension : DefModExtension
    {
        public float biomeEmissionRate = 0f;
        public FloatRange biomeAllowedTempRange = FloatRange.Zero;
        public bool biomeAllowInWinter;
    }
}