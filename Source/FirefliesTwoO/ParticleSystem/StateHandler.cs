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

        /// <summary>
        /// Determines if the system is currently active for a given <see cref="Map"/> based on weather, temperature, season, and sunlight conditions.
        /// </summary>
        /// <param name="map">
        /// The <see cref="Map"/> for which the active state is to be determined. If null, the method returns false.
        /// </param>
        /// <returns>
        /// A boolean indicating whether the system is active for the provided map.
        /// </returns>
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

        /// <summary>
        /// Destroys a given particle system by removing its associated GameObject from the scene.
        /// </summary>
        /// <param name="particleSystem">
        /// The <see cref="ParticleSystem"/> to be destroyed. If null, the method performs no actions.
        /// </param>
        public static void DestroyParticleSystem(ParticleSystem particleSystem)
        {
            if (particleSystem == null) return;
            Object.Destroy(particleSystem.gameObject);
        }

        /// <summary>
        /// Restores the state of a given particle system, including its active status and simulation speed.
        /// </summary>
        /// <param name="particleSystem">
        /// The <see cref="ParticleSystem"/> to modify. If null, the method performs no actions.
        /// </param>
        /// <param name="isSystemActive">
        /// A boolean value indicating whether the particle system should be active or inactive.
        /// If true, the particle system is activated and starts playing. If false, it is deactivated and stopped.
        /// </param>
        /// <param name="simulationSpeed">
        /// The speed at which the particle system simulates, controlling the speed of particle emission and movement.
        /// </param>
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
        
        /// <summary>
        /// Sets the activation state of the specified particle system.
        /// </summary>
        /// <param name="particleSystem">
        /// The <see cref="ParticleSystem"/> to modify. If null, the method performs no actions.
        /// </param>
        /// <param name="isActive">
        /// A boolean value indicating whether the particle system should be active or inactive.
        /// If true, the particle system is activated and starts playing. If false, it is deactivated and stopped.
        /// </param>
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