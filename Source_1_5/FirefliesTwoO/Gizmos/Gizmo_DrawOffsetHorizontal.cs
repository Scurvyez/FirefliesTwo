using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public class Gizmo_DrawOffsetHorizontal : Gizmo
    {
        private readonly CompAdjustableDrawPos comp;
        private CompProperties_AdjustableDrawPos Props => (CompProperties_AdjustableDrawPos)comp.props;
        
        public Gizmo_DrawOffsetHorizontal(CompAdjustableDrawPos comp)
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
            Widgets.Label(labelRect, "FF_HorizontalOffset".Translate());
            Text.Anchor = oldAnchor;
            
            Rect leftButton = new(mainRect.x + 10f, mainRect.y + 30f, 50f, 40f);
            Rect rightButton = new(mainRect.x + 80f, mainRect.y + 30f, 50f, 40f);
            
            if (Widgets.ButtonText(leftButton, "←"))
                comp.OffsetX -= Props.offsetStep;
            
            if (Widgets.ButtonText(rightButton, "→"))
                comp.OffsetX += Props.offsetStep;
            
            return new GizmoResult(GizmoState.Clear);
        }
    }
}