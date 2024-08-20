using RimWorld;
using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public static class State
    {
        public static bool IsActive(Map map)
        {
            float currentHour = GenLocalDate.DayPercent(map) * 24f;
            bool active = (FFDefOf.FF_Config.startHour <= FFDefOf.FF_Config.endHour) ?
                (currentHour >= FFDefOf.FF_Config.startHour && currentHour < FFDefOf.FF_Config.endHour) :
                (currentHour >= FFDefOf.FF_Config.startHour || currentHour < FFDefOf.FF_Config.endHour);
            return active;
        }
        
        public static void DestroyParticleSystem(ParticleSystem particleSystem)
        {
            if (particleSystem == null) return;
            //string system = _particleSystem.gameObject.name;
            Object.Destroy(particleSystem.gameObject);
            //FFLog.Message($"{system} destroyed.");
        }
    }
}