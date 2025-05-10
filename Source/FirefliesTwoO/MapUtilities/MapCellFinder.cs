using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace FirefliesTwoO
{
    public static class MapCellFinder
    {
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