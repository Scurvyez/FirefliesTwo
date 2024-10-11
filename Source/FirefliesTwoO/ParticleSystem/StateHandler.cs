using RimWorld;
using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public static class StateHandler
    {
        public static bool IsActive(Map map)
        {
            if (map == null) return false;
            
            Season currentSeason = GenLocalDate.Season(map);
            if (currentSeason is Season.Winter or Season.PermanentWinter) return false;
            if (map.mapTemperature.OutdoorTemp < FFDefOf.FF_Config.outdoorTempThreshold) return false;
            
            float currentSunGlow = GenCelestial.CurCelestialSunGlow(map);
            if (currentSunGlow > FFDefOf.FF_Config.sunGlowThreshold) return false;
            
            return map.weatherManager.curWeather == WeatherDefOf.Clear;
        }
        
        public static void DestroyParticleSystem(ParticleSystem particleSystem)
        {
            if (particleSystem == null) return;
            Object.Destroy(particleSystem.gameObject);
        }
        
        public static void RestoreParticleSystemState(ParticleSystem particleSystem, bool isSystemActive, float simulationSpeed)
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