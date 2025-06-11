using UnityEngine;
using Verse;

namespace FirefliesTwoO;

public class CompDrawAdditionalGraphicsAdjustable : ThingComp
{
    private CompProperties_DrawAdditionalGraphicsAdjustable Props 
        => (CompProperties_DrawAdditionalGraphicsAdjustable)props;
    
    public override void PostDraw()
    {
        CompAdjustableDrawPos offsetComp = parent
            .GetComp<CompAdjustableDrawPos>();
        
        float offsetX = offsetComp?.OffsetX ?? 0f;
        float offsetZ = offsetComp?.OffsetZ ?? 0f;
        Vector3 offset = new(offsetX, 0f, offsetZ);
        
        foreach (GraphicData graphic in Props.graphics)
        {
            graphic.Graphic.Draw(parent.DrawPos + offset + graphic.drawOffset,
                parent.Rotation, parent);
        }
    }
}