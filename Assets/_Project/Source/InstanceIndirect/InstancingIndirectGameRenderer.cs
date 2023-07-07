using ConwaysGameOfLife.Source.Infrastructure;
using ConwaysGameOfLife.Source.RenderMeshInstancedJobs;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ConwaysGameOfLife.Source.InstanceIndirect
{
    public class InstancingIndirectGameRenderer : IGameRenderer
    {
        private readonly InstancingIndirectTileRenderer _tileRenderer;
        private readonly GameOfLife _gameOfLife;
        private readonly GameConfiguration _gameConfiguration;

        public InstancingIndirectGameRenderer(GameConfiguration gameConfiguration, GameOfLife gameOfLife,
            InstancingIndirectTileRenderer tileRenderer)
        {
            _gameConfiguration = gameConfiguration;
            _gameOfLife = gameOfLife;
            _tileRenderer = tileRenderer;
        }

        public void Render()
        {
            NativeQueue<Matrix4x4> renderData =
                new NativeQueue<Matrix4x4>(Allocator.TempJob);

            RenderJobInstancing renderJobInstancing = new RenderJobInstancing()
            {
                Cells = _gameOfLife.Grid.Cells,
                CellSize = _gameConfiguration.CellSize,
                RenderData = renderData.AsParallelWriter()
            };

            JobHandle jobHandle = renderJobInstancing.Schedule(_gameOfLife.Grid.Cells.Length, 64);
            jobHandle.Complete();

            NativeArray<Matrix4x4> renderDataArray = renderData.ToArray(Allocator.TempJob);
            _tileRenderer.Render(renderDataArray);

            renderData.Dispose();
            renderDataArray.Dispose();
        }

        public void Dispose()
        {
            _tileRenderer.Dispose();
        }
    }
}