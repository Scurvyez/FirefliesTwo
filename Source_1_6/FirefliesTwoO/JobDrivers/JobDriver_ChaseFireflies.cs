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
        private Mote _bugnetMote;
        
        private const TargetIndex DestCellInd = TargetIndex.A;
        private const int ChaseRadius = 6;
        private const int ChaseTicksMin = 30;
        private const int ChaseTicksMax = 120;
        private const int WaitTicksAtCell = 60;
        
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
                    TryDoChaseEffect();
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
                .HasTrait(FFDefOf.NightOwl) ? 1.45f : 1f;
            JoyUtility.JoyTickCheckEnd(pawn, 1,
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
        
        private void TryDoChaseEffect()
        {
            float yOffset = (pawn.Position.x % 2 + pawn.Position.z % 2) / 10f;
            if (_bugnetMote == null || _bugnetMote.Destroyed)
            {
                _bugnetMote = MoteMaker.MakeAttachedOverlay(pawn, 
                    FFDefOf.FF_Mote_BugNet, Vector3.zero);
                _bugnetMote.link1.rotateWithTarget = true;
                _bugnetMote.yOffset = yOffset;
            }
            _bugnetMote.Maintain();
        }
        
        private void FireflyCatchAttempt()
        {
            ThoughtDef thoughtToGain;
            bool caught = false;
            
            if (Rand.Chance(FFDefOf.FF_Config.caughtAFireflyChance))
            {
                float baseChance = JobDriverUtils.CalculateCatchChance(pawn);
                float finalChanceToCatch = Mathf.Clamp01(baseChance);
                
                caught = Rand.Chance(finalChanceToCatch);
                thoughtToGain = caught
                    ? FFDefOf.FF_CaughtAFirefly 
                    : FFDefOf.FF_SquishedAFirefly;
            }
            else
            {
                thoughtToGain = FFDefOf.FF_ChasedFireflies;
            }
            
            if (thoughtToGain != null)
            {
                pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(thoughtToGain);
            }
            
            ThingDef fireflyDef = FFDefOf.FF_JarGlassA;
            if (!caught || pawn.inventory.innerContainer.Contains(fireflyDef, 1)) 
                return;
            
            Thing firefly = ThingMaker.MakeThing(fireflyDef);
            pawn.inventory?.innerContainer?.TryAdd(firefly);
        }
    }
}