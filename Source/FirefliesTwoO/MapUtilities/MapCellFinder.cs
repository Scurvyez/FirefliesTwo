using Verse;

namespace FirefliesTwoO
{
    public static class MapCellFinder
    {
        public static bool TryFindRandomEmissionCell(IntVec3 root, MapComponent_NightlySpawning mapComp, out IntVec3 result)
        {
            if (mapComp == null || mapComp.ValidEmissionCells.NullOrEmpty())
            {
                result = root;
                return false;
            }
            result = mapComp.ValidEmissionCells.RandomElement();
            return true;
        }
    }
}