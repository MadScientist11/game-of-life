using System;
using ConwaysGameOfLife.Source.RenderMeshInstanced;
using ConwaysGameOfLife.Source.RenderMeshInstancedJobs;
using UnityEngine;

namespace ConwaysGameOfLife.Source.Infrastructure
{
    public static class GameFactory
    {
        public enum GameType
        {
            RenderMeshInstanced = 0,
            RenderMeshInstancedWithJobs = 1,
        }

        public static GameOfLife CreateGameOfLife(GameConfiguration gameConfiguration, GameType gameType)
        {
            Grid grid = new Grid(gameConfiguration.GridWidth, gameConfiguration.GridHeight, gameConfiguration.CellSize,
                Vector2.zero);
            grid.Initialize();
            return gameType switch
            {
                GameType.RenderMeshInstanced => new GameOfLife(grid, new SimpleGenerationProcessor(grid)),
                GameType.RenderMeshInstancedWithJobs => new GameOfLife(grid, new JobsGenerationProcessor(grid)),
                _ => throw new ArgumentOutOfRangeException(nameof(gameType), gameType, null)
            };
        }

        public static IGameRenderer CreateGameRenderer(GameOfLife gameOfLife, GameConfiguration gameConfiguration,
            GameType gameType)
        {

            InstancingTileRenderer instancingTileRenderer = new InstancingTileRenderer(gameConfiguration.CellMaterial);
            instancingTileRenderer.Initialize();
            switch (gameType)
            {
                case GameType.RenderMeshInstanced:
              
                    return new InstancingGameRender(gameOfLife, instancingTileRenderer);
                case GameType.RenderMeshInstancedWithJobs:
           
                    return new InstancingWithJobsGameRender(gameConfiguration, gameOfLife, instancingTileRenderer);
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameType), gameType, null);
            }
        }
    }
}