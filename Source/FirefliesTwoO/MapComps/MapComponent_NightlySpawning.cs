using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public class MapComponent_NightlySpawning : MapComponent
    {
        private const float MaxParticlesMultiplier = 0.5f;
        private const float EmissionRateBase = 0.25f;
        private const float EmissionRatePower = 2.7f;
        private const float ParticleAlpha = 2.5f;

        private ParticleSystem _particleSystem;
        private MeshManager _meshManager;
        private List<IntVec3> _validEmissionCells;
        private Mesh _spawnAreaMesh;
        private bool _particlesSpawned;
        private bool _isSystemActive;
        private float _simulationSpeed;
        private int _mapID;
        private bool _allColumnsValidated;

        private static bool DrawMeshNow => MeshOverlayDrawer.DrawFireflySpawnMesh;
        
        public MapComponent_NightlySpawning(Map map) : base(map)
        {
            LongEventHandler.ExecuteWhenFinished(InitializeMapSystems);
        }

        public override void MapRemoved()
        {
            base.MapRemoved();
            StateHandler.DestroyParticleSystem(_particleSystem);
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            
            if (_particleSystem == null) return;
            bool isActive = StateHandler.IsActive(map);

            switch (isActive)
            {
                case true when !_particlesSpawned:
                    ActivateParticleSystem();
                    break;
                case false when _particlesSpawned:
                    DeactivateParticleSystem();
                    break;
            }
            
            // If particle system is inactive, no need to proceed further
            if (!isActive) return;

            // Validate cells and construct mesh
            if (_allColumnsValidated) return;
            _allColumnsValidated = _meshManager.ValidateCells();
            
            if (!_allColumnsValidated) return;
            // Only construct mesh if all columns have been validated
            _meshManager.ConstructMesh(_spawnAreaMesh);
            _validEmissionCells = _meshManager.FinalValidCells;

            // Set particle system shape after all columns are validated
            ParticleSystem.ShapeModule shapeModule = _particleSystem.shape;
            shapeModule.mesh = _spawnAreaMesh;
            
            // Set particle system parameters after all cells have been validated
            UpdateParticleSystemParameters();
        }

        public override void MapComponentUpdate()
        {
            if (!map.IsPlayerHome || _particleSystem == null) return;
            UpdateSimulationSpeed();

            if (!DrawMeshNow) return;
            if (_validEmissionCells is { Count: > 0 })
            {
                MeshOverlayDrawer.DrawMeshArea(_validEmissionCells);
            }
        }

        private void InitializeMapSystems()
        {
            if (_particleSystem != null || !FFDefOf.FF_Config.allowedBiomes.Contains(map.Biome)) return;

            _mapID = map.GetHashCode();
            _spawnAreaMesh = new Mesh();
            _meshManager = new MeshManager(map, IsPositionValid);
            _particleSystem = Builder.CreateFireflyParticleSystem(_mapID);
            _particleSystem.transform.position = Vector3.zero;

            ColorManager.GetBaseColorGradient(_particleSystem);
            ColorManager.SetParticleAlpha(_particleSystem, ParticleAlpha);
            StateHandler.RestoreParticleSystemState(_particleSystem, _isSystemActive, _simulationSpeed);
        }

        private bool IsPositionValid(Vector3 position)
        {
            IntVec3 intVecPosition = position.ToIntVec3();
            return !intVecPosition.InNoZoneEdgeArea(map) &&
                   !intVecPosition.Fogged(map) &&
                   !intVecPosition.Roofed(map) &&
                   map.terrainGrid.TerrainAt(intVecPosition).IsSoil &&
                   Rand.Value <= 0.33f &&
                   intVecPosition.Standable(map) &&
                   !intVecPosition.IsPolluted(map);
        }

        private void ActivateParticleSystem()
        {
            StateHandler.SetParticleSystemState(_particleSystem, true);
            _particlesSpawned = true;
            _isSystemActive = true;
        }

        private void DeactivateParticleSystem()
        {
            StateHandler.SetParticleSystemState(_particleSystem, false);
            _particlesSpawned = false;
            _isSystemActive = false;
            _allColumnsValidated = false;
            _validEmissionCells?.Clear();
            _meshManager.Reset();
            _spawnAreaMesh.Clear();
        }

        private void UpdateParticleSystemParameters()
        {
            ParticleSystem.MainModule main = _particleSystem.main;
            ParticleSystem.EmissionModule emission = _particleSystem.emission;

            main.maxParticles = Mathf.FloorToInt(_validEmissionCells.Count * MaxParticlesMultiplier);
            emission.rateOverTime = Mathf.Max(4, 
                Mathf.FloorToInt(_validEmissionCells.Count * Mathf.Pow(EmissionRateBase, EmissionRatePower)));
        }

        private void UpdateSimulationSpeed()
        {
            ParticleSystem.MainModule main = _particleSystem.main;
            main.simulationSpeed = (float)Find.TickManager.CurTimeSpeed * 1f;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref _particlesSpawned, "firefliesSpawned", defaultValue: false);
            Scribe_Values.Look(ref _isSystemActive, "isSystemActive", defaultValue: false);
            Scribe_Values.Look(ref _simulationSpeed, "simulationSpeed", defaultValue: 1f);
        }
    }
}