using System;
using ConwaysGameOfLife.Source.InstanceIndirect;
using ConwaysGameOfLife.Source.RenderMeshInstanced;
using ConwaysGameOfLife.Source.RenderMeshInstancedJobs;
using UnityEngine;

namespace ConwaysGameOfLife.Source.Infrastructure
{
    public static class GameFactory
    {
        public enum RenderType
        {
            Instancing = 0,
            InstancingJobs = 1,
            IndirectJobs = 2,
        }
        
        public enum AlgorithmProcessing
        {
            Simple = 0,
            Jobs = 1,
            Compute = 2,
        }


        public static GameOfLife CreateGameOfLife(GameConfiguration gameConfiguration,
            AlgorithmProcessing algorithmProcessing)
        {
            Grid grid = new Grid(gameConfiguration.GridWidth, gameConfiguration.GridHeight, gameConfiguration.CellSize,
                Vector2.zero);
            grid.Initialize();
            
            switch (algorithmProcessing)
            {
                case AlgorithmProcessing.Simple:
                    return new GameOfLife(grid, new SimpleGenerationProcessor(grid));
                case AlgorithmProcessing.Jobs:
                    return new GameOfLife(grid, new JobsGenerationProcessor(grid));
                case AlgorithmProcessing.Compute:
                    Debug.LogWarning("Compute generation is not supported yet");
                    ComputeShader shader = Resources.Load<ComputeShader>("GameCompute");
                    ComputeGenerationProcessor processor = new ComputeGenerationProcessor(shader, grid);
                    processor.Initialize();
                    return new GameOfLife(grid, processor);
                default:
                    throw new ArgumentOutOfRangeException(nameof(algorithmProcessing), algorithmProcessing, null);
            }
        }

        public static IGameRenderer CreateGameRenderer(GameOfLife gameOfLife, GameConfiguration gameConfiguration,
            RenderType renderType)
        {
            InstancingTileRenderer instancingTileRenderer = new InstancingTileRenderer(gameConfiguration.CellMaterial);
            instancingTileRenderer.Initialize();
            switch (renderType)
            {
                case RenderType.Instancing:
                    return new InstancingGameRender(gameOfLife, instancingTileRenderer);
                case RenderType.InstancingJobs:
                    return new InstancingWithJobsGameRender(gameConfiguration, gameOfLife, instancingTileRenderer);
                case RenderType.IndirectJobs:
                    InstancingIndirectTileRenderer indirectTileRenderer =
                        new InstancingIndirectTileRenderer(gameConfiguration);
                    indirectTileRenderer.Initialize();
                    return new InstancingIndirectGameRenderer(gameConfiguration, gameOfLife, indirectTileRenderer);
                default:
                    throw new ArgumentOutOfRangeException(nameof(renderType), renderType, null);
            }
            
          
        }
    }
}