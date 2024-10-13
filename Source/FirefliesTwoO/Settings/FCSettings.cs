using System.Collections.Generic;
using Verse;

namespace FirefliesTwoO
{
    public class FCSettings : ModSettings
    {
        private static FCSettings _instance;

        public Dictionary<string, float> biomeSpawnRates = new();
        private List<string> biomeKeys;
        private List<float> spawnRateValues;
        
        public FCSettings()
        {
            _instance = this;
        }
        
        public override void ExposeData()
        {
            base.ExposeData();
            biomeSpawnRates ??= new Dictionary<string, float>();
            Scribe_Collections.Look(ref biomeSpawnRates, "biomeSpawnRates", LookMode.Value, LookMode.Value, ref biomeKeys, ref spawnRateValues);
        }
    }
}