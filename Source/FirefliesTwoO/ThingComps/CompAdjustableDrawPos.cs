using System.Collections.Generic;
using Verse;

namespace FirefliesTwoO
{
    public class CompAdjustableDrawPos : ThingComp
    {
        public float OffsetX = 0f;
        public float OffsetZ = 0f;
        
        private CompProperties_AdjustableDrawPos Props 
            => (CompProperties_AdjustableDrawPos)props;
        
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo g in base.CompGetGizmosExtra())
                yield return g;
            
            yield return new Gizmo_DrawOffsetHorizontal(this);
            yield return new Gizmo_DrawOffsetVertical(this);
        }
        
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref OffsetX, "OffsetX", 0f);
            Scribe_Values.Look(ref OffsetZ, "OffsetZ", 0f);
        }
    }
}