using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    [StaticConstructorOnStartup]
    public class MapComponent_FFNightlySpawning : MapComponent
    {
        private ParticleSystem _ffParticleSystem;
        private ParticleMeshGenerator _meshGenerator;
        private bool _firefliesSpawned;
        private bool _isSystemActive;
        private float _simulationSpeed;
        private readonly Vector3 _mapSize;

        private List<IntVec3> _validEmissionCells;

        public MapComponent_FFNightlySpawning(Map map) : base(map)
        {
            _mapSize = new Vector3(map.Size.x, 1f, map.Size.z);
            // Run initialization via a LongEventHandler to combat intermittent crashing
            LongEventHandler.ExecuteWhenFinished(InitializeMapSystems);
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            if (!FFDefOf.FF_Config.allowedBiomes.Contains(map.Biome)) return;

            if (IsActive)
            {
                if (!_firefliesSpawned)
                {
                    SetParticleSystemState(true);
                }
            }
            else
            {
                if (!_firefliesSpawned) return;
                SetParticleSystemState(false);
                RecalculateMesh();
            }
        }

        public override void MapComponentUpdate()
        {
            if (!FFDefOf.FF_Config.allowedBiomes.Contains(map.Biome)) return;
            if (_ffParticleSystem == null) return;

            ParticleSystem.MainModule main = _ffParticleSystem.main;
            main.simulationSpeed = (float)Find.TickManager.CurTimeSpeed * FFDefOf.FF_Config.particleVelocityFactor;
            
            DEBUG_DrawSpawnableArea();
        }

        private bool IsActive
        {
            get
            {
                float currentHour = GenLocalDate.DayPercent(map) * 24f;
                bool active = (FFDefOf.FF_Config.startHour <= FFDefOf.FF_Config.endHour) ?
                    (currentHour >= FFDefOf.FF_Config.startHour && currentHour < FFDefOf.FF_Config.endHour) :
                    (currentHour >= FFDefOf.FF_Config.startHour || currentHour < FFDefOf.FF_Config.endHour);
                return active;
            }
        }

        private void InitializeMapSystems()
        {
            _meshGenerator = new ParticleMeshGenerator(map, IsPositionValid);

            if (!FFDefOf.FF_Config.allowedBiomes.Contains(map.Biome))
            {
                string allowedBiomeNames = string.Join(", ", FFDefOf.FF_Config.allowedBiomes.ConvertAll(biome => biome.defName));
                FFLog.Message($"The {map.Biome.defName} biome does not support firefly spawning. Supported biomes are: {allowedBiomeNames}");
                return;
            }

            if (_ffParticleSystem == null)
            {
                _ffParticleSystem = ParticleBuilder.CreateFireflyParticleSystem(_mapSize, null);
                _ffParticleSystem.transform.position = Vector3.zero;
            }

            RecalculateMesh();
            RestoreParticleSystemState();
        }

        private void RecalculateMesh()
        {
            if (_ffParticleSystem == null) return;
            Mesh spawnAreaMesh = _meshGenerator.CreateMeshFromValidCells();
            ParticleSystem.ShapeModule shapeModule = _ffParticleSystem.shape;
            shapeModule.mesh = spawnAreaMesh;

            FFLog.Message("Particle system mesh recalculated.");
        }

        private bool IsPositionValid(Vector3 position)
        {
            IntVec3 intVecPosition = position.ToIntVec3();
            float posLightAmount = map.glowGrid.GroundGlowAt(intVecPosition);
            return intVecPosition.InBounds(map) // in bounds
                   && !map.terrainGrid.TerrainAt(intVecPosition).IsWater // not water
                   && map.roofGrid.RoofAt(intVecPosition) == null // not roofed
                   && posLightAmount < 0.05f // low light
                   && map.thingGrid.ThingsAt(intVecPosition).All(thing =>
                       thing.def.category != ThingCategory.Building || // no buildings
                       thing.def.building.isNaturalRock); // no natural stone
        }

        private void RestoreParticleSystemState()
        {
            if (_ffParticleSystem == null) return;
            _ffParticleSystem.gameObject.SetActive(_isSystemActive);

            if (_isSystemActive)
            {
                _ffParticleSystem.Play();
            }
            else
            {
                _ffParticleSystem.Stop();
            }
            ParticleSystem.MainModule mainModule = _ffParticleSystem.main;
            mainModule.simulationSpeed = _simulationSpeed;
        }

        private void SetParticleSystemState(bool isActive)
        {
            if (_ffParticleSystem == null) return;
            if (isActive)
            {
                _ffParticleSystem.gameObject.SetActive(true);
                _ffParticleSystem.Play();
            }
            else
            {
                _ffParticleSystem.gameObject.SetActive(false);
                _ffParticleSystem.Stop();
            }
            _firefliesSpawned = isActive;
            _isSystemActive = isActive;
        }

        private void DEBUG_DrawSpawnableArea()
        {
            _validEmissionCells = _meshGenerator.GetValidCells();
            if (_validEmissionCells is { Count: > 0 })
            {
                GenDraw.DrawFieldEdges(_validEmissionCells, Color.yellow);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref _firefliesSpawned, "firefliesSpawned", defaultValue: false);
            Scribe_Values.Look(ref _isSystemActive, "isSystemActive", defaultValue: false);
            Scribe_Values.Look(ref _simulationSpeed, "simulationSpeed", defaultValue: 1f);
        }
    }
}
