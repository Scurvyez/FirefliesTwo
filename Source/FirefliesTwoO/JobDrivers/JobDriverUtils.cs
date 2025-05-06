using RimWorld;
using Verse;

namespace FirefliesTwoO
{
    public static class JobDriverUtils
    {
        public static float CalculateCatchChance(Pawn pawn)
        {
            float baseChance = 1.0f;
            float sightFactor = pawn.health.capacities
                .GetLevel(PawnCapacityDefOf.Sight);
            float manipulationFactor = pawn.health.capacities
                .GetLevel(PawnCapacityDefOf.Manipulation);
            float normalizedMoveSpeedFactor = pawn
                .GetStatValue(StatDefOf.MoveSpeed) / 4.6f;
            float nimbleFactor = pawn.story.traits
                .HasTrait(FFDefOf.Nimble) ? 1.2f : 0.75f;
            
            baseChance *= sightFactor;
            baseChance *= manipulationFactor;
            baseChance *= normalizedMoveSpeedFactor;
            baseChance *= nimbleFactor;
            
            return baseChance;
        }
    }
}