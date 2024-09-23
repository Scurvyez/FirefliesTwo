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
        private int _currentColumn;
        private int _totalColumns;

        private readonly Func<Vector3, bool> _isPositionValid;
        private readonly List<IntVec3> _validCells;

        public List<IntVec3> FinalValidCells => _validCells;

        public MeshManager(Map map, Func<Vector3, bool> isPositionValid)
        {
            _map = map;
            _altitudeLayer = AltitudeLayer.VisEffects.AltitudeFor();
            _isPositionValid = isPositionValid;
            _validCells = [];
            _currentColumn = 0;
            _totalColumns = _map.Size.x;

            FFLog.Message("MeshManager initialized.");
        }
        
        public void Reset()
        {
            FFLog.Message("Resetting MeshManager.");
            _validCells.Clear();
            _currentColumn = 0;
        }

        public bool ValidateCells()
        {
            // Check if we've processed all columns
            if (_currentColumn >= _totalColumns)
            {
                FFLog.Message("All columns processed.");
                return true;
            }

            // Temporary list to hold valid cells for this column
            List<IntVec3> columnValidCells = [];
            for (int zCoord = 0; zCoord < _map.Size.z; zCoord++)
            {
                Vector3 cellPos = new (_currentColumn, 0, zCoord);
                Vector3 worldPos = cellPos + new Vector3(0, _altitudeLayer, 0);
                if (_isPositionValid(worldPos))
                {
                    columnValidCells.Add(new IntVec3(_currentColumn, 0, zCoord));
                }
            }

            _validCells.AddRange(columnValidCells);
            _currentColumn++;
            FFLog.Message($"Moving to next column: {_currentColumn}");
            return false; // Still validating cells
        }

        public void ConstructMesh(Mesh mesh)
        {
            FFLog.Message($"Constructing mesh. Valid cells count: {_validCells.Count}");
            List<Vector3> vertices = [];
            List<int> triangles = [];

            int vertexIndex = 0;
            foreach (IntVec3 cell in _validCells)
            {
                Vector3 worldPosition = cell.ToVector3() + new Vector3(0, _altitudeLayer, 0);

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
            FFLog.Message("Mesh construction complete.");
        }
    }
}