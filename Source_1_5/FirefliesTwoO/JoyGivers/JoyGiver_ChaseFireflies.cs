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
            Map map = pawn?.Map;
            MapComponent_NightlySpawning mapComp = map?.GetComponent<MapComponent_NightlySpawning>();
            
            if (mapComp is not { ParticlesSpawned: true }
                || !JoyUtility.EnjoyableOutsideNow(pawn))
            {
                return null;
            }
            if (PawnUtility.WillSoonHaveBasicNeed(pawn))
            {
                return null;
            }
            if (map.dangerWatcher.DangerRating >= StoryDanger.High)
            {
                return null;
            }
            if (!MapCellFinder.TryFindRandomEmissionCell(mapComp, pawn, out IntVec3 result, 45f))
            {
                return null;
            }
            if (PawnUtility.KnownDangerAt(result, map, pawn))
            {
                return null;
            }
            
            return JobMaker.MakeJob(def.jobDef, result);
        }
    }
}