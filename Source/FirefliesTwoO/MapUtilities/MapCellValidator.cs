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

        /// Determines if a specific <see cref="Map"/> cell is valid for particle emission in a mesh,
        /// based on several environmental and positional criteria.
        /// <param name="position">
        /// The positional coordinates in the <see cref="Map"/> (in 3D) to validate for particle emission.
        /// </param>
        /// <returns>
        /// Returns true if the cell meets all conditions for particle emission, otherwise false.
        /// </returns>
        public bool IsCellValidForParticleEmissionMesh(Vector3 position)
        {
            IntVec3 intVecPosition = position.ToIntVec3();                           // ~0.1 ms
            return Rand.Value <= 0.33f &&                                            // ~0.1 ms
                   !intVecPosition.InNoZoneEdgeArea(_map) &&                         // ~0.1 ms
                   !intVecPosition.IsPolluted(_map) &&                               // ~0.1 ms
                   !intVecPosition.Roofed(_map) &&                                   // ~0.2 ms
                   intVecPosition.Standable(_map) &&                                 // ~0.2 ms
                   !intVecPosition.Fogged(_map) &&                                   // ~0.5 ms
                   _map.terrainGrid.TerrainAt(intVecPosition).IsSoil &&              // ~0.5 ms
                   _map.glowGrid.GroundGlowAt(intVecPosition) < GroundGlowThreshold; // ~0.5 ms
        }
    }
}