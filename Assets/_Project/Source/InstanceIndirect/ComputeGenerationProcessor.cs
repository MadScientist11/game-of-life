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
        private readonly GameConfiguration _gameConfiguration;
        private readonly int[] _dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
        private readonly int[] _dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

        private int _computeKernel;

        private int[] _populationArray;

        public ComputeGenerationProcessor(GameConfiguration gameConfiguration, ComputeShader generationCs, Grid grid)
        {
            _gameConfiguration = gameConfiguration;
            _generationCS = generationCs;
            _grid = grid;
        }


        public void Initialize()
        {
            _computeKernel = _generationCS.FindKernel("ComputeGeneration");
            _generationCS.SetInt("gridWidth", _gameConfiguration.GridWidth);
            _generationCS.SetInt("gridHeight", _gameConfiguration.GridHeight);
            _generationCS.SetInts("dX", _dx);
            _generationCS.SetInts("dY", _dy);
            _populationArray = new int[_grid.Cells.Length];
        }

        public void RunGeneration()
        {
            NativeArray<int> cellPopulation = new NativeArray<int>(_grid.Cells.Length, Allocator.Temp,
                NativeArrayOptions.UninitializedMemory);
            for (var i = 0; i < _grid.Cells.Length; i++)
            {
                cellPopulation[i] = Convert.ToInt32(_grid.Cells[i].Populated);
            }

            ComputeBuffer gamePopulation =
                new ComputeBuffer(_gameConfiguration.GridWidth * _gameConfiguration.GridHeight, sizeof(int));
            gamePopulation.SetData(cellPopulation);
            _generationCS.SetBuffer(_computeKernel, "gamePopulation", gamePopulation);
            ComputeBuffer newGamePopulation =
                new ComputeBuffer(_gameConfiguration.GridWidth * _gameConfiguration.GridHeight, sizeof(int));
            _generationCS.SetBuffer(_computeKernel, "newGamePopulation", gamePopulation);
            DispatchShader();
            newGamePopulation.GetData(_populationArray);
            
            for (var i = 0; i < _grid.Cells.Length; i++)
            {
                Cell cell = new Cell(_grid.Cells[i].Box);
                cell.Populated = Convert.ToBoolean(_populationArray[i]);
                _grid.Cells[i] = cell;
            }
            
            gamePopulation.Dispose();
            newGamePopulation.Dispose();
        }

        private void DispatchShader()
        {
            _generationCS.GetKernelThreadGroupSizes(_computeKernel, out uint x, out uint y, out _);
            int threadGroupsX = Mathf.CeilToInt(_gameConfiguration.GridWidth / x);
            int threadGroupsY = Mathf.CeilToInt(_gameConfiguration.GridHeight / y);
            _generationCS.Dispatch(_computeKernel, threadGroupsX, threadGroupsY, 1);
        }
    }
}