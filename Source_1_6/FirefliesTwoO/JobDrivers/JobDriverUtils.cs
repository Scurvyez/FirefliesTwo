using RimWorld;
using Verse;

namespace FirefliesTwoO
{
    public static class JobDriverUtils
    {
        /// Calculates the chance of successfully catching fireflies based on the pawn's abilities and traits.
        /// <param name="pawn">The <see cref="Pawn"/> attempting to catch fireflies. Used to determine factors such as sight, manipulation, movement speed, and traits.</param>
        /// <returns>A float value representing the calculated chance of success, ranging between 0 and 1.</returns>
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