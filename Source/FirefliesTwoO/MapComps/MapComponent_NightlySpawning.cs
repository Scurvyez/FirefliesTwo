using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public class MapComponent_NightlySpawning : MapComponent
    {
        private const float MaxParticlesMultiplier = 0.5f;
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
        }

        public override void MapComponentUpdate()
        {
            if (!map.IsPlayerHome || _particleSystem == null) return;
            UpdateSimulationSpeed();

            if (DrawMeshNow)
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

            InitializeParticleSystem();
        }

        private void InitializeParticleSystem()
        {
            ParticleSystem.MainModule main = _particleSystem.main;
            ParticleSystem.EmissionModule emission = _particleSystem.emission;

            RecalculateMesh();
            main.maxParticles = Mathf.FloorToInt(_validEmissionCells.Count * MaxParticlesMultiplier);
            emission.rateOverTime = Mathf.Max(4, Mathf.FloorToInt(_validEmissionCells.Count * Mathf.Pow(0.25f, EmissionRatePower)));

            FFLog.Message($"Emission Cells Count: {_validEmissionCells.Count}");
            FFLog.Message($"Particle Count: {main.maxParticles}");
            FFLog.Message($"Emission Rate: {emission.rateOverTime.constant}");

            ColorManager.GetBaseColorGradient(_particleSystem);
            ColorManager.SetParticleAlpha(_particleSystem, ParticleAlpha);
            StateHandler.RestoreParticleSystemState(_particleSystem, _isSystemActive, _simulationSpeed);
        }

        private void RecalculateMesh()
        {
            if (_particleSystem == null) return;
            
            _meshManager.UpdateMeshFromValidCells(_spawnAreaMesh);
            _validEmissionCells = _meshManager.FinalValidCells;

            ParticleSystem.ShapeModule shapeModule = _particleSystem.shape;
            shapeModule.mesh = _spawnAreaMesh;
        }

        private bool IsPositionValid(Vector3 position)
        {
            IntVec3 intVecPosition = position.ToIntVec3();
            return intVecPosition.ToVector3().InBounds(map) &&
                   map.terrainGrid.TerrainAt(intVecPosition).IsSoil &&
                   !intVecPosition.Roofed(map) &&
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
            UpdateParticleSystemParameters();
        }

        private void UpdateParticleSystemParameters()
        {
            RecalculateMesh();
            ParticleSystem.MainModule main = _particleSystem.main;
            ParticleSystem.EmissionModule emission = _particleSystem.emission;

            main.maxParticles = Mathf.FloorToInt(_validEmissionCells.Count * MaxParticlesMultiplier);
            emission.rateOverTime = Mathf.Max(4, Mathf.FloorToInt(_validEmissionCells.Count * Mathf.Pow(0.25f, EmissionRatePower)));

            FFLog.Message($"Updated Particle Count: {main.maxParticles}");
            FFLog.Message($"Updated Emission Rate: {emission.rateOverTime.constant}");
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