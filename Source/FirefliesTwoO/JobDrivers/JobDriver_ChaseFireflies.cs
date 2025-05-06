using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace FirefliesTwoO
{
    public class JobDriver_ChaseFireflies : JobDriver
    {
        private int _tickCounter = 0;
        private int _randomTickInterval;
        private int _waitCounter = 0;
        private bool _waitingAtCell = false;
        
        private const TargetIndex DestCellInd = TargetIndex.A;
        private const int ChaseRadius = 10;
        private const int ChaseTicksMin = 30;
        private const int ChaseTicksMax = 120;
        private const int WaitTicksAtCell = 120;
        
        public override string GetReport() => "FF_ChasingFireflies".Translate();
        
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(DestCellInd), job, 
                1, -1, null, errorOnFailed);
        }
        
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnBurningImmobile(DestCellInd);
            
            MapComponent_NightlySpawning mapComp = pawn.Map?
                .GetComponent<MapComponent_NightlySpawning>();
            
            if (mapComp is not { ParticlesSpawned: true })
            {
                EndJobWith(JobCondition.Incompletable);
                yield break;
            }
            
            yield return Toils_Reserve.Reserve(DestCellInd);
            yield return Toils_Goto.GotoCell(DestCellInd, PathEndMode.OnCell);
            yield return ChaseFirefliesToil(mapComp, job.GetTarget(DestCellInd).Cell);
        }
        
        private Toil ChaseFirefliesToil(MapComponent_NightlySpawning mapComp, 
            IntVec3 centerCell)
        {
            Toil chaseFireflies = new()
            {
                initAction = () =>
                {
                    ChaseFirefliesToilInitAction(mapComp, centerCell);
                },
                tickAction = () =>
                {
                    ChaseFirefliesToilTickAction(mapComp);
                }
            };
            chaseFireflies.FailOn(() => pawn.Position.Roofed(pawn.Map));
            chaseFireflies.FailOn(() => !JoyUtility.EnjoyableOutsideNow(pawn));
            chaseFireflies.FailOn(() => !mapComp.ParticlesSpawned);
            chaseFireflies.AddFinishAction(() =>
            {
                mapComp.ValidChaseCells.Clear();
                FireflyCatchAttempt();
            });
            chaseFireflies.defaultCompleteMode = ToilCompleteMode.Delay;
            chaseFireflies.defaultDuration = job.def.joyDuration;
            return chaseFireflies;
        }
        
        private void ChaseFirefliesToilInitAction(MapComponent_NightlySpawning mapComp, IntVec3 centerCell)
        {
            _tickCounter = 0;
            _randomTickInterval = Rand.RangeInclusive(ChaseTicksMin, ChaseTicksMax);
                    
            List<IntVec3> chaseCells = MapCellFinder
                .TryGetValidEmissionCellsInRadius(
                    mapComp, pawn, centerCell, ChaseRadius);
            mapComp.ValidChaseCells = chaseCells;
                    
            if (chaseCells.NullOrEmpty())
            {
                EndJobWith(JobCondition.Incompletable);
            }
        }
        
        private void ChaseFirefliesToilTickAction(MapComponent_NightlySpawning mapComp)
        {
            float joyGainFactor = pawn.story.traits
                .HasTrait(FFDefOf.NightOwl) ? 0.62f : 0.475f;
            JoyUtility.JoyTickCheckEnd(pawn, 
                JoyTickFullJoyAction.EndJob, joyGainFactor);
            
            if (_waitingAtCell)
            {
                _waitCounter++;
                if (_waitCounter < WaitTicksAtCell)
                    return;
                
                _waitingAtCell = false;
                _waitCounter = 0;
                return;
            }
            
            if (_tickCounter++ >= _randomTickInterval)
            {
                _tickCounter = 0;
                _randomTickInterval = Rand.RangeInclusive(ChaseTicksMin, ChaseTicksMax);
                if (mapComp.ValidChaseCells.NullOrEmpty()) 
                    return;
                
                IntVec3 newChaseCell = mapComp.ValidChaseCells.RandomElement();
                if (pawn.Position != newChaseCell)
                {
                    pawn.pather?.StartPath(newChaseCell, PathEndMode.OnCell);
                }
                _waitingAtCell = true;
                _waitCounter = 0;
            }
        }
        
        private void FireflyCatchAttempt()
        {
            ThoughtDef thoughtToGain;
            if (Rand.Chance(FFDefOf.FF_Config.caughtAFireflyChance))
            {
                float baseChance = JobDriverUtils.CalculateCatchChance(pawn);
                float finalChanceToCatch = Mathf.Clamp(baseChance, 0.1f, 1.0f);
            
                thoughtToGain = Rand.Chance(finalChanceToCatch)
                    ? FFDefOf.FF_CaughtAFirefly 
                    : FFDefOf.FF_SquishedAFirefly;
            }
            else
            {
                thoughtToGain = FFDefOf.FF_ChasedFireflies;
            }
            
            if (thoughtToGain == null)
                return;
            
            pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(thoughtToGain);
        }
    }
}