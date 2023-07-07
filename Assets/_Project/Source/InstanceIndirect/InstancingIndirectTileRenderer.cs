using System.Collections.Generic;
using ConwaysGameOfLife.Source.Infrastructure;
using Unity.Collections;
using UnityEngine;

namespace ConwaysGameOfLife.Source.InstanceIndirect
{
    public class InstancingIndirectTileRenderer
    {
        private RenderParams _renderParams;
        private Mesh _tileMesh;
        private const int CommandCount = 1;

        private GraphicsBuffer _commandBuffer;
        private GraphicsBuffer.IndirectDrawIndexedArgs[] _commandData;
        private readonly GameConfiguration _gameConfiguration;
        private GraphicsBuffer _transformsBuffer;
        private static readonly int Transforms = Shader.PropertyToID("Transforms");

        public InstancingIndirectTileRenderer(GameConfiguration gameConfiguration)
        {
            _gameConfiguration = gameConfiguration;
            _renderParams = new RenderParams(gameConfiguration.CellMaterialForIndirectInstancing);
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

            _renderParams.worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one);
            _renderParams.matProps = new MaterialPropertyBlock();
            _transformsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured,
                _gameConfiguration.GridWidth * _gameConfiguration.GridHeight,
                4 * 4 * sizeof(float));
            _commandData = new GraphicsBuffer.IndirectDrawIndexedArgs[CommandCount];
            _commandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, CommandCount, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        }

        public void Render(List<Matrix4x4> tiles)
        {
            if (tiles.Count > 0)
                Graphics.RenderMeshInstanced(_renderParams, _tileMesh, 0, tiles);
        }

        public void Render(NativeArray<Matrix4x4> tiles)
        {
            _transformsBuffer.SetData(tiles);
            _renderParams.matProps.SetBuffer(Transforms, _transformsBuffer);
            _commandData[0].indexCountPerInstance = _tileMesh.GetIndexCount(0);
            _commandData[0].baseVertexIndex = _tileMesh.GetBaseVertex(0);
            _commandData[0].startIndex = _tileMesh.GetIndexStart(0);
            _commandData[0].instanceCount = (uint)tiles.Length;
            _commandBuffer.SetData(_commandData);
            Graphics.RenderMeshIndirect(_renderParams, _tileMesh, _commandBuffer, CommandCount);
        }


        public void Dispose()
        {
            _transformsBuffer?.Dispose();
            _transformsBuffer = null;
            _commandBuffer?.Dispose();
            _commandBuffer = null;
        }
    }
}