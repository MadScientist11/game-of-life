using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace ConwaysGameOfLife.Source.Infrastructure
{
    public class InstancingTileRenderer 
    {
        private readonly RenderParams _renderParams;
        private Mesh _tileMesh;

        public InstancingTileRenderer(Material tileMaterial)
        {
            _renderParams = new RenderParams(tileMaterial);
        }

        public void Initialize()
        {
            _tileMesh = new Mesh();
            _tileMesh.vertices = new[]
            {
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(1, 1, 0),
                new Vector3(0, 1, 0)
            };
            _tileMesh.triangles = new int[]
            {
                2, 1, 0,
                3, 2, 0
            };
            _tileMesh.uv = new[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1)
            };
            _tileMesh.RecalculateNormals();
        }

        public void Render(List<Matrix4x4> tiles)
        {
            if (tiles.Count > 0)
                Graphics.RenderMeshInstanced(_renderParams, _tileMesh, 0, tiles);
        }
        
        public void Render(NativeArray<Matrix4x4> tiles)
        {
            if (tiles.Length > 0)
                Graphics.RenderMeshInstanced(_renderParams, _tileMesh, 0, tiles);
        }
    }
}