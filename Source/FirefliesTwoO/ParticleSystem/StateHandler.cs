using RimWorld;
using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public static class StateHandler
    {
        private static NightlySpawningExtension _ext;
        private static Season _curSeason;

        static StateHandler()
        {
            _ext = Find.CurrentMap.Biome
                .GetModExtension<NightlySpawningExtension>();
        }
        
        public static bool IsActive(Map map)
        {
            if (map == null || _ext == null) return false;
            _curSeason = GenLocalDate.Season(map);
            
            if (!_ext.biomeAllowInWinter 
                && (_curSeason is Season.Winter or Season.PermanentWinter)) 
                return false;
            
            if (map.mapTemperature.OutdoorTemp > _ext.biomeAllowedTempRange.max 
                || map.mapTemperature.OutdoorTemp < _ext.biomeAllowedTempRange.min) 
                return false;
            
            float currentSunGlow = GenCelestial.CurCelestialSunGlow(map);
            if (currentSunGlow > FFDefOf.FF_Config.sunGlowThreshold) 
                return false;
            
            return map.weatherManager.curWeather == WeatherDefOf.Clear;
        }
        
        public static void DestroyParticleSystem(ParticleSystem particleSystem)
        {
            if (particleSystem == null) return;
            Object.Destroy(particleSystem.gameObject);
        }
        
        public static void RestoreParticleSystemState(
            ParticleSystem particleSystem, bool isSystemActive, float simulationSpeed)
        {
            if (particleSystem == null) return;
            particleSystem.gameObject.SetActive(isSystemActive);

            if (isSystemActive)
            {
                particleSystem.Play();
            }
            else
            {
                particleSystem.Stop();
            }
            ParticleSystem.MainModule mainModule = particleSystem.main;
            mainModule.simulationSpeed = simulationSpeed;
        }

        public static void SetParticleSystemState(ParticleSystem particleSystem, bool isActive)
        {
            if (particleSystem == null) return;
            if (isActive)
            {
                particleSystem.gameObject.SetActive(true);
                particleSystem.Play();
            }
            else
            {
                particleSystem.gameObject.SetActive(false);
                particleSystem.Stop();
            }
        }
    }
}