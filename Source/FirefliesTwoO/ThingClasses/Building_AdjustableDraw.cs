using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public class Building_AdjustableDraw : Building
    {
        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            CompAdjustableDrawPos comp = GetComp<CompAdjustableDrawPos>();
            if (comp != null)
            {
                drawLoc.x += comp.OffsetX;
                drawLoc.z += comp.OffsetZ;
            }
            base.DrawAt(drawLoc, flip);
        }
    }
}