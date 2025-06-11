using System.Collections.Generic;
using Verse;

namespace FirefliesTwoO
{
    public class CompProperties_DrawAdditionalGraphicsAdjustable : CompProperties
    {
        public List<GraphicData> graphics;
        
        public CompProperties_DrawAdditionalGraphicsAdjustable()
        {
            compClass = typeof(CompDrawAdditionalGraphicsAdjustable);
        }
    }
}