using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public class Building_AdjustableDraw : Building
    {
        private CompAdjustableDrawPos _comp;
        
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            _comp = GetComp<CompAdjustableDrawPos>();
        }
        
        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            if (_comp != null)
            {
                drawLoc.x += _comp.OffsetX;
                drawLoc.z += _comp.OffsetZ;
            }
            base.DrawAt(drawLoc, flip);
        }
        
        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            if (_comp != null)
            {
                _comp.OffsetX = 0f;
                _comp.OffsetZ = 0f;
            }
            base.DeSpawn(mode);
        }
    }
}