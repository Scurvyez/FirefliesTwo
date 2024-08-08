using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public class DebugDrawer
    {
        public List<LineRenderer> _lineRenderers;
        
        private readonly Vector3 _spawnableArea;

        public DebugDrawer(Vector3 spawnableArea)
        {
            _lineRenderers = [];
            _spawnableArea = spawnableArea;
        }

        public void CreateLineRenderer(int index)
        {
            LineRenderer lineRenderer = new GameObject($"DEBUG_FFBoxRenderer_{index}").AddComponent<LineRenderer>();
            lineRenderer.positionCount = 5;
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(ShaderDatabase.TransparentPostLight);
            lineRenderer.material.SetColor(Shader.PropertyToID("_Color"), FFLog.MessageMsgCol);
            _lineRenderers.Add(lineRenderer);
        }

        public void DrawBoundingBoxes(List<FFSystem> ffSystems)
        {
            for (int i = 0; i < ffSystems.Count; i++)
            {
                FFSystem ffSystem = ffSystems[i];
                LineRenderer lineRenderer = _lineRenderers[i];

                Vector3 halfExtent = _spawnableArea / 2f;
                Vector3[] corners = new Vector3[5];
                corners[0] = ffSystem.Fireflies.transform.position + new Vector3(-halfExtent.x, 0, -halfExtent.z);
                corners[1] = ffSystem.Fireflies.transform.position + new Vector3(halfExtent.x, 0, -halfExtent.z);
                corners[2] = ffSystem.Fireflies.transform.position + new Vector3(halfExtent.x, 0, halfExtent.z);
                corners[3] = ffSystem.Fireflies.transform.position + new Vector3(-halfExtent.x, 0, halfExtent.z);
                corners[4] = corners[0];

                lineRenderer.SetPositions(corners);
            }
        }
    }
}