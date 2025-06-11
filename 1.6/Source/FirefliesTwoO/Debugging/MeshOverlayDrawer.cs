using System.Collections.Generic;
using System.Linq;
using LudeonTK;
using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public class MeshOverlayDrawer
    {
        [TweakValue("FF2.0 : Graphics", 0f, 100f)]
        public static bool DrawFireflySpawnMesh;
        [TweakValue("FF2.0 : Graphics", 0f, 100f)]
        public static bool DrawValidChaseCells;
        
        private static BoolGrid fieldGrid;
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        
        public static void DrawMeshArea(List<IntVec3> cells, Color highlightColor)
        {
            if (cells.Count > 0)
            {
                DrawMeshBounds(cells, highlightColor);
            }
        }

        private static void DrawMeshBounds(List<IntVec3> cells, 
            Color color, float? altOffset = null)
        {
            Map currentMap = Find.CurrentMap;
            Material material = CreateEdgeMaterial(color);
            InitializeFieldGrid(currentMap);
            float altitudeOffset = altOffset 
                                   ?? GetRandomOffsetForColor(color);
            MarkCellsInGrid(cells, currentMap);
            DrawEdgesForCells(cells, currentMap, material, altitudeOffset);
        }

        private static Material CreateEdgeMaterial(Color color)
        {
            MaterialRequest req = new()
            {
                shader = ShaderDatabase.TransparentPostLight,
                color = color,
                BaseTexPath = "UI/Overlays/TargetHighlight_Side"
            };
            Material material = MaterialPool.MatFrom(req);
            material.GetTexture(MainTex).wrapMode = TextureWrapMode.Clamp;
            return material;
        }

        private static void InitializeFieldGrid(Map map)
        {
            if (fieldGrid == null)
            {
                fieldGrid = new BoolGrid(map);
            }
            else
            {
                fieldGrid.ClearAndResizeTo(map);
            }
        }

        private static float GetRandomOffsetForColor(Color color)
        {
            return Rand.ValueSeeded(color.ToOpaque()
                .GetHashCode()) * (1f / 26f) / 10f;
        }

        private static void MarkCellsInGrid(List<IntVec3> cells, Map map)
        {
            foreach (IntVec3 cell in cells.Where(
                         cell => cell.InBounds(map)))
            {
                fieldGrid[cell.x, cell.z] = true;
            }
        }

        private static void DrawEdgesForCells(List<IntVec3> cells, 
            Map map, Material material, float altitudeOffset)
        {
            int mapSizeX = map.Size.x;
            int mapSizeZ = map.Size.z;

            foreach (IntVec3 cell in cells)
            {
                if (!cell.InBounds(map)) 
                    continue;
                
                bool[] edgesNeeded = new bool[4];
                edgesNeeded[0] = cell.z < mapSizeZ - 1 && !fieldGrid[cell.x, cell.z + 1]; // North
                edgesNeeded[1] = cell.x < mapSizeX - 1 && !fieldGrid[cell.x + 1, cell.z]; // East
                edgesNeeded[2] = cell.z > 0 && !fieldGrid[cell.x, cell.z - 1]; // South
                edgesNeeded[3] = cell.x > 0 && !fieldGrid[cell.x - 1, cell.z]; // West
                
                for (int direction = 0; direction < 4; direction++)
                {
                    if (!edgesNeeded[direction]) continue;
                    Vector3 drawPos = cell.ToVector3ShiftedWithAltitude(
                        AltitudeLayer.MetaOverlays) + new Vector3(0f, altitudeOffset, 0f);
                    Graphics.DrawMesh(MeshPool.plane10, drawPos, 
                        new Rot4(direction).AsQuat, material, 0);
                }
            }
        }
    }
}