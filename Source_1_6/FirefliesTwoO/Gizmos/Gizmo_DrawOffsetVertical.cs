using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public class Gizmo_DrawOffsetVertical : Gizmo
    {
        private readonly CompAdjustableDrawPos comp;
        private CompProperties_AdjustableDrawPos Props => (CompProperties_AdjustableDrawPos)comp.props;
        
        public Gizmo_DrawOffsetVertical(CompAdjustableDrawPos comp)
        {
            this.comp = comp;
        }
        
        public override float GetWidth(float maxWidth) => 140f;
        
        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect mainRect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), Height);
            Widgets.DrawWindowBackground(mainRect);
            
            Rect labelRect = mainRect.TopPartPixels(24f);
            TextAnchor oldAnchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(labelRect, "FF_VerticalOffset".Translate());
            Text.Anchor = oldAnchor;
            
            Rect upButton = new(mainRect.x + 10f, mainRect.y + 30f, 50f, 40f);
            Rect downButton = new(mainRect.x + 80f, mainRect.y + 30f, 50f, 40f);
            
            if (Widgets.ButtonText(upButton, "↑"))
                comp.OffsetZ += Props.offsetStep;
            
            if (Widgets.ButtonText(downButton, "↓"))
                comp.OffsetZ -= Props.offsetStep;
            
            return new GizmoResult(GizmoState.Clear);
        }
    }
}