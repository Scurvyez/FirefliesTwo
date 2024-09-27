using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public class MapCellValidator
    {
        private readonly Map _map;
        private const float GroundGlowThreshold = 0.5f;

        public MapCellValidator(Map map)
        {
            _map = map;
        }
        
        public bool IsCellValidForParticleEmissionMesh(Vector3 position)
        {
            IntVec3 intVecPosition = position.ToIntVec3();
            return Rand.Value <= 0.33f && 
                   !intVecPosition.InNoZoneEdgeArea(_map) &&
                   !intVecPosition.Fogged(_map) &&
                   !intVecPosition.Roofed(_map) &&
                   _map.terrainGrid.TerrainAt(intVecPosition).IsSoil &&
                   intVecPosition.Standable(_map) &&
                   !intVecPosition.IsPolluted(_map) &&
                   _map.glowGrid.GroundGlowAt(intVecPosition) < GroundGlowThreshold;
        }
    }
}