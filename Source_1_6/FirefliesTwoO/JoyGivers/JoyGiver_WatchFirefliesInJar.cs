using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace FirefliesTwoO
{
    public class JoyGiver_WatchFirefliesInJar : JoyGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            List<Thing> potentialTargets = pawn.Map.listerThings
                .ThingsOfDef(FFDefOf.FF_JarGlassA);
            
            foreach (Thing target in potentialTargets.InRandomOrder())
            {
                if (!pawn.Map.areaManager.Home[target.Position]) 
                    continue;
                
                if (!pawn.CanReach(target, PathEndMode.ClosestTouch, Danger.None)
                    || pawn.Map.reservationManager.IsReserved(target))
                    continue;
                
                Job job = JobMaker.MakeJob(def.jobDef, target);
                job.ignoreJoyTimeAssignment = false;
                return job;
            }
            return null;
        }
    }
}