using Verse;

namespace FirefliesTwoO
{
    public class NightlySpawningExtension : DefModExtension
    {
        public float biomeEmissionRate = 1f;
        public FloatRange biomeAllowedTempRange = new(-100f, 100f);
        public bool biomeAllowInWinter;
    }
}