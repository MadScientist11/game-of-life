using System;
using ConwaysGameOfLife.Source.Infrastructure;
using Unity.Collections;
using UnityEngine;
using Grid = ConwaysGameOfLife.Source.Infrastructure.Grid;

namespace ConwaysGameOfLife.Source.InstanceIndirect
{
    public class ComputeGenerationProcessor : IGenerationProcessor
    {
        private readonly ComputeShader _generationCS;
        private readonly Grid _grid;
        private readonly int[] _dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
        private readonly int[] _dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

        private int _computeKernel;

        private Cell[] _populationArray;

        public ComputeGenerationProcessor(ComputeShader generationCs, Grid grid)
        {
            _generationCS = generationCs;
            _grid = grid;
        }


        public void Initialize()
        {
            _computeKernel = _generationCS.FindKernel("ComputeGeneration");
            _generationCS.SetInt("gridWidth", _grid.Width);
            _generationCS.SetInt("gridHeight", _grid.Height);
            _generationCS.SetInts("dX", _dx);
            _generationCS.SetInts("dY", _dy);
            _populationArray = new Cell[_grid.Cells.Length];
        }

        public void RunGeneration()
        {
            ComputeBuffer gamePopulation =
                new ComputeBuffer(_grid.Width * _grid.Height, Cell.GetSize());
            gamePopulation.SetData(_grid.Cells);
            _generationCS.SetBuffer(_computeKernel, "gamePopulation", gamePopulation);
            ComputeBuffer newGamePopulation =
                new ComputeBuffer(_grid.Width * _grid.Height, Cell.GetSize());
            _generationCS.SetBuffer(_computeKernel, "newGamePopulation", newGamePopulation);
            DispatchShader();
            newGamePopulation.GetData(_populationArray);
            _grid.Cells.CopyFrom(_populationArray);
            gamePopulation.Dispose();
            newGamePopulation.Dispose();
        }

        private void DispatchShader()
        {
            _generationCS.GetKernelThreadGroupSizes(_computeKernel, out uint x, out uint y, out _);
            int threadGroupsX = Mathf.CeilToInt(_grid.Width / x);
            int threadGroupsY = Mathf.CeilToInt(_grid.Height / y);
            _generationCS.Dispatch(_computeKernel, threadGroupsX, threadGroupsY, 1);
        }
    }
}