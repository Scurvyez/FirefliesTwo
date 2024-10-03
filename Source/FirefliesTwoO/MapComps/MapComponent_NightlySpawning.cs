using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public class MapComponent_NightlySpawning : MapComponent
    {
        public bool ParticlesSpawned;
        
        private const float MaxParticlesMultiplier = 0.5f;
        private const float EmissionRateBase = 0.25f;
        private const float EmissionRatePower = 2.7f;
        private const float ParticleAlpha = 2.5f;

        private NightlySpawningExtension _ext;
        private float _biomeEmissionRateFactor;
        private ParticleSystem _particleSystem;
        private MeshManager _meshManager;
        private MapCellValidator _cellValidator;
        private List<IntVec3> _validEmissionCells;
        private Mesh _spawnAreaMesh;
        private bool _isSystemActive;
        private float _simulationSpeed;
        private int _mapID;
        private bool _allColumnsValidated;

        public List<IntVec3> ValidEmissionCells => _validEmissionCells;
        private static bool DrawMeshNow => MeshOverlayDrawer.DrawFireflySpawnMesh;
        
        public MapComponent_NightlySpawning(Map map) : base(map)
        {
            LongEventHandler.ExecuteWhenFinished(InitializeMapSystems);
        }

        public override void FinalizeInit()
        {
            if (!map.Biome.HasModExtension<NightlySpawningExtension>()) return;
            _ext = map.Biome.GetModExtension<NightlySpawningExtension>();
            _biomeEmissionRateFactor = _ext.biomeEmissionRate > 0f ? _ext.biomeEmissionRate : 1f;
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
            bool isClearWeather = map.weatherManager.curWeather == WeatherDefOf.Clear;
            bool isActive = StateHandler.IsActiveBelowSunGlowThreshold(map) && isClearWeather;
            
            switch (isActive)
            {
                case true when !ParticlesSpawned:
                    ActivateParticleSystem();
                    break;
                case false when ParticlesSpawned:
                    DeactivateParticleSystem();
                    break;
            }
            
            if (!isActive) return;
            if (_allColumnsValidated) return;
            _allColumnsValidated = _meshManager.ValidateCells();
            
            if (!_allColumnsValidated) return;
            _meshManager.ConstructMesh(_spawnAreaMesh);
            _validEmissionCells = _meshManager.FinalValidCells;
            ParticleSystem.ShapeModule shapeModule = _particleSystem.shape;
            shapeModule.mesh = _spawnAreaMesh;
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
            if (_particleSystem != null) return;
            if (_ext != null)
            {
                _mapID = map.GetHashCode();
                _spawnAreaMesh = new Mesh();
                _cellValidator = new MapCellValidator(map);
                _meshManager = new MeshManager(map, _cellValidator.IsCellValidForParticleEmissionMesh);
                _particleSystem = Builder.CreateFireflyParticleSystem(_mapID);
                _particleSystem.transform.position = Vector3.zero;

                ColorManager.GetBaseColorGradient(_particleSystem);
                ColorManager.SetParticleAlpha(_particleSystem, ParticleAlpha);
                StateHandler.RestoreParticleSystemState(_particleSystem, _isSystemActive, _simulationSpeed);
            }
            else
            {
                List<string> allowedBiomes = [];
                foreach (BiomeDef biomeDef in DefDatabase<BiomeDef>.AllDefsListForReading)
                {
                    if (biomeDef.HasModExtension<NightlySpawningExtension>())
                    {
                        allowedBiomes.Add(biomeDef.defName);
                    }
                }

                string message = allowedBiomes.Count > 0
                    ? $"Supported biomes for firefly spawning: {string.Join(", ", allowedBiomes)}"
                    : "No biomes support firefly spawning.";
                FFLog.Message(message);
            }
        }

        private void ActivateParticleSystem()
        {
            StateHandler.SetParticleSystemState(_particleSystem, true);
            ParticlesSpawned = true;
            _isSystemActive = true;
        }

        private void DeactivateParticleSystem()
        {
            StateHandler.SetParticleSystemState(_particleSystem, false);
            ParticlesSpawned = false;
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
            
            int validCellsCount = _validEmissionCells.Count;
            float rawEmissionRate = validCellsCount * Mathf.Pow(EmissionRateBase, EmissionRatePower);
            float biomeAdjustedEmissionRate = rawEmissionRate * _biomeEmissionRateFactor;
            float finalEmissionRate = Mathf.Max(4, Mathf.FloorToInt(biomeAdjustedEmissionRate));
            
            FFLog.Message($"Raw Emission Rate (before biome factor): {rawEmissionRate}");
            FFLog.Message($"Biome-Adjusted Emission Rate: {biomeAdjustedEmissionRate}");
            FFLog.Message($"Final Emission Rate (after applying min cap): {finalEmissionRate}");
            
            emission.rateOverTime = finalEmissionRate;
        }

        private void UpdateSimulationSpeed()
        {
            ParticleSystem.MainModule main = _particleSystem.main;
            main.simulationSpeed = (float)Find.TickManager.CurTimeSpeed * 1f;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ParticlesSpawned, "particlesSpawned", defaultValue: false);
            Scribe_Values.Look(ref _isSystemActive, "isSystemActive", defaultValue: false);
            Scribe_Values.Look(ref _simulationSpeed, "simulationSpeed", defaultValue: 1f);
        }
    }
}