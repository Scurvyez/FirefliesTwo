using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace FirefliesTwoO
{
    public static class MapCellFinder
    {
        /// <summary>
        /// Attempts to find a random valid emission cell within the specified maximum distance from the pawn's position.
        /// </summary>
        /// <param name="mapComp">
        /// The <see cref="MapComponent"/> containing the list of valid emission cells.
        /// </param>
        /// <param name="pawn">
        /// The <see cref="Pawn"/> for whom the emission cell reachability is evaluated.
        /// </param>
        /// <param name="result">
        /// Outputs the resulting emission cell if a valid cell is found. Returns <c>IntVec3.Invalid</c> if no valid cell is found.
        /// </param>
        /// <param name="maxDistance">
        /// The maximum distance from the pawn's position within which a valid cell can be found. Defaults to 50 if not specified.
        /// </param>
        /// <returns>
        /// <c>true</c> if a valid emission cell is found; otherwise, <c>false</c>.
        /// </returns>
        public static bool TryFindRandomEmissionCell(MapComponent_NightlySpawning mapComp, 
            Pawn pawn, out IntVec3 result, float? maxDistance = 50f)
        {
            result = IntVec3.Invalid;
            if (mapComp == null || mapComp.ValidEmissionCells.NullOrEmpty())
                return false;
            
            foreach (IntVec3 cell in mapComp.ValidEmissionCells.InRandomOrder())
            {
                if (pawn.Position.DistanceTo(cell) > maxDistance) continue;
                if (!cell.Standable(pawn.Map)) continue;
                if (!pawn.CanReach(cell, PathEndMode.OnCell, Danger.None)) 
                    continue;
                
                result = cell;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Retrieves a list of valid emission cells within a specified radius from a center point.
        /// </summary>
        /// <param name="mapComp">
        /// The <c>MapComponent</c> which contains the valid emission cells to check.
        /// </param>
        /// <param name="pawn">
        /// The <c>Pawn</c> whose reachability is considered when determining valid cells.
        /// </param>
        /// <param name="center">
        /// The center point from which to evaluate cells within the radius.
        /// </param>
        /// <param name="radius">
        /// The radius within which emission cells are evaluated.
        /// </param>
        /// <returns>
        /// A list of valid emission cells within the given radius that the pawn can stand on and reach.
        /// Returns an empty list if the map component is null, contains no valid emission cells, or no cells meet the criteria.
        /// </returns>
        public static List<IntVec3> TryGetValidEmissionCellsInRadius(
            MapComponent_NightlySpawning mapComp, Pawn pawn, IntVec3 center, int radius)
        {
            List<IntVec3> result = [];
            if (mapComp == null || mapComp.ValidEmissionCells.NullOrEmpty()) 
                return result;
            
            int radiusSquared = radius * radius;
            foreach (IntVec3 cell in mapComp.ValidEmissionCells)
            {
                if ((cell - center).LengthHorizontalSquared > radiusSquared)
                    continue;
                
                if (!cell.Standable(pawn.Map) 
                    || !pawn.CanReach(cell, PathEndMode.OnCell, Danger.None))
                    continue;
                
                result.Add(cell);
            }
            return result;
        }
    }
}