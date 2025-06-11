using Verse;

namespace FirefliesTwoO
{
    public class CompProperties_AdjustableDrawPos : CompProperties
    {
        public float offsetStep = 0.05f;
        
        public CompProperties_AdjustableDrawPos()
        {
            compClass = typeof(CompAdjustableDrawPos);
        }
    }
} 