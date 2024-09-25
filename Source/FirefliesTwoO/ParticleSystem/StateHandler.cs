using RimWorld;
using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public static class StateHandler
    {
        public static bool IsActiveInHoursRange(Map map)
        {
            float currentHour = GenLocalDate.DayPercent(map) * 24f;
            bool active = (FFDefOf.FF_Config.startHour <= FFDefOf.FF_Config.endHour) ?
                (currentHour >= FFDefOf.FF_Config.startHour && currentHour < FFDefOf.FF_Config.endHour) :
                (currentHour >= FFDefOf.FF_Config.startHour || currentHour < FFDefOf.FF_Config.endHour);
            return active;
        }
        
        public static bool IsActiveBelowSunGlowThreshold(Map map)
        {
            float currentSunGlow = GenCelestial.CurCelestialSunGlow(map);
            bool active = currentSunGlow <= FFDefOf.FF_Config.sunGlowThreshold;
            return active;
        }
        
        public static void DestroyParticleSystem(ParticleSystem particleSystem)
        {
            if (particleSystem == null) return;
            //string system = _particleSystem.gameObject.name;
            Object.Destroy(particleSystem.gameObject);
            //FFLog.Message($"{particleSystem.gameObject.name} destroyed.");
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