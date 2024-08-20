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
        
        public MapComponent_NightlySpawning(Map map) : base(map)
        {
            // Run initialization via a LongEventHandler to combat intermittent crashing
            LongEventHandler.ExecuteWhenFinished(InitializeMapSystems);
        }

        public override void MapRemoved()
        {
            base.MapRemoved();
            LongEventHandler.ExecuteWhenFinished(DestroyParticleSystem);
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            
            if (_particleSystem == null) return;
            if (IsActive)
            {
                if (!_particlesSpawned)
                {
                    SetParticleSystemState(true);
                }
            }
            else
            {
                if (!_particlesSpawned) return;
                SetParticleSystemState(false);
                RecalculateMesh();
                ColorManager.RecalculateBaseColorGradient(_particleSystem);
            }
        }

        public override void MapComponentUpdate()
        {
            if (_particleSystem != null)
            {
                ParticleSystem.MainModule main = _particleSystem.main;
                main.simulationSpeed = (float)Find.TickManager.CurTimeSpeed * 1f;
            }
            //DEBUG_DrawSpawnableArea();
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
                RestoreParticleSystemState();
            }
            else
            {
                string allowedBiomeNames = string.Join(", ", FFDefOf.FF_Config.allowedBiomes.ConvertAll(biome => biome.defName));
                FFLog.Message($"The {map.Biome.defName} biome does not support firefly spawning. Supported biomes are: {allowedBiomeNames}");
            }
        }

        private void DestroyParticleSystem()
        {
            if (_particleSystem == null) return;
            //string system = _particleSystem.gameObject.name;
            Object.Destroy(_particleSystem.gameObject);
            //FFLog.Message($"{system} destroyed.");
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

        private void RestoreParticleSystemState()
        {
            if (_particleSystem == null) return;
            _particleSystem.gameObject.SetActive(_isSystemActive);

            if (_isSystemActive)
            {
                _particleSystem.Play();
            }
            else
            {
                _particleSystem.Stop();
            }
            ParticleSystem.MainModule mainModule = _particleSystem.main;
            mainModule.simulationSpeed = _simulationSpeed;
        }

        private void SetParticleSystemState(bool isActive)
        {
            if (_particleSystem == null) return;
            if (isActive)
            {
                _particleSystem.gameObject.SetActive(true);
                _particleSystem.Play();
            }
            else
            {
                _particleSystem.gameObject.SetActive(false);
                _particleSystem.Stop();
            }
            _particlesSpawned = isActive;
            _isSystemActive = isActive;
        }

        private void DEBUG_DrawSpawnableArea()
        {
            if (_validEmissionCells.Count > 0)
            {
                GenDraw.DrawFieldEdges(_validEmissionCells, Color.white);
            }
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