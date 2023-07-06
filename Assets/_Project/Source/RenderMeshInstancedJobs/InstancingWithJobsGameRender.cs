using System.Collections.Generic;
using ConwaysGameOfLife.Source.Infrastructure;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ConwaysGameOfLife.Source.RenderMeshInstancedJobs
{
    public class InstancingWithJobsGameRender : IGameRenderer
    {
        private readonly GameConfiguration _gameConfiguration;
        private readonly GameOfLife _gameOfLife;
        private readonly InstancingTileRenderer _tileRenderer;
        private readonly List<Matrix4x4> _renderTilesData = new();

        public InstancingWithJobsGameRender(GameConfiguration gameConfiguration, GameOfLife gameOfLife, InstancingTileRenderer tileRenderer)
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
    }
}