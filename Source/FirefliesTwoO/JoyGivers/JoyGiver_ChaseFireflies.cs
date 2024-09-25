using RimWorld;
using Verse;
using Verse.AI;

namespace FirefliesTwoO
{
    public class JoyGiver_ChaseFireflies : JoyGiver
    {
        public override float GetChance(Pawn pawn)
        {
            float chanceFactor = FFDefOf.FF_Config.chaseFirefliesJobFactor;
            return base.GetChance(pawn) * chanceFactor;
        }

        public override Job TryGiveJob(Pawn pawn)
        {
            MapComponent_NightlySpawning mapComp = pawn.Map.GetComponent<MapComponent_NightlySpawning>();
            if (!JoyUtility.EnjoyableOutsideNow(pawn) || !mapComp.ParticlesSpawned)
            {
                return null;
            }
            return !RCellFinder.TryFindSkygazeCell(pawn.Position, pawn, out IntVec3 result) ? null : JobMaker.MakeJob(def.jobDef, result);
        }
    }
}