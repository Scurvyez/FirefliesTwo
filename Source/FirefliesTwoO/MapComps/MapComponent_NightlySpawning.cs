using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public class MapComponent_NightlySpawning : MapComponent
    {
        private ParticleSystem _particleSystem;
        private MeshGenerator _meshGenerator;
        private List<IntVec3> _validEmissionCells;
        private Mesh _spawnAreaMesh;
        private bool _particlesSpawned;
        private bool _isSystemActive;
        private float _simulationSpeed;
        private int _mapID;
        
        private static bool _drawMeshNow => MeshOverlayDrawer.DrawFireflySpawnMesh;
        
        public MapComponent_NightlySpawning(Map map) : base(map)
        {
            // Run initialization via a LongEventHandler to combat intermittent crashing
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
            if (StateHandler.IsActive(map))
            {
                if (!_particlesSpawned)
                {
                    StateHandler.SetParticleSystemState(_particleSystem, true);
                    _particlesSpawned = true;
                    _isSystemActive = true;
                }
                else
                {
                    if (map.weatherManager.curWeather == WeatherDefOf.Clear) return;
                    StateHandler.SetParticleSystemState(_particleSystem, false);
                    _particlesSpawned = false;
                    _isSystemActive = false;
                }
            }
            else
            {
                if (!_particlesSpawned) return;
                StateHandler.SetParticleSystemState(_particleSystem, false);
                _particlesSpawned = false;
                _isSystemActive = false;
                RecalculateMesh();
                ColorManager.RecalculateBaseColorGradient(_particleSystem);
            }
        }

        public override void MapComponentUpdate()
        {
            if (!map.IsPlayerHome) return;
            if (_particleSystem != null)
            {
                ParticleSystem.MainModule main = _particleSystem.main;
                main.simulationSpeed = (float)Find.TickManager.CurTimeSpeed * 1f;
            }

            if (_drawMeshNow)
            {
                MeshOverlayDrawer.DrawMeshArea(_validEmissionCells);
            }
        }
        
        private void InitializeMapSystems()
        {
            if (_particleSystem != null) return;
            if (FFDefOf.FF_Config.allowedBiomes.Contains(map.Biome))
            {
                _mapID = map.GetHashCode();
                _spawnAreaMesh = new Mesh();
                _meshGenerator = new MeshGenerator(map, IsPositionValid);
                _particleSystem = Builder.CreateFireflyParticleSystem(_mapID);
                _particleSystem.transform.position = Vector3.zero;

                ParticleSystem.MainModule main = _particleSystem.main;
                ParticleSystem.EmissionModule emission = _particleSystem.emission;
                main.maxParticles = Mathf.FloorToInt(FFDefOf.FF_Config.particlesMaxCountCurve.Evaluate(map.Size.x));
                emission.rateOverTime = Mathf.FloorToInt(FFDefOf.FF_Config.particleEmissionRateCurve.Evaluate(map.Size.x));
            
                RecalculateMesh();
                ColorManager.RecalculateBaseColorGradient(_particleSystem);
                ColorManager.AmplifyParticleShaderAlpha(_particleSystem, 2.5f);
                StateHandler.RestoreParticleSystemState(_particleSystem, _isSystemActive, _simulationSpeed);
            }
            else
            {
                string allowedBiomeNames = string.Join(", ", FFDefOf.FF_Config.allowedBiomes.ConvertAll(biome => biome.defName));
                FFLog.Message($"The {map.Biome.defName} biome does not support firefly spawning. Supported biomes are: {allowedBiomeNames}");
            }
        }

        private void RecalculateMesh()
        {
            if (_particleSystem == null) return;
            _meshGenerator.UpdateMeshFromValidCells(_spawnAreaMesh);
            _validEmissionCells = _meshGenerator.FinalValidCells;
            
            ParticleSystem.ShapeModule shapeModule = _particleSystem.shape;
            shapeModule.mesh = _spawnAreaMesh;
        }

        private bool IsPositionValid(Vector3 position)
        {
            IntVec3 intVecPosition = position.ToIntVec3();
            return intVecPosition.ToVector3().InBounds(map)
                   && map.terrainGrid.TerrainAt(intVecPosition).IsSoil
                   && !intVecPosition.Roofed(map)
                   && intVecPosition.Standable(map)
                   && !intVecPosition.IsPolluted(map);
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