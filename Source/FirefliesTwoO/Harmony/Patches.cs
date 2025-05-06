using HarmonyLib;
using Verse;

namespace FirefliesTwoO
{
    [StaticConstructorOnStartup]
    public static class Patches
    {
        private const float GroundGlowThreshold = 0.5f;
        
        static Patches()
        {
            Harmony harmony = new ("rimworld.scurvyez.firefliestwoo");
            
            harmony.Patch(original: AccessTools.Method(typeof(Pawn), "TickRare"),
                postfix: new HarmonyMethod(typeof(Patches), nameof(PawnTickRarePostfix)));
        }
        
        private static void PawnTickRarePostfix(Pawn __instance)
        {
            if (!__instance.Spawned || !__instance.IsColonist || 
                __instance.IsColonyMech || __instance.DeadOrDowned) 
                return;
            
            if (__instance.needs?.mood?.thoughts?.memories?
                    .GetFirstMemoryOfDef(FFDefOf.FF_SawFireflies) != null) 
                return;
            
            MapComponent_NightlySpawning mapComp = __instance.Map
                .GetComponent<MapComponent_NightlySpawning>();
            
            if (mapComp is not { ParticlesSpawned: true }) 
                return;
            
            if (__instance.Map.glowGrid
                    .GroundGlowAt(__instance.Position) < GroundGlowThreshold)
            {
                __instance.needs?.mood?.thoughts?.memories?
                    .TryGainMemory(FFDefOf.FF_SawFireflies);
            }
        }
    }
}