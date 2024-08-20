using System.Collections.Generic;
using LudeonTK;
using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public class MeshOverlayDrawer
    {
        [TweakValue("Graphics", 0f, 100f)]
        public static bool DrawFireflySpawnMesh;
        
        public static void DrawMeshArea(List<IntVec3> cells)
        {
            if (cells.Count > 0)
            {
                GenDraw.DrawFieldEdges(cells, Color.white);
            }
        }
    }
}