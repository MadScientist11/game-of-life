using System;
using ConwaysGameOfLife.Source.Infrastructure;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace ConwaysGameOfLife.Source.RenderMeshInstancedJobs
{
    [BurstCompile]
    struct NewGenerationJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Cell> Cells;
        [WriteOnly]
        public NativeArray<Cell> NextGenCells;

        public int Width;
        public int Height;


        public void Execute(int index)
        {
            Cell cell = Cells[index];
            NativeArray<int> index2D = To2DIndex(index);

            cell.Populated = Convert.ToInt32(AreRulesForCellSurvivalSatisfied(index2D[0], index2D[1]));


            NextGenCells[index] = cell;
        }

        private int To1DIndex(int x, int y)
        {
            return x * Width + y;
        }

        private NativeArray<int> To2DIndex(int i)
        {
            NativeArray<int> index = new NativeArray<int>(2, Allocator.Temp);
            index[0] = i / Width;
            index[1] = i % Width;
            return index;
        }

        private NativeList<Cell> GetNeighbours(int x, int y)
        {
            NativeArray<int> dx = new NativeArray<int>(8, Allocator.Temp)
            {
                [0] = -1,
                [1] = -1,
                [2] = -1,
                [3] = 0,
                [4] = 0,
                [5] = 1,
                [6] = 1,
                [7] = 1,
            };
            NativeArray<int> dy = new NativeArray<int>(8, Allocator.Temp)
            {
                [0] = -1,
                [1] = 0,
                [2] = 1,
                [3] = -1,
                [4] = 1,
                [5] = -1,
                [6] = 0,
                [7] = 1,
            };
         
            NativeList<Cell> neighbours = new NativeList<Cell>(8,Allocator.Temp);

            for (int i = 0; i < dx.Length; i++)
            {
                int newRow = x + dx[i];
                int newCol = y + dy[i];

                if (newRow >= 0 && newRow < Height && newCol >= 0 && newCol < Width)
                    neighbours.Add(Cells[To1DIndex(newRow, newCol)]);
            }

            return neighbours;
        }


        private bool AreRulesForCellSurvivalSatisfied(int x, int y)
        {
            int isCellPopulated = Cells[To1DIndex(x, y)].Populated;
            NativeList<Cell> neighbours = GetNeighbours(x, y);

            int aliveNeighboursCount = 0;
            int neighboursCount = neighbours.Length;
            for (var i = 0; i < neighboursCount; i++)
            {
                if (neighbours[i].Populated == 1)
                    aliveNeighboursCount++;
            }


            return AliveCellWithTwoOrThreeSurvivalsLives(isCellPopulated, aliveNeighboursCount) ||
                   AnyDeadCellWithThreeSurvivalsBecomesAlive(isCellPopulated, aliveNeighboursCount);
        }

        private bool AliveCellWithTwoOrThreeSurvivalsLives(int isCellPopulated, int aliveNeighboursCount)
        {
            return isCellPopulated == 1 && (aliveNeighboursCount == 2 || aliveNeighboursCount == 3);
        }

        private bool AnyDeadCellWithThreeSurvivalsBecomesAlive(int isCellPopulated, int aliveNeighboursCount)
        {
            return isCellPopulated == 0 && aliveNeighboursCount == 3;
        }
    }
}