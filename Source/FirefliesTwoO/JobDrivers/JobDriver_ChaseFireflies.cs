using System.Collections.Generic;
using RimWorld;
using UnityEngine;
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
            
            yield return CreateGoToRandomEmissionCellToil(mapComp);
            yield return CreateChaseFirefliesToil(mapComp);
        }
        
        private Toil CreateGoToRandomEmissionCellToil(MapComponent_NightlySpawning mapComp)
        {
            return new Toil
            {
                initAction = delegate
                {
                    if (MapCellFinder.TryFindRandomEmissionCell(pawn.Position, pawn, mapComp, out IntVec3 newCell))
                    {
                        pawn.pather.StartPath(newCell, PathEndMode.OnCell);
                    }
                    else
                    {
                        EndJobWith(JobCondition.Incompletable);
                    }
                }
            };
        }
        
        private Toil CreateChaseFirefliesToil(MapComponent_NightlySpawning mapComp)
        {
            randomTickInterval = Rand.RangeInclusive(30, 120);

            Toil chaseFireflies = new Toil
            {
                tickAction = () => ExecuteChaseFirefliesTickAction(mapComp),
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = job.def.joyDuration
            };

            chaseFireflies.AddFinishAction(HandleFireflyCatchAttempt);
            chaseFireflies.FailOn(() => pawn.Position.Roofed(pawn.Map));
            chaseFireflies.FailOn(() => !mapComp.ParticlesSpawned);
            return chaseFireflies;
        }
        
        private void ExecuteChaseFirefliesTickAction(MapComponent_NightlySpawning mapComp)
        {
            tickCounter++;
            float joyGainFactor = pawn.story.traits.HasTrait(FFDefOf.NightOwl) ? 0.62f : 0.475f;
            JoyUtility.JoyTickCheckEnd(pawn, JoyTickFullJoyAction.EndJob, joyGainFactor);

            if (tickCounter < randomTickInterval) return;
            if (MapCellFinder.TryFindRandomEmissionCell(pawn.Position, pawn, mapComp, out IntVec3 newCell))
            {
                pawn.pather.StartPath(newCell, PathEndMode.OnCell);
            }

            tickCounter = 0;
            randomTickInterval = Rand.RangeInclusive(30, 120);
        }
        
        private void HandleFireflyCatchAttempt()
        {
            if (!Rand.Chance(FFDefOf.FF_Config.caughtAFireflyChance)) return;
            float baseChance = CalculateCatchChance();
            float finalChanceToCatch = Mathf.Clamp(baseChance, 0.1f, 1.0f);

            ThoughtDef thoughtToGain = Rand.Chance(finalChanceToCatch)
                ? FFDefOf.FF_CaughtAFirefly : FFDefOf.FF_SquishedAFirefly;
            pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(thoughtToGain);
        }
        
        private float CalculateCatchChance()
        {
            float baseChance = 1.0f;
            float sightFactor = pawn.health.capacities.GetLevel(PawnCapacityDefOf.Sight);
            float manipulationFactor = pawn.health.capacities.GetLevel(PawnCapacityDefOf.Manipulation);
            float normalizedMoveSpeedFactor = pawn.GetStatValue(StatDefOf.MoveSpeed) / 4.6f;
            float nimbleFactor = pawn.story.traits.HasTrait(FFDefOf.Nimble) ? 1.2f : 0.75f;

            baseChance *= sightFactor;
            baseChance *= manipulationFactor;
            baseChance *= normalizedMoveSpeedFactor;
            baseChance *= nimbleFactor;

            return baseChance;
        }

        public override string GetReport()
        {
            return "FF_ChasingFireflies".Translate();
        }
    }
}