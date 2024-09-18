using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public class MeshManager
    {
        private readonly Map _map;
        private float _altitudeLayer;
        private readonly Func<Vector3, bool> _isPositionValid;
        private readonly List<IntVec3> _validCells;
        private readonly System.Random _random;

        public List<IntVec3> FinalValidCells => _validCells;
        
        public MeshManager(Map map, Func<Vector3, bool> isPositionValid)
        {
            _map = map;
            _altitudeLayer = AltitudeLayer.VisEffects.AltitudeFor();
            _isPositionValid = isPositionValid;
            _validCells = [];
            _random = new System.Random();
        }

        public void UpdateMeshFromValidCells(Mesh mesh)
        {
            _validCells.Clear();
            List<Vector3> vertices = [];
            List<int> triangles = [];

            int vertexIndex = 0;

            for (int xCoord = 0; xCoord < _map.Size.x; xCoord++)
            {
                for (int zCoord = 0; zCoord < _map.Size.z; zCoord++)
                {
                    Vector3 cellPosition = new(xCoord, 0, zCoord);
                    Vector3 worldPosition = cellPosition + new Vector3(0, _altitudeLayer, 0);

                    if (!_isPositionValid(worldPosition)) continue;

                    _validCells.Add(new IntVec3(xCoord, 0, zCoord));
                }
            }

            Shuffle(_validCells);
            int removalCount = _validCells.Count / 4;
            _validCells.RemoveRange(removalCount, _validCells.Count - removalCount);

            foreach (IntVec3 cell in _validCells)
            {
                Vector3 worldPosition = cell.ToVector3() + new Vector3(0, AltitudeLayer.VisEffects.AltitudeFor(), 0);

                vertices.Add(worldPosition + new Vector3(-0.5f, 0, -0.5f)); // Bottom-left
                vertices.Add(worldPosition + new Vector3(0.5f, 0, -0.5f));  // Bottom-right
                vertices.Add(worldPosition + new Vector3(0.5f, 0, 0.5f));   // Top-right
                vertices.Add(worldPosition + new Vector3(-0.5f, 0, 0.5f));  // Top-left

                triangles.Add(vertexIndex + 0);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);

                triangles.Add(vertexIndex + 0);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 3);

                vertexIndex += 4;
            }

            mesh.Clear();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
        }

        private void Shuffle<T>(List<T> validCells)
        {
            int cellsCount = validCells.Count;
            while (cellsCount > 1)
            {
                cellsCount--;
                int randomCell = _random.Next(cellsCount + 1);
                (validCells[randomCell], validCells[cellsCount]) = (validCells[cellsCount], validCells[randomCell]);
            }
        }
    }
}