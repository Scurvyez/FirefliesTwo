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
            MapComponent_NightlySpawning mapComp = pawn?.Map
                .GetComponent<MapComponent_NightlySpawning>();
            
            if (mapComp is not { ParticlesSpawned: true }
                || !JoyUtility.EnjoyableOutsideNow(pawn))
            {
                return null;
            }
            
            return !MapCellFinder.TryFindRandomEmissionCell(
                mapComp, pawn, out IntVec3 result, 45f) 
                ? null 
                : JobMaker.MakeJob(def.jobDef, result);
        }
    }
}