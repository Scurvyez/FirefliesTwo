using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace FirefliesTwoO
{
    public class JobDriver_ChaseFireflies : JobDriver
    {
        private int tickCounter = 0;
        private int randomTickInterval;
        
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }
        
        protected override IEnumerable<Toil> MakeNewToils()
        {
            MapComponent_NightlySpawning mapComp = pawn.Map?.GetComponent<MapComponent_NightlySpawning>();
            if (mapComp == null || !mapComp.ParticlesSpawned)
            {
                EndJobWith(JobCondition.Incompletable);
                yield break;
            }

            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            randomTickInterval = Rand.RangeInclusive(30, 120);
            
            Toil chaseFireflies = new()
            {
                tickAction = delegate
                {
                    tickCounter++;
                    JoyUtility.JoyTickCheckEnd(pawn, JoyTickFullJoyAction.EndJob, 0.475f);

                    if (tickCounter < randomTickInterval) return;
                    if (RCellFinder.TryFindSkygazeCell(pawn.Position, pawn, out IntVec3 newCell))
                    {
                        pawn.pather.StartPath(newCell, PathEndMode.OnCell);
                    }
                    tickCounter = 0;
                    randomTickInterval = Rand.RangeInclusive(30, 120);
                },
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = job.def.joyDuration
            };

            chaseFireflies.AddFinishAction(() =>
            {
                if (!Rand.Chance(FFDefOf.FF_Config.caughtFirefliesChance)) return;
                pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(FFDefOf.FF_CaughtFireflies);
            });

            chaseFireflies.FailOn(() => pawn.Position.Roofed(pawn.Map));
            chaseFireflies.FailOn(() => !mapComp.ParticlesSpawned);
            yield return chaseFireflies;
        }

        public override string GetReport()
        {
            return "FF_ChasingFireflies".Translate();
        }
        
        // this is literally not needed one fucking bit as the job will never resume on load
        // :smugSip:
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref tickCounter, "tickCounter", 0);
            Scribe_Values.Look(ref randomTickInterval, "randomTickInterval", 60);
        }
    }
}