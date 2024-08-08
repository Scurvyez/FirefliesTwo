using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    [StaticConstructorOnStartup]
    public class MapComponent_FFNightlySpawning : MapComponent
    {
        private bool _firefliesSpawned;
        
        private static readonly Color DefaultFireflyColor = new(1f, 1f, 0f, 1f);
        private static Vector3 _spawnableArea;
        private readonly List<FFSystem> _ffSystems;
        
        private bool IsActive
        {
            get
            {
                float currentHour = GenLocalDate.DayPercent(map) * 24f;
                return (FFDefOf.FF_Config.startHour <= FFDefOf.FF_Config.endHour) ?
                    (currentHour >= FFDefOf.FF_Config.startHour && currentHour < FFDefOf.FF_Config.endHour) :
                    (currentHour >= FFDefOf.FF_Config.startHour || currentHour < FFDefOf.FF_Config.endHour);
            }
        }
        
        public MapComponent_FFNightlySpawning(Map map) : base(map)
        {
            if (FFDefOf.FF_Config.allowedBiomes.Contains(map.Biome))
            {
                _spawnableArea = TryGetSpawnableArea();
                _ffSystems = [];
            
                for (int i = 0; i < FFDefOf.FF_Config.numFFSystems; i++)
                {
                    FFSystem ffSystem = CreateFFSystem();
                    TryGetEmitPosition(ffSystem);
                    _ffSystems.Add(ffSystem);
                }
            }
            else
            {
                string allowedBiomeNames = string.Join(", ", FFDefOf.FF_Config.allowedBiomes.ConvertAll(biome => biome.defName));
                FFLog.Warning($"The {map.Biome.defName} biome does not support firefly spawning. Supported biomes are: {allowedBiomeNames}");
            }
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            
            if (!FFDefOf.FF_Config.allowedBiomes.Contains(map.Biome)) return;
            if (IsActive)
            {
                if (_firefliesSpawned) return;
                foreach (FFSystem ffSystem in _ffSystems)
                {
                    ffSystem.Particles.Play();
                }
                _firefliesSpawned = true;
            }
            else
            {
                if (!_firefliesSpawned) return;
                foreach (FFSystem ffSystem in _ffSystems)
                {
                    ffSystem.Particles.Stop();
                    TryGetEmitPosition(ffSystem);
                }
                _firefliesSpawned = false;
            }
        }

        public override void MapComponentUpdate()
        {
            if (!FFDefOf.FF_Config.allowedBiomes.Contains(map.Biome)) return;
            foreach (FFSystem ffSystem in _ffSystems)
            {
                ffSystem.Speed = (float)Find.TickManager.CurTimeSpeed * FFDefOf.FF_Config.particleVelocityFactor;
            }
        }

        private Vector3 TryGetSpawnableArea()
        {
            Vector3 spawnableArea = new (
                CellRect.SingleCell(map.cellIndices.IndexToCell(0)).Area * FFDefOf.FF_Config.spawnableAreaCellFactor, 1f,
                CellRect.SingleCell(map.cellIndices.IndexToCell(0)).Area * FFDefOf.FF_Config.spawnableAreaCellFactor);
            return spawnableArea;
        }
        
        private void TryGetEmitPosition(FFSystem ffSystem)
        {
            bool found = false;

            for (int attempt = 0; attempt < 100; attempt++)
            {
                IntVec3 cellPosition = CellFinder.RandomNotEdgeCell(FFDefOf.FF_Config.minEdgeDistance, map);

                if (map.roofGrid.RoofAt(cellPosition) != null || !cellPosition.InBounds(map)) continue;
                Vector3 newPosition = cellPosition.ToVector3() + new Vector3(0f, AltitudeLayer.VisEffects.AltitudeFor(), 0f);
                ffSystem.Fireflies.transform.position = newPosition;
                found = true;
                break;
            }

            if (!found)
            {
                FFLog.Warning("Failed to find a valid position for at least one FFSystem.");
            }
        }
        
        private static FFSystem CreateFFSystem()
        {
            GameObject fireflies = new ("FFSystem");
            ParticleSystem particles = fireflies.AddComponent<ParticleSystem>();
            ParticleSystemRenderer renderer = fireflies.AddComponent<ParticleSystemRenderer>();

            ParticleBuilder.ConfigureParticleSystem(particles, FFDefOf.FF_Config.particlesMaxCount, FFDefOf.FF_Config.particleLifetime);
            ParticleBuilder.ConfigureShapeModule(particles, _spawnableArea);
            ParticleBuilder.ConfigureEmissionModule(particles, FFDefOf.FF_Config.particleEmissionRate);
            ParticleBuilder.ConfigureNoiseModule(particles);
            ParticleBuilder.ConfigureSizeOverLifetimeModule(particles, FFDefOf.FF_Config.particleSizeFactor);
            ParticleBuilder.ConfigureColorOverLifetimeModule(particles, DefaultFireflyColor);
            Material material = new (ParticleBuilder.GetParticleShader());
            ParticleBuilder.ConfigureRenderer(particles, material, Assets.Firefly);
            
            return new FFSystem
            {
                Fireflies = fireflies,
                Particles = particles,
                Renderer = renderer
            };
        }
    }
}