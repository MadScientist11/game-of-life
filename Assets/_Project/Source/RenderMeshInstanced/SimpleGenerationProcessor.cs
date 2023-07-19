using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ConwaysGameOfLife.Source.Infrastructure;
using UnityEngine;
using Grid = ConwaysGameOfLife.Source.Infrastructure.Grid;

namespace ConwaysGameOfLife.Source.RenderMeshInstanced
{
    public class SimpleGenerationProcessor : IGenerationProcessor
    {
        private readonly Infrastructure.Grid _grid;
        private readonly List<Vector2Int> _nextGenDeadCells = new();
        private readonly List<Vector2Int> _nextGenAliveCells = new();

        public SimpleGenerationProcessor(Grid grid)
        {
            _grid = grid;
        }

        public void RunGeneration()
        {
            _nextGenDeadCells.Clear();
            _nextGenAliveCells.Clear();

            for (int x = 0; x < _grid.Width; x++)
            {
                for (int y = 0; y < _grid.Height; y++)
                {
                    if (AreRulesForCellSurvivalSatisfied(x, y))
                    {
                        _nextGenAliveCells.Add(new Vector2Int(x, y));
                    }
                    else
                    {
                        _nextGenDeadCells.Add(new Vector2Int(x, y));
                    }
                }
            }

            for (var i = 0; i < _nextGenAliveCells.Count; i++)
            {
                _grid.PopulateCell(_nextGenAliveCells[i].x, _nextGenAliveCells[i].y);
            }

            for (var i = 0; i < _nextGenDeadCells.Count; i++)
            {
                _grid.KillCell(_nextGenDeadCells[i].x, _nextGenDeadCells[i].y);
            }
        }


        private bool AreRulesForCellSurvivalSatisfied(int x, int y)
        {
            int isCellPopulated = _grid[x, y].Populated;
            List<Cell> neighbours = _grid.GetNeighbours(x, y);

            int aliveNeighboursCount = 0;
            int neighboursCount = neighbours.Count;
            for (var i = 0; i < neighboursCount; i++)
            {
                if (neighbours[i].Populated == 1)
                    aliveNeighboursCount++;
            }


            return AliveCellWithTwoOrThreeSurvivalsLives(isCellPopulated, aliveNeighboursCount) ||
                   AnyDeadCellWithThreeSurvivalsBecomesAlive(isCellPopulated, aliveNeighboursCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool AliveCellWithTwoOrThreeSurvivalsLives(int isCellPopulated, int aliveNeighboursCount)
        {
            return isCellPopulated == 1 && (aliveNeighboursCount == 2 || aliveNeighboursCount == 3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool AnyDeadCellWithThreeSurvivalsBecomesAlive(int isCellPopulated, int aliveNeighboursCount)
        {
            return isCellPopulated == 0 && aliveNeighboursCount == 3;
        }
    }
}