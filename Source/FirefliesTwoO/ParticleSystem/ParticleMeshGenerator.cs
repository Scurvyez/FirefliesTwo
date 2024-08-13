using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public class ParticleMeshGenerator
    {
        private readonly Map _map;
        private readonly Func<Vector3, bool> _isPositionValid;
        private readonly List<IntVec3> _validCells;

        public ParticleMeshGenerator(Map map, Func<Vector3, bool> isPositionValid)
        {
            _map = map;
            _isPositionValid = isPositionValid;
            _validCells = [];
        }

        public Mesh CreateMeshFromValidCells()
        {
            _validCells.Clear();
            Mesh mesh = new ();
            List<Vector3> vertices = [];
            List<int> triangles = [];

            int vertexIndex = 0;

            for (int x = 0; x < _map.Size.x; x++)
            {
                for (int z = 0; z < _map.Size.z; z++)
                {
                    Vector3 cellPosition = new (x, 0, z);
                    Vector3 worldPosition = cellPosition + new Vector3(0, AltitudeLayer.VisEffects.AltitudeFor(), 0);

                    if (!_isPositionValid(worldPosition)) continue;

                    // Add cell to valid cells list
                    _validCells.Add(new IntVec3(x, 0, z));

                    // Add vertices for the cell
                    vertices.Add(worldPosition + new Vector3(-0.5f, 0, -0.5f)); // Bottom-left
                    vertices.Add(worldPosition + new Vector3(0.5f, 0, -0.5f));  // Bottom-right
                    vertices.Add(worldPosition + new Vector3(0.5f, 0, 0.5f));   // Top-right
                    vertices.Add(worldPosition + new Vector3(-0.5f, 0, 0.5f));  // Top-left

                    // Create two triangles per cell
                    triangles.Add(vertexIndex + 0);
                    triangles.Add(vertexIndex + 1);
                    triangles.Add(vertexIndex + 2);

                    triangles.Add(vertexIndex + 0);
                    triangles.Add(vertexIndex + 2);
                    triangles.Add(vertexIndex + 3);

                    vertexIndex += 4;
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();

            return mesh;
        }

        public List<IntVec3> GetValidCells()
        {
            return _validCells;
        }
    }
}
